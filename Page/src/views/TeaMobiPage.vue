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
          id: "Server1",
          name: "server1"
        },
        {
          id: "Server2",
          name: "server2"
        },
        {
          id: "Server3",
          name: "server3"
        },
        {
          id: "Server4",
          name: "server4"
        },
        {
          id: "Server5",
          name: "server5"
        },
        {
          id: "Server6",
          name: "server6"
        },
        {
          id: "Server7",
          name: "server7"
        },
        {
          id: "Server8910",
          name: "server8910"
        },
        {
          id: "Server11",
          name: "server11"
        },
        {
          id: "Server12",
          name: "server12"
        },
        {
          id: "Server13",
          name: "server13"
        },
        {
          id: "Server14",
          name: "server14"
        },
        {
          id: "Super1",
          name: "super1"
        },
        {
          id: "Super2",
          name: "super2"
        },
        {
          id: "Universe1",
          name: "universe1"
        },
        {
          id: "Naga",
          name: "naga"
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
    <NpcsPage v-if="currentPage === 'npcs'" :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <SkillsPage v-else-if="currentPage === 'skills'" :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <MonstersPage v-else-if="currentPage === 'mobs'" :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <ItemOptionsPage v-else-if="currentPage === 'itemOptions'" :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <MapsPage v-else-if="currentPage === 'maps'" :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <PartsPage v-else-if="currentPage === 'parts'" :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
    <ItemsPage v-else :servers="servers" :defaultServerId="currentLang == 'vi' ? 'Server1' : 'Universe1'" />
  </div>
</template>