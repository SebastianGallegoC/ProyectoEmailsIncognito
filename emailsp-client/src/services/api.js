import axios from 'axios'

// Detecta automáticamente el host del navegador y usa puerto 7000
// En desarrollo: usa VITE_API_URL o localhost:7000
// En producción: usa el mismo host del navegador (VPS) con puerto 7000
const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (typeof window !== 'undefined' && window.location.hostname !== 'localhost'
    ? `${window.location.protocol}//${window.location.hostname}:7000`
    : 'http://localhost:7000')

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Interceptor para agregar el token JWT automáticamente
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('emailsP_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Interceptor para manejar errores globalmente
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token inválido o expirado
      localStorage.removeItem('emailsP_token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default apiClient
