import { createTheme } from '@mui/material/styles';
import {SxProps, Theme} from "@mui/material";

export const COLORS = {
    bgDark: '#121212',
    surface1: '#1e1e1e',
    surface2: '#2d2d2d',
    primary: '#8ab4f8',
    secondary: '#346cd2',
    border: '#3c4043',
    textMain: '#e8eaed',
    textSub: '#9aa0a6',
    osuPink: '#ff8bcb',
    osuPinkGray: '#c75aa0',
    osuPinkGrayLight: '#e059b0',
    success: '#81c995',
    playerColor: `#ffffff`,
    refereeColor: `#8ab4f8`,
    chatMessage: `#d5d5d5`,
    botMessage: `#ecdbfa`,
    refereeMessage: `#dbf5fa`,
    highlight: `#ff008c`,
};

export const theme = createTheme({
    palette: {
        mode: 'dark',
        primary: { main: COLORS.primary },
        secondary: { main: COLORS.surface2, contrastText: COLORS.textMain },
        background: { default: COLORS.bgDark, paper: COLORS.surface1 },
        text: { primary: COLORS.textMain, secondary: COLORS.textSub },
        divider: COLORS.border,
    },
    components: {
        MuiCssBaseline: {
            styleOverrides: {
                body: {
                    backgroundColor: COLORS.bgDark,
                    color: COLORS.textMain,
                    scrollbarWidth: 'thin',
                    '&::-webkit-scrollbar': { width: '8px' },
                    '&::-webkit-scrollbar-thumb': {
                        backgroundColor: COLORS.border,
                        borderRadius: '10px',
                    },
                },
            },
        },
        MuiAlert: {
            styleOverrides: {
                root: {
                    color: "#fff",
                },
            }

        },
        MuiTypography: {
            styleOverrides: {
                root: {
                    padding: '0 2px',
                }
            }
        },
        MuiSwitch: {
            styleOverrides: {
                root: {
                    '& .MuiSwitch-switchBase': {
                        color: COLORS.osuPink,
                        '&.Mui-checked': {
                            color: COLORS.osuPink,
                            '& + .MuiSwitch-track': {
                                backgroundColor: COLORS.osuPink,
                            },
                        },
                    }
                }
            }
        },
        MuiTextField: {
            defaultProps: {
                variant: 'outlined',
                size: 'small',
            },
            styleOverrides: {
                root: {
                    '& .MuiOutlinedInput-root': {
                        padding: '8px 10px',
                        backgroundColor: '#121212',
                        borderRadius: '8px',
                        color: COLORS.textMain,

                        '& fieldset': {
                            borderColor: COLORS.border,
                            transition: 'all 0.2s ease-in-out',
                        },
                        '&:hover fieldset': {
                            borderColor: COLORS.textSub,
                        },
                        '&.Mui-focused fieldset': {
                            borderColor: COLORS.osuPink,
                            borderWidth: '1px',
                        },
                    },
                    '& .MuiInputLabel-root': {
                        padding: '8px 10px',
                        color: COLORS.textSub,
                        '&.Mui-focused': {
                            color: COLORS.osuPink,
                        },
                    },
                    '& .MuiInputBase-input': {
                        '&::placeholder': {
                            color: COLORS.textSub,
                            opacity: 0.6,
                        },
                    },
                },
            },
        },
        MuiButton: {
            defaultProps: {
                disableElevation: true,
                variant: 'contained',
            },
            styleOverrides: {
                root: {
                    borderRadius: '8px',
                    textTransform: 'none',
                    backgroundColor: COLORS.surface2,
                    color: COLORS.textMain,
                    border: `1px solid ${COLORS.border}`,
                    transition: 'all 0.2s ease-in-out',
                    '&:hover': {
                        backgroundColor: COLORS.surface2,
                        borderColor: COLORS.osuPink,
                        boxShadow: 'none',
                    },
                    '&:active': {
                        transform: 'scale(0.98)',
                        borderColor: COLORS.osuPink,
                    },
                    '&:focus-visible': {
                        borderColor: COLORS.osuPink,
                        outline: 'none',
                    },
                    '&.Mui-disabled': {
                        backgroundColor: `${COLORS.surface2}88 !important`,
                        color: `${COLORS.textSub}88`,
                        border: `1px solid ${COLORS.border}55`,
                    }
                },
                outlined: {
                    backgroundColor: 'transparent',
                    color: COLORS.textMain,
                    border: `1px solid ${COLORS.border}`,
                    '&:hover': {
                        backgroundColor: 'transparent',
                        borderColor: COLORS.osuPink,
                    }
                },
                contained: {
                    backgroundColor: COLORS.osuPinkGray,
                    '&:hover': {
                        backgroundColor: COLORS.osuPinkGrayLight,
                        borderColor: COLORS.osuPinkGrayLight,
                    },
                },
            },
        },
        MuiFormLabel: {
            styleOverrides: {
                root: {
                    color: COLORS.textSub,
                },
            },
        },MuiInputLabel: {
            styleOverrides: {
                root: {
                    color: COLORS.textSub, // Domyślnie szary
                    fontSize: '0.85rem',
                    '&.Mui-focused': {
                        color: COLORS.osuPink, // Różowy tylko gdy aktywny
                    },
                    '&.MuiFormLabel-filled': {

                    }
                },
            },
        },
        MuiSelect: {
            defaultProps: {
                size: 'small',
            },
            styleOverrides: {
                root: {
                    padding: '8px 10px',
                    backgroundColor: '#121212',
                    borderRadius: '8px',
                    '& .MuiSelect-select': {
                        padding: '8px 10px',
                        minHeight: 'auto',
                        display: 'flex',
                        alignItems: 'center',
                        color: COLORS.textMain,
                    },
                    '& .MuiOutlinedInput-notchedOutline': {
                        borderColor: COLORS.border,
                        transition: 'all 0.2s ease-in-out',
                    },
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                        borderColor: COLORS.textSub,
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                        borderColor: COLORS.osuPink,
                        borderWidth: '1px',
                    },
                    '& .MuiSelect-icon': {
                        color: COLORS.textSub, // Ikona szara, żeby nie odciągać uwagi
                    },
                },
            },
        },
        MuiMenuItem: {
            styleOverrides: {
                root: {
                    padding: '8px 12px',
                    fontSize: '0.875rem',
                    color: COLORS.textMain,
                    '&:hover': {
                        backgroundColor: COLORS.surface2,
                        color: COLORS.osuPink,
                    },
                    '&.Mui-selected': {
                        backgroundColor: `${COLORS.osuPinkGray}33`,
                        color: COLORS.osuPink,
                        '&:hover': {
                            backgroundColor: `${COLORS.osuPinkGray}55`,
                        },
                    },
                },
            },
        },
    },
});


export const labelStyle: SxProps<Theme> = {
    color: COLORS.textSub,
    fontSize: '0.85rem',
    fontWeight: 700,
    letterSpacing: '0.05em',
    mb: 1,
    display: 'block'
};