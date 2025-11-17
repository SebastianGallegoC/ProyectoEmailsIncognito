<template>
  <aside 
    class="fixed left-0 top-0 h-screen w-64 bg-[#292A2D] shadow-2xl z-50 flex flex-col border-r border-[#3C4043]"
  >
    <!-- Logo/Header -->
    <div class="p-6 border-b border-[#3C4043]">
      <div class="flex items-center gap-3">
        <div class="w-10 h-10 rounded-full bg-gradient-to-br from-[#8AB4F8] to-[#669DF6] flex items-center justify-center">
          <svg class="w-6 h-6 text-[#202124]" fill="currentColor" viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 3c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm0 14.2c-2.5 0-4.71-1.28-6-3.22.03-1.99 4-3.08 6-3.08 1.99 0 5.97 1.09 6 3.08-1.29 1.94-3.5 3.22-6 3.22z"/>
          </svg>
        </div>
        <div>
          <h1 class="text-[#E8EAED] font-semibold text-lg flex items-center gap-2">
            EmailsP 
            <span class="text-xs bg-[#8AB4F8] text-[#202124] px-2 py-0.5 rounded-full">Anónimo</span>
          </h1>
          <p class="text-[#9AA0A6] text-xs flex items-center gap-1">
            🔒 Modo Privado
          </p>
        </div>
      </div>
    </div>

    <!-- Navigation Menu -->
    <nav class="flex-1 p-4 overflow-y-auto">
      <ul class="space-y-2">
        <li v-for="item in menuItems" :key="item.path">
          <router-link
            :to="item.path"
            class="flex items-center gap-3 px-4 py-3 rounded-lg transition-all duration-200 group"
            :class="isActive(item.path) 
              ? 'bg-[#8AB4F8] text-[#202124] shadow-lg' 
              : 'text-[#E8EAED] hover:bg-[#3C4043] hover:text-[#8AB4F8]'"
          >
            <component 
              :is="item.icon" 
              class="w-5 h-5 transition-transform group-hover:scale-110"
            />
            <span class="font-medium">{{ item.label }}</span>
          </router-link>
        </li>
      </ul>
    </nav>

    <!-- User Info & Logout -->
    <div class="p-4 border-t border-[#3C4043]">
      <div class="flex items-center justify-between px-4 py-3 rounded-lg bg-[#3C4043]">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-full bg-[#8AB4F8] flex items-center justify-center">
            <span class="text-[#202124] font-semibold text-sm">{{ userInitial }}</span>
          </div>
          <span class="text-[#E8EAED] text-sm font-medium">{{ username }}</span>
        </div>
        <button
          @click="handleLogout"
          class="text-[#9AA0A6] hover:text-[#F28B82] transition-colors"
          title="Cerrar sesión"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
        </button>
      </div>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Icons as components
const MailIcon = {
  template: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>`
}

const ContactsIcon = {
  template: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" /></svg>`
}

const SparklesIcon = {
  template: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" /></svg>`
}

const AnalyticsIcon = {
  template: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" /></svg>`
}

const menuItems = [
  { path: '/send-email', label: 'Enviar Correo', icon: MailIcon },
  { path: '/contacts', label: 'Contactos', icon: ContactsIcon },
  { path: '/refactor-text', label: 'Formalizar Texto', icon: SparklesIcon },
  { path: '/analyze-message', label: 'Analizar Mensaje', icon: AnalyticsIcon }
]

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const username = computed(() => authStore.username || 'Usuario')
const userInitial = computed(() => username.value.charAt(0).toUpperCase())

const isActive = (path) => {
  return route.path === path
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
