<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();

const copyToClipboard = (content) => {
  try {
    navigator.clipboard.writeText(content);
  } catch (err) {
    console.error('Failed to copy to clipboard', err);
  }
}

</script>

<template>
  <div class="map" :style="{ width }">
    <div class="badges">
      <div class="badge id" @click="copyToClipboard(id);" @touchstart="copyToClipboard(id);" :title="t('clickToCopy') + ' ID'">
        ID: {{ id }}
        <img src="../assets/Copy.svg" style="height: 10px;" alt="Copy" />
      </div>
    </div>
    <div class="content">
      <img class="icon" :src="'Maps/' + id + '_tile.png'" :alt="'Map ' + id" :title="'Map ' + id" />
      <div style="display: flex; flex-direction: column; align-items: start;">
        <h2 class="name" :title="name.length == 0 ? t('noName') : name">{{ name.length == 0 ? t('noName') : name }}</h2>
        <div style="display: flex; flex-direction: row; gap: 5px;">
          <a v-if="hasFullMapImg" class="more-info" :href="`Maps/${id}.png`" target="_blank">{{ t('viewMapImg') }}</a>
          <span class="material-icons-round">open_in_new</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      hasFullMapImg: true,
    }
  },
  props: {
    width: {
      type: String,
      default: '350px',
    },
    name: {
      type: String,
      required: true,
    },
    id: {
      type: Number,
      required: true,
    },
  },
  async mounted()
  {
    try {
      let response = await fetch(`Maps/${this.id}.png`, {method: 'HEAD'});
      this.hasFullMapImg = response.ok;
    }
    catch {
      this.hasFullMapImg = false;
    }
  }
}
</script>

<style scoped>
.map {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  display: flex;
  gap: 10px;
  flex-direction: column;
  background-color: var(--component-bg);
  border-radius: 10px;
  padding: 15px;
  color: var(--component-color);
  position: relative;
  min-width: 275px;
  min-height: 75px;
  height: fit-content;
}

.badges {
  position: absolute;
  top: -12px;
  width: calc(100% - 30px);
  font-weight: bold;
  color: rgb(0, 0, 0);
  font-size: 12px;
  text-transform: uppercase;
  display: flex;
  flex-direction: row-reverse;
  justify-content: space-between;
}

.badge {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 1px;
  padding: 4px 8px;
  border-radius: 4px;
  cursor: default;
  user-select: none;
}

.badge.id {
  background-color: #50e0ff;
  cursor: pointer;
}

.content {
  display: flex;
  padding-top: 3px;
}

.icon {
  user-select: none;
  width: 65px;
  height: 65px;
  margin-right: 15px;
  font-size: 14px;
  font-weight: bold;
  object-fit: contain;
}

.name {
  font-weight: bold;
  font-size: 25px;
  flex: 1;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  margin: 0;
}

.more-info {
  display: flex;
  user-select: none;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  gap: 5px;
  cursor: pointer;
  font-size: 18px;
}

.more-info p {
  margin: 0;
}

@media screen and (max-width: 1032px) {
    .map {
        width: 100% !important;
    }
}
</style>