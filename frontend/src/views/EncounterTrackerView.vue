<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Button from 'primevue/button'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import InputNumber from 'primevue/inputnumber'
import InputText from 'primevue/inputtext'
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import { useEncounters } from '@/composables/useEncounters'
import type { components } from '@/api/generated/schema'

type Participant = components['schemas']['ParticipantDto']

const route = useRoute()
const router = useRouter()
const toast = useToast()
const campaignId = Number(route.params.campaignId)
const encounterId = Number(route.params.encounterId)

const {
  encounter,
  loading,
  error,
  fetchEncounter,
  addParticipant,
  removeParticipant,
  applyDamageToParticipant,
  advanceTurn,
  nextRound
} = useEncounters()

const showAddParticipant = ref(false)
const showDamage = ref(false)
const selectedParticipant = ref<Participant | null>(null)
const damageAmount = ref(1)
const actionLoading = ref(false)

const newParticipant = ref({
  participantType: 1,
  displayName: '',
  initiative: 10,
  maxHP: 10,
  ac: 10
})

onMounted(() => fetchEncounter(campaignId, encounterId))

const sortedParticipants = computed(() => {
  if (!encounter.value?.participants) return []
  return [...encounter.value.participants].sort((a, b) => (b.initiative ?? 0) - (a.initiative ?? 0))
})

function hpColor(current: number, max: number) {
  if (max === 0) return '#9ca3af'
  const pct = (current / max) * 100
  if (pct > 60) return '#166534'
  if (pct > 30) return '#a16207'
  return '#9b1c1c'
}

function hpPct(current: number, max: number) {
  return max > 0 ? Math.round((current / max) * 100) : 0
}

