import apiClient from './api'

export const aiService = {
  /**
   * Reformular texto de informal a formal usando IA
   * @param {string} text - Texto a reformular (máximo 2000 caracteres)
   * @returns {Promise<{formalText: string}>}
   */
  async refactorText(text) {
    const response = await apiClient.post('/api/AI/refactor', {
      text
    })
    
    return response.data
  },

  /**
   * Analizar consecuencias de un mensaje
   * @param {Object} params
   * @param {string} params.text - Texto a analizar (10-5000 caracteres)
   * @param {string} params.context - Contexto: 'workplace', 'academic', 'personal'
   * @param {string} params.country - País: 'CO', 'PE', 'EC', 'VE', 'BO', 'PA'
   * @returns {Promise<Object>}
   */
  async analyzeConsequences({ text, context = 'workplace', country = 'CO' }) {
    const response = await apiClient.post('/api/AI/analyze-consequences', {
      text,
      context,
      country
    })
    
    return response.data
  }
}
