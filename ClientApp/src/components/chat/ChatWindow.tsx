import { useEffect, useRef } from "react";
import { Box, Button, Typography, Link } from "@mui/material";
import { useChat } from "../../Context/ChatContext.tsx";
import { useGlobalContext } from "../../Context/GlobalContext.tsx";
import { useConfigContext } from "../../Context/ConfigContext.tsx";
import {COLORS} from "../../utils/ThemeCreator.ts";
import {useTranslation} from "react-i18next";

export const ChatWindow = () => {
    const { chats, activeChat, joinChannel } = useChat();
    const {t} = useTranslation();
    const { username } = useGlobalContext();
    const { userHighlight, alertOnKeyword, alertOnMention, highlightOnKeyword, keywordList } = useConfigContext();
    const bottomRef = useRef<HTMLDivElement>(null);

    const currentChat = activeChat ? chats[activeChat] : null;

    useEffect(() => {
        bottomRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [currentChat?.messages]);

    const audioRef = useRef(new Audio('/sounds/chat.wav'));

    useEffect(() => {
        const messages = currentChat?.messages;
        if (!messages || messages.length === 0) return;

        const lastMessage = messages[messages.length - 1];
        const messageText = lastMessage.message.toLowerCase();

        const isMe = lastMessage.sender.toLowerCase() === username?.toLowerCase();
        if (isMe) return;


        const isMentioned = username && messageText.includes(username.toLowerCase());
        const containsKeyword = keywordList.some(keyword =>
            keyword !== "" && messageText.includes(keyword.toLowerCase())
        );

        const shouldPlay = (alertOnMention && isMentioned) || (alertOnKeyword && containsKeyword);

        if (shouldPlay) {
            audioRef.current.currentTime = 0;
            audioRef.current.play().catch(e => console.error("Audio playback failed:", e));
        }

    }, [currentChat?.messages.length]);

    if (!activeChat || !currentChat) {
        return <Box sx={{ flex: 1, display: "flex", alignItems: "center", justifyContent: "center" }} />;
    }

    const formatTimestamp = (dateString: string) => {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return "??:??:??";
        return new Intl.DateTimeFormat('pl-PL', {
            hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false
        }).format(date);
    };


    const renderMessageContent = (text: string, color: string, senderColor: string) => {
        const urlRegex = /(https?:\/\/[^\s]+)/g;
        const parts = text.split(urlRegex);

        return parts.map((part, i) => {
            if (part.match(urlRegex)) {
                return (
                    <Link
                        key={i}
                        href={part}
                        target="_blank"
                        rel="noopener noreferrer"
                        sx={{
                            color: `${color}` || "#ffcc22",
                            textDecoration: 'none',
                            padding: '0 2px',
                            borderBottom: `1px solid ${color || "#ffcc22"}`,
                            borderRadius: '4px',
                            transition: 'all 0.2s',
                            display: 'inline-block',
                            '&:hover': {
                                backgroundColor: senderColor ? `${senderColor}33` : "#ffcc2233",
                                borderColor: senderColor || "#ffcc22",
                            }
                        }}
                    >
                        {part.length > 40 ? part.substring(0, 37) + "..." : part}
                    </Link>
                );
            }
            return <span key={i}>{part}</span>;
        });
    };

    return (
        <Box sx={{ flex: 1, display: "flex", flexDirection: "column", bgcolor: COLORS.bgDark, minHeight: 0 }}>
            <Box sx={{ p: '15px 24px', borderBottom: `1px solid ${COLORS.border}` }}>
                <Typography variant="h6" sx={{ color: COLORS.textMain, fontWeight: 600, fontSize: '1.1rem' }}>
                    {currentChat.metadata.displayName}
                </Typography>
                <Typography sx={{ color: COLORS.textSub, fontSize: '0.8rem', mt: 0.5 }}>
                    {currentChat.metadata.tournamentName}
                    {currentChat.metadata.stage ? ` • ${currentChat.metadata.stage}` : ""}
                </Typography>
            </Box>

            <Box sx={{
                flex: 1, overflowY: "auto", p: 3, display: "flex", flexDirection: "column", gap: 1, minHeight: 0,
                '&::-webkit-scrollbar': { width: '6px' },
                '&::-webkit-scrollbar-thumb': { backgroundColor: COLORS.border, borderRadius: '10px' }
            }}>
                {currentChat.messages.map((msg, idx) => {
                    const isBancho = msg.sender.toLowerCase() === "banchobot";
                    const isUser = msg.sender.toLowerCase() === username?.toLowerCase();

                    let senderColor = COLORS.playerColor;
                    if (isBancho) senderColor = COLORS.osuPink;
                    if (isUser) senderColor = "#4a9eff";

                    let textShadow = isBancho ? `0px 0px 6px ${COLORS.osuPink}` : (isUser ? `0px 0px 6px #4a9eff` : 'none');

                    let textColor = COLORS.chatMessage;
                    if (isBancho) textColor = COLORS.botMessage;
                    if (isUser) textColor = COLORS.refereeMessage;

                    const mpMatch = msg.message.match(/https:\/\/osu\.ppy\.sh\/(?:mp|community\/matches)\/(\d+)/);
                    const mpId = mpMatch ? mpMatch[1] : null;
                    const allPlayerReady = /^\s*[Aa]ll players are ready[.!?]?\s*$/.test(msg.message);

                    const isUserMentioned = username && msg.message.toLowerCase().includes(username.toLowerCase());
                    const isHighlighted = allPlayerReady || (isUserMentioned && userHighlight);

                    const containsKeyword = keywordList.map(keyword => keyword && msg.message.toLowerCase().includes(keyword.toLowerCase()));
                    if (containsKeyword && highlightOnKeyword) textColor = COLORS.highlight;

                    if (isHighlighted) textColor = COLORS.highlight;

                    return (
                        <Box key={idx} sx={{ display: 'flex', alignItems: 'baseline', mb: 0.2 }}>
                            <Typography sx={{ color: COLORS.textSub, fontSize: '0.9rem', fontFamily: 'Roboto Mono, monospace', width: '85px', flexShrink: 0 }}>
                                [{formatTimestamp(msg.timestamp)}]
                            </Typography>

                            <Typography sx={{ color: senderColor, fontWeight: 700, fontSize: '0.9rem', whiteSpace: 'nowrap', textShadow: textShadow, width: '130px', textAlign: 'right', pr: 1.5, flexShrink: 0 }}>
                                {msg.sender}:
                            </Typography>

                            <Typography sx={{
                                color: textColor,
                                fontSize: '0.9rem',
                                lineHeight: 1.5,
                                wordBreak: 'break-word',
                                textDecoration: allPlayerReady ? "underline" : "none",
                            }}>
                                {renderMessageContent(msg.message, textColor, senderColor)}
                            </Typography>

                            {mpId && (
                                <Button
                                    variant="outlined"
                                    size="small"
                                    onClick={() => joinChannel(`#mp_${mpId}`)}
                                    sx={{
                                        ml: 2, color: textColor, backgroundColor: "transparent", borderColor: textColor, fontSize: '0.7rem', minWidth: 'fit-content', py: 0.5, textTransform: 'none',
                                        '&:hover': { borderColor: senderColor, bgcolor: `${senderColor}33` }
                                    }}
                                >
                                    {t("chat.inputs.joinLobby", {name: `#mp_${mpId}`})}
                                </Button>
                            )}
                        </Box>
                    );
                })}
                <div ref={bottomRef} />
            </Box>
        </Box>
    );
};