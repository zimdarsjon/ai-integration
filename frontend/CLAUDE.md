# Frontend Agent CLAUDE.md

## Stack
- Vue 3 (Composition API + `<script setup>`)
- TypeScript (strict mode)
- PrimeVue 4 (component library)
- Pinia (state management)
- Vue Router 4
- Axios (HTTP client)
- openapi-typescript (generated API types)

## Project Structure
```
frontend/src/
├── api/
│   ├── client.ts            ← Central Axios instance (auth + error interceptors)
│   └── generated/
│       └── schema.d.ts      ← Auto-generated from OpenAPI spec — NEVER edit manually
├── components/              ← Reusable, dumb UI components
│   └── [Module]/            ← Grouped by domain: Users/, Products/, etc.
├── composables/             ← Reusable logic, prefixed with "use"
│   └── useAuth.ts
├── stores/                  ← Pinia stores, one per domain
│   └── useAuthStore.ts
├── views/                   ← Route-level page components
│   └── [Module]/
├── router/
│   └── index.ts             ← Vue Router config with auth guards
├── types/                   ← App-level TypeScript types (not generated)
└── main.ts
```

## Core Rules

### Components
- Always use `<script setup lang="ts">` — never Options API
- Components are **dumb**: they receive props, emit events, call composables — no direct API calls
- PascalCase filenames: `UserCard.vue`, `ProductTable.vue`
- Group by domain, not by type: `/components/Users/UserCard.vue` not `/components/cards/UserCard.vue`
- Use PrimeVue components as the base — don't rebuild what PrimeVue provides

```vue
<!-- CORRECT -->
<script setup lang="ts">
import { useUsers } from '@/composables/useUsers'
const { users, loading } = useUsers()
</script>

<!-- WRONG — API call in component -->
<script setup lang="ts">
import axios from 'axios'
const users = await axios.get('/api/users')
</script>
```

### Composables
- All API calls and business logic live in composables or stores
- Prefix with `use`: `useUsers.ts`, `useProducts.ts`
- Return reactive refs + async functions (never return raw promises)
- Handle loading and error state internally

```typescript
// useUsers.ts
export function useUsers() {
  const users = ref<UserDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchUsers() {
    loading.value = true
    error.value = null
    try {
      users.value = await userApi.getAll()
    } catch (e) {
      error.value = 'Failed to load users'
    } finally {
      loading.value = false
    }
  }

  return { users, loading, error, fetchUsers }
}
```

### Pinia Stores
- One store per domain: `useAuthStore`, `useUserStore`, `useProductStore`
- Use the **setup store** syntax (not options syntax)
- Stores manage global/shared state — local component state stays in composables
- Auth store is the source of truth for the current user and JWT token

```typescript
// stores/useAuthStore.ts
export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const user = ref<UserDto | null>(null)
  const isAuthenticated = computed(() => !!token.value)

  function setToken(newToken: string) { token.value = newToken }
  function logout() { token.value = null; user.value = null }

  return { token, user, isAuthenticated, setToken, logout }
})
```

### Axios Client (`src/api/client.ts`)
- Single Axios instance — never import raw `axios` in composables or components
- Request interceptor attaches JWT: `Authorization: Bearer <token>`
- Response interceptor handles 401 → redirect to `/login`

```typescript
// api/client.ts
const client = axios.create({ baseURL: import.meta.env.VITE_API_URL })

client.interceptors.request.use((config) => {
  const auth = useAuthStore()
  if (auth.token) config.headers.Authorization = `Bearer ${auth.token}`
  return config
})

client.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401) router.push('/login')
    return Promise.reject(error)
  }
)

export default client
```

## TypeScript
- Strict mode enabled — no `any`, no type assertions without justification
- API response types come from `src/api/generated/schema.d.ts` — use them directly
- Define prop types inline with `defineProps<{ ... }>()`
- Use `emit` with typed declarations: `defineEmits<{ submit: [user: UserDto] }>()`

## PrimeVue Usage
- Import components individually (not globally) for tree-shaking
- Use PrimeVue's `useToast()` for all user notifications
- Use PrimeVue's `useConfirm()` for destructive action confirmations
- Prefer PrimeVue's `DataTable` for any tabular data
- Theme and PrimeIcons configured in `main.ts`

## Vue Router
- Auth guard in `router/index.ts` using `beforeEach` — checks `useAuthStore().isAuthenticated`
- Protected routes use `meta: { requiresAuth: true }`
- Lazy-load route components: `component: () => import('@/views/Users/UserListView.vue')`

## Environment Variables
- All env vars prefixed with `VITE_`
- `VITE_API_URL` — base URL for the .NET API
- Access via `import.meta.env.VITE_API_URL` (never `process.env`)

## Generated Types — Critical Rules
- `src/api/generated/schema.d.ts` is **auto-generated** — never edit it manually
- To regenerate after API changes:
  ```bash
  npx openapi-typescript ../contracts/openapi.yaml -o src/api/generated/schema.d.ts
  ```
- Import types from the generated file for all API request/response shapes
