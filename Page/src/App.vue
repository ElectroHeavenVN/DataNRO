<script setup>
import { useI18n } from 'vue-i18n'
import HSNRPage from './views/HSNRPage.vue';
import TeaMobiPage from './views/TeaMobiPage.vue';
import SelectGamePublisherPage from './views/SelectGamePublisherPage.vue';
import NotFound from './components/NotFound.vue';

const { t } = useI18n();

</script>

<script>
export default {
  data() {
    return {
      title: 'DataNRO',
    }
  },
  methods: {
    changeTitle() {
      let title = location.pathname.replace(/^\/+|\/+$/g, '').split('/')[1];
      if (title === 'DataNRO') 
        this.title = 'DataNRO';
      else if (title === 'HSNR')
        this.title = 'DataHSNR';
    if (location.pathname !== '/DataNRO/')
      document.title = this.title + ' - Server ' + location.pathname.replace(/^\/+|\/+$/g, '').split('/')[1];
    else 
      document.title = this.title + " by ElectroHeavenVN";
    },
    openNav() {
      document.getElementById("mySidenav").style.width = "150px";
    },
    closeNav() {
      document.getElementById("mySidenav").style.width = "0";
    },
    setPage(page)
    {
      window.history.pushState({}, '', window.location.origin + window.location.pathname + '?page=' + page);
      this.closeNav();
    }
  },
  computed: {
    currentPath() {
      let path = location.pathname.replace(/^\/+|\/+$/g, '').split('/');
      if (path.length == 2)
        return path[1];
      if (path.length == 1)
        return '';
      return 'notFound';
    }
  },
  mounted() {
    this.changeTitle();
  }
}
</script>

<template>
  <div v-if="currentPath == 'TeaMobi' || currentPath == 'HSNR'" id="mySidenav" class="sidenav">
    <a href="javascript:void(0)" class="closebtn" @click=closeNav>&times;</a>
    <span class="hoverable" @click="setPage('items')">{{ t("items") }}</span>
    <span class="hoverable" @click="setPage('npcs')">{{ t("npcs") }}</span>
    <span class="hoverable" @click="setPage('skills')">{{ t("skills") }}</span>
    <span class="hoverable" @click="setPage('mobs')">{{ t("mobs") }}</span>
    <!-- <span class="hoverable" @click="setPage('itemOptions')">{{ t("itemOptions") }}</span> -->
    <span class="hoverable" @click="setPage('maps')">{{ t("maps") }}</span>
    <!-- <span class="hoverable" @click="setPage('parts')">{{ t("parts") }}</span> -->
  </div>
  <header>
    <nav>
      <div class="wrapper">
        <div href="/" class="content head">
          <span v-if="currentPath == 'TeaMobi' || currentPath == 'HSNR'" style="font-size: 25px; cursor:pointer; position: relative; top:1px;" @click=openNav>&#9776;</span>
          <a v-if="currentPath == 'HSNR'" href="/DataNRO/HSNR/">
            <img src="/DataHSNR.png" alt="HSNR">
          </a>
          <a v-else-if="currentPath == 'TeaMobi'"  href="/DataNRO/TeaMobi/">
          <img src="/DataNRO.png" alt="TeaMobi">
          </a>
          <div style="display: flex; flex-direction: column; align-items: center;">
            <a href="/DataNRO/">
              <h1>{{ title }}</h1>
            </a>
            <a href="https://hits.seeyoufarm.com" style="position: relative; top: -5px;" target="_blank">
              <img :src="'https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Felectroheavenvn.github.io%2FDataNRO%2F&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&edge_flat=false&title=' + encodeURIComponent(t('visits'))" style="width: auto; height: 15px;"/>
            </a>
          </div>
          <a v-if="currentPath == ''" href="/DataNRO/TeaMobi/">
            <img src="/DataNRO.png" alt="TeaMobi">
          </a>
          <a v-if="currentPath == ''" href="/DataNRO/HSNR/">
            <img src="/DataHSNR.png" alt="HSNR">
          </a>
        </div>
        <div class="content">
          <div class= "links">
            <a href="/" target="_blank">
              <img src="./assets/Home-icon.svg" alt="Home" style="width: 25px;" >
            </a>
            <a href="https://discord.gg/yzHjZbfuAR" target="_blank">
              <img src="./assets/discord-mark-white.svg" alt="Discord" style="width: 25px;" >
            </a>
            <a href="https://github.com/ElectroHeavenVN/DataNRO" target="_blank">
              <img src="./assets/github-mark-white.svg" alt="GitHub" style="width: 25px;" >
            </a>
          </div>
        </div>
      </div>
    </nav>
  </header>
  <div id="main">
    <div class="wrapper">
      <div class="content">
        <TeaMobiPage v-if="currentPath == 'TeaMobi'" />
        <HSNRPage v-else-if="currentPath == 'HSNR'" />
        <SelectGamePublisherPage v-else-if="currentPath == ''" />
        <NotFound v-else />
      </div>
    </div>
  </div>
  <footer>
    <p>Copyright &copy; 2025 ElectroHeavenVN.</p>
    <p>&nbsp;</p>
    <p>All rights reserved.</p>
  </footer>
</template>

<style scoped>
.sidenav {
  height: 100%;
  width: 0;
  position: fixed;
  z-index: 1001;
  top: 0;
  left: 0;
  background-color: var(--component-bg);
  color: var(--component-color);
  overflow-x: hidden;
  transition: 0.5s;
  padding-top: 60px;
  white-space: nowrap;
}

.sidenav span,
.sidenav a {
  user-select: none;  
  cursor: pointer;
  padding: 8px 8px 8px 32px;
  font-size: 20px;
  color: var(--component-color);
  display: block;
  transition: 0.3s;
}

.sidenav .closebtn {
  position: absolute;
  top: 0;
  right: 25px;
  font-size: 36px;
  margin-left: 50px;
  text-decoration: none;
}

nav {
    width: 100%;
    height: 60px;
    background-color: var(--component-bg);
    color: var(--component-color);
    display: flex;
    justify-content: center;
    font-weight: 700;
    position: fixed;
    top: 0;
    z-index: 1000
}

@media (prefers-color-scheme: light) {
  nav .links img {
    filter: invert(1);
  }
}

.wrapper {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: calc(100% - 60px);
}

nav .head {
    cursor: pointer;
    -webkit-user-select: none;
    user-select: none
}

nav .head h1 {
    font-size: 1.5rem;
    margin: 0
}

nav .head span:hover,
nav .head h1:hover {
    opacity: .6
}

nav .head img {
    width: 32px;
    height: 32px;
    border-radius: 5px;
}

nav .content {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 10px;
    font-weight: bold;
    color: var(--component-color);
    background-color: var(--component-bg);
}

nav .content a {
    text-decoration: none;
    color: inherit !important;
}

nav .content a img {
  display: flex; 
  justify-content: center;
}

nav .content img:hover {
    opacity: .6
}

#main .content {
    width: 100%;
    margin-top: 55px
}

.links {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 15px;
}

.links a {
  display: flex;
  align-items: center;
  justify-content: center;
}

@media screen and (max-height: 450px) {
  .sidenav {padding-top: 15px;}
  .sidenav a {font-size: 18px;}
}

@media screen and (max-width: 1390px) {
    .wrapper {
        padding: 0 20px
    }
}

@media screen and (max-width: 600px) {
    nav .links a:first-child {
        display:none
    }
}
</style>
