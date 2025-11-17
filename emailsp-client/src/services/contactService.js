import apiClient from './api'

export const contactService = {
  /**
   * Obtener lista de contactos con paginación y búsqueda
   * @param {Object} params - Parámetros de consulta
   * @param {string} params.q - Término de búsqueda
   * @param {number} params.page - Número de página
   * @param {number} params.pageSize - Tamaño de página
   * @returns {Promise<{items: Array, total: number, page: number, pageSize: number}>}
   */
  async getContacts({ q = '', page = 1, pageSize = 10 } = {}) {
    const params = new URLSearchParams()
    if (q) params.append('q', q)
    params.append('page', page)
    params.append('pageSize', pageSize)
    
    const response = await apiClient.get(`/api/Contacts?${params}`)
    return response.data
  },

  /**
   * Obtener contacto por ID
   * @param {number} id 
   * @returns {Promise<Object>}
   */
  async getContactById(id) {
    const response = await apiClient.get(`/api/Contacts/${id}`)
    return response.data
  },

  /**
   * Crear nuevo contacto
   * @param {Object} contact 
   * @param {string} contact.name
   * @param {string} contact.email
   * @param {string} contact.phoneNumber
   * @returns {Promise<Object>}
   */
  async createContact(contact) {
    const response = await apiClient.post('/api/Contacts', contact)
    return response.data
  },

  /**
   * Actualizar contacto existente
   * @param {number} id 
   * @param {Object} contact 
   * @returns {Promise<string>}
   */
  async updateContact(id, contact) {
    const response = await apiClient.put(`/api/Contacts/${id}`, contact)
    return response.data
  },

  /**
   * Eliminar contacto
   * @param {number} id 
   * @returns {Promise<void>}
   */
  async deleteContact(id) {
    await apiClient.delete(`/api/Contacts/${id}`)
  },

  /**
   * Marcar contacto como favorito
   * @param {number} id 
   * @returns {Promise<string>}
   */
  async markAsFavorite(id) {
    const response = await apiClient.post(`/api/Contacts/${id}/favorite`)
    return response.data
  },

  /**
   * Desmarcar contacto como favorito
   * @param {number} id 
   * @returns {Promise<string>}
   */
  async removeFavorite(id) {
    const response = await apiClient.delete(`/api/Contacts/${id}/favorite`)
    return response.data
  },

  /**
   * Bloquear contacto
   * @param {number} id 
   * @returns {Promise<string>}
   */
  async blockContact(id) {
    const response = await apiClient.post(`/api/Contacts/${id}/block`)
    return response.data
  },

  /**
   * Desbloquear contacto
   * @param {number} id 
   * @returns {Promise<string>}
   */
  async unblockContact(id) {
    const response = await apiClient.delete(`/api/Contacts/${id}/block`)
    return response.data
  }
}
