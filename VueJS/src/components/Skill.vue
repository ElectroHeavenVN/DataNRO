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

const getGenderClass = (gender) => {
  switch (gender) {
    case 0:
      return 'genderEarth';
    case 1:
      return 'genderNamekian';
    case 2:
      return 'genderSaiyan';
    default:
      return 'genderCommon';
  }
}

const getGenderString = (gender) => t(getGenderClass(gender));

const formatPowerRequired = (powerRequired) => {
  if (powerRequired < 10) 
    return '';
  if (powerRequired < 1000)
    return powerRequired.toString();
  if (powerRequired < 1000000) 
    return (powerRequired / 1000) + 'K';
  if (powerRequired < 1000000000) 
    return (powerRequired / 1000000) + 'M';
  return (powerRequired / 1000000000) + 'B';
}

const breakMultiLine = (text) => {
  let words = text.split(' ');
  let result = '';
  for (let i = 0; i < words.length; i++) {
    result += words[i] + ' ';
    if (i % 15 === 0 && i !== 0) {
      result += '\n';
    }
  }
  return result;
}
</script>

<template>
  <div class="skill" :style="{ width, height }">
    <div class="badges">
      <div class="badge id" @click="copyToClipboard(id);" @touchstart="copyToClipboard(id);" :title="t('clickToCopySkillID')">
        Skill ID: {{ id }}
        <img src="../assets/Copy.svg" style="height: 10px;" alt="Copy" />
      </div>
      <div style="display: flex; gap: 10px;">
        <div :class="'badge gender ' + getGenderClass(gender)" :title="t('gender') + ': ' + getGenderString(gender)">{{ getGenderString(gender) }}</div>
        <div class="badge level" :title="t('level') + ': ' + level"> {{ t('level') + ': ' + level }} </div>
      </div>
    </div>
    <div class="content">
      <img class="icon" :src="'Icons/' + icon + '.png'" :alt="'Icon ' + icon" :title="'Icon ' + icon"/>
      <div class="name-desc-gender">
        <div class="name-desc">
          <h2 class="name" :title=name>{{ name }}</h2>
          <div class="description" :title="breakMultiLine(description)">{{ description }}</div>
        </div>
      </div>
    </div>
    <div class="info">
      <div>
        <span>{{ t('type') }}: </span>
        <span class="type">{{ getTypeString(type) }}</span>
      </div>
      <!-- <span class="level">{{ level }}</span> -->
      <div v-if="formatPowerRequired(powerRequired) !== ''">
        <span>{{ t('powerRequired') }}: </span>
        <span class="power-required">{{ formatPowerRequired(powerRequired) }}</span>
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
      default: '100px',
    },
    maxFight: {
      type: Number,
      required: true,
    },
    manaUse: {
      type: Number,
      required: true,
    },
    skillId: {
      type: Number,
      required: true,
    },
    dx: {
      type: Number,
      required: true,
    },
    dy: {
      type: Number,
      required: true,
    },
    damage: {
      type: Number,
      required: true,
    },
    price: {
      type: Number,
      required: true,
    },
    coolDown: {
      type: Number,
      required: true,
    },
    icon: {
      type: Number,
      required: true,
    },
    name: {
      type: String,
      required: true,
    },
    description: {  //moreInfo
      type: String,
      required: true,
    },
    gender: {
      type: Number,
      required: true,
    },
    level: {  //point
      type: Number,
      required: true,
    },
    powerRequired: {  //powRequire
      type: Number,
      default: 0,
    },
  },
};
</script>

<style scoped>
.skill {
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

.content {
  display: flex;
  padding-top: 3px;
}

.icon {
  user-select: none;
  width: 50px;
  height: 50px;
  margin-right: 15px;
  font-size: 14px;
  font-weight: bold;
  object-fit: contain;
}

.name-desc-gender {
  display: flex;
  flex-direction: row;
  font-weight: bold;
  font-size: 12px;
  flex: 1;
  justify-content: space-between;
  overflow: hidden;
}

.name-desc {
  overflow: hidden;
  flex: 1;
}

.gender {
  width: max-content;
  flex: 1;
}

.gender.genderEarth {
  background-color: #50ff50;
}

.gender.genderNamekian {
  background-color: #00ffaa;
}

.gender.genderSaiyan {
  background-color: #ffbb50;
}

.gender.genderCommon {
  background-color: #ffffff;
}

.name {
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  padding-right: 10px;
  margin: 0;
}

.description {
  font-size: 12px;
  color: #aaa;
  text-overflow: ellipsis;
  display: -webkit-box;
  line-clamp: 2;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  line-height: 0.9rem;
}

.info {
  position: absolute;
  width: calc(100% - 30px);
  top: calc(100% - 28px);
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  font-weight: bold;
}

.type:last-child {
  color: #2ee59d;
}

.power-required {
  color: #ff0000;
}

@media screen and (max-width: 1032px) {
    .skill {
        width: 100% !important;
    }
}
</style>
