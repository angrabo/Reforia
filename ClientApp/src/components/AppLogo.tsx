import {Box} from "@mui/material";
import {COLORS} from "../utils/ThemeCreator.ts";

const AppLogo = () => {
    return (
        <Box sx={{
            p: 3,
            borderBottom: `1px solid ${COLORS.border}`,
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            gap: 3
        }}>
            <img src={"logo.png"} alt={"Reforia Logo"} width={120} height={120}/>
        </Box>
    )
}

export default AppLogo;