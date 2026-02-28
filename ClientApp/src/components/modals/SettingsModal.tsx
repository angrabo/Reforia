import {
    Button,
    TextField,
    FormControlLabel,
    Switch,
    Typography,
    Box,
    Divider,
    FormControl,
    InputLabel,
    Select,
    MenuItem, Tooltip
} from '@mui/material';
import {useConfigContext} from "../../Context/ConfigContext.tsx";
import {useGlobalContext} from "../../Context/GlobalContext.tsx";
import {WebResponse} from "../../models/types.ts";
import {signalRClient} from "../../utils/FunctionClient.ts";
import {useAlert} from "../../providers/AlertProvider.tsx";
import {ModalBase} from "./ModalBase.tsx";
import {COLORS} from "../../utils/ThemeCreator.ts";
import {Trans, useTranslation} from "react-i18next";
import {useState} from "react";
import {InfoOutlined as InfoOutlinedIcon} from "@mui/icons-material";

interface SettingsModalProps {
    open: boolean;
    onClose: () => void;
}

interface ChangeSettingsFunction {
    success: boolean;
}

export const SettingsModal = ({open, onClose}: SettingsModalProps) => {
    const {t, i18n} = useTranslation();
    const {showAlert} = useAlert();
    const {username, appVersion} = useGlobalContext();
    const {
        setApiToken, apiToken,
        userHighlight, setUserHighlight,
        alertOnMention, setAlertOnMention,
        alertOnKeyword, setAlertOnKeyword,
        highlightOnKeyword, setHighlightOnKeyword,
        keywordList, setKeywordList,
        showBeatmapBanner, setShowBeatmapBanner
    } = useConfigContext();

    const [localApiToken, setLocalApiToken] = useState(apiToken);
    const [localUserHighlight, setLocalUserHighlight] = useState(userHighlight);
    const [localLanguage, setLocalLanguage] = useState(i18n.language);
    const [localAlertOnMention, setLocalAlertOnMention] = useState(alertOnMention);
    const [localHighlightOnKeyword, setLocalHighlightOnKeyword] = useState(highlightOnKeyword);
    const [localAlertOnKeyword, setLocalAlertOnKeyword] = useState(alertOnKeyword);
    const [localKeywordList, setLocalKeywordList] = useState(keywordList.join(', '));
    const [localShowBeatmapBanner, setLocalShowBeatmapBanner] = useState(showBeatmapBanner);

    const handleSaveAndClose = async () => {
        const processedKeywords = localKeywordList
            .split(',')
            .filter(k => k !== "");

        const response: WebResponse<ChangeSettingsFunction> =
            await signalRClient.callFunction("ChangeSettingsFunction", {
                ApiToken: localApiToken,
                IsUserHighlighted: localUserHighlight,
                Language: localLanguage,
                AlertOnMention: localAlertOnMention,
                AlertOnKeyword: localAlertOnKeyword,
                HighlightOnKeyword: localHighlightOnKeyword,
                KeywordList: processedKeywords,
                ShowBeatmapBanner: localShowBeatmapBanner
            });

        if (response.body?.success) {
            setApiToken(localApiToken);
            setUserHighlight(localUserHighlight);
            setAlertOnMention(localAlertOnMention);
            setHighlightOnKeyword(localHighlightOnKeyword);
            setAlertOnKeyword(localAlertOnKeyword);
            setKeywordList(processedKeywords);
            setShowBeatmapBanner(localShowBeatmapBanner);
            i18n.changeLanguage(localLanguage);

            onClose();
        } else {
            showAlert(t('settings.alerts.saveError'), "error", 3000);
        }
    }

    const SectionHeader = ({title}: { title: string }) => (
        <Typography variant="overline" sx={{color: COLORS.osuPink, fontWeight: 'bold', mb: 1, display: 'block'}}>
            {title}
        </Typography>
    );

    return (
        <ModalBase open={open} onClose={onClose} title={t('settings.title')} maxWidth={500} sx={{p: 0}}>
            <Box sx={{display: 'flex', flexDirection: 'column', maxHeight: '75vh'}}>
                <Box sx={{
                    px: 3, py: 1, overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: 2,
                    '&::-webkit-scrollbar': {width: '6px'},
                    '&::-webkit-scrollbar-thumb': {backgroundColor: 'rgba(255,255,255,0.1)', borderRadius: '10px'}
                }}>

                    <Box>
                        <SectionHeader title={t('settings.sections.appearance')}/>
                        <FormControlLabel
                            control={<Switch checked={localUserHighlight}
                                             onChange={(e) => setLocalUserHighlight(e.target.checked)}
                                             color="primary"/>}
                            label={<Typography
                                variant="body2">{t('settings.labels.highlightSelf', {name: username})}</Typography>}
                        />
                        <FormControlLabel
                            control={<Switch checked={localShowBeatmapBanner}
                                             onChange={(e) => setLocalShowBeatmapBanner(e.target.checked)}
                                             color="primary"/>}
                            label={<Typography variant="body2">{t('settings.labels.showBeatmapBanner')}</Typography>}
                        />
                    </Box>

                    <Divider sx={{opacity: 0.1}}/>

                    <Box sx={{display: 'flex', flexDirection: 'column', gap: 1}}>
                        <SectionHeader title={t('settings.sections.notifications')}/>
                        <FormControlLabel
                            control={<Switch checked={localAlertOnMention}
                                             onChange={(e) => setLocalAlertOnMention(e.target.checked)}
                                             color="primary"/>}
                            label={<Typography variant="body2">{t('settings.labels.alertOnMention')}</Typography>}
                        />
                        <FormControlLabel
                            control={<Switch checked={localHighlightOnKeyword}
                                             onChange={(e) => setLocalHighlightOnKeyword(e.target.checked)}
                                             color="primary"/>}
                            label={<Typography variant="body2">{t('settings.labels.highlightOnKeyword')}</Typography>}
                        />
                        <FormControlLabel
                            control={<Switch checked={localAlertOnKeyword}
                                             onChange={(e) => setLocalAlertOnKeyword(e.target.checked)}
                                             color="primary"/>}
                            label={<Typography variant="body2">{t('settings.labels.alertOnKeyword')}</Typography>}
                        />
                        <TextField
                            label={t('settings.labels.keywordList')}
                            placeholder="key1, key2, key3"
                            fullWidth
                            size="small"
                            variant="outlined"
                            value={localKeywordList}
                            onChange={(e) => setLocalKeywordList(e.target.value)}
                            helperText={
                                <Tooltip
                                    title={
                                        <Box sx={{ p: 0.5,  fontSize: '0.8rem' }}>
                                            <Trans
                                                i18nKey="settings.labels.keywordHelper"
                                                components={{
                                                    bold: <strong style={{color: COLORS.highlight, fontWeight: 700}}/>
                                                }}
                                            />
                                        </Box>
                                    }
                                    arrow
                                    placement="top"
                                >
                                    <Box component={"span"} sx={{display: 'flex', alignItems: 'center'}}>
                                        {t('settings.labels.keywordHelperInfo')}
                                        <InfoOutlinedIcon sx={{ fontSize: '1.1rem', color: COLORS.textSub, cursor: 'help', ml: 2 }} />
                                    </Box>
                                </Tooltip>
                            }
                            sx={{mt: 1}}
                        />
                    </Box>

                    <Divider sx={{opacity: 0.1}}/>

                    <Box>
                        <SectionHeader title={t('settings.sections.integrations')}/>
                        <TextField
                            label={t('settings.labels.apiToken')}
                            type="password"
                            fullWidth
                            size="small"
                            value={localApiToken || ""}
                            onChange={(e) => setLocalApiToken(e.target.value)}
                        />
                    </Box>

                    <Divider sx={{opacity: 0.1, m: 2}}/>
                    <Box>
                        <SectionHeader title={t('settings.sections.language')}/>
                        <FormControl fullWidth size="small">
                            <InputLabel>{t('settings.labels.language')}</InputLabel>
                            <Select
                                value={localLanguage || "en"}
                                label={t('settings.labels.language')}
                                onChange={(e) => setLocalLanguage(e.target.value)}
                            >
                                <MenuItem value="pl">{t('languages.pl')}</MenuItem>
                                <MenuItem value="en">{t('languages.en')}</MenuItem>
                            </Select>
                        </FormControl>
                    </Box>

                    <Divider sx={{opacity: 0.1, m: 2}}/>

                    <Box sx={{pb: 1, display: 'flex', justifyContent: 'space-between'}}>
                        <Typography variant="caption"
                                    sx={{color: COLORS.textSub}}>{t('settings.labels.version')}</Typography>
                        <Typography variant="caption" sx={{fontWeight: 'bold'}}>{appVersion}</Typography>
                    </Box>
                </Box>

                <Box sx={{
                    p: 2,
                    px: 3,
                    borderTop: `1px solid rgba(255,255,255,0.08)`,
                    display: 'flex',
                    justifyContent: 'flex-end',
                    bgcolor: COLORS.surface1
                }}>
                    <Button onClick={handleSaveAndClose} variant="contained">
                        {t('settings.actions.save')}
                    </Button>
                </Box>
            </Box>
        </ModalBase>
    );
};