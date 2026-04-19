<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import { useReference } from '@/composables/useReference'
import type { ItemDetail } from '@/composables/useReference'
import type { components } from '@/api/generated/schema'

type ItemType = components['schemas']['ItemType']

const router = useRouter()
const { items, loading, error, fetchItems, fetchItem } = useReference()

const search = ref('')
const filterType = ref<ItemType | null>(null)
const selectedItem = ref<ItemDetail | null>(null)

const itemTypeMap: Record<number, { label: string; icon: string }> = {
  0: { label: 'Weapon', icon: '⚔️' },
  1: { label: 'Armor', icon: '🛡️' },
  2: { label: 'Potion', icon: '🧪' },
  3: { label: 'Other', icon: '📦' },
}

const rarityMap: Record<number, { label: string; color: string }> = {
  0: { label: 'Common', color: '#9ca3af' },
  1: { label: 'Uncommon', color: '#166534' },
  2: { label: 'Rare', color: '#1e40af' },
  3: { label: 'Very Rare', color: '#5b21b6' },
  4: { label: 'Legendary', color: '#c89b3c' },
  5: { label: 'Artifact', color: '#9b1c1c' },
}

const itemTypeFilters: Array<{ value: ItemType | null; label: string }> = [
  { value: null, label: 'All' },
  { value: 0, label: '⚔️ Weapon' },
  { value: 1, label: '🛡️ Armor' },
  { value: 2, label: '🧪 Potion' },
  { value: 3, label: '📦 Other' },
]

onMounted(() => fetchItems())

const filtered = computed(() => {
  let list = items.value
  if (search.value) {
    const q = search.value.toLowerCase()
    list = list.filter(i => i.name?.toLowerCase().includes(q))
  }
  if (filterType.value !== null) {
    list = list.filter(i => i.itemType === filterType.value)
  }
  return list
})

async function openItem(id: number) {
  selectedItem.value = await fetchItem(id)
}

function itemIcon(type: ItemType | undefined | null): string {
  if (type == null) return '📦'
  return itemTypeMap[type]?.icon ?? '📦'
}

function itemLabel(type: ItemType | undefined | null): string {
  if (type == null) return 'Other'
  return itemTypeMap[type]?.label ?? 'Other'
}

function rarityLabel(rarity: components['schemas']['ItemRarity'] | undefined | null): string {
  if (rarity == null) return ''
  return rarityMap[rarity]?.label ?? ''
}

function rarityColor(rarity: components['schemas']['ItemRarity'] | undefined | null): string {
  if (rarity == null) return '#9ca3af'
  return rarityMap[rarity]?.color ?? '#9ca3af'
}

function formatCost(cp: number): string {
  if (!cp) return '—'
  if (cp >= 1000) return `${Math.floor(cp / 1000)} gp`
  if (cp >= 100) return `${Math.floor(cp / 100)} sp`
  return `${cp} cp`
}
</script>

