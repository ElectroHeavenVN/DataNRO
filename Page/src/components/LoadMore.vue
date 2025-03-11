<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div style="display: flex; justify-content: center; padding: 50px 50px 30px 50px;">
    <div class="load-more" @click="loadMore" @mousedown="loadAllHold" @mouseup="loadAllCancel" @touchstart="loadAllHold"
      @touchend="loadAllCancel" @mouseleave="loadAllCancel" @touchcancel="loadAllCancel">
      <div class="load-more-text">
        <span class="material-icons-round" style="font-size: 2rem;">keyboard_arrow_down</span>
        <p style="margin: 0; font-size: 1.5rem;">{{ t('loadMore') }}</p>
      </div>
      <span style="font-size: 15px;">{{ t('loadMoreHold') }}</span>
      <span v-if="warning" style="font-size: 10px; text-decoration: underline;">({{ t('loadMoreWarning') }}!)</span>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    warning: {
      type: Boolean,
      required: false,
      default: false
    }
  },
  data() {
    return {
      holdTimeout: null,
      cancelLoadAll: false
    }
  },
  methods: {
    loadMore() {
      this.$emit('load-more');
    },
    loadAllHold() {
      this.holdTimeout = setTimeout(() => {
        if (this.cancelLoadAll) 
          return;
        this.$emit('load-all');
      }, 2000);
      this.cancelLoadAll = false;
    },
    loadAllCancel() {
      clearTimeout(this.holdTimeout);
      this.cancelLoadAll = true;
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
  flex-direction: column;
  align-items: center;
  cursor: pointer;
  font-size: 20px;
  background-color: var(--component-bg);
  border-radius: 10px;
  width: 50%;
  user-select: none;
  transition-duration: .2s;
  padding: 10px;
}

.load-more-text {
  display: flex;
  align-items: center;
  gap: 5px;
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