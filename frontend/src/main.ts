import { createApp } from 'vue'
import { createPinia } from 'pinia'
import PrimeVue from 'primevue/config'
import { definePreset } from '@primevue/themes'
import Aura from '@primevue/themes/aura'
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'
import 'primeicons/primeicons.css'
import router from '@/router'
import App from './App.vue'
import './style.css'

const DndTheme = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#fefce8',
      100: '#fef9c3',
      200: '#fef08a',
      300: '#fde047',
      400: '#e8c06a',
      500: '#c89b3c',
      600: '#a37c2e',
      700: '#8b6520',
      800: '#6b4a14',
      900: '#3d2a08',
      950: '#1e1504'
    },
    colorScheme: {
      dark: {
        surface: {
          0: '#ffffff',
          50: '#e8d5b0',
          100: '#b8a880',
          200: '#8b7a5a',
          300: '#6b5a3a',
          400: '#4a3a24',
          500: '#3d2a14',
          600: '#2a1e0e',
          700: '#1e1410',
          800: '#1a1208',
          900: '#120d05',
          950: '#0f0a08'
        }
      }
    }
  }
})

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: DndTheme,
    options: {
      darkModeSelector: '.app-dark',
      cssLayer: false
    }
  }
})
app.use(ToastService)
app.use(ConfirmationService)

app.mount('#app')
