import {Box, IconButton, Stack, Typography} from "@mui/material";
import {useChat} from "../../Context/ChatContext.tsx";
import {COLORS, labelStyle} from "../../utils/ThemeCreator.ts";
import {useTranslation} from "react-i18next";
import RefreshIcon from '@mui/icons-material/Refresh';

const LobbySettings = () => {

    const { chats, activeChat, sendMessage } = useChat();
    const { t } = useTranslation();
    const currentChat = activeChat ? chats[activeChat] : null;

    const lobbySettings = currentChat?.metadata.lobbySettings;

    const handleRefresh = () => {
        sendMessage(`!mp settings`);
    };

    const settings = [
        {
            label: t('sidebar.settings.freemod'),
            value: lobbySettings?.freemod == null
                ? ""
                : (lobbySettings.freemod === "true" ? t('common.yes') : t('common.no')),
            color: COLORS.primary
        },
        { label: t('sidebar.settings.mods'), value: lobbySettings?.mods, color: COLORS.primary },
        { label: t('sidebar.settings.winCondition'), value: lobbySettings?.winCondition, color: COLORS.primary },
        { label: t('sidebar.settings.teamMode'), value: lobbySettings?.teamMode, color: COLORS.primary },
    ];

    return (
        <Box sx={{ mt: 3 }}>
            <Stack
                direction="row"
                alignItems="center"
                justifyContent="space-between"
                sx={{ mb: 2, lineHeight: 1 }}
            >
                <Typography sx={{ ...labelStyle, mb: 0 }}>
                    {t('sidebar.sections.settings')}
                </Typography>

                <IconButton
                    onClick={handleRefresh}
                    size="small"
                    sx={{
                        p: 0,
                        color: COLORS.textSub,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        '&:hover': {
                            color: COLORS.primary,
                            backgroundColor: 'transparent',
                            '& svg': {
                                transform: 'rotate(90deg)',
                            }
                        }
                    }}
                >
                    <RefreshIcon
                        sx={{
                            fontSize: '1.3rem',
                            transition: 'transform 0.4s ease-in-out',
                            display: 'block'
                        }}
                    />
                </IconButton>
            </Stack>

            <Stack spacing={0.5}>
                {settings.map((s) => (
                    <Box key={s.label} sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Typography sx={{ color: COLORS.textSub, fontSize: '0.85rem' }}>{s.label}:</Typography>
                        <Typography sx={{ color: s.color, fontSize: '0.85rem', fontWeight: 500 }}>{s.value}</Typography>
                    </Box>
                ))}
            </Stack>
        </Box>
    );
};

export default LobbySettings;