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
      <a class="hoverable publisher" href="TeaMobi/" :title="t('officialServer')">
        <img src="../assets/teamobi.png" v-if="currentLang == 'vi'" alt="TeaMobi" />
        <img src="../assets/teamobiEng.png" v-else alt="TeaMobi" />
        <span>{{ t('teaMobiName') }}</span>
        <span style="font-size: 15px; margin: 0;">{{ t('officialServer') }}</span>
      </a>
      <a class="hoverable publisher" href="HSNR/" :title="t('privateServer')">
        <img src="../assets/hsnr.png" alt="HSNR" />
        <span>{{ t('hsnrName') }}🏴&zwj;☠️</span>
        <span style="font-size: 15px; margin: 0;">{{ t('privateServer') }}</span>
      </a>
      <a class="hoverable publisher" href="BlueFake/" :title="t('privateAndScamServer')">
        <img src="../assets/blue.png" alt="BlueFake" />
        <span>{{ t('nroBlueFakeName') }}🐓</span>
        <span style="font-size: 15px; margin: 0;">{{ t('privateAndScamServer') }}</span>
      </a>
      <a class="hoverable publisher" href="ILoveNRO/" :title="t('privateAndScamServer')">
        <img src="../assets/ilovenro.png" alt="ILoveNRO" />
        <span>{{ t('iLoveNROName') }}🐓</span>
        <span style="font-size: 15px; margin: 0;">{{ t('privateAndScamServer') }}</span>
      </a>
    </div>
    <a class="ghStars" href="https://github.com/ElectroHeavenVN/DataNRO/" target="_blank">
      {{ t('ghStars') }}
      <img src="https://img.shields.io/github/stars/ElectroHeavenVN/DataNRO?style=social" alt="GitHub stars">
    </a>
  </div>
</template>

<style scoped>
.selectPublisher {
  margin-top: 25px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-grow: 1;
  flex-direction: column;
}

.selectPublisher h1 {
  text-align: center;
}

.publishers {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-wrap: wrap;
  max-width: 1200px;
}

.publisher {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  user-select: none;
  padding: 20px;
  border-radius: 10px;
  background-color: var(--component-bg);
  color: var(--component-color);
  text-align: center;
  margin: 10px;
  display: flex;
  flex-direction: column;
  width: 400px;
  height: 325px;
  justify-content: space-between;
  text-decoration: none;
  font-weight: bold;
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

.ghStars {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  background-color: var(--component-bg);
  color: var(--component-color);
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  font-size: 20px;
  flex-direction: row;
  gap: 15px;
  border-radius: 10px;
  width: 50%;
  min-width: 420px;
  user-select: none;
  transition-duration: .2s;
  padding: 10px;
  margin-top: 50px;
  text-decoration: none;
  font-weight: bold;
  flex-wrap: wrap;
}

.ghStars:hover {
  box-shadow: 0 0 10px 1px orange;
}

.ghStars img {
  height: 30px;
}

@media screen and (max-width: 500px) {
  .ghStars {
    min-width: unset;
  }
}
@media screen and (max-width: 600px) {
  .publisher {
    width: 100%;
  }
  .publisher span {
    font-size: 1rem;
  }

  .ghStars {
    width: calc(100% - 40px);
    margin-top: 20px;
  }
}

</style>
