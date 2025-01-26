import { createI18n } from 'vue-i18n';
import en from './locales/en.json';
import vi from './locales/vi.json';

const messages = {
    en, vi 
};

const browserLanguage = navigator.language.split('-')[0];
const defaultLanguage = ['en', 'vi'].includes(browserLanguage) ? browserLanguage : 'en';

const i18n = createI18n({
    legacy: false,
    locale: defaultLanguage, 
    fallbackLocale: 'en', 
    messages,
});

export default i18n;
