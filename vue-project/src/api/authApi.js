import api from './api'

export const authAPI = {

    async register(userData) {
        // Map role string to roleId (1=admin, 2=customer)
        let roleId = 2; // Default to customer
        if (userData.roleId === 'admin' || userData.roleId === 1) {
            roleId = 1;
        }
        
        const response = await api.post('/Auth/register', {
            username: userData.username || '',
            email: userData.email || '',
            password: userData.password || '',
            roleId: roleId,
            firstName: userData.firstName || userData.firstname || '',
            lastName: userData.lastName || userData.lastname || ''
        });
        return response;
    },

    async login(credentials) {
        // Backend expects UsernameOrEmail field
        const usernameOrEmail = credentials.email || credentials.username || credentials.emailOrUsername || '';
        
        const response = await api.post('/Auth/login', {
            usernameOrEmail: usernameOrEmail,
            password: credentials.password || ''
        });

        // Store token if login successful
        if (response.data) {
            // Handle different response structures
            const token = response.data.accessToken || response.data.token;
            const refreshToken = response.data.refreshToken;
            const user = response.data.user;
            
            if (token) {
                localStorage.setItem('accessToken', token);
            }
            if (refreshToken) {
                localStorage.setItem('refreshToken', refreshToken);
            }
            if (user) {
                localStorage.setItem('user', JSON.stringify(user));
            }
        }

        return response;
    },

    // async logout() {
    //     localStorage.removeItem('accessToken');
    //     localStorage.removeItem('refreshToken');
    //     localStorage.removeItem('user');
    // },

    // async refreshToken(refreshToken) {
    //     return api.post('/Auth/refresh', { refreshToken });
    // },

    // async getCurrentUser() {
    //     return api.get('/Auth/me');
    // },
}