<template>
  <Dialog
    v-if="selectedItem"
    :visible="true"
    :header="`${itemIcon(selectedItem.itemType)} ${selectedItem.name}`"
    modal
    class="w-full max-w-lg"
    @update:visible="selectedItem = null"
  >
    <div class="space-y-3 text-sm">
      <div class="flex flex-wrap gap-2">
        <Tag :value="itemLabel(selectedItem.itemType)" severity="info" />
        <Tag
          v-if="selectedItem.magicItem?.rarity != null"
          :value="rarityLabel(selectedItem.magicItem.rarity)"
          :style="`background: ${rarityColor(selectedItem.magicItem.rarity)}22; color: ${rarityColor(selectedItem.magicItem.rarity)};`"
        />
        <Tag v-if="selectedItem.magicItem?.requiresAttunement" value="Requires Attunement" severity="warn" />
      </div>

      <div class="flex gap-4 text-xs" style="color: var(--dnd-parchment-dim);">
        <span><span style="color:var(--dnd-gold)">Weight:</span> {{ selectedItem.weightLbs }} lb</span>
        <span><span style="color:var(--dnd-gold)">Cost:</span> {{ formatCost(selectedItem.costCp ?? 0) }}</span>
      </div>

      <p style="color: var(--dnd-parchment);">{{ selectedItem.description }}</p>

      <div v-if="selectedItem.weapon">
        <div class="dnd-panel-header">Weapon</div>
        <div class="text-xs space-y-1" style="color: var(--dnd-parchment-dim);">
          <div><span style="color:var(--dnd-gold)">Damage:</span> {{ selectedItem.weapon.damageDice }} {{ selectedItem.weapon.damageType }}</div>
          <div v-if="selectedItem.weapon.normalRangeFt"><span style="color:var(--dnd-gold)">Range:</span> {{ selectedItem.weapon.normalRangeFt }}/{{ selectedItem.weapon.longRangeFt }} ft</div>
          <div v-if="selectedItem.weapon.properties?.length"><span style="color:var(--dnd-gold)">Properties:</span> {{ selectedItem.weapon.properties?.join(', ') }}</div>
        </div>
      </div>

      <div v-if="selectedItem.armor">
        <div class="dnd-panel-header">Armor</div>
        <div class="text-xs" style="color: var(--dnd-parchment-dim);">
          <span style="color:var(--dnd-gold)">Base AC:</span> {{ selectedItem.armor.baseAC }}
          <span v-if="selectedItem.armor.maxDexBonus != null"> + Dex (max {{ selectedItem.armor.maxDexBonus }})</span>
        </div>
      </div>

      <div v-if="selectedItem.magicItem?.properties">
        <div class="dnd-panel-header">Magic Properties</div>
        <p style="color: var(--dnd-parchment);">{{ selectedItem.magicItem.properties }}</p>
      </div>
    </div>
    <template #footer>
      <Button label="Close" severity="secondary" @click="selectedItem = null" />
    </template>
  </Dialog>

  <div class="max-w-5xl mx-auto px-4 py-8">
    <div class="flex items-center gap-3 mb-6">
      <Button icon="pi pi-arrow-left" text style="color: var(--dnd-parchment-dim);" @click="router.back()" />
      <div>
        <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">🎒 Items</h1>
        <p class="text-sm" style="color: var(--dnd-parchment-dim);">{{ items.length }} items in the armory</p>
      </div>
    </div>

    <div class="dnd-panel rounded-xl p-4 mb-5 flex flex-wrap gap-3 items-center">
      <InputText v-model="search" placeholder="Search items..." class="flex-1 min-w-[200px]" />
      <div class="flex gap-1 flex-wrap">
        <button
          v-for="tf in itemTypeFilters"
          :key="String(tf.value)"
          class="px-2 py-1 rounded text-xs font-semibold cursor-pointer border-none transition-colors"
          :style="filterType === tf.value
            ? 'background: var(--dnd-gold); color: #0f0a08;'
            : 'background: var(--dnd-surface-2); color: var(--dnd-parchment-dim); border: 1px solid var(--dnd-border);'"
          @click="filterType = tf.value"
        >{{ tf.label }}</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-12" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-2xl" style="color: var(--dnd-gold);" />
    </div>
    <div v-else-if="error" style="color: var(--dnd-red-light);">⚠️ {{ error }}</div>
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
      <button
        v-for="item in filtered"
        :key="item.id"
        class="dnd-panel rounded-xl p-4 text-left cursor-pointer hover:scale-[1.02] transition-all border-none w-full"
        @click="openItem(item.id!)"
      >
        <div class="flex items-start gap-2 mb-1">
          <span class="text-lg flex-shrink-0">{{ itemIcon(item.itemType) }}</span>
          <div>
            <div class="font-bold text-sm" style="color: var(--dnd-parchment);">{{ item.name }}</div>
            <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ itemLabel(item.itemType) }}</div>
          </div>
        </div>
        <div class="flex gap-3 mt-2 text-xs" style="color: var(--dnd-gold);">
          <span>{{ item.weightLbs }} lb</span>
          <span>{{ formatCost(item.costCp ?? 0) }}</span>
        </div>
      </button>
      <div v-if="filtered.length === 0" class="sm:col-span-2 lg:col-span-3 text-center py-8" style="color: var(--dnd-parchment-dim);">
        No items match your search.
      </div>
    </div>
  </div>
</template>
