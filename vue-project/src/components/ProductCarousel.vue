<script setup>
import { ref, computed } from 'vue';
import { ShoppingBag, Heart, Star, ChevronLeft, ChevronRight } from 'lucide-vue-next';

const currentIndex = ref(0);

const products = [
    { id: 1, name: 'Wireless Headphones', category: 'Electronics', price: 129.99, rating: 4.5, reviews: 234, image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop', badge: 'Best Seller' },
    { id: 2, name: 'Smart Watch Pro', category: 'Electronics', price: 299.99, rating: 4.8, reviews: 456, image: 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=400&fit=crop', badge: 'New' },
    { id: 3, name: 'Designer Backpack', category: 'Fashion', price: 79.99, rating: 4.3, reviews: 189, image: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400&h=400&fit=crop', badge: '' },
    { id: 4, name: 'Running Shoes', category: 'Sports', price: 149.99, rating: 4.6, reviews: 567, image: 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=400&fit=crop', badge: 'Hot' },
    { id: 5, name: 'Coffee Maker', category: 'Home', price: 89.99, rating: 4.4, reviews: 312, image: 'https://images.unsplash.com/photo-1517668808822-9ebb02f2a0e6?w=400&h=400&fit=crop', badge: 'Sale' },
    { id: 6, name: 'Yoga Mat Premium', category: 'Sports', price: 45.99, rating: 4.7, reviews: 423, image: 'https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=400&h=400&fit=crop', badge: '' },
    { id: 7, name: 'Leather Wallet', category: 'Fashion', price: 59.99, rating: 4.5, reviews: 278, image: 'https://images.unsplash.com/photo-1627123424574-724758594e93?w=400&h=400&fit=crop', badge: 'New' },
    { id: 8, name: 'Desk Lamp Modern', category: 'Home', price: 69.99, rating: 4.2, reviews: 156, image: 'https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=400&h=400&fit=crop', badge: '' },
];

const itemsPerView = 4;
const maxIndex = Math.max(0, products.length - itemsPerView);

const canGoNext = computed(() => currentIndex.value < maxIndex);
const canGoPrev = computed(() => currentIndex.value > 0);

const translateX = computed(() => {
    return -(currentIndex.value * (100 / itemsPerView));
});

const nextSlide = () => {
    if (canGoNext.value) {
        currentIndex.value++;
    }
};

const prevSlide = () => {
    if (canGoPrev.value) {
        currentIndex.value--;
    }
};

const goToSlide = (index) => {
    currentIndex.value = index;
};

const addToWishlist = (productId) => {
    console.log('Added to wishlist:', productId);
};

const addToCart = (productId) => {
    console.log('Added to cart:', productId);
};
</script>

<template>
    <div class="relative px-12">
        <!-- Carousel Container -->
        <div class="overflow-hidden">
            <div 
                class="flex transition-transform duration-500 ease-out"
                :style="{ transform: `translateX(${translateX}%)` }"
            >
                <div 
                    v-for="product in products" 
                    :key="product.id"
                    class="w-1/4 shrink-0 px-3"
                >
                    <div class="group bg-white rounded-2xl overflow-hidden shadow-sm hover:shadow-2xl transition-all duration-300 border border-gray-100 h-full">
                        <!-- Product Image -->
                        <div class="relative overflow-hidden aspect-square bg-gray-100">
                            <img :src="product.image" :alt="product.name"
                                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                            <!-- Badge -->
                            <div v-if="product.badge"
                                class="absolute top-3 left-3 px-3 py-1 bg-linear-to-r from-violet-600 to-purple-600 text-white text-xs font-bold rounded-full shadow-lg">
                                {{ product.badge }}
                            </div>
                            <!-- Quick Actions -->
                            <div class="absolute top-3 right-3 flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                <button @click="addToWishlist(product.id)"
                                    class="w-10 h-10 bg-white rounded-full flex items-center justify-center shadow-lg hover:bg-red-50 transition-colors"
                                    aria-label="Add to wishlist">
                                    <Heart class="w-5 h-5 text-gray-700 hover:text-red-500 transition-colors" />
                                </button>
                            </div>
                            <!-- Add to Cart Overlay -->
                            <div class="absolute inset-x-0 bottom-0 p-4 bg-linear-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                <button @click="addToCart(product.id)"
                                    class="w-full py-2 bg-white text-gray-900 font-semibold rounded-lg hover:bg-gray-100 transition-colors flex items-center justify-center gap-2">
                                    <ShoppingBag class="w-4 h-4" />
                                    Add to Cart
                                </button>
                            </div>
                        </div>
                        <!-- Product Info -->
                        <div class="p-4 space-y-3">
                            <div class="flex items-start justify-between gap-2">
                                <h3 class="font-bold text-gray-900 line-clamp-1 group-hover:text-violet-600 transition-colors">
                                    {{ product.name }}
                                </h3>
                            </div>
                            <div class="flex items-center gap-1">
                                <Star v-for="i in 5" :key="i" class="w-4 h-4"
                                    :class="i <= Math.floor(product.rating) ? 'text-yellow-400 fill-yellow-400' : 'text-gray-300'" />
                                <span class="text-sm text-gray-600 ml-1">({{ product.reviews }})</span>
                            </div>
                            <div class="flex items-center justify-between">
                                <span class="text-2xl font-black text-gray-900">${{ product.price }}</span>
                                <span class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded-full">{{ product.category }}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Navigation Arrows -->
        <button 
            @click="prevSlide"
            :disabled="!canGoPrev"
            class="absolute left-0 top-1/2 -translate-y-1/2 w-12 h-12 bg-white rounded-full shadow-xl flex items-center justify-center transition-all duration-300 z-10 group disabled:opacity-30 disabled:cursor-not-allowed hover:scale-110"
            :class="canGoPrev ? 'hover:bg-violet-600' : ''"
            aria-label="Previous products"
        >
            <ChevronLeft class="w-6 h-6 transition-colors" :class="canGoPrev ? 'text-gray-900 group-hover:text-white' : 'text-gray-400'" />
        </button>
        <button 
            @click="nextSlide"
            :disabled="!canGoNext"
            class="absolute right-0 top-1/2 -translate-y-1/2 w-12 h-12 bg-white rounded-full shadow-xl flex items-center justify-center transition-all duration-300 z-10 group disabled:opacity-30 disabled:cursor-not-allowed hover:scale-110"
            :class="canGoNext ? 'hover:bg-violet-600' : ''"
            aria-label="Next products"
        >
            <ChevronRight class="w-6 h-6 transition-colors" :class="canGoNext ? 'text-gray-900 group-hover:text-white' : 'text-gray-400'" />
        </button>

        <!-- Progress Indicators -->
        <div class="flex items-center justify-center gap-2 mt-8">
            <button
                v-for="i in (maxIndex + 1)" 
                :key="i"
                @click="goToSlide(i - 1)"
                class="transition-all duration-300 rounded-full cursor-pointer hover:bg-violet-400"
                :class="currentIndex === (i - 1) ? 'w-8 h-2 bg-violet-600' : 'w-2 h-2 bg-gray-300'"
                :aria-label="`Go to slide ${i}`"
            ></button>
        </div>
    </div>
</template>

<style scoped>
/* Smooth carousel transitions */
</style>