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

const getTypeString = (type) => {
  switch (type) {
    case 0:
      return t('typeShirts');
    case 1:
      return t('typePants');
    case 2:
      return t('typeGloves');
    case 3:
      return t('typeShoes');
    case 4:
      return t('typeRadars');
    case 5:
      return t('typeAvatarsAndDisguises');
    case 6:
      return t('typeSenzuBeans');
    case 7:
      return t('typeSkillBooks');
    case 8:
      return t('typeQuestItems');
    case 9:
      return t('typeGolds');
    case 10:  
      return t('typeGreenGems');
    case 11:
      return t('typeBackpacks');
    case 12:
      return t('typeDragonBalls');
    case 13:
      return t('typeCharms');
    case 14:
      return t('typeUpgradeStones');
    case 15:
      return t('typeRubble');
    case 16:
      return t('typeMagicBottle');
    case 18:
      return t('typePet');
    case 22:
      return t('typeSatellites');
    case 23:
      return t('typeFlyPlatforms');
    case 24:
      return t('typeVIPFlyPlatforms');
    case 25:
      return t('typeTenRadarsPacks');
    case 27:
      return t('typeMiscellaneousOrEventItems');
    case 28:
      return t('typeFlags');
    case 29:
      return t('typeConsumableBuffItems');
    case 30:
      return t('typeCrystals');
    case 31:
      return t('typeVietnameseCakes');
    case 32:
      return t('typeTrainingSuites');
    case 33:
      return t('typeCollectionCards');
    case 34:
      return t('typeRubies');
    case 35:
      return t('typeSecretSkillsBooks');
    case 36:
      return t('typeTitles');
    case 37:
      return t('typeSkillBooks2');
    default:
      return  t('typeUnknown') + ' (' + type + ')';
  }
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
  <div class="item" :style="{ width, height }">
    <div class="badges">
      <div class="badge id" @click="copyToClipboard(id);" @touchstart="copyToClipboard(id);" :title="t('clickToCopy') + ' ID'">
        ID: {{ id }}
        <font-awesome-icon icon="fa-solid fa-copy" alt="Copy"/>
      </div>
      <div style="display: flex; gap: 10px;">
        <div class="badge new-item" v-if="isNewItem" :title="t('thisIsNewItem')">{{ t('new') }}</div>
        <div :class="'badge gender ' + getGenderClass(gender)" :title="t('gender') + ': ' + getGenderString(gender)">{{ getGenderString(gender) }}</div>
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
      default: '75px',
    },
    icon: {
      type: Number,
      required: true,
    },
    name: {
      type: String,
      required: true,
    },
    description: {
      type: String,
      required: true,
    },
    id: {
      type: Number,
      required: true,
    },
    type: {
      type: Number,
      required: true,
    },
    gender: {
      type: Number,
      required: true,
    },
    level: {
      type: Number,
      required: true,
    },
    powerRequired: {
      type: Number,
      default: 0,
    },
    isNewItem: {
      type: Boolean,
      default: false,
    },
  },
};
</script>

<style scoped>
.item {
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

.badge.new-item {
  background-color: #ffff00;
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
    .item {
        width: 100% !important;
    }
}
</style>
