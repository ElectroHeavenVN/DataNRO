<script setup>
import { useI18n } from 'vue-i18n'
import ItemPage from './components/ItemPage.vue';
const { t } = useI18n();

document.title += ' - Server ' + location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
</script>

<template>
  <ItemPage server="Universe1" />
</template>

<style>
.hoverable:hover {
    transform: translateY(-3px);
    box-shadow: 0 3px 10px #0003;
}

.hoverable {
    transition-duration: .2s;
}
</style>
