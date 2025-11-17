<template>
  <div class="min-h-screen bg-[#202124] flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <!-- Logo Header -->
      <div class="text-center mb-8">
        <div class="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gradient-to-br from-[#8AB4F8] to-[#669DF6] mb-4 shadow-2xl">
          <svg class="w-12 h-12 text-[#202124]" fill="currentColor" viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 3c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm0 14.2c-2.5 0-4.71-1.28-6-3.22.03-1.99 4-3.08 6-3.08 1.99 0 5.97 1.09 6 3.08-1.29 1.94-3.5 3.22-6 3.22z"/>
            <circle cx="12" cy="8" r="2" fill="#202124"/>
            <path d="M12 14c-2 0-4 1-4 2v1h8v-1c0-1-2-2-4-2z" fill="#202124"/>
          </svg>
        </div>
        <h1 class="text-3xl font-bold text-[#E8EAED] mb-2">EmailsP - Anónimo</h1>
        <p class="text-[#9AA0A6] mb-1">🔒 Envío de correos anónimos y privados</p>
        <p class="text-[#8AB4F8] text-sm">Tu identidad protegida siempre</p>
      </div>

      <!-- Login Card -->
      <div class="bg-[#292A2D] rounded-2xl shadow-2xl p-8 border border-[#3C4043]">
        <form @submit.prevent="handleLogin" class="space-y-6">
          <!-- Username Input -->
          <div>
            <label for="username" class="block text-sm font-medium text-[#E8EAED] mb-2">
              Usuario
            </label>
            <input
              id="username"
              v-model="username"
              type="text"
              required
              class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent transition-all"
              placeholder="admin"
            />
          </div>

          <!-- Password Input -->
          <div>
            <label for="password" class="block text-sm font-medium text-[#E8EAED] mb-2">
              Contraseña
            </label>
            <input
              id="password"
              v-model="password"
              type="password"
              required
              class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent transition-all"
              placeholder="••••••••"
            />
          </div>

          <!-- Error Message -->
          <div v-if="authStore.error" class="bg-[#3C1F1F] border border-[#F28B82] rounded-lg p-4">
            <p class="text-[#F28B82] text-sm">{{ authStore.error }}</p>
          </div>

          <!-- Submit Button -->
          <button
            type="submit"
            :disabled="authStore.isLoading"
            class="w-full py-3 px-4 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-xl transform hover:-translate-y-0.5"
          >
            <span v-if="!authStore.isLoading">Iniciar Sesión</span>
            <span v-else class="flex items-center justify-center gap-2">
              <svg class="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Cargando...
            </span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('admin')
const password = ref('admin123')

const handleLogin = async () => {
  const success = await authStore.login(username.value, password.value)
  if (success) {
    router.push('/send-email')
  }
}
</script>
