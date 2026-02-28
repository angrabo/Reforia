import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { AlertColor, Box } from '@mui/material';
import { signalRClient } from "../utils/FunctionClient.ts";
import { TimedAlert } from "../components/TimedAlert.tsx";

interface AlertItem {
    id: string;
    message: string;
    severity: AlertColor;
    duration: number;
}

interface AlertContextType {
    showAlert: (message: string, severity: AlertColor, duration?: number) => void;
}

const AlertContext = createContext<AlertContextType | undefined>(undefined);

export const AlertProvider = ({ children }: { children: ReactNode }) => {
    const [alerts, setAlerts] = useState<AlertItem[]>([]);

    const showAlert = (message: string, severity: AlertColor, duration = 5000) => {
        const id = crypto.randomUUID();
        setAlerts((prev) => [...prev, { id, message, severity, duration }]);
    };

    const removeAlert = (id: string) => {
        setAlerts((prev) => prev.filter((alert) => alert.id !== id));
    };

    useEffect(() => {
        signalRClient.setOnAlertHandler((message, severity) => {
            showAlert(message, severity, severity === 'error' ? 8000 : 4000);
        });

        signalRClient.start();
    }, []);

    return (
        <AlertContext.Provider value={{ showAlert }}>
            {children}

            <Box
                sx={{
                    position: 'fixed',
                    top: 20,
                    right: 20,
                    zIndex: 2000,
                    display: 'flex',
                    flexDirection: 'column',
                    gap: 1,
                    pointerEvents: 'none'
                }}
            >
                {alerts.map((alert) => (
                    <Box key={alert.id} sx={{ pointerEvents: 'auto', position: 'relative' }}>
                        <TimedAlert
                            open={true}
                            onClose={() => removeAlert(alert.id)}
                            message={alert.message}
                            severity={alert.severity}
                            duration={alert.duration}
                        />
                    </Box>
                ))}
            </Box>
        </AlertContext.Provider>
    );
};

export const useAlert = () => {
    const context = useContext(AlertContext);
    if (!context) throw new Error("useAlert must be used within AlertProvider");
    return context;
};