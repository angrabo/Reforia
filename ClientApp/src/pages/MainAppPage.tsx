import {ChatProvider} from "../Context/ChatContext.tsx";
import ChatList from "../components/chat/ChatList.tsx";
import {ChatWindow} from "../components/chat/ChatWindow.tsx";
import ChatInput from "../components/chat/ChatInput.tsx";
import {useGlobalContext} from "../Context/GlobalContext.tsx";
import BeatmapBanner from "../components/BeatmapBanner.tsx";
import TournamentSidebar from "../components/TournamentSidebar.tsx";

const MainAppPage = () => {

    const { connectionId } = useGlobalContext();

    return (
        <>
            <ChatProvider connectionId={connectionId}>
                <div style={{ display: "flex", height: "100vh" }}>
                    <ChatList />
                    <div style={{ flex: 1, display: "flex", flexDirection: "column" }}>
                        <ChatWindow />
                        <ChatInput />
                        <BeatmapBanner/>
                    </div>
                    <TournamentSidebar/>
                </div>
            </ChatProvider>
        </>
    );
}

export default MainAppPage;