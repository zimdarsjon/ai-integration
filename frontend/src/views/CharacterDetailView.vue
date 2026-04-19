<script setup lang="ts">
import { onMounted, computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Button from 'primevue/button'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import InputNumber from 'primevue/inputnumber'
import InputText from 'primevue/inputtext'
import Tabs from 'primevue/tabs'
import TabList from 'primevue/tablist'
import Tab from 'primevue/tab'
import TabPanels from 'primevue/tabpanels'
import TabPanel from 'primevue/tabpanel'
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import { useCharacters } from '@/composables/useCharacters'
import { useReference } from '@/composables/useReference'
import client from '@/api/client'
import type { components } from '@/api/generated/schema'

type SpellSummary = components['schemas']['SpellSummaryDto']
type CharacterClassDto = components['schemas']['CharacterClassDto']

const route = useRoute()
const router = useRouter()
const toast = useToast()
const id = Number(route.params.id)

const { character, loading, error, fetchCharacter, applyDamage, heal, updateCurrency, addSpell, removeSpell, prepareSpell, levelUp } = useCharacters()

const showDamage = ref(false)
const showHeal = ref(false)
const actionAmount = ref(1)
const actionLoading = ref(false)

const editingCurrency = ref(false)
const currencyDraft = ref({ cp: 0, sp: 0, ep: 0, gp: 0, pp: 0 })
const currencyLoading = ref(false)

function openCurrencyEdit() {
  const c = character.value?.currency
  currencyDraft.value = {
    cp: c?.cp ?? 0,
    sp: c?.sp ?? 0,
    ep: c?.ep ?? 0,
    gp: c?.gp ?? 0,
    pp: c?.pp ?? 0,
  }
  editingCurrency.value = true
}

async function saveCurrency() {
  currencyLoading.value = true
  try {
    await updateCurrency(id, currencyDraft.value)
    editingCurrency.value = false
    toast.add({ severity: 'success', summary: 'Currency updated', life: 2000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to update currency', life: 3000 })
  } finally {
    currencyLoading.value = false
  }
}

onMounted(() => fetchCharacter(id))

const hpPercent = computed(() => {
  if (!character.value) return 0
  const { currentHP, maxHP } = character.value
  return (maxHP ?? 0) > 0 ? Math.round(((currentHP ?? 0) / (maxHP ?? 1)) * 100) : 0
})

const hpBarColor = computed(() => {
  const pct = hpPercent.value
  if (pct > 60) return '#166534'
  if (pct > 30) return '#a16207'
  return '#9b1c1c'
})

async function submitDamage() {
  actionLoading.value = true
  try {
    await applyDamage(id, actionAmount.value)
    showDamage.value = false
    toast.add({ severity: 'warn', summary: '⚔️ Damage Applied', detail: `-${actionAmount.value} HP`, life: 2000 })
    actionAmount.value = 1
  } finally {
    actionLoading.value = false
  }
}

async function submitHeal() {
  actionLoading.value = true
  try {
    await heal(id, actionAmount.value)
    showHeal.value = false
    toast.add({ severity: 'success', summary: '💚 Healed', detail: `+${actionAmount.value} HP`, life: 2000 })
    actionAmount.value = 1
  } finally {
    actionLoading.value = false
  }
}

function fmt(mod: number | undefined) {
  const m = mod ?? 0
  return m >= 0 ? `+${m}` : `${m}`
}

function profIcon(type: number | undefined) {
  if (type === 3) return { icon: 'pi pi-star-fill', color: '#c89b3c' }
  if (type === 2) return { icon: 'pi pi-circle-fill', color: '#166534' }
  if (type === 1) return { icon: 'pi pi-circle-fill', color: '#a16207' }
  return { icon: 'pi pi-circle', color: '#4a3a24' }
}

// --- Level Up ---

const { classes: referenceClasses, fetchClasses } = useReference()

const showLevelUp = ref(false)
const levelUpClassId = ref<number | null>(null)
const levelUpSubclassId = ref<number | null>(null)
const levelUpHpChoice = ref<'average' | 'roll'>('average')
const levelUpCustomHp = ref(0)
const levelUpLoading = ref(false)

const levelUpClass = computed(() =>
  character.value?.classes?.find(c => c.classId === levelUpClassId.value) ?? null
)

const levelUpRefClass = computed(() =>
  referenceClasses.value.find(c => c.id === levelUpClassId.value) ?? null
)

const nextLevel = computed(() => (levelUpClass.value?.level ?? 0) + 1)

const newFeatures = computed(() => {
  if (!levelUpRefClass.value) return []
  return (levelUpRefClass.value.features ?? []).filter(f => f.level === nextLevel.value)
})

const availableSubclasses = computed(() => {
  if (!levelUpRefClass.value) return []
  return (levelUpRefClass.value.subclasses ?? []).filter(s => s.choiceLevel === nextLevel.value)
})

const needsSubclassChoice = computed(() => {
  if (!levelUpClass.value) return false
  return availableSubclasses.value.length > 0 && !levelUpClass.value.subclassId
})

const selectedSubclass = computed(() =>
  availableSubclasses.value.find(s => s.id === levelUpSubclassId.value) ?? null
)

// New spell slot row at next level (to show what changes)
const nextSlotRow = computed(() => {
  if (!levelUpRefClass.value) return null
  return levelUpRefClass.value.spellSlotProgression?.find(r => r.level === nextLevel.value) ?? null
})

const currentSlotRow = computed(() => {
  if (!levelUpRefClass.value) return null
  return levelUpRefClass.value.spellSlotProgression?.find(r => r.level === (levelUpClass.value?.level ?? 0)) ?? null
})

const nextLimitRow = computed(() =>
  levelUpRefClass.value?.spellLimitProgression?.find(r => r.level === nextLevel.value) ?? null
)
const currentLimitRow = computed(() =>
  levelUpRefClass.value?.spellLimitProgression?.find(r => r.level === (levelUpClass.value?.level ?? 0)) ?? null
)

const newSpellsToChoose = computed(() => {
  const next = nextLimitRow.value?.spellsKnown
  if (next == null) return 0
  const current = currentLimitRow.value?.spellsKnown ?? 0
  return Math.max(0, next - current)
})

const newCantripsToChoose = computed(() => {
  const next = nextLimitRow.value?.cantripsKnown
  if (next == null) return 0
  const current = currentLimitRow.value?.cantripsKnown ?? 0
  return Math.max(0, next - current)
})

const levelUpSpellSearch = ref('')
const levelUpSelectedSpells = ref<number[]>([])
const levelUpSpells = ref<SpellSummary[]>([])
const levelUpSpellsLoading = ref(false)

async function loadLevelUpSpells(classId: number) {
  if (levelUpSpells.value.length) return
  levelUpSpellsLoading.value = true
  try {
    const { data } = await client.get<SpellSummary[]>(`/api/reference/spells?classId=${classId}`)
    levelUpSpells.value = data
  } finally {
    levelUpSpellsLoading.value = false
  }
}

const levelUpCantripOptions = computed(() => {
  const knownIds = new Set(character.value?.spells?.map(s => s.spellId) ?? [])
  const q = levelUpSpellSearch.value.toLowerCase()
  return levelUpSpells.value
    .filter(s => (s.level ?? 0) === 0 && !knownIds.has(s.id))
    .filter(s => !q || s.name?.toLowerCase().includes(q))
})

const levelUpSpellOptions = computed(() => {
  const knownIds = new Set(character.value?.spells?.map(s => s.spellId) ?? [])
  const maxLevel = Math.max(...(levelUpRefClass.value?.spellSlotProgression
    ?.find(r => r.level === nextLevel.value)?.slots
    ?.map((count, i) => count > 0 ? i + 1 : 0) ?? [0]))
  const q = levelUpSpellSearch.value.toLowerCase()
  return levelUpSpells.value
    .filter(s => (s.level ?? 0) > 0 && (s.level ?? 0) <= maxLevel && !knownIds.has(s.id))
    .filter(s => !q || s.name?.toLowerCase().includes(q))
})

function toggleLevelUpSpell(spellId: number, isCantripPick: boolean) {
  const limit = isCantripPick ? newCantripsToChoose.value : newSpellsToChoose.value
  const otherType = isCantripPick
    ? levelUpSelectedSpells.value.filter(id => levelUpSpells.value.find(s => s.id === id && (s.level ?? 0) > 0))
    : levelUpSelectedSpells.value.filter(id => levelUpSpells.value.find(s => s.id === id && (s.level ?? 0) === 0))
  const sameType = isCantripPick
    ? levelUpSelectedSpells.value.filter(id => levelUpSpells.value.find(s => s.id === id && (s.level ?? 0) === 0))
    : levelUpSelectedSpells.value.filter(id => levelUpSpells.value.find(s => s.id === id && (s.level ?? 0) > 0))

  if (sameType.includes(spellId)) {
    levelUpSelectedSpells.value = [...otherType, ...sameType.filter(id => id !== spellId)]
  } else if (sameType.length < limit) {
    levelUpSelectedSpells.value = [...otherType, ...sameType, spellId]
  }
}

const levelUpSpellsValid = computed(() => {
  const hasSpellPicking = newSpellsToChoose.value > 0 || newCantripsToChoose.value > 0
  if (!hasSpellPicking) return true
  const selectedCantrips = levelUpSelectedSpells.value.filter(id =>
    levelUpSpells.value.find(s => s.id === id && (s.level ?? 0) === 0)
  ).length
  const selectedLeveled = levelUpSelectedSpells.value.filter(id =>
    levelUpSpells.value.find(s => s.id === id && (s.level ?? 0) > 0)
  ).length
  return selectedCantrips === newCantripsToChoose.value && selectedLeveled === newSpellsToChoose.value
})

function conMod(): number {
  const con = character.value?.abilityScores?.find(a => a.abbreviation === 'CON')
  return con?.modifier ?? 0
}

function averageHp(hitDie: number): number {
  return Math.max(1, Math.floor(hitDie / 2) + 1 + conMod())
}

const hpIncrease = computed(() => {
  const hitDie = levelUpClass.value?.hitDie ?? 8
  if (levelUpHpChoice.value === 'average') return averageHp(hitDie)
  return Math.max(1, levelUpCustomHp.value + conMod())
})

async function openLevelUp() {
  if (!referenceClasses.value.length) await fetchClasses()
  const allClasses = character.value?.classes ?? []
  levelUpClassId.value = allClasses.length === 1 ? (allClasses[0].classId ?? null) : null
  levelUpSubclassId.value = null
  levelUpHpChoice.value = 'average'
  levelUpCustomHp.value = 1
  levelUpSelectedSpells.value = []
  levelUpSpells.value = []
  levelUpSpellSearch.value = ''
  showLevelUp.value = true
  // watch() handles loading spells when levelUpClassId changes
}

async function confirmLevelUp() {
  if (!levelUpClassId.value) return
  if (needsSubclassChoice.value && !levelUpSubclassId.value) return
  levelUpLoading.value = true
  try {
    await levelUp(id, levelUpClassId.value, hpIncrease.value, levelUpSubclassId.value, levelUpSelectedSpells.value)
    showLevelUp.value = false
    const spellCount = levelUpSelectedSpells.value.length
    const detail = spellCount > 0 ? `+${hpIncrease.value} HP · ${spellCount} spell${spellCount > 1 ? 's' : ''} added` : `+${hpIncrease.value} HP`
    toast.add({ severity: 'success', summary: `Level ${nextLevel.value}!`, detail, life: 3000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Level up failed', life: 3000 })
  } finally {
    levelUpLoading.value = false
  }
}

watch(levelUpClassId, async (classId) => {
  levelUpSelectedSpells.value = []
  levelUpSpells.value = []
  levelUpSpellSearch.value = ''
  if (classId && showLevelUp.value && (newSpellsToChoose.value > 0 || newCantripsToChoose.value > 0)) {
    await loadLevelUpSpells(classId)
  }
})

// --- Spell management ---

// CasterType enum: None=0, Full=1, Half=2, Third=3, Warlock=4
// Preparation style per class:
//   Spellbook (Wizard): add spells to spellbook, then prepare from spellbook
//   Prepared (Cleric, Druid, Paladin): prepare from full class list, no spellbook
//   Known (Bard, Sorcerer, Warlock, Ranger): fixed list, no daily prep toggle needed
const SPELLBOOK_CLASSES = new Set(['Wizard'])
const KNOWN_CASTER_CLASSES = new Set(['Bard', 'Sorcerer', 'Warlock', 'Ranger'])

function spellStyle(cls: CharacterClassDto): 'spellbook' | 'prepared' | 'known' | 'none' {
  if ((cls.casterType ?? 0) === 0) return 'none'
  if (SPELLBOOK_CLASSES.has(cls.className ?? '')) return 'spellbook'
  if (KNOWN_CASTER_CLASSES.has(cls.className ?? '')) return 'known'
  return 'prepared'
}

function abilityMod(abbr: string | null | undefined): number {
  if (!abbr || !character.value) return 0
  const score = character.value.abilityScores?.find(a => a.abbreviation === abbr)
  return score?.modifier ?? 0
}

function preparedLimit(cls: CharacterClassDto): number {
  return Math.max(1, abilityMod(cls.spellcastingAbility) + (cls.level ?? 0))
}

function preparedCount(cls: CharacterClassDto): number {
  const spells = character.value?.spells ?? []
  if (spellStyle(cls) === 'spellbook') {
    // Wizard: count prepared spells in spellbook (level > 0)
    return spells.filter(s => s.isPrepared && (s.level ?? 0) > 0).length
  }
  // Prepared casters: count all prepared leveled spells
  return spells.filter(s => s.isPrepared && (s.level ?? 0) > 0).length
}

// Class spell list loading (for prepared casters + wizard "add to spellbook")
const classSpellLists = ref<Record<number, SpellSummary[]>>({})
const classSpellListsLoading = ref<Record<number, boolean>>({})
const spellSearch = ref<Record<number, string>>({})
const showAddSpellbook = ref<Record<number, boolean>>({})
const showClassList = ref<Record<number, boolean>>({})

async function loadClassSpells(cls: CharacterClassDto) {
  const classId = cls.classId!
  if (classSpellLists.value[classId] !== undefined) return
  classSpellListsLoading.value = { ...classSpellListsLoading.value, [classId]: true }
  try {
    const { data } = await client.get<SpellSummary[]>(`/api/reference/spells?classId=${classId}`)
    classSpellLists.value = { ...classSpellLists.value, [classId]: data }
  } finally {
    classSpellListsLoading.value = { ...classSpellListsLoading.value, [classId]: false }
  }
}

// Highest spell level the character can currently cast, based on available spell slots
const maxAccessibleSpellLevel = computed(() => {
  const slots = character.value?.spellSlots ?? []
  const withSlots = slots.filter(s => (s.totalSlots ?? 0) > 0).map(s => s.slotLevel ?? 0)
  return withSlots.length > 0 ? Math.max(...withSlots) : 0
})

function filteredClassSpells(cls: CharacterClassDto): SpellSummary[] {
  const maxLevel = maxAccessibleSpellLevel.value
  const all = (classSpellLists.value[cls.classId!] ?? []).filter(s => (s.level ?? 0) <= maxLevel)
  const q = (spellSearch.value[cls.classId!] ?? '').toLowerCase()
  return q ? all.filter(s => s.name?.toLowerCase().includes(q)) : all
}

function isInSpellbook(spellId: number): boolean {
  return character.value?.spells?.some(s => s.spellId === spellId) ?? false
}

function isPrepared(spellId: number): boolean {
  return character.value?.spells?.find(s => s.spellId === spellId)?.isPrepared ?? false
}

const spellActionLoading = ref<Record<number, boolean>>({})

async function handlePrepare(cls: CharacterClassDto, spell: SpellSummary) {
  const spellId = spell.id!
  spellActionLoading.value = { ...spellActionLoading.value, [spellId]: true }
  try {
    const currently = isPrepared(spellId)
    await prepareSpell(id, spellId, !currently, {
      name: spell.name ?? '',
      level: spell.level ?? 0,
      school: spell.school ?? '',
      isConcentration: spell.isConcentration ?? false,
      isRitual: spell.isRitual ?? false,
      castingTime: spell.castingTime ?? '',
      range: spell.range ?? '',
      duration: spell.duration ?? '',
    })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to update spell', life: 2000 })
  } finally {
    spellActionLoading.value = { ...spellActionLoading.value, [spellId]: false }
  }
}

async function handleAddToSpellbook(cls: CharacterClassDto, spell: SpellSummary) {
  const spellId = spell.id!
  spellActionLoading.value = { ...spellActionLoading.value, [spellId]: true }
  try {
    await addSpell(id, spellId, {
      name: spell.name ?? '',
      level: spell.level ?? 0,
      school: spell.school ?? '',
      isConcentration: spell.isConcentration ?? false,
      isRitual: spell.isRitual ?? false,
      castingTime: spell.castingTime ?? '',
      range: spell.range ?? '',
      duration: spell.duration ?? '',
    })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to add spell', life: 2000 })
  } finally {
    spellActionLoading.value = { ...spellActionLoading.value, [spellId]: false }
  }
}

async function handleRemoveFromSpellbook(spellId: number) {
  spellActionLoading.value = { ...spellActionLoading.value, [spellId]: true }
  try {
    await removeSpell(id, spellId)
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to remove spell', life: 2000 })
  } finally {
    spellActionLoading.value = { ...spellActionLoading.value, [spellId]: false }
  }
}

function spellLevelLabel(level: number): string {
  if (level === 0) return 'Cantrips'
  const s = ['', 'st', 'nd', 'rd']
  return `${level}${level <= 3 ? s[level] : 'th'} Level`
}

function currencyLabel(key: string) {
  const map: Record<string, { label: string; color: string; emoji: string }> = {
    cp: { label: 'Copper', color: '#8b5e3c', emoji: '🪙' },
    sp: { label: 'Silver', color: '#9ca3af', emoji: '🥈' },
    ep: { label: 'Electrum', color: '#6b8b9b', emoji: '💎' },
    gp: { label: 'Gold', color: '#c89b3c', emoji: '🏅' },
    pp: { label: 'Platinum', color: '#e5e7eb', emoji: '💠' },
  }
  return map[key] ?? { label: key.toUpperCase(), color: '#e8d5b0', emoji: '💰' }
}
</script>

<template>
  <Toast />
  <Dialog v-model:visible="showDamage" header="⚔️ Apply Damage" modal>
    <div class="flex items-center gap-3 py-2">
      <label class="font-medium text-sm" style="color: var(--dnd-gold);">Damage Amount:</label>
      <InputNumber v-model="actionAmount" :min="1" :max="999" />
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showDamage = false" />
      <Button label="Apply Damage" severity="danger" :loading="actionLoading" @click="submitDamage" />
    </template>
  </Dialog>

  <Dialog v-model:visible="showHeal" header="💚 Heal" modal>
    <div class="flex items-center gap-3 py-2">
      <label class="font-medium text-sm" style="color: var(--dnd-gold);">Heal Amount:</label>
      <InputNumber v-model="actionAmount" :min="1" :max="999" />
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showHeal = false" />
      <Button label="Heal" severity="success" :loading="actionLoading" @click="submitHeal" />
    </template>
  </Dialog>

  <!-- Level Up Dialog -->
  <Dialog v-model:visible="showLevelUp" header="⬆️ Level Up" modal style="width: min(600px, 95vw); max-height: 90vh;">
    <div class="space-y-5 py-2 overflow-y-auto" style="max-height: 70vh;">

      <!-- Class selection (multiclass only) -->
      <div v-if="(character?.classes?.length ?? 0) > 1" class="space-y-2">
        <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Which class?</p>
        <div class="flex flex-wrap gap-2">
          <button
            v-for="cls in character!.classes"
            :key="cls.classId"
            class="px-3 py-2 rounded text-sm font-semibold border transition-colors cursor-pointer"
            :style="levelUpClassId === cls.classId
              ? 'background: var(--dnd-gold); color: #0f0a08; border-color: var(--dnd-gold);'
              : 'background: var(--dnd-surface-2); color: var(--dnd-parchment); border-color: transparent;'"
            @click="levelUpClassId = cls.classId ?? null"
          >
            {{ cls.className }} (Lv {{ cls.level }})
          </button>
        </div>
      </div>

      <template v-if="levelUpClassId">
        <!-- Level preview banner -->
        <div class="rounded-lg p-3 text-center" style="background: var(--dnd-gold); color: #0f0a08;">
          <p class="text-xs font-semibold uppercase tracking-widest">Advancing to</p>
          <p class="text-3xl font-bold">{{ levelUpClass?.className }} Level {{ nextLevel }}</p>
        </div>

        <!-- New features -->
        <div v-if="newFeatures.length" class="space-y-2">
          <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">New Features</p>
          <div
            v-for="feat in newFeatures"
            :key="feat.name ?? ''"
            class="rounded-lg p-3 space-y-1"
            style="background: var(--dnd-surface-2);"
          >
            <p class="text-sm font-bold" style="color: var(--dnd-parchment);">{{ feat.name }}</p>
            <p class="text-xs leading-relaxed" style="color: var(--dnd-parchment-dim);">{{ feat.description }}</p>
          </div>
        </div>
        <div v-else class="text-sm" style="color: var(--dnd-parchment-dim);">
          No new class features at this level.
        </div>

        <!-- Subclass selection -->
        <div v-if="needsSubclassChoice" class="space-y-2">
          <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
            Choose your {{ levelUpRefClass?.name }} subclass
          </p>
          <div class="space-y-2">
            <button
              v-for="sub in availableSubclasses"
              :key="sub.id"
              class="w-full text-left rounded-lg p-3 border transition-colors cursor-pointer"
              :style="levelUpSubclassId === sub.id
                ? 'background: color-mix(in srgb, var(--dnd-gold) 15%, var(--dnd-surface-2)); border-color: var(--dnd-gold);'
                : 'background: var(--dnd-surface-2); border-color: transparent;'"
              @click="levelUpSubclassId = sub.id ?? null"
            >
              <p class="text-sm font-bold" style="color: var(--dnd-parchment);">{{ sub.name }}</p>
              <p class="text-xs mt-1 leading-relaxed" style="color: var(--dnd-parchment-dim);">{{ sub.description }}</p>
            </button>
          </div>
          <!-- Preview subclass features granted at this level -->
          <div v-if="selectedSubclass" class="space-y-2 mt-2">
            <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-parchment-dim);">
              Subclass Features
            </p>
            <div
              v-for="sf in (selectedSubclass.features ?? []).filter(f => f.level === nextLevel)"
              :key="sf.name ?? ''"
              class="rounded-lg p-3 space-y-1"
              style="background: var(--dnd-surface-2); border-left: 3px solid var(--dnd-gold);"
            >
              <p class="text-sm font-bold" style="color: var(--dnd-parchment);">{{ sf.name }}</p>
              <p class="text-xs leading-relaxed" style="color: var(--dnd-parchment-dim);">{{ sf.description }}</p>
            </div>
          </div>
        </div>

        <!-- Spell slot changes -->
        <div v-if="nextSlotRow?.slots?.some(s => s > 0)" class="space-y-2">
          <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Spell Slot Changes</p>
          <div class="rounded-lg p-3" style="background: var(--dnd-surface-2);">
            <div class="flex flex-wrap gap-2">
              <template v-for="(count, idx) in nextSlotRow!.slots" :key="idx">
                <div v-if="count > 0" class="text-center">
                  <div class="text-xs" style="color: var(--dnd-parchment-dim);">Lv {{ idx + 1 }}</div>
                  <div class="text-sm font-bold" style="color: var(--dnd-parchment);">
                    {{ currentSlotRow?.slots?.[idx] ?? 0 }}
                    <span style="color: var(--dnd-gold);"> → {{ count }}</span>
                  </div>
                </div>
              </template>
            </div>
          </div>
        </div>

        <!-- Spell selection for Known spellcasters -->
        <div v-if="newCantripsToChoose > 0 || newSpellsToChoose > 0" class="space-y-3">
          <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Choose New Spells</p>

          <div v-if="levelUpSpellsLoading" class="text-sm text-center py-4" style="color: var(--dnd-parchment-dim);">
            <i class="pi pi-spinner pi-spin mr-2" />Loading spell list...
          </div>
          <template v-else>
            <div class="relative">
              <i class="pi pi-search absolute left-3 top-1/2 -translate-y-1/2 text-xs" style="color: var(--dnd-parchment-dim);" />
              <input
                v-model="levelUpSpellSearch"
                placeholder="Search spells..."
                class="w-full pl-8 pr-3 py-2 rounded text-sm"
                style="background: var(--dnd-surface-2); color: var(--dnd-parchment); border: 1px solid transparent; outline: none;"
              />
            </div>

            <!-- Cantrips -->
            <div v-if="newCantripsToChoose > 0" class="space-y-1">
              <p class="text-xs font-semibold" style="color: var(--dnd-parchment-dim);">
                Cantrips
                <span style="color: var(--dnd-gold);">
                  ({{ levelUpSelectedSpells.filter(id => levelUpSpells.find(s => s.id === id && (s.level ?? 0) === 0)).length }}/{{ newCantripsToChoose }})
                </span>
              </p>
              <div class="max-h-40 overflow-y-auto space-y-1 pr-1">
                <button
                  v-for="spell in levelUpCantripOptions"
                  :key="spell.id"
                  class="w-full text-left px-3 py-2 rounded text-sm border transition-colors cursor-pointer"
                  :style="levelUpSelectedSpells.includes(spell.id!)
                    ? 'background: color-mix(in srgb, var(--dnd-gold) 15%, var(--dnd-surface-2)); border-color: var(--dnd-gold);'
                    : 'background: var(--dnd-surface-2); border-color: transparent;'"
                  @click="toggleLevelUpSpell(spell.id!, true)"
                >
                  <span class="font-medium" style="color: var(--dnd-parchment);">{{ spell.name }}</span>
                  <span class="ml-2 text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }}</span>
                </button>
                <p v-if="!levelUpCantripOptions.length" class="text-xs py-2 text-center" style="color: var(--dnd-parchment-dim);">No cantrips available</p>
              </div>
            </div>

            <!-- Leveled spells -->
            <div v-if="newSpellsToChoose > 0" class="space-y-1">
              <p class="text-xs font-semibold" style="color: var(--dnd-parchment-dim);">
                Spells
                <span style="color: var(--dnd-gold);">
                  ({{ levelUpSelectedSpells.filter(id => levelUpSpells.find(s => s.id === id && (s.level ?? 0) > 0)).length }}/{{ newSpellsToChoose }})
                </span>
              </p>
              <div class="max-h-48 overflow-y-auto space-y-1 pr-1">
                <button
                  v-for="spell in levelUpSpellOptions"
                  :key="spell.id"
                  class="w-full text-left px-3 py-2 rounded text-sm border transition-colors cursor-pointer"
                  :style="levelUpSelectedSpells.includes(spell.id!)
                    ? 'background: color-mix(in srgb, var(--dnd-gold) 15%, var(--dnd-surface-2)); border-color: var(--dnd-gold);'
                    : 'background: var(--dnd-surface-2); border-color: transparent;'"
                  @click="toggleLevelUpSpell(spell.id!, false)"
                >
                  <span class="font-medium" style="color: var(--dnd-parchment);">{{ spell.name }}</span>
                  <span class="ml-2 text-xs" style="color: var(--dnd-parchment-dim);">Lv {{ spell.level }} · {{ spell.school }}</span>
                  <span v-if="spell.isConcentration" class="ml-1 text-xs" style="color: var(--dnd-parchment-dim);">· C</span>
                </button>
                <p v-if="!levelUpSpellOptions.length" class="text-xs py-2 text-center" style="color: var(--dnd-parchment-dim);">No spells available</p>
              </div>
            </div>
          </template>
        </div>

        <!-- HP increase -->
        <div class="space-y-3">
          <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Hit Points</p>
          <div class="flex gap-2">
            <button
              class="flex-1 py-2 rounded text-sm font-semibold border cursor-pointer transition-colors"
              :style="levelUpHpChoice === 'average'
                ? 'background: var(--dnd-gold); color: #0f0a08; border-color: var(--dnd-gold);'
                : 'background: var(--dnd-surface-2); color: var(--dnd-parchment); border-color: transparent;'"
              @click="levelUpHpChoice = 'average'"
            >
              Take Average
              <span class="block text-xs font-normal opacity-75">
                d{{ levelUpClass?.hitDie }} avg {{ Math.floor((levelUpClass?.hitDie ?? 8) / 2) + 1 }}
                + {{ conMod() >= 0 ? '+' : '' }}{{ conMod() }} CON = {{ averageHp(levelUpClass?.hitDie ?? 8) }}
              </span>
            </button>
            <button
              class="flex-1 py-2 rounded text-sm font-semibold border cursor-pointer transition-colors"
              :style="levelUpHpChoice === 'roll'
                ? 'background: var(--dnd-gold); color: #0f0a08; border-color: var(--dnd-gold);'
                : 'background: var(--dnd-surface-2); color: var(--dnd-parchment); border-color: transparent;'"
              @click="levelUpHpChoice = 'roll'"
            >
              Roll d{{ levelUpClass?.hitDie }}
              <span class="block text-xs font-normal opacity-75">Enter your roll result</span>
            </button>
          </div>

          <div v-if="levelUpHpChoice === 'roll'" class="flex items-center gap-3">
            <label class="text-sm" style="color: var(--dnd-parchment-dim);">Roll result (1–{{ levelUpClass?.hitDie }}):</label>
            <InputNumber
              v-model="levelUpCustomHp"
              :min="1"
              :max="levelUpClass?.hitDie ?? 20"
            />
          </div>

          <div class="rounded-lg p-3 flex items-center gap-3" style="background: var(--dnd-surface-2);">
            <span class="text-2xl" style="color: var(--dnd-gold);">♥</span>
            <div>
              <p class="text-xs" style="color: var(--dnd-parchment-dim);">HP increase</p>
              <p class="text-2xl font-bold" style="color: var(--dnd-parchment);">+{{ hpIncrease }}</p>
              <p class="text-xs" style="color: var(--dnd-parchment-dim);">
                {{ character?.currentHP }} → {{ (character?.currentHP ?? 0) + hpIncrease }} /
                {{ (character?.maxHP ?? 0) + hpIncrease }} max
              </p>
            </div>
          </div>
        </div>
      </template>
    </div>

    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showLevelUp = false" :disabled="levelUpLoading" />
      <Button
        label="Confirm Level Up"
        icon="pi pi-arrow-up"
        :loading="levelUpLoading"
        :disabled="!levelUpClassId || (needsSubclassChoice && !levelUpSubclassId)"
        style="background: var(--dnd-gold); border-color: var(--dnd-gold); color: #0f0a08;"
        @click="confirmLevelUp"
      />
    </template>
  </Dialog>

  <div class="max-w-6xl mx-auto px-4 py-6">
    <Button icon="pi pi-arrow-left" label="Back" text class="mb-4" style="color: var(--dnd-parchment-dim);" @click="router.back()" />

    <div v-if="loading" class="text-center py-16" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-3xl mb-3" style="color: var(--dnd-gold);" />
      <p>Loading character scroll...</p>
    </div>
    <div v-else-if="error" class="p-4 rounded-lg" style="color: var(--dnd-red-light);">⚠️ {{ error }}</div>
    <div v-else-if="character">

      <!-- Character Header -->
      <div class="dnd-panel rounded-xl p-5 mb-5">
        <div class="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">{{ character.name }}</h1>
            <p class="text-sm mt-1" style="color: var(--dnd-parchment-dim);">
              {{ character.raceName }}<span v-if="character.subraceName"> ({{ character.subraceName }})</span>
              ·
              <span v-for="cls in character.classes" :key="cls.classId ?? 0" class="mr-1">
                {{ cls.className }} {{ cls.level }}
              </span>
              <span v-if="character.alignmentName">· {{ character.alignmentName }}</span>
            </p>
            <p v-if="character.backgroundName" class="text-xs mt-0.5" style="color: var(--dnd-parchment-dim);">
              Background: {{ character.backgroundName }}
            </p>
          </div>
          <div class="flex gap-2 flex-wrap">
            <Button
              v-if="character.inspiration"
              label="Inspired"
              icon="pi pi-star-fill"
              severity="warn"
              size="small"
              disabled
            />
            <Button label="Damage" severity="danger" icon="pi pi-minus" size="small" @click="showDamage = true" />
            <Button label="Heal" severity="success" icon="pi pi-plus" size="small" @click="showHeal = true" />
            <Button label="Level Up" icon="pi pi-arrow-up" size="small" style="background: var(--dnd-gold); border-color: var(--dnd-gold); color: #0f0a08;" @click="openLevelUp" />
          </div>
        </div>

        <!-- HP bar -->
        <div class="mt-4">
          <div class="flex justify-between text-xs mb-1" style="color: var(--dnd-parchment-dim);">
            <span>Hit Points</span>
            <span style="color: var(--dnd-parchment);">
              {{ character.currentHP }}
              <span v-if="(character.tempHP ?? 0) > 0" style="color: #3b82f6;"> +{{ character.tempHP }} temp</span>
              / {{ character.maxHP }}
            </span>
          </div>
          <div class="hp-bar-track">
            <div
              class="hp-bar-fill"
              :style="{ width: hpPercent + '%', background: hpBarColor }"
            />
          </div>
        </div>
      </div>

      <!-- Core Stat Bubbles -->
      <div class="grid grid-cols-4 sm:grid-cols-6 lg:grid-cols-8 gap-2 mb-5">
        <div class="stat-bubble col-span-2">
          <div class="stat-label">AC</div>
          <div class="stat-value">{{ character.ac }}</div>
        </div>
        <div class="stat-bubble col-span-2">
          <div class="stat-label">Speed</div>
          <div class="stat-value">{{ character.speed }}</div>
          <div class="stat-mod">ft.</div>
        </div>
        <div class="stat-bubble col-span-2">
          <div class="stat-label">Prof. Bonus</div>
          <div class="stat-value">+{{ character.proficiencyBonus }}</div>
        </div>
        <div class="stat-bubble col-span-2">
          <div class="stat-label">Level</div>
          <div class="stat-value">{{ character.totalLevel }}</div>
        </div>
        <template v-if="(character.deathSaveSuccesses ?? 0) > 0 || (character.deathSaveFailures ?? 0) > 0">
          <div class="stat-bubble col-span-2">
            <div class="stat-label">Death Saves</div>
            <div class="flex gap-1 mt-0.5">
              <span v-for="i in 3" :key="'s'+i" class="text-xs" :style="i <= (character.deathSaveSuccesses ?? 0) ? 'color: #166534;' : 'color: #3d2a14;'">●</span>
              <span class="text-xs mx-0.5" style="color: var(--dnd-border-bright);">|</span>
              <span v-for="i in 3" :key="'f'+i" class="text-xs" :style="i <= (character.deathSaveFailures ?? 0) ? 'color: #9b1c1c;' : 'color: #3d2a14;'">●</span>
            </div>
          </div>
        </template>
      </div>

      <!-- Tabs -->
      <Tabs value="combat">
        <TabList>
          <Tab value="combat">⚔️ Combat</Tab>
          <Tab value="spells" v-if="character.spellSlots && character.spellSlots.length > 0">✨ Spells</Tab>
          <Tab value="inventory">🎒 Inventory</Tab>
          <Tab value="features">📜 Features</Tab>
          <Tab value="bio">📖 Biography</Tab>
        </TabList>
        <TabPanels>

          <!-- COMBAT TAB -->
          <TabPanel value="combat">
            <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 pt-4">

              <!-- Ability Scores -->
              <div class="dnd-panel rounded-xl p-4">
                <div class="dnd-panel-header">Ability Scores</div>
                <div class="grid grid-cols-2 gap-2">
                  <div
                    v-for="score in character.abilityScores"
                    :key="score.abilityScoreId ?? 0"
                    class="stat-bubble"
                  >
                    <div class="stat-label">{{ score.abbreviation }}</div>
                    <div class="stat-value">{{ score.totalScore }}</div>
                    <div class="stat-mod">{{ fmt(score.modifier) }}</div>
                  </div>
                </div>
              </div>

              <!-- Saving Throws + Skills -->
              <div class="space-y-4">
                <div class="dnd-panel rounded-xl p-4">
                  <div class="dnd-panel-header">Saving Throws</div>
                  <div class="space-y-1.5">
                    <div
                      v-for="st in character.savingThrows"
                      :key="st.abilityScoreId ?? 0"
                      class="flex items-center justify-between text-sm"
                    >
                      <span class="flex items-center gap-2">
                        <i :class="st.isProficient ? 'pi pi-circle-fill' : 'pi pi-circle'" class="text-xs" :style="st.isProficient ? 'color: #166534;' : 'color: #3d2a14;'" />
                        <span style="color: var(--dnd-parchment);">{{ st.name }}</span>
                      </span>
                      <span class="font-bold" style="color: var(--dnd-gold);">{{ fmt(st.bonus) }}</span>
                    </div>
                  </div>
                </div>

                <!-- Conditions -->
                <div v-if="character.conditions && character.conditions.length > 0" class="dnd-panel rounded-xl p-4">
                  <div class="dnd-panel-header">⚠️ Active Conditions</div>
                  <div class="flex flex-wrap gap-2">
                    <Tag
                      v-for="cond in character.conditions"
                      :key="cond.id ?? 0"
                      :value="cond.name ?? ''"
                      severity="danger"
                    />
                  </div>
                </div>

                <!-- Currency -->
                <div class="dnd-panel rounded-xl p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="dnd-panel-header">💰 Currency</div>
                    <Button
                      v-if="!editingCurrency"
                      icon="pi pi-pencil"
                      text
                      size="small"
                      style="color: var(--dnd-gold);"
                      @click="openCurrencyEdit"
                    />
                  </div>

                  <!-- Read-only display -->
                  <div v-if="!editingCurrency" class="grid grid-cols-5 gap-1 text-center">
                    <div v-for="key in ['cp','sp','ep','gp','pp']" :key="key">
                      <div class="text-base font-bold" :style="{ color: currencyLabel(key).color }">
                        {{ (character.currency as Record<string, number> | null | undefined)?.[key] ?? 0 }}
                      </div>
                      <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ currencyLabel(key).label.substring(0,2).toUpperCase() }}</div>
                    </div>
                  </div>

                  <!-- Edit mode -->
                  <div v-else class="space-y-2">
                    <div v-for="key in (['cp','sp','ep','gp','pp'] as const)" :key="key" class="flex items-center justify-between gap-2">
                      <label class="text-xs font-semibold w-16" :style="{ color: currencyLabel(key).color }">
                        {{ currencyLabel(key).emoji }} {{ currencyLabel(key).label }}
                      </label>
                      <InputNumber
                        v-model="currencyDraft[key]"
                        :min="0"
                        :max="999999"
                        fluid
                      />
                    </div>
                    <div class="flex gap-2 pt-1">
                      <Button
                        label="Save"
                        icon="pi pi-check"
                        size="small"
                        :loading="currencyLoading"
                        @click="saveCurrency"
                      />
                      <Button
                        label="Cancel"
                        icon="pi pi-times"
                        size="small"
                        severity="secondary"
                        :disabled="currencyLoading"
                        @click="editingCurrency = false"
                      />
                    </div>
                  </div>
                </div>
              </div>

              <!-- Skills -->
              <div class="dnd-panel rounded-xl p-4">
                <div class="dnd-panel-header">Skills</div>
                <div class="space-y-1 max-h-80 overflow-y-auto pr-1">
                  <div
                    v-for="skill in character.skills"
                    :key="skill.skillId ?? 0"
                    class="flex items-center justify-between text-sm"
                  >
                    <span class="flex items-center gap-1.5">
                      <i
                        :class="profIcon(skill.proficiencyType ?? 0).icon"
                        class="text-xs"
                        :style="{ color: profIcon(skill.proficiencyType ?? 0).color }"
                      />
                      <span style="color: var(--dnd-parchment);">{{ skill.name }}</span>
                      <span class="text-xs" style="color: var(--dnd-parchment-dim);">({{ skill.abilityAbbreviation }})</span>
                    </span>
                    <span class="font-bold text-xs" style="color: var(--dnd-gold);">{{ fmt(skill.bonus) }}</span>
                  </div>
                </div>
                <!-- Hit Dice -->
                <div v-if="character.hitDice && character.hitDice.length > 0" class="mt-4">
                  <div class="dnd-panel-header">Hit Dice</div>
                  <div class="flex flex-wrap gap-2">
                    <div v-for="hd in character.hitDice" :key="hd.className ?? ''" class="text-xs" style="color: var(--dnd-parchment);">
                      {{ hd.remaining }}/{{ hd.total }} d{{ hd.hitDie }} ({{ hd.className }})
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </TabPanel>

          <!-- SPELLS TAB -->
          <TabPanel value="spells">
            <div class="pt-4 space-y-4">

              <!-- Spell Slots -->
              <div class="dnd-panel rounded-xl p-4">
                <div class="dnd-panel-header">✨ Spell Slots</div>
                <div class="grid grid-cols-3 sm:grid-cols-5 lg:grid-cols-9 gap-2 mt-2">
                  <div
                    v-for="slot in character.spellSlots"
                    :key="slot.slotLevel ?? 0"
                    class="text-center rounded p-2"
                    style="background: var(--dnd-surface-2);"
                  >
                    <div class="text-xs mb-1" style="color: var(--dnd-parchment-dim);">Lv {{ slot.slotLevel }}</div>
                    <div class="flex justify-center gap-0.5 flex-wrap mb-1">
                      <span
                        v-for="i in (slot.totalSlots ?? 0)"
                        :key="i"
                        class="w-3 h-3 rounded-full border"
                        :style="i <= ((slot.totalSlots ?? 0) - (slot.usedSlots ?? 0))
                          ? 'background: var(--dnd-gold); border-color: var(--dnd-gold);'
                          : 'background: transparent; border-color: var(--dnd-border-bright);'"
                      />
                    </div>
                    <div class="text-xs font-bold" style="color: var(--dnd-gold);">
                      {{ (slot.totalSlots ?? 0) - (slot.usedSlots ?? 0) }}/{{ slot.totalSlots }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Per-class spell management -->
              <div
                v-for="cls in character.classes?.filter(c => (c.casterType ?? 0) !== 0)"
                :key="cls.classId ?? 0"
                class="dnd-panel rounded-xl p-4 space-y-3"
              >
                <!-- Class header -->
                <div class="flex flex-wrap items-center justify-between gap-2">
                  <div>
                    <span class="font-bold" style="color: var(--dnd-gold);">{{ cls.className }}</span>
                    <span class="text-xs ml-2" style="color: var(--dnd-parchment-dim);">
                      {{ cls.spellcastingAbility }} · Level {{ cls.level }}
                    </span>
                    <span v-if="spellStyle(cls) !== 'known'" class="text-xs ml-2" style="color: var(--dnd-parchment-dim);">
                      · {{ preparedCount(cls) }}/{{ preparedLimit(cls) }} prepared
                    </span>
                  </div>
                  <!-- Load class list button for spellbook/prepared styles -->
                  <Button
                    v-if="spellStyle(cls) === 'spellbook' && !showAddSpellbook[cls.classId!]"
                    label="Add to Spellbook"
                    icon="pi pi-plus"
                    size="small"
                    severity="secondary"
                    @click="showAddSpellbook[cls.classId!] = true; loadClassSpells(cls)"
                  />
                  <Button
                    v-if="spellStyle(cls) === 'prepared'"
                    :label="showClassList[cls.classId!] ? 'Hide Class List' : 'Browse Class List'"
                    :icon="showClassList[cls.classId!] ? 'pi pi-chevron-up' : 'pi pi-list'"
                    size="small"
                    severity="secondary"
                    @click="showClassList[cls.classId!] = !showClassList[cls.classId!]; loadClassSpells(cls)"
                  />
                </div>

                <!-- SPELLBOOK STYLE (Wizard): show spellbook with prepare toggle + remove -->
                <template v-if="spellStyle(cls) === 'spellbook'">
                  <!-- Spellbook spells grouped by level -->
                  <div
                    v-for="spellLevel in [...new Set((character.spells ?? []).map(s => s.level ?? 0))].sort((a,b) => a - b)"
                    :key="spellLevel"
                    class="space-y-1"
                  >
                    <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
                      {{ spellLevelLabel(spellLevel) }}
                    </p>
                    <div
                      v-for="spell in (character.spells ?? []).filter(s => (s.level ?? 0) === spellLevel).sort((a,b) => (a.name ?? '').localeCompare(b.name ?? ''))"
                      :key="spell.spellId ?? 0"
                      class="flex items-center justify-between p-2 rounded gap-2"
                      style="background: var(--dnd-surface-2);"
                    >
                      <div class="flex items-center gap-2 min-w-0">
                        <span class="text-xs font-bold w-5 h-5 rounded-full flex-shrink-0 flex items-center justify-center"
                          :style="spellLevel === 0 ? 'background: #3d2a14; color: var(--dnd-parchment-dim);' : 'background: var(--dnd-gold-dark); color: var(--dnd-parchment);'">
                          {{ spellLevel === 0 ? 'C' : spellLevel }}
                        </span>
                        <div class="min-w-0">
                          <div class="text-sm font-medium truncate" style="color: var(--dnd-parchment);">{{ spell.name }}</div>
                          <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }} · {{ spell.castingTime }}</div>
                        </div>
                      </div>
                      <div class="flex items-center gap-1 flex-shrink-0">
                        <Tag v-if="spell.isConcentration" value="C" severity="info" />
                        <Tag v-if="spell.isRitual" value="R" severity="secondary" />
                        <!-- Prepare toggle (only for leveled spells) -->
                        <Button
                          v-if="spellLevel > 0"
                          :label="spell.isPrepared ? 'Prepared' : 'Prepare'"
                          :icon="spell.isPrepared ? 'pi pi-check-circle' : 'pi pi-circle'"
                          size="small"
                          :severity="spell.isPrepared ? 'success' : 'secondary'"
                          :loading="spellActionLoading[spell.spellId!]"
                          @click="handlePrepare(cls, { id: spell.spellId, name: spell.name, level: spell.level, school: spell.school, isConcentration: spell.isConcentration, isRitual: spell.isRitual, castingTime: spell.castingTime, range: spell.range, duration: spell.duration })"
                        />
                        <Button
                          icon="pi pi-trash"
                          size="small"
                          severity="danger"
                          text
                          :loading="spellActionLoading[spell.spellId!]"
                          @click="handleRemoveFromSpellbook(spell.spellId!)"
                        />
                      </div>
                    </div>
                  </div>
                  <p v-if="!character.spells || character.spells.length === 0" class="text-sm" style="color: var(--dnd-parchment-dim);">
                    Your spellbook is empty. Add spells using the button above.
                  </p>

                  <!-- Add to spellbook panel -->
                  <div v-if="showAddSpellbook[cls.classId!]" class="rounded-lg p-3 space-y-2" style="background: var(--dnd-surface-2);">
                    <div class="flex items-center justify-between">
                      <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Add to Spellbook</p>
                      <Button icon="pi pi-times" text size="small" style="color: var(--dnd-parchment-dim);" @click="showAddSpellbook[cls.classId!] = false" />
                    </div>
                    <InputText
                      v-model="spellSearch[cls.classId!]"
                      placeholder="Search spells..."
                      size="small"
                      fluid
                    />
                    <div v-if="classSpellListsLoading[cls.classId!]" class="text-xs" style="color: var(--dnd-parchment-dim);">Loading...</div>
                    <div v-else class="space-y-1 max-h-60 overflow-y-auto">
                      <div
                        v-for="spell in filteredClassSpells(cls).filter(s => !isInSpellbook(s.id!))"
                        :key="spell.id"
                        class="flex items-center justify-between p-2 rounded"
                        style="background: var(--dnd-surface-1);"
                      >
                        <div class="flex items-center gap-2">
                          <span class="text-xs font-bold w-5 h-5 rounded-full flex items-center justify-center"
                            :style="(spell.level ?? 0) === 0 ? 'background: #3d2a14; color: var(--dnd-parchment-dim);' : 'background: var(--dnd-gold-dark); color: var(--dnd-parchment);'">
                            {{ spell.level === 0 ? 'C' : spell.level }}
                          </span>
                          <div>
                            <div class="text-sm font-medium" style="color: var(--dnd-parchment);">{{ spell.name }}</div>
                            <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }}</div>
                          </div>
                        </div>
                        <Button
                          icon="pi pi-plus"
                          size="small"
                          :loading="spellActionLoading[spell.id!]"
                          @click="handleAddToSpellbook(cls, spell)"
                        />
                      </div>
                      <p v-if="filteredClassSpells(cls).filter(s => !isInSpellbook(s.id!)).length === 0" class="text-xs text-center py-2" style="color: var(--dnd-parchment-dim);">
                        All available spells are in your spellbook.
                      </p>
                    </div>
                  </div>
                </template>

                <!-- PREPARED STYLE (Cleric, Druid, Paladin): always show prepared spells, browse to toggle -->
                <template v-else-if="spellStyle(cls) === 'prepared'">
                  <!-- Currently prepared spells (always visible) -->
                  <div>
                    <p class="text-xs font-semibold uppercase tracking-widest mb-2" style="color: var(--dnd-gold);">Prepared Spells</p>
                    <div
                      v-for="spellLevel in [...new Set((character.spells ?? []).filter(s => s.isPrepared).map(s => s.level ?? 0))].sort((a,b) => a - b)"
                      :key="spellLevel"
                      class="space-y-1 mb-2"
                    >
                      <p class="text-xs font-semibold" style="color: var(--dnd-parchment-dim);">{{ spellLevelLabel(spellLevel) }}</p>
                      <div
                        v-for="spell in (character.spells ?? []).filter(s => s.isPrepared && (s.level ?? 0) === spellLevel)"
                        :key="spell.spellId ?? 0"
                        class="flex items-center justify-between p-2 rounded gap-2"
                        style="background: var(--dnd-surface-2);"
                      >
                        <div class="flex items-center gap-2 min-w-0">
                          <span class="text-xs font-bold w-5 h-5 rounded-full flex-shrink-0 flex items-center justify-center"
                            :style="spellLevel === 0 ? 'background: #3d2a14; color: var(--dnd-parchment-dim);' : 'background: var(--dnd-gold-dark); color: var(--dnd-parchment);'">
                            {{ spellLevel === 0 ? 'C' : spellLevel }}
                          </span>
                          <div class="min-w-0">
                            <div class="text-sm font-medium truncate" style="color: var(--dnd-parchment);">{{ spell.name }}</div>
                            <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }} · {{ spell.castingTime }}</div>
                          </div>
                        </div>
                        <Button
                          label="Unprepare"
                          icon="pi pi-times-circle"
                          size="small"
                          severity="secondary"
                          :loading="spellActionLoading[spell.spellId!]"
                          @click="handlePrepare(cls, { id: spell.spellId, name: spell.name, level: spell.level, school: spell.school, isConcentration: spell.isConcentration, isRitual: spell.isRitual, castingTime: spell.castingTime, range: spell.range, duration: spell.duration })"
                        />
                      </div>
                    </div>
                    <p v-if="!(character.spells ?? []).some(s => s.isPrepared)" class="text-sm" style="color: var(--dnd-parchment-dim);">
                      No spells prepared. Browse the class list to prepare spells.
                    </p>
                  </div>

                  <!-- Full class list (toggleable) -->
                  <div v-if="showClassList[cls.classId!]" class="rounded-lg p-3 space-y-2 border-t" style="border-color: var(--dnd-border-bright);">
                    <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">{{ cls.className }} Spell List</p>
                    <InputText
                      v-model="spellSearch[cls.classId!]"
                      placeholder="Search spells..."
                      size="small"
                      fluid
                    />
                    <div v-if="classSpellListsLoading[cls.classId!]" class="text-xs" style="color: var(--dnd-parchment-dim);">Loading...</div>
                    <div
                      v-for="spellLevel in [...new Set(filteredClassSpells(cls).map(s => s.level ?? 0))].sort((a,b) => a - b)"
                      :key="spellLevel"
                      class="space-y-1"
                    >
                      <p class="text-xs font-semibold" style="color: var(--dnd-parchment-dim);">{{ spellLevelLabel(spellLevel) }}</p>
                      <div
                        v-for="spell in filteredClassSpells(cls).filter(s => (s.level ?? 0) === spellLevel)"
                        :key="spell.id"
                        class="flex items-center justify-between p-2 rounded gap-2"
                        style="background: var(--dnd-surface-2);"
                      >
                        <div class="flex items-center gap-2 min-w-0">
                          <span class="text-xs font-bold w-5 h-5 rounded-full flex-shrink-0 flex items-center justify-center"
                            :style="spellLevel === 0 ? 'background: #3d2a14; color: var(--dnd-parchment-dim);' : 'background: var(--dnd-gold-dark); color: var(--dnd-parchment);'">
                            {{ spellLevel === 0 ? 'C' : spellLevel }}
                          </span>
                          <div class="min-w-0">
                            <div class="text-sm font-medium truncate" style="color: var(--dnd-parchment);">{{ spell.name }}</div>
                            <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }} · {{ spell.castingTime }}</div>
                          </div>
                        </div>
                        <Button
                          :label="isPrepared(spell.id!) ? 'Prepared' : 'Prepare'"
                          :icon="isPrepared(spell.id!) ? 'pi pi-check-circle' : 'pi pi-circle'"
                          size="small"
                          :severity="isPrepared(spell.id!) ? 'success' : 'secondary'"
                          :loading="spellActionLoading[spell.id!]"
                          @click="handlePrepare(cls, spell)"
                        />
                      </div>
                    </div>
                  </div>
                </template>

                <!-- KNOWN STYLE (Bard, Sorcerer, Warlock, Ranger): read-only list -->
                <template v-else-if="spellStyle(cls) === 'known'">
                  <div
                    v-for="spellLevel in [...new Set((character.spells ?? []).map(s => s.level ?? 0))].sort((a,b) => a - b)"
                    :key="spellLevel"
                    class="space-y-1"
                  >
                    <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
                      {{ spellLevelLabel(spellLevel) }}
                    </p>
                    <div
                      v-for="spell in (character.spells ?? []).filter(s => (s.level ?? 0) === spellLevel)"
                      :key="spell.spellId ?? 0"
                      class="flex items-center justify-between p-2 rounded gap-2"
                      style="background: var(--dnd-surface-2);"
                    >
                      <div class="flex items-center gap-2">
                        <span class="text-xs font-bold w-5 h-5 rounded-full flex items-center justify-center"
                          :style="spellLevel === 0 ? 'background: #3d2a14; color: var(--dnd-parchment-dim);' : 'background: var(--dnd-gold-dark); color: var(--dnd-parchment);'">
                          {{ spellLevel === 0 ? 'C' : spellLevel }}
                        </span>
                        <div>
                          <div class="text-sm font-medium" style="color: var(--dnd-parchment);">{{ spell.name }}</div>
                          <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ spell.school }} · {{ spell.castingTime }}</div>
                        </div>
                      </div>
                      <div class="flex gap-1">
                        <Tag v-if="spell.isConcentration" value="Conc." severity="info" />
                        <Tag v-if="spell.isRitual" value="Ritual" severity="secondary" />
                      </div>
                    </div>
                  </div>
                  <p v-if="!character.spells || character.spells.length === 0" class="text-sm" style="color: var(--dnd-parchment-dim);">No spells known.</p>
                </template>
              </div>

            </div>
          </TabPanel>

          <!-- INVENTORY TAB -->
          <TabPanel value="inventory">
            <div class="pt-4">
              <div class="dnd-panel rounded-xl p-4">
                <div class="flex items-center justify-between mb-3">
                  <div class="dnd-panel-header">🎒 Inventory</div>
                  <span class="text-xs" style="color: var(--dnd-parchment-dim);">
                    {{ character.currentWeightLbs?.toFixed(1) ?? 0 }} / {{ character.carryingCapacityLbs?.toFixed(0) ?? '—' }} lb
                  </span>
                </div>

                <div v-if="!character.inventory || character.inventory.length === 0" class="text-sm py-4 text-center" style="color: var(--dnd-parchment-dim);">
                  Your pack is empty.
                </div>
                <div v-else class="space-y-1">
                  <div
                    v-for="item in character.inventory"
                    :key="item.id ?? 0"
                    class="flex items-center justify-between p-2 rounded text-sm"
                    :style="item.isEquipped ? 'background: rgba(200,155,60,0.08); border: 1px solid rgba(200,155,60,0.2);' : 'background: var(--dnd-surface-2);'"
                  >
                    <div class="flex items-center gap-2">
                      <i v-if="item.isEquipped" class="pi pi-shield text-xs" style="color: var(--dnd-gold);" />
                      <i v-else class="pi pi-box text-xs" style="color: var(--dnd-parchment-dim);" />
                      <div>
                        <div style="color: var(--dnd-parchment);">{{ item.name }}</div>
                        <div class="text-xs" style="color: var(--dnd-parchment-dim);">{{ item.itemType }}</div>
                      </div>
                    </div>
                    <div class="flex items-center gap-3 text-xs" style="color: var(--dnd-parchment-dim);">
                      <span>×{{ item.quantity }}</span>
                      <span>{{ item.weightLbs }} lb</span>
                      <Tag v-if="item.isAttuned" value="Attuned" severity="warn" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </TabPanel>

          <!-- FEATURES TAB -->
          <TabPanel value="features">
            <div class="pt-4 space-y-3">
              <div v-if="!character.features || character.features.length === 0" class="text-sm py-4" style="color: var(--dnd-parchment-dim);">
                No features or traits.
              </div>
              <div
                v-for="feat in character.features"
                :key="feat.id ?? 0"
                class="dnd-panel rounded-xl p-4"
              >
                <div class="flex items-start justify-between gap-2 mb-2">
                  <div>
                    <span class="font-bold text-sm" style="color: var(--dnd-parchment);">{{ feat.name }}</span>
                    <span class="text-xs ml-2 px-1.5 py-0.5 rounded" style="background: var(--dnd-surface-2); color: var(--dnd-parchment-dim);">
                      {{ feat.source }}
                    </span>
                  </div>
                  <div v-if="feat.usesMax" class="text-xs font-bold flex items-center gap-1" style="color: var(--dnd-gold);">
                    <i class="pi pi-refresh" />
                    {{ feat.usesRemaining }}/{{ feat.usesMax }}
                  </div>
                </div>
                <p class="text-sm" style="color: var(--dnd-parchment-dim);">{{ feat.description }}</p>
              </div>
            </div>
          </TabPanel>

          <!-- BIO TAB -->
          <TabPanel value="bio">
            <div class="pt-4 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div class="dnd-panel rounded-xl p-4 space-y-3">
                <div class="dnd-panel-header">Appearance</div>
                <div class="grid grid-cols-2 gap-2 text-sm">
                  <div v-if="character.age"><span style="color:var(--dnd-gold)" class="text-xs">Age: </span><span style="color:var(--dnd-parchment)">{{ character.age }}</span></div>
                  <div v-if="character.height"><span style="color:var(--dnd-gold)" class="text-xs">Height: </span><span style="color:var(--dnd-parchment)">{{ character.height }}</span></div>
                  <div v-if="character.weight"><span style="color:var(--dnd-gold)" class="text-xs">Weight: </span><span style="color:var(--dnd-parchment)">{{ character.weight }}</span></div>
                  <div v-if="character.eyes"><span style="color:var(--dnd-gold)" class="text-xs">Eyes: </span><span style="color:var(--dnd-parchment)">{{ character.eyes }}</span></div>
                  <div v-if="character.skin"><span style="color:var(--dnd-gold)" class="text-xs">Skin: </span><span style="color:var(--dnd-parchment)">{{ character.skin }}</span></div>
                  <div v-if="character.hair"><span style="color:var(--dnd-gold)" class="text-xs">Hair: </span><span style="color:var(--dnd-parchment)">{{ character.hair }}</span></div>
                </div>
                <div v-if="character.languages && character.languages.length">
                  <div class="dnd-panel-header mt-2">Languages</div>
                  <div class="flex flex-wrap gap-1">
                    <Tag v-for="lang in character.languages" :key="lang" :value="lang" severity="secondary" />
                  </div>
                </div>
              </div>
              <div class="space-y-3">
                <div v-if="character.personalityTraits" class="dnd-panel rounded-xl p-4">
                  <div class="dnd-panel-header">Personality Traits</div>
                  <p class="text-sm" style="color: var(--dnd-parchment);">{{ character.personalityTraits }}</p>
                </div>
                <div v-if="character.ideals" class="dnd-panel rounded-xl p-4">
                  <div class="dnd-panel-header">Ideals</div>
                  <p class="text-sm" style="color: var(--dnd-parchment);">{{ character.ideals }}</p>
                </div>
                <div v-if="character.bonds" class="dnd-panel rounded-xl p-4">
                  <div class="dnd-panel-header">Bonds</div>
                  <p class="text-sm" style="color: var(--dnd-parchment);">{{ character.bonds }}</p>
                </div>
                <div v-if="character.flaws" class="dnd-panel rounded-xl p-4">
                  <div class="dnd-panel-header">Flaws</div>
                  <p class="text-sm" style="color: var(--dnd-parchment);">{{ character.flaws }}</p>
                </div>
              </div>
              <div v-if="character.backstory" class="md:col-span-2 dnd-panel rounded-xl p-4">
                <div class="dnd-panel-header">Backstory</div>
                <p class="text-sm leading-relaxed" style="color: var(--dnd-parchment);">{{ character.backstory }}</p>
              </div>
            </div>
          </TabPanel>
        </TabPanels>
      </Tabs>
    </div>
  </div>
</template>
