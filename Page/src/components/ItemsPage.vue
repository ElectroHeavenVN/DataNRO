<script setup>
import { useI18n } from 'vue-i18n'
import Item from './Item.vue';
import moment from 'moment/min/moment-with-locales';
import LoadMore from './LoadMore.vue';
import LoadingPage from './LoadingPage.vue';
import PageHeader from './PageHeader.vue';
const { t } = useI18n();

</script>

<template>
  <LoadingPage v-if="loading" />
  <div v-else>
    <PageHeader :title="t('items')" :servers="servers" :selectedServerIndex="selectedServerIndex" :defaultServerId="defaultServerId"
      :jsonFileName="'ItemTemplates.json'" :placeholder="t('searchItem')" :lastUpdated="lastUpdated"
      @change-server="changeServer" @inputText="checkDeleteAll" @search="searchItem" @changeSort="changeSort"
      @inverseSort="inverseSort" />
    <div class="items">
      <Item v-for="item in visibleItems" :icon=item.icon :name=item.name :description=item.description :id=item.id
        :type=item.type :gender=item.gender :level=item.level
        :isNewItem="items.map(i => i.id).indexOf(item.id) > items.length - 50" :powerRequired=item.strRequire
        class="hoverable" />
    </div>
    <LoadMore v-if="filteredItems.length > 30 && visibleItems.length < filteredItems.length" @load-more="loadMore"
      @load-all="loadAll" :warning="true" />
  </div>
</template>

<script>
export default {
  components: {
    Item,
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
      items: [],
      filteredItems: [],
      visibleItems: [],
      currentSort: 'id',
      selectedServerIndex: 1,
      lastUpdated: '',
    }
  },
  methods: {
    async getItems() {
      let response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/ItemTemplates.json');
      let data = await response.json();
      this.items = data;
      this.filteredItems = [...data];
      if (this.reversed)
        this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
      response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/LastUpdated');
      data = await response.text();
      let date = new Date(data);
      this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';
      await new Promise(resolve => setTimeout(resolve, 1000));
      this.loading = false;
    },
    loadMore() {
      this.visibleItems = this.filteredItems.slice(0, this.visibleItems.length + 30);
    },
    loadAll() {
      this.visibleItems = this.filteredItems;
    },
    changeSort(e) {
      this.currentSort = e.target.value;
      this.sortItems();
    },
    sortItems() {
      switch (this.currentSort) {
        case 'id':
          this.filteredItems.sort((a, b) => a.id - b.id);
          break;
        case 'name':
          this.filteredItems.sort((a, b) => a.name.localeCompare(b.name));
          break;
        case 'icon':
          this.filteredItems.sort((a, b) => a.icon - b.icon);
          break;
      }
      if (this.reversed)
        this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
    },
    checkDeleteAll(e) {
      const search = e.target.value.toLowerCase();
      if (search === '') {
        this.filteredItems = [...this.items];
        this.sortItems();
        return;
      }
    },
    searchItem(e) {
      const search = this.replaceVietnameseChars(e.target.value.toLowerCase());
      this.filteredItems = this.items.filter(item => this.replaceVietnameseChars((item.name + '|' + item.description + '|' + item.id).toLowerCase()).includes(search));
      if (this.reversed)
        this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
    },
    inverseSort(e) {
      this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
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
      if (e.target.selectedIndex === this.selectedServerIndex - 1)
        return; 
      this.selectedServerIndex = e.target.selectedIndex + 1;
      this.getItems();
    },
  },
  mounted() {
    let index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
    if (index !== -1)
      this.selectedServerIndex = index + 1;
    else
      this.selectedServerIndex = 1;
    let serverFromURL = new URL(window.location.href).searchParams.get('server');
    if (serverFromURL) {
      let serverIndex = this.servers.map(s => s.id).indexOf(serverFromURL);
      if (serverIndex !== -1) {
        this.selectedServerIndex = serverIndex + 1;
      }
    }
    moment.locale(navigator.language);
    this.getItems();
  },
};
</script>

<style scoped>
.items {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}
</style>