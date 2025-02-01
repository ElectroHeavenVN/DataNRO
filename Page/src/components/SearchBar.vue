<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div class="search-bar">
    <span class="material-icons-round" style="font-size: 2rem;">search</span>
    <input type="text" :placeholder="placeholder" value="" @input="onInput" @change="onSearch" />
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
      this.$emit('input', event);
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
  width: 100%;
  padding: 15px;
  background-color: #1c1a23;
  border: none;
  border-radius: 10px;
  color: #fff;
  outline: none;
  font-size: 1rem;
}

</style>