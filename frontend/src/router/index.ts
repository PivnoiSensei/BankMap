import { createRouter, createWebHashHistory } from 'vue-router'
const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/admin',
      name: 'home',
      component: () => import('@/components/TheMap.vue'),
      meta: { isAdmin: true,  title: 'Map Vector - Admin' }
    },
    {
      path: '/client',
      name: 'client',
      component: () => import('@/components/TheMap.vue'),
      meta: { isAdmin: false,  title: 'Map Vector - Client' }
    }
  ],
})

router.beforeEach((to, from, next) => {
  const title = to.meta.title as string;

  if (title) {
    document.title = title;
  } else {
    document.title = 'Page';
  }
  
  next();
});

export default router
