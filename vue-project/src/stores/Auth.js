import { defineStore } from 'pinia'
import { authAPI } from '../api/authApi'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    accessToken: localStorage.getItem('accessToken'),
    refreshToken: localStorage.getItem('refreshToken'),
    isAuthenticated: false
  }),

  getters: {
    currentUser: (state) => state.user,
    isAdmin: (state) => state.user?.role === 'ADMIN',
    isCustomer: (state) => state.user?.role === 'CUSTOMER'
  },

  actions: {
    async register(registerData) {
      try {
        const response = await authAPI.register(registerData)
        if (response.data.succeeded) {
          this.setAuth(response.data.data)
          return response.data
        }
      } catch (error) {
        throw new Error(error.response?.data?.message || 'Registration failed')
      }
    },

    async login(credentials) {
      try {
        const response = await authAPI.login(credentials)
        if (response.data.succeeded) {
          this.setAuth(response.data.data)
          return response.data
        }
      } catch (error) {
        throw new Error(error.response?.data?.message || 'Login failed')
      }
    },

    async logout() {
      try {
        await authAPI.logout()
      } catch (error) {
        console.error('Logout error:', error)
      } finally {
        this.clearAuth()
      }
    },

    async refreshToken() {
      try {
        const response = await authAPI.refreshToken(this.refreshToken)
        if (response.data.succeeded) {
          this.setAuth(response.data.data)
          return response.data.data.accessToken
        }
      } catch (error) {
        this.clearAuth()
        throw error
      }
    },

    async getCurrentUser() {
      try {
        const response = await authAPI.getCurrentUser()
        if (response.data.succeeded) {
          this.user = response.data.data
          this.isAuthenticated = true
        }
      } catch (error) {
        this.clearAuth()
      }
    },

    async forgotPassword(email) {
      try {
        const response = await authAPI.forgotPassword({ email })
        return response.data
      } catch (error) {
        throw new Error(error.response?.data?.message || 'Failed to send reset email')
      }
    },

    // async resetPassword(resetData) {
    //   try {
    //     const response = await authAPI.resetPassword(resetData)
    //     return response.data
    //   } catch (error) {
    //     throw new Error(error.response?.data?.message || 'Password reset failed')
    //   }
    // },

    setAuth(authData) {
      this.accessToken = authData.accessToken
      this.refreshToken = authData.refreshToken
      this.user = authData.user
      this.isAuthenticated = true

      localStorage.setItem('accessToken', authData.accessToken)
      localStorage.setItem('refreshToken', authData.refreshToken)
    },

    clearAuth() {
      this.user = null
      this.accessToken = null
      this.refreshToken = null
      this.isAuthenticated = false

      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
    },

    initializeAuth() {
      if (this.accessToken) {
        this.getCurrentUser()
      }
    }
  }
})