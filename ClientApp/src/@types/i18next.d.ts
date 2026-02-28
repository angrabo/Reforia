import 'i18next';
import translation from '../public/locales/pl/translation.json';

declare module 'i18next' {
    interface CustomTypeOptions {
        defaultNS: 'translation';
        resources: {
            translation: typeof translation;
        };
    }
}