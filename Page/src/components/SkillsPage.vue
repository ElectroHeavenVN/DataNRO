<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import SkillTemplate from './SkillTemplate.vue';
import LoadMore from './LoadMore.vue';
import PageHeader from './PageHeader.vue';
import LoadingPage from './LoadingPage.vue';
const { t } = useI18n();

</script>

<template>
  <LoadingPage v-if="loading" />
  <div v-else>
    <PageHeader :title="t('skills')" :servers="servers" :selectedServerIndex="selectedServerIndex" :defaultServerId="defaultServerId"
      :jsonFileName="'NClasses.json'" :placeholder="t('searchSkill')" :lastUpdated="lastUpdated"
      @change-server="changeServer" @inputText="checkDeleteAll" @search="searchSkillTemplates" @changeSort="changeSort"
      @inverseSort="inverseSort" />
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
      if (e.target.selectedIndex === this.selectedServerIndex - 1)
        return; 
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
.skillTemplates {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}
</style>