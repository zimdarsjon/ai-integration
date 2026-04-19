import { ref } from 'vue'
import client from '@/api/client'
import type { components } from '@/api/generated/schema'

export type SpellSummary = components['schemas']['SpellSummaryDto']
export type SpellDetail = components['schemas']['SpellDetailDto']
export type MonsterSummary = components['schemas']['MonsterSummaryDto']
export type MonsterDetail = components['schemas']['MonsterDetailDto']
export type ItemSummary = components['schemas']['ItemSummaryDto']
export type ItemDetail = components['schemas']['ItemDetailDto']
export type RaceDto = components['schemas']['RaceDto']
export type ClassDto = components['schemas']['ClassDto']
export type SkillDto = components['schemas']['SkillDto']
export type ReferenceItem = components['schemas']['ReferenceItemDto']
export type BackgroundSummary = components['schemas']['BackgroundSummaryDto']
export type BackgroundDetail = components['schemas']['BackgroundDetailDto']

export function useReference() {
  const spells = ref<SpellSummary[]>([])
  const monsters = ref<MonsterSummary[]>([])
  const items = ref<ItemSummary[]>([])
  const races = ref<RaceDto[]>([])
  const classes = ref<ClassDto[]>([])
  const conditions = ref<ReferenceItem[]>([])
  const backgrounds = ref<BackgroundSummary[]>([])
  const skills = ref<SkillDto[]>([])
  const languages = ref<ReferenceItem[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchSpells(filters?: { level?: number; schoolId?: number; classId?: number }) {
    loading.value = true
    error.value = null
    try {
      const params = new URLSearchParams()
      if (filters?.level !== undefined) params.set('level', String(filters.level))
      if (filters?.schoolId !== undefined) params.set('schoolId', String(filters.schoolId))
      if (filters?.classId !== undefined) params.set('classId', String(filters.classId))
      const { data } = await client.get<SpellSummary[]>(`/api/reference/spells?${params}`)
      spells.value = data
    } catch {
      error.value = 'Failed to load spells'
    } finally {
      loading.value = false
    }
  }

  async function fetchSpell(id: number) {
    const { data } = await client.get<SpellDetail>(`/api/reference/spells/${id}`)
    return data
  }

  async function fetchMonsters(filters?: { maxCr?: number; creatureTypeId?: number }) {
    loading.value = true
    error.value = null
    try {
      const params = new URLSearchParams()
      if (filters?.maxCr !== undefined) params.set('maxCr', String(filters.maxCr))
      if (filters?.creatureTypeId !== undefined) params.set('creatureTypeId', String(filters.creatureTypeId))
      const { data } = await client.get<MonsterSummary[]>(`/api/reference/monsters?${params}`)
      monsters.value = data
    } catch {
      error.value = 'Failed to load monsters'
    } finally {
      loading.value = false
    }
  }

  async function fetchMonster(id: number) {
    const { data } = await client.get<MonsterDetail>(`/api/reference/monsters/${id}`)
    return data
  }

  async function fetchItems(type?: string) {
    loading.value = true
    error.value = null
    try {
      const params = type ? `?type=${type}` : ''
      const { data } = await client.get<ItemSummary[]>(`/api/reference/items${params}`)
      items.value = data
    } catch {
      error.value = 'Failed to load items'
    } finally {
      loading.value = false
    }
  }

  async function fetchItem(id: number) {
    const { data } = await client.get<ItemDetail>(`/api/reference/items/${id}`)
    return data
  }

  async function fetchRaces() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<RaceDto[]>('/api/reference/races')
      races.value = data
    } catch {
      error.value = 'Failed to load races'
    } finally {
      loading.value = false
    }
  }

  async function fetchClasses() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<ClassDto[]>('/api/reference/classes')
      classes.value = data
    } catch {
      error.value = 'Failed to load classes'
    } finally {
      loading.value = false
    }
  }

  async function fetchConditions() {
    const { data } = await client.get<ReferenceItem[]>('/api/reference/conditions')
    conditions.value = data
    return data
  }

  async function fetchBackgrounds() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<BackgroundSummary[]>('/api/Backgrounds')
      backgrounds.value = data
    } catch {
      error.value = 'Failed to load backgrounds'
    } finally {
      loading.value = false
    }
  }

  async function fetchBackground(id: number) {
    const { data } = await client.get<BackgroundDetail>(`/api/Backgrounds/${id}`)
    return data
  }

  async function fetchSkills() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<SkillDto[]>('/api/reference/skills')
      skills.value = data
    } catch {
      error.value = 'Failed to load skills'
    } finally {
      loading.value = false
    }
  }

  async function fetchLanguages() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<ReferenceItem[]>('/api/reference/languages')
      languages.value = data
    } catch {
      error.value = 'Failed to load languages'
    } finally {
      loading.value = false
    }
  }

  return {
    spells, monsters, items, races, classes, conditions, backgrounds, skills, languages,
    loading, error,
    fetchSpells, fetchSpell,
    fetchMonsters, fetchMonster,
    fetchItems, fetchItem,
    fetchRaces, fetchClasses,
    fetchConditions,
    fetchBackgrounds, fetchBackground,
    fetchSkills, fetchLanguages
  }
}
