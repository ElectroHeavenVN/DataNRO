<script setup>
import { useI18n } from 'vue-i18n'
import HSNRPage from './views/HSNRPage.vue';
import TeaMobiPage from './views/TeaMobiPage.vue';
import SelectGamePublisherPage from './views/SelectGamePublisherPage.vue';
import NotFound from './components/NotFound.vue';

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
          <a v-if="currentWorkflowLink !== ''" class="workflowRunning content links" :href="currentWorkflowLink" target="_blank" :title="t('workflowCurrentlyUpdatingDesc')">
          <span>
            {{ t("workflowCurrentlyUpdating") }}
          </span>
          <svg width="20px" height="20px" fill="none" viewBox="0 0 16 16" class="anim-rotate" xmlns="http://www.w3.org/2000/svg">
            <path fill="none" stroke="#DBAB0A" stroke-width="2" d="M3.05 3.05a7 7 0 1 1 9.9 9.9 7 7 0 0 1-9.9-9.9Z" opacity=".5"></path>
            <path fill="#DBAB0A" fill-rule="evenodd" d="M8 4a4 4 0 1 0 0 8 4 4 0 0 0 0-8Z" clip-rule="evenodd"></path>
            <path fill="#DBAB0A" d="M14 8a6 6 0 0 0-6-6V0a8 8 0 0 1 8 8h-2Z"></path>
          </svg>
        </a>
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
              <font-awesome-icon icon="fa-solid fa-house" fixed-width alt="Home" />  
            </a>
            <a @click="showDiscordEmbed">
              <font-awesome-icon icon="fa-brands fa-discord" fixed-width alt="Discord" />
            </a>
            <div>
              <a href="https://github.com/ElectroHeavenVN/DataNRO" target="_blank">
                <font-awesome-icon icon="fa-brands fa-github" fixed-width alt="GitHub" />
              </a>
            </div>
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
      <iframe v-if="discordEmbedShown" class="discordEmbed" src="https://discord.com/widget?id=1115634791321190420&theme=dark" allowtransparency="true" frameborder="0" sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts"></iframe>
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
      discordEmbedShown: false,
      currentWorkflowLink: ""
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
    },
    showDiscordEmbed() {
      this.discordEmbedShown = !this.discordEmbedShown;
    },
    async getWorkflowStatus() {
      const response = await fetch('https://api.github.com/repos/ElectroHeavenVN/DataNRO/actions/runs');
      const data = await response.json();
      const workflowRuns = data.workflow_runs.filter(run => run.path === '.github/workflows/update-data.yml');
      const latestRun = workflowRuns[0];
      return latestRun;
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
    this.getWorkflowStatus().then(data => {
      if (data.status === "in_progress")
        this.currentWorkflowLink = data.html_url;
    });
  }
}
</script>

<style scoped>

.workflowRunning {
  white-space: nowrap;
}

.anim-rotate {
    animation: rotate-keyframes 1s linear infinite;
}

@keyframes rotate-keyframes {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
}

.discordEmbed {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  position: fixed;
  top: 50px;
  right: 10px;
  z-index: 1000;
  border: none;
  border-radius: 10px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
  width: 350px;
  height: calc(100vh - 60px);
}

.checkbox {
	opacity: 0;
	position: absolute;
}

.label {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 1px;
	background-color: var(--component-color);
	border-radius: 15px;
	cursor: pointer;
	display: flex;
	align-items: center;
	/* justify-content: space-between; */
	padding: 3px;
	position: relative;
	height: 15px;
	width: 35px;
  gap: 5px;
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
	transform: translateX(17px);
}

.fa-moon {
	color: #ffaa00;
}

.fa-sun {
	color: #ff9900;
}

.fa-discord {
  color: #5865F2;
  filter: 
    drop-shadow(1px 1px 1px #00ffff) 
    drop-shadow(-1px 1px 1px #00ffff) 
    drop-shadow(1px -1px 1px #00ffff) 
    /* drop-shadow(-1px -1px 1px #48c2ff)  */
    !important;
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
    margin-top: 20px;
}

.links {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.links *:hover {
  opacity: .8;
}

.links a {
  display: flex;
  align-items: center;
  justify-content: center;
}

.links a * {
  font-size: 20px;
}

@media screen and (max-height: 450px) {
  .sidenav {
    padding-top: 15px;
  }

  .sidenav a {
    font-size: 18px;
  }
}

@media screen and (max-width: 600px) {
  .wrapper {
    width: calc(100% - 20px);
  }

  #main .content {    
    width: calc(100% - 20px);
    margin-left: 10px;
  }

  .discordEmbed {
    width: calc(100% - 20px);
  }
}

@media screen and (max-width: 450px) {
  .workflowRunning span {
    display: none;
  }
}
</style>
