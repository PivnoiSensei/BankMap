import { createRouter, createWebHistory } from 'vue-router'
import TheMap from '../components/TheMap.vue' 

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: TheMap
    }
  ],
})

export default router
