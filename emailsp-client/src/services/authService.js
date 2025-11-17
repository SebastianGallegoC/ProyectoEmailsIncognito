import apiClient from './api'

export const authService = {
  /**
   * Login con credenciales
   * @param {string} username 
   * @param {string} password 
   * @returns {Promise<{token: string, expiration: string}>}
   */
  async login(username, password) {
    const response = await apiClient.post('/api/Auth/login', {
      username,
      password
    })
    
    // Guardar token en localStorage
    if (response.data.token) {
      localStorage.setItem('emailsP_token', response.data.token)
    }
    
    return response.data
  },

  /**
   * Logout - elimina el token
   */
  logout() {
    localStorage.removeItem('emailsP_token')
  },

  /**
   * Verifica si el usuario está autenticado
   * @returns {boolean}
   */
  isAuthenticated() {
    return !!localStorage.getItem('emailsP_token')
  },

  /**
   * Obtiene el token actual
   * @returns {string|null}
   */
  getToken() {
    return localStorage.getItem('emailsP_token')
  }
}
