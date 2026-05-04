<script setup>
import { useI18n } from 'vue-i18n'
import moment from 'moment/min/moment-with-locales';
import LoadingPage from './LoadingPage.vue';
import LoadMore from './LoadMore.vue';
import PageHeader from './PageHeader.vue';
const { t } = useI18n();
</script>

<template>
	<LoadingPage v-if="loading" />
	<div v-else class="item-options-page">
		<PageHeader :title="t('itemOptions')" :servers="servers" :selectedServerIndex="selectedServerIndex"
			:defaultServerId="defaultServerId" :jsonFileName="'ItemOptionTemplates.json'"
			:placeholder="t('searchItemOption')" :lastUpdated="lastUpdated" :includeIcon="false" @change-server="changeServer"
			@inputText="checkDeleteAll" @search="searchItemOptions" @changeSort="changeSort"
			@inverseSort="inverseSort" />

		<div class="tableWrap">
			<table class="itemOptionsTable">
				<thead>
					<tr>
						<th class="idCol">{{ t('itemOptionId') }}</th>
						<th class="typeCol">{{ t('itemOptionType') }}</th>
						<th>{{ t('itemOptionName') }}</th>
					</tr>
				</thead>
				<tbody>
					<tr v-for="itemOption in visibleItemOptions" :key="itemOption.id">
						<td class="idCol">{{ itemOption.id }}</td>
						<td class="typeCol">{{ itemOption.type }}</td>
						<td class="nameCol">{{ itemOption.name || '[No name]' }}</td>
					</tr>
				</tbody>
			</table>
		</div>
		<LoadMore v-if="filteredItemOptions.length > 30 && visibleItemOptions.length < filteredItemOptions.length"
			@load-more="loadMore" @load-all="loadAll" />
	</div>
</template>

<script>
export default {
	components: {
		LoadingPage,
		LoadMore,
		PageHeader,
	},
	props: {
		servers: {
			type: Array,
			required: true,
		},
		defaultServerId: {
			type: String,
			default: "",
		},
	},
	data() {
		return {
			loading: true,
			reversed: false,
			sortKey: 'id',
			itemOptions: [],
			filteredItemOptions: [],
			visibleItemOptions: [],
			selectedServerIndex: 1,
			lastUpdated: '',
		}
	},
	methods: {
		async getItemOptions() {
			const server = this.servers[this.selectedServerIndex - 1];
			const response = await fetch(server.id + '/ItemOptionTemplates.json');
			const data = await response.json();
			this.itemOptions = data;
			this.filteredItemOptions = [...data];
			this.sortItemOptions();

			const lastUpdatedResponse = await fetch(server.id + '/LastUpdated');
			const lastUpdatedData = await lastUpdatedResponse.text();
			const date = new Date(lastUpdatedData);
			this.lastUpdated = date.toLocaleString() + ' (' + moment(date).fromNow() + ')';

			await new Promise(resolve => setTimeout(resolve, 1000));
			this.loading = false;
		},
		sortItemOptions() {
			switch (this.sortKey) {
				case 'id':
					this.filteredItemOptions.sort((a, b) => a.id - b.id);
					break;
				case 'name':
					this.filteredItemOptions.sort((a, b) => (a.name || '').localeCompare(b.name || ''));
					break;
			}

			if (this.reversed)
				this.filteredItemOptions.reverse();

			this.visibleItemOptions = this.filteredItemOptions.slice(0, 30);
		},
		loadMore() {
			this.visibleItemOptions = this.filteredItemOptions.slice(0, this.visibleItemOptions.length + 30);
		},
		loadAll() {
			this.visibleItemOptions = this.filteredItemOptions;
		},
		changeSort(e) {
			this.sortKey = e.target.value;
			this.sortItemOptions();
		},
		inverseSort(e) {
			this.filteredItemOptions.reverse();
			this.visibleItemOptions = this.filteredItemOptions.slice(0, 30);
			this.reversed = e.reversed;
		},
		checkDeleteAll(e) {
			const search = e.target.value.toLowerCase();
			if (search === '') {
				this.filteredItemOptions = [...this.itemOptions];
				this.sortItemOptions();
				const url = new URL(window.location.href);
				url.searchParams.delete('q');
				window.history.pushState({}, '', url.toString());
			}
		},
		searchItemOptions(e) {
			const search = this.replaceVietnameseChars(e.target.value.toLowerCase());
			const url = new URL(window.location.href);
			if (e?.target?.value) {
				url.searchParams.set('q', e.target.value);
			} else {
				url.searchParams.delete('q');
			}
			window.history.pushState({}, '', url.toString());

			this.filteredItemOptions = this.itemOptions.filter(itemOption =>
				this.replaceVietnameseChars((itemOption.id + '|' + itemOption.type + '|' + (itemOption.name || '')).toLowerCase()).includes(search)
			);
			this.sortItemOptions();
		},
		replaceVietnameseChars(str) {
			return str.replace(/á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/g, 'a')
				.replace(/đ/g, 'd')
				.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/g, 'e')
				.replace(/í|ì|ỉ|ĩ|ị/g, 'i')
				.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/g, 'o')
				.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/g, 'u')
				.replace(/ý|ỳ|ỷ|ỹ|ỵ/g, 'y');
		},
		changeServer(e) {
			if (e.target.selectedIndex === this.selectedServerIndex - 1)
				return;
			this.selectedServerIndex = e.target.selectedIndex + 1;
			this.loading = true;
			this.getItemOptions();
		},
	},
	mounted() {
		const index = this.servers.map(s => s.id).indexOf(this.defaultServerId);
		if (index !== -1)
			this.selectedServerIndex = index + 1;
		else
			this.selectedServerIndex = 1;

		const serverFromURL = new URL(window.location.href).searchParams.get('server');
		if (serverFromURL) {
			const serverIndex = this.servers.map(s => s.id).indexOf(serverFromURL);
			if (serverIndex !== -1) {
				this.selectedServerIndex = serverIndex + 1;
			}
		}

		moment.locale(navigator.language);
		this.getItemOptions();
	},
};
</script>

<style scoped>
.item-options-page {
	width: 100%;
}

.tableWrap {
	overflow-x: auto;
	border: 2px solid var(--component-border);
	border-radius: 12px;
	background-color: var(--component-bg);
}

.itemOptionsTable {
	width: 100%;
	border-collapse: collapse;
	color: var(--component-color);
}

.itemOptionsTable th,
.itemOptionsTable td {
	padding: 12px 14px;
	border-bottom: 1px solid var(--component-border);
	text-align: left;
	vertical-align: top;
}

.itemOptionsTable th {
	position: sticky;
	top: 0;
	background: var(--component-bg);
	z-index: 1;
}

.idCol,
.typeCol {
	width: 110px;
	white-space: nowrap;
}

.nameCol {
	word-break: break-word;
}

.itemOptionsTable tbody tr:nth-child(odd) {
	background: color-mix(in srgb, var(--component-border) 10%, transparent);
}
</style>