import { defineStore } from 'pinia'
import { authAPI } from '../api/authApi'


export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user') || 'null'),
    token: localStorage.getItem('accessToken')
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    userRole: (state) => state.user?.roleName || state.user?.role || null
  },

  actions: {
    async login(payload) {
      const res = await authAPI.login(payload)
      // Handle different response structures
      this.token = res.data.accessToken || res.data.token
      this.user = res.data.user
      
      if (this.token) {
        localStorage.setItem('accessToken', this.token)
      }
      if (res.data.user) {
        localStorage.setItem('user', JSON.stringify(res.data.user))
      }
      return res
    },

    async register(payload) {
      const res = await authAPI.register(payload)
      return res
    },

    logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
    },

    // Initialize from localStorage on app start
    initializeAuth() {
      const token = localStorage.getItem('accessToken')
      const user = localStorage.getItem('user')
      if (token) {
        this.token = token
      }
      if (user) {
        try {
          this.user = JSON.parse(user)
        } catch (e) {
          this.user = null
        }
      }
    }
  }
})