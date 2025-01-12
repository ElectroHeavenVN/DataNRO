import { createRouter, createWebHistory } from 'vue-router';
import ItemsPage from './views/ItemsPage.vue';
import NPCsPage from './views/NPCsPage.vue';

const routes = [
  // {
  //   path: '/Items',
  //   name: 'Items',
  //   component: ItemsPage,
  // },
  // {
  //   path: '/NPCs',
  //   name: 'NPCs',
  //   component: NPCsPage,
  // },
  // {
  //   path: '/TeaMobi',
  //   name: 'TeaMobi',
  //   component: TeaMobiPage,
  // },
  // {
  //   path: '/HSNR',
  //   name: 'Hồi sinh Ngọc Rồng',
  //   component: HSNRPage,
  // },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
