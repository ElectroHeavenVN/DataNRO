<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div style="display: flex; justify-content: center; padding: 50px 50px 30px 50px;">
    <div class="load-more" @click="loadMore">
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
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  display: flex;
  justify-content: center;
  cursor: pointer;
  font-size: 20px;
  flex-direction: row;
  gap: 5px;
  background-color: var(--component-bg);
  border-radius: 10px;
  width: 50%;
  user-select: none;
  transition-duration: .2s;
  padding: 10px;
}

.load-more:hover {
  box-shadow: 0 0 10px 1px aqua;
}

.load-more * {
  position: relative;
  left: -4px;
}

@media screen and (max-width: 700px) {
  .load-more {
    width: 100%;
  }
}

</style>