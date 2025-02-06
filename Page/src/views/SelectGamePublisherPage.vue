<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<script>
export default {
  computed: {
    currentLang() {
      return navigator.language.split('-')[0];
    }
  },
};
</script>

<template>
  <div class="selectPublisher">
    <h1>{{ t('selectPublisher') }}</h1>
    <div class="publishers">
      <a class="hoverable publisher" href="TeaMobi/">
        <img src="../assets/teamobi.png" v-if="currentLang == 'vi'" alt="TeaMobi" />
        <img src="../assets/teamobiEng.png" v-else alt="TeaMobi" />
        <span>{{ t('teaMobiName') }}</span>
      </a>
      <a class="hoverable publisher" href="HSNR/">
        <img src="../assets/hsnr.png" alt="HSNR" />
        <span>🏴&zwj;☠️{{ t('hsnrName') }}</span>
      </a>
    </div>
  </div>
</template>

<style scoped>
.selectPublisher {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-grow: 1;
  flex-direction: column;
}

.publishers {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-wrap: wrap;
}

.publisher {
  user-select: none;
  padding: 20px;
  border-radius: 10px;
  background-color: var(--component-bg);
  color: #fff;
  text-align: center;
  margin: 10px;
  display: flex;
  flex-direction: column;
  width: 400px;
  height: 325px;
  justify-content: space-between;
}

.publisher img {
  width: 100%;
  height: 100%; 
  object-fit: contain;
}

.publisher span {
  font-size: 1.4rem;
  margin-top: 10px;
}

@media screen and (max-width: 600px) {
  .publisher {
    width: 100%;
  }
  .publisher span {
    font-size: 1rem;
  }
}

</style>
