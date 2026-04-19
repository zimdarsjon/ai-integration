import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface UserDto {
  id: number
  email: string
  firstName: string
  lastName: string
  createdAt: string
}

export interface AuthResponseDto {
  token: string
  expiresAt: string
  user: UserDto
}

const TOKEN_KEY = 'auth_token'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(TOKEN_KEY))
  const user = ref<UserDto | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  function setAuth(response: AuthResponseDto) {
    token.value = response.token
    user.value = response.user
    localStorage.setItem(TOKEN_KEY, response.token)
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
  }

  return { token, user, isAuthenticated, setAuth, logout }
})
