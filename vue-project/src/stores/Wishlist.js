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

    // Load wishlist from localStorage
    function loadWishlist() {
        try {
            const saved = localStorage.getItem('wishlist');
            if (saved) {
                wishlistItems.value = JSON.parse(saved);
            }
        } catch (err) {
            console.error('Error loading wishlist from localStorage:', err);
            wishlistItems.value = [];
        }
    }

    // Save wishlist to localStorage
    function saveWishlist() {
        try {
            localStorage.setItem('wishlist', JSON.stringify(wishlistItems.value));
        } catch (err) {
            console.error('Error saving wishlist to localStorage:', err);
        }
    }

    // Add item to wishlist
    function addToWishlist(product) {
        // Check if already in wishlist
        if (isInWishlist.value(product.id)) {
            console.log('Product already in wishlist');
            return { success: false, message: 'Already in wishlist' };
        }

        // Add to local state
        wishlistItems.value.push(product);
        saveWishlist();
        
        return { success: true };
    }

    // Remove item from wishlist
    function removeFromWishlist(productId) {
        const index = wishlistItems.value.findIndex(item => item.id === productId);
        if (index > -1) {
            wishlistItems.value.splice(index, 1);
            saveWishlist();
            return { success: true };
        }
        return { success: false, message: 'Item not found in wishlist' };
    }

    // Toggle item in wishlist (add if not present, remove if present)
    function toggleWishlist(product) {
        if (isInWishlist.value(product.id)) {
            return removeFromWishlist(product.id);
        } else {
            return addToWishlist(product);
        }
    }

    // Clear entire wishlist
    function clearWishlist() {
        wishlistItems.value = [];
        saveWishlist();
        return { success: true };
    }

    // Move all items to cart (mock implementation)
    function moveAllToCart() {
        const inStockItems = wishlistItems.value.filter(item => item.inStock);
        if (inStockItems.length === 0) {
            return { success: false, message: 'No items in stock' };
        }
        
        // Clear wishlist after successful move
        clearWishlist();
        return { success: true, count: inStockItems.length };
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
    function initWishlist() {
        loadWishlist();
    }

    /**
     * Clear error message
     */
    function clearError() {
        error.value = null;
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
        loadWishlist,
        saveWishlist,
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
