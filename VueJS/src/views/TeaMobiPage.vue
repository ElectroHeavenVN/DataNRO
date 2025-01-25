<script setup>
import ItemsPage from '../components/ItemsPage.vue';
import MonstersPage from '../components/MonstersPage.vue';
import NpcsPage from '../components/NpcsPage.vue';
import SkillsPage from '../components/SkillsPage.vue';
import ItemOptionsPage from '../components/ItemOptionsPage.vue';
import MapsPage from '../components/MapsPage.vue';
import PartsPage from '../components/PartsPage.vue';
</script>

<script>
export default {
  data() {
    return {
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
    <NpcsPage v-if="currentPage === 'npcs'" :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <SkillsPage v-else-if="currentPage === 'skills'" :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <MonstersPage v-else-if="currentPage === 'mobs'" :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <ItemOptionsPage v-else-if="currentPage === 'itemOptions'" :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <MapsPage v-else-if="currentPage === 'maps'" :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <PartsPage v-else-if="currentPage === 'parts'" :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <ItemsPage v-else :servers="servers" :defaultServer="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
  </div>
</template>