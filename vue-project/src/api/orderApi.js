import api from './api.js'

export const orderApi = {
    // Create a new order from cart
    createOrder: (orderData) => api.post('/order', orderData),
    
    // Get all orders for the current user
    getOrders: () => api.get('/order'),
    
    // Get a specific order by ID
    getOrderById: (orderId) => api.get(`/order/${orderId}`),
    
    // Update order status (admin only)
    updateOrderStatus: (orderId, status) => api.put(`/order/${orderId}/status`, { status })
}
