<script setup lang="ts">
import { onMounted, ref, reactive, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Select from 'primevue/select'
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import client from '@/api/client'
import { useReference } from '@/composables/useReference'
import type { components } from '@/api/generated/schema'

type CharacterDetail = components['schemas']['CharacterDetailDto']
type RaceDto = components['schemas']['RaceDto']
type ClassDto = components['schemas']['ClassDto']
type SubraceDto = components['schemas']['SubraceDto']
type SubclassDto = components['schemas']['SubclassDto']
type BackgroundDetail = components['schemas']['BackgroundDetailDto']
type SpellSummary = components['schemas']['SpellSummaryDto']

const router = useRouter()
const toast = useToast()
const {
  races, classes, backgrounds, skills, languages,
  fetchRaces, fetchClasses, fetchBackgrounds, fetchBackground, fetchSkills, fetchLanguages
} = useReference()

const submitting = ref(false)
const step = ref(1)

const selectedSpellIds = ref<Set<number>>(new Set())
const spellsByClass = ref<Record<number, SpellSummary[]>>({})
const spellsLoading = ref(false)

const selectedSkillIds = ref<Set<number>>(new Set())
const selectedLanguageIds = ref<Set<number>>(new Set())

const form = reactive({
  name: '',
  raceId: null as number | null,
  subraceId: null as number | null,
  alignmentId: null as number | null,
  backgroundId: null as number | null,
  classes: [{ classId: null as number | null, subclassId: null as number | null, level: 1 }],
  abilityScores: {
    1: 10, 2: 10, 3: 10, 4: 10, 5: 10, 6: 10
  } as Record<number, number>
})

const alignments = [
  { id: 1, name: 'Lawful Good' }, { id: 2, name: 'Neutral Good' }, { id: 3, name: 'Chaotic Good' },
  { id: 4, name: 'Lawful Neutral' }, { id: 5, name: 'True Neutral' }, { id: 6, name: 'Chaotic Neutral' },
  { id: 7, name: 'Lawful Evil' }, { id: 8, name: 'Neutral Evil' }, { id: 9, name: 'Chaotic Evil' }
]

const abilityNames: Record<number, string> = {
  1: 'STR', 2: 'DEX', 3: 'CON', 4: 'INT', 5: 'WIS', 6: 'CHA'
}

const abilityAbbrevToId: Record<string, number> = {
  STR: 1, DEX: 2, CON: 3, INT: 4, WIS: 5, CHA: 6
}

type BonusSource = { amount: number; source: string }

const racialBonusesByAbility = computed(() => {
  const result: Record<number, BonusSource[]> = {}

  const addBonuses = (bonuses: { bonus?: number | null; abbreviation?: string | null }[] | null | undefined, sourceName: string) => {
    for (const b of bonuses ?? []) {
      const id = abilityAbbrevToId[b.abbreviation ?? '']
      if (id && b.bonus) {
        result[id] = result[id] ?? []
        result[id].push({ amount: b.bonus, source: sourceName })
      }
    }
  }

  if (selectedRace.value) {
    addBonuses(selectedRace.value.abilityBonuses, selectedRace.value.name ?? 'Race')
    if (form.subraceId) {
      const sr = selectedRace.value.subraces?.find(s => s.id === form.subraceId)
      if (sr) addBonuses(sr.abilityBonuses, sr.name ?? 'Subrace')
    }
  }

  return result
})

const totalAbilityScores = computed(() => {
  const result: Record<number, number> = {}
  for (const id of [1, 2, 3, 4, 5, 6]) {
    const base = form.abilityScores[id] ?? 10
    const bonus = (racialBonusesByAbility.value[id] ?? []).reduce((sum, b) => sum + b.amount, 0)
    result[id] = base + bonus
  }
  return result
})

const selectedRace = ref<RaceDto | null>(null)
const selectedBackground = ref<BackgroundDetail | null>(null)

function onRaceChange() {
  form.subraceId = null
  selectedRace.value = races.value.find(r => r.id === form.raceId) ?? null
}

function onSubraceChange(cls: typeof form.classes[0]) {
  void cls
}

async function onBackgroundChange() {
  selectedBackground.value = null
  if (form.backgroundId) {
    selectedBackground.value = await fetchBackground(form.backgroundId)
  }
}

function getSelectedClass(classId: number | null): ClassDto | null {
  if (!classId) return null
  return classes.value.find(c => c.id === classId) ?? null
}

function getAvailableSubclasses(classId: number | null, level: number): SubclassDto[] {
  const cls = getSelectedClass(classId)
  if (!cls?.subclasses) return []
  return cls.subclasses.filter(s => (s.choiceLevel ?? 3) <= level)
}

function onClassChange(entry: typeof form.classes[0]) {
  entry.subclassId = null
}

function addClass() {
  form.classes.push({ classId: null, subclassId: null, level: 1 })
}

function removeClass(idx: number) {
  form.classes.splice(idx, 1)
}

// HP calculation: max hit die at level 1, average+1 per additional level, + CON mod × total level
const estimatedHP = computed(() => {
  const conScore = totalAbilityScores.value[3] ?? 10
  const conMod = Math.floor((conScore - 10) / 2)
  let total = 0

  for (const entry of form.classes) {
    const cls = getSelectedClass(entry.classId)
    const die = cls?.hitDie ?? 8
    const avg = Math.ceil(die / 2) + 1
    if (entry.level >= 1) {
      total += die + conMod // level 1: max die
      total += (entry.level - 1) * (avg + conMod) // subsequent levels: average
    }
  }
  return Math.max(1, total)
})

const spellcastingClasses = computed(() =>
  form.classes
    .map(entry => ({ entry, cls: getSelectedClass(entry.classId) }))
    .filter((x): x is { entry: typeof form.classes[0]; cls: ClassDto } =>
      x.cls !== null && (x.cls.casterType ?? 0) !== 0
    )
)

const hasSpellcasters = computed(() => spellcastingClasses.value.length > 0)

const totalSteps = computed(() => hasSpellcasters.value ? 5 : 4)

const stepLabels = computed(() => {
  const labels = ['1. Identity', '2. Build', '3. Skills', '4. Stats']
  if (hasSpellcasters.value) labels.push('5. Spells')
  return labels
})

async function loadSpellsForClass(classId: number) {
  if (spellsByClass.value[classId]) return
  spellsLoading.value = true
  try {
    const { data } = await client.get<SpellSummary[]>(`/api/reference/spells?classId=${classId}`)
    spellsByClass.value = { ...spellsByClass.value, [classId]: data }
  } catch {
    spellsByClass.value = { ...spellsByClass.value, [classId]: [] }
  } finally {
    spellsLoading.value = false
  }
}

watch(
  () => form.classes.map(c => c.classId),
  (classIds) => {
    for (const classId of classIds) {
      if (!classId) continue
      const cls = getSelectedClass(classId)
      if (!cls || (cls.casterType ?? 0) === 0) continue
      loadSpellsForClass(classId)
    }
  },
  { deep: true }
)

watch(hasSpellcasters, (has) => {
  if (!has && step.value > 4) step.value = 4
})

// --- Skill & Language selection helpers ---

const classSkillChoices = computed(() =>
  form.classes
    .map(entry => ({ entry, cls: getSelectedClass(entry.classId) }))
    .filter((x): x is { entry: typeof form.classes[0]; cls: ClassDto } =>
      x.cls !== null && x.cls.skillChoices !== null && x.cls.skillChoices !== undefined
    )
)

function countSkillsFromClass(cls: ClassDto): number {
  return (cls.skillChoices?.availableSkills ?? [])
    .filter(s => selectedSkillIds.value.has(s.id!))
    .length
}

function toggleSkill(id: number, cls: ClassDto) {
  const next = new Set(selectedSkillIds.value)
  if (next.has(id)) {
    next.delete(id)
  } else {
    if (countSkillsFromClass(cls) >= (cls.skillChoices?.numberOfChoices ?? 0)) return
    next.add(id)
  }
  selectedSkillIds.value = next
}

// Languages auto-granted by race (matched by name against the full language list)
const autoGrantedLanguageIds = computed((): Set<number> => {
  const raceNames = new Set((selectedRace.value?.languages ?? []).map(n => n.toLowerCase()))
  const ids = new Set<number>()
  for (const lang of languages.value) {
    if (raceNames.has((lang.name ?? '').toLowerCase())) ids.add(lang.id!)
  }
  return ids
})

// Number of additional language choices from the background
const languageChoiceLimit = computed(() => selectedBackground.value?.languagesGranted ?? 0)

function toggleLanguage(id: number) {
  if (autoGrantedLanguageIds.value.has(id)) return
  const next = new Set(selectedLanguageIds.value)
  if (next.has(id)) {
    next.delete(id)
  } else {
    if (next.size >= languageChoiceLimit.value) return
    next.add(id)
  }
  selectedLanguageIds.value = next
}

// Clear any selected languages that are now auto-granted when race changes
watch(autoGrantedLanguageIds, (grantedIds) => {
  const filtered = new Set([...selectedLanguageIds.value].filter(id => !grantedIds.has(id)))
  if (filtered.size !== selectedLanguageIds.value.size) selectedLanguageIds.value = filtered
})

function spellLevelLabel(level: number): string {
  if (level === 0) return 'Cantrips'
  const suffixes = ['', 'st', 'nd', 'rd']
  return `${level}${level <= 3 ? suffixes[level] : 'th'} Level`
}

function getSpellsForClassByLevel(classId: number, level: number): SpellSummary[] {
  return (spellsByClass.value[classId] ?? []).filter(s => s.level === level)
}

function getAvailableSpellLevels(cls: ClassDto, level: number): number[] {
  const levels: number[] = [0]
  const row = cls.spellSlotProgression?.find(r => r.level === level)
  if (row?.slots) {
    row.slots.forEach((count, idx) => { if (count > 0) levels.push(idx + 1) })
  }
  return levels
}

function slotSummary(cls: ClassDto, level: number): string {
  const row = cls.spellSlotProgression?.find(r => r.level === level)
  if (!row?.slots?.length) return ''
  return row.slots
    .map((count, idx) => count > 0 ? `${count} × ${idx + 1}${idx < 3 ? ['st','nd','rd'][idx] : 'th'}` : null)
    .filter(Boolean)
    .join(', ')
}

// Returns the character-creation spell selection limit for a given class+level+spellLevel.
//
// Priority:
//   cantrips  → always from CantripsKnown
//   leveled   → StartingSpells (if set, e.g. Wizard spellbook)
//             → SpellsKnown   (if set, e.g. Bard / Sorcerer / Warlock / Ranger)
//             → prepared formula: abilityMod + classLevel (Cleric / Druid / Paladin, and Wizard daily limit)
//
// Note: for Wizard, StartingSpells is the spellbook creation limit while the daily
//       prepared limit (INT mod + level) is shown separately as informational text.
function getSpellLimit(cls: ClassDto, classLevel: number, spellLevel: number): number {
  const limitRow = cls.spellLimitProgression?.find(r => r.level === classLevel)
  if (!limitRow) return Infinity
  if (spellLevel === 0) return limitRow.cantripsKnown ?? Infinity
  // Explicit creation-time limit (e.g. Wizard spellbook)
  if (limitRow.startingSpells !== null && limitRow.startingSpells !== undefined) return limitRow.startingSpells
  // Fixed spells known (Bard, Sorcerer, Warlock, Ranger)
  if (limitRow.spellsKnown !== null && limitRow.spellsKnown !== undefined) return limitRow.spellsKnown
  // Prepared formula: ability mod + class level (minimum 1)
  const abilId = { STR: 1, DEX: 2, CON: 3, INT: 4, WIS: 5, CHA: 6 }[cls.spellcastingAbility ?? ''] ?? 0
  const abilMod = abilId ? Math.floor((totalAbilityScores.value[abilId] - 10) / 2) : 0
  return Math.max(1, abilMod + classLevel)
}

// Human-readable label for the limit shown next to each spell group header.
function spellLimitLabel(cls: ClassDto, classLevel: number, spellLevel: number): string {
  if (spellLevel === 0) return 'cantrips'
  const limitRow = cls.spellLimitProgression?.find(r => r.level === classLevel)
  if (!limitRow) return 'spells'
  if (limitRow.startingSpells !== null && limitRow.startingSpells !== undefined) return 'in spellbook'
  if (limitRow.spellsKnown !== null && limitRow.spellsKnown !== undefined) return 'known'
  return 'prepared'
}

// For prepared-formula classes, shows the daily prepared limit as informational text.
// For Wizard, this is INT mod + level (separate from the spellbook selection limit).
function preparedLimitNote(cls: ClassDto, classLevel: number): string {
  const limitRow = cls.spellLimitProgression?.find(r => r.level === classLevel)
  if (!limitRow) return ''
  // Only show note when startingSpells differs from the daily prepared formula
  // (i.e. Wizard and other spellbook classes, or prepared casters)
  const hasFixedKnown = limitRow.spellsKnown !== null && limitRow.spellsKnown !== undefined
  if (hasFixedKnown) return '' // known-spell classes don't have a separate prepared limit
  const abilId = { STR: 1, DEX: 2, CON: 3, INT: 4, WIS: 5, CHA: 6 }[cls.spellcastingAbility ?? ''] ?? 0
  const abilMod = abilId ? Math.floor((totalAbilityScores.value[abilId] - 10) / 2) : 0
  const preparedCount = Math.max(1, abilMod + classLevel)
  const abilLabel = cls.spellcastingAbility ?? 'ability'
  return `Prepares ${preparedCount} per day (${abilLabel} mod + level)`
}

// Count how many of the selected spells belong to a given class+spellLevel bucket.
function countSelected(classId: number, spellLevel: number): number {
  const spellsInBucket = getSpellsForClassByLevel(classId, spellLevel).map(s => s.id!)
  return spellsInBucket.filter(id => selectedSpellIds.value.has(id)).length
}

function toggleSpell(spell: SpellSummary, cls: ClassDto, classLevel: number) {
  const id = spell.id!
  const next = new Set(selectedSpellIds.value)
  if (next.has(id)) {
    next.delete(id)
  } else {
    const limit = getSpellLimit(cls, classLevel, spell.level ?? 0)
    const current = countSelected(cls.id!, spell.level ?? 0)
    if (current >= limit) return // at limit — ignore
    next.add(id)
  }
  selectedSpellIds.value = next
}

onMounted(async () => {
  await Promise.all([fetchRaces(), fetchClasses(), fetchBackgrounds(), fetchSkills(), fetchLanguages()])
})

async function submit() {
  if (!form.name.trim()) {
    toast.add({ severity: 'warn', summary: 'Name required', life: 2000 })
    return
  }
  if (form.classes.some(c => !c.classId)) {
    toast.add({ severity: 'warn', summary: 'Select a class for each entry', life: 2000 })
    return
  }

  submitting.value = true
  try {
    const payload = {
      name: form.name,
      raceId: form.raceId ?? undefined,
      subraceId: form.subraceId ?? undefined,
      alignmentId: form.alignmentId ?? undefined,
      backgroundId: form.backgroundId ?? undefined,
      classes: form.classes.map(c => ({ classId: c.classId!, level: c.level })),
      abilityScores: form.abilityScores,
      skillIds: [...selectedSkillIds.value],
      languageIds: [...selectedLanguageIds.value]
    }
    const { data } = await client.post<CharacterDetail>('/api/character', payload)
    if (selectedSpellIds.value.size > 0) {
      await Promise.all(
        [...selectedSpellIds.value].map(spellId =>
          client.post(`/api/Character/${data.id}/spells/${spellId}`)
        )
      )
    }
    toast.add({ severity: 'success', summary: `${data.name} created!`, life: 3000 })
    router.push(`/characters/${data.id}`)
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to create character', life: 3000 })
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <Toast />
  <div class="max-w-2xl mx-auto px-4 py-8">
    <div class="flex items-center gap-3 mb-6">
      <Button icon="pi pi-arrow-left" text style="color: var(--dnd-parchment-dim);" @click="router.back()" />
      <div>
        <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">🧙 Create Character</h1>
        <p class="text-sm" style="color: var(--dnd-parchment-dim);">Forge your legend</p>
      </div>
    </div>

    <!-- Step tabs -->
    <div class="flex gap-1 mb-6">
      <button
        v-for="(label, idx) in stepLabels"
        :key="idx"
        class="flex-1 py-2 text-sm font-semibold rounded border-none cursor-pointer transition-colors"
        :style="step === idx + 1
          ? 'background: var(--dnd-gold); color: #0f0a08;'
          : 'background: var(--dnd-surface-2); color: var(--dnd-parchment-dim);'"
        @click="step = idx + 1"
      >
        {{ label }}
      </button>
    </div>

    <div class="dnd-panel rounded-xl p-6 space-y-5">

      <!-- STEP 1: Identity -->
      <div v-if="step === 1" class="space-y-4">
        <div>
          <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">
            Character Name *
          </label>
          <InputText v-model="form.name" placeholder="Aragorn, Gandalf, Drizzt..." fluid />
        </div>

        <!-- Race -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Race</label>
            <Select
              v-model="form.raceId"
              :options="races"
              optionLabel="name"
              optionValue="id"
              placeholder="Choose race"
              fluid
              @change="onRaceChange"
            />
          </div>
          <div v-if="selectedRace?.subraces && selectedRace.subraces.length > 0">
            <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Subrace</label>
            <Select
              v-model="form.subraceId"
              :options="selectedRace.subraces"
              optionLabel="name"
              optionValue="id"
              placeholder="Choose subrace"
              fluid
            />
          </div>
        </div>

        <!-- Race description & traits -->
        <div v-if="selectedRace" class="rounded-lg p-3 space-y-2" style="background: var(--dnd-surface-2);">
          <p v-if="selectedRace.description" class="text-sm" style="color: var(--dnd-parchment);">
            {{ selectedRace.description }}
          </p>
          <div v-if="selectedRace.abilityBonuses && selectedRace.abilityBonuses.length" class="flex flex-wrap gap-2">
            <span
              v-for="bonus in selectedRace.abilityBonuses"
              :key="bonus.abbreviation"
              class="text-xs px-2 py-0.5 rounded font-semibold"
              style="background: var(--dnd-gold); color: #0f0a08;"
            >
              +{{ bonus.bonus }} {{ bonus.abbreviation }}
            </span>
          </div>
          <div v-if="selectedRace.traits && selectedRace.traits.length" class="space-y-1 mt-1">
            <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Traits</p>
            <div v-for="trait in selectedRace.traits" :key="trait.name" class="text-xs" style="color: var(--dnd-parchment-dim);">
              <span class="font-semibold" style="color: var(--dnd-parchment);">{{ trait.name }}:</span>
              {{ trait.description }}
            </div>
          </div>
        </div>

        <!-- Subrace description & traits -->
        <div
          v-if="form.subraceId && selectedRace?.subraces"
          class="rounded-lg p-3 space-y-2"
          style="background: var(--dnd-surface-2);"
        >
          <template v-for="sr in selectedRace!.subraces!" :key="sr.id">
            <template v-if="sr.id === form.subraceId">
              <p v-if="sr.description" class="text-sm" style="color: var(--dnd-parchment);">{{ sr.description }}</p>
              <div v-if="sr.abilityBonuses && sr.abilityBonuses.length" class="flex flex-wrap gap-2">
                <span
                  v-for="bonus in sr.abilityBonuses"
                  :key="bonus.abbreviation"
                  class="text-xs px-2 py-0.5 rounded font-semibold"
                  style="background: var(--dnd-gold); color: #0f0a08;"
                >
                  +{{ bonus.bonus }} {{ bonus.abbreviation }}
                </span>
              </div>
              <div v-if="sr.traits && sr.traits.length" class="space-y-1">
                <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Traits</p>
                <div v-for="trait in sr.traits" :key="trait.name" class="text-xs" style="color: var(--dnd-parchment-dim);">
                  <span class="font-semibold" style="color: var(--dnd-parchment);">{{ trait.name }}:</span>
                  {{ trait.description }}
                </div>
              </div>
            </template>
          </template>
        </div>

        <div>
          <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Alignment</label>
          <Select
            v-model="form.alignmentId"
            :options="alignments"
            optionLabel="name"
            optionValue="id"
            placeholder="Choose alignment"
            fluid
          />
        </div>

        <!-- Background dropdown -->
        <div>
          <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Background</label>
          <Select
            v-model="form.backgroundId"
            :options="backgrounds"
            optionLabel="name"
            optionValue="id"
            placeholder="Choose background"
            fluid
            @change="onBackgroundChange"
          />
        </div>

        <!-- Background details -->
        <div v-if="selectedBackground" class="rounded-lg p-3 space-y-2" style="background: var(--dnd-surface-2);">
          <p v-if="selectedBackground.description" class="text-sm" style="color: var(--dnd-parchment);">
            {{ selectedBackground.description }}
          </p>
          <div v-if="selectedBackground.skillProficiencies && selectedBackground.skillProficiencies.length" class="flex flex-wrap gap-2">
            <span
              v-for="skill in selectedBackground.skillProficiencies"
              :key="skill"
              class="text-xs px-2 py-0.5 rounded font-semibold"
              style="background: var(--dnd-surface-1); border: 1px solid var(--dnd-gold); color: var(--dnd-gold);"
            >
              {{ skill }}
            </span>
          </div>
          <p v-if="selectedBackground.toolProficiencyDescription" class="text-xs" style="color: var(--dnd-parchment-dim);">
            <span class="font-semibold" style="color: var(--dnd-parchment);">Tools:</span>
            {{ selectedBackground.toolProficiencyDescription }}
          </p>
          <p v-if="selectedBackground.startingEquipmentDescription" class="text-xs" style="color: var(--dnd-parchment-dim);">
            <span class="font-semibold" style="color: var(--dnd-parchment);">Equipment:</span>
            {{ selectedBackground.startingEquipmentDescription }}
          </p>
          <div v-if="selectedBackground.features && selectedBackground.features.length" class="space-y-1 mt-1">
            <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Features</p>
            <div v-for="feature in selectedBackground.features" :key="feature.id" class="text-xs" style="color: var(--dnd-parchment-dim);">
              <span class="font-semibold" style="color: var(--dnd-parchment);">{{ feature.name }}:</span>
              {{ feature.description }}
            </div>
          </div>
        </div>
      </div>

      <!-- STEP 2: Class Build -->
      <div v-if="step === 2" class="space-y-4">
        <div class="dnd-panel-header">Classes</div>

        <div v-for="(cls, idx) in form.classes" :key="idx" class="space-y-3 p-3 rounded" style="background: var(--dnd-surface-2);">
          <div class="flex gap-3 items-end">
            <div class="flex-1">
              <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Class</label>
              <Select
                v-model="cls.classId"
                :options="classes"
                optionLabel="name"
                optionValue="id"
                placeholder="Choose class"
                fluid
                @change="() => onClassChange(cls)"
              />
            </div>
            <div class="w-24 min-w-0">
              <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Level</label>
              <InputNumber v-model="cls.level" :min="1" :max="20" fluid @update:model-value="() => { if (cls.subclassId && !getAvailableSubclasses(cls.classId, cls.level).find(s => s.id === cls.subclassId)) cls.subclassId = null }" />
            </div>
            <Button
              v-if="form.classes.length > 1"
              icon="pi pi-trash"
              severity="danger"
              text
              size="small"
              @click="removeClass(idx)"
            />
          </div>

          <!-- Class description & features -->
          <div v-if="getSelectedClass(cls.classId)" class="space-y-2">
            <div class="text-xs space-y-1" style="color: var(--dnd-parchment-dim);">
              <p v-if="getSelectedClass(cls.classId)?.description" style="color: var(--dnd-parchment);">
                {{ getSelectedClass(cls.classId)?.description }}
              </p>
              <p>
                <span class="font-semibold" style="color: var(--dnd-gold);">Hit Die:</span>
                d{{ getSelectedClass(cls.classId)?.hitDie }}
              </p>
              <p v-if="getSelectedClass(cls.classId)?.savingThrows?.length">
                <span class="font-semibold" style="color: var(--dnd-gold);">Saves:</span>
                {{ getSelectedClass(cls.classId)?.savingThrows?.join(', ') }}
              </p>
            </div>

            <!-- Class features up to current level -->
            <div
              v-if="getSelectedClass(cls.classId)?.features?.filter(f => (f.level ?? 1) <= cls.level).length"
              class="space-y-1"
            >
              <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Features (Level {{ cls.level }})</p>
              <div
                v-for="feature in getSelectedClass(cls.classId)?.features?.filter(f => (f.level ?? 1) <= cls.level)"
                :key="feature.name"
                class="text-xs"
                style="color: var(--dnd-parchment-dim);"
              >
                <span class="font-semibold" style="color: var(--dnd-parchment);">
                  [Lv{{ feature.level }}] {{ feature.name }}:
                </span>
                {{ feature.description }}
              </div>
            </div>

            <!-- Subclass selection -->
            <div v-if="getAvailableSubclasses(cls.classId, cls.level).length">
              <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">Subclass</label>
              <Select
                v-model="cls.subclassId"
                :options="getAvailableSubclasses(cls.classId, cls.level)"
                optionLabel="name"
                optionValue="id"
                placeholder="Choose subclass"
                fluid
              />
              <!-- Subclass description -->
              <div
                v-if="cls.subclassId"
                class="mt-2 space-y-1"
              >
                <template
                  v-for="sc in getAvailableSubclasses(cls.classId, cls.level)"
                  :key="sc.id"
                >
                  <template v-if="sc.id === cls.subclassId">
                    <p v-if="sc.description" class="text-xs" style="color: var(--dnd-parchment);">{{ sc.description }}</p>
                    <div
                      v-if="sc.features?.filter(f => (f.level ?? 1) <= cls.level).length"
                      class="space-y-1"
                    >
                      <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Subclass Features</p>
                      <div
                        v-for="sf in sc.features?.filter(f => (f.level ?? 1) <= cls.level)"
                        :key="sf.name"
                        class="text-xs"
                        style="color: var(--dnd-parchment-dim);"
                      >
                        <span class="font-semibold" style="color: var(--dnd-parchment);">
                          [Lv{{ sf.level }}] {{ sf.name }}:
                        </span>
                        {{ sf.description }}
                      </div>
                    </div>
                  </template>
                </template>
              </div>
            </div>
          </div>
        </div>

        <Button
          label="Add Class (Multiclass)"
          icon="pi pi-plus"
          severity="secondary"
          size="small"
          @click="addClass"
        />

        <!-- HP preview -->
        <div class="rounded-lg p-3 flex items-center gap-3" style="background: var(--dnd-surface-2);">
          <span class="text-lg font-bold" style="color: var(--dnd-gold);">♥</span>
          <div>
            <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Estimated Starting HP</p>
            <p class="text-2xl font-bold" style="color: var(--dnd-parchment);">{{ estimatedHP }}</p>
            <p class="text-xs" style="color: var(--dnd-parchment-dim);">Based on class hit dice + CON modifier (set in Stats step)</p>
          </div>
        </div>
      </div>

      <!-- STEP 3: Skills & Languages -->
      <div v-if="step === 3" class="space-y-6">
        <div class="dnd-panel-header">Skills &amp; Languages</div>

        <!-- Class skill choices -->
        <div v-if="classSkillChoices.length" class="space-y-5">
          <div v-for="{ cls } in classSkillChoices" :key="cls.id" class="space-y-2">
            <div class="flex items-baseline gap-2">
              <p class="text-sm font-bold" style="color: var(--dnd-gold);">{{ cls.name }} Skills</p>
              <span class="text-xs" style="color: var(--dnd-parchment-dim);">
                Choose {{ cls.skillChoices!.numberOfChoices }}
                ({{ countSkillsFromClass(cls) }}/{{ cls.skillChoices!.numberOfChoices }} selected)
              </span>
            </div>
            <div class="grid grid-cols-2 sm:grid-cols-3 gap-1.5">
              <button
                v-for="skill in cls.skillChoices!.availableSkills"
                :key="skill.id"
                class="flex items-center gap-2 rounded px-3 py-2 text-left text-xs border transition-colors"
                :class="selectedSkillIds.has(skill.id!) || countSkillsFromClass(cls) < cls.skillChoices!.numberOfChoices
                  ? 'cursor-pointer'
                  : 'cursor-not-allowed opacity-40'"
                :style="selectedSkillIds.has(skill.id!)
                  ? 'background: var(--dnd-gold); color: #0f0a08; border-color: var(--dnd-gold);'
                  : 'background: var(--dnd-surface-2); color: var(--dnd-parchment); border-color: transparent;'"
                @click="toggleSkill(skill.id!, cls)"
              >
                <span
                  class="w-3.5 h-3.5 rounded-sm border flex-shrink-0 flex items-center justify-center text-xs font-bold"
                  :style="selectedSkillIds.has(skill.id!)
                    ? 'border-color: #0f0a08; color: #0f0a08;'
                    : 'border-color: var(--dnd-gold); color: var(--dnd-gold);'"
                >
                  {{ selectedSkillIds.has(skill.id!) ? '✓' : '' }}
                </span>
                <span class="flex-1 font-semibold">{{ skill.name }}</span>
                <span class="opacity-60">{{ skill.abilityAbbreviation }}</span>
              </button>
            </div>
          </div>
        </div>

        <div v-if="!classSkillChoices.length" class="text-sm" style="color: var(--dnd-parchment-dim);">
          No skill choices required — select a class in the Build step to see available skills.
        </div>

        <!-- Background skills (display only) -->
        <div v-if="selectedBackground?.skillProficiencies?.length" class="rounded-lg p-3 space-y-2" style="background: var(--dnd-surface-2);">
          <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Background Skills (Auto-granted)</p>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="skill in selectedBackground.skillProficiencies"
              :key="skill"
              class="text-xs px-2 py-0.5 rounded font-semibold"
              style="background: var(--dnd-surface-1); border: 1px solid var(--dnd-gold); color: var(--dnd-gold);"
            >
              {{ skill }}
            </span>
          </div>
        </div>

        <!-- Language selection -->
        <div class="space-y-2">
          <div class="flex items-baseline gap-2">
            <p class="text-sm font-bold" style="color: var(--dnd-gold);">Languages</p>
            <span class="text-xs" style="color: var(--dnd-parchment-dim);">
              <template v-if="languageChoiceLimit > 0">
                Choose {{ languageChoiceLimit }} ({{ selectedLanguageIds.size }}/{{ languageChoiceLimit }} selected)
              </template>
              <template v-else>No additional language choices</template>
            </span>
          </div>

          <!-- Auto-granted language badges -->
          <div v-if="autoGrantedLanguageIds.size > 0" class="space-y-1">
            <p class="text-xs" style="color: var(--dnd-parchment-dim);">Auto-granted by race:</p>
            <div class="flex flex-wrap gap-2">
              <span
                v-for="lang in languages.filter(l => autoGrantedLanguageIds.has(l.id!))"
                :key="lang.id"
                class="text-xs px-2 py-0.5 rounded font-semibold"
                style="background: var(--dnd-surface-1); border: 1px solid var(--dnd-gold); color: var(--dnd-gold);"
              >
                {{ lang.name }}
              </span>
            </div>
          </div>

          <!-- Selectable additional languages -->
          <template v-if="languageChoiceLimit > 0">
            <p class="text-xs" style="color: var(--dnd-parchment-dim);">
              Your background grants {{ languageChoiceLimit }} additional language{{ languageChoiceLimit !== 1 ? 's' : '' }} of your choice:
            </p>
            <div class="grid grid-cols-2 sm:grid-cols-3 gap-1.5">
              <button
                v-for="lang in languages.filter(l => !autoGrantedLanguageIds.has(l.id!))"
                :key="lang.id"
                class="flex items-center gap-2 rounded px-3 py-2 text-left text-xs border transition-colors"
                :class="selectedLanguageIds.has(lang.id!) || selectedLanguageIds.size < languageChoiceLimit
                  ? 'cursor-pointer'
                  : 'cursor-not-allowed opacity-40'"
                :style="selectedLanguageIds.has(lang.id!)
                  ? 'background: var(--dnd-gold); color: #0f0a08; border-color: var(--dnd-gold);'
                  : 'background: var(--dnd-surface-2); color: var(--dnd-parchment); border-color: transparent;'"
                @click="toggleLanguage(lang.id!)"
              >
                <span
                  class="w-3.5 h-3.5 rounded-sm border flex-shrink-0 flex items-center justify-center text-xs font-bold"
                  :style="selectedLanguageIds.has(lang.id!)
                    ? 'border-color: #0f0a08; color: #0f0a08;'
                    : 'border-color: var(--dnd-gold); color: var(--dnd-gold);'"
                >
                  {{ selectedLanguageIds.has(lang.id!) ? '✓' : '' }}
                </span>
                <span class="font-semibold">{{ lang.name }}</span>
              </button>
            </div>
          </template>
          <p v-else-if="autoGrantedLanguageIds.size === 0" class="text-xs" style="color: var(--dnd-parchment-dim);">
            Select a race and background to see language grants.
          </p>
        </div>
      </div>

      <!-- STEP 4: Ability Scores -->
      <div v-if="step === 4" class="space-y-4">
        <div class="dnd-panel-header">Ability Scores</div>
        <p class="text-xs" style="color: var(--dnd-parchment-dim);">
          Enter your ability scores (before racial bonuses). Standard array: 15, 14, 13, 12, 10, 8
        </p>
        <div class="grid grid-cols-2 sm:grid-cols-3 gap-3">
          <div
            v-for="(abbr, id) in abilityNames"
            :key="id"
            class="rounded-lg p-3"
            style="background: var(--dnd-surface-2);"
          >
            <label class="text-xs font-semibold uppercase tracking-widest block mb-1.5" style="color: var(--dnd-gold);">
              {{ abbr }}
            </label>
            <InputNumber
              v-model="form.abilityScores[Number(id)]"
              :min="3"
              :max="20"
              fluid
            />
            <!-- Racial bonus sources -->
            <div v-if="racialBonusesByAbility[Number(id)]?.length" class="mt-1.5 space-y-0.5">
              <div
                v-for="bonus in racialBonusesByAbility[Number(id)]"
                :key="bonus.source"
                class="text-xs"
                style="color: var(--dnd-parchment-dim);"
              >
                <span style="color: var(--dnd-gold); font-weight: 600;">+{{ bonus.amount }}</span> from {{ bonus.source }}
              </div>
            </div>
            <!-- Total with modifier — always shown -->
            <div class="mt-2 pt-1.5 border-t" style="border-color: var(--dnd-gold);">
              <span class="text-xs" style="color: var(--dnd-parchment-dim);">Total: </span>
              <span class="text-lg font-bold" style="color: var(--dnd-parchment);">{{ totalAbilityScores[Number(id)] }}</span>
              <span class="text-xs ml-1" style="color: var(--dnd-parchment-dim);">
                ({{ totalAbilityScores[Number(id)] >= 10 ? '+' : '' }}{{ Math.floor((totalAbilityScores[Number(id)] - 10) / 2) }} mod)
              </span>
            </div>
          </div>
        </div>

        <!-- HP summary -->
        <div class="rounded-lg p-3 flex items-center gap-3" style="background: var(--dnd-surface-2);">
          <span class="text-lg font-bold" style="color: var(--dnd-gold);">♥</span>
          <div>
            <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Starting HP</p>
            <p class="text-2xl font-bold" style="color: var(--dnd-parchment);">{{ estimatedHP }}</p>
            <p class="text-xs" style="color: var(--dnd-parchment-dim);">
              CON modifier: {{ totalAbilityScores[3] >= 10 ? '+' : '' }}{{ Math.floor((totalAbilityScores[3] - 10) / 2) }}
              <template v-if="racialBonusesByAbility[3]?.length">
                (includes racial +{{ racialBonusesByAbility[3].reduce((s, b) => s + b.amount, 0) }})
              </template>
            </p>
          </div>
        </div>
      </div>

      <!-- STEP 5: Spell Selection -->
      <div v-if="step === 5" class="space-y-6">
        <div class="dnd-panel-header">Spell Selection</div>
        <p class="text-xs" style="color: var(--dnd-parchment-dim);">
          Choose your starting spells. Cantrips are at-will; leveled spells consume spell slots.
        </p>

        <div v-if="spellsLoading" class="text-sm" style="color: var(--dnd-parchment-dim);">Loading spells...</div>

        <div v-for="{ entry, cls } in spellcastingClasses" :key="cls.id" class="space-y-4">
          <!-- Class header -->
          <div class="space-y-0.5">
            <div class="flex items-baseline gap-3">
              <span class="text-base font-bold" style="color: var(--dnd-gold);">{{ cls.name }}</span>
              <span class="text-xs" style="color: var(--dnd-parchment-dim);">
                Spellcasting: {{ cls.spellcastingAbility }}
              </span>
              <span v-if="slotSummary(cls, entry.level)" class="text-xs" style="color: var(--dnd-parchment-dim);">
                · Slots: {{ slotSummary(cls, entry.level) }}
              </span>
            </div>
            <p v-if="preparedLimitNote(cls, entry.level)" class="text-xs italic" style="color: var(--dnd-parchment-dim);">
              {{ preparedLimitNote(cls, entry.level) }}
            </p>
          </div>

          <!-- Spells grouped by level -->
          <div
            v-for="spellLevel in getAvailableSpellLevels(cls, entry.level)"
            :key="spellLevel"
            class="space-y-1"
          >
            <!-- Level header with counter -->
            <div class="flex items-baseline justify-between mb-1.5">
              <p class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
                {{ spellLevelLabel(spellLevel) }}
              </p>
              <span class="text-xs" :style="countSelected(cls.id!, spellLevel) >= getSpellLimit(cls, entry.level, spellLevel)
                ? 'color: var(--dnd-gold); font-weight: 600;'
                : 'color: var(--dnd-parchment-dim);'">
                {{ countSelected(cls.id!, spellLevel) }} /
                {{ getSpellLimit(cls, entry.level, spellLevel) === Infinity ? '∞' : getSpellLimit(cls, entry.level, spellLevel) }}
                {{ spellLimitLabel(cls, entry.level, spellLevel) }}
              </span>
            </div>
            <p
              v-if="!getSpellsForClassByLevel(cls.id!, spellLevel).length"
              class="text-xs"
              style="color: var(--dnd-parchment-dim);"
            >
              No spells available.
            </p>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-1.5">
              <button
                v-for="spell in getSpellsForClassByLevel(cls.id!, spellLevel)"
                :key="spell.id"
                class="flex items-center gap-2 rounded px-3 py-2 text-left text-xs border transition-colors"
                :class="selectedSpellIds.has(spell.id!) || countSelected(cls.id!, spellLevel) < getSpellLimit(cls, entry.level, spellLevel)
                  ? 'cursor-pointer'
                  : 'cursor-not-allowed opacity-40'"
                :style="selectedSpellIds.has(spell.id!)
                  ? 'background: var(--dnd-gold); color: #0f0a08; border-color: var(--dnd-gold);'
                  : 'background: var(--dnd-surface-2); color: var(--dnd-parchment); border-color: transparent;'"
                @click="toggleSpell(spell, cls, entry.level)"
              >
                <span
                  class="w-3.5 h-3.5 rounded-sm border flex-shrink-0 flex items-center justify-center text-xs font-bold"
                  :style="selectedSpellIds.has(spell.id!)
                    ? 'border-color: #0f0a08; color: #0f0a08;'
                    : 'border-color: var(--dnd-gold); color: var(--dnd-gold);'"
                >
                  {{ selectedSpellIds.has(spell.id!) ? '✓' : '' }}
                </span>
                <span class="flex-1 font-semibold">{{ spell.name }}</span>
                <span class="opacity-70 flex-shrink-0">{{ spell.school }}</span>
                <span v-if="spell.isConcentration" class="opacity-70 flex-shrink-0" title="Concentration">C</span>
                <span v-if="spell.isRitual" class="opacity-70 flex-shrink-0" title="Ritual">R</span>
              </button>
            </div>
          </div>

          <div v-if="spellcastingClasses.length > 1" class="border-t" style="border-color: var(--dnd-surface-2);" />
        </div>

        <!-- Selected count -->
        <div class="rounded-lg p-3 flex items-center gap-2" style="background: var(--dnd-surface-2);">
          <span style="color: var(--dnd-gold);">✦</span>
          <span class="text-sm" style="color: var(--dnd-parchment);">
            <span class="font-bold">{{ selectedSpellIds.size }}</span>
            <span style="color: var(--dnd-parchment-dim);"> spell{{ selectedSpellIds.size !== 1 ? 's' : '' }} selected</span>
          </span>
        </div>
      </div>
    </div>

    <!-- Navigation -->
    <div class="flex justify-between mt-5">
      <Button
        v-if="step > 1"
        label="Previous"
        icon="pi pi-chevron-left"
        severity="secondary"
        @click="step--"
      />
      <div v-else />

      <div class="flex gap-2">
        <Button
          v-if="step < totalSteps"
          label="Next"
          icon="pi pi-chevron-right"
          icon-pos="right"
          @click="step++"
        />
        <Button
          v-else
          label="Create Character"
          icon="pi pi-star"
          :loading="submitting"
          @click="submit"
        />
      </div>
    </div>
  </div>
</template>
