import { ref } from 'vue'
import client from '@/api/client'
import type { components } from '@/api/generated/schema'

export type EncounterSummary = components['schemas']['EncounterSummaryDto']
export type EncounterDetail = components['schemas']['EncounterDetailDto']

export function useEncounters() {
  const encounters = ref<EncounterSummary[]>([])
  const encounter = ref<EncounterDetail | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchEncounters(campaignId: number) {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<EncounterSummary[]>(`/api/campaigns/${campaignId}/encounter`)
      encounters.value = data
    } catch {
      error.value = 'Failed to load encounters'
    } finally {
      loading.value = false
    }
  }

  async function fetchEncounter(campaignId: number, encounterId: number) {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<EncounterDetail>(`/api/campaigns/${campaignId}/encounter/${encounterId}`)
      encounter.value = data
    } catch {
      error.value = 'Failed to load encounter'
    } finally {
      loading.value = false
    }
  }

  async function createEncounter(campaignId: number, payload: { name: string; description: string; sessionId?: number }) {
    const { data } = await client.post<EncounterDetail>(`/api/campaigns/${campaignId}/encounter`, payload)
    return data
  }

  async function deleteEncounter(campaignId: number, encounterId: number) {
    await client.delete(`/api/campaigns/${campaignId}/encounter/${encounterId}`)
  }

  async function addParticipant(campaignId: number, encounterId: number, payload: {
    participantType: number
    characterId?: number
    monsterId?: number
    displayName?: string
    initiative: number
    maxHP?: number
    ac?: number
  }) {
    const { data } = await client.post<EncounterDetail>(
      `/api/campaigns/${campaignId}/encounter/${encounterId}/participants`,
      payload
    )
    encounter.value = data
    return data
  }

  async function removeParticipant(campaignId: number, encounterId: number, participantId: number) {
    const { data } = await client.delete<EncounterDetail>(
      `/api/campaigns/${campaignId}/encounter/${encounterId}/participants/${participantId}`
    )
    encounter.value = data
    return data
  }

  async function applyDamageToParticipant(
    campaignId: number,
    encounterId: number,
    participantId: number,
    amount: number,
    syncToCharacter = true
  ) {
    const { data } = await client.post<EncounterDetail>(
      `/api/campaigns/${campaignId}/encounter/${encounterId}/participants/${participantId}/damage`,
      { amount, syncToCharacter }
    )
    encounter.value = data
    return data
  }

  async function advanceTurn(campaignId: number, encounterId: number) {
    const { data } = await client.post<EncounterDetail>(
      `/api/campaigns/${campaignId}/encounter/${encounterId}/advance-turn`
    )
    encounter.value = data
    return data
  }

  async function nextRound(campaignId: number, encounterId: number) {
    const { data } = await client.post<EncounterDetail>(
      `/api/campaigns/${campaignId}/encounter/${encounterId}/next-round`
    )
    encounter.value = data
    return data
  }

  return {
    encounters,
    encounter,
    loading,
    error,
    fetchEncounters,
    fetchEncounter,
    createEncounter,
    deleteEncounter,
    addParticipant,
    removeParticipant,
    applyDamageToParticipant,
    advanceTurn,
    nextRound
  }
}
