<script setup>
import { useI18n } from 'vue-i18n'
import Item from './Item.vue';
const { t } = useI18n();

</script>

<template>
  <div v-if="loading" style="width: 100%; text-align: center;">{{ t('loading') }}...</div>
  <div v-else>
    <div class="title">
      <h1>{{ t('items') }}</h1>
      <div style="display: flex; flex-direction: row; gap: 20px; align-items: center;">
        <span>{{ t('selectServer') }}</span>
        <select @change="changeServer">
          <option v-for = "server in servers" :value="server">{{ t(server.toLowerCase()) }}</option>
        </select>
      </div>
    </div>
    <div class="searching">
      <div class="search-bar">
        <span class="material-symbols-outlined" style="font-size: 2rem;">search</span>
        <input type="text" :placeholder="t('searchItem')" value="" @input="checkDeleteAll" @change="searchItem" />
      </div>
      <div class="sort">
        <span class="material-symbols-outlined" style="font-size: 2rem; transform: scale(1, -1); cursor: pointer;" @click="inverseSort">sort</span>
          <select @change="changeSort">
            <option value="id" selected="">ID</option>
            <option value="name">{{ t('name') }}</option>
            <option value="icon">Icon</option>
          </select>
        </div>
    </div>
    <div class="items">
      <Item v-for="item in visibleItems" :key=item.id :icon=item.icon :name=item.name
        :description=item.description :id=item.id :type=item.type :gender=item.gender :level=item.level
        :isNewItem="items.map(i => i.id).indexOf(item.id) > items.length - 50" :powerRequired=item.strRequire class="hoverable" />
    </div>
    <div class="load-more hoverable" v-if="filteredItems.length > 30 && visibleItems.length < filteredItems.length" @click="loadMore">
      <span class="material-symbols-outlined" style="font-size: 2rem;">keyboard_arrow_down</span>
      <p style="margin: 0; font-size: 1.5rem;">{{ t('loadMore') }}</p>
    </div>
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
  },
  data() {
    return {
      loading: true,
      reversed: false,
      items: [],
      filteredItems: [],
      visibleItems: [],
      currentSort: 'id',
      selectedServer: 1,
    }
  },
  methods: {
    async getItems() {
      const response = await fetch(this.servers[this.selectedServer] + '/ItemTemplates.json');
      let data = await response.json();
      this.items = data;
      this.filteredItems = [...data];
      if (this.reversed) 
        this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
      this.loading = false;
    },
    loadMore() {
      this.visibleItems = this.filteredItems.slice(0, this.visibleItems.length + 30);
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
        this.filteredItems = this.items;
        if (this.reversed) 
          this.filteredItems.reverse();
        this.sortItems();
        return;
      }
    },
    searchItem(e) {
      const search = e.target.value.toLowerCase();
      this.filteredItems = this.items.filter(item => this.replaceVietnameseChars((item.name + '|' + item.description + '|' + item.id).toLowerCase()).includes(search));
      if (this.reversed) 
        this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
    },
    inverseSort(e) {
      this.filteredItems.reverse();
      this.visibleItems = this.filteredItems.slice(0, 30);
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
      this.selectedServer = e.target.selectedIndex;
      this.getItems();
    },
  },
  mounted() {
    this.getItems();
  },
};
</script>

<style scoped>
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

.title h1 {
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

.items {
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

  .sort select {
    width: 100%;
  }
}

</style>