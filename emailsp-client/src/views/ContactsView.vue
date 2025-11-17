<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between mb-8">
      <div class="flex items-center gap-4">
        <div class="w-12 h-12 rounded-xl bg-[#8AB4F8] flex items-center justify-center">
          <svg class="w-7 h-7 text-[#202124]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
        </div>
        <div>
          <h1 class="text-3xl font-bold text-[#E8EAED]">Contactos</h1>
          <p class="text-[#9AA0A6]">Gestiona tus contactos de correo</p>
        </div>
      </div>
      <button
        @click="showCreateModal = true"
        class="py-3 px-6 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 shadow-lg hover:shadow-xl flex items-center gap-2"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Nuevo Contacto
      </button>
    </div>

    <!-- Search & Filters -->
    <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border border-[#3C4043]">
      <div class="flex gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            @input="handleSearch"
            type="text"
            placeholder="Buscar por nombre, email o teléfono..."
            class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
          />
        </div>
      </div>
    </div>

    <!-- Contacts List -->
    <div v-if="isLoading" class="bg-[#292A2D] rounded-2xl shadow-xl p-12 border border-[#3C4043] text-center">
      <svg class="animate-spin h-12 w-12 text-[#8AB4F8] mx-auto mb-4" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
      <p class="text-[#9AA0A6]">Cargando contactos...</p>
    </div>

    <div v-else-if="contacts.length === 0" class="bg-[#292A2D] rounded-2xl shadow-xl p-12 border border-[#3C4043] text-center">
      <svg class="w-16 h-16 text-[#9AA0A6] mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
      </svg>
      <p class="text-[#9AA0A6] mb-4">No hay contactos</p>
      <button
        @click="showCreateModal = true"
        class="text-[#8AB4F8] hover:text-[#669DF6] font-medium"
      >
        Crear tu primer contacto
      </button>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="contact in contacts"
        :key="contact.id"
        class="bg-[#292A2D] rounded-xl shadow-lg p-6 border border-[#3C4043] hover:border-[#8AB4F8] transition-all group"
      >
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 rounded-full bg-[#8AB4F8] flex items-center justify-center">
              <span class="text-[#202124] font-semibold text-lg">{{ contact.name.charAt(0).toUpperCase() }}</span>
            </div>
            <div>
              <h3 class="text-[#E8EAED] font-semibold">{{ contact.name }}</h3>
              <div class="flex items-center gap-2 mt-1">
                <button
                  @click="toggleFavorite(contact)"
                  class="transition-colors"
                  :class="contact.isFavorite ? 'text-yellow-400' : 'text-[#9AA0A6] hover:text-yellow-400'"
                >
                  <svg class="w-4 h-4" :fill="contact.isFavorite ? 'currentColor' : 'none'" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
                  </svg>
                </button>
                <span
                  v-if="contact.isBlocked"
                  class="text-[#F28B82] text-xs font-medium"
                >
                  Bloqueado
                </span>
              </div>
            </div>
          </div>
        </div>

        <div class="space-y-2 mb-4">
          <div class="flex items-center gap-2 text-sm">
            <svg class="w-4 h-4 text-[#9AA0A6]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
            </svg>
            <span class="text-[#E8EAED]">{{ contact.email }}</span>
          </div>
          <div v-if="contact.phoneNumber" class="flex items-center gap-2 text-sm">
            <svg class="w-4 h-4 text-[#9AA0A6]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
            </svg>
            <span class="text-[#E8EAED]">{{ contact.phoneNumber }}</span>
          </div>
        </div>

        <div class="flex gap-2">
          <button
            @click="editContact(contact)"
            class="flex-1 py-2 px-3 bg-[#3C4043] text-[#E8EAED] rounded-lg hover:bg-[#5F6368] transition-colors text-sm"
          >
            Editar
          </button>
          <button
            @click="toggleBlock(contact)"
            class="py-2 px-3 bg-[#3C4043] rounded-lg hover:bg-[#5F6368] transition-colors text-sm"
            :class="contact.isBlocked ? 'text-[#81C784]' : 'text-[#F28B82]'"
          >
            {{ contact.isBlocked ? 'Desbloquear' : 'Bloquear' }}
          </button>
          <button
            @click="deleteContact(contact)"
            class="py-2 px-3 bg-[#3C4043] text-[#F28B82] rounded-lg hover:bg-[#5F6368] transition-colors text-sm"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="totalPages > 1" class="flex items-center justify-center gap-2">
      <button
        @click="changePage(currentPage - 1)"
        :disabled="currentPage === 1"
        class="py-2 px-4 bg-[#292A2D] text-[#E8EAED] rounded-lg hover:bg-[#3C4043] transition-colors disabled:opacity-50 disabled:cursor-not-allowed border border-[#3C4043]"
      >
        Anterior
      </button>
      <span class="text-[#E8EAED] px-4">Página {{ currentPage }} de {{ totalPages }}</span>
      <button
        @click="changePage(currentPage + 1)"
        :disabled="currentPage === totalPages"
        class="py-2 px-4 bg-[#292A2D] text-[#E8EAED] rounded-lg hover:bg-[#3C4043] transition-colors disabled:opacity-50 disabled:cursor-not-allowed border border-[#3C4043]"
      >
        Siguiente
      </button>
    </div>

    <!-- Create/Edit Modal -->
    <Teleport to="body">
      <div
        v-if="showCreateModal || showEditModal"
        class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 p-4"
        @click.self="closeModal"
      >
        <div class="bg-[#292A2D] rounded-2xl shadow-2xl p-8 border border-[#3C4043] max-w-md w-full">
          <h2 class="text-2xl font-bold text-[#E8EAED] mb-6">
            {{ showEditModal ? 'Editar Contacto' : 'Nuevo Contacto' }}
          </h2>

          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-[#E8EAED] mb-2">Nombre</label>
              <input
                v-model="form.name"
                type="text"
                required
                class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-[#E8EAED] mb-2">Email</label>
              <input
                v-model="form.email"
                type="email"
                required
                class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-[#E8EAED] mb-2">Teléfono (Opcional)</label>
              <input
                v-model="form.phoneNumber"
                type="tel"
                class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
              />
            </div>

            <div v-if="modalError" class="bg-[#3C1F1F] border border-[#F28B82] rounded-lg p-4">
              <p class="text-[#F28B82] text-sm">{{ modalError }}</p>
            </div>

            <div class="flex gap-4 pt-4">
              <button
                type="submit"
                :disabled="isSubmitting"
                class="flex-1 py-3 px-6 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg"
              >
                {{ isSubmitting ? 'Guardando...' : 'Guardar' }}
              </button>
              <button
                type="button"
                @click="closeModal"
                class="px-6 py-3 bg-[#3C4043] text-[#E8EAED] rounded-lg hover:bg-[#5F6368] transition-colors"
              >
                Cancelar
              </button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { contactService } from '@/services/contactService'

const contacts = ref([])
const isLoading = ref(false)
const searchQuery = ref('')
const currentPage = ref(1)
const totalPages = ref(1)
const pageSize = 10

const showCreateModal = ref(false)
const showEditModal = ref(false)
const isSubmitting = ref(false)
const modalError = ref('')

const form = ref({
  id: null,
  name: '',
  email: '',
  phoneNumber: ''
})

const loadContacts = async () => {
  isLoading.value = true
  try {
    const response = await contactService.getContacts({
      q: searchQuery.value,
      page: currentPage.value,
      pageSize
    })
    contacts.value = response.items
    totalPages.value = Math.ceil(response.total / pageSize)
  } catch (error) {
    console.error('Error loading contacts:', error)
  } finally {
    isLoading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadContacts()
}

const changePage = (page) => {
  currentPage.value = page
  loadContacts()
}

const toggleFavorite = async (contact) => {
  try {
    if (contact.isFavorite) {
      await contactService.removeFavorite(contact.id)
    } else {
      await contactService.markAsFavorite(contact.id)
    }
    loadContacts()
  } catch (error) {
    console.error('Error toggling favorite:', error)
  }
}

const toggleBlock = async (contact) => {
  try {
    if (contact.isBlocked) {
      await contactService.unblockContact(contact.id)
    } else {
      await contactService.blockContact(contact.id)
    }
    loadContacts()
  } catch (error) {
    console.error('Error toggling block:', error)
  }
}

const editContact = (contact) => {
  form.value = { ...contact }
  showEditModal.value = true
}

const deleteContact = async (contact) => {
  if (confirm(`¿Estás seguro de eliminar a ${contact.name}?`)) {
    try {
      await contactService.deleteContact(contact.id)
      loadContacts()
    } catch (error) {
      console.error('Error deleting contact:', error)
    }
  }
}

const handleSubmit = async () => {
  isSubmitting.value = true
  modalError.value = ''

  try {
    if (showEditModal.value) {
      await contactService.updateContact(form.value.id, form.value)
    } else {
      await contactService.createContact(form.value)
    }
    closeModal()
    loadContacts()
  } catch (error) {
    modalError.value = error.response?.data?.message || 'Error al guardar el contacto'
  } finally {
    isSubmitting.value = false
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  modalError.value = ''
  form.value = {
    id: null,
    name: '',
    email: '',
    phoneNumber: ''
  }
}

onMounted(() => {
  loadContacts()
})
</script>
