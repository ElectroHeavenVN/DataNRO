<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import Npc from './Npc.vue';
const { t } = useI18n();

</script>

<template>
<div v-if="loading" class="loading">{{ t('loading') }}...</div>
  <div v-else>
    <div class="title">
      <div>
        <h1>{{ t('npcs') }}</h1>
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
        <span class="material-symbols-outlined" style="font-size: 2rem;">search</span>
        <input type="text" :placeholder="t('searchNPC')" value="" @input="checkDeleteAll" @change="searchNpc" />
      </div>
      <div class="sort">
        <span class="material-symbols-outlined" style="font-size: 2rem; transform: scale(1, -1); cursor: pointer;" @click="inverseSort">sort</span>
          <select @change="changeSort">
            <option value="id" selected="">ID</option>
            <option value="name">{{ t('name') }}</option>
          </select>
        </div>
    </div>
    <div class="npcs">
      <Npc v-for="npc in visibleNpcs" :name=npc.name :id=npc.npcTemplateId :head=npc.headId :body=npc.bodyId :legs=npc.legId class="hoverable" />
    </div>
    <div class="load-more hoverable" v-if="filteredNpcs.length > 30 && visibleNpcs.length < filteredNpcs.length" @click="loadMore">
      <span class="material-symbols-outlined" style="font-size: 2rem;">keyboard_arrow_down</span>
      <p style="margin: 0; font-size: 1.5rem;">{{ t('loadMore') }}</p>
    </div>
  </div>
</template>

<script>
export default {
  components: {
    Npc,
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
      npcs: [],
      filteredNpcs: [],
      visibleNpcs: [],
      currentSort: 'id',
      selectedServerIndex: 1,
      lastUpdated: '',
    }
  },
  methods: {
    async getNpcs() {
      let response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/NpcTemplates.json');
      let data = await response.json();
      this.npcs = data;
      this.filteredNpcs = [...data];
      if (this.reversed) 
        this.filteredNpcs.reverse();
      this.visibleNpcs = this.filteredNpcs.slice(0, 30);
      response = await fetch(this.servers[this.selectedServerIndex - 1].id + '/LastUpdated');
      data = await response.text();
      let date = new Date(data);
      this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';
      this.loading = false;
    },
    loadMore() {
      this.visibleNpcs = this.filteredNpcs.slice(0, this.visibleNpcs.length + 30);
    },
    changeSort(e) {
      this.currentSort = e.target.value;
      this.sortNpcs();
    },
    sortNpcs() {
      switch (this.currentSort) {
        case 'id':
          this.filteredNpcs.sort((a, b) => a.id - b.id);
          break;
        case 'name':
          this.filteredNpcs.sort((a, b) => a.name.localeCompare(b.name));
          break;
      }
      if (this.reversed) 
        this.filteredNpcs.reverse();
      this.visibleNpcs = this.filteredNpcs.slice(0, 30);
    },
    checkDeleteAll(e) {
      const search = e.target.value.toLowerCase();
      if (search === '') {
        this.filteredNpcs = [...this.npcs];
        if (this.reversed) 
          this.filteredNpcs.reverse();
        this.sortNpcs();
        return;
      }
    },
    searchNpc(e) {
      const search =  this.replaceVietnameseChars(e.target.value.toLowerCase());
      this.filteredNpcs = this.npcs.filter(npc => this.replaceVietnameseChars((npc.name + '|' + npc.id).toLowerCase()).includes(search));
      if (this.reversed) 
        this.filteredNpcs.reverse();
      this.visibleNpcs = this.filteredNpcs.slice(0, 30);
    },
    inverseSort(e) {
      this.filteredNpcs.reverse();
      this.visibleNpcs = this.filteredNpcs.slice(0, 30);
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
      this.getNpcs();
    },
  },
  mounted() {
    let index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
    if (index !== -1) 
      this.selectedServerIndex = index + 1;
    else 
      this.selectedServerIndex = 1;
    moment.locale(navigator.language);
    this.getNpcs().then(() => {
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

.material-symbols-outlined {
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

.npcs {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 25px;
}

.load-more {
  display: flex;
  justify-content: center;
  margin: 50px 50px 30px 50px;
  cursor: pointer;
  font-size: 20px;
  flex-direction: row;
  gap: 10px;
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