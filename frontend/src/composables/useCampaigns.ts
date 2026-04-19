import { ref } from 'vue'
import client from '@/api/client'
import type { components } from '@/api/generated/schema'

export type CampaignSummary = components['schemas']['CampaignSummaryDto']
export type CampaignDetail = components['schemas']['CampaignDetailDto']

export function useCampaigns() {
  const campaigns = ref<CampaignSummary[]>([])
  const campaign = ref<CampaignDetail | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchMyCampaigns() {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<CampaignSummary[]>('/api/campaign')
      campaigns.value = data
    } catch {
      error.value = 'Failed to load campaigns'
    } finally {
      loading.value = false
    }
  }

  async function fetchCampaign(id: number) {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.get<CampaignDetail>(`/api/campaign/${id}`)
      campaign.value = data
    } catch {
      error.value = 'Failed to load campaign'
    } finally {
      loading.value = false
    }
  }

  async function createCampaign(payload: { name: string; description: string; setting: string }) {
    const { data } = await client.post<CampaignDetail>('/api/campaign', payload)
    return data
  }

  return { campaigns, campaign, loading, error, fetchMyCampaigns, fetchCampaign, createCampaign }
}
