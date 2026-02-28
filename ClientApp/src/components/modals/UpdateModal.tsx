import { useState } from 'react';
import { Typography, Box, LinearProgress, Button } from '@mui/material';
import { relaunch } from "@tauri-apps/plugin-process";
import { Update } from "@tauri-apps/plugin-updater";
import { useGlobalContext } from "../../Context/GlobalContext.tsx";
import { ModalBase } from "./ModalBase";
import {COLORS} from "../../utils/ThemeCreator.ts";
import {Trans, useTranslation} from "react-i18next";

interface UpdateModalProps {
    open: boolean;
    update: Update | null;
    onClose: () => void;
}

export const UpdateModal = ({ open, update, onClose }: UpdateModalProps) => {
    const [isDownloading, setIsDownloading] = useState(false);
    const [downloadProgress, setDownloadProgress] = useState(0);
    const { appVersion } = useGlobalContext();
    const {t} = useTranslation();

    if (!update) return null;

    const handleUpdate = async () => {
        setIsDownloading(true);
        let downloaded = 0;
        let contentLength = 0;

        try {
            await update.downloadAndInstall((event) => {
                switch (event.event) {
                    case 'Started':
                        contentLength = event.data.contentLength || 0;
                        break;
                    case 'Progress':
                        downloaded += event.data.chunkLength;
                        if (contentLength > 0) {
                            const percent = Math.round((downloaded / contentLength) * 100);
                            setDownloadProgress(percent);
                        }
                        break;
                    case 'Finished':
                        console.log('Download finished');
                        break;
                }
            });

            await relaunch();
        } catch (error) {
            console.error("Update failed:", error);
            setIsDownloading(false);
        }
    };

    return (
        <ModalBase
            open={open}
            onClose={isDownloading ? () => {} : onClose}
            title={t('update.title')}
            maxWidth={450}
            sx={{
                backgroundColor: COLORS.surface1,
                border: `1px solid ${COLORS.border}`,
                color: COLORS.textMain,
            }}
        >
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <Box>
                    <Typography variant="body2" sx={{ color: COLORS.textSub }}>
                        {t('update.message')}
                    </Typography>
                    <Box sx={{ mt: 2, display: 'flex', justifyContent: 'space-between' }}>
                        <Typography variant="caption" sx={{ color: COLORS.textSub }}>
                            <Trans
                                values={{ version: appVersion }}
                                i18nKey="update.currentVersion"
                                components={{
                                    bold: <strong style={{fontWeight: 700}}/>
                                }}
                            />
                        </Typography>
                        <Typography variant="caption" sx={{ color: COLORS.osuPink, fontWeight: 'bold' }}>
                            {t('update.newVersion', { version: update.version })}
                        </Typography>
                    </Box>
                </Box>

                {update.body && (
                    <Box sx={{
                        backgroundColor: COLORS.bgDark,
                        p: 1.5,
                        borderRadius: 1,
                        border: `1px solid ${COLORS.border}`,
                        maxHeight: '150px',
                        overflowY: 'auto'
                    }}>
                        <Typography variant="caption" sx={{ whiteSpace: 'pre-line', color: COLORS.textMain }}>
                            {update.body}
                        </Typography>
                    </Box>
                )}

                {isDownloading && (
                    <Box sx={{ width: '100%', mt: 1 }}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                            <Typography variant="caption" sx={{ color: COLORS.textSub }}>
                                {t('update.downloading')}
                            </Typography>
                            <Typography variant="caption" sx={{ color: COLORS.osuPink }}>
                                {downloadProgress}%
                            </Typography>
                        </Box>
                        <LinearProgress
                            variant="determinate"
                            value={downloadProgress}
                            sx={{
                                height: 8,
                                borderRadius: 4,
                                backgroundColor: COLORS.bgDark,
                                '& .MuiLinearProgress-bar': {
                                    backgroundColor: COLORS.osuPink,
                                }
                            }}
                        />
                    </Box>
                )}

                <Box sx={{
                    mt: 2,
                    pt: 2,
                    borderTop: `1px solid ${COLORS.border}`,
                    display: 'flex',
                    justifyContent: 'flex-end',
                    gap: 1
                }}>
                    {!isDownloading && (
                        <Button variant={"outlined"} onClick={onClose}>
                            {t('update.later')}
                        </Button>
                    )}
                    <Button
                        onClick={handleUpdate}
                        variant="contained"
                        disabled={isDownloading}
                    >
                        {isDownloading ? t('update.updating') : t(`update.barUpdateNow`)}
                    </Button>
                </Box>
            </Box>
        </ModalBase>
    );
};