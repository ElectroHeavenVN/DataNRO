<script setup>
import { useI18n } from 'vue-i18n'
import Sort from './Sort.vue';
import SearchBar from './SearchBar.vue';
import SelectServer from './SelectServer.vue';
const { t } = useI18n();
</script>

<template>
  <div class="title">
    <div>
      <div style="display: flex; flex-direction: row; gap: 5px; align-items: center;">
        <h1>{{ title }}</h1>
        <a class="material-icons-round" :title="t('viewRaw')"
          :href="servers[selectedServerIndex - 1].id + '/' + jsonFileName" target="_blank"
          style="color: unset !important;">open_in_new</a>
      </div>
      <h2>{{ t('lastUpdated') }}: {{ lastUpdated }}</h2>
    </div>
    <SelectServer :servers="servers" :defaultServerId="defaultServerId" @change-server="changeServer" />
  </div>
  <div class="searchBar">
    <SearchBar :placeholder="placeholder" :defaultValue="getQueryFromUrl()" @inputText="inputText" @search="search" />
    <Sort :defaultValue="getSortFromUrl()" @change-sort="changeSort" @inverse-sort="inverseSort" />
  </div>
</template>

<script>
export default {
  emits: ['changeServer', 'inputText', 'search', 'changeSort', 'inverseSort'],
  props: {
    title: {
      type: String,
      required: true,
    },
    servers: {
      type: Array,
      required: true,
    },
    selectedServerIndex: {
      type: Number,
      default: 1,
    },
    defaultServerId: {
      type: String,
      default: "",
    },
    jsonFileName: {
      type: String,
      required: true,
    },
    placeholder: {
      type: String,
      default: '',
    },
    lastUpdated: {
      type: String,
      default: '',
    },
  },
  methods: {
    getQueryFromUrl() {
      return new URL(window.location.href).searchParams.get('q') || '';
    },
    getSortFromUrl() {
      return new URL(window.location.href).searchParams.get('sort') || '';
    },
    changeServer(e) {
      this.$emit('changeServer', e);
    },
    inputText(e) {
      let url = new URL(window.location.href);
      if (!e?.target?.value) {
        url.searchParams.delete('q');
        window.history.pushState({}, '', url.toString());
      }
      this.$emit('inputText', e);
    },
    search(e) {
      let url = new URL(window.location.href);
      if (e?.target?.value) {
        url.searchParams.set('q', e.target.value);
      } else {
        url.searchParams.delete('q');
      }
      window.history.pushState({}, '', url.toString());
      this.$emit('search', e);
    },
    changeSort(e) {
      let url = new URL(window.location.href);
      url.searchParams.set('sort', e.target.value);
      window.history.pushState({}, '', url.toString());
      this.$emit('changeSort', e);
    },
    inverseSort(e) {
      this.$emit('inverseSort', e);
    },
  },
  mounted() {
    const query = this.getQueryFromUrl();
    if (query) {
      this.$emit('inputText', { target: { value: query } });
      this.$emit('search', { target: { value: query } });
    }
    const sort = this.getSortFromUrl();
    if (sort) {
      this.$emit('changeSort', { target: { value: sort } });
    }
  },
};
</script>

<style scoped>
.title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  margin-top: 30px;
  margin-bottom: 20px;
  gap: 20px;
}

.title h1,
.title h2 {
  margin: 0;
}

h2 {
  font-size: 1rem;
}

.searchBar {
  display: flex;
  justify-content: space-between;
  padding-bottom: 30px;
}

@media screen and (max-width: 700px) {
  .searchBar {
    flex-wrap: wrap;
    gap: 20px;
  }
}
</style>