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

const getGenderString = (gender) => {
  switch (gender) {
    case 0:
      return t('genderEarth');
    case 1:
      return t('genderNamekian');
    case 2:
      return t('genderSaiyan');
    default:
      return t('genderCommon');
  }
}

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
      <div class="badge id" @click="copyToClipboard(id);" :title="t('clickToCopy') + ' ID'">
        ID: {{ id }}
        <img src="../assets/Copy.svg" style="height: 10px;" alt="Copy" />
      </div>
      <div class="badge new-item" v-if="isNewItem" :title="t('thisIsNewItem')">NEW</div>
    </div>
    <div class="content">
      <img class="icon" :src="'Icons/' + icon + '.png'" :alt="'Icon ' + icon" :title="'Icon ' + icon"/>
      <div class="name-desc-gender">
        <div class="name-desc">
          <h2 class="name" :title=name>{{ name }}</h2>
          <div class="description" :title="breakMultiLine(description)">{{ description }}</div>
        </div>
        <div style="padding-top: 5px; display: flex; flex-direction: row; max-width: max-content;">
          <span style="padding-right: 3px;">{{ t('gender') }}:</span>
          <span class="gender">{{ getGenderString(gender) }}</span>
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
      default: '450px',
    },
    height: {
      type: String,
      default: '100px',
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
  display: flex;
  gap: 10px;
  flex-direction: column;
  background-color: #1c1a23;
  border-radius: 10px;
  padding: 15px;
  color: white;
  position: relative;
  min-width: 275px;
  min-height: 100px;
  justify-content: space-between;
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

.badge.new-item {
  background-color: #ffff00;
}

.badge.id {
  background-color: #37ff00;
  cursor: pointer;
}

.content {
  display: flex;
  padding-top: 3px;
}

.icon {
  width: 75px;
  height: 75px;
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
  color: #0ff;
  width: max-content;
  flex: 1;
}

.name {
  font-size: 1.75em;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  padding-right: 10px;
  margin: 0;
}

.description {
  font-size: 14px;
  color: #aaa;
  text-overflow: ellipsis;
  display: -webkit-box;
  line-clamp: 2;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.info {
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

@media screen and (max-width: 1002px) {
    .item {
        width: 100% !important;
    }
}
</style>
