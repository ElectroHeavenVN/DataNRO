<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div class="sort">
    <div class="sort-icon" @click="inverseSort">
      <span class="material-icons-round" style="font-size: 2rem; position: relative; top: 3px; left: 2px;">straight</span>
      <span class="material-icons-round" style="position: relative; left: -8px;">sort</span>
    </div>
    <select @change="changeSort" :aria-label="t('selectServer')">
      <option value="id" selected="">ID</option>
      <option value="name">{{ t('name') }}</option>
      <option value="icon">Icon</option>
    </select>
  </div>
</template>

<script>
export default {
  methods: {
    changeSort(e) {
      this.$emit('change-sort', e);
    },
    inverseSort(e) {
      e.target.parentElement.style.transform = e.target.parentElement.style.transform === 'scale(1, -1)' ? 'none' : 'scale(1, -1)';
      this.$emit('inverse-sort', {reversed: e.target.parentElement.style.transform === 'scale(1, -1)'});
    }
  }
}
</script>

<style scoped>

.material-icons-round {
  font-size: 2.5rem;
}

select {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  background-color: var(--component-bg);
  padding: 15px;
  border-radius: 10px;
  outline: none;
  font-size: 1rem;
  width: 100px;
}

.sort {
  color: var(--component-color);
  display: flex;
  align-items: center;
  gap: 10px;
  justify-content: flex-end;
  margin-left: 10px;
}

.sort-icon {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-content: center;
  cursor: pointer;
  user-select: none;
  max-width: 50px;
}

@media screen and (max-width: 700px) {
  .sort {
    width: 100%;
    justify-content: flex-start;
    margin-left: 0px;
  }

  .sort select {
    width: 100%;
  }
}

</style>