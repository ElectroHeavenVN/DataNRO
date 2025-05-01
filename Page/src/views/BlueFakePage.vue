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
        {
          id: "Blue1",
          name: "blue1"
        }
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
  <!-- <ItemPage v-if="currentPage === 'items'" :servers="servers" /> -->
  <NpcsPage v-if="currentPage === 'npcs'" :servers="servers" />
  <SkillsPage v-else-if="currentPage === 'skills'" :servers="servers" />
  <MonstersPage v-else-if="currentPage === 'mobs'" :servers="servers" />
  <ItemOptionsPage v-else-if="currentPage === 'itemOptions'" :servers="servers" />
  <MapsPage v-else-if="currentPage === 'maps'" :servers="servers" />
  <PartsPage v-else-if="currentPage === 'parts'" :servers="servers" />
  <ItemsPage v-else :servers="servers" />
</template>