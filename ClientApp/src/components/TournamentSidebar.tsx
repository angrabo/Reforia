import {
    Box,
    Divider,
} from '@mui/material';
import LobbySettings from "./sidebar/LobbySettings.tsx";
import {PlayerList} from "./sidebar/PlayerList.tsx";
import QuickCommands from "./sidebar/QuickCommands.tsx";
import {useChat} from "../Context/ChatContext.tsx";
import {COLORS} from "../utils/ThemeCreator.ts";

export default function TournamentSidebar() {

    const { chats, activeChat } = useChat();
    const currentChat = activeChat ? chats[activeChat] : null;
    if (!activeChat || !currentChat || chats[activeChat].metadata.type != "tournament") {
        return null;
    }

    return (
        <Box
            sx={{
                width: 320,
                height: '100vh',
                bgcolor: COLORS.bgDark,
                p: 2,
                borderRight: `1px solid ${COLORS.border}`,
                display: 'flex',
                flexDirection: 'column',
                boxSizing: 'border-box',
                overflow: 'hidden'
            }}
        >
            <Box sx={{
                display: 'flex',
                flexDirection: 'column',
                flex: 1,
                minHeight: 0,
                overflow: 'hidden'
            }}>
                <LobbySettings />
                <PlayerList />
            </Box>

            <Box sx={{ flexShrink: 0 }}>
                <Divider sx={{ my: 3, borderColor: COLORS.border }} />
                <QuickCommands />
            </Box>
        </Box>
    );
}