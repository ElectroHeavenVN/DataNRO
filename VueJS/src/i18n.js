import { createI18n } from 'vue-i18n';

const messages = {
    en: {
        thisIsNewItem: 'This is new item',
        clickToCopy: 'Click to copy',
        gender: 'Planet',
        type: 'Type',
        powerRequired: 'Power Required',
    },
    vi: {
        thisIsNewItem: 'Đây là vật phẩm mới',
        clickToCopy: 'Nhấn để sao chép',
        gender: 'Hệ',
        type: 'Loại',
        powerRequired: 'Sức mạnh yêu cầu',
    },
};

const browserLanguage = navigator.language.split('-')[0];
const defaultLanguage = ['en', 'vi'].includes(browserLanguage) ? browserLanguage : 'en';

const i18n = createI18n({
    locale: defaultLanguage, 
    fallbackLocale: 'en', 
    messages,
});

export default i18n;
