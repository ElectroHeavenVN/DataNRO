import { createRouter, createWebHistory } from 'vue-router';
import NotFound from './components/NotFound.vue';

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
  {
    path: '/:pathMatch(.*)*', // Catch all routes
    name: 'NotFound',
    component: NotFound
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
