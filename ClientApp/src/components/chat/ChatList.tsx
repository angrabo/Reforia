import {useEffect, useState} from "react";
import {
    Box, Typography, List, ListItemText, Collapse,
    Button, Divider, ListItemButton, IconButton
} from "@mui/material";
import {
    ExpandLess,
    ExpandMore,
    Close as CloseIcon,
    DragIndicator as DragIcon,
    DeleteOutline as DeleteIcon
} from "@mui/icons-material";
import {DragDropContext, Droppable, Draggable, DropResult} from "@hello-pangea/dnd";
import {useChat} from "../../Context/ChatContext.tsx";
import AddChatModal from "./AddChatModal.tsx";
import {SettingsIcon} from "lucide-react";
import {SettingsModal} from "../modals/SettingsModal.tsx";
import AppLogo from "../AppLogo.tsx";
import {COLORS} from "../../utils/ThemeCreator.ts";
import {useTranslation} from "react-i18next";

const HANDLE_WIDTH = 28;

const ChatList = () => {
    const {
        groupedChats,
        setActiveChat,
        activeChat,
        leaveChannel,
        reorderChats,
        reorderTournaments,
        removeTournament,
        removeStage,
        addChat
    } = useChat();

    const [openTourneys, setOpenTourneys] = useState<Record<string, boolean>>({});
    const [modalOpen, setModalOpen] = useState(false);
    const [settingModalOpen, setSettingModalOpen] = useState(false);

    const { t } = useTranslation();

    useEffect(() => {
        addChat("BanchoBot", "Banchobot", "General");
    }, []);

    const handleCloseChat = (e: React.MouseEvent, chatId: string) => {
        e.stopPropagation();
        leaveChannel(chatId);
    };

    const handleRemoveTournament = (e: React.MouseEvent, tName: string) => {
        e.stopPropagation();
        removeTournament(tName);
    };

    const handleRemoveStage = (e: React.MouseEvent, tName: string, sName: string) => {
        e.stopPropagation();
        removeStage(tName, sName);
    };

    const toggleTourney = (tournament: string) => {
        setOpenTourneys(prev => ({
            ...prev,
            [tournament]: prev[tournament] === false ? true : false
        }));
    };

    const onDragEnd = (result: DropResult) => {
        const {source, destination, type} = result;
        if (!destination) return;
        if (type === "section") {
            reorderTournaments(source.index, destination.index);
            return;
        }
        if (source.droppableId === destination.droppableId && source.index === destination.index) return;

        const [sourceTourney, sourceStage] = source.droppableId.split('|');
        const [destTourney, destStage] = destination.droppableId.split('|');

        reorderChats(sourceTourney, sourceStage, source.index, destination.index, destTourney, destStage);
    };

    return (
        <Box sx={{
            width: 280,
            height: '100vh',
            bgcolor: COLORS.bgDark,
            color: 'white',
            borderRight: `1px solid ${COLORS.border}`,
            display: 'flex',
            flexDirection: 'column'
        }}>
            <AppLogo/>
            <DragDropContext onDragEnd={onDragEnd}>
                <Droppable droppableId="TOURNAMENTS_ROOT" type="section">
                    {(rootProvided) => (
                        <Box
                            ref={rootProvided.innerRef}
                            {...rootProvided.droppableProps}
                            sx={{flexGrow: 1, overflowY: 'auto', py: 1}}
                        >
                            {Object.entries(groupedChats).map(([tournament, stages], tIndex) => {
                                const isOpen = openTourneys[tournament] !== false;

                                return (
                                    <Draggable key={`tourney-${tournament}`} draggableId={`tourney-${tournament}`}
                                               index={tIndex}>
                                        {(tourneyDraggableProvided, tourneySnapshot) => (
                                            <Box
                                                ref={tourneyDraggableProvided.innerRef}
                                                {...tourneyDraggableProvided.draggableProps}
                                                sx={{
                                                    mb: 1,
                                                    bgcolor: tourneySnapshot.isDragging ? 'rgba(255, 255, 255, 0.05)' : 'transparent'
                                                }}
                                            >
                                                <Droppable droppableId={`${tournament}|General`} type="chat">
                                                    {(headerDropProvided, headerSnapshot) => (
                                                        <Box
                                                            ref={headerDropProvided.innerRef}
                                                            {...headerDropProvided.droppableProps}
                                                            sx={{bgcolor: headerSnapshot.isDraggingOver ? 'rgba(144, 202, 249, 0.1)' : 'transparent'}}
                                                        >
                                                            <ListItemButton
                                                                onClick={() => toggleTourney(tournament)}
                                                                sx={{
                                                                    py: 0.5, px: 1,
                                                                    '&:hover .section-actions': {opacity: 1},
                                                                    '&:hover .section-drag-handle': {opacity: 0.6}
                                                                }}
                                                            >
                                                                <Box
                                                                    {...tourneyDraggableProvided.dragHandleProps}
                                                                    className="section-drag-handle"
                                                                    sx={{
                                                                        width: HANDLE_WIDTH,
                                                                        opacity: 0,
                                                                        display: 'flex',
                                                                        color: COLORS.textSub,
                                                                        transition: 'opacity 0.2s'
                                                                    }}
                                                                >
                                                                    <DragIcon sx={{fontSize: 18}}/>
                                                                </Box>

                                                                <ListItemText
                                                                    primary={tournament === "Custom" ? "CUSTOM" : tournament.toUpperCase()}
                                                                    primaryTypographyProps={{
                                                                        variant: 'caption',
                                                                        fontWeight: 800,
                                                                        color: COLORS.textSub,
                                                                        style: {letterSpacing: '1px'}
                                                                    }}
                                                                />

                                                                <Box className="section-actions" sx={{
                                                                    opacity: 0,
                                                                    display: 'flex',
                                                                    alignItems: 'center',
                                                                    gap: 0.5,
                                                                    mr: 1,
                                                                    transition: 'opacity 0.2s'
                                                                }}>
                                                                    {tournament !== "Custom" && (
                                                                        <IconButton size="small"
                                                                                    onClick={(e) => handleRemoveTournament(e, tournament)}
                                                                                    sx={{
                                                                                        color: '#666',
                                                                                        p: 0.2,
                                                                                        '&:hover': {color: '#f44336'}
                                                                                    }}>
                                                                            <DeleteIcon sx={{fontSize: 16}}/>
                                                                        </IconButton>
                                                                    )}
                                                                </Box>
                                                                {isOpen ? <ExpandLess fontSize="small"
                                                                                      sx={{color: COLORS.textSub}}/> :
                                                                    <ExpandMore fontSize="small"
                                                                                sx={{color: COLORS.textSub}}/>}
                                                            </ListItemButton>
                                                            <Box
                                                                sx={{display: 'none'}}>{headerDropProvided.placeholder}</Box>
                                                        </Box>
                                                    )}
                                                </Droppable>

                                                <Collapse in={isOpen} timeout="auto">
                                                    {Object.entries(stages).map(([stage, chats]) => (
                                                        <Box key={stage} sx={{'&:hover .stage-delete': {opacity: 1}}}>
                                                            {stage !== "General" && (
                                                                <Box sx={{
                                                                    pl: 1,
                                                                    pr: 2,
                                                                    mt: 1,
                                                                    display: 'flex',
                                                                    justifyContent: 'space-between',
                                                                    alignItems: 'center'
                                                                }}>
                                                                    <Typography variant="caption" sx={{
                                                                        color: '#555',
                                                                        fontWeight: 700,
                                                                        pl: `${HANDLE_WIDTH}px`
                                                                    }}>
                                                                        {stage.toUpperCase()}
                                                                    </Typography>
                                                                    <IconButton
                                                                        className="stage-delete"
                                                                        size="small"
                                                                        onClick={(e) => handleRemoveStage(e, tournament, stage)}
                                                                        sx={{
                                                                            opacity: 0,
                                                                            p: 0,
                                                                            color: '#444',
                                                                            transition: 'opacity 0.2s',
                                                                            '&:hover': {color: '#f44336'}
                                                                        }}
                                                                    >
                                                                        <CloseIcon sx={{fontSize: 12}}/>
                                                                    </IconButton>
                                                                </Box>
                                                            )}

                                                            <Droppable droppableId={`${tournament}|${stage}`}
                                                                       type="chat">
                                                                {(provided, snapshot) => (
                                                                    <List
                                                                        {...provided.droppableProps}
                                                                        ref={provided.innerRef}
                                                                        component="div"
                                                                        disablePadding
                                                                        sx={{
                                                                            minHeight: '20px',
                                                                            bgcolor: snapshot.isDraggingOver ? 'rgba(255, 255, 255, 0.02)' : 'transparent',
                                                                            mb: 1
                                                                        }}
                                                                    >
                                                                        {chats.map((chat, index) => {
                                                                            const isActive = activeChat === chat.id;
                                                                            const isChannel = chat.type !== "user";

                                                                            let itemColor = COLORS.playerColor;
                                                                            if (isChannel) itemColor = COLORS.refereeColor;
                                                                            if (chat.id.toLowerCase() === "banchobot") itemColor = COLORS.osuPink;

                                                                            let hoverBg = 'rgba(144, 202, 249, 0.08)';
                                                                            if (chat.id.toLowerCase() === "banchobot") hoverBg = `rgba(255, 102, 170, 0.08)`;

                                                                            return (
                                                                                <Draggable key={chat.id}
                                                                                           draggableId={chat.id}
                                                                                           index={index}>
                                                                                    {(draggableProvided, dragSnapshot) => (
                                                                                        <ListItemButton
                                                                                            ref={draggableProvided.innerRef}
                                                                                            {...draggableProvided.draggableProps}
                                                                                            onClick={() => setActiveChat(chat.id)}
                                                                                            sx={{
                                                                                                pl: stage === "General" ? 1 : 3,
                                                                                                py: 0.5,
                                                                                                borderLeft: isActive ? `3px solid ${itemColor}` : '3px solid transparent',
                                                                                                bgcolor: dragSnapshot.isDragging ? 'rgba(255, 255, 255, 0.1)' : (isActive ? hoverBg : 'transparent'),
                                                                                                display: 'flex',
                                                                                                justifyContent: 'space-between',
                                                                                                alignItems: 'center',
                                                                                                '&:hover': {bgcolor: 'rgba(255, 255, 255, 0.03)'},
                                                                                                '&:hover .close-chat-btn': {opacity: 1},
                                                                                                '&:hover .drag-handle': {opacity: 0.6}
                                                                                            }}
                                                                                        >
                                                                                            <Box sx={{
                                                                                                display: 'flex',
                                                                                                alignItems: 'center',
                                                                                                overflow: 'hidden',
                                                                                                flexGrow: 1
                                                                                            }}>
                                                                                                <Box
                                                                                                    {...draggableProvided.dragHandleProps}
                                                                                                    className="drag-handle"
                                                                                                    sx={{
                                                                                                        width: HANDLE_WIDTH,
                                                                                                        opacity: 0,
                                                                                                        display: 'flex',
                                                                                                        transition: 'opacity 0.2s',
                                                                                                        color: COLORS.textSub
                                                                                                    }}
                                                                                                >
                                                                                                    <DragIcon
                                                                                                        sx={{fontSize: 18}}/>
                                                                                                </Box>

                                                                                                <Box
                                                                                                    sx={{overflow: 'hidden'}}>
                                                                                                    <Typography
                                                                                                        variant="body2"
                                                                                                        sx={{
                                                                                                            color: isActive ? itemColor : COLORS.textMain,
                                                                                                            fontWeight: isActive ? 600 : 400,
                                                                                                            whiteSpace: 'nowrap',
                                                                                                            overflow: 'hidden',
                                                                                                            textOverflow: 'ellipsis'
                                                                                                        }}>
                                                                                                        {chat.displayName.toLowerCase().includes("[closed]") ?
                                                                                                            chat.displayName.replace("[Closed]", `[${t('chat.closed')}]`) :
                                                                                                            chat.displayName}
                                                                                                    </Typography>
                                                                                                    <Typography
                                                                                                        variant="caption"
                                                                                                        sx={{
                                                                                                            color: isChannel ? COLORS.refereeMessage : COLORS.osuPink,
                                                                                                            fontFamily: 'Roboto Mono, monospace',
                                                                                                            opacity: 0.8,
                                                                                                            fontSize: '0.7rem',
                                                                                                            display: 'block'
                                                                                                        }}>
                                                                                                        {isChannel ? chat.id : `@${chat.id.replace('#', '')}`}
                                                                                                    </Typography>
                                                                                                </Box>
                                                                                            </Box>

                                                                                            <IconButton
                                                                                                className="close-chat-btn"
                                                                                                size="small"
                                                                                                onClick={(e) => handleCloseChat(e, chat.id)}
                                                                                                sx={{
                                                                                                    color: '#666',
                                                                                                    opacity: 0,
                                                                                                    transition: 'opacity 0.2s',
                                                                                                    '&:hover': {
                                                                                                        color: '#f44336',
                                                                                                        bgcolor: 'transparent'
                                                                                                    }
                                                                                                }}
                                                                                            >
                                                                                                <CloseIcon
                                                                                                    sx={{fontSize: 14}}/>
                                                                                            </IconButton>
                                                                                        </ListItemButton>
                                                                                    )}
                                                                                </Draggable>
                                                                            );
                                                                        })}
                                                                        {provided.placeholder}
                                                                    </List>
                                                                )}
                                                            </Droppable>
                                                        </Box>
                                                    ))}
                                                </Collapse>
                                            </Box>
                                        )}
                                    </Draggable>
                                );
                            })}
                            {rootProvided.placeholder}
                        </Box>
                    )}
                </Droppable>
            </DragDropContext>

            <Divider sx={{borderColor: COLORS.border}}/>
            <Box sx={{p: 2, bgcolor: COLORS.bgDark, display: 'flex', justifyContent: 'space-between', gap: 1}}>
                <Button variant="outlined" fullWidth onClick={() => setModalOpen(true)}>
                    + {t('chatList.actions.addChannel')}
                </Button>
                <IconButton onClick={() => setSettingModalOpen(true)} sx={{
                    color: COLORS.textSub,
                    transition: 'transform 0.2s ease-in-out',
                    '&:hover': {
                        color: COLORS.osuPink,
                        backgroundColor: 'rgba(138, 180, 248, 0.08)',
                        transform: 'rotate(30deg)'
                    }
                }}>
                    <SettingsIcon/>
                </IconButton>
            </Box>

            <SettingsModal open={settingModalOpen} onClose={() => setSettingModalOpen(false)}/>
            <AddChatModal open={modalOpen} onClose={() => setModalOpen(false)}/>

        </Box>
    );
};

export default ChatList;