<script setup>
import { ref, computed } from 'vue';
import { RouterLink } from 'vue-router';
import {
    Search,
    SlidersHorizontal,
    Grid3x3,
    List,
    Star,
    Heart,
    ShoppingCart,
    ChevronDown,
    X,
    Filter
} from 'lucide-vue-next';
import { useWishlistStore } from '../stores/Wishlist.js';
import productsApi from '../api/productsApi';
import { onMounted } from 'vue';

// View Mode
const viewMode = ref('grid'); // 'grid' or 'list'

// Products Data (Demo)
const allProducts = ref([
    { id: 1, name: 'Wireless Headphones Pro', category: 'Electronics', price: 129.99, originalPrice: 159.99, rating: 4.5, reviews: 234, image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&h=500&fit=crop', badge: 'Best Seller', inStock: true },
    { id: 2, name: 'Smart Watch Ultra', category: 'Electronics', price: 299.99, originalPrice: null, rating: 4.8, reviews: 456, image: 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=500&h=500&fit=crop', badge: 'New', inStock: true },
    { id: 3, name: 'Designer Backpack Premium', category: 'Fashion', price: 79.99, originalPrice: 99.99, rating: 4.3, reviews: 189, image: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=500&h=500&fit=crop', badge: 'Sale', inStock: true },
    { id: 4, name: 'Running Shoes Elite', category: 'Sports', price: 149.99, originalPrice: null, rating: 4.6, reviews: 567, image: 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500&h=500&fit=crop', badge: 'Hot', inStock: true },
    { id: 5, name: 'Coffee Maker Deluxe', category: 'Home', price: 89.99, originalPrice: 119.99, rating: 4.4, reviews: 312, image: 'https://images.unsplash.com/photo-1517668808822-9ebb02f2a0e6?w=500&h=500&fit=crop', badge: 'Sale', inStock: true },
    { id: 6, name: 'Yoga Mat Premium Plus', category: 'Sports', price: 45.99, originalPrice: null, rating: 4.7, reviews: 423, image: 'https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=500&h=500&fit=crop', badge: '', inStock: true },
    { id: 7, name: 'Leather Wallet Classic', category: 'Fashion', price: 59.99, originalPrice: null, rating: 4.5, reviews: 278, image: 'https://images.unsplash.com/photo-1627123424574-724758594e93?w=500&h=500&fit=crop', badge: 'New', inStock: false },
    { id: 8, name: 'Desk Lamp Modern LED', category: 'Home', price: 69.99, originalPrice: 89.99, rating: 4.2, reviews: 156, image: 'https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=500&h=500&fit=crop', badge: '', inStock: true },
    { id: 9, name: 'Bluetooth Speaker XL', category: 'Electronics', price: 179.99, originalPrice: null, rating: 4.9, reviews: 789, image: 'https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=500&h=500&fit=crop', badge: 'Best Seller', inStock: true },
    { id: 10, name: 'Sunglasses Aviator', category: 'Fashion', price: 129.99, originalPrice: 159.99, rating: 4.6, reviews: 345, image: 'https://images.unsplash.com/photo-1511499767150-a48a237f0083?w=500&h=500&fit=crop', badge: 'Sale', inStock: true },
    { id: 11, name: 'Dumbbell Set 20kg', category: 'Sports', price: 199.99, originalPrice: null, rating: 4.8, reviews: 234, image: 'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=500&h=500&fit=crop', badge: '', inStock: true },
    { id: 12, name: 'Kitchen Blender Pro', category: 'Home', price: 119.99, originalPrice: 149.99, rating: 4.5, reviews: 412, image: 'https://images.unsplash.com/photo-1585515320310-259814833379?w=500&h=500&fit=crop', badge: 'Hot', inStock: true },
]);

// Filter State
const searchQuery = ref('');
const selectedCategory = ref('All');
const priceRange = ref([0, 500]);
const sortBy = ref('featured');
const showFilters = ref(false);
const addToWishlist = useWishlistStore();

const categories = ['All', 'Electronics', 'Fashion', 'Sports', 'Home'];
const sortOptions = [
    { value: 'featured', label: 'Featured' },
    { value: 'price-low', label: 'Price: Low to High' },
    { value: 'price-high', label: 'Price: High to Low' },
    { value: 'rating', label: 'Highest Rated' },
    { value: 'newest', label: 'Newest First' },
];

// Filtered and Sorted Products
const filteredProducts = computed(() => {
    let filtered = allProducts.value.filter(product => {
        const matchesSearch = product.name.toLowerCase().includes(searchQuery.value.toLowerCase());
        const matchesCategory = selectedCategory.value === 'All' || product.category === selectedCategory.value;
        const matchesPrice = product.price >= priceRange.value[0] && product.price <= priceRange.value[1];
        return matchesSearch && matchesCategory && matchesPrice;
    });

    // Sort
    switch (sortBy.value) {
        case 'price-low':
            filtered.sort((a, b) => a.price - b.price);
            break;
        case 'price-high':
            filtered.sort((a, b) => b.price - a.price);
            break;
        case 'rating':
            filtered.sort((a, b) => b.rating - a.rating);
            break;
        case 'newest':
            filtered.sort((a, b) => (b.badge === 'New' ? 1 : 0) - (a.badge === 'New' ? 1 : 0));
            break;
    }

    return filtered;
});

// Actions
const handleAddToWishlist = (productId, event) => {
    event.preventDefault();
    event.stopPropagation();
    addToWishlist(productId);
    console.log('Added to wishlist:', productId);
};

const addToCart = (productId, event) => {
    event.preventDefault();
    event.stopPropagation();
    console.log('Added to cart:', productId);
};

const clearFilters = () => {
    searchQuery.value = '';
    selectedCategory.value = 'All';
    priceRange.value = [0, 500];
    sortBy.value = 'featured';
};

const loading = ref(false);
const error = ref(null);

const fetchProducts = async () => {
    try {
        loading.value = true;
        error.value = null;
        const response = await productsApi.getProducts();
        allProducts.value = response.data.map(p => ({
            ...p,
            rating: p.rating || 4.0 + Math.random(), // Add random rating if missing
            reviews: p.reviews || Math.floor(Math.random() * 500),
            image: p.image || 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&h=500&fit=crop',
            inStock: p.stock > 0
        }));
    } catch (err) {
        error.value = 'Failed to load products';
        console.error(err);
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    fetchProducts();
});
</script>

<template>
    <div class="min-h-screen bg-linear-to-b from-gray-50 to-white">
        <!-- Page Header -->
        <section class="bg-linear-to-r from-slate-900 via-slate-800 to-slate-900 text-white py-16">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center space-y-4">
                    <h1 class="text-5xl md:text-6xl font-black">
                        Our <span
                            class="bg-linear-to-r from-violet-400 to-purple-400 bg-clip-text text-transparent">Products</span>
                    </h1>
                    <p class="text-gray-300 text-lg max-w-2xl mx-auto">
                        Discover our curated collection of premium products for every lifestyle
                    </p>
                </div>
            </div>
        </section>
        <!-- Main Content -->
        <section class="py-12">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <!-- Search and Controls Bar -->
                <div class="mb-8 space-y-4">
                    <div class="flex flex-col lg:flex-row gap-4">
                        <!-- Search -->
                        <div class="flex-1 relative">
                            <Search class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                            <input v-model="searchQuery" type="text" placeholder="Search for products..."
                                class="w-full pl-12 pr-4 py-3 border-2 text-gray-900 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors" />
                        </div>
                        <!-- Sort Dropdown -->
                        <div class="relative">
                            <select v-model="sortBy"
                                class="w-full lg:w-64 px-4 py-3 text-gray-900 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors appearance-none bg-white pr-10">
                                <option v-for="option in sortOptions" :key="option.value" :value="option.value">
                                    {{ option.label }}
                                </option>
                            </select>
                            <ChevronDown
                                class="absolute right-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 pointer-events-none" />
                        </div>
                        <!-- View Mode Toggle -->
                        <div class="flex gap-2 bg-gray-100 rounded-xl p-1">
                            <button @click="viewMode = 'grid'" :class="viewMode === 'grid' ? 'bg-white shadow-sm' : ''"
                                class="p-2.5 rounded-lg transition-all">
                                <Grid3x3 class="w-5 h-5 text-gray-700" />
                            </button>
                            <button @click="viewMode = 'list'" :class="viewMode === 'list' ? 'bg-white shadow-sm' : ''"
                                class="p-2.5 rounded-lg transition-all">
                                <List class="w-5 h-5 text-gray-700" />
                            </button>
                        </div>
                        <!-- Filter Toggle Button -->
                        <button @click="showFilters = !showFilters"
                            class="px-6 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 transition-colors flex items-center gap-2 justify-center">
                            <Filter class="w-5 h-5" />
                            Filters
                        </button>
                    </div>
                    <!-- Active Filters -->
                    <div v-if="searchQuery || selectedCategory !== 'All' || priceRange[0] > 0 || priceRange[1] < 500"
                        class="flex flex-wrap items-center gap-3">
                        <span class="text-sm text-gray-600 font-medium">Active Filters:</span>
                        <button v-if="searchQuery" @click="searchQuery = ''"
                            class="px-3 py-1.5 bg-violet-100 text-violet-700 rounded-lg text-sm font-medium flex items-center gap-2 hover:bg-violet-200 transition-colors">
                            Search: "{{ searchQuery }}"
                            <X class="w-4 h-4" />
                        </button>
                        <button v-if="selectedCategory !== 'All'" @click="selectedCategory = 'All'"
                            class="px-3 py-1.5 bg-violet-100 text-violet-700 rounded-lg text-sm font-medium flex items-center gap-2 hover:bg-violet-200 transition-colors">
                            {{ selectedCategory }}
                            <X class="w-4 h-4" />
                        </button>
                        <button @click="clearFilters"
                            class="px-3 py-1.5 bg-red-100 text-red-700 rounded-lg text-sm font-medium hover:bg-red-200 transition-colors">
                            Clear All
                        </button>
                    </div>
                    <!-- Filter Panel -->
                    <transition name="slide-fade">
                        <div v-if="showFilters"
                            class="p-6 bg-white rounded-xl border-2 border-gray-200 shadow-lg space-y-6">
                            <!-- Category Filter -->
                            <div>
                                <label class="block text-sm font-bold text-gray-900 mb-3">Category</label>
                                <div class="flex flex-wrap gap-2">
                                    <button v-for="category in categories" :key="category"
                                        @click="selectedCategory = category"
                                        class="px-4 py-2 rounded-lg font-medium transition-all duration-300" :class="selectedCategory === category
                                            ? 'bg-linear-to-r from-violet-600 to-purple-600 text-white shadow-lg'
                                            : 'bg-gray-100 text-gray-700 hover:bg-gray-200'">
                                        {{ category }}
                                    </button>
                                </div>
                            </div>
                            <!-- Price Range -->
                            <div>
                                <label class="block text-sm font-bold text-gray-900 mb-3">
                                    Price Range: ${{ priceRange[0] }} - ${{ priceRange[1] }}
                                </label>
                                <div class="space-y-3">
                                    <input v-model.number="priceRange[0]" type="range" min="0" max="500"
                                        class="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-violet-600" />
                                    <input v-model.number="priceRange[1]" type="range" min="0" max="500"
                                        class="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-violet-600" />
                                </div>
                            </div>
                            <!-- Results Count -->
                            <div class="pt-4 border-t border-gray-200">
                                <p class="text-sm text-gray-600 font-medium">
                                    Showing <span class="text-violet-600 font-bold">{{ filteredProducts.length }}</span>
                                    of <span class="font-bold">{{ allProducts.length }}</span> products
                                </p>
                            </div>
                        </div>
                    </transition>
                </div>
                <!-- Products Grid/List -->
                <div v-if="loading" class="flex flex-col items-center justify-center py-20 gap-4">
                    <Loader2 class="w-12 h-12 text-violet-600 animate-spin" />
                    <p class="text-gray-500 font-medium">Loading amazing products...</p>
                </div>

                <div v-else-if="error" class="bg-red-50 border-2 border-red-100 p-8 rounded-2xl text-center">
                    <AlertCircle class="w-12 h-12 text-red-500 mx-auto mb-4" />
                    <h3 class="text-lg font-bold text-red-900 mb-2">{{ error }}</h3>
                    <button @click="fetchProducts"
                        class="px-6 py-2 bg-red-600 text-white rounded-xl font-semibold hover:bg-red-700 transition-colors">
                        Try Again
                    </button>
                </div>

                <div v-else-if="filteredProducts.length > 0">
                    <!-- Grid View -->
                    <div v-if="viewMode === 'grid'"
                        class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                        <div v-for="product in filteredProducts" :key="product.id"
                            class="group bg-white rounded-2xl overflow-hidden shadow-sm hover:shadow-2xl transition-all duration-300 border border-gray-100">
                            <!-- Product Image -->
                            <RouterLink :to="`/product/${product.id}`" class="block">
                                <div class="relative overflow-hidden aspect-square bg-gray-100">
                                    <img :src="product.image" :alt="product.name"
                                        class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                                    <!-- Badge -->
                                    <div v-if="product.badge"
                                        class="absolute top-3 left-3 px-3 py-1 bg-linear-to-r from-violet-600 to-purple-600 text-white text-xs font-bold rounded-full shadow-lg">
                                        {{ product.badge }}
                                    </div>
                                    <!-- Stock Status -->
                                    <div v-if="!product.inStock"
                                        class="absolute top-3 right-3 px-3 py-1 bg-red-500 text-white text-xs font-bold rounded-full shadow-lg">
                                        Out of Stock
                                    </div>
                                    <!-- Quick Actions -->
                                    <div
                                        class="absolute top-3 right-3 flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                        <button @click="handleAddToWishlist(product)"
                                            class="w-10 h-10 bg-white rounded-full flex items-center justify-center shadow-lg hover:bg-red-50 transition-colors"
                                            aria-label="Add to wishlist">
                                            <Heart class="w-5 h-5 text-gray-700 hover:text-red-500 transition-colors" />
                                        </button>
                                    </div>
                                    <!-- Add to Cart Overlay -->
                                    <div
                                        class="absolute inset-x-0 bottom-0 p-4 bg-linear-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                        <button @click="(e) => addToCart(product.id, e)" :disabled="!product.inStock"
                                            class="w-full py-2 bg-white text-gray-900 font-semibold rounded-lg hover:bg-gray-100 transition-colors flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed">
                                            <ShoppingCart class="w-4 h-4" />
                                            {{ product.inStock ? 'Add to Cart' : 'Out of Stock' }}
                                        </button>
                                    </div>
                                </div>
                            </RouterLink>
                            <!-- Product Info -->
                            <div class="p-4 space-y-3">
                                <RouterLink :to="`/product/${product.id}`" class="block">
                                    <h3
                                        class="font-bold text-gray-900 line-clamp-2 group-hover:text-violet-600 transition-colors">
                                        {{ product.name }}
                                    </h3>
                                </RouterLink>
                                <div class="flex items-center gap-1">
                                    <Star v-for="i in 5" :key="i" class="w-4 h-4"
                                        :class="i <= Math.floor(product.rating) ? 'text-yellow-400 fill-yellow-400' : 'text-gray-300'" />
                                    <span class="text-sm text-gray-600 ml-1">({{ product.reviews }})</span>
                                </div>
                                <div class="flex items-center justify-between">
                                    <div>
                                        <span class="text-2xl font-black text-gray-900">${{ product.price }}</span>
                                        <span v-if="product.originalPrice"
                                            class="text-sm text-gray-400 line-through ml-2">${{ product.originalPrice
                                            }}</span>
                                    </div>
                                    <span class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded-full">{{
                                        product.category }}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- List View -->
                    <div v-else class="space-y-4">
                        <div v-for="product in filteredProducts" :key="product.id"
                            class="group bg-white rounded-2xl overflow-hidden shadow-sm hover:shadow-xl transition-all duration-300 border border-gray-100">
                            <div class="flex flex-col md:flex-row">
                                <!-- Product Image -->
                                <RouterLink :to="`/product/${product.id}`" class="block">
                                    <div class="relative w-full md:w-64 h-64 md:h-auto overflow-hidden bg-gray-100">
                                        <img :src="product.image" :alt="product.name"
                                            class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                                        <div v-if="product.badge"
                                            class="absolute top-3 left-3 px-3 py-1 bg-linear-to-r from-violet-600 to-purple-600 text-white text-xs font-bold rounded-full shadow-lg">
                                            {{ product.badge }}
                                        </div>
                                    </div>
                                </RouterLink>
                                <!-- Product Info -->
                                <div class="flex-1 p-6 flex flex-col justify-between">
                                    <div>
                                        <div class="flex items-start justify-between mb-3">
                                            <RouterLink :to="`/product/${product.id}`">
                                                <h3
                                                    class="text-2xl font-bold text-gray-900 group-hover:text-violet-600 transition-colors">
                                                    {{ product.name }}
                                                </h3>
                                            </RouterLink>
                                            <button @click="(e) => handleAddToWishlist(product.id, e)"
                                                class="w-10 h-10 rounded-full flex items-center justify-center hover:bg-gray-100 transition-colors">
                                                <Heart class="w-5 h-5 text-gray-700" />
                                            </button>
                                        </div>
                                        <div class="flex items-center gap-2 mb-4">
                                            <div class="flex items-center gap-1">
                                                <Star v-for="i in 5" :key="i" class="w-4 h-4"
                                                    :class="i <= Math.floor(product.rating) ? 'text-yellow-400 fill-yellow-400' : 'text-gray-300'" />
                                            </div>
                                            <span class="text-sm text-gray-600">({{ product.reviews }} reviews)</span>
                                            <span
                                                class="text-sm text-gray-500 bg-gray-100 px-3 py-1 rounded-full ml-auto">{{
                                                product.category }}</span>
                                        </div>
                                        <p class="text-gray-600 mb-4">
                                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Perfect for
                                            everyday use with premium quality materials.
                                        </p>
                                    </div>
                                    <div class="flex items-center justify-between">
                                        <div>
                                            <span class="text-3xl font-black text-gray-900">${{ product.price }}</span>
                                            <span v-if="product.originalPrice"
                                                class="text-sm text-gray-400 line-through ml-2">${{
                                                product.originalPrice }}</span>
                                            <span v-if="!product.inStock"
                                                class="text-sm text-red-500 font-semibold ml-4">Out of Stock</span>
                                        </div>
                                        <button @click="(e) => addToCart(product.id, e)" :disabled="!product.inStock"
                                            class="px-6 py-3 bg-linear-to-r from-violet-600 to-purple-600 text-white font-semibold rounded-xl hover:shadow-lg hover:shadow-violet-600/50 transition-all duration-300 flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed">
                                            <ShoppingCart class="w-5 h-5" />
                                            {{ product.inStock ? 'Add to Cart' : 'Unavailable' }}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- No Results -->
                <div v-else class="text-center py-20">
                    <div class="inline-flex items-center justify-center w-20 h-20 bg-gray-100 rounded-full mb-6">
                        <Search class="w-10 h-10 text-gray-400" />
                    </div>
                    <h3 class="text-2xl font-bold text-gray-900 mb-2">No products found</h3>
                    <p class="text-gray-600 mb-6">Try adjusting your filters or search terms</p>
                    <button @click="clearFilters"
                        class="px-6 py-3 bg-violet-600 text-white font-semibold rounded-xl hover:bg-violet-700 transition-colors">
                        Clear All Filters
                    </button>
                </div>
            </div>
        </section>
    </div>
</template>

<style scoped>
.slide-fade-enter-active {
    transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
    transition: all 0.2s ease-in;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
    transform: translateY(-10px);
    opacity: 0;
}
</style>