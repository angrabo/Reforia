import React, { useState, useEffect } from 'react';
import { Alert, AlertColor, LinearProgress, Box, Collapse } from '@mui/material';

interface TimedAlertProps {
    open: boolean;
    onClose: () => void;
    message: string;
    severity: AlertColor;
    duration?: number;
}

export const TimedAlert: React.FC<TimedAlertProps> = ({
                                                          open,
                                                          onClose,
                                                          message,
                                                          severity,
                                                          duration = 5000,
                                                      }) => {
    const [progress, setProgress] = useState(0);
    const [isPaused, setIsPaused] = useState(false);

    useEffect(() => {
        if (!open) return;

        const step = 100;
        const timer = setInterval(() => {
            if (!isPaused) {
                setProgress((prev) => {
                    const next = prev + (step / duration) * 100;
                    if (next >= 100) {
                        clearInterval(timer);
                        onClose();
                        return 100;
                    }
                    return next;
                });
            }
        }, step);

        return () => clearInterval(timer);
    }, [open, isPaused, duration, onClose]);

    return (
        <Collapse in={open}>
            <Box
                onMouseEnter={() => setIsPaused(true)}
                onMouseLeave={() => setIsPaused(false)}
                sx={{
                    boxShadow: 3,
                    mb: 1
                }}
            >
                <Alert severity={severity} variant="filled" onClose={onClose}>
                    {message}
                </Alert>
                <LinearProgress
                    variant="determinate"
                    value={progress}
                    sx={{
                        marginTop: -0.5,
                        height: 4,
                        backgroundColor: 'rgba(255,255,255,0.3)',
                        '& .MuiLinearProgress-bar': { backgroundColor: '#fff' }
                    }}
                />
            </Box>
        </Collapse>
    );
};