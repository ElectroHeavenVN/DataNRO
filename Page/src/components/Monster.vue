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

const formatHP = (hp) => {
  if (hp < 1000)
    return hp.toString();
  if (hp < 1000000) 
    return (hp / 1000) + 'K';
  if (hp < 1000000000) 
    return (hp / 1000000) + 'M';
  return (hp / 1000000000) + 'B';
}

const getTypeClass = (type) => {
  switch (type) {
    case 0:
      return 'typeImmovable';
    case 1:
      return 'typeWalking';
    case 4:
      return 'typeFlying';
    default:
      return 'typeUnknown';
  }
}

const getTypeString = (type) => t(getTypeClass(type));

</script>

<template>
  <div class="mob" :style="{ width, height }">
    <div class="badges">
      <div class="badge id" @click="copyToClipboard(id);" @touchstart="copyToClipboard(id);" :title="t('clickToCopy') + ' ID'">
        ID: {{ id }}
        <img src="../assets/Copy.svg" style="height: 10px;" alt="Copy" />
      </div>
      <div :class="'badge type ' + getTypeClass(type)" :title="t('type') + ': ' + getTypeString(type)">{{ getTypeString(type) }}</div>
    </div>
    <div class="content">
      <img class="icon" :src="'Monsters/' + id + '.png'" :alt="'ID: ' + id" :title="'ID: ' + id" />
      <div>
        <h2 class="name" :title="name">{{ name }}</h2>
        <div class="hp" :title="'HP: ' + hp">HP: {{ formatHP(hp) }}</div>
      </div>
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
    range: {
      type: Number,
      required: true,
    },
    speed: {
      type: Number,
      required: true,
    },
    type: {
      type: Number,
      required: true,
    },
    dartType: {
      type: Number,
      required: true,
    },
    hp: {
      type: Number,
      required: true,
    },
  }
}
</script>

<style scoped>
.mob {
  display: flex;
  gap: 10px;
  flex-direction: column;
  background-color: #1c1a23;
  border-radius: 10px;
  padding: 15px;
  color: white;
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
  padding: 4px 8px;
  border-radius: 4px;
  cursor: default;
  user-select: none;
}

.badge.id {
  background-color: #50e0ff;
  cursor: pointer;
}

.badge.type {
  cursor: default;
}

.typeImmovable {
  background-color: black;
  color: white;
}

.typeWalking {
  background-color: white;
}

.typeFlying {
  background-color: #60ff60;
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
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  padding-right: 10px;
  margin: 0;
}

.hp {
  font-size: 12px;
  color: #aaa;
}

@media screen and (max-width: 1032px) {
    .mob {
        width: 100% !important;
    }
}
</style>