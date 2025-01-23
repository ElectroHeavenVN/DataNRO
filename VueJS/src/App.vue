<script setup>
import { useI18n } from 'vue-i18n'
import HSNRPage from './views/HSNRPage.vue';
import TeaMobiPage from './views/TeaMobiPage.vue';
import SelectGamePublisherPage from './views/SelectGamePublisherPage.vue';

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
      let title = location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
      if (title === 'DataNRO') 
        this.title = 'DataNRO';
      else if (title === 'HSNR')
        this.title = 'DataHSNR';
    if (location.pathname !== '/')
      document.title = this.title + ' - Server ' + location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
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
      if (location.pathname !== '/')
        location.href = '/';
    },
    setPage(page)
    {
      window.history.pushState({}, '', window.location.origin + window.location.pathname + '?page=' + page);
    }
  },
  computed: {
    currentPath() {
      return location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
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
    <span @click="setPage('itemOptions')">{{ t("itemOptions") }}</span>
    <span @click="setPage('maps')">{{ t("maps") }}</span>
    <span @click="setPage('parts')">{{ t("parts") }}</span>
  </div>
  <header>
    <nav>
      <div class="wrapper">
        <div href="/" class="content head">
          <span v-if="currentPath == 'TeaMobi' || currentPath == 'HSNR'" style="font-size:30px; cursor:pointer; padding-right: 10px;" @click=openNav>&#9776;</span>
          <img @click="goHome" src="./assets/vue.svg" :alt="title">
          <h1 @click="goHome">{{ title }}</h1>
        </div>
        <div class="content">
          <div class="content links">
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
        <SelectGamePublisherPage v-else />
      </div>
    </div>
  </div>
  <footer>
    <p>Copyright &copy; 2025 ElectroHeavenVN.</p>
    <p>&nbsp;</p>
    <p>All rights reserved.</p>
  </footer>
</template>

<style>
.hoverable:hover {
    transform: translateY(-3px);
    box-shadow: 0 3px 10px #0003;
}

.hoverable {
    transition-duration: .2s;
}

#main {
    width: 100%;
    align-items: center;
    display: flex;
    flex-direction: column;
    min-height: calc(100vh - 90px);
    justify-content: flex-start
}


footer {
    height: 60px;
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    align-items: center;
    font-size: 1.2rem;
    background-color: #1c1a23;
    margin-top: 20px
}

footer p {
  margin: 0;
}

footer a {
    color: cyan;
    text-decoration: none
}

</style>

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
  padding: 8px 8px 8px 32px;
  text-decoration: none;
  font-size: 20px;
  color: #818181;
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
    height: 32px
}

nav .content {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 10px
}

nav .content a {
    color: #fff;
    text-decoration: none;
    margin-left: 20px
}

nav .content a:hover {
    opacity: .6
}

#main .content {
    width: 100%;
    margin-top: 55px
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
    nav .links {
        display:none
    }
}
</style>
