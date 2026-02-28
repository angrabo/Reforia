import { Box, Paper, Stack, SxProps, Theme, Typography } from "@mui/material";
import { useChat } from "../../Context/ChatContext.tsx";
import {COLORS, labelStyle} from "../../utils/ThemeCreator.ts";
import {useTranslation} from "react-i18next";

const cardStyle: SxProps<Theme> = {
    bgcolor: COLORS.surface1,
    border: `1px solid ${COLORS.border}`,
    borderRadius: '8px',
    p: 2,
    backgroundImage: 'none',
};

const getTeamColor = (team: string | undefined) => {
    if (team === "Blue") return "#448aff";
    if (team === "Red") return "#ff5252";
    return COLORS.border;
};

export const PlayerList = () => {
    const { chats, activeChat } = useChat();
    const { t } = useTranslation();
    const currentChat = activeChat ? chats[activeChat] : null;

    const players = currentChat?.metadata.players || [];
    const lobbySize = currentChat?.metadata.lobbySettings?.lobbySize || 16;

    return (
        <Box sx={{
            mt: 3,
            display: 'flex',
            flexDirection: 'column',
            flex: 1,
            minHeight: 0
        }}>
            <Typography sx={labelStyle}>
                {t('sidebar.sections.players', {count: players.length, maxCount: lobbySize})}
            </Typography>

            <Stack
                spacing={1}
                sx={{
                    flex: 1,
                    overflowY: 'auto',
                    pr: 1,
                    '&::-webkit-scrollbar': { width: '6px' },
                    '&::-webkit-scrollbar-thumb': {
                        backgroundColor: COLORS.border,
                        borderRadius: '10px',
                    }
                }}
            >
                {players.map((player) => (
                    <Paper
                        key={player.username}
                        sx={{
                            ...cardStyle,
                            p: '8px 10px',
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            borderLeft: `4px solid ${getTeamColor(player.Team)}`,
                            transition: 'all 0.2s ease'
                        }}
                    >
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                            <Box sx={{
                                width: 10,
                                height: 10,
                                borderRadius: '50%',
                                bgcolor: player.isReady ? COLORS.success : COLORS.textSub,
                                boxShadow: player.isReady ? `0 0 8px ${COLORS.success}` : 'none'
                            }} />

                            <Box>
                                <Typography sx={{ color: COLORS.textMain, fontSize: '0.9rem', fontWeight: 500 }}>
                                    {player.username}
                                </Typography>
                                {player.Team !== "None" && (
                                    <Typography sx={{ color: getTeamColor(player.Team), fontSize: '0.7rem', fontWeight: 'bold' }}>
                                        {player.Team.toUpperCase()} TEAM
                                    </Typography>
                                )}
                            </Box>
                        </Box>

                        <Typography sx={{ color: COLORS.textSub, fontSize: '0.8rem' }}>
                            SLOT {player.slot}
                        </Typography>
                    </Paper>
                ))}

                {players.length === 0 && (
                    <Typography sx={{ color: COLORS.textSub, textAlign: 'center', mt: 2, fontSize: '0.8rem' }}>
                        {t('sidebar.players.noData')}
                    </Typography>
                )}
            </Stack>
        </Box>
    );
};