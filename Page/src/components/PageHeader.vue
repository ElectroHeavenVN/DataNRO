<script setup>
import { useI18n } from 'vue-i18n'
import Sort from './Sort.vue';
import SearchBar from './SearchBar.vue';
import SelectServer from './SelectServer.vue';
const { t } = useI18n();
</script>

<template>
  <div>
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
      <SelectServer :servers="servers" :defaultValue="getServerIdFromUrl()" :defaultServerId="defaultServerId"
        @change-server="changeServer" />
    </div>
    <div class="searchBar">
      <SearchBar :placeholder="placeholder" :defaultValue="getQueryFromUrl()" @inputText="inputText" @search="search" />
      <Sort :defaultValue="getSortFromUrl()" :inverse="getSortInverseFromUrl()" @change-sort="changeSort"
        @inverse-sort="inverseSort" />
    </div>
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
  data() {
    return {
      firstTimeChangeServer: true,
    };
  },
  methods: {
    getQueryFromUrl() {
      return new URL(window.location.href).searchParams.get('q') || '';
    },
    getSortFromUrl() {
      return new URL(window.location.href).searchParams.get('sort') || '';
    },
    getSortInverseFromUrl() {
      return new URL(window.location.href).searchParams.get('inverse') === 'true';
    },
    getServerIdFromUrl() {
      return new URL(window.location.href).searchParams.get('server') || this.defaultServerId;
    },
    changeServer(e) {
      let url = new URL(window.location.href);
      let index = this.servers.map(s => s.id).indexOf(e?.target?.value);
      let defaultIndex = 0;
      if (this.defaultServerId !== "")
        defaultIndex = this.servers.map(s => s.id).indexOf(this.defaultServerId);
      if (index !== -1 && index !== defaultIndex) {
        url.searchParams.set('server', e.target.value);
      } else {
        url.searchParams.delete('server');
      }
      if (!this.firstTimeChangeServer) {
        url.searchParams.delete('q');
        window.history.pushState({}, '', url.toString());
        this.$nextTick(() => {
          this.$el.querySelector('.searchBar input').value = '';
        });
      }
      else {
        this.firstTimeChangeServer = false;
      }
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
      let url = new URL(window.location.href);
      const inverse = url.searchParams.get('inverse') === 'true';
      if (inverse) {
        url.searchParams.delete('inverse');
      } else {
        url.searchParams.set('inverse', 'true');
      }
      window.history.pushState({}, '', url.toString());
      this.$emit('inverseSort', e);
    },
  },
  mounted() {
    const serverId = this.getServerIdFromUrl();
    if (serverId) {
      const index = this.servers.map(s => s.id).indexOf(serverId);
      if (index !== -1) {
        this.$emit('changeServer', { target: { selectedIndex: index } });
      }
      else {
        let url = new URL(window.location.href);
        url.searchParams.delete('server');
        window.history.pushState({}, '', url.toString());
      }
    }
    const query = this.getQueryFromUrl();
    if (query) {
      this.$emit('inputText', { target: { value: query } });
      this.$emit('search', { target: { value: query } });
    }
    const sort = this.getSortFromUrl();
    if (sort) {
      this.$emit('changeSort', { target: { value: sort } });
    }
    const inverse = this.getSortInverseFromUrl();
    if (inverse) {
      this.$emit('inverseSort', { reversed: true });
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