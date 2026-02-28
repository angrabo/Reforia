import {createContext, ReactNode, useContext, useEffect, useState} from "react";
import i18n from "i18next";

interface ConfigContextType {
    userHighlight: boolean;
    apiToken: string;
    setApiToken: (newToken: string) => void;
    setUserHighlight: (newHighlight: boolean) => void;
    alertOnMention: boolean;
    setAlertOnMention: (newAlert: boolean) => void;
    alertOnKeyword: boolean;
    setAlertOnKeyword: (newAlert: boolean) => void;
    highlightOnKeyword: boolean;
    setHighlightOnKeyword: (newHighlight: boolean) => void;
    keywordList: string[];
    setKeywordList: (newList: string[]) => void;
    showBeatmapBanner: boolean;
    setShowBeatmapBanner: (newShow: boolean) => void;
    language: string;
    setLanguage: (newLanguage: string) => void;
}

const ConfigContext = createContext<ConfigContextType | undefined>(undefined);

export const ConfigProvider = ({ children }: { children: ReactNode }) => {
    const [userHighlight, setUserHighlight] = useState<boolean>(true);
    const [apiToken, setApiToken] = useState<string>("");
    const [language, setLanguage] = useState<string>("");
    const [alertOnMention, setAlertOnMention] = useState<boolean>(true);
    const [alertOnKeyword, setAlertOnKeyword] = useState<boolean>(false);
    const [highlightOnKeyword, setHighlightOnKeyword] = useState<boolean>(false);
    const [keywordList, setKeywordList] = useState<string[]>([]);
    const [showBeatmapBanner, setShowBeatmapBanner] = useState<boolean>(true);

    useEffect(() => {
        const handleLanguageChange = (lng: string) => {
            setLanguage(lng);
        };

        i18n.on('languageChanged', handleLanguageChange);
        return () => i18n.off('languageChanged', handleLanguageChange);
    }, []);

    return (
        <ConfigContext.Provider value={{
            userHighlight, setUserHighlight,
            apiToken, setApiToken,
            language, setLanguage,
            alertOnMention, setAlertOnMention,
            alertOnKeyword, setAlertOnKeyword,
            highlightOnKeyword, setHighlightOnKeyword,
            keywordList, setKeywordList,
            showBeatmapBanner, setShowBeatmapBanner
        }}>
            {children}
        </ConfigContext.Provider>
    );
};

export const useConfigContext = () => {
    const context = useContext(ConfigContext);

    if (context === undefined) {
        throw new Error('useConfigContext must be used within a ConfigProvider');
    }

    return context;
};