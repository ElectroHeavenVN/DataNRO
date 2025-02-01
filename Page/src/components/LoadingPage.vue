<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div class="loading">
    <img :src="loadingImg" alt="loading">
    <span>{{ t('loading') }}...</span>
  </div>
</template>

<script>
export default {
  data() {
    return {
      loadingImg: "../Loading/1.png",
    }
  },
  mounted() {
    this.loadingImg = `../Loading/${Math.floor(Math.random() * 44) + 1}.png`;
  }
}
</script>

<style scoped>
.loading {
  font-size: 30px;
  user-select: none;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 80vh;
}

.loading img {
  height: 60vh;
  max-width: 100%;
  object-fit: contain;
}

.loading span {
  position: fixed;
  bottom: 100px;
  right: 70px;
}
</style>