import { Box, Typography, Stack, Divider } from '@mui/material';
import {useChat} from "../Context/ChatContext.tsx";
import {useConfigContext} from "../Context/ConfigContext.tsx";
import {useTranslation} from "react-i18next";

const BeatmapBanner = () => {

    const { chats, activeChat } = useChat();
    const { apiToken } = useConfigContext();
    const {showBeatmapBanner} = useConfigContext()
    const { t } = useTranslation();

    const currentChat = activeChat ? chats[activeChat] : null;
    if (!activeChat || !currentChat || chats[activeChat].metadata.type != "tournament" || !showBeatmapBanner) {
        return null;
    }

    if (!activeChat) return null;

    const beatmap = currentChat.metadata.beatmap;

    if (!apiToken) return (
        <Box sx={{backgroundColor: '#121212', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
            <Typography sx={{ color: 'white', textAlign: 'center', py: 4 }}>
                {t('beatmap.noApiToken')}
            </Typography>
        </Box>
    )

    if (!beatmap) return (
        <Box sx={{backgroundColor: '#121212', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
            <Typography variant="h6" sx={{ color: 'white', textAlign: 'center', py: 4 }}>
                {t('beatmap.noBeatmap')}
            </Typography>
        </Box>
    )

    return (
        <Box
            sx={{
                position: 'relative',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                px: 3,
                py: 2,
                color: 'white',
                overflow: 'hidden',
                backgroundImage: `linear-gradient(rgba(0,0,0,0.7), rgba(0,0,0,0.7)), url(https://assets.ppy.sh/beatmaps/${beatmap?.setId}/covers/cover.jpg)`,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
            }}
        >
            <Typography variant="h5" component="h2" sx={{ fontWeight: 'bold', mb: 0.5 }}>
                {beatmap?.title}
            </Typography>

            <Stack direction="row" spacing={2} sx={{ mb: 1, opacity: 0.9 }}>
                <Typography variant="body2">
                    mapper <strong>{beatmap?.creator}</strong>
                </Typography>
                <Typography variant="body2">
                    {t('beatmap.difficulty')} <strong style={{ color: '#ff66aa' }}>{beatmap?.version}</strong>
                </Typography>
            </Stack>

            <Stack
                direction="row"
                spacing={2}
                divider={<Divider orientation="vertical" flexItem sx={{ bgcolor: 'rgba(255,255,255,0.2)' }} />}
                sx={{ flexWrap: 'wrap' }}
            >
                <StatItem label="CS" value={beatmap?.circleSize as string} />
                <StatItem label="HP" value={beatmap?.healthDrain as string} />
                <StatItem label="AR" value={beatmap?.approachRate as string} />
                <StatItem label="OD" value={beatmap?.overallDifficulty as string} />
                <StatItem label="BPM" value={beatmap?.bpm as string} />
                <StatItem
                    label="SR"
                    value={`${parseFloat(beatmap?.starRating || "0").toFixed(2)}*`}
                    isHighlight
                />
            </Stack>
        </Box>
    );
};

const StatItem = ({ label, value, isHighlight }: { label: string, value: string | number, isHighlight?: boolean }) => (
    <Typography variant="caption" sx={{ fontSize: '0.75rem', fontWeight: isHighlight ? 'bold' : 'normal' }}>
        {label}: <Box component="span" sx={{ color: isHighlight ? '#ff66aa' : 'inherit' }}>{value}</Box>
    </Typography>
);

export default BeatmapBanner;