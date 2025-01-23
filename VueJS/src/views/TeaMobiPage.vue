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
        "Server4",
        "Server5",
        "Server6",
        "Server7",
        "Server8910",
        "Server11",
        "Server12",
        "Server13",
        "Server14",
        "Super1",
        "Super2",
        "Universe1",
        "Naga",
      ],
      currentPage: new URL(window.location.href).searchParams.get('page') || 'defaultPage'
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
  computed: {
    currentLang() {
      return navigator.language.split('-')[0];
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
    <ItemPage v-else :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
  </div>
</template>

<style scoped>
</style>
