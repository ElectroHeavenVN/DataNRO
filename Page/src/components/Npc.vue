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
  <div class="npc" :style="{ width, height }">
    <div class="badges">
      <div class="badge id" @click="copyToClipboard(id);" @touchstart="copyToClipboard(id);" :title="t('clickToCopy') + ' ID'">
        ID: {{ id }}
        <font-awesome-icon icon="fa-solid fa-copy" alt="Copy"/>
      </div>
    </div>
    <div class="content">
      <img class="icon" :src="'NPCs/' + id + '.png'" :alt="head + ', ' + body + ', ' + legs" :title="t('head') + ': ' + head + ', ' + t('body') + ': ' + body + ', ' + t('legs') + ': ' + legs" />
      <h2 class="name" :title="name.length == 0 ? t('noName') : name">{{ name.length == 0 ? t('noName') : name }}</h2>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    width: {
      type: String,
      default: '350px',
    },
    height: {
      type: String,
      default: '75px',
    },
    name: {
      type: String,
      required: true,
    },
    id: {
      type: Number,
      required: true,
    },
    head: {
      type: Number,
      required: true,
    },
    body: {
      type: Number,
      required: true,
    },
    legs: {
      type: Number,
      required: true,
    },
  }
}
</script>

<style scoped>
.npc {
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
  padding-right: 10px;
  margin: 0;
  align-self: center;
}

@media screen and (max-width: 851px) {
    .npc {
        width: 100% !important;
    }
}
</style>