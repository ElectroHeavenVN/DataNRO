<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<script>
export default {
  name: 'SelectGamePublisherPage',
};
</script>

<template>
  <div class="selectPublisher">
    <h1>{{ t('selectPublisher') }}</h1>
    <div class="publishers">
      <a class="hoverable publisher" href="/TeaMobi/">
        <img src="../assets/teamobi.png" alt="TeaMobi" />
        <span>{{ t('teaMobiName') }}</span>
      </a>
      <a class="hoverable publisher" href="/HSNR/">
        <img src="../assets/hsnr.png" alt="HSNR" />
        <span>{{ t('hsnrName') }}</span>
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
  padding: 20px;
  border-radius: 10px;
  background-color: #1c1a23;
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
  font-size: 1.5rem;
  margin-top: 10px;
}

</style>
