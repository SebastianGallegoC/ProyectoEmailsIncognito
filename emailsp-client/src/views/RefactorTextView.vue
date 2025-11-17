<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center gap-4 mb-8">
      <div class="w-12 h-12 rounded-xl bg-[#8AB4F8] flex items-center justify-center">
        <svg class="w-7 h-7 text-[#202124]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
        </svg>
      </div>
      <div>
        <h1 class="text-3xl font-bold text-[#E8EAED]">Formalizar Texto</h1>
        <p class="text-[#9AA0A6]">Convierte texto informal en formal usando IA</p>
      </div>
    </div>

    <!-- Main Content -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Input Section -->
      <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border border-[#3C4043]">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-[#E8EAED]">Texto Original</h2>
          <span class="text-xs text-[#9AA0A6]">{{ inputText.length }} / 2000</span>
        </div>
        <textarea
          v-model="inputText"
          rows="12"
          maxlength="2000"
          placeholder="Escribe tu texto informal aquí... Ejemplo: 'oye men, la vdd es que el jefe se pasa con los horarios'"
          class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent resize-none"
        ></textarea>
        
        <button
          @click="handleRefactor"
          :disabled="isLoading || inputText.length === 0"
          class="w-full mt-4 py-3 px-6 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-xl flex items-center justify-center gap-2"
        >
          <svg v-if="!isLoading" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
          </svg>
          <svg v-else class="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span>{{ isLoading ? 'Formalizando...' : 'Formalizar Texto' }}</span>
        </button>
      </div>

      <!-- Output Section -->
      <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border border-[#3C4043]">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-[#E8EAED]">Texto Formal</h2>
          <button
            v-if="formalText"
            @click="copyToClipboard"
            class="text-[#8AB4F8] hover:text-[#669DF6] transition-colors flex items-center gap-1 text-sm"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
            </svg>
            {{ copied ? 'Copiado!' : 'Copiar' }}
          </button>
        </div>

        <div
          v-if="!formalText && !isLoading"
          class="h-[288px] flex flex-col items-center justify-center text-center text-[#9AA0A6]"
        >
          <svg class="w-16 h-16 mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p>El texto formalizado aparecerá aquí</p>
        </div>

        <div
          v-else
          class="min-h-[288px] px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] whitespace-pre-wrap"
        >
          {{ formalText }}
        </div>

        <div v-if="formalText" class="mt-4 flex gap-2">
          <button
            @click="useFormalText"
            class="flex-1 py-2 px-4 bg-[#3C4043] text-[#E8EAED] rounded-lg hover:bg-[#5F6368] transition-colors text-sm"
          >
            Usar en Correo
          </button>
          <button
            @click="clearAll"
            class="px-4 py-2 bg-[#3C4043] text-[#F28B82] rounded-lg hover:bg-[#5F6368] transition-colors text-sm"
          >
            Limpiar
          </button>
        </div>
      </div>
    </div>

    <!-- Error Message -->
    <div v-if="errorMessage" class="bg-[#3C1F1F] border border-[#F28B82] rounded-lg p-4">
      <p class="text-[#F28B82]">{{ errorMessage }}</p>
    </div>

    <!-- Examples -->
    <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border border-[#3C4043]">
      <h3 class="text-lg font-semibold text-[#E8EAED] mb-4">Ejemplos de Uso</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <button
          v-for="(example, index) in examples"
          :key="index"
          @click="inputText = example"
          class="p-4 bg-[#3C4043] rounded-lg text-left hover:bg-[#5F6368] transition-colors group"
        >
          <p class="text-[#9AA0A6] text-sm mb-2 group-hover:text-[#8AB4F8]">Ejemplo {{ index + 1 }}</p>
          <p class="text-[#E8EAED] text-sm line-clamp-2">{{ example }}</p>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { aiService } from '@/services/aiService'

const router = useRouter()
const inputText = ref('')
const formalText = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const copied = ref(false)

const examples = [
  'oye men, la vdd es que el jefe se pasa con los horarios, no mms',
  'wey necesito que me ayudes con este proyecto, esta re dificil',
  'hola profe, disculpa pero no pude hacer la tarea porque se me olvido',
  'brother, el cliente esta re molesto con el proyecto, que hacemos?'
]

const handleRefactor = async () => {
  if (inputText.value.length === 0) return

  isLoading.value = true
  errorMessage.value = ''
  formalText.value = ''

  try {
    const response = await aiService.refactorText(inputText.value)
    formalText.value = response.formalText
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Error al formalizar el texto'
  } finally {
    isLoading.value = false
  }
}

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(formalText.value)
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 2000)
  } catch (error) {
    console.error('Error al copiar:', error)
  }
}

const useFormalText = () => {
  // Guardar en localStorage para usar en la vista de enviar correo
  localStorage.setItem('emailBody', formalText.value)
  router.push('/send-email')
}

const clearAll = () => {
  inputText.value = ''
  formalText.value = ''
  errorMessage.value = ''
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
