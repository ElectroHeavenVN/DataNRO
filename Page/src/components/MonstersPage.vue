<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import Monster from './Monster.vue';
import LoadMore from './LoadMore.vue';
import PageHeader from './PageHeader.vue';
import LoadingPage from './LoadingPage.vue';
const { t } = useI18n();

</script>

<template>
  <LoadingPage v-if="loading" />
  <div v-else>
    <PageHeader :title="t('mobs')" :servers="servers" :selectedServerIndex="selectedServerIndex" :defaultServerId="defaultServerId"
      :jsonFileName="'MobTemplates.json'" :placeholder="t('searchMob')" :lastUpdated="lastUpdated"
      @change-server="changeServer" @inputText="checkDeleteAll" @search="searchMob" @changeSort="changeSort"
      @inverseSort="inverseSort" />
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
      if (e.target.selectedIndex === this.selectedServerIndex - 1)
        return; 
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
.mobs {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}
</style>