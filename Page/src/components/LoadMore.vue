<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div style="display: flex; justify-content: center; padding: 50px 50px 30px 50px;">
    <div class="load-more hoverable" @click="loadMore">
      <span class="material-icons-round" style="font-size: 2rem;">keyboard_arrow_down</span>
      <p style="margin: 0; font-size: 1.5rem;">{{ t('loadMore') }}</p>
    </div>
  </div>
</template>

<script>
export default {
  methods: {
    loadMore() {
      this.$emit('load-more');
    }
  }
}
</script>

<style scoped>

.load-more {
  display: flex;
  justify-content: center;
  cursor: pointer;
  font-size: 20px;
  flex-direction: row;
  gap: 10px;
  box-shadow: 0 0 10px 1px aqua;
  border-radius: 10px;
  width: 50%;
  user-select: none;
}

@media screen and (max-width: 700px) {
  .load-more {
    width: 100%;
  }
}

</style>