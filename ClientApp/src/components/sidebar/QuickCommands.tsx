import {Box, Button, Grid, Typography} from "@mui/material";
import {useChat} from "../../Context/ChatContext.tsx";
import {labelStyle} from "../../utils/ThemeCreator.ts";
import {useTranslation} from "react-i18next";

const QuickCommands = () => {

    const {activeChat, chats, sendMessage} = useChat();
    const { t } = useTranslation();

    const currentChat = activeChat ? chats[activeChat] : null;

    const sx = {
        fontSize: '0.75rem',
    }

    const handleStart = async () => {
        if (!currentChat) return;
        sendMessage(`!mp start 10`);
    }

    const handleTimer = async () => {
        if (!currentChat) return;
        sendMessage(`!mp timer 120`);
    }

    const handleAbort = async () => {
        if (!currentChat) return;
        sendMessage(`!mp abort`);
    }

    const handleClose = async () => {
        if (!currentChat) return;
        sendMessage(`!mp close`);
    }
    return (
        <Box sx={{mt: -2}}>
            <Typography sx={labelStyle}>{t('sidebar.sections.commands')}</Typography>
            <Grid container spacing={1}>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" onClick={handleStart} disableElevation sx={sx}>{t('sidebar.commands.start')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" onClick={handleTimer} disableElevation sx={sx}>{t('sidebar.commands.timer')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" onClick={handleAbort} disableElevation sx={sx}>{t('sidebar.commands.abort')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" onClick={handleClose} disableElevation sx={sx}>{t('sidebar.commands.close')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" disabled disableElevation sx={sx}>{t('sidebar.commands.lazer')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" disabled disableElevation sx={sx}>{t('sidebar.commands.client')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" disabled disableElevation sx={sx}>{t('sidebar.commands.ref')}</Button>
                </Grid>
                <Grid size={6}>
                    <Button fullWidth variant="outlined" disabled disableElevation sx={sx}>{t('sidebar.commands.streamers')}</Button>
                </Grid>
            </Grid>
        </Box>
    );
};

export default QuickCommands;