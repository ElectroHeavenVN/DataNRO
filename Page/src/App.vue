<script setup>
import { useI18n } from 'vue-i18n'
import HSNRPage from './views/HSNRPage.vue';
import TeaMobiPage from './views/TeaMobiPage.vue';
import SelectGamePublisherPage from './views/SelectGamePublisherPage.vue';
import NotFound from './components/NotFound.vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const { t } = useI18n();

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
              <img :src="'https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Felectroheavenvn.github.io%2FDataNRO%2F&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&edge_flat=false&title=' + encodeURIComponent(t('visits'))" style="width: auto; height: 12px;"/>
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
          <div>
            <input type="checkbox" class="checkbox" @change="changeTheme" id="chk" />
            <label class="label" for="chk">
              <font-awesome-icon icon="fa-solid fa-sun" />
              <font-awesome-icon icon="fa-solid fa-moon" />
              <div class="ball"></div>
            </label>
          </div>
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
    <p>Copyright &copy; 2025 ElectroHeavenVN. All rights reserved.</p>
  </footer>
</template>

<script>
export default {
  data() {
    return {
      title: 'DataNRO',
    }
  },
  methods: {
    changeTitle() {
      let currentPath = this.currentPath;
      if (location.pathname !== '/DataNRO/') {
        if (currentPath !== 'pageNotFound')
          document.title = 'DataNRO - Server ' + this.currentPath;
        else 
          document.title = "DataNRO - " + useI18n().t(currentPath);
      }
      else 
        document.title = "DataNRO by ElectroHeavenVN";
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
    },
    changeTheme() {
      let theme = localStorage.getItem("theme") || (window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
      document.documentElement.setAttribute('data-theme', theme === 'light' ? 'dark' : 'light');
      localStorage.setItem("theme", theme === 'light' ? 'dark' : 'light');
    }
  },
  computed: {
    currentPath() {
      let path = location.pathname.replace(/^\/+|\/+$/g, '').split('/');
      if (path.length == 2)
        return path[1];
      if (path.length == 1)
        return '';
      return 'pageNotFound';
    }
  },
  mounted() {
    this.changeTitle();
    if (!localStorage.getItem("theme")) 
      localStorage.setItem("theme", (window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light"));
    let theme = localStorage.getItem("theme");
    document.documentElement.setAttribute('data-theme', theme);
    document.getElementById('chk').checked = theme === 'light';
  }
}
</script>

<style scoped>

.checkbox {
	opacity: 0;
	position: absolute;
}

.label {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 1px;
	background-color: var(--component-color);
	border-radius: 20px;
	cursor: pointer;
	display: flex;
	align-items: center;
	/* justify-content: space-between; */
	padding: 3px;
	position: relative;
	height: 20px;
	width: 38px;
  gap: 9px;
}

.label .ball {
	background-color: var(--component-bg);
	border-radius: 50%;
	position: absolute;
	height: 18px;
	width: 18px;
	transform: translateX(0px);
	transition: transform 0.2s linear;
}

.checkbox:checked + .label .ball {
	transform: translateX(20px);
}

.fa-moon {
	color: #ffaa00;
}

.fa-sun {
	color: #ff9900;
}

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

.wrapper {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: calc(100% - 60px);
}

#main .content {
    width: 100%;
    margin-top: 55px
}

.links {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
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
</style>
