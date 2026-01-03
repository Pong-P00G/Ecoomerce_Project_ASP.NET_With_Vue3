import api from './api.js'

export const profileApi = {
    // Get user profile
    getProfile: () => api.get('/profile'),

    // Update profile
    updateProfile: (data) => api.put('/profile', data),

    // Change password
    changePassword: (data) => api.put('/profile/password', data),

    // Upload avatar
    uploadAvatar: (file) => {
        const formData = new FormData()
        formData.append('avatar', file)
        return api.post('/profile/avatar', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },

    // Addresses
    getAddresses: () => api.get('/profile/addresses'),
    addAddress: (data) => api.post('/profile/addresses', data),
    updateAddress: (id, data) => api.put(`/profile/addresses/${id}`, data),
    deleteAddress: (id) => api.delete(`/profile/addresses/${id}`),
    setDefaultAddress: (id) => api.put(`/profile/addresses/${id}/default`),

    // Payment methods
    getPaymentMethods: () => api.get('/profile/payment-methods'),
    addPaymentMethod: (data) => api.post('/profile/payment-methods', data),
    updatePaymentMethod: (id, data) => api.put(`/profile/payment-methods/${id}`, data),
    deletePaymentMethod: (id) => api.delete(`/profile/payment-methods/${id}`),
    setDefaultPaymentMethod: (id) => api.put(`/profile/payment-methods/${id}/default`),

    // Wishlist
    getWishlist: () => api.get('/profile/wishlist'),
    addToWishlist: (productId) => api.post('/profile/wishlist', { productId }),
    removeFromWishlist: (productId) => api.delete(`/profile/wishlist/${productId}`)
}

