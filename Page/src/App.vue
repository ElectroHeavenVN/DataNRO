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
          <span v-if="currentPath == 'TeaMobi' || currentPath == 'HSNR'"
            style="font-size: 25px; cursor:pointer; position: relative; top:1px;" @click=openNav>&#9776;</span>
          <a v-if="currentPath == 'HSNR'" href="/DataNRO/HSNR/">
            <img src="/DataHSNR.png" alt="HSNR">
          </a>
          <a v-else-if="currentPath == 'TeaMobi'" href="/DataNRO/TeaMobi/">
            <img src="/DataNRO.png" alt="TeaMobi">
          </a>
          <div style="display: flex; flex-direction: column; align-items: center;">
            <a href="/DataNRO/" style="position: relative; top: 3px">
              <h1>{{ title }}</h1>
            </a>
            <span style="font-size: 10px; position: relative; top: -3px">by ElectroHeavenVN</span>
          </div>
          <a v-if="currentPath == ''" href="/DataNRO/TeaMobi/">
            <img src="/DataNRO.png" alt="TeaMobi">
          </a>
          <a v-if="currentPath == ''" href="/DataNRO/HSNR/">
            <img src="/DataHSNR.png" alt="HSNR">
          </a>
        </div>
        <div v-if="lastWorkflowLink !== ''" class="lastWorkflow">
          <a v-if="lastWorkflowStatus === 'in_progress'" class="content links" :href="lastWorkflowLink" target="_blank"
            :title="t('workflowRunningDesc')">
            <span class="lastWorkflowLongText" style='display: block'>
              {{ t("workflowRunning") }}
            </span>
            <span class="lastWorkflowShortText" style='display: block'>
              {{ t("workflowRunningShort") }}
            </span>
            <svg width="20px" height="20px" fill="none" viewBox="0 0 16 16" class="anim-rotate"
              xmlns="http://www.w3.org/2000/svg">
              <path fill="none" stroke="#DBAB0A" stroke-width="2" d="M3.05 3.05a7 7 0 1 1 9.9 9.9 7 7 0 0 1-9.9-9.9Z"
                opacity=".5"></path>
              <path fill="#DBAB0A" fill-rule="evenodd" d="M8 4a4 4 0 1 0 0 8 4 4 0 0 0 0-8Z" clip-rule="evenodd"></path>
              <path fill="#DBAB0A" d="M14 8a6 6 0 0 0-6-6V0a8 8 0 0 1 8 8h-2Z"></path>
            </svg>
          </a>
          <a v-else-if="lastWorkflowStatus === 'success'" class="content links" :href="lastWorkflowLink" target="_blank"
            :title="t('workflowSuccessDesc')">
            <span class="lastWorkflowLongText">
              {{ t("workflowSuccess") }}
            </span>
            <span class="lastWorkflowShortText">
              {{ t("workflowSuccessShort") }}
            </span>
            <svg width="16" height="16" style="margin-top: 2px"
              class="octicon octicon-check-circle-fill color-fg-success" viewBox="0 0 16 16" version="1.1" role="img">
              <path fill="#2BD853"
                d="M8 16A8 8 0 1 1 8 0a8 8 0 0 1 0 16Zm3.78-9.72a.751.751 0 0 0-.018-1.042.751.751 0 0 0-1.042-.018L6.75 9.19 5.28 7.72a.751.751 0 0 0-1.042.018.751.751 0 0 0-.018 1.042l2 2a.75.75 0 0 0 1.06 0Z">
              </path>
            </svg>
          </a>
          <a v-else-if="lastWorkflowStatus === 'failure'" class="content links" :href="lastWorkflowLink" target="_blank"
            :title="t('workflowFailureDesc')">
            <span class="lastWorkflowLongText">
              {{ t("workflowFailure") }}
            </span>
            <span class="lastWorkflowShortText">
              {{ t("workflowFailureShort") }}
            </span>
            <svg width="16" height="16" style="margin-top: 2px" class="octicon octicon-x-circle-fill color-fg-danger" viewBox="0 0 16 16" version="1.1" role="img">
              <path fill="#FF9492"
                d="M2.343 13.657A8 8 0 1 1 13.658 2.343 8 8 0 0 1 2.343 13.657ZM6.03 4.97a.751.751 0 0 0-1.042.018.751.751 0 0 0-.018 1.042L6.94 8 4.97 9.97a.749.749 0 0 0 .326 1.275.749.749 0 0 0 .734-.215L8 9.06l1.97 1.97a.749.749 0 0 0 1.275-.326.749.749 0 0 0-.215-.734L9.06 8l1.97-1.97a.749.749 0 0 0-.326-1.275.749.749 0 0 0-.734.215L8 6.94Z">
              </path>
            </svg>
          </a>
          <a v-else class="content links" :href="lastWorkflowLink" target="_blank" :title="t('workflowCancelledDesc')">
            <span class="lastWorkflowLongText">
              {{ t("workflowCancelled") }}
            </span>
            <span class="lastWorkflowShortText">
              {{ t("workflowCancelledShort") }}
            </span>
            <svg width="16" height="16" style="margin-top: 2px" class="octicon octicon-stop neutral-check" viewBox="0 0 16 16" version="1.1" role="img">
              <path fill="#B7BDC8" d="M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z">
              </path>
            </svg>
          </a>
        </div>
        <div class="content">
          <div class="themeSwitcher">
            <input type="checkbox" @change="changeTheme" id="chk" />
            <font-awesome-icon icon="fa-solid fa-sun" class="light" />
            <font-awesome-icon icon="fa-solid fa-moon" class="dark" />
          </div>
          <div class="links">
            <a href="/" target="_blank">
              <font-awesome-icon icon="fa-solid fa-house" fixed-width alt="Home" />
            </a>
            <a @click="showDiscordEmbed">
              <font-awesome-icon icon="fa-brands fa-discord" fixed-width alt="Discord" />
            </a>
            <a href="https://github.com/ElectroHeavenVN/DataNRO" target="_blank">
              <font-awesome-icon icon="fa-brands fa-github" fixed-width alt="GitHub" />
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
      <iframe v-if="discordEmbedShown" class="discordEmbed"
        src="https://discord.com/widget?id=1115634791321190420&theme=dark" allowtransparency="true" frameborder="0"
        sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts"></iframe>
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
      lastWorkflowLink: "",
      lastWorkflowStatus: ""  // in_progress, success, failure, cancelled
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
    setPage(page) {
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
      this.lastWorkflowStatus = data.status;
      this.lastWorkflowLink = data.html_url;
      if (data.status !== "in_progress") {
        this.lastWorkflowStatus = data.conclusion;
      }
    });
  }
}
</script>

