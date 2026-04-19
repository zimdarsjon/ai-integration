<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/useAuthStore'
import { useAuth } from '@/composables/useAuth'
import Button from 'primevue/button'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const { logout } = useAuth()

const showNav = computed(() => authStore.isAuthenticated)
const mobileMenuOpen = ref(false)

const navItems = [
  { label: 'Tavern', path: '/', icon: 'pi pi-home' },
  { label: 'Characters', path: '/characters', icon: 'pi pi-user' },
  { label: 'Campaigns', path: '/campaigns', icon: 'pi pi-book' },
  { label: 'Compendium', path: '/compendium', icon: 'pi pi-search' },
]

function isActive(path: string) {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}
</script>

<template>
  <div class="min-h-screen" style="background-color: var(--dnd-bg); color: var(--dnd-parchment);">
    <nav v-if="showNav" class="sticky top-0 z-50 shadow-2xl" style="background: var(--dnd-nav); border-bottom: 2px solid var(--dnd-gold-dark);">
      <div class="max-w-7xl mx-auto px-4 flex items-center justify-between h-14">
        <div class="flex items-center gap-1">
          <!-- Logo -->
          <button
            class="flex items-center gap-2 mr-4 group cursor-pointer border-none bg-transparent p-0"
            @click="router.push('/')"
          >
            <span class="text-2xl">⚔️</span>
            <span class="font-bold text-base hidden sm:block" style="color: var(--dnd-gold); text-shadow: 0 0 12px rgba(200,155,60,0.6);">
              Tome &amp; Scroll
            </span>
          </button>

          <!-- Desktop nav items -->
          <div class="hidden md:flex items-center gap-1">
            <button
              v-for="item in navItems"
              :key="item.path"
              class="flex items-center gap-1.5 px-3 py-1.5 rounded text-sm font-medium transition-all duration-150 cursor-pointer border-none"
              :style="isActive(item.path)
                ? 'background: rgba(200,155,60,0.2); color: var(--dnd-gold); border: 1px solid rgba(200,155,60,0.4);'
                : 'background: transparent; color: var(--dnd-parchment-dim); border: 1px solid transparent;'"
              @click="router.push(item.path)"
            >
              <i :class="item.icon" class="text-xs" />
              {{ item.label }}
            </button>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <span class="text-sm hidden sm:block" style="color: var(--dnd-parchment-dim);">
            {{ authStore.user?.firstName }} {{ authStore.user?.lastName }}
          </span>
          <Button
            label="Leave Tavern"
            icon="pi pi-sign-out"
            size="small"
            severity="secondary"
            outlined
            @click="logout"
          />
          <!-- Mobile menu toggle -->
          <button
            class="md:hidden border-none bg-transparent cursor-pointer p-1"
            style="color: var(--dnd-gold);"
            @click="mobileMenuOpen = !mobileMenuOpen"
          >
            <i class="pi pi-bars text-lg" />
          </button>
        </div>
      </div>

      <!-- Mobile nav -->
      <div v-if="mobileMenuOpen" class="md:hidden px-4 pb-3 pt-1 border-t" style="border-color: var(--dnd-border); background: var(--dnd-nav);">
        <div class="flex flex-col gap-1">
          <button
            v-for="item in navItems"
            :key="item.path"
            class="flex items-center gap-2 px-3 py-2 rounded text-sm cursor-pointer border-none text-left"
            :style="isActive(item.path)
              ? 'background: rgba(200,155,60,0.15); color: var(--dnd-gold);'
              : 'background: transparent; color: var(--dnd-parchment-dim);'"
            @click="router.push(item.path); mobileMenuOpen = false"
          >
            <i :class="item.icon" />
            {{ item.label }}
          </button>
        </div>
      </div>
    </nav>

    <RouterView />
  </div>
</template>
