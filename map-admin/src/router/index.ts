import { createRouter, createWebHistory } from 'vue-router'
import TheMap from '../components/TheMap.vue' 
// import ClientMap from '../components/ClientMap.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/admin',
      name: 'home',
      component: TheMap
    },
    // {
    //   path: '/client',
    //   name: 'client',
    //   component: ClientMap
    // }
  ],
})

export default router
