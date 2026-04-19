<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/useAuthStore'

const router = useRouter()
const authStore = useAuthStore()

const sections = [
  {
    icon: '🧙',
    title: 'Characters',
    subtitle: 'Your Adventurers',
    description: 'Create and track your heroes. Manage HP, spells, inventory, conditions, and full character sheets.',
    path: '/characters',
    cta: 'View Characters',
    color: '#3d1a6b'
  },
  {
    icon: '🗺️',
    title: 'Campaigns',
    subtitle: 'Your Quests',
    description: 'Manage campaigns, track sessions, run encounters, and coordinate with your party.',
    path: '/campaigns',
    cta: 'View Campaigns',
    color: '#1a3d1a'
  },
  {
    icon: '📖',
    title: 'Compendium',
    subtitle: 'Reference Library',
    description: 'Browse spells, monsters, items, races, and classes from the D&D rulebooks.',
    path: '/compendium',
    cta: 'Open Compendium',
    color: '#3d2a1a'
  },
]
</script>

<template>
  <div class="max-w-5xl mx-auto px-4 py-10">
    <!-- Hero -->
    <div class="text-center mb-12">
      <div class="text-6xl mb-4">⚔️</div>
      <h1 class="text-3xl font-bold mb-2" style="color: var(--dnd-gold); text-shadow: 0 0 20px rgba(200,155,60,0.5);">
        Welcome back, {{ authStore.user?.firstName ?? 'Adventurer' }}!
      </h1>
      <p class="text-base" style="color: var(--dnd-parchment-dim);">
        The realm awaits. What shall we do today?
      </p>
    </div>

    <!-- Main action cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
      <button
        v-for="section in sections"
        :key="section.path"
        class="dnd-panel rounded-xl p-6 text-left cursor-pointer transition-all duration-200 hover:scale-[1.02] border-none w-full"
        style="background: var(--dnd-surface);"
        @click="router.push(section.path)"
      >
        <div class="text-4xl mb-3">{{ section.icon }}</div>
        <div class="section-label mb-1">{{ section.subtitle }}</div>
        <h2 class="text-lg font-bold mb-2" style="color: var(--dnd-parchment);">{{ section.title }}</h2>
        <p class="text-sm mb-4" style="color: var(--dnd-parchment-dim);">{{ section.description }}</p>
        <span class="text-xs font-semibold uppercase tracking-wider" style="color: var(--dnd-gold);">
          {{ section.cta }} →
        </span>
      </button>
    </div>

    <!-- Quick tips -->
    <div class="dnd-panel p-5 rounded-xl">
      <div class="dnd-panel-header mb-3">📜 Quick Start</div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 text-sm" style="color: var(--dnd-parchment-dim);">
        <div class="flex gap-2">
          <span style="color: var(--dnd-gold);">1.</span>
          <span>Create or select a <strong style="color:var(--dnd-parchment)">Character</strong> to begin tracking your adventurer.</span>
        </div>
        <div class="flex gap-2">
          <span style="color: var(--dnd-gold);">2.</span>
          <span>Start a <strong style="color:var(--dnd-parchment)">Campaign</strong> and invite your party members.</span>
        </div>
        <div class="flex gap-2">
          <span style="color: var(--dnd-gold);">3.</span>
          <span>Run <strong style="color:var(--dnd-parchment)">Encounters</strong> with the turn-by-turn combat tracker.</span>
        </div>
        <div class="flex gap-2">
          <span style="color: var(--dnd-gold);">4.</span>
          <span>Browse the <strong style="color:var(--dnd-parchment)">Compendium</strong> for spells, monsters, and items.</span>
        </div>
      </div>
    </div>
  </div>
</template>
