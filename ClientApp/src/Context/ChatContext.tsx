import React, {
    createContext,
    useContext,
    useEffect,
    useRef,
    useState,
    useMemo
} from "react";
import {signalRClient} from "../utils/FunctionClient.ts";
import {CreateIrcConnectionFunction, IrcMessage} from "../types/types.ts";
import {useGlobalContext} from "./GlobalContext.tsx";

/* ===========================
   Typy
=========================== */

interface ChatMetadata {
    id: string;
    type: "channel" | "user" | "tournament";
    tournamentName?: string;
    stage?: string;
    displayName: string;
    beatmap?: Beatmap;
    lobbySettings?: lobbySettings;
    players?: Player[];
}

interface lobbySettings {
    freemod: string;
    mods: string;
    winCondition: string;
    teamMode: string;
    lobbySize: number;
}

interface Player {
    username: string;
    isReady: boolean;
    Team: | "None" | "Blue" | "Red";
    slot: number;
}

interface Beatmap {
    id: string;
    setId: string;
    artist: string;
    title: string;
    creator: string;
    version: string;
    circleSize: string;
    overallDifficulty: string;
    approachRate: string;
    healthDrain: string;
    bpm: string;
    length: string;
    starRating: string;
}

interface ChatState {
    metadata: ChatMetadata;
    messages: IrcMessage[];
}

interface ChatContextType {
    chats: Record<string, ChatState>;
    activeChat: string | null;
    setActiveChat: (chatId: string) => void;
    sendMessage: (text: string, overrideChatId?: string) => void;
    addChat: (id: string, tournament?: string, stage?: string) => void;
    reconnectChannels: (newConnId: string) => void;
    joinChannel: (id: string, tournament?: string, stage?: string) => Promise<void>;
    leaveChannel: (id: string) => Promise<void>;
    removeChat: (id: string) => void;
    changeChatDisplayName: (chatId: string, newDisplayName: string) => void;
    groupedChats: Record<string, Record<string, ChatMetadata[]>>;
    tournamentOrder: string[];
    reorderChats: (
        sourceTourney: string,
        sourceStage: string,
        startIndex: number,
        endIndex: number,
        destTourney?: string,
        destStage?: string
    ) => void;
    reorderTournaments: (startIndex: number, endIndex: number) => void;
    removeTournament: (name: string) => void;
    removeStage: (tourneyName: string, stageName: string) => void;
}

type ChatMap = Record<string, ChatState>;

const ChatContext = createContext<ChatContextType | undefined>(undefined);

const normalizeChatId = (id: string) =>
    id.trim().replace(/\s+/g, "_").toLowerCase();

const resolveChatType = (chatId: string): ChatMetadata["type"] => {
    if (chatId.startsWith("#mp_")) return "tournament";
    if (chatId.startsWith("#")) return "channel";
    return "user";
};

const parseBanchoRoomName = (message: string): string | null => {
    const roomMatch = message.match(/Room name:\s*(.+?),\s*History:/);
    if (!roomMatch) return null;

    const rawName = roomMatch[1].trim();
    const tournamentRegex = /^(.+?):\s*\((.+?)\)\s*vs\s*\((.+?)\)/;
    const match = rawName.match(tournamentRegex);

    if (match) {
        const [, id, team1, team2] = match;
        return `${id}: ${team1} vs. ${team2}`;
    }

    return rawName;
};

