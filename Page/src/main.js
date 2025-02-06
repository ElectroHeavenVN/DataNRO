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

(() => {
    let isShowArona = false, isShowPlana = false, lock = false;
    const btns = document.querySelectorAll('.go-fuck-ublock-origin-and-to-top');

    const handleScroll = () => {
        if (lock) return;
        btns.forEach(btn => {
            if (btn.classList.contains('arona')) {
                if (document.documentElement.scrollTop >= 200) {
                    if (!isShowArona) {
                        btn.classList.add('load');
                        isShowArona = true;
                    }
                } else if (isShowArona) {
                    btn.classList.remove('load');
                    isShowArona = false;
                }
            }
            else if (btn.classList.contains('plana')) {
                if (document.documentElement.scrollTop >= 200) {
                    if (!isShowPlana) {
                        btn.classList.add('load');
                        isShowPlana = true;
                    }
                } else if (isShowPlana) {
                    btn.classList.remove('load');
                    isShowPlana = false;
                }
            }

        });
    };
    window.addEventListener('scroll', handleScroll);

    btns.forEach(btn => {
        const handleClick = () => {
            lock = true;
            btns.forEach(btn => btn.classList.add('ani-leave'));
            window.scrollTo({ top: 0, behavior: 'smooth' });
            setTimeout(() => {
                btns.forEach(btn => {
                    btn.classList.remove('ani-leave');
                    btn.classList.add('left');
                });
            }, 300);

            setTimeout(() => btns.forEach(btn => btn.classList.remove('load')), 500);

            setTimeout(() => {
                lock = false;
                isShowArona = false;
                isShowPlana = false;
                btns.forEach(btn => btn.classList.remove('left'));
            }, 1000);
        };

        btn.addEventListener('click', handleClick);
    });
})();

createApp(App)
    .use(i18n)
    .mount('#app')