async function submitAddParticipant() {
  actionLoading.value = true
  try {
    await addParticipant(campaignId, encounterId, newParticipant.value)
    showAddParticipant.value = false
    newParticipant.value = { participantType: 1, displayName: '', initiative: 10, maxHP: 10, ac: 10 }
    toast.add({ severity: 'success', summary: 'Participant added', life: 2000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to add participant', life: 2000 })
  } finally {
    actionLoading.value = false
  }
}

function openDamage(p: Participant) {
  selectedParticipant.value = p
  damageAmount.value = 1
  showDamage.value = true
}

async function submitDamage() {
  if (!selectedParticipant.value?.id) return
  actionLoading.value = true
  try {
    await applyDamageToParticipant(campaignId, encounterId, selectedParticipant.value.id, damageAmount.value)
    showDamage.value = false
    toast.add({ severity: 'warn', summary: `⚔️ ${damageAmount.value} damage dealt`, life: 2000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to apply damage', life: 2000 })
  } finally {
    actionLoading.value = false
  }
}

async function doAdvanceTurn() {
  try {
    await advanceTurn(campaignId, encounterId)
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to advance turn', life: 2000 })
  }
}

async function doNextRound() {
  try {
    await nextRound(campaignId, encounterId)
    toast.add({ severity: 'info', summary: `Round ${encounter.value?.round ?? '?'}`, life: 2000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to advance round', life: 2000 })
  }
}

async function doRemove(p: Participant) {
  if (!p.id) return
  try {
    await removeParticipant(campaignId, encounterId, p.id)
    toast.add({ severity: 'secondary', summary: `${p.displayName} removed`, life: 2000 })
  } catch {
    toast.add({ severity: 'error', summary: 'Failed to remove', life: 2000 })
  }
}
</script>

<template>
  <Toast />

  <Dialog v-model:visible="showAddParticipant" header="⚔️ Add Participant" modal class="w-full max-w-md">
    <div class="flex flex-col gap-4 pt-2">
      <div class="flex flex-col gap-1.5">
        <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Name</label>
        <InputText v-model="newParticipant.displayName" placeholder="Goblin Warrior" fluid />
      </div>
      <div class="grid grid-cols-3 gap-3">
        <div class="flex flex-col gap-1.5">
          <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Initiative</label>
          <InputNumber v-model="newParticipant.initiative" :min="0" :max="30" />
        </div>
        <div class="flex flex-col gap-1.5">
          <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">Max HP</label>
          <InputNumber v-model="newParticipant.maxHP" :min="1" :max="999" />
        </div>
        <div class="flex flex-col gap-1.5">
          <label class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">AC</label>
          <InputNumber v-model="newParticipant.ac" :min="1" :max="30" />
        </div>
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showAddParticipant = false" />
      <Button label="Add to Battle" icon="pi pi-shield" :loading="actionLoading" @click="submitAddParticipant" />
    </template>
  </Dialog>

  <Dialog v-model:visible="showDamage" :header="`⚔️ Damage — ${selectedParticipant?.displayName}`" modal>
    <div class="flex items-center gap-3 py-2">
      <label class="text-sm" style="color: var(--dnd-gold);">Damage:</label>
      <InputNumber v-model="damageAmount" :min="1" :max="999" />
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="showDamage = false" />
      <Button label="Apply" severity="danger" :loading="actionLoading" @click="submitDamage" />
    </template>
  </Dialog>

  <div class="max-w-4xl mx-auto px-4 py-6">
    <Button icon="pi pi-arrow-left" label="Back" text class="mb-4" style="color: var(--dnd-parchment-dim);" @click="router.back()" />

    <div v-if="loading" class="text-center py-16" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-3xl" style="color: var(--dnd-gold);" />
    </div>
    <div v-else-if="error" class="p-4" style="color: var(--dnd-red-light);">⚠️ {{ error }}</div>
    <div v-else-if="encounter">

      <!-- Encounter Header -->
      <div class="dnd-panel rounded-xl p-5 mb-5">
        <div class="flex items-center justify-between flex-wrap gap-3">
          <div>
            <h1 class="text-xl font-bold" style="color: var(--dnd-gold);">⚔️ {{ encounter.name }}</h1>
            <p v-if="encounter.description" class="text-sm mt-0.5" style="color: var(--dnd-parchment-dim);">{{ encounter.description }}</p>
          </div>
          <div class="flex items-center gap-3">
            <div class="text-center">
              <div class="text-2xl font-bold" style="color: var(--dnd-gold);">{{ encounter.round }}</div>
              <div class="section-label">Round</div>
            </div>
            <Tag :value="encounter.isActive ? 'Battle!' : 'Ended'" :severity="encounter.isActive ? 'danger' : 'secondary'" />
          </div>
        </div>

        <!-- Combat Controls -->
        <div class="flex gap-2 mt-4 pt-4 flex-wrap" style="border-top: 1px solid var(--dnd-border);">
          <Button label="Next Turn" icon="pi pi-step-forward" @click="doAdvanceTurn" />
          <Button label="Next Round" icon="pi pi-forward" severity="warn" @click="doNextRound" />
          <Button label="Add Combatant" icon="pi pi-plus" severity="secondary" @click="showAddParticipant = true" />
        </div>
      </div>

      <!-- Initiative Order -->
      <div class="space-y-2">
        <div class="dnd-panel-header px-1">Initiative Order</div>
        <div
          v-for="(p, idx) in sortedParticipants"
          :key="p.id ?? idx"
          class="dnd-panel rounded-xl p-4"
          :style="p.isActive ? 'border-color: var(--dnd-gold); box-shadow: 0 0 12px rgba(200,155,60,0.3);' : ''"
        >
          <div class="flex items-center gap-4 flex-wrap">
            <!-- Initiative badge -->
            <div class="text-center min-w-[2.5rem]">
              <div class="text-lg font-bold" style="color: var(--dnd-gold);">{{ p.initiative }}</div>
              <div class="section-label" style="font-size: 0.55rem;">INIT</div>
            </div>

            <!-- Name + type -->
            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 flex-wrap">
                <span class="font-bold text-sm" style="color: var(--dnd-parchment);">{{ p.displayName }}</span>
                <Tag
                  :value="p.participantType === 0 ? '🧙 PC' : '👹 Monster'"
                  :severity="p.participantType === 0 ? 'info' : 'warn'"
                />
                <Tag v-if="p.isActive" value="Current Turn" severity="danger" />
              </div>
              <!-- Conditions -->
              <div v-if="p.conditions && p.conditions.length > 0" class="flex flex-wrap gap-1 mt-1">
                <Tag v-for="cond in p.conditions" :key="cond" :value="cond" severity="danger" />
              </div>
            </div>

            <!-- HP bar -->
            <div class="flex flex-col items-end gap-1 min-w-[120px]">
              <div class="text-xs" style="color: var(--dnd-parchment-dim);">
                HP {{ p.currentHP }} / {{ p.maxHP }}
                <span class="ml-2" style="color: var(--dnd-gold);">AC {{ p.ac }}</span>
              </div>
              <div class="hp-bar-track w-28">
                <div
                  class="hp-bar-fill"
                  :style="{
                    width: hpPct(p.currentHP ?? 0, p.maxHP ?? 0) + '%',
                    background: hpColor(p.currentHP ?? 0, p.maxHP ?? 0)
                  }"
                />
              </div>
            </div>

            <!-- Actions -->
            <div class="flex gap-1">
              <Button
                icon="pi pi-minus"
                severity="danger"
                size="small"
                text
                title="Apply damage"
                @click="openDamage(p)"
              />
              <Button
                icon="pi pi-trash"
                severity="secondary"
                size="small"
                text
                title="Remove from combat"
                @click="doRemove(p)"
              />
            </div>
          </div>
        </div>

        <div v-if="sortedParticipants.length === 0" class="text-center py-8 dnd-panel rounded-xl" style="color: var(--dnd-parchment-dim);">
          <div class="text-3xl mb-2">⚔️</div>
          <p class="text-sm">No combatants yet. Add participants to begin!</p>
        </div>
      </div>
    </div>
  </div>
</template>
