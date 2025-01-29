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

const getClass = (c) => {
  switch (c) {
    case 0:
      return 'classEarth';
    case 1:
      return 'classNamekian';
    case 2:
      return 'classSaiyan';
    case 4:
      return 'classMonkey';
    default:
    return 'classCommon';
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

const getManaTypeStr = (manaType) => {
  switch (manaType)
  {
    case 0:
      return t('manaTypeKi');
    case 1:
      return t('manaTypeKiPercent');
    case 2:
      return t('manaTypeKi100Percent');
  }
  return "";
}

const getTypeStr = (type) => {
  switch (type)
  {
    case 0:
      return t('skillTypeUnkown');
    case 1:
      return t('skillTypeDoDamage');
    case 2:
      return t('skillTypeRescure');
    case 3:
      return t('skillTypeUseWithoutFocus');
    case 4:
      return t('skillTypeSpecial');
  }
  return "";
}

const formatCooldown = (coolDown) => {
  if (coolDown < 1000) {
    return coolDown + 'ms';
  }
  if (coolDown < 60000) {
    return Math.round((coolDown / 1000) * 100) / 100 + 's';
  }
  if (coolDown < 3600000) {
    return Math.round((coolDown / 60000) * 100) / 100 + 'm';
  }
  return Math.round((coolDown / 3600000) * 100) / 100 + 'h';
}

const formatPowerRequired = (powerRequired) => {
  if (powerRequired < 1000)
    return powerRequired.toString();
  if (powerRequired < 1000000) 
    return (powerRequired / 1000) + 'K';
  if (powerRequired < 1000000000) 
    return (powerRequired / 1000000) + 'M';
  return (powerRequired / 1000000000) + 'B';
}

</script>

<template>
  <div class="skillTemplate" :style="{ width }">
    <div class="badges">
      <div class="badge id" @click="copyToClipboard(id);" @touchstart="copyToClipboard(id);" :title="t('clickToCopySkillID')">
        ID: {{ id }}
        <img src="../assets/Copy.svg" style="height: 10px;" alt="Copy" />
      </div>
      <div style="display: flex; gap: 10px;">
        <div class="badge mana-type" :title="t('manaType') + ': ' + getManaTypeStr(manaUseType)">{{ t('manaType') + ': ' + getManaTypeStr(manaUseType) }}</div>
        <div :class="'badge class ' + getClass(classId)" :title="className + ' (' + classId + ')'">{{ className }}</div>
      </div>
    </div>
    <div class="content">
      <img class="icon" :src="'Icons/' + icon + '.png'" :alt="'Icon ' + icon" :title="'Icon ' + icon"/>
      <div class="info-container">
        <div class="name-desc">
          <h2 class="name" :title=name>{{ name }}</h2>
          <div class="description" :title="breakMultiLine(description)">{{ description }}</div>
        </div>
      </div>
    </div>
    <div class="info">
      <div>
        <span>{{ t('type') }}: </span>
        <span class="type">{{ getTypeStr(type) }}</span>
      </div>
      <div class="damInfo" :title="breakMultiLine(damInfo.replace('#', 'X'))">
        {{ damInfo.substring(0, damInfo.indexOf('#')) }}
        <span v-if="damInfo.indexOf('#') != -1">X</span>
        {{ damInfo.substring(damInfo.indexOf('#') + 1) }}
      </div>
    </div>
    <div v-if="moreInfoShown" class="skills" >
      <details v-for="skill in skills" :key="skill.skillId" class="skill" name="skill">
        <summary>{{ t('level') }} {{ skill.point }}</summary>
        <p>{{ skill.moreInfo }}</p>
        <ul>
          <li>{{ t('skillId') }}: {{ skill.skillId }}</li>
          <li>{{ t('mana') }}: {{ skill.manaUse + (manaUseType != 0 ? '%' : '') }} KI</li>
          <li>{{ t('damage') }}: {{ skill.damage }}</li>
          <li>{{ t('cooldown') }}: {{ formatCooldown(skill.coolDown) }}</li>
          <li>{{ t('powerRequired') }}: {{ formatPowerRequired(skill.powRequire) }}</li>
          <li>{{ t('skillRange') }}: {{ skill.dx }}x{{ skill.dy }}</li>
        </ul>
      </details>
    </div>
    <div class="more-info" @click="toggleMoreInfo">
      <span class="material-icons-round">keyboard_arrow_down</span>
      <p>{{ t('moreInfo') }}</p>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    classId: {
      type: Number,
      required: true,
    },
    className: {
      type: String,
      required: true,
    },
    id: {
      type: Number,
      required: true,
    },
    maxPoint: {
      type: Number,
      required: true,
    },
    manaUseType: {
      type: Number,
      required: true,
    },
    type: {
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
    description: {
      type: String,
      required: true,
    },
    damInfo: {
      type: String,
      required: true,
    },
    skills: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      width: '500px',
      moreInfoShown: false,
    }
  },
  methods: {
    toggleMoreInfo(e) {
      this.moreInfoShown = !this.moreInfoShown;
      if (this.moreInfoShown) {
        //this.width = '1050px';
        e.currentTarget.querySelector('span').style.transform = 'scaleY(-1)';
      } else {
        //this.width = '500px';
        e.currentTarget.querySelector('span').style.transform = 'scaleY(1)';
      }
    }
  }
};
</script>

<style scoped>
.skillTemplate {
  display: flex;
  gap: 10px;
  flex-direction: column;
  background-color: #1c1a23;
  border-radius: 10px;
  padding: 15px;
  color: white;
  position: relative;
  width: 500px;
  height: fit-content;
  min-height: 100px;
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

.badge.mana-type {
  background-color: #50e0ff;
}

.badge.id {
  background-color: #ffff00;
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

.info-container {
  display: flex;
  flex-direction: row;
  font-weight: bold;
  font-size: 12px;
  flex: 1;
  justify-content: space-between;
  overflow: hidden;
}

.class {
  width: max-content;
  flex: 1;
}

.class.classEarth {
  background-color: #50ff50;
}

.class.classNamekian {
  background-color: #00ffaa;
}

.class.classSaiyan {
  background-color: #ffbb50;
}

.class.classCommon {
  background-color: #ffffff;
}

.class.classMonkey {
  background-color: black;
  color: white;
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

.skills {
  display: flex;
  /* flex-direction: row; */
  flex-direction: column;
  gap: 10px;
  /* flex-wrap: wrap;
  height: 300px; */
  overflow-y: scroll;
  font-size: 14px;
  background-color: #221f2c;
  padding: 10px;
}

.skill {
  width: 240px;
}

.skill p {
  font-weight: bold;
  margin: 0;
}

.skill ul {
  padding: 0;
  padding-left: 20px;
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

.name-desc {
  overflow: hidden;
  flex: 1;
}

.info {
  display: flex;
  flex-direction: column;
  font-size: 14px;
  font-weight: bold;
}

.type {
  color: #2ee59d;
}

.damInfo > span {
  color: red;
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
