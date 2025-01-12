<script setup>
const copyToClipboard = (content) => {
  try {
    navigator.clipboard.writeText(content);
  } catch (err) {
    console.error('Failed to copy to clipboard', err);
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
      <div class="badge new-item" v-if="isNewItem" :title="$t('thisIsNewItem')">NEW</div>
      <div class="badge id" @click="copyToClipboard(id);" :title="$t('clickToCopy') + ' ID'">ID: {{ id }}</div>
    </div>
    <div class="content">
      <img class="icon" :src="'Icons/' + icon + '.png'" :alt="'Icon ' + icon" :title="'Icon ' + icon"/>
      <div style="overflow: hidden;">
        <div class="gender-and-name">
          <span class="name" :title=name>{{ name }}</span>
          <div style="padding-top: 5px;">
            <span>{{ $t('gender') }}: </span>
            <span class="gender">{{ gender }}</span>
          </div>
        </div>
        <div class="description" :title="breakMultiLine(description)">{{ description }}</div>
      </div>
    </div>
    <div class="info">
      <div>
        <span>{{ $t('type') }}: </span>
        <span class="type">{{ type }}</span>
      </div>
      <!-- <span class="level">{{ level }}</span> -->
      <div v-if="powerRequired">
        <span>{{ $t('powerRequired') }}: </span>
        <span class="power-required">{{ powerRequired }}</span>
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
      type: String,
      required: true,
    },
    gender: {
      type: String,
      required: true,
    },
    level: {
      type: Number,
      required: true,
    },
    powerRequired: {
      type: String,
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
  width: 70px;
  height: 70px;
  margin-right: 15px;
  font-size: 15px;
}

.gender-and-name {
  display: flex;
  flex-direction: row;
  font-weight: bold;
  font-size: 12px;
}

.gender {
  color: #0ff;
}

.name {
  font-size: 16px;
  display: inline-block;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  flex: 1;
}

.description {
  font-size: 14px;
  color: #bbb;
  padding-top: 5px;
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
</style>
