import apiClient from './api'

export const emailService = {
  /**
   * Enviar email con adjuntos
   * @param {Object} emailData
   * @param {Array<string>} emailData.to - Lista de destinatarios
   * @param {string} emailData.subject - Asunto
   * @param {string} emailData.body - Cuerpo HTML
   * @param {FileList|Array<File>} emailData.attachments - Archivos adjuntos
   * @returns {Promise<string>}
   */
  async sendEmail({ to, subject, body, attachments = [] }) {
    const formData = new FormData()
    
    // Agregar destinatarios
    to.forEach(email => formData.append('To', email))
    
    formData.append('Subject', subject)
    formData.append('Body', body)
    
    // Agregar adjuntos
    for (const file of attachments) {
      formData.append('Attachments', file)
    }
    
    const response = await apiClient.post('/Email/Send', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    
    return response.data
  },

  /**
   * Enviar email simple sin adjuntos
   * @param {Object} emailData
   * @param {string} emailData.to - Destinatario
   * @param {string} emailData.subject - Asunto
   * @param {string} emailData.body - Cuerpo HTML
   * @returns {Promise<string>}
   */
  async sendSimpleEmail({ to, subject, body }) {
    const response = await apiClient.post('/Email/SendSimple', {
      to,
      subject,
      body
    })
    
    return response.data
  }
}
