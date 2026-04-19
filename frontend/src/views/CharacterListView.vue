<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import { useCharacters } from '@/composables/useCharacters'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast from 'primevue/toast'

const router = useRouter()
const confirm = useConfirm()
const toast = useToast()
const { characters, loading, error, fetchMyCharacters, deleteCharacter } = useCharacters()

onMounted(fetchMyCharacters)

function hpPercent(current: number, max: number) {
  return max > 0 ? Math.round((current / max) * 100) : 0
}

function hpColor(pct: number) {
  if (pct > 60) return '#166534'
  if (pct > 30) return '#a16207'
  return '#9b1c1c'
}

function confirmDelete(id: number, name: string) {
  confirm.require({
    message: `Retire ${name} from the realm? This cannot be undone.`,
    header: '⚠️ Retire Character',
    icon: 'pi pi-exclamation-triangle',
    rejectLabel: 'Keep',
    acceptLabel: 'Retire',
    accept: async () => {
      await deleteCharacter(id)
      toast.add({ severity: 'success', summary: 'Character Retired', detail: `${name} has left the realm`, life: 3000 })
    }
  })
}
</script>

<template>
  <ConfirmDialog />
  <Toast />
  <div class="max-w-6xl mx-auto px-4 py-8">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold" style="color: var(--dnd-gold);">🧙 My Characters</h1>
        <p class="text-sm mt-0.5" style="color: var(--dnd-parchment-dim);">Your adventurers and heroes</p>
      </div>
      <Button
        label="New Character"
        icon="pi pi-plus"
        @click="router.push('/characters/new')"
      />
    </div>

    <div v-if="loading" class="text-center py-16" style="color: var(--dnd-parchment-dim);">
      <i class="pi pi-spinner pi-spin text-3xl mb-3" style="color: var(--dnd-gold);" />
      <p>Consulting the tome...</p>
    </div>

    <div v-else-if="error" class="dnd-panel p-4 rounded-lg" style="border-color: var(--dnd-red);">
      <p style="color: var(--dnd-red-light);">⚠️ {{ error }}</p>
    </div>

    <div v-else-if="characters.length === 0" class="text-center py-16 dnd-panel rounded-xl">
      <div class="text-5xl mb-4">📜</div>
      <h2 class="text-lg font-bold mb-2" style="color: var(--dnd-parchment);">No adventurers yet</h2>
      <p class="text-sm mb-6" style="color: var(--dnd-parchment-dim);">Your legend awaits. Create your first character to begin.</p>
      <Button label="Create Character" icon="pi pi-plus" @click="router.push('/characters/new')" />
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="c in characters"
        :key="c.id"
        class="dnd-panel rounded-xl overflow-hidden cursor-pointer transition-all duration-200 hover:scale-[1.01]"
        style="border-color: var(--dnd-border);"
        @click="router.push(`/characters/${c.id}`)"
      >
        <!-- Portrait / Header -->
        <div class="relative h-28 flex items-end p-3" style="background: linear-gradient(135deg, #1a0a2e 0%, #0a0a1a 100%);">
          <div v-if="c.portraitUrl" class="absolute inset-0 overflow-hidden">
            <img :src="c.portraitUrl ?? undefined" :alt="c.name ?? ''" class="w-full h-full object-cover opacity-50" />
          </div>
          <div v-else class="absolute inset-0 flex items-center justify-center opacity-10">
            <span class="text-7xl">🧙</span>
          </div>
          <div class="relative z-10">
            <h2 class="text-base font-bold leading-tight" style="color: var(--dnd-parchment);">{{ c.name }}</h2>
            <p class="text-xs" style="color: var(--dnd-gold);">{{ c.raceName }} · {{ c.classes }}</p>
          </div>
          <div class="absolute top-2 right-2 z-10">
            <span class="text-xs font-bold px-2 py-0.5 rounded" style="background: rgba(200,155,60,0.2); color: var(--dnd-gold); border: 1px solid var(--dnd-gold-dark);">
              Lvl {{ c.totalLevel }}
            </span>
          </div>
        </div>

        <!-- Stats -->
        <div class="p-3 space-y-2">
          <div class="flex justify-between text-xs" style="color: var(--dnd-parchment-dim);">
            <span>Hit Points</span>
            <span style="color: var(--dnd-parchment);">{{ c.currentHP }} / {{ c.maxHP }}</span>
          </div>
          <div class="hp-bar-track">
            <div
              class="hp-bar-fill"
              :style="{
                width: hpPercent(c.currentHP ?? 0, c.maxHP ?? 0) + '%',
                background: hpColor(hpPercent(c.currentHP ?? 0, c.maxHP ?? 0))
              }"
            />
          </div>

          <div class="flex gap-2 pt-1">
            <Button
              label="View Sheet"
              icon="pi pi-eye"
              size="small"
              class="flex-1"
              @click.stop="router.push(`/characters/${c.id}`)"
            />
            <Button
              icon="pi pi-trash"
              severity="danger"
              size="small"
              text
              @click.stop="confirmDelete(c.id!, c.name!)"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
