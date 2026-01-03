<script setup>
import { ref, onMounted, computed } from 'vue'
import { RouterLink } from 'vue-router'
import { profileApi } from '../api/profileApi'
import { cartApi } from '../api/cartApi'
import {
    Heart,
    ShoppingCart,
    Trash2,
    Star,
    AlertCircle,
    Loader2,
    X,
    Eye,
    Package,
    CheckCircle,
    TrendingUp
} from 'lucide-vue-next'

// State
const wishlistItems = ref([])
const loading = ref(true)
const error = ref(null)
const success = ref(null)
const removingItems = ref(new Set())
const addingToCart = ref(new Set())

// Computed
const totalValue = computed(() => {
    return wishlistItems.value.reduce((sum, item) => sum + (item.price || item.product?.price || 0), 0)
})

const inStockCount = computed(() => {
    return wishlistItems.value.filter(item => (item.inStock !== false && (item.stock || item.product?.stock || 0) > 0)).length
})

// Fetch wishlist
const fetchWishlist = async () => {
    try {
        loading.value = true
        error.value = null
        const response = await profileApi.getWishlist()
        wishlistItems.value = response.data.data || response.data || []
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to load wishlist'
        console.error('Error fetching wishlist:', err)
        // Fallback sample data
        wishlistItems.value = []
    } finally {
        loading.value = false
    }
}

// Remove item from wishlist
const removeFromWishlist = async (productId) => {
    removingItems.value.add(productId)
    try {
        await profileApi.removeFromWishlist(productId)
        wishlistItems.value = wishlistItems.value.filter(item => 
            (item.id !== productId && item.productId !== productId)
        )
        success.value = 'Item removed from wishlist'
        setTimeout(() => success.value = null, 3000)
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to remove item'
    } finally {
        removingItems.value.delete(productId)
    }
}

// Add to cart
const addToCart = async (product) => {
    const productId = product.id || product.productId
    addingToCart.value.add(productId)
    try {
        await cartApi.addToCart(productId, 1)
        success.value = 'Item added to cart!'
        setTimeout(() => success.value = null, 3000)
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to add to cart'
    } finally {
        addingToCart.value.delete(productId)
    }
}

// Move all to cart
const moveAllToCart = async () => {
    if (!wishlistItems.value.length) return
    
    loading.value = true
    error.value = null
    
    try {
        const inStockItems = wishlistItems.value.filter(item => 
            (item.inStock !== false && (item.stock || item.product?.stock || 0) > 0)
        )
        
        if (inStockItems.length === 0) {
            error.value = 'No items in stock to add to cart'
            return
        }
        
        const promises = inStockItems.map(item => 
            cartApi.addToCart(item.id || item.productId, 1)
        )
        
        await Promise.all(promises)
        success.value = `${inStockItems.length} item(s) added to cart!`
        setTimeout(() => success.value = null, 3000)
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to add items to cart'
    } finally {
        loading.value = false
    }
}

// Clear entire wishlist
const clearWishlist = async () => {
    if (!confirm('Are you sure you want to clear your entire wishlist?')) return
    
    loading.value = true
    error.value = null
    
    try {
        // Remove all items one by one
        const promises = wishlistItems.value.map(item => 
            profileApi.removeFromWishlist(item.id || item.productId)
        )
        await Promise.all(promises)
        wishlistItems.value = []
        success.value = 'Wishlist cleared successfully!'
        setTimeout(() => success.value = null, 3000)
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to clear wishlist'
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    fetchWishlist()
})
</script>

