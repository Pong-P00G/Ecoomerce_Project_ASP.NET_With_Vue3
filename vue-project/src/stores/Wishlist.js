import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useWishlistStore = defineStore('wishlist', () => {
    // State
    const wishlistItems = ref([]);
    const loading = ref(false);
    const error = ref(null);

    // Getters (Computed)
    const wishlistCount = computed(() => wishlistItems.value.length);

    const totalValue = computed(() => {
        return wishlistItems.value.reduce((sum, item) => sum + item.price, 0);
    });

    const inStockCount = computed(() => {
        return wishlistItems.value.filter(item => item.inStock).length;
    });

    const isInWishlist = computed(() => {
        return (productId) => {
            return wishlistItems.value.some(item => item.id === productId);
        };
    });

    // Actions


    // Fetch all wishlist items from backend
    async function fetchWishlist() {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch('/api/wishlist', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    // Add authorization if needed
                    // 'Authorization': `Bearer ${getAuthToken()}`
                }
            });
            if (!response.ok) {
                throw new Error('Failed to fetch wishlist');
            }
            const data = await response.json();
            wishlistItems.value = data.items || data;
            return { success: true, data: wishlistItems.value };
        } catch (err) {
            error.value = err.message;
            console.error('Error fetching wishlist:', err);
            return { success: false, error: err.message };
        } finally {
            loading.value = false;
        }
    }
    //  Add item to wishlist
    //  @param {Object} product - Product object or product ID
    async function addToWishlist(product) {
        // If product is just an ID, fetch the product details first
        const productId = typeof product === 'object' ? product.id : product;
        // Check if already in wishlist
        if (isInWishlist.value(productId)) {
            console.log('Product already in wishlist');
            return { success: false, message: 'Already in wishlist' };
        }
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch('/api/wishlist', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${getAuthToken()}`
                },
                body: JSON.stringify({
                    productId: productId
                })
            });
            if (!response.ok) {
                throw new Error('Failed to add to wishlist');
            }
            const data = await response.json();
            // Add to local state
            if (typeof product === 'object') {
                wishlistItems.value.push(product);
            } else {
                // If only ID was provided, fetch the full product data
                await fetchWishlist();
            }
            return { success: true, data };
        } catch (err) {
            error.value = err.message;
            console.error('Error adding to wishlist:', err);
            return { success: false, error: err.message };
        } finally {
            loading.value = false;
        }
    }


    //  emove item from wishlist
    //  @param {number|string} productId - Product ID to remove

    async function removeFromWishlist(productId) {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`/api/wishlist/${productId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${getAuthToken()}`
                }
            });
            if (!response.ok) {
                throw new Error('Failed to remove from wishlist');
            }
            // Remove from local state
            wishlistItems.value = wishlistItems.value.filter(
                item => item.id !== productId
            );
            return { success: true };
        } catch (err) {
            error.value = err.message;
            console.error('Error removing from wishlist:', err);
            return { success: false, error: err.message };
        } finally {
            loading.value = false;
        }
    }


    //  Toggle item in wishlist (add if not present, remove if present)
    //  @param {Object|number} product - Product object or ID

    async function toggleWishlist(product) {
        const productId = typeof product === 'object' ? product.id : product;
        if (isInWishlist.value(productId)) {
            return await removeFromWishlist(productId);
        } else {
            return await addToWishlist(product);
        }
    }

    //  Clear entire wishlist

    async function clearWishlist() {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch('/api/wishlist/clear', {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${getAuthToken()}`
                }
            });
            if (!response.ok) {
                throw new Error('Failed to clear wishlist');
            }
            wishlistItems.value = [];
            return { success: true };
        } catch (err) {
            error.value = err.message;
            console.error('Error clearing wishlist:', err);
            return { success: false, error: err.message };
        } finally {
            loading.value = false;
        }
    }

    //  Move all items to cart

    async function moveAllToCart() {
        loading.value = true;
        error.value = null;
        try {
            const inStockItems = wishlistItems.value.filter(item => item.inStock);
            if (inStockItems.length === 0) {
                return { success: false, message: 'No items in stock' };
            }
            // Add all items to cart
            const promises = inStockItems.map(item =>
                fetch('/api/cart', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        // 'Authorization': `Bearer ${getAuthToken()}`
                    },
                    body: JSON.stringify({
                        productId: item.id,
                        quantity: 1
                    })
                })
            );
            const responses = await Promise.all(promises);
            // Check if all requests were successful
            const allSuccess = responses.every(res => res.ok);
            if (!allSuccess) {
                throw new Error('Some items failed to add to cart');
            }
            // Clear wishlist after successful move
            await clearWishlist();
            return { success: true, count: inStockItems.length };
        } catch (err) {
            error.value = err.message;
            console.error('Error moving to cart:', err);
            return { success: false, error: err.message };
        } finally {
            loading.value = false;
        }
    }
    /**
     * Get wishlist item by product ID
     * @param {number|string} productId
     */
    function getWishlistItem(productId) {
        return wishlistItems.value.find(item => item.id === productId);
    }
    /**
     * Initialize wishlist (call on app mount)
     */
    async function initWishlist() {
        await fetchWishlist();
    }
    /**
     * Clear error message
     */
    function clearError() {
        error.value = null;
    }
    // Helper function to get auth token (implement based on your auth system)
    function getAuthToken() {
        return localStorage.getItem('authToken') || '';
    }
    return {
        // State
        wishlistItems,
        loading,
        error,
        // Getters
        wishlistCount,
        totalValue,
        inStockCount,
        isInWishlist,
        // Actions
        fetchWishlist,
        addToWishlist,
        removeFromWishlist,
        toggleWishlist,
        clearWishlist,
        moveAllToCart,
        getWishlistItem,
        initWishlist,
        clearError
    };
});