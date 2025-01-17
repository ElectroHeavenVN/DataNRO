<script setup>
import { useI18n } from 'vue-i18n'
import ItemPage from './components/ItemPage.vue';
const { t } = useI18n();

document.title += ' - Server ' + location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
</script>

<script>
export default {
  data() {
    return {
      title: 'DataNRO',
      servers : [
        "Server1",
        "Server2",
        "Server3",
        "Server4",
        "Server5",
        "Server6",
        "Server7",
        "Server8910",
        "Server11",
        "Server12",
        "Server13",
        "Server14",
        "Super1",
        "Super2",
        "Universe1",
        "Naga",
      ]
    }
  },
  methods: {
    changeTitle() {
      let title = location.pathname.replace(/^\/+|\/+$/g, '').split('/')[0];
      if (title === 'DataNRO') 
        this.title = 'DataNRO';
      else if (title === 'HSNR')
        this.title = 'DataHSNR';
    }
  },
  mounted() {
    this.changeTitle();
  }
}
</script>

<template>
  <header>
    <nav>
      <div class="wrapper">
        <div href="/" class="content head">
          <img src="./assets/vue.svg" :alt="title">
          <h1>{{ title }}</h1>
        </div>
        <div class="content">
          <div class="content links">
            <a href="?page=items">{{ t("items") }}</a>
            <a href="?page=npcs">{{ t("npcs") }}</a>
            <a href="?page=skills">{{ t("skills") }}</a>
            <a href="?page=mobs">{{ t("mobs") }}</a>
            <a href="?page=itemOptions">{{ t("itemOptions") }}</a>
            <a href="?page=maps">{{ t("maps") }}</a>
            <a href="?page=parts">{{ t("parts") }}</a>
          </div>
        </div>
      </div>
    </nav>
  </header>
  <div id="main">
    <div class="wrapper">
      <div class="content">
        <ItemPage :servers="servers" />
      </div>
    </div>
  </div>
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
    min-height: calc(100vh - 140px);
    justify-content: flex-start
}
</style>

<style scoped>
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
    width: calc(100% - 80px);
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

nav .head:hover {
    opacity: .6
}

nav .head img {
    width: 32px;
    height: 32px;
    margin-right: 10px
}

nav .content {
    display: flex;
    align-items: center;
    justify-content: center
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
    margin-top: 50px
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
