<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const sections = [
  {
    icon: '✨',
    title: 'Spells',
    description: 'Browse the complete spell list. Filter by level, school, or class.',
    path: '/compendium/spells',
    count: 'Cantrips through 9th level'
  },
  {
    icon: '👹',
    title: 'Monsters',
    description: 'Search the monster manual. Filter by Challenge Rating and creature type.',
    path: '/compendium/monsters',
    count: 'CR 0 to CR 30'
  },
  {
    icon: '🎒',
    title: 'Items',
    description: 'Weapons, armor, potions, scrolls, and magic items.',
    path: '/compendium/items',
    count: 'Common through Artifact rarity'
  },
]
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 py-8">
    <div class="mb-8">
      <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">📖 Compendium</h1>
      <p class="text-sm mt-1" style="color: var(--dnd-parchment-dim);">
        Your reference library — spells, monsters, items, and more
      </p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
      <button
        v-for="section in sections"
        :key="section.path"
        class="dnd-panel rounded-xl p-5 text-left cursor-pointer transition-all duration-200 hover:scale-[1.02] border-none w-full"
        @click="router.push(section.path)"
      >
        <div class="text-3xl mb-3">{{ section.icon }}</div>
        <h2 class="text-base font-bold mb-1" style="color: var(--dnd-parchment);">{{ section.title }}</h2>
        <p class="text-sm mb-3" style="color: var(--dnd-parchment-dim);">{{ section.description }}</p>
        <span class="text-xs" style="color: var(--dnd-gold);">{{ section.count }} →</span>
      </button>
    </div>

    <!-- Reference quick-access -->
    <div class="dnd-panel rounded-xl p-5">
      <div class="dnd-panel-header mb-3">📜 D&amp;D 5e Quick Reference</div>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-3 text-sm" style="color: var(--dnd-parchment-dim);">
        <div>
          <div class="section-label mb-1">Conditions</div>
          <ul class="space-y-0.5 text-xs">
            <li>Blinded</li>
            <li>Charmed</li>
            <li>Frightened</li>
            <li>Paralyzed</li>
            <li>Poisoned</li>
            <li>Prone</li>
            <li>Stunned</li>
            <li>Unconscious</li>
          </ul>
        </div>
        <div>
          <div class="section-label mb-1">Action Types</div>
          <ul class="space-y-0.5 text-xs">
            <li>Action</li>
            <li>Bonus Action</li>
            <li>Reaction</li>
            <li>Free Action</li>
            <li>Movement</li>
          </ul>
        </div>
        <div>
          <div class="section-label mb-1">Dice</div>
          <ul class="space-y-0.5 text-xs">
            <li>d4 — Dagger</li>
            <li>d6 — Short Sword</li>
            <li>d8 — Longsword (1H)</li>
            <li>d10 — Longsword (2H)</li>
            <li>d12 — Greataxe</li>
            <li>2d6 — Greatsword</li>
          </ul>
        </div>
        <div>
          <div class="section-label mb-1">Rest Rules</div>
          <ul class="space-y-0.5 text-xs">
            <li>Short Rest: 1 hour</li>
            <li>Long Rest: 8 hours</li>
            <li>SR: Spend Hit Dice</li>
            <li>LR: Full HP + HD½</li>
            <li>LR: All spell slots</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>
