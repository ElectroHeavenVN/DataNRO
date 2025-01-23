<script setup>
import { useI18n } from 'vue-i18n'
const {t} = useI18n();
</script>

<template>
  <div>
    <h1>{{ t('pageNotFound') }}</h1>
    <p>{{ t('pageNotFoundDesc') }}</p>
    <a href="/">{{ t('goBackToHomepage') }}</a>
  </div>
</template>

<style scoped>
h1 {
  color: red;
}
</style>