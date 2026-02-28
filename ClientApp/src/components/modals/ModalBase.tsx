import React, {ReactNode} from 'react';
import {Modal, Box, Typography, IconButton, SxProps, Theme, ModalProps} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import {COLORS} from "../../utils/ThemeCreator.ts";

interface ModalBaseProps extends Omit<ModalProps, 'children'> {
    title?: string;
    children: ReactNode;
    onClose: () => void;
    maxWidth?: number | string;
    sx?: SxProps<Theme>;
}

const baseBoxStyle: SxProps<Theme> = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    bgcolor: COLORS.surface1,
    boxShadow: 24,
    borderRadius: 2,
    display: 'flex',
    flexDirection: 'column',
    outline: 'none',
    maxHeight: '90vh'
};

export const ModalBase: React.FC<ModalBaseProps> = ({open, onClose, title, children, maxWidth = 400, sx}) => {
    return (
        <Modal open={open} onClose={onClose}>
            <Box sx={{...baseBoxStyle, width: maxWidth, ...sx}}>
                <Box sx={{ p: 3, pb: 1, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Typography variant="h6" component="h2">{title}</Typography>
                    <IconButton onClick={onClose} size="small"><CloseIcon/></IconButton>
                </Box>

                <Box sx={{
                    flexGrow: 1,
                    overflowY: 'auto',
                    px: 3,
                    pb: 2
                }}>
                    {children}
                </Box>
            </Box>
        </Modal>
    );
};