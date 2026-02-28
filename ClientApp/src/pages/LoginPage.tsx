import {
    Box,
    Button,
    TextField,
    Typography,
    Paper,
    Container, CircularProgress, Link
} from '@mui/material';
import {WebResponse} from "../models/types.ts";
import {signalRClient} from "../utils/FunctionClient.ts";
import {useGlobalContext} from "../Context/GlobalContext.tsx";
import {useAlert} from "../providers/AlertProvider.tsx";
import {ErrorCode} from "../models/ErrorCode.ts";
import {useState} from "react";
import MainAppPage from "./MainAppPage.tsx";
import {COLORS} from "../utils/ThemeCreator.ts";
import {Trans, useTranslation} from "react-i18next";

interface CreateIrcConnectionFunction {
    connectionId: string;
}

interface SetIrcCredentialsFunction {
    success: boolean;
}

const LoginPage = () => {
    const [localUsername, setLocalUsername] = useState("");
    const [password, setPassword] = useState("");
    const [isLogging, setIsLogging] = useState(false);

    const {updateConnectionId, username, updateUsername} = useGlobalContext();
    const {showAlert} = useAlert();
    const [shouldOpenApp, setShouldOpenApp] = useState(false);
    const { t } = useTranslation();


    const handleLogin = async () => {
        setIsLogging(true);
        try {
            await signalRClient.start();

            const response: WebResponse<SetIrcCredentialsFunction> = await signalRClient.callFunction("SetIrcCredentialsFunction", {
                Nick: localUsername,
                Password: password
            });

            if (!response.body?.success) {
                showAlert(t('alerts.unknown'), "error", 3000);
                setIsLogging(false);
                return;
            }

            const connResponse: WebResponse<CreateIrcConnectionFunction> = await signalRClient.callFunction("CreateIrcConnectionFunction");

            if (connResponse.errorCode === ErrorCode.UserNotAllowedToConnect) {
                showAlert(t('alerts.banned'), "error", 3000);
            } else if (connResponse.body?.connectionId) {
                updateConnectionId(connResponse.body.connectionId);
                updateUsername(localUsername);
                setShouldOpenApp(true);
                return;
            } else if (connResponse.errorCode === ErrorCode.CannotConnectToIrc) {
                showAlert(t('alerts.cannotConnectToIrc'), "error", 3000);
            } else {
                showAlert(t('alerts.unknown'), "error", 3000);
            }
        } catch (err) {
            showAlert(t('alerts.connectionError'), "error", 3000);
        } finally {
            setIsLogging(false);
        }
    };

    if (shouldOpenApp) {
        return <MainAppPage/>;
    }

    return (
        <Box
            sx={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: COLORS.bgDark,
            }}
        >
            <Container maxWidth="xs">
                <Paper
                    elevation={0}
                    sx={{
                        padding: 4,
                        backgroundColor: COLORS.surface1,
                        color: COLORS.textMain,
                        borderRadius: 3,
                        border: `1px solid ${COLORS.border}`,
                    }}
                >
                    <Box sx={{textAlign: 'center', mb: 3, display: 'flex', justifyContent: 'center'}}>
                        <img src="/logo.png" alt="logo" width="100"/>
                    </Box>

                    <Typography variant="body2" sx={{mb: 3, color: COLORS.textSub}}>
                        {t('login.loginMessage')}
                    </Typography>

                    <Box component="form" noValidate onSubmit={(e) => {
                        e.preventDefault();
                        handleLogin();
                    }}>
                        <TextField
                            onChange={(e) => setLocalUsername(e.target.value)}
                            margin="normal"
                            fullWidth
                            required
                            defaultValue={username}
                            label={t('login.username')}
                            variant="outlined"
                        />
                        <TextField
                            onChange={(e) => setPassword(e.target.value)}
                            margin="normal"
                            required
                            fullWidth
                            label={t('login.password')}
                            type="password"
                            variant="outlined"
                        />

                        {!isLogging ? (
                            <Box sx={{display: "flex", justifyContent: "center", mt: 3, mb: 2}}>
                                <Button
                                    type="submit"
                                    fullWidth
                                    variant="contained"
                                >
                                    {t('login.login')}
                                </Button>
                            </Box>

                        ) : (
                            <Box sx={{display: "flex", justifyContent: "center", mt: 3, mb: 2}}>
                                <CircularProgress size={32} sx={{color: COLORS.primary}}/>
                            </Box>
                        )}

                        <Typography variant="caption" sx={{ mt: 2, display: 'block', color: COLORS.textSub, textAlign: 'center' }}>
                            <Trans i18nKey="login.credentialsInfo">
                                Credentials can be obtained at:
                                <Link
                                    href="https://osu.ppy.sh/home/account/edit#legacy-api"
                                    target="_blank"
                                    rel="noreferrer"
                                    underline="hover"
                                    sx={{
                                        color: COLORS.primary,
                                        cursor: 'pointer',
                                        textDecoration: 'underline',
                                        '&:hover': { color: COLORS.osuPink }
                                    }}
                                >
                                    osu! website
                                </Link>
                            </Trans>
                        </Typography>
                    </Box>
                </Paper>
            </Container>
        </Box>
    );
};

export default LoginPage;