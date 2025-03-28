<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import SkillTemplate from './SkillTemplate.vue';
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
          <h1>{{ t('skills') }}</h1>
          <a class="material-icons-round" :title="t('viewRaw')"
            :href="servers[selectedServerIndex - 1].id + '/NClasses.json'" target="_blank"
            style="color: unset !important;">open_in_new</a>
        </div>
        <h5>{{ t('lastUpdated') }}: {{ lastUpdated }}</h5>
      </div>
      <SelectServer :servers="servers" :defaultServerId="defaultServerId" @change-server="changeServer" />
    </div>
    <div class="searchBar">
      <SearchBar :placeholder="t('searchSkill')" @inputText="checkDeleteAll" @search="searchSkillTemplates" />
      <Sort @change-sort="changeSort" @inverse-sort="inverseSort" />
    </div>
    <div>
      <div class="skillTemplates">
        <SkillTemplate v-for="skillTemplates in visibleSkillTemplates" :classId=skillTemplates.classId
          :className=skillTemplates.className :id=skillTemplates.id :maxPoint=skillTemplates.maxPoint
          :manaUseType=skillTemplates.manaUseType :type=skillTemplates.type :icon=skillTemplates.icon
          :name=skillTemplates.name :description=skillTemplates.description :damInfo=skillTemplates.damInfo
          :skills=skillTemplates.skills class="hoverable" />
      </div>
    </div>
    <LoadMore v-if="filteredSkillTemplates.length > 10 && visibleSkillTemplates.length < filteredSkillTemplates.length"
      @load-more="loadMore" @load-all="loadAll" />
  </div>
</template>

<script>
export default {
  components: {
    SkillTemplate,
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
      skillTemplates: [],
      filteredSkillTemplates: [],
      visibleSkillTemplates: [],
      currentSort: 'id',
      selectedServerIndex: 1,
      lastUpdated: '',
    }
  },
  methods: {
    async getSkillTemplates() {
      let response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/NClasses.json');
      let data = await response.json();
      //map this to skillTemplates
      let mappedData = [];
      data.forEach(e => {
        e['skillTemplates'].forEach(s => {
          mappedData.push({
            classId: e['classId'],
            className: e['name'],
            id: s['id'],
            maxPoint: s['maxPoint'],
            manaUseType: s['manaUseType'],
            type: s['type'],
            icon: s['icon'],
            name: s['name'],
            description: s['description'],
            damInfo: s['damInfo'],
            skills: s['skills'],
          });
        });
      });
      mappedData.sort((a, b) => a.id - b.id);
      this.skillTemplates = mappedData;
      this.filteredSkillTemplates = [...mappedData];
      if (this.reversed)
        this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
      response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/LastUpdated');
      data = await response.text();
      let date = new Date(data);
      this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';
      await new Promise(resolve => setTimeout(resolve, 1000));
      this.loading = false;
    },
    loadMore() {
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, this.visibleSkillTemplates.length + 10);
    },
    loadAll() {
      this.visibleSkillTemplates = this.filteredSkillTemplates;
    },
    changeSort(e) {
      this.currentSort = e.target.value;
      this.sortSkillTemplates();
    },
    sortSkillTemplates() {
      switch (this.currentSort) {
        case 'id':
          this.filteredSkillTemplates.sort((a, b) => a.id - b.id);
          break;
        case 'name':
          this.filteredSkillTemplates.sort((a, b) => a.name.localeCompare(b.name));
          break;
        case 'icon':
          this.filteredSkillTemplates.sort((a, b) => a.icon - b.icon);
          break;
      }
      if (this.reversed)
        this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
    },
    checkDeleteAll(e) {
      const search = e.target.value.toLowerCase();
      if (search === '') {
        this.filteredSkillTemplates = [...this.skillTemplates];
        this.sortSkillTemplates();
        return;
      }
    },
    searchSkillTemplates(e) {
      const search = this.replaceVietnameseChars(e.target.value.toLowerCase());
      this.filteredSkillTemplates = this.skillTemplates.filter(skillTemplate => this.replaceVietnameseChars((skillTemplate.name + '|' + skillTemplate.description + '|' + skillTemplate.damInfo + '|' + skillTemplate.id).toLowerCase()).includes(search));
      if (this.reversed)
        this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
    },
    inverseSort(e) {
      this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
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
      this.getSkillTemplates();
    },
  },
  mounted() {
    let index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
    if (index !== -1)
      this.selectedServerIndex = index + 1;
    else
      this.selectedServerIndex = 1;
    moment.locale(navigator.language);
    this.getSkillTemplates();
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

.skillTemplates {
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