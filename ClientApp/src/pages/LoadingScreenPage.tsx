import { Box, CircularProgress } from '@mui/material';

const LoadingScreenPage = () => {
    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                backgroundColor: '#0c0c0c',
                minHeight: '100vh',
                width: '100vw',
                position: 'fixed',
                top: 0,
                left: 0,
                zIndex: 9999,
            }}
        >
            <CircularProgress
                size={60}
                thickness={4}
                sx={{
                    color: '#5b89ff',
                    filter: 'drop-shadow(0px 0px 10px rgba(91, 137, 255, 0.3))'
                }}
            />
        </Box>
    );
};

export default LoadingScreenPage;