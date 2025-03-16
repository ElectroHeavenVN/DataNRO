<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import Monster from './Monster.vue';
import LoadMore from './LoadMore.vue';
import Sort from './Sort.vue';
import SearchBar from './SearchBar.vue';
import SelectServer from './SelectServer.vue';
import LoadingPage from './LoadingPage.vue';
const { t } = useI18n();

</script>

<template>
  <LoadingPage v-if="loading" />
  <div v-else>
    <div class="title">
      <div>
        <div style="display: flex; flex-direction: row; gap: 5px; align-items: center;">
          <h1>{{ t('mobs') }}</h1>
          <a class="material-icons-round" :title="t('viewRaw')"
            :href="servers[selectedServerIndex - 1].id + '/MobTemplates.json'" target="_blank"
            style="color: unset !important;">open_in_new</a>
        </div>
        <h5>{{ t('lastUpdated') }}: {{ lastUpdated }}</h5>
      </div>
      <SelectServer :servers="servers" :defaultServerId="defaultServerId" @change-server="changeServer" />
    </div>
    <div class="searchBar">
      <SearchBar :placeholder="t('searchMob')" @inputText="checkDeleteAll" @search="searchMob" />
      <Sort @change-sort="changeSort" @inverse-sort="inverseSort" />
    </div>
    <div class="mobs">
      <Monster v-for="mob in visibleMobs" :name=mob.name :id=mob.mobTemplateId :range=mob.rangeMove :speed=mob.speed
        :type=mob.type :dartType=mob.dartType :hp=mob.hp class="hoverable" />
    </div>
    <LoadMore v-if="filteredMobs.length > 30 && visibleMobs.length < filteredMobs.length" @load-more="loadMore"
      @load-all="loadAll" />
  </div>
</template>

<script>
export default {
  components: {
    Monster,
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
      mobs: [],
      filteredMobs: [],
      visibleMobs: [],
      currentSort: 'id',
      selectedServerIndex: 1,
      lastUpdated: '',
    }
  },
  methods: {
    async getMobs() {
      let response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/MobTemplates.json');
      let data = await response.json();
      this.mobs = data;
      this.filteredMobs = [...data];
      if (this.reversed)
        this.filteredMobs.reverse();
      this.visibleMobs = this.filteredMobs.slice(0, 30);
      response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/LastUpdated');
      data = await response.text();
      let date = new Date(data);
      this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';
      await new Promise(resolve => setTimeout(resolve, 1000));
      this.loading = false;
    },
    loadMore() {
      this.visibleMobs = this.filteredMobs.slice(0, this.visibleMobs.length + 30);
    },
    loadAll() {
      this.visibleMobs = this.filteredMobs;
    },
    changeSort(e) {
      this.currentSort = e.target.value;
      this.sortMobs();
    },
    sortMobs() {
      switch (this.currentSort) {
        case 'id':
          this.filteredMobs.sort((a, b) => a.id - b.id);
          break;
        case 'name':
          this.filteredMobs.sort((a, b) => a.name.localeCompare(b.name));
          break;
      }
      if (this.reversed)
        this.filteredMobs.reverse();
      this.visibleMobs = this.filteredMobs.slice(0, 30);
    },
    checkDeleteAll(e) {
      const search = e.target.value.toLowerCase();
      if (search === '') {
        this.filteredMobs = [...this.mobs];
        this.sortMobs();
        return;
      }
    },
    searchMob(e) {
      const search = this.replaceVietnameseChars(e.target.value.toLowerCase());
      this.filteredMobs = this.mobs.filter(mob => this.replaceVietnameseChars((mob.name + '|' + mob.id).toLowerCase()).includes(search));
      if (this.reversed)
        this.filteredMobs.reverse();
      this.visibleMobs = this.filteredMobs.slice(0, 30);
    },
    inverseSort(e) {
      this.filteredMobs.reverse();
      this.visibleMobs = this.filteredMobs.slice(0, 30);
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
      this.getMobs();
    },
  },
  mounted() {
    let index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
    if (index !== -1)
      this.selectedServerIndex = index + 1;
    else
      this.selectedServerIndex = 1;
    moment.locale(navigator.language);
    this.getMobs();
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
.title h5 {
  margin: 0;
}

.mobs {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}

.searchBar {
  display: flex;
  justify-content: space-between;
  padding-bottom: 30px;
}

select {
  padding: 15px;
  background-color: var(--component-bg);
  border: none;
  border-radius: 10px;
  color: #fff;
  outline: none;
  font-size: 1rem;
  width: 100px;
}

@media screen and (max-width: 700px) {
  .searchBar {
    flex-wrap: wrap;
    gap: 20px;
  }
}
</style>