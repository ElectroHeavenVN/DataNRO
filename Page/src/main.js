import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import i18n from './i18n'

let lang = navigator.language.split('-')[0];
if (lang === 'vi')
    document.documentElement.lang = navigator.language;
else
    document.documentElement.lang = 'en';

window.history.pushState = new Proxy(window.history.pushState, {
    apply: (target, thisArg, argArray) => {
        let result = target.apply(thisArg, argArray);
        window.dispatchEvent(new Event('pushstate'));
        return result;
    },
    });

createApp(App)
    .use(i18n)
    .mount('#app')
