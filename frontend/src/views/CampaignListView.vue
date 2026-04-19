<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Tag from 'primevue/tag'
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import { useCampaigns } from '@/composables/useCampaigns'

const router = useRouter()
const toast = useToast()
const { campaigns, loading, error, fetchMyCampaigns, createCampaign } = useCampaigns()

const showCreate = ref(false)
const form = ref({ name: '', description: '', setting: '' })
const creating = ref(false)

onMounted(fetchMyCampaigns)

async function submitCreate() {
  creating.value = true
  try {
    const created = await createCampaign(form.value)
    showCreate.value = false
    form.value = { name: '', description: '', setting: '' }
    toast.add({ severity: 'success', summary: '🗺️ Campaign Created', detail: `${created.name} is ready`, life: 3000 })
    await fetchMyCampaigns()
  } catch {
    toast.add({ severity: 'error', summary: 'Failed', detail: 'Could not create campaign', life: 3000 })
  } finally {
    creating.value = false
  }
}
</script>

<template>
  <Toast />
  <Dialog v-model:visible="showCreate" header="🗺️ New Campaign" modal class="w-full max-w-md">
    <div class="flex flex-col gap-4 pt-2">
      <div class="flex flex-col gap-1.5">
        <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Campaign Name</label>
        <InputText v-model="form.name" placeholder="The Lost Mine of Phandelver" fluid />
      </div>
      <div class="flex flex-col gap-1.5">
        <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Description</label>
        <Textarea v-model="form.description" placeholder="A brief summary of the campaign..." rows="3" fluid />
      </div>
      <div class="flex flex-col gap-1.5">
        <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Setting</label>
        <InputText v-model="form.setting" placeholder="Forgotten Realms, Eberron..." fluid />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showCreate = false" />
      <Button label="Begin Campaign" icon="pi pi-flag" :loading="creating" @click="submitCreate" />
    </template>
  </Dialog>

  <div class="max-w-5xl mx-auto px-4 py-8">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">🗺️ My Campaigns</h1>
        <p class="text-sm mt-0.5" style="color: var(--dnd-parchment-dim);">Your ongoing and past adventures</p>
      </div>
      <Button label="New Campaign" icon="pi pi-plus" @click="showCreate = true" />
    </div>

    <div v-if="loading" class="text-center py-16" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-3xl mb-3" style="color: var(--dnd-gold);" />
      <p>Unrolling the map...</p>
    </div>
    <div v-else-if="error" class="dnd-panel p-4 rounded-lg" style="border-color: var(--dnd-red);">
      <p style="color: var(--dnd-red-light);">⚠️ {{ error }}</p>
    </div>
    <div v-else-if="campaigns.length === 0" class="text-center py-16 dnd-panel rounded-xl">
      <div class="text-5xl mb-4">🗺️</div>
      <h2 class="text-lg font-bold mb-2" style="color: var(--dnd-parchment);">No campaigns yet</h2>
      <p class="text-sm mb-6" style="color: var(--dnd-parchment-dim);">The realm awaits. Start your first campaign.</p>
      <Button label="Create Campaign" icon="pi pi-plus" @click="showCreate = true" />
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div
        v-for="c in campaigns"
        :key="c.id"
        class="dnd-panel rounded-xl p-5 cursor-pointer transition-all duration-200 hover:scale-[1.01]"
        @click="router.push(`/campaigns/${c.id}`)"
      >
        <div class="flex items-start justify-between gap-2 mb-2">
          <h2 class="text-base font-bold" style="color: var(--dnd-parchment);">{{ c.name }}</h2>
          <Tag
            :value="c.isActive ? 'Active' : 'Inactive'"
            :severity="c.isActive ? 'success' : 'secondary'"
          />
        </div>
        <p class="text-xs mb-1" style="color: var(--dnd-gold);">DM: {{ c.dmName }}</p>
        <p v-if="c.description" class="text-sm mb-3 line-clamp-2" style="color: var(--dnd-parchment-dim);">{{ c.description }}</p>
        <div class="flex items-center gap-3 text-xs" style="color: var(--dnd-parchment-dim);">
          <span><i class="pi pi-users mr-1" />{{ c.memberCount }} members</span>
          <span class="capitalize">{{ c.userRole }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
