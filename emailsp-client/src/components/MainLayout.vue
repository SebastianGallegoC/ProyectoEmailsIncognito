<template>
  <div class="min-h-screen bg-[#202124] flex">
    <!-- Sidebar -->
    <AppSidebar />

    <!-- Main Content Area -->
    <main class="flex-1 ml-64 p-8">
      <div class="max-w-7xl mx-auto">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import AppSidebar from '@/components/AppSidebar.vue'
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
