<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import { useReference } from '@/composables/useReference'
import type { MonsterDetail } from '@/composables/useReference'

const router = useRouter()
const { monsters, loading, error, fetchMonsters, fetchMonster } = useReference()

const search = ref('')
const filterMaxCr = ref<number | null>(null)
const selectedMonster = ref<MonsterDetail | null>(null)

onMounted(() => fetchMonsters())

const filtered = computed(() => {
  let list = monsters.value
  if (search.value) {
    const q = search.value.toLowerCase()
    list = list.filter(m => m.name?.toLowerCase().includes(q) || m.creatureType?.toLowerCase().includes(q))
  }
  if (filterMaxCr.value !== null) {
    list = list.filter(m => (m.challengeRating ?? 0) <= filterMaxCr.value!)
  }
  return list
})

async function openMonster(id: number) {
  selectedMonster.value = await fetchMonster(id)
}

function crColor(cr: number): string {
  if (cr <= 1) return '#2d5a27'
  if (cr <= 5) return '#a16207'
  if (cr <= 10) return '#c05621'
  if (cr <= 20) return '#9b1c1c'
  return '#5b21b6'
}

function formatCr(cr: number): string {
  if (cr === 0.125) return '⅛'
  if (cr === 0.25) return '¼'
  if (cr === 0.5) return '½'
  return String(cr)
}

const crFilters = [null, 1, 5, 10, 15, 20, 30]
</script>

<template>
  <Dialog
    v-if="selectedMonster"
    :visible="true"
    :header="`👹 ${selectedMonster.name}`"
    modal
    class="w-full max-w-2xl"
    @update:visible="selectedMonster = null"
  >
    <div class="space-y-3 text-sm">
      <div class="flex flex-wrap gap-2">
        <Tag :value="`CR ${formatCr(selectedMonster.challengeRating ?? 0)}`" severity="warn" />
        <Tag :value="selectedMonster.creatureType ?? ''" severity="secondary" />
        <Tag :value="selectedMonster.size ?? ''" severity="info" />
        <Tag v-if="selectedMonster.isLegendary" value="Legendary" severity="danger" />
      </div>

      <div class="grid grid-cols-3 gap-2 text-center">
        <div class="stat-bubble">
          <div class="stat-label">AC</div>
          <div class="stat-value">{{ selectedMonster.ac }}</div>
        </div>
        <div class="stat-bubble">
          <div class="stat-label">HP</div>
          <div class="stat-value">{{ selectedMonster.maxHP }}</div>
          <div class="stat-mod">{{ selectedMonster.hitDice }}</div>
        </div>
        <div class="stat-bubble">
          <div class="stat-label">XP</div>
          <div class="stat-value text-lg">{{ selectedMonster.xp?.toLocaleString() }}</div>
        </div>
      </div>

      <div class="grid grid-cols-6 gap-1">
        <div v-for="score in selectedMonster.abilityScores" :key="score.abbreviation ?? ''" class="stat-bubble">
          <div class="stat-label">{{ score.abbreviation }}</div>
          <div class="stat-value text-base">{{ score.score }}</div>
          <div class="stat-mod">{{ (score.modifier ?? 0) >= 0 ? '+' : '' }}{{ score.modifier }}</div>
        </div>
      </div>

      <div v-if="selectedMonster.traits && selectedMonster.traits.length">
        <div class="dnd-panel-header">Traits</div>
        <div v-for="trait in selectedMonster.traits" :key="trait.name ?? ''" class="mb-2">
          <strong style="color:var(--dnd-gold)">{{ trait.name }}.</strong>
          <span style="color:var(--dnd-parchment)"> {{ trait.description }}</span>
        </div>
      </div>

      <div v-if="selectedMonster.actions && selectedMonster.actions.length">
        <div class="dnd-panel-header">Actions</div>
        <div v-for="action in selectedMonster.actions" :key="action.name ?? ''" class="mb-2">
          <strong style="color:var(--dnd-gold)">{{ action.name }}.</strong>
          <span style="color:var(--dnd-parchment)"> {{ action.description }}</span>
        </div>
      </div>
    </div>
    <template #footer>
      <Button label="Close" severity="secondary" @click="selectedMonster = null" />
    </template>
  </Dialog>

  <div class="max-w-5xl mx-auto px-4 py-8">
    <div class="flex items-center gap-3 mb-6">
      <Button icon="pi pi-arrow-left" text style="color: var(--dnd-parchment-dim);" @click="router.back()" />
      <div>
        <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">👹 Monsters</h1>
        <p class="text-sm" style="color: var(--dnd-parchment-dim);">{{ monsters.length }} monsters in the bestiary</p>
      </div>
    </div>

    <div class="dnd-panel rounded-xl p-4 mb-5 flex flex-wrap gap-3 items-center">
      <InputText v-model="search" placeholder="Search monsters..." class="flex-1 min-w-[200px]" />
      <div class="flex gap-1 flex-wrap">
        <span class="text-xs" style="color: var(--dnd-parchment-dim);">Max CR:</span>
        <button
          v-for="cr in crFilters"
          :key="String(cr)"
          class="px-2 py-1 rounded text-xs font-semibold cursor-pointer border-none transition-colors"
          :style="filterMaxCr === cr
            ? 'background: var(--dnd-gold); color: #0f0a08;'
            : 'background: var(--dnd-surface-2); color: var(--dnd-parchment-dim); border: 1px solid var(--dnd-border);'"
          @click="filterMaxCr = cr"
        >
          {{ cr === null ? 'All' : cr }}
        </button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-12" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-2xl" style="color: var(--dnd-gold);" />
    </div>
    <div v-else-if="error" style="color: var(--dnd-red-light);">⚠️ {{ error }}</div>
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
      <button
        v-for="monster in filtered"
        :key="monster.id"
        class="dnd-panel rounded-xl p-4 text-left cursor-pointer hover:scale-[1.02] transition-all border-none w-full"
        @click="openMonster(monster.id!)"
      >
        <div class="flex items-start justify-between gap-2 mb-2">
          <span class="font-bold text-sm" style="color: var(--dnd-parchment);">{{ monster.name }}</span>
          <span
            class="text-xs font-bold px-2 py-0.5 rounded"
            :style="`background: ${crColor(monster.challengeRating ?? 0)}33; color: ${crColor(monster.challengeRating ?? 0)};`"
          >
            CR {{ formatCr(monster.challengeRating ?? 0) }}
          </span>
        </div>
        <div class="text-xs" style="color: var(--dnd-parchment-dim);">
          {{ monster.size }} {{ monster.creatureType }}
        </div>
        <div class="flex gap-3 mt-2 text-xs" style="color: var(--dnd-gold);">
          <span>AC {{ monster.ac }}</span>
          <span>HP {{ monster.maxHP }}</span>
          <span>{{ monster.xp?.toLocaleString() }} XP</span>
        </div>
      </button>
      <div v-if="filtered.length === 0" class="sm:col-span-2 lg:col-span-3 text-center py-8" style="color: var(--dnd-parchment-dim);">
        No monsters match your search.
      </div>
    </div>
  </div>
</template>
