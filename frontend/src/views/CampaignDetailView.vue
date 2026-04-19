<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Button from 'primevue/button'
import Tag from 'primevue/tag'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import { useCampaigns } from '@/composables/useCampaigns'
import { useEncounters } from '@/composables/useEncounters'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const id = Number(route.params.id)
const { campaign, loading, error, fetchCampaign } = useCampaigns()
const { encounters, fetchEncounters, createEncounter } = useEncounters()

const showNewEncounter = ref(false)
const encounterForm = ref({ name: '', description: '' })
const creatingEncounter = ref(false)

onMounted(async () => {
  await fetchCampaign(id)
  await fetchEncounters(id)
})

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

async function submitEncounter() {
  creatingEncounter.value = true
  try {
    await createEncounter(id, encounterForm.value)
    showNewEncounter.value = false
    encounterForm.value = { name: '', description: '' }
    await fetchEncounters(id)
    toast.add({ severity: 'success', summary: '⚔️ Encounter Created', life: 2000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to create encounter', life: 2000 })
  } finally {
    creatingEncounter.value = false
  }
}
</script>

<template>
  <Toast />
  <Dialog v-model:visible="showNewEncounter" header="⚔️ New Encounter" modal class="w-full max-w-md">
    <div class="flex flex-col gap-4 pt-2">
      <div class="flex flex-col gap-1.5">
        <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Encounter Name</label>
        <InputText v-model="encounterForm.name" placeholder="Goblin Ambush" fluid />
      </div>
      <div class="flex flex-col gap-1.5">
        <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Description</label>
        <Textarea v-model="encounterForm.description" placeholder="The party is ambushed..." rows="3" fluid />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showNewEncounter = false" />
      <Button label="Create Encounter" icon="pi pi-shield" :loading="creatingEncounter" @click="submitEncounter" />
    </template>
  </Dialog>

  <div class="max-w-5xl mx-auto px-4 py-6">
    <Button icon="pi pi-arrow-left" label="Back" text class="mb-4" style="color: var(--dnd-parchment-dim);" @click="router.back()" />

    <div v-if="loading" class="text-center py-16" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-3xl mb-3" style="color: var(--dnd-gold);" />
      <p>Loading campaign scroll...</p>
    </div>
    <div v-else-if="error" class="p-4" style="color: var(--dnd-red-light);">⚠️ {{ error }}</div>
    <div v-else-if="campaign" class="space-y-5">

      <!-- Header -->
      <div class="dnd-panel rounded-xl p-5">
        <div class="flex items-start justify-between gap-4 flex-wrap">
          <div>
            <div class="flex items-center gap-3 mb-1">
              <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">{{ campaign.name }}</h1>
              <Tag
                :value="campaign.isActive ? 'Active' : 'Inactive'"
                :severity="campaign.isActive ? 'success' : 'secondary'"
              />
            </div>
            <p class="text-sm" style="color: var(--dnd-parchment-dim);">
              <span style="color: var(--dnd-gold);">DM:</span> {{ campaign.dmName }}
            </p>
            <p v-if="campaign.setting" class="text-sm" style="color: var(--dnd-parchment-dim);">
              <span style="color: var(--dnd-gold);">Setting:</span> {{ campaign.setting }}
            </p>
          </div>
        </div>
        <div v-if="campaign.description" class="mt-3 pt-3" style="border-top: 1px solid var(--dnd-border);">
          <p class="text-sm" style="color: var(--dnd-parchment);">{{ campaign.description }}</p>
          <p v-if="campaign.notes" class="text-sm mt-2 italic" style="color: var(--dnd-parchment-dim);">{{ campaign.notes }}</p>
        </div>
      </div>

      <!-- Members -->
      <div class="dnd-panel rounded-xl p-4">
        <div class="dnd-panel-header mb-3">⚔️ Party Members ({{ campaign.members?.length ?? 0 }})</div>
        <DataTable :value="campaign.members" size="small">
          <Column field="userName" header="Player" />
          <Column field="characterName" header="Character">
            <template #body="{ data }">
              <button
                v-if="data.characterName && data.characterId"
                class="border-none bg-transparent cursor-pointer font-medium hover:underline"
                style="color: var(--dnd-gold);"
                @click="router.push(`/characters/${data.characterId}`)"
              >
                {{ data.characterName }}
              </button>
              <span v-else class="italic" style="color: var(--dnd-parchment-dim);">No character</span>
            </template>
          </Column>
          <Column field="role" header="Role">
            <template #body="{ data }">
              <Tag
                :value="data.role === 'DungeonMaster' ? '👑 DM' : '🗡️ Player'"
                :severity="data.role === 'DungeonMaster' ? 'warn' : 'info'"
              />
            </template>
          </Column>
        </DataTable>
      </div>

      <!-- Encounters -->
      <div class="dnd-panel rounded-xl p-4">
        <div class="flex items-center justify-between mb-3">
          <div class="dnd-panel-header">⚔️ Encounters</div>
          <Button label="New Encounter" icon="pi pi-plus" size="small" @click="showNewEncounter = true" />
        </div>

        <div v-if="encounters.length === 0" class="text-center py-6" style="color: var(--dnd-parchment-dim);">
          <div class="text-3xl mb-2">🐉</div>
          <p class="text-sm">No encounters yet. Prepare for battle!</p>
        </div>

        <div v-else class="space-y-2">
          <div
            v-for="enc in encounters"
            :key="enc.id"
            class="flex items-center justify-between p-3 rounded cursor-pointer transition-colors"
            style="background: var(--dnd-surface-2); border: 1px solid var(--dnd-border);"
            @click="router.push(`/campaigns/${id}/encounters/${enc.id}`)"
          >
            <div>
              <span class="font-medium text-sm" style="color: var(--dnd-parchment);">{{ enc.name }}</span>
              <div class="text-xs mt-0.5" style="color: var(--dnd-parchment-dim);">
                Round {{ enc.round }} · {{ enc.participantCount }} participants
              </div>
            </div>
            <div class="flex items-center gap-2">
              <Tag
                :value="enc.isActive ? 'In Progress' : 'Complete'"
                :severity="enc.isActive ? 'warn' : 'secondary'"
              />
              <i class="pi pi-chevron-right text-xs" style="color: var(--dnd-gold);" />
            </div>
          </div>
        </div>
      </div>

      <!-- Sessions -->
      <div v-if="campaign.sessions && campaign.sessions.length > 0" class="dnd-panel rounded-xl p-4">
        <div class="dnd-panel-header mb-3">📋 Sessions</div>
        <div class="space-y-2">
          <div
            v-for="session in [...(campaign.sessions ?? [])].sort((a, b) => (b.sessionNumber ?? 0) - (a.sessionNumber ?? 0))"
            :key="session.id"
            class="p-3 rounded"
            style="background: var(--dnd-surface-2); border: 1px solid var(--dnd-border);"
          >
            <div class="flex items-center justify-between mb-1">
              <span class="font-medium text-sm" style="color: var(--dnd-parchment);">
                Session {{ session.sessionNumber }}: {{ session.title }}
              </span>
              <span class="text-xs" style="color: var(--dnd-parchment-dim);">{{ formatDate(session.date!) }}</span>
            </div>
            <p v-if="session.summary" class="text-xs" style="color: var(--dnd-parchment-dim);">{{ session.summary }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
