<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center gap-4 mb-8">
      <div class="w-12 h-12 rounded-xl bg-[#8AB4F8] flex items-center justify-center">
        <svg class="w-7 h-7 text-[#202124]" fill="currentColor" viewBox="0 0 24 24">
          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 3c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm0 14.2c-2.5 0-4.71-1.28-6-3.22.03-1.99 4-3.08 6-3.08 1.99 0 5.97 1.09 6 3.08-1.29 1.94-3.5 3.22-6 3.22z"/>
        </svg>
      </div>
      <div>
        <h1 class="text-3xl font-bold text-[#E8EAED] flex items-center gap-3">
          Enviar Correo Anónimo.
          <span class="text-sm bg-[#8AB4F8] text-[#202124] px-3 py-1 rounded-full font-normal">🔒 Privado</span>
        </h1>
        <p class="text-[#9AA0A6]">Tu identidad permanecerá oculta • Envío seguro y anónimo</p>
      </div>
    </div>

    <!-- Main Form Card -->
    <div class="bg-[#292A2D] rounded-2xl shadow-xl p-8 border border-[#3C4043]">
      <form @submit.prevent="handleSendEmail" class="space-y-6">
        <!-- Recipients -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <label class="block text-sm font-medium text-[#E8EAED]">
              Destinatarios
            </label>
            <button
              type="button"
              @click="showContactPicker = true"
              class="text-[#8AB4F8] hover:text-[#669DF6] text-sm font-medium transition-colors flex items-center gap-1"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              Seleccionar de Contactos
            </button>
          </div>
          <div class="space-y-2">
            <div v-for="(recipient, index) in recipients" :key="index" class="flex gap-2">
              <input
                v-model="recipients[index]"
                type="email"
                required
                placeholder="destinatario@example.com"
                class="flex-1 px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
              />
              <button
                v-if="recipients.length > 1"
                type="button"
                @click="removeRecipient(index)"
                class="px-4 py-2 bg-[#3C4043] text-[#F28B82] rounded-lg hover:bg-[#5F6368] transition-colors"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
          </div>
          <button
            type="button"
            @click="addRecipient"
            class="mt-2 text-[#8AB4F8] hover:text-[#669DF6] text-sm font-medium transition-colors flex items-center gap-1"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            Agregar destinatario
          </button>
        </div>

        <!-- Subject -->
        <div>
          <label for="subject" class="block text-sm font-medium text-[#E8EAED] mb-2">
            Asunto
          </label>
          <input
            id="subject"
            v-model="subject"
            type="text"
            required
            placeholder="Ingresa el asunto del correo"
            class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
          />
        </div>

        <!-- Body -->
        <div>
          <label for="body" class="block text-sm font-medium text-[#E8EAED] mb-2">
            Mensaje
          </label>
          <textarea
            id="body"
            v-model="body"
            rows="16"
            required
            placeholder="Escribe tu mensaje aquí... (Soporta HTML)"
            class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent resize-none"
          ></textarea>
          <p class="mt-1 text-xs text-[#9AA0A6]">Puedes usar HTML en el mensaje</p>
        </div>

        <!-- Success/Error Messages -->
        <div v-if="successMessage" class="bg-[#1F3A1F] border border-[#81C784] rounded-lg p-4">
          <p class="text-[#81C784]">{{ successMessage }}</p>
        </div>

        <div v-if="errorMessage" class="bg-[#3C1F1F] border border-[#F28B82] rounded-lg p-4">
          <p class="text-[#F28B82]">{{ errorMessage }}</p>
        </div>

        <!-- Actions -->
        <div class="flex gap-4 pt-4">
          <button
            type="submit"
            :disabled="isLoading"
            class="flex-1 py-3 px-6 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-xl"
          >
            <span v-if="!isLoading">Enviar Correo</span>
            <span v-else class="flex items-center justify-center gap-2">
              <svg class="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Enviando...
            </span>
          </button>
          <button
            type="button"
            @click="resetForm"
            class="px-6 py-3 bg-[#3C4043] text-[#E8EAED] rounded-lg hover:bg-[#5F6368] transition-colors"
          >
            Limpiar
          </button>
        </div>
      </form>
    </div>

    <!-- Contact Picker Modal -->
    <Teleport to="body">
      <div
        v-if="showContactPicker"
        class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 p-4"
        @click.self="showContactPicker = false"
      >
        <div class="bg-[#292A2D] rounded-2xl shadow-2xl border border-[#3C4043] max-w-2xl w-full max-h-[80vh] flex flex-col">
          <div class="p-6 border-b border-[#3C4043]">
            <h2 class="text-2xl font-bold text-[#E8EAED]">Seleccionar Contactos</h2>
            <p class="text-[#9AA0A6] text-sm mt-1">Selecciona uno o más contactos para agregar como destinatarios</p>
          </div>

          <div class="flex-1 overflow-y-auto p-6">
            <div v-if="loadingContacts" class="text-center py-8">
              <svg class="animate-spin h-8 w-8 text-[#8AB4F8] mx-auto" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-[#9AA0A6] mt-2">Cargando contactos...</p>
            </div>

            <div v-else-if="contacts.length === 0" class="text-center py-8">
              <svg class="w-16 h-16 text-[#9AA0A6] mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              <p class="text-[#9AA0A6]">No tienes contactos guardados</p>
            </div>

            <div v-else class="space-y-2">
              <label
                v-for="contact in contacts"
                :key="contact.id"
                class="flex items-center gap-3 p-4 bg-[#3C4043] rounded-lg hover:bg-[#5F6368] transition-colors cursor-pointer"
              >
                <input
                  type="checkbox"
                  :value="contact.id"
                  v-model="selectedContacts"
                  class="w-5 h-5 rounded border-[#5F6368] text-[#8AB4F8] focus:ring-2 focus:ring-[#8AB4F8] focus:ring-offset-0 bg-[#292A2D]"
                />
                <div class="flex items-center gap-3 flex-1">
                  <div class="w-10 h-10 rounded-full bg-[#8AB4F8] flex items-center justify-center flex-shrink-0">
                    <span class="text-[#202124] font-semibold">{{ contact.name.charAt(0).toUpperCase() }}</span>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2">
                      <p class="text-[#E8EAED] font-medium truncate">{{ contact.name }}</p>
                      <span v-if="contact.isFavorite" class="text-yellow-400">⭐</span>
                    </div>
                    <p class="text-[#9AA0A6] text-sm truncate">{{ contact.email }}</p>
                  </div>
                </div>
              </label>
            </div>
          </div>

          <div class="p-6 border-t border-[#3C4043] flex gap-4">
            <button
              @click="addSelectedContacts"
              :disabled="selectedContacts.length === 0"
              class="flex-1 py-3 px-6 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Agregar {{ selectedContacts.length > 0 ? `(${selectedContacts.length})` : '' }}
            </button>
            <button
              @click="showContactPicker = false"
              class="px-6 py-3 bg-[#3C4043] text-[#E8EAED] rounded-lg hover:bg-[#5F6368] transition-colors"
            >
              Cancelar
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { emailService } from '@/services/emailService'
import { contactService } from '@/services/contactService'

const recipients = ref([''])
const subject = ref('')
const body = ref('')
const isLoading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

// Contact Picker
const showContactPicker = ref(false)
const contacts = ref([])
const loadingContacts = ref(false)
const selectedContacts = ref([])

// Check if there's a body from localStorage (from RefactorTextView)
onMounted(() => {
  const savedBody = localStorage.getItem('emailBody')
  if (savedBody) {
    body.value = savedBody
    localStorage.removeItem('emailBody')
  }
  loadContacts()
})

const loadContacts = async () => {
  loadingContacts.value = true
  try {
    const response = await contactService.getContacts({ pageSize: 100 })
    contacts.value = response.items.filter(c => !c.isBlocked)
  } catch (error) {
    console.error('Error loading contacts:', error)
  } finally {
    loadingContacts.value = false
  }
}

const addSelectedContacts = () => {
  selectedContacts.value.forEach(contactId => {
    const contact = contacts.value.find(c => c.id === contactId)
    if (contact && !recipients.value.includes(contact.email)) {
      // Reemplazar el primer campo vacío o agregar nuevo
      const emptyIndex = recipients.value.findIndex(r => r.trim() === '')
      if (emptyIndex !== -1) {
        recipients.value[emptyIndex] = contact.email
      } else {
        recipients.value.push(contact.email)
      }
    }
  })
  showContactPicker.value = false
  selectedContacts.value = []
}

const addRecipient = () => {
  recipients.value.push('')
}

const removeRecipient = (index) => {
  recipients.value.splice(index, 1)
}

const handleSendEmail = async () => {
  isLoading.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    const validRecipients = recipients.value.filter(r => r.trim() !== '')
    
    // Envolver el cuerpo en HTML con estilos para que se vea bien
    const styledBody = `
      <div style="color: #ffffff; background-color: transparent; font-family: Arial, sans-serif; line-height: 1.6;">
        ${body.value}
      </div>
    `
    
    // Si solo hay un destinatario, usar sendSimpleEmail
    if (validRecipients.length === 1) {
      await emailService.sendSimpleEmail({
        to: validRecipients[0],
        subject: subject.value,
        body: styledBody
      })
    } else {
      await emailService.sendEmail({
        to: validRecipients,
        subject: subject.value,
        body: styledBody,
        attachments: []
      })
    }

    successMessage.value = '¡Correo enviado exitosamente!'
    resetForm()
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Error al enviar el correo'
  } finally {
    isLoading.value = false
  }
}

const resetForm = () => {
  recipients.value = ['']
  subject.value = ''
  body.value = ''
}
</script>
