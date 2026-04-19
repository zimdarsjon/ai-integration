<script setup lang="ts">
import { ref } from 'vue'
import { useAuth } from '@/composables/useAuth'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'
import Message from 'primevue/message'

const { register, loading, error } = useAuth()

const firstName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')

async function onSubmit() {
  await register(email.value, password.value, firstName.value, lastName.value)
}
</script>

<template>
  <div class="auth-container">
    <div class="auth-card">
      <div class="text-center mb-8">
        <div class="text-5xl mb-3">📜</div>
        <h1 class="text-2xl font-bold mb-1" style="color: var(--dnd-gold);">Begin Your Legend</h1>
        <p class="text-sm" style="color: var(--dnd-parchment-dim);">Register your name in the annals of adventure</p>
      </div>

      <Message v-if="error" severity="error" class="mb-4">{{ error }}</Message>

      <form @submit.prevent="onSubmit" class="flex flex-col gap-4">
        <div class="flex gap-3">
          <div class="flex flex-col gap-1.5 flex-1">
            <label for="firstName" class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
              First Name
            </label>
            <InputText
              id="firstName"
              v-model="firstName"
              placeholder="Aria"
              autocomplete="given-name"
              required
              class="w-full"
            />
          </div>
          <div class="flex flex-col gap-1.5 flex-1">
            <label for="lastName" class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
              Last Name
            </label>
            <InputText
              id="lastName"
              v-model="lastName"
              placeholder="Brightblade"
              autocomplete="family-name"
              required
              class="w-full"
            />
          </div>
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="email" class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
            Email
          </label>
          <InputText
            id="email"
            v-model="email"
            type="email"
            placeholder="you@example.com"
            autocomplete="email"
            required
            class="w-full"
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="password" class="text-xs font-semibold uppercase tracking-widest" style="color: var(--dnd-gold);">
            Password
          </label>
          <Password
            id="password"
            v-model="password"
            toggleMask
            placeholder="At least 8 characters"
            autocomplete="new-password"
            required
            class="w-full"
            inputClass="w-full"
          />
        </div>

        <Button
          type="submit"
          label="Forge Your Legend"
          icon="pi pi-star"
          :loading="loading"
          class="w-full mt-2"
        />
      </form>

      <div class="mt-6 pt-4 text-center text-sm" style="border-top: 1px solid var(--dnd-border); color: var(--dnd-parchment-dim);">
        Already adventuring?
        <RouterLink to="/login" class="font-semibold ml-1 hover:underline" style="color: var(--dnd-gold);">
          Sign in
        </RouterLink>
      </div>
    </div>
  </div>
</template>
