<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center gap-4 mb-8">
      <div class="w-12 h-12 rounded-xl bg-[#8AB4F8] flex items-center justify-center">
        <svg class="w-7 h-7 text-[#202124]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
        </svg>
      </div>
      <div>
        <h1 class="text-3xl font-bold text-[#E8EAED]">Analizar Mensaje</h1>
        <p class="text-[#9AA0A6]">Evalúa riesgos legales, emocionales y efectividad</p>
      </div>
    </div>

    <!-- Input Section -->
    <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border border-[#3C4043]">
      <form @submit.prevent="handleAnalyze" class="space-y-6">
        <!-- Message Text -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <label class="block text-sm font-medium text-[#E8EAED]">Mensaje a Analizar</label>
            <span class="text-xs text-[#9AA0A6]">{{ messageText.length }} / 5000</span>
          </div>
          <textarea
            v-model="messageText"
            rows="6"
            maxlength="5000"
            required
            placeholder="Escribe el mensaje que deseas analizar..."
            class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] placeholder-[#9AA0A6] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent resize-none"
          ></textarea>
        </div>

        <!-- Context & Country -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-[#E8EAED] mb-2">Contexto</label>
            <select
              v-model="context"
              class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
            >
              <option value="workplace">Laboral</option>
              <option value="academic">Académico</option>
              <option value="personal">Personal</option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-medium text-[#E8EAED] mb-2">País</label>
            <select
              v-model="country"
              class="w-full px-4 py-3 bg-[#3C4043] border border-[#5F6368] rounded-lg text-[#E8EAED] focus:outline-none focus:ring-2 focus:ring-[#8AB4F8] focus:border-transparent"
            >
              <option value="CO">Colombia</option>
              <option value="PE">Perú</option>
              <option value="EC">Ecuador</option>
              <option value="VE">Venezuela</option>
              <option value="BO">Bolivia</option>
              <option value="PA">Panamá</option>
            </select>
          </div>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="isAnalyzing || messageText.length < 10"
          class="w-full py-3 px-6 bg-[#8AB4F8] hover:bg-[#669DF6] text-[#202124] font-semibold rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-xl flex items-center justify-center gap-2"
        >
          <svg v-if="!isAnalyzing" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" />
          </svg>
          <svg v-else class="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span>{{ isAnalyzing ? 'Analizando (10-15s)...' : 'Analizar Mensaje' }}</span>
        </button>
      </form>
    </div>

    <!-- Error Message -->
    <div v-if="errorMessage" class="bg-[#3C1F1F] border border-[#F28B82] rounded-lg p-4">
      <p class="text-[#F28B82]">{{ errorMessage }}</p>
    </div>

    <!-- Analysis Results -->
    <div v-if="analysis" class="space-y-6">
      <!-- Executive Summary -->
      <div 
        class="rounded-2xl shadow-xl p-8 border-2"
        :class="getSummaryCardClasses()"
      >
        <div class="flex items-start gap-4 mb-4">
          <div class="text-4xl">{{ getSummaryIcon() }}</div>
          <div class="flex-1">
            <h2 class="text-2xl font-bold text-white mb-2">{{ analysis.actionableRecommendations.messageStatus }}</h2>
            <p class="text-white/90 text-lg">{{ analysis.actionableRecommendations.executiveSummary }}</p>
          </div>
        </div>
        <div class="mt-6 pt-6 border-t border-white/20">
          <p class="text-white font-bold text-xl text-center">
            Recomendación: {{ translateRecommendation(analysis.actionableRecommendations.finalRecommendation) }}
          </p>
        </div>
      </div>

      <!-- Risk Analysis Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Legal Risk -->
        <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border-l-4" :style="{ borderColor: getRiskColor(analysis.legalRisk.level) }">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg flex items-center justify-center" :style="{ backgroundColor: getRiskColor(analysis.legalRisk.level) + '20' }">
              <span class="text-2xl">⚖️</span>
            </div>
            <div>
              <h3 class="text-lg font-bold text-[#E8EAED]">Riesgo Legal</h3>
              <p class="text-sm font-medium" :style="{ color: getRiskColor(analysis.legalRisk.level) }">
                {{ translateLevel(analysis.legalRisk.level) }}
              </p>
            </div>
          </div>
          <p class="text-[#E8EAED] mb-4">{{ analysis.legalRisk.description }}</p>
          
          <div v-if="analysis.legalRisk.potentialIssues.length" class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Problemas Potenciales:</p>
            <ul class="space-y-1">
              <li v-for="(issue, idx) in analysis.legalRisk.potentialIssues" :key="idx" class="text-sm text-[#E8EAED] flex items-start gap-2">
                <span class="text-[#F28B82]">•</span>
                <span>{{ issue }}</span>
              </li>
            </ul>
          </div>

          <div v-if="analysis.legalRisk.legalReferences.length" class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Referencias Legales:</p>
            <ul class="space-y-1">
              <li v-for="(ref, idx) in analysis.legalRisk.legalReferences" :key="idx" class="text-xs text-[#8AB4F8]">
                {{ ref }}
              </li>
            </ul>
          </div>

          <div class="bg-[#3C4043] rounded-lg p-3">
            <p class="text-xs text-[#9AA0A6]">Realidad práctica:</p>
            <p class="text-sm text-[#E8EAED] mt-1">{{ analysis.legalRisk.practicalReality }}</p>
          </div>
        </div>

        <!-- Emotional Impact -->
        <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border-l-4" :style="{ borderColor: getRiskColor(analysis.emotionalImpact.level) }">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg flex items-center justify-center" :style="{ backgroundColor: getRiskColor(analysis.emotionalImpact.level) + '20' }">
              <span class="text-2xl">😤</span>
            </div>
            <div>
              <h3 class="text-lg font-bold text-[#E8EAED]">Impacto Emocional</h3>
              <p class="text-sm font-medium" :style="{ color: getRiskColor(analysis.emotionalImpact.level) }">
                {{ translateLevel(analysis.emotionalImpact.level) }}
              </p>
            </div>
          </div>
          <p class="text-[#E8EAED] mb-4">{{ analysis.emotionalImpact.description }}</p>
          
          <div class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Tono Detectado:</p>
            <span class="inline-block px-3 py-1 bg-[#3C4043] rounded-full text-sm text-[#E8EAED]">
              {{ analysis.emotionalImpact.detectedTone }}
            </span>
          </div>

          <div v-if="analysis.emotionalImpact.triggerWords.length" class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Palabras Sensibles:</p>
            <div class="flex flex-wrap gap-2">
              <span 
                v-for="(word, idx) in analysis.emotionalImpact.triggerWords" 
                :key="idx"
                class="px-2 py-1 bg-[#F28B82]/20 text-[#F28B82] rounded text-sm"
              >
                {{ word }}
              </span>
            </div>
          </div>

          <div class="bg-[#3C4043] rounded-lg p-3">
            <p class="text-xs text-[#9AA0A6]">Contexto cultural:</p>
            <p class="text-sm text-[#E8EAED] mt-1">{{ analysis.emotionalImpact.culturalContext }}</p>
          </div>
        </div>

        <!-- Effectiveness -->
        <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border-l-4 border-[#8AB4F8]">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg bg-[#8AB4F8]/20 flex items-center justify-center">
              <span class="text-2xl">📊</span>
            </div>
            <div>
              <h3 class="text-lg font-bold text-[#E8EAED]">Efectividad</h3>
              <p class="text-sm font-medium text-[#8AB4F8]">
                {{ analysis.effectiveness.probabilityOfAction }}% de éxito
              </p>
            </div>
          </div>

          <div class="mb-4">
            <div class="w-full bg-[#3C4043] rounded-full h-3 overflow-hidden">
              <div 
                class="h-full rounded-full transition-all duration-500"
                :style="{ 
                  width: analysis.effectiveness.probabilityOfAction + '%',
                  backgroundColor: getEffectivenessColor(analysis.effectiveness.probabilityOfAction)
                }"
              ></div>
            </div>
          </div>

          <p class="text-[#E8EAED] mb-4">{{ analysis.effectiveness.reasoning }}</p>

          <div v-if="analysis.effectiveness.missingElements.length" class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Elementos Faltantes:</p>
            <ul class="space-y-1">
              <li v-for="(elem, idx) in analysis.effectiveness.missingElements" :key="idx" class="text-sm text-[#E8EAED] flex items-start gap-2">
                <span class="text-[#F28B82]">•</span>
                <span>{{ elem }}</span>
              </li>
            </ul>
          </div>

          <div v-if="analysis.effectiveness.strengthPoints.length" class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Puntos Fuertes:</p>
            <ul class="space-y-1">
              <li v-for="(point, idx) in analysis.effectiveness.strengthPoints" :key="idx" class="text-sm text-[#E8EAED] flex items-start gap-2">
                <span class="text-[#81C784]">✓</span>
                <span>{{ point }}</span>
              </li>
            </ul>
          </div>

          <div class="bg-[#3C4043] rounded-lg p-3">
            <p class="text-xs text-[#9AA0A6]">Recomendaciones locales:</p>
            <p class="text-sm text-[#E8EAED] mt-1">{{ analysis.effectiveness.localRecommendations }}</p>
          </div>
        </div>

        <!-- Backlash -->
        <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border-l-4" :style="{ borderColor: getRiskColor(analysis.backlash.level) }">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-lg flex items-center justify-center" :style="{ backgroundColor: getRiskColor(analysis.backlash.level) + '20' }">
              <span class="text-2xl">⚠️</span>
            </div>
            <div>
              <h3 class="text-lg font-bold text-[#E8EAED]">Riesgo de Represalias</h3>
              <p class="text-sm font-medium" :style="{ color: getRiskColor(analysis.backlash.level) }">
                {{ translateLevel(analysis.backlash.level) }}
              </p>
            </div>
          </div>

          <div v-if="analysis.backlash.potentialConsequences.length" class="mb-4">
            <p class="text-sm font-medium text-[#9AA0A6] mb-2">Consecuencias Potenciales:</p>
            <ul class="space-y-1">
              <li v-for="(cons, idx) in analysis.backlash.potentialConsequences" :key="idx" class="text-sm text-[#E8EAED] flex items-start gap-2">
                <span class="text-[#F28B82]">•</span>
                <span>{{ cons }}</span>
              </li>
            </ul>
          </div>

          <div class="bg-[#3C4043] rounded-lg p-3 mb-4">
            <p class="text-xs text-[#9AA0A6]">Consejo de mitigación:</p>
            <p class="text-sm text-[#E8EAED] mt-1">{{ analysis.backlash.mitigationAdvice }}</p>
          </div>

          <div class="bg-[#8AB4F8]/10 border border-[#8AB4F8] rounded-lg p-3">
            <p class="text-xs text-[#8AB4F8] font-medium">Protecciones locales:</p>
            <p class="text-sm text-[#E8EAED] mt-1">{{ analysis.backlash.localProtections }}</p>
          </div>
        </div>
      </div>

      <!-- Action Plan -->
      <div class="bg-[#292A2D] rounded-2xl shadow-xl p-6 border border-[#3C4043]">
        <h3 class="text-xl font-bold text-[#E8EAED] mb-6 flex items-center gap-2">
          <span>📋</span>
          Plan de Acción
        </h3>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div v-if="analysis.actionableRecommendations.strengthsToKeep.length">
            <div class="bg-[#1F3A1F] border border-[#81C784] rounded-lg p-4">
              <p class="text-[#81C784] font-semibold mb-3 flex items-center gap-2">
                <span>✅</span>
                Qué Mantener
              </p>
              <ul class="space-y-2">
                <li v-for="(strength, idx) in analysis.actionableRecommendations.strengthsToKeep" :key="idx" class="text-sm text-[#E8EAED]">
                  {{ strength }}
                </li>
              </ul>
            </div>
          </div>

          <div v-if="analysis.actionableRecommendations.canImprove.length">
            <div class="bg-[#3A2F1F] border border-[#F59E0B] rounded-lg p-4">
              <p class="text-[#F59E0B] font-semibold mb-3 flex items-center gap-2">
                <span>💡</span>
                Puedes Mejorar
              </p>
              <ul class="space-y-2">
                <li v-for="(improve, idx) in analysis.actionableRecommendations.canImprove" :key="idx" class="text-sm text-[#E8EAED]">
                  {{ improve }}
                </li>
              </ul>
            </div>
          </div>

          <div v-if="analysis.actionableRecommendations.mustImprove.length" class="md:col-span-2">
            <div class="bg-[#3C1F1F] border border-[#F28B82] rounded-lg p-4">
              <p class="text-[#F28B82] font-semibold mb-3 flex items-center gap-2">
                <span>🚨</span>
                DEBES Mejorar (Obligatorio)
              </p>
              <ul class="space-y-2">
                <li v-for="(must, idx) in analysis.actionableRecommendations.mustImprove" :key="idx" class="text-sm text-[#E8EAED]">
                  {{ must }}
                </li>
              </ul>
            </div>
          </div>
        </div>

        <div class="mt-6 bg-[#3C4043] rounded-lg p-4">
          <p class="text-sm font-medium text-[#9AA0A6] mb-2">Próximos Pasos:</p>
          <p class="text-[#E8EAED]">{{ analysis.overall.nextSteps }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { aiService } from '@/services/aiService'

const messageText = ref('')
const context = ref('workplace')
const country = ref('CO')
const isAnalyzing = ref(false)
const errorMessage = ref('')
const analysis = ref(null)

const handleAnalyze = async () => {
  if (messageText.value.length < 10) return

  isAnalyzing.value = true
  errorMessage.value = ''
  analysis.value = null

  try {
    const result = await aiService.analyzeConsequences({
      text: messageText.value,
      context: context.value,
      country: country.value
    })
    analysis.value = result
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Error al analizar el mensaje'
  } finally {
    isAnalyzing.value = false
  }
}

const getRiskColor = (level) => {
  const colors = {
    'Low': '#10b981',
    'Medium': '#f59e0b',
    'High': '#f97316',
    'Critical': '#ef4444'
  }
  return colors[level] || '#9AA0A6'
}

const getEffectivenessColor = (percentage) => {
  if (percentage >= 70) return '#10b981'
  if (percentage >= 40) return '#f59e0b'
  return '#ef4444'
}

const getSummaryCardClasses = () => {
  if (!analysis.value) return ''
  const status = analysis.value.actionableRecommendations.messageStatus
  
  if (status === 'Optimal') return 'bg-gradient-to-br from-green-600 to-green-700'
  if (status === 'NeedsImprovement') return 'bg-gradient-to-br from-yellow-600 to-orange-600'
  return 'bg-gradient-to-br from-red-600 to-red-700'
}

const getSummaryIcon = () => {
  if (!analysis.value) return '📊'
  const status = analysis.value.actionableRecommendations.messageStatus
  
  if (status === 'Optimal') return '✨'
  if (status === 'NeedsImprovement') return '⚠️'
  return '🚨'
}

const translateLevel = (level) => {
  const translations = {
    'Low': 'Bajo',
    'Medium': 'Medio',
    'High': 'Alto',
    'Critical': 'Crítico'
  }
  return translations[level] || level
}

const translateRecommendation = (rec) => {
  const translations = {
    'Send': 'Enviar',
    'SendWithCaution': 'Enviar con Precaución',
    'Revise': 'Revisar Antes de Enviar',
    'DoNotSend': 'NO ENVIAR'
  }
  return translations[rec] || rec
}
</script>