<style scoped>
.lastWorkflow {
  white-space: nowrap;
  padding-right: 40px;
}

.lastWorkflow a {
  gap: 5px;
}

.lastWorkflow .lastWorkflowLongText {
  display: block;
}

.lastWorkflow .lastWorkflowShortText {
  display: none;
  font-size: 12px;
}

.themeSwitcher {
  font-size: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 20px;
}

.themeSwitcher input {
  opacity: 0;
  position: absolute;
  width: 20px;
  height: 20px;
  margin: 0;
}

[data-theme="light"] .themeSwitcher .light {
  display: none;
}

[data-theme="dark"] .themeSwitcher .dark {
  display: none;
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

.fa-discord {
  color: #5865F2;
  filter:
    drop-shadow(1px 1px 1px #00ffff)
    drop-shadow(-1px 1px 1px #00ffff)
    drop-shadow(1px -1px 1px #00ffff)
    drop-shadow(-1px -1px 1px #00ffff) 
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

@media screen and (max-width: 550px) {
  .lastWorkflow .lastWorkflowLongText {
    display: none;
  }

  .lastWorkflow .lastWorkflowShortText {
    display: block;
  }

  .lastWorkflow {
  padding-right: 0;
}

  .links {
    gap: 5px;
  }
}

@media screen and (max-width: 380px) {
  .lastWorkflow .lastWorkflowShortText {
    display: none;
  }
}
</style>