<template>
    <div class="min-h-screen bg-gray-50 py-8">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <!-- Header -->
            <div class="mb-8">
                <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-6">
                    <div>
                        <h1 class="text-3xl font-bold text-gray-900 flex items-center gap-3 mb-2">
                            <Heart class="w-8 h-8 text-pink-600" />
                            My Wishlist
                        </h1>
                        <p class="text-gray-500">
                            {{ wishlistItems.length }} items • ${{ totalValue.toFixed(2) }} total value
                        </p>
                    </div>
                    
                    <div v-if="wishlistItems.length > 0 && !loading" class="flex flex-wrap gap-3">
                        <button 
                            @click="moveAllToCart"
                            :disabled="loading || inStockCount === 0"
                            class="px-6 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center gap-2">
                            <ShoppingCart class="w-4 h-4" />
                            Add All to Cart
                        </button>
                        <button 
                            @click="clearWishlist"
                            :disabled="loading"
                            class="px-6 py-3 bg-white border-2 border-gray-200 text-gray-700 rounded-xl font-semibold hover:border-rose-300 hover:text-rose-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center gap-2">
                            <Trash2 class="w-4 h-4" />
                            Clear Wishlist
                        </button>
                    </div>
                </div>
            </div>

            <!-- Messages -->
            <Transition name="slide-fade">
                <div v-if="error"
                    class="mb-6 bg-rose-50 border border-rose-200 rounded-2xl p-4 flex items-center gap-3">
                    <AlertCircle class="w-5 h-5 text-rose-600 shrink-0" />
                    <div class="flex-1">
                        <p class="text-sm font-semibold text-rose-900">Error</p>
                        <p class="text-xs text-rose-700 mt-0.5">{{ error }}</p>
                    </div>
                    <button @click="error = null" class="text-rose-600 hover:text-rose-800">
                        <X class="w-4 h-4" />
                    </button>
                </div>
            </Transition>

            <Transition name="slide-fade">
                <div v-if="success"
                    class="mb-6 bg-emerald-50 border border-emerald-200 rounded-2xl p-4 flex items-center gap-3">
                    <CheckCircle class="w-5 h-5 text-emerald-600 shrink-0" />
                    <span class="flex-1 font-medium text-emerald-900">{{ success }}</span>
                    <button @click="success = null" class="text-emerald-600 hover:text-emerald-800">
                        <X class="w-4 h-4" />
                    </button>
                </div>
            </Transition>

            <!-- Stats Cards -->
            <div v-if="wishlistItems.length > 0 && !loading" class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-6">
                    <div class="flex items-center justify-between mb-4">
                        <div class="w-12 h-12 bg-blue-50 rounded-2xl flex items-center justify-center">
                            <Package class="w-6 h-6 text-blue-600" />
                        </div>
                    </div>
                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Total Items</p>
                    <p class="text-3xl font-bold text-gray-900">{{ wishlistItems.length }}</p>
                </div>

                <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-6">
                    <div class="flex items-center justify-between mb-4">
                        <div class="w-12 h-12 bg-emerald-50 rounded-2xl flex items-center justify-center">
                            <CheckCircle class="w-6 h-6 text-emerald-600" />
                        </div>
                    </div>
                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">In Stock</p>
                    <p class="text-3xl font-bold text-emerald-600">{{ inStockCount }}</p>
                </div>

                <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-6">
                    <div class="flex items-center justify-between mb-4">
                        <div class="w-12 h-12 bg-purple-50 rounded-2xl flex items-center justify-center">
                            <TrendingUp class="w-6 h-6 text-purple-600" />
                        </div>
                    </div>
                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Total Value</p>
                    <p class="text-3xl font-bold text-gray-900">${{ totalValue.toFixed(2) }}</p>
                </div>
            </div>

            <!-- Loading State -->
            <div v-if="loading" class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-12">
                <div class="flex flex-col items-center justify-center gap-4">
                    <Loader2 class="w-8 h-8 animate-spin text-blue-600" />
                    <p class="text-gray-500 font-medium">Loading your wishlist...</p>
                </div>
            </div>

            <!-- Empty Wishlist -->
            <div v-else-if="wishlistItems.length === 0" class="bg-white rounded-[32px] border border-gray-100 shadow-sm text-center py-20">
                <div class="inline-flex items-center justify-center w-24 h-24 bg-pink-50 rounded-full mb-6">
                    <Heart class="w-12 h-12 text-pink-400" />
                </div>
                <h3 class="text-2xl font-bold text-gray-900 mb-2">Your Wishlist is Empty</h3>
                <p class="text-gray-500 mb-8 max-w-md mx-auto">
                    Start adding items you love to build your perfect collection
                </p>
                <RouterLink 
                    to="/product"
                    class="inline-flex items-center gap-2 px-8 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 transition-colors">
                    <Eye class="w-5 h-5" />
                    Browse Products
                </RouterLink>
            </div>

            <!-- Wishlist Items -->
            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                <div 
                    v-for="item in wishlistItems" 
                    :key="item.id || item.productId"
                    class="group bg-white rounded-[32px] border border-gray-100 shadow-sm overflow-hidden hover:shadow-lg transition-all duration-300">
                    
                    <!-- Image Container -->
                    <RouterLink :to="`/product/${item.id || item.productId}`" class="block relative">
                        <div class="relative overflow-hidden aspect-square bg-gray-100">
                            <img 
                                v-if="item.image || item.product?.image" 
                                :src="item.image || item.product?.image" 
                                :alt="item.name || item.product?.name"
                                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" 
                            />
                            <div v-else class="w-full h-full flex items-center justify-center">
                                <Package class="w-16 h-16 text-gray-300" />
                            </div>
                            
                            <!-- Stock Overlay -->
                            <div 
                                v-if="!item.inStock || (item.stock || item.product?.stock || 0) === 0"
                                class="absolute inset-0 bg-white/90 backdrop-blur-sm flex items-center justify-center">
                                <div class="text-center">
                                    <div class="w-12 h-12 mx-auto mb-2 bg-rose-100 rounded-full flex items-center justify-center">
                                        <X class="w-6 h-6 text-rose-600" />
                                    </div>
                                    <p class="text-sm font-bold text-gray-900">Out of Stock</p>
                                </div>
                            </div>

                            <!-- Remove Button -->
                            <button 
                                @click.prevent="removeFromWishlist(item.id || item.productId)"
                                :disabled="removingItems.has(item.id || item.productId)"
                                class="absolute top-4 right-4 w-10 h-10 bg-white/90 backdrop-blur-sm border border-gray-200 rounded-xl flex items-center justify-center hover:bg-rose-50 hover:border-rose-300 transition-all duration-300 disabled:opacity-50 shadow-sm"
                                aria-label="Remove from wishlist">
                                <Loader2 v-if="removingItems.has(item.id || item.productId)" class="w-4 h-4 text-rose-600 animate-spin" />
                                <Trash2 v-else class="w-4 h-4 text-gray-400 group-hover:text-rose-600" />
                            </button>
                        </div>
                    </RouterLink>

                    <!-- Content -->
                    <div class="p-5 space-y-3">
                        <!-- Category Tag -->
                        <span v-if="item.category || item.product?.category" class="inline-block px-2 py-1 text-[10px] font-bold text-blue-600 bg-blue-50 rounded-lg uppercase tracking-wider">
                            {{ item.category || item.product?.category }}
                        </span>

                        <!-- Title -->
                        <RouterLink :to="`/product/${item.id || item.productId}`">
                            <h3 class="font-bold text-gray-900 line-clamp-2 group-hover:text-blue-600 transition-colors leading-tight mb-2">
                                {{ item.name || item.product?.name || 'Product' }}
                            </h3>
                        </RouterLink>

                        <!-- Rating -->
                        <div v-if="item.rating || item.product?.rating" class="flex items-center gap-2">
                            <div class="flex items-center gap-0.5">
                                <Star 
                                    v-for="i in 5" 
                                    :key="i" 
                                    class="w-3.5 h-3.5"
                                    :class="i <= Math.floor(item.rating || item.product?.rating || 0) ? 'text-yellow-400 fill-yellow-400' : 'text-gray-200'" 
                                />
                            </div>
                            <span class="text-xs text-gray-500 font-medium">
                                {{ (item.rating || item.product?.rating || 0).toFixed(1) }}
                            </span>
                        </div>

                        <!-- Price -->
                        <div class="flex items-baseline gap-2 pt-1">
                            <span class="text-2xl font-bold text-gray-900">
                                ${{ ((item.price || item.product?.price || 0)).toLocaleString() }}
                            </span>
                            <span v-if="item.originalPrice || item.product?.originalPrice" class="text-sm text-gray-400 line-through">
                                ${{ (item.originalPrice || item.product?.originalPrice || 0).toLocaleString() }}
                            </span>
                        </div>

                        <!-- Add to Cart Button -->
                        <button 
                            @click="addToCart(item)"
                            :disabled="!item.inStock && (item.stock || item.product?.stock || 0) === 0 || addingToCart.has(item.id || item.productId)"
                            class="w-full py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2">
                            <Loader2 v-if="addingToCart.has(item.id || item.productId)" class="w-4 h-4 animate-spin" />
                            <ShoppingCart v-else class="w-4 h-4" />
                            {{ addingToCart.has(item.id || item.productId) ? 'Adding...' : ((item.inStock !== false && (item.stock || item.product?.stock || 0) > 0) ? 'Add to Cart' : 'Out of Stock') }}
                        </button>
                    </div>
                </div>
            </div>

            <!-- Bottom Actions -->
            <div v-if="wishlistItems.length > 0 && !loading" class="mt-8 flex flex-col sm:flex-row gap-4 justify-center">
                <RouterLink 
                    to="/product"
                    class="px-8 py-3 bg-white border-2 border-gray-200 text-gray-700 rounded-xl font-semibold hover:border-blue-500 hover:text-blue-600 transition-colors text-center">
                    Continue Shopping
                </RouterLink>
                <button 
                    v-if="inStockCount > 0"
                    @click="moveAllToCart"
                    :disabled="loading"
                    class="px-8 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2">
                    <ShoppingCart class="w-5 h-5" />
                    Add {{ inStockCount }} Item{{ inStockCount !== 1 ? 's' : '' }} to Cart
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.slide-fade-enter-active {
    transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
    transition: all 0.2s ease-in;
}

.slide-fade-enter-from {
    transform: translateY(-10px);
    opacity: 0;
}

.slide-fade-leave-to {
    transform: translateY(-10px);
    opacity: 0;
}
</style>
