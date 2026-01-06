import axios from "axios"
import { useAuthStore } from "../stores/Auth"

const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api",
    timeout: 10000,
    headers: {
        "Content-Type": "application/json"
    },
    withCredentials: true
})

// ==========================
// REQUEST INTERCEPTOR
// ==========================
api.interceptors.request.use(
    (config) => {
        console.log('🚀 Making request to:', config.baseURL + config.url)
        console.log('🚀 Full URL:', config.url)
        console.log('🚀 Method:', config.method)

        const token = localStorage.getItem("accessToken")
        if (token) {
            console.log('🔑 Adding token to request')
            config.headers.Authorization = `Bearer ${token}`
        } else {
            console.log('⚠️ No token found')
        }
        return config
    },
    (error) => {
        console.error('❌ Request setup error:', error)
        return Promise.reject(error)
    }
)

// ==========================
// RESPONSE INTERCEPTOR
// ==========================
api.interceptors.response.use(
    (response) => {
        console.log('✅ Response received:', response.status, response.config.url)
        return response
    },
    async (error) => {
        console.error('❌ Response error:', {
            status: error.response?.status,
            message: error.message,
            url: error.config?.url,
            data: error.response?.data
        })

        const originalRequest = error.config

        // Prevent infinite loop
        if (
            error.response?.status === 401 &&
            !originalRequest._retry &&
            !originalRequest.url.includes("/auth/refresh")
        ) {
            originalRequest._retry = true

            try {
                const authStore = useAuthStore()
                const newToken = await authStore.refreshToken()

                // Save token
                localStorage.setItem("accessToken", newToken)

                // Retry original request
                originalRequest.headers.Authorization = `Bearer ${newToken}`
                return api(originalRequest)
            } catch (refreshError) {
                console.error('❌ Token refresh failed:', refreshError)
                const authStore = useAuthStore()
                authStore.clearAuth()

                // Soft redirect (router preferred)
                window.location.replace("/login")
                return Promise.reject(refreshError)
            }
        }

        return Promise.reject(error)
    }
)

export default api