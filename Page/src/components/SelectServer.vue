<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n();
</script>

<template>
  <div class="select-server">
    <span style="white-space: nowrap;">{{ t('server') }}</span>
    <select @change="changeServer" :aria-label="t('selectServer')">
      <option v-for = "server in servers" :value="server.id">{{ t(server.name.toLowerCase()) }}</option>
    </select>
  </div>
</template>

<script>
export default {
  data() {
    return {
      selectedServerIndex: 0,
    }
  },
  props: {
    servers: {
      type: Array,
      required: true,
    },
    defaultValue: {
      type: String,
      default: "",
    },
    defaultServerId: {
      type: String,
      default: "",
    },
  },
  methods: {
    changeServer(event) {
      this.$emit('change-server', event);
    }
  },
  mounted() {
    let index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
    if (index !== -1) 
      this.selectedServerIndex = index + 1;
    else 
      this.selectedServerIndex = 1;
    if (this.defaultValue !== "") {
      let serverIndex = this.servers.map(s => s.id).indexOf(this.defaultValue);
      if (serverIndex !== -1) {
        this.selectedServerIndex = serverIndex + 1;
      }
    }
    document.querySelector(".select-server select").selectedIndex = this.selectedServerIndex - 1;
    this.$emit('change-server', { target: document.querySelector(".select-server select") });
  },
}
</script>

<style scoped>

.select-server {
  display: flex; 
  flex-direction: row;
  gap: 20px;
  align-items: center;
}

span {
  font-size: 1.25rem;
  font-weight: bold;
}

select {
  border-style: solid;
  border-color: var(--component-border);
  border-width: 2px;
  padding: 15px;
  background-color: var(--component-bg);
  border-radius: 10px;
  color: var(--component-color);
  outline: none;
  font-size: 1.25rem;
  width: 200px;
  padding: 10px;
  height: fit-content;
}

@media screen and (max-width: 700px) {
  .select-server {
    width: 100%;
  }

  .select-server select {
    width: 100%;
  }
}
</style>