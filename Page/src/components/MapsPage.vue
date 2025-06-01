<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import Map from './Map.vue';
import LoadMore from './LoadMore.vue';
import PageHeader from './PageHeader.vue';
import LoadingPage from './LoadingPage.vue';
const { t } = useI18n();

</script>

<template>
  <LoadingPage v-if="loading" />
  <div v-else>
    <PageHeader :servers="servers" :selectedServerIndex="selectedServerIndex" :defaultServerId="defaultServerId"
      :jsonFileName="'Maps.json'" :placeholder="t('searchMap')" :lastUpdated="lastUpdated" @change-server="changeServer"
      @inputText="checkDeleteAll" @search="searchMap" @changeSort="changeSort" @inverseSort="inverseSort" />
    <div class="maps">
      <Map v-for="map in visibleMaps" :name=map.name :id=map.id class="hoverable" />
    </div>
    <LoadMore v-if="filteredMaps.length > 30 && visibleMaps.length < filteredMaps.length" @load-more="loadMore"
      @load-all="loadAll" />
  </div>
</template>

<script>
export default {
  components: {
    Map,
  },
  props: {
    servers: {
      type: Array,
      required: true,
    },
    defaultServerId: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      loading: true,
      reversed: false,
      maps: [],
      filteredMaps: [],
      visibleMaps: [],
      currentSort: 'id',
      selectedServerIndex: 1,
      lastUpdated: '',
    }
  },
  methods: {
    async getMaps() {
      let response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/Maps.json');
      let data = await response.json();
      this.maps = data;
      this.filteredMaps = [...data];
      if (this.reversed)
        this.filteredMaps.reverse();
      this.visibleMaps = this.filteredMaps.slice(0, 30);
      response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/LastUpdated');
      data = await response.text();
      let date = new Date(data);
      this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';
      await new Promise(resolve => setTimeout(resolve, 1000));
      this.loading = false;
    },
    loadMore() {
      this.visibleMaps = this.filteredMaps.slice(0, this.visibleMaps.length + 30);
    },
    loadAll() {
      this.visibleMaps = this.filteredMaps;
    },
    changeSort(e) {
      this.currentSort = e.target.value;
      this.sortMaps();
    },
    sortMaps() {
      switch (this.currentSort) {
        case 'id':
          this.filteredMaps.sort((a, b) => a.id - b.id);
          break;
        case 'name':
          this.filteredMaps.sort((a, b) => a.name.localeCompare(b.name));
          break;
      }
      if (this.reversed)
        this.filteredMaps.reverse();
      this.visibleMaps = this.filteredMaps.slice(0, 30);
    },
    checkDeleteAll(e) {
      const search = e.target.value.toLowerCase();
      if (search === '') {
        this.filteredMaps = [...this.maps];
        this.sortMaps();
        return;
      }
    },
    searchMap(e) {
      const search = this.replaceVietnameseChars(e.target.value.toLowerCase());
      this.filteredMaps = this.maps.filter(map => this.replaceVietnameseChars((map.name + '|' + map.id).toLowerCase()).includes(search));
      if (this.reversed)
        this.filteredMaps.reverse();
      this.visibleMaps = this.filteredMaps.slice(0, 30);
    },
    inverseSort(e) {
      this.filteredMaps.reverse();
      this.visibleMaps = this.filteredMaps.slice(0, 30);
      this.reversed = e.reversed;
    },
    replaceVietnameseChars(str) {
      return str.replace(/á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/g, 'a')
        .replace(/đ/g, 'd')
        .replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/g, 'e')
        .replace(/í|ì|ỉ|ĩ|ị/g, 'i')
        .replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/g, 'o')
        .replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/g, 'u')
        .replace(/ý|ỳ|ỷ|ỹ|ỵ/g, 'y');
    },
    changeServer(e) {
      this.selectedServerIndex = e.target.selectedIndex + 1;
      this.getMaps();
    },
  },
  mounted() {
    let index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
    if (index !== -1)
      this.selectedServerIndex = index + 1;
    else
      this.selectedServerIndex = 1;
    moment.locale(navigator.language);
    this.getMaps();
  },
};
</script>

<style scoped>
.maps {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}

@media screen and (max-width: 700px) {
  .searchBar {
    flex-wrap: wrap;
    gap: 20px;
  }
}
</style>