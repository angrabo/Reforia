import ReactDOM from "react-dom/client";
import App from "./App";
import "./App.css"
import {GlobalProvider} from "./Context/GlobalContext.tsx";
import {AlertProvider} from "./providers/AlertProvider.tsx";
import {ConfigProvider} from "./Context/ConfigContext.tsx";
import {CssBaseline, ThemeProvider} from "@mui/material";
import {theme} from "./utils/ThemeCreator.ts";
import {Suspense} from "react";
import './i18n';

document.addEventListener('contextmenu', (event) => {
    event.preventDefault();
});

document.addEventListener('focusin', (e) => {
    const target = e.target as HTMLInputElement;
    if (target && target.tagName === 'INPUT') {
        target.setAttribute('autocomplete', 'off');
    }
});

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
    <Suspense fallback="Loading translations...">
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            <GlobalProvider>
                <ConfigProvider>
                    <AlertProvider>
                        <App/>
                    </AlertProvider>
                </ConfigProvider>
            </GlobalProvider>
        </ThemeProvider>
    </Suspense>
);