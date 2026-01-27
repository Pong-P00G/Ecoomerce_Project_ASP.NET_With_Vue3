import api from './api.js'

// Get or create a cart session ID
const getCartSessionId = () => {
    let sessionId = localStorage.getItem('cartSessionId')
    if (!sessionId) {
        sessionId = crypto.randomUUID()
        localStorage.setItem('cartSessionId', sessionId)
    }
    return sessionId
}

export const cartApi = {
    // Get cart items
    getCart: () => api.get('/cart', {
        headers: { 'X-Cart-Session': getCartSessionId() }
    }),
    
    // Add item to cart
    addToCart: (productId, quantity = 1) => api.post('/cart', 
        { productId, quantity, sessionId: getCartSessionId() },
        { headers: { 'X-Cart-Session': getCartSessionId() } }
    ),
    
    // Update cart item quantity
    updateCartItem: (itemId, quantity) => api.put(`/cart/${itemId}`, 
        { quantity },
        { headers: { 'X-Cart-Session': getCartSessionId() } }
    ),
    
    // Remove item from cart
    removeFromCart: (itemId) => api.delete(`/cart/${itemId}`, {
        headers: { 'X-Cart-Session': getCartSessionId() }
    }),
    
    // Clear cart
    clearCart: () => api.delete('/cart/clear', {
        headers: { 'X-Cart-Session': getCartSessionId() }
    }),
    
    // Apply promo code
    applyPromoCode: (code) => api.post('/cart/promo', { code }, {
        headers: { 'X-Cart-Session': getCartSessionId() }
    }),

    // Clear cart session (for logout or after checkout)
    clearSession: () => {
        localStorage.removeItem('cartSessionId')
    }
}

