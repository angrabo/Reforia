import { useState } from "react";
import { Box, InputBase, IconButton, Paper } from "@mui/material";
import SendRoundedIcon from '@mui/icons-material/SendRounded';
import { useChat } from "../../Context/ChatContext.tsx";
import {COLORS} from "../../utils/ThemeCreator.ts";
import {useTranslation} from "react-i18next";

const ChatInput = () => {
    const [text, setText] = useState("");
    const { sendMessage, chats, activeChat } = useChat();
    const {t} = useTranslation();

    const currentChat = activeChat ? chats[activeChat] : null;

    if (!activeChat || !currentChat) {
        return <Box sx={{ height: '80px', bgcolor: COLORS.bgDark }} />;
    }

    const send = async () => {
        if (!text.trim()) return;
        await sendMessage(text);
        setText("");
    };

    return (
        <Box sx={{
            p: 2,
            bgcolor: COLORS.bgDark,
            borderTop: `1px solid ${COLORS.border}`
        }}>
            <Paper
                elevation={0}
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    bgcolor: COLORS.surface1,
                    border: `1px solid ${COLORS.border}`,
                    borderRadius: '8px',
                    px: 1.5,
                    py: 0.5,
                    transition: 'border-color 0.2s',
                    '&:focus-within': {
                        borderColor: COLORS.primary,
                    }
                }}
            >
                <InputBase
                    sx={{
                        flex: 1,
                        color: COLORS.textMain,
                        fontSize: '0.95rem',
                        ml: 1
                    }}
                    placeholder={t('chat.inputs.messagePlaceholder')}
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === "Enter") {
                            e.preventDefault();
                            send();
                        }
                    }}
                />
                <IconButton
                    onClick={send}
                    disabled={!text.trim()}
                    sx={{
                        color: text.trim() ? COLORS.primary : COLORS.textSub,
                        transition: '0.2s',
                        '&:hover': {
                            bgcolor: 'rgba(255, 255, 255, 0.05)',
                        }
                    }}
                >
                    <SendRoundedIcon fontSize="small" />
                </IconButton>
            </Paper>
        </Box>
    );
};

export default ChatInput;