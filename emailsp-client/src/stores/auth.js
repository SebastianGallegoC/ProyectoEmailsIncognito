import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService } from '@/services/authService'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(authService.getToken())
  const username = ref(localStorage.getItem('emailsP_username') || '')
  const isLoading = ref(false)
  const error = ref(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value)

  // Actions
  async function login(user, pass) {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await authService.login(user, pass)
      token.value = response.token
      username.value = user
      localStorage.setItem('emailsP_username', user)
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Error al iniciar sesión'
      return false
    } finally {
      isLoading.value = false
    }
  }

  function logout() {
    authService.logout()
    token.value = null
    username.value = ''
    localStorage.removeItem('emailsP_username')
  }

  return {
    // State
    token,
    username,
    isLoading,
    error,
    // Getters
    isAuthenticated,
    // Actions
    login,
    logout
  }
})
