import {createContext, ReactNode, useContext, useEffect, useState} from "react";
import {getVersion} from "@tauri-apps/api/app";

interface GlobalContextType {
    connectionId: string;
    username: string;
    updateUsername: (newUsername: string) => void;
    updateConnectionId: (newId: string) => void;
    appVersion: string;
}

const GlobalContext = createContext<GlobalContextType | undefined>(undefined);

export const GlobalProvider = ({ children }: { children: ReactNode }) => {
    const [connectionId, setConnectionId] = useState<string>("");
    const [username, setUsername] = useState<string>("");
    const [appVersion, setAppVersion] = useState<string>("...");

    useEffect(() => {
        getVersion()
            .then(setAppVersion)
            .catch((err) => {
                console.error("Failed to fetch version:", err);
                setAppVersion("Unknown");
            });
    }, []);

    const updateConnectionId = (newId: string) => {
        setConnectionId( newId);
    };

    const updateUsername = (newUsername: string) => {
        setUsername(newUsername);
    }


    return (
        <GlobalContext.Provider value={{ connectionId, username, updateUsername, updateConnectionId, appVersion }}>
            {children}
        </GlobalContext.Provider>
    );
};

export const useGlobalContext = () => {
    const context = useContext(GlobalContext);

    if (context === undefined) {
        throw new Error('useGlobalContext must be used within a GlobalProvider');
    }

    return context;
};