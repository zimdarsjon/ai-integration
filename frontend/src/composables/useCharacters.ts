import { ref } from 'vue'
import client from '@/api/client'
import type { components } from '@/api/generated/schema'

export type CharacterSummary = components['schemas']['CharacterSummaryDto']
export type CharacterDetail = components['schemas']['CharacterDetailDto']

export function useCharacters() {
  const characters = ref<CharacterSummary[]>([])
  const character = ref<CharacterDetail | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchMyCharacters() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<CharacterSummary[]>('/api/character')
      characters.value = data
    } catch {
      error.value = 'Failed to load characters'
    } finally {
      loading.value = false
    }
  }

  async function fetchCharacter(id: number) {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<CharacterDetail>(`/api/character/${id}`)
      character.value = data
    } catch {
      error.value = 'Failed to load character'
    } finally {
      loading.value = false
    }
  }

  async function deleteCharacter(id: number) {
    await client.delete(`/api/character/${id}`)
    characters.value = characters.value.filter(c => c.id !== id)
  }

  async function applyDamage(id: number, amount: number) {
    const { data } = await client.post<CharacterDetail>(`/api/character/${id}/damage`, { amount })
    character.value = data
  }

  async function heal(id: number, amount: number) {
    const { data } = await client.post<CharacterDetail>(`/api/character/${id}/heal`, { amount })
    character.value = data
  }

  async function updateCurrency(id: number, currency: { cp: number; sp: number; ep: number; gp: number; pp: number }) {
    await client.put(`/api/character/${id}/currency`, currency)
    if (character.value) {
      character.value.currency = { cp: currency.cp, sp: currency.sp, ep: currency.ep, gp: currency.gp, pp: currency.pp }
    }
  }

  async function addSpell(id: number, spellId: number, spellMeta?: { name: string; level: number; school: string; isConcentration: boolean; isRitual: boolean; castingTime: string; range: string; duration: string }) {
    await client.post(`/api/character/${id}/spells/${spellId}`)
    if (character.value && spellMeta) {
      character.value.spells = [...(character.value.spells ?? []), {
        spellId,
        isPrepared: false,
        name: spellMeta.name,
        level: spellMeta.level,
        school: spellMeta.school,
        isConcentration: spellMeta.isConcentration,
        isRitual: spellMeta.isRitual,
        castingTime: spellMeta.castingTime,
        range: spellMeta.range,
        duration: spellMeta.duration,
      }]
    }
  }

  async function removeSpell(id: number, spellId: number) {
    await client.delete(`/api/character/${id}/spells/${spellId}`)
    if (character.value)
      character.value.spells = character.value.spells?.filter(s => s.spellId !== spellId) ?? []
  }

  async function prepareSpell(id: number, spellId: number, isPrepared: boolean, spellMeta?: { name: string; level: number; school: string; isConcentration: boolean; isRitual: boolean; castingTime: string; range: string; duration: string }) {
    await client.put(`/api/character/${id}/spells/${spellId}/prepare`, { isPrepared })
    if (character.value) {
      const existing = character.value.spells?.find(s => s.spellId === spellId)
      if (existing) {
        existing.isPrepared = isPrepared
      } else if (spellMeta) {
        character.value.spells = [...(character.value.spells ?? []), {
          spellId,
          isPrepared,
          name: spellMeta.name,
          level: spellMeta.level,
          school: spellMeta.school,
          isConcentration: spellMeta.isConcentration,
          isRitual: spellMeta.isRitual,
          castingTime: spellMeta.castingTime,
          range: spellMeta.range,
          duration: spellMeta.duration,
        }]
      }
    }
  }

  async function levelUp(id: number, classId: number, hpIncrease: number, subclassId?: number | null, spellIds?: number[]) {
    const { data } = await client.post<CharacterDetail>(`/api/character/${id}/level-up`, { classId, hpIncrease, subclassId: subclassId ?? null, spellIds: spellIds ?? [] })
    character.value = data
  }

  async function updateSpellSlot(id: number, slotLevel: number, usedSlots: number) {
    await client.put(`/api/character/${id}/spell-slots`, { slotLevel, usedSlots })
    await fetchCharacter(id)
  }

  return {
    characters,
    character,
    loading,
    error,
    fetchMyCharacters,
    fetchCharacter,
    deleteCharacter,
    applyDamage,
    heal,
    updateCurrency,
    addSpell,
    removeSpell,
    prepareSpell,
    levelUp,
    updateSpellSlot
  }
}
