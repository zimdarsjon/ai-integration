import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/useAuthStore'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('@/views/HomeView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/characters',
      component: () => import('@/views/CharacterListView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/characters/new',
      component: () => import('@/views/CharacterCreateView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/characters/:id',
      component: () => import('@/views/CharacterDetailView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/campaigns',
      component: () => import('@/views/CampaignListView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/campaigns/:id',
      component: () => import('@/views/CampaignDetailView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/campaigns/:campaignId/encounters/:encounterId',
      component: () => import('@/views/EncounterTrackerView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/compendium',
      component: () => import('@/views/CompendiumView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/compendium/spells',
      component: () => import('@/views/SpellsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/compendium/monsters',
      component: () => import('@/views/MonstersView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/compendium/items',
      component: () => import('@/views/ItemsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/login',
      component: () => import('@/views/LoginView.vue')
    },
    {
      path: '/register',
      component: () => import('@/views/RegisterView.vue')
    }
  ]
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return '/login'
  }
})

export default router
