import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import LoginView from '../views/LoginView.vue'
import MainLayout from '../components/MainLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresGuest: true }
    },
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          redirect: '/send-email'
        },
        {
          path: 'send-email',
          name: 'send-email',
          component: () => import('../views/SendEmailView.vue')
        },
        {
          path: 'contacts',
          name: 'contacts',
          component: () => import('../views/ContactsView.vue')
        },
        {
          path: 'refactor-text',
          name: 'refactor-text',
          component: () => import('../views/RefactorTextView.vue')
        },
        {
          path: 'analyze-message',
          name: 'analyze-message',
          component: () => import('../views/AnalyzeMessageView.vue')
        }
      ]
    }
  ],
})

// Navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next('/send-email')
  } else {
    next()
  }
})

export default router
