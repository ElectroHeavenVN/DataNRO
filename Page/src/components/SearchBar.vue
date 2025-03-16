<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div class="search-bar">
    <span class="material-icons-round" style="font-size: 2rem;">search</span>
    <input type="search" :placeholder="placeholder" @input="onInput" @change="onSearch" />
  </div>
</template>

<script>
export default {
  props: {
    placeholder: {
      type: String,
      default: '',
    },
  },
  methods: {
    onInput(event) {
      this.$emit('inputText', event);
    },
    onSearch(event) {
      this.$emit('search', event);
    },
  },
}
</script>

<style scoped>

.search-bar {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 1rem;
}

.search-bar input {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  width: 100%;
  padding: 15px;
  background-color: var(--component-bg);
  border-radius: 10px;
  color: var(--component-color);
  outline: none;
  font-size: 1rem;
}

</style>