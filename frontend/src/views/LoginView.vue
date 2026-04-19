<script setup lang="ts">
import { ref } from 'vue'
import { useAuth } from '@/composables/useAuth'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'
import Message from 'primevue/message'

const { login, loading, error } = useAuth()

const email = ref('')
const password = ref('')

async function onSubmit() {
  await login(email.value, password.value)
}
</script>

<template>
  <div class="auth-container">
    <div class="auth-card">
      <!-- Header -->
      <div class="text-center mb-8">
        <div class="text-5xl mb-3">⚔️</div>
        <h1 class="text-2xl font-bold mb-1" style="color: var(--dnd-gold);">Tome &amp; Scroll</h1>
        <p class="text-sm" style="color: var(--dnd-parchment-dim);">Enter your credentials, adventurer</p>
      </div>

      <Message v-if="error" severity="error" class="mb-4">{{ error }}</Message>

      <form @submit.prevent="onSubmit" class="flex flex-col gap-4">
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
            :feedback="false"
            toggleMask
            placeholder="Your secret passphrase"
            autocomplete="current-password"
            required
            class="w-full"
            inputClass="w-full"
          />
        </div>

        <Button
          type="submit"
          label="Enter the Realm"
          icon="pi pi-sign-in"
          :loading="loading"
          class="w-full mt-2"
        />
      </form>

      <div class="mt-6 pt-4 text-center text-sm" style="border-top: 1px solid var(--dnd-border); color: var(--dnd-parchment-dim);">
        New adventurer?
        <RouterLink to="/register" class="font-semibold ml-1 hover:underline" style="color: var(--dnd-gold);">
          Create your legend
        </RouterLink>
      </div>
    </div>
  </div>
</template>
