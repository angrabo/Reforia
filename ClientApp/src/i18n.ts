import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import translationPL from '../public/locales/pl/translation.json';
import translationEN from '../public/locales/en/translation.json';

i18n
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        resources: {
            pl: { translation: translationPL },
            en: { translation: translationEN },
        },

        fallbackLng: 'en',
        supportedLngs: ['pl', 'en'],

        detection: {
            order: ['localStorage', 'navigator'],
            caches: ['localStorage'],
        },

        interpolation: {
            escapeValue: false,
        },

        debug: true,
    });

export default i18n;