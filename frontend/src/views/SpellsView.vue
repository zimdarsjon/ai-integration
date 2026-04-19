<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import { useReference } from '@/composables/useReference'
import type { SpellDetail } from '@/composables/useReference'

const router = useRouter()
const { spells, loading, error, fetchSpells, fetchSpell } = useReference()

const search = ref('')
const filterLevel = ref<number | null>(null)
const selectedSpell = ref<SpellDetail | null>(null)
const loadingDetail = ref(false)

onMounted(() => fetchSpells())

const filtered = computed(() => {
  let list = spells.value
  if (search.value) {
    const q = search.value.toLowerCase()
    list = list.filter(s => s.name?.toLowerCase().includes(q) || s.school?.toLowerCase().includes(q))
  }
  if (filterLevel.value !== null) {
    list = list.filter(s => s.level === filterLevel.value)
  }
  return list
})

async function openSpell(id: number) {
  loadingDetail.value = true
  selectedSpell.value = await fetchSpell(id)
  loadingDetail.value = false
}

function schoolColor(school: string | undefined | null): string {
  const map: Record<string, string> = {
    Abjuration: '#1e3a5f', Conjuration: '#2d4a1a', Divination: '#3d2a6b',
    Enchantment: '#5f1e3a', Evocation: '#5f1a1a', Illusion: '#1a3d4a',
    Necromancy: '#2a1a3d', Transmutation: '#4a3a1a'
  }
  return map[school ?? ''] ?? '#2a1a0e'
}

</script>

<template>
  <Dialog
    v-if="selectedSpell"
    :visible="true"
    :header="`✨ ${selectedSpell.name}`"
    modal
    class="w-full max-w-lg"
    @update:visible="selectedSpell = null"
  >
    <div class="space-y-3 text-sm">
      <div class="flex flex-wrap gap-2">
        <Tag :value="selectedSpell.level === 0 ? 'Cantrip' : `Level ${selectedSpell.level}`" severity="info" />
        <Tag :value="selectedSpell.school ?? ''" severity="secondary" />
        <Tag v-if="selectedSpell.isConcentration" value="Concentration" severity="warn" />
        <Tag v-if="selectedSpell.isRitual" value="Ritual" severity="success" />
      </div>
      <div class="grid grid-cols-2 gap-2" style="color: var(--dnd-parchment-dim);">
        <div><span style="color:var(--dnd-gold)">Cast Time: </span>{{ selectedSpell.castingTime }}</div>
        <div><span style="color:var(--dnd-gold)">Range: </span>{{ selectedSpell.range }}</div>
        <div><span style="color:var(--dnd-gold)">Duration: </span>{{ selectedSpell.duration }}</div>
        <div><span style="color:var(--dnd-gold)">Classes: </span>{{ selectedSpell.classes?.join(', ') }}</div>
      </div>
      <hr style="border-color: var(--dnd-border);" />
      <p style="color: var(--dnd-parchment);">{{ selectedSpell.description }}</p>
      <p v-if="selectedSpell.atHigherLevels" class="italic" style="color: var(--dnd-parchment-dim);">
        <strong style="color:var(--dnd-gold)">At Higher Levels:</strong> {{ selectedSpell.atHigherLevels }}
      </p>
    </div>
    <template #footer>
      <Button label="Close" severity="secondary" @click="selectedSpell = null" />
    </template>
  </Dialog>

  <div class="max-w-5xl mx-auto px-4 py-8">
    <div class="flex items-center gap-3 mb-6">
      <Button icon="pi pi-arrow-left" text style="color: var(--dnd-parchment-dim);" @click="router.back()" />
      <div>
        <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">✨ Spells</h1>
        <p class="text-sm" style="color: var(--dnd-parchment-dim);">{{ spells.length }} spells in the compendium</p>
      </div>
    </div>

    <!-- Filters -->
    <div class="dnd-panel rounded-xl p-4 mb-5 flex flex-wrap gap-3 items-center">
      <InputText v-model="search" placeholder="Search spells..." class="flex-1 min-w-[200px]" />
      <div class="flex gap-1 flex-wrap">
        <button
          v-for="lvl in [null, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9]"
          :key="String(lvl)"
          class="px-2 py-1 rounded text-xs font-semibold cursor-pointer border-none transition-colors"
          :style="filterLevel === lvl
            ? 'background: var(--dnd-gold); color: #0f0a08;'
            : 'background: var(--dnd-surface-2); color: var(--dnd-parchment-dim); border: 1px solid var(--dnd-border);'"
          @click="filterLevel = lvl"
        >
          {{ lvl === null ? 'All' : lvl === 0 ? 'Cantrip' : `${lvl}` }}
        </button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-12" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-2xl" style="color: var(--dnd-gold);" />
    </div>
    <div v-else-if="error" style="color: var(--dnd-red-light);">⚠️ {{ error }}</div>
    <div v-else class="space-y-1">
      <button
        v-for="spell in filtered"
        :key="spell.id"
        class="w-full flex items-center justify-between p-3 rounded-lg cursor-pointer transition-all hover:scale-[1.005] border-none text-left"
        :style="`background: ${schoolColor(spell.school)}; border: 1px solid var(--dnd-border);`"
        @click="openSpell(spell.id!)"
      >
        <div class="flex items-center gap-3">
          <span class="text-xs font-bold w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0"
            style="background: rgba(200,155,60,0.2); color: var(--dnd-gold);">
            {{ spell.level === 0 ? 'C' : spell.level }}
          </span>
          <div>
            <div class="font-medium text-sm" style="color: var(--dnd-parchment);">{{ spell.name }}</div>
            <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }} · {{ spell.castingTime }}</div>
          </div>
        </div>
        <div class="flex gap-1 flex-wrap">
          <Tag v-if="spell.isConcentration" value="C" severity="warn" />
          <Tag v-if="spell.isRitual" value="R" severity="success" />
        </div>
      </button>

      <div v-if="filtered.length === 0" class="text-center py-8" style="color: var(--dnd-parchment-dim);">
        No spells match your search.
      </div>
    </div>
  </div>
</template>
