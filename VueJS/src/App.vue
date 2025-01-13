<script setup>
import { useI18n } from 'vue-i18n'
import Item from './components/Item.vue'
const { t } = useI18n();

document.title += ' - Server ' + location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
</script>

<template>
  <div v-if="loading">{{ t('loading') }}...</div>
  <div v-else class="items">
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
      />
  </div>
</template>

<script>
export default {
  components: {
    Item,
  },
  data() {
    return {
      loading: true,
      items: [],
      server: 'Server1'
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

<style>
.hoverable:hover {
    transform: translateY(-3px);
    box-shadow: 0 3px 10px #0003;
}

.hoverable {
    transition-duration: .2s;
}

.items {
  display: flex;
  flex-direction: column;
  gap: 25px;
}
</style>
