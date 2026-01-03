import api from './api'

export const authAPI = {
    async register(userData) {
        return api.post('/auth/register', userData);
    },

    // Login
    async login(credentials) {
        return api.post('/auth/login', credentials);
    },

    // Logout
    async logout() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
    },

    // Refresh token
    async refreshToken(refreshToken) {
        return api.post('/auth/refresh', { refreshToken });
    },

    // Get current user
    async getCurrentUser() {
        return api.get('/auth/me');
    },
}