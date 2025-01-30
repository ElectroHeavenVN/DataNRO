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
      document.getElementById("mySidenav").style.width = "250px";
    },
    closeNav() {
      document.getElementById("mySidenav").style.width = "0";
    },
    goHome() {
      if (location.pathname !== '/DataNRO/')
        location.href = '/DataNRO/';
    },
    goToNRO() {
      location.href = '/DataNRO/TeaMobi/';
    },
    goToHSNR() {
      location.href = '/DataNRO/HSNR/';
    },
    setPage(page)
    {
      window.history.pushState({}, '', window.location.origin + window.location.pathname + '?page=' + page);
    }
  },
  computed: {
    currentPath() {
      return location.pathname.replace(/^\/+|\/+$/g, '').split('/')[1] ?? '';
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
    <span @click="setPage('items')">{{ t("items") }}</span>
    <span @click="setPage('npcs')">{{ t("npcs") }}</span>
    <span @click="setPage('skills')">{{ t("skills") }}</span>
    <span @click="setPage('mobs')">{{ t("mobs") }}</span>
    <!-- <span @click="setPage('itemOptions')">{{ t("itemOptions") }}</span> -->
    <span @click="setPage('maps')">{{ t("maps") }}</span>
    <!-- <span @click="setPage('parts')">{{ t("parts") }}</span> -->
  </div>
  <header>
    <nav>
      <div class="wrapper">
        <div href="/" class="content head">
          <span v-if="currentPath == 'TeaMobi' || currentPath == 'HSNR'" style="font-size: 25px; cursor:pointer;" @click=openNav>&#9776;</span>
          <img v-if="currentPath == 'HSNR'" @click="goHome" src="/DataHSNR.png" :alt="title">
          <img v-else-if="currentPath == 'TeaMobi'" @click="goHome" src="/DataNRO.png" :alt="title">
          <div style="display: flex; flex-direction: column; align-items: center;">
            <h1 @click="goHome" style="position: relative; top: 5px;">{{ title }}</h1>
            <a href="https://hits.seeyoufarm.com" style="position: relative; top: -3px;">
              <img :src="'https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Felectroheavenvn.github.io%2FDataNRO%2F&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&edge_flat=false&title=' + encodeURIComponent(t('visits'))" style="width: auto; height: 15px;"/>
            </a>
          </div>
          <img v-if="currentPath == ''" @click="goToNRO" src="/DataNRO.png" :alt="title">
          <img v-if="currentPath == ''" @click="goToHSNR" src="/DataHSNR.png" :alt="title">
        </div>
        <div class="content">
          <div class= "links">
            <a href="/">
              <img src="./assets/Home-icon.svg" alt="Home" style="width: 25px;" >
            </a>
            <a href="https://discord.gg/yzHjZbfuAR">
              <img src="./assets/discord-mark-white.svg" alt="Discord" style="width: 25px;" >
            </a>
            <a href="https://github.com/ElectroHeavenVN/DataNRO">
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
  background-color: #111;
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
  text-decoration: none;
  font-size: 20px;
  color: #c9c9c9;
  display: block;
  transition: 0.3s;
}

.sidenav span:hover,
.sidenav a:hover {
  color: #f1f1f1;
}

.sidenav .closebtn {
  position: absolute;
  top: 0;
  right: 25px;
  font-size: 36px;
  margin-left: 50px;
}

nav {
    width: 100%;
    height: 60px;
    background-color: #1c1a23;
    color: #fff;
    display: flex;
    justify-content: center;
    font-weight: 700;
    position: fixed;
    top: 0;
    z-index: 1000
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
}

nav .content a {
    color: #fff;
    text-decoration: none;
}

nav .content a:hover {
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
