<script setup>
import ItemPage from '../components/ItemPage.vue';
import NpcPage from '../components/NpcPage.vue';
</script>

<script>
export default {
  name: 'HSNRPage',
  data() {
    return {
      title: 'DataNRO',
      servers : [
        "Server1",
        "Server2",
        "Server3",
      ],
      currentPage: new URL(window.location.href).searchParams.get('page') || 'items'
    }
  },
  methods: {
    updateDefaultPage()
    {
      if (this.currentPage === null)
      {
        window.history.pushState({}, '', window.location.href + '?page=items');
        this.currentPage = 'items';
      }
    },
    handlePushState() {
      console.log('handlePushState');
      this.currentPage = new URL(window.location.href).searchParams.get('page') || 'items';
    }
  },
  mounted() {    
    window.addEventListener('pushstate', this.handlePushState);
    this.updateDefaultPage();
  },
  beforeDestroy() {
    window.removeEventListener('pushstate', this.handlePushState);
  }
};
</script>

<template>
  <div>
    <!-- <ItemPage v-if="currentPage === 'items'" :servers="servers" /> -->
    <NpcPage v-if="currentPage === 'npcs'" />
    <div v-else-if="currentPage === 'skills'"></div>
    <div v-else-if="currentPage === 'mobs'"></div>
    <div v-else-if="currentPage === 'itemOptions'"></div>
    <div v-else-if="currentPage === 'maps'"></div>
    <div v-else-if="currentPage === 'parts'"></div>
    <ItemPage v-else :servers="servers" />
  </div>
</template>

<style scoped>
</style>
