<script setup>
import { useI18n } from 'vue-i18n'
import Item from './Item.vue';
const { t } = useI18n();
</script>

<template>
  <div v-if="loading">{{ t('loading') }}...</div>
  <div v-else>
    <h1>{{ t('items') }}</h1>
    <div class="items">
      <Item
        v-for="(item, index) in items"
        :key=item.id
        :icon=item.icon
        :name=item.name
        :description=item.description
        :id=item.id
        :type=item.type
        :gender=item.gender
        :level=item.level
        :isNewItem="index > items.length - 50"
        :powerRequired=item.strRequire
        class="hoverable"
        />
    </div>
  </div>
</template>

<script>
export default {
  components: {
    Item,
  },
  props: {
    server: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      items: [],
    }
  },
  methods: {
    async getItems() {
      const response = await fetch(this.server + '/ItemTemplates.json');
      let data = await response.json();
      data = data.slice(data.length - 100);
      //data = data.slice(50, 100);
      this.items = data;
      this.loading = false;
    },
  },
  mounted() {
    this.getItems();
  },
};
</script>

<style scoped>
.items {
  display: flex;
  flex-direction: column;
  gap: 25px;
}
</style>