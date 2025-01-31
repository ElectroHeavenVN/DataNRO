<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import SkillTemplate from './SkillTemplate.vue';
const { t } = useI18n();

</script>

<template>
  <div v-if="loading" class="loading">{{ t('loading') }}...</div>
  <div v-else>
    <div class="title">
      <div>
        <h1>{{ t('skills') }}</h1>
        <h5>{{ t('lastUpdated') }}: {{ lastUpdated }}</h5>
      </div>
      <div class="select-server">
        <span style="white-space: nowrap;">{{ t('selectServer') }}</span>
        <select @change="changeServer">
          <option v-for = "server in servers" :value="server.id">{{ t(server.name.toLowerCase()) }}</option>
        </select>
      </div>
    </div>
    <div class="searching">
      <div class="search-bar">
        <span class="material-icons-round" style="font-size: 2rem;">search</span>
        <input type="text" :placeholder="t('searchSkill')" value="" @input="checkDeleteAll" @change="searchSkillTemplates" />
      </div>
      <div class="sort">
        <span class="material-icons-round" style="font-size: 2rem; transform: scale(1, -1); cursor: pointer;" @click="inverseSort">sort</span>
          <select @change="changeSort">
            <option value="id" selected="">ID</option>
            <option value="name">{{ t('name') }}</option>
            <option value="icon">Icon</option>
          </select>
        </div>
    </div>
    <div>
      <div class="skillTemplates">
        <SkillTemplate v-for="skillTemplates in visibleSkillTemplates" 
          :classId=skillTemplates.classId
          :className=skillTemplates.className
          :id=skillTemplates.id 
          :maxPoint=skillTemplates.maxPoint
          :manaUseType=skillTemplates.manaUseType
          :type=skillTemplates.type
          :icon=skillTemplates.icon
          :name=skillTemplates.name
          :description=skillTemplates.description 
          :damInfo=skillTemplates.damInfo
          :skills=skillTemplates.skills 
          class="hoverable" />
      </div>
    </div>
    <div class="load-more hoverable" v-if="filteredSkillTemplates.length > 10 && visibleSkillTemplates.length < filteredSkillTemplates.length" @click="loadMore">
      <span class="material-icons-round" style="font-size: 2rem;">keyboard_arrow_down</span>
      <p style="margin: 0; font-size: 1.5rem;">{{ t('loadMore') }}</p>
    </div>
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
      this.skillTemplates = mappedData;
      this.filteredSkillTemplates = [...mappedData];
      if (this.reversed) 
        this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
      response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/LastUpdated');
      data = await response.text();
      let date = new Date(data);
      this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';
      this.loading = false;
    },
    loadMore() {
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, this.visibleSkillTemplates.length + 10);
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
        if (this.reversed) 
          this.filteredSkillTemplates.reverse();
        this.sortSkillTemplates();
        return;
      }
    },
    searchSkillTemplates(e) {
      const search =  this.replaceVietnameseChars(e.target.value.toLowerCase());
      this.filteredSkillTemplates = this.skillTemplates.filter(skillTemplate => this.replaceVietnameseChars((skillTemplate.name + '|' + skillTemplate.description + '|' + skillTemplate.damInfo + '|' + skillTemplate.id).toLowerCase()).includes(search));
      if (this.reversed) 
        this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
    },
    inverseSort(e) {
      this.filteredSkillTemplates.reverse();
      this.visibleSkillTemplates = this.filteredSkillTemplates.slice(0, 10);
      e.target.style.transform = e.target.style.transform === 'scale(1, -1)' ? 'scale(1, 1)' : 'scale(1, -1)';
      this.reversed = e.target.style.transform === 'scale(1, 1)';
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
    this.getSkillTemplates().then(() => {
      document.querySelector(".select-server select").selectedIndex = this.selectedServerIndex - 1;
    });
  },
};
</script>

<style scoped>
.loading {
  width: 100%;
  left: 0px;
  display: flex;
  justify-content: center;
  align-items: center;
  position: absolute;
  top: calc(50% - 50px);
  font-size: 30px;
}

.material-icons-round {
  user-select: none;
}

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

.title span {
  font-size: 1.25rem;
  font-weight: bold;
}

.title select {
  font-size: 1.25rem;
  width: 200px;
  padding: 10px;
  height: fit-content;
}

.select-server {
  display: flex; 
  flex-direction: row;
  gap: 20px;
  align-items: center;
}

.skillTemplates {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}

.load-more {
  display: flex;
  justify-content: center;
  margin: 50px 500px 30px 500px;
  cursor: pointer;
  font-size: 20px;
  flex-direction: row;
  gap: 10px;
  box-shadow: 0 0 10px 1px aqua;
  border-radius: 10px;
}

.searching {
  display: flex;
  justify-content: space-between;
  padding-bottom: 30px;
}

.search-bar {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 1rem;
}

.search-bar input {
  width: 100%;
  padding: 15px;
  background-color: #1c1a23;
  border: none;
  border-radius: 10px;
  color: #fff;
  outline: none;
  font-size: 1rem;
}

.sort {
  display: flex;
  align-items: center;
  gap: 10px;
  justify-content: flex-end;
  margin-left: 15px;
}

select {
  padding: 15px;
  background-color: #1c1a23;
  border: none;
  border-radius: 10px;
  color: #fff;
  outline: none;
  font-size: 1rem;
  width: 100px;
}

@media screen and (max-width: 700px) {
  .searching {
    flex-wrap: wrap;
    gap: 20px;
  }

  .sort {
    width: 100%;
    justify-content: flex-start;
    margin-left: 0px;
  }

  .select-server {
    width: 100%;
  }

  .select-server select,
  .sort select {
    width: 100%;
  }
}
</style>