const parsePlayerData = (message: string): Partial<Player> | null => {
    const playerMatch = message.match(
        /^Slot\s+(\d+)\s+(Ready|Not Ready)\s+https?:\/\/osu\.ppy\.sh\/u\/\d+\s+([^\s\[]+)(?:\s+\[Team\s+(Blue|Red)\s*\])?/
    );

    if (playerMatch) {
        return {
            slot: parseInt(playerMatch[1]),
            isReady: playerMatch[2] === "Ready",
            username: playerMatch[3],
            Team: (playerMatch[4] as "Blue" | "Red") || "None"
        };
    }
    return null;
};

const resolveDisplayName = (
    chatId: string,
    msg: IrcMessage,
    existing?: ChatMetadata
): string => {
    if (msg.sender.toLowerCase() === "banchobot") {
        const parsed = parseBanchoRoomName(msg.message);
        if (parsed) return parsed;
    }

    if (!chatId.startsWith("#")) {
        return msg.sender;
    }

    return existing?.displayName ?? chatId;
};

const getBeatmapIdFromMessage = (msg: IrcMessage): string | null => {
    if (msg.sender.toLowerCase() !== "banchobot") return null;
    const match = msg.message.match(/b(?:eatmaps)?\/(\d+)/);
    return match ? match[1] : null;
};

const parseLobbySettings = (message: string, currentSettings?: lobbySettings): lobbySettings | null => {
    const settings: lobbySettings = currentSettings
        ? {...currentSettings}
        : {freemod: "false", mods: "None", winCondition: "Score", teamMode: "HeadToHead", lobbySize: 16};

    const msg = message.trim();

    if (msg.includes("Team mode:")) {
        const teamModeMatch = msg.match(/Team mode:\s*([a-zA-Z]+)/);
        const winCondMatch = msg.match(/Win condition:\s*([a-zA-Z0-9]+)/);
        if (teamModeMatch) settings.teamMode = teamModeMatch[1];
        if (winCondMatch) settings.winCondition = winCondMatch[1];
        return settings;
    }

    if (msg.startsWith("Changed match settings to")) {
        const content = msg.replace("Changed match settings to ", "");
        const parts = content.split(", ");
        parts.forEach(part => {
            const p = part.trim();
            if (p.includes("slots")) {
                settings.lobbySize = parseInt(p.split(" ")[0]);
            } else if (["HeadToHead", "TagCoop", "TeamVs", "TagTeamVs"].includes(p)) {
                settings.teamMode = p;
            } else {
                settings.winCondition = p;
            }
        });
        return settings;
    }

    if (msg.startsWith("Active mods:")) {
        const modsPart = msg.replace("Active mods:", "").trim();
        const hasFreemod = modsPart.toLowerCase().includes("freemod");
        settings.freemod = hasFreemod ? "true" : "false";

        const cleanMods = modsPart
            .split(", ")
            .filter(m => m.toLowerCase() !== "freemod" && m !== "")
            .join(", ");

        settings.mods = cleanMods === "" ? "None" : cleanMods;
        return settings;
    }

    if (msg.includes("Enabled") || msg.includes("Disabled all mods")) {
        if (msg.toLowerCase().includes("enabled freemod")) settings.freemod = "true";
        if (msg.toLowerCase().includes("disabled freemod")) settings.freemod = "false";

        if (msg.startsWith("Disabled all mods")) {
            settings.mods = "None";
        } else {
            const enabledMatch = msg.match(/Enabled\s+(.*?)(?:,\s*enabled freemod|,\s*disabled freemod|$)/i);
            if (enabledMatch && enabledMatch[1]) {
                const extracted = enabledMatch[1].trim();
                if (extracted.toLowerCase() !== "freemod") {
                    settings.mods = extracted;
                }
            }
        }
        return settings;
    }

    return null;
};

export const ChatProvider = ({ connectionId, children }: { connectionId: string; children: React.ReactNode; }) => {
    const [chats, setChats] = useState<ChatMap>({});
    const [activeChat, setActiveChat] = useState<string | null>(null);
    const [chatOrder, setChatOrder] = useState<string[]>([]);
    const [tournamentOrder, setTournamentOrder] = useState<string[]>([]);
    const [manualStages, setManualStages] = useState<Record<string, string[]>>({});

    const joinedRef = useRef(false);
    const {username, updateConnectionId} = useGlobalContext();

    const chatsRef = useRef(chats);
    useEffect(() => { chatsRef.current = chats; }, [chats]);

    useEffect(() => {
        const handleRestoreSession = async () => {
            console.log("Restoring IRC session...");

            const response = await signalRClient.callFunction<CreateIrcConnectionFunction>("CreateIrcConnectionFunction");

            if (response.body?.connectionId) {
                const newId = response.body.connectionId;
                updateConnectionId(newId);

                const activeChats = Object.values(chatsRef.current);
                for (const chat of activeChats) {
                    if (chat.metadata.type === "channel") {
                        await signalRClient.callFunction("JoinIrcChannelFunction", {
                            ConnectionId: newId,
                            Channel: chat.metadata.id
                        });
                    }
                }
            }
        };

        signalRClient.setOnReconnectHandler(handleRestoreSession);
    }, []);

    useEffect(() => {
        const init = async () => {
            try {
                await signalRClient.start();
                await signalRClient.joinIrcConnection(connectionId);
                joinedRef.current = true;
            } catch (err) {
                console.error("SignalR init error:", err);
            }
        };

        init();

        const handler = (msg: IrcMessage) => {
            if (msg.connectionId !== connectionId) return;

            const chatId = normalizeChatId(msg.chatId);
            const text = msg.message.trim();

            setChats(prev => {
                const existing = prev[chatId];
                let currentMetadata = existing ? {...existing.metadata} : null;
                let currentMessages = existing ? [...existing.messages, msg] : [msg];

                if (!currentMetadata) {
                    const type = resolveChatType(chatId);
                    const tName = "Custom";
                    const displayName = resolveDisplayName(chatId, msg);

                    setTournamentOrder(old => old.includes(tName) ? old : [...old, tName]);

                    currentMetadata = {
                        id: chatId,
                        type,
                        tournamentName: tName,
                        stage: "General",
                        displayName,
                        players: [],
                    };
                } else {
                    currentMetadata.displayName = resolveDisplayName(chatId, msg, currentMetadata);
                }

                if (msg.sender.toLowerCase() === "banchobot") {
                    const updatedSettings = parseLobbySettings(text, currentMetadata.lobbySettings);
                    if (updatedSettings) {
                        currentMetadata.lobbySettings = updatedSettings;
                    }

                    let playersUpdate: ((prev: Player[]) => Player[]) | null = null;
                    const playerData = parsePlayerData(text);
                    const teamMatch = text.match(/^(.+?)\s+changed\s+to\s+(Blue|Red)$/);
                    const moveMatch = text.match(/^(.+?)\s+moved\s+to\s+slot\s+(\d+)$/);
                    const leftMatch = text.match(/^(.+?)\s+left\s+the\s+game\.$/);
                    const joinedMatch = text.match(/^(.+?)\s+joined\s+in\s+slot\s+(\d+)(?:\s+for\s+team\s+(blue|red))?\.$/i);
                    if (text === "All players are ready") {
                        playersUpdate = (p) => p.map(x => ({...x, isReady: true}));
                    } else if (text === "The match has started!") {
                        playersUpdate = (p) => p.map(x => ({...x, isReady: false}));
                    } else if (playerData) {
                        playersUpdate = (p) => {
                            const idx = p.findIndex(x => x.username === playerData.username);
                            if (idx > -1) {
                                const next = [...p];
                                next[idx] = {...next[idx], ...playerData} as Player;
                                return next;
                            }
                            return [...p, playerData as Player];
                        };
                    } else if (teamMatch) {
                        const [, user, team] = teamMatch;
                        playersUpdate = (p) => p.map(x => x.username === user ? {
                            ...x,
                            Team: team as "Blue" | "Red"
                        } : x);
                    } else if (moveMatch) {
                        const [, user, slot] = moveMatch;
                        playersUpdate = (p) => p.map(x => x.username === user ? {...x, slot: parseInt(slot)} : x);
                    } else if (leftMatch) {
                        const user = leftMatch[1];
                        playersUpdate = (p) => p.filter(x => x.username !== user);
                    } else if (joinedMatch) {
                        const [, user, slot, teamColor] = joinedMatch;

                        let assignedTeam: "None" | "Blue" | "Red" = "None";
                        if (teamColor?.toLowerCase() === "blue") assignedTeam = "Blue";
                        if (teamColor?.toLowerCase() === "red") assignedTeam = "Red";

                        playersUpdate = (p) => [
                            ...p.filter(x => x.username !== user),
                            {
                                username: user,
                                slot: parseInt(slot),
                                isReady: false,
                                Team: assignedTeam
                            }
                        ];
                    }

                    if (playersUpdate) {
                        const updatedPlayers = playersUpdate(currentMetadata.players || []);
                        currentMetadata.players = [...updatedPlayers].sort((a, b) => a.slot - b.slot);
                    }
                }

                return {
                    ...prev,
                    [chatId]: {
                        metadata: currentMetadata as ChatMetadata,
                        messages: currentMessages
                    }
                };
            });

            const beatmapId = getBeatmapIdFromMessage(msg);
            if (beatmapId) {
                signalRClient.callFunction("GetBeatmapInfoFunction", {BeatmapId: beatmapId})
                    .then(res => {
                        const beatmapData = res.body as Beatmap;
                        setChats(prev => {
                            if (!prev[chatId]) return prev;
                            return {
                                ...prev,
                                [chatId]: {
                                    ...prev[chatId],
                                    metadata: {...prev[chatId].metadata, beatmap: beatmapData}
                                }
                            };
                        });
                    })
                    .catch(err => console.error("Error fetching beatmap:", err));
            }
        };

        signalRClient.on<IrcMessage>("ircMessage", handler);

        return () => {
            signalRClient.off("ircMessage", handler);
            if (joinedRef.current) {
                signalRClient.leaveIrcConnection(connectionId);
            }
        };
    }, [connectionId]);

    /* ===========================
       API
    =========================== */

    const addChat = (id: string, tournament?: string, stage?: string) => {
        const chatId = normalizeChatId(id);
        const tName = tournament || "Custom";
        const sName = stage || "General";

        setTournamentOrder(old => old.includes(tName) ? old : [...old, tName]);
        setManualStages(prev => ({
            ...prev,
            [tName]: prev[tName]?.includes(sName) ? prev[tName] : [...(prev[tName] || []), sName]
        }));

        setChats(prev => {
            if (prev[chatId]) return prev;
            setChatOrder(old => [...old, chatId]);
            return {
                ...prev,
                [chatId]: {
                    metadata: {
                        id: chatId,
                        type: chatId.startsWith("#mp_") ? "tournament" : (chatId.startsWith("#") ? "channel" : "user"),
                        tournamentName: tName,
                        stage: sName,
                        displayName: id
                    },
                    messages: []
                }
            };
        });
    };

    const groupedChats = useMemo(() => {
        const groups: Record<string, Record<string, ChatMetadata[]>> = {};

        tournamentOrder.forEach(t => {
            groups[t] = {};
            (manualStages[t] || []).forEach(s => {
                groups[t][s] = [];
            });
        });

        chatOrder.forEach(chatId => {
            const chat = chats[chatId];
            if (!chat) return;
            const t = chat.metadata.tournamentName || "Custom";
            const s = chat.metadata.stage || "General";

            if (!groups[t]) groups[t] = {};
            groups[t][s] ??= [];
            groups[t][s].push(chat.metadata);
        });

        return groups;
    }, [chats, chatOrder, tournamentOrder, manualStages]);

    const removeTournament = (name: string) => {
        setTournamentOrder(prev => prev.filter(t => t !== name));
        const chatsToRemove = Object.values(chats).filter(c => c.metadata.tournamentName === name);
        chatsToRemove.forEach(c => leaveChannel(c.metadata.id));
    };

    const removeStage = (tourneyName: string, stageName: string) => {
        setManualStages(prev => ({
            ...prev,
            [tourneyName]: (prev[tourneyName] || []).filter(s => s !== stageName)
        }));
        const chatsToRemove = Object.values(chats).filter(c => c.metadata.tournamentName === tourneyName && c.metadata.stage === stageName);
        chatsToRemove.forEach(c => removeChat(c.metadata.id));
    };

    const reorderTournaments = (startIndex: number, endIndex: number) => {
        setTournamentOrder(old => {
            const result = Array.from(old);
            const [removed] = result.splice(startIndex, 1);
            result.splice(endIndex, 0, removed);
            return result;
        });
    };

    const reorderChats = (
        sourceTourney: string,
        sourceStage: string,
        startIndex: number,
        endIndex: number,
        destTourney?: string,
        destStage?: string
    ) => {
        const targetTourney = destTourney || sourceTourney;
        const targetStage = destStage || sourceStage;

        const sourceChats = groupedChats[sourceTourney]?.[sourceStage] || [];
        if (sourceChats.length === 0) return;

        const movingChat = sourceChats[startIndex];
        const movingChatId = movingChat.id;

        setManualStages(prev => ({
            ...prev,
            [targetTourney]: prev[targetTourney]?.includes(targetStage) ? prev[targetTourney] : [...(prev[targetTourney] || []), targetStage]
        }));

        setChats(prev => {
            const chat = prev[movingChatId];
            if (!chat) return prev;

            return {
                ...prev,
                [movingChatId]: {
                    ...chat,
                    metadata: {
                        ...chat.metadata,
                        tournamentName: targetTourney,
                        stage: targetStage
                    }
                }
            };
        });

        setChatOrder(oldOrder => {
            const newOrder = [...oldOrder];

            const oldIdx = newOrder.indexOf(movingChatId);
            if (oldIdx !== -1) {
                newOrder.splice(oldIdx, 1);
            }

            const destChats = (groupedChats[targetTourney]?.[targetStage] || []).filter(c => c.id !== movingChatId);

            if (destChats.length > 0) {
                let targetIdx;
                if (endIndex < destChats.length) {
                    const referenceId = destChats[endIndex].id;
                    targetIdx = newOrder.indexOf(referenceId);
                } else {
                    const lastInDestId = destChats[destChats.length - 1].id;
                    targetIdx = newOrder.indexOf(lastInDestId) + 1;
                }

                if (targetIdx === -1) newOrder.push(movingChatId);
                else newOrder.splice(targetIdx, 0, movingChatId);
            } else {
                newOrder.push(movingChatId);
            }

            return newOrder;
        });
    };


    const reconnectChannels = async (newConnId: string) => {
        chats && Object.values(chats).forEach(c => {
            if (c.metadata.type === "channel") {
                signalRClient.callFunction("JoinIrcChannelFunction", {ConnectionId: newConnId, Channel: c.metadata.id});
            }
        });
    }

    const joinChannel = async (id: string, tournament?: string, stage?: string) => {
        const chatId = normalizeChatId(id);

        if (chatId.startsWith("#")) {
            await signalRClient.callFunction("JoinIrcChannelFunction", {
                ConnectionId: connectionId,
                Channel: chatId
            });
        }

        addChat(id, tournament, stage);
        setActiveChat(chatId);

        if (chatId.startsWith("#mp_")) {
            sendMessage("!mp settings", chatId);
        }
    };

    const leaveChannel = async (id: string) => {
        const chatId = normalizeChatId(id);
        removeChat(chatId);
        await signalRClient.callFunction("LeaveIrcChannelFunction", {ConnectionId: connectionId, Channel: chatId});
        setActiveChat(null);
    }

    const changeChatDisplayName = (chatId: string, newDisplayName: string) => {
        const id = normalizeChatId(chatId);

        setChats(prev => {
            const existing = prev[id];
            if (!existing || existing.metadata.displayName === newDisplayName)
                return prev;

            return {
                ...prev,
                [id]: {
                    ...existing,
                    metadata: {
                        ...existing.metadata,
                        displayName: newDisplayName
                    }
                }
            };
        });
    };

    const removeChat = (id: string) => {
        const chatId = normalizeChatId(id);
        setChatOrder(old => old.filter(c => c !== chatId));
        setChats(prev => {
            const {[chatId]: _, ...rest} = prev;
            return rest;
        });
    };

    const markChatAsClosed = (chatId: string) => {
        const normalizedId = chatId.toLowerCase();
        setChats((prev: ChatMap): ChatMap => {
            const existingChat = prev[normalizedId];
            if (!existingChat || existingChat.metadata.displayName.startsWith("[Closed]")) return prev;

            return {
                ...prev,
                [normalizedId]: {
                    ...existingChat,
                    metadata: {
                        ...existingChat.metadata,
                        displayName: `[Closed] ${existingChat.metadata.displayName}`
                    }
                }
            };
        });
    };

    const sendMessage = async (text: string, overrideChatId?: string) => {
        const target = normalizeChatId(overrideChatId || activeChat || "");
        if (!target) return;

        const newMessage: IrcMessage = {
            chatId: target,
            connectionId,
            sender: username || "You",
            message: text,
            timestamp: new Date().toISOString()
        };

        if (text.trim().toLowerCase() === "!mp close" && target.startsWith("#mp_")) {
            markChatAsClosed(target);
        }

        setChats(prev => {
            const chat = prev[target];
            if (!chat) return prev;

            return {
                ...prev,
                [target]: {
                    ...chat,
                    messages: [...chat.messages, newMessage]
                }
            };
        });

        await signalRClient.callFunction("SendIrcMessageFunction", {
            ConnectionId: connectionId,
            Channel: target,
            Message: text
        });
    };

    return (
        <ChatContext.Provider
            value={{
                chats,
                activeChat,
                setActiveChat,
                sendMessage,
                addChat,
                joinChannel,
                leaveChannel,
                removeChat,
                changeChatDisplayName,
                reorderChats,
                reorderTournaments,
                removeTournament,
                removeStage,
                reconnectChannels,
                tournamentOrder,
                groupedChats
            }}
        >
            {children}
        </ChatContext.Provider>
    );
};

export const useChat = () => {
    const ctx = useContext(ChatContext);
    if (!ctx) throw new Error("useChat must be used inside ChatProvider");
    return ctx;
};