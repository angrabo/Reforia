import {useEffect, useState} from "react";
import {invoke} from "@tauri-apps/api/core";
import {WebResponse} from "./models/types";
import LoginPage from "./pages/LoginPage";
import {useGlobalContext} from "./Context/GlobalContext";
import {signalRClient} from "./utils/FunctionClient";
import {useAlert} from "./providers/AlertProvider";
import MainAppPage from "./pages/MainAppPage";
import LoadingScreenPage from "./pages/LoadingScreenPage";
import {CreateIrcConnectionFunction, GetIrcCredentialsFunction, GetSettingsFunction} from "./types/types.ts";
import {useConfigContext} from "./Context/ConfigContext.tsx";
import {ErrorCode} from "./models/ErrorCode.ts";
import {check, Update} from '@tauri-apps/plugin-updater';
import {UpdateModal} from "./components/modals/UpdateModal.tsx";
import {useTranslation} from "react-i18next";
import i18n from "i18next";

type AuthState = "loading" | "login" | "app";


const App = () => {
    const [authState, setAuthState] = useState<AuthState>("loading");

    const {updateConnectionId, updateUsername} = useGlobalContext();
    const {setUserHighlight, setApiToken, setLanguage, setAlertOnKeyword, setHighlightOnKeyword, setKeywordList, setAlertOnMention, setShowBeatmapBanner} = useConfigContext();
    const {showAlert} = useAlert();
    const {t} = useTranslation();

    const [update, setUpdate] = useState<Update | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        const checkForUpdates = async () => {
            try {
                const result = await check();
                if (result) {
                    setUpdate(result);
                    setIsModalOpen(true);
                }
            } catch (error) {
                console.error("Failed to check for updates:", error);
            }
        };

        checkForUpdates();

        const setup = async () => {
            try {
                await signalRClient.start();
                await signalRClient.waitForConnection();

                const credentials = await getIrcCredentials();
                if (!credentials) {
                    setAuthState("login");
                    return;
                }

                const connectionSuccess = await createIrcConnection();
                if (!connectionSuccess) {
                    showAlert(t('alerts.notLogged'), "warning", 5000);
                    setAuthState("login");
                    return;
                }

                await getConfiguration();

                setAuthState("app");

            } catch (err) {
                console.error("Setup failed:", err);
                showAlert(t('alerts.notLogged'), "warning", 5000);
                setAuthState("login");
            } finally {
                await invoke("finish_loading");
            }
        };

        setup();
    }, []);

    const getConfiguration = async () => {
        try {
            const response: WebResponse<GetSettingsFunction> = await signalRClient.callFunction("GetSettingsFunction");
            response.body?.isUserHighlighted ? setUserHighlight(true) : setUserHighlight(false);
            response.body?.apiToken ? setApiToken(response.body.apiToken) : setApiToken("");
            response.body?.alertOnKeyword ? setAlertOnKeyword(true) : setAlertOnKeyword(false);
            response.body?.highlightOnKeyword ? setHighlightOnKeyword(true) : setHighlightOnKeyword(false);
            response.body?.keywordList ? setKeywordList(response.body.keywordList) : setKeywordList([]);
            response.body?.alertOnMention ? setAlertOnMention(true) : setAlertOnMention(false);
            response.body?.showBeatmapBanner ? setShowBeatmapBanner(true) : setShowBeatmapBanner(false);
            const langFromServer = response.body?.language;
            if (langFromServer) {
                setLanguage(langFromServer);
                i18n.changeLanguage(langFromServer);
            }
        } catch (err) {
            console.error("Config fetch failed:", err);
        }
    };

    const getIrcCredentials = async () => {
        try {
            const response: WebResponse<GetIrcCredentialsFunction> = await signalRClient.callFunction("GetIrcCredentialsFunction");
            if (response.body?.nick) {
                updateUsername(response.body.nick);
                return true;
            }
            return false;
        } catch (err) {
            return false;
        }
    };

    const createIrcConnection = async () => {
        try {
            const response: WebResponse<CreateIrcConnectionFunction> = await signalRClient.callFunction("CreateIrcConnectionFunction");

            if (response.errorCode === ErrorCode.UserNotAllowedToConnect) {
                showAlert(t('alerts.banned'), "error", 3000);
                return false;
            }

            if (response.body?.connectionId) {
                updateConnectionId(response.body.connectionId);
                return true;
            }
            return false;
        } catch (err) {
            return false;
        }
    };

    if (authState === "loading") {
        return <LoadingScreenPage/>;
    }

    if (authState === "login") {
        return <>
            <UpdateModal
                open={isModalOpen}
                update={update}
                onClose={() => setIsModalOpen(false)}
            />
            <LoginPage/>
        </>;
    }

    return <>
        <UpdateModal
            open={isModalOpen}
            update={update}
            onClose={() => setIsModalOpen(false)}
        />
        <MainAppPage/>
    </>;
};

export default App;