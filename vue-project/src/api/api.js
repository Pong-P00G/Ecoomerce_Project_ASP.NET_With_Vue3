import axios from "axios"
import { useAuthStore } from "../stores/Auth"

const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || "https://localhost:7074/api",
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
        const token = localStorage.getItem("accessToken")
        if (token) {
            config.headers.Authorization = `Bearer ${token}`
        }
        return config
    },
    (error) => Promise.reject(error)
)

// ==========================
// RESPONSE INTERCEPTOR
// ==========================
api.interceptors.response.use(
    (response) => response,
    async (error) => {
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