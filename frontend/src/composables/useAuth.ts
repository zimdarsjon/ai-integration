import { ref } from 'vue'
import client from '@/api/client'
import { useAuthStore, type AuthResponseDto } from '@/stores/useAuthStore'
import { useRouter } from 'vue-router'

export function useAuth() {
  const authStore = useAuthStore()
  const router = useRouter()
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function login(email: string, password: string) {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.post<AuthResponseDto>('/api/auth/login', { email, password })
      authStore.setAuth(data)
      await router.push('/')
    } catch (e: unknown) {
      error.value = extractError(e)
    } finally {
      loading.value = false
    }
  }

  async function register(email: string, password: string, firstName: string, lastName: string) {
    loading.value = true
    error.value = null
    try {
      const { data } = await client.post<AuthResponseDto>('/api/auth/register', {
        email,
        password,
        firstName,
        lastName
      })
      authStore.setAuth(data)
      await router.push('/')
    } catch (e: unknown) {
      error.value = extractError(e)
    } finally {
      loading.value = false
    }
  }

  function logout() {
    authStore.logout()
    router.push('/login')
  }

  return { loading, error, login, register, logout }
}

function extractError(e: unknown): string {
  if (
    typeof e === 'object' &&
    e !== null &&
    'response' in e &&
    typeof (e as { response?: { data?: { error?: string } } }).response?.data?.error === 'string'
  ) {
    return (e as { response: { data: { error: string } } }).response.data.error
  }
  return 'Something went wrong. Please try again.'
}
