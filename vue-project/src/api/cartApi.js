import api from './api.js'

export const cartApi = {
    // Get cart items
    getCart: () => api.get('/cart'),
    
    // Add item to cart
    addToCart: (productId, quantity = 1) => api.post('/cart', { productId, quantity }),
    
    // Update cart item quantity
    updateCartItem: (itemId, quantity) => api.put(`/cart/${itemId}`, { quantity }),
    
    // Remove item from cart
    removeFromCart: (itemId) => api.delete(`/cart/${itemId}`),
    
    // Clear cart
    clearCart: () => api.delete('/cart/clear'),
    
    // Apply promo code
    applyPromoCode: (code) => api.post('/cart/promo', { code })
}

