<template>
    <div class="max-w-3xl mx-auto p-5 text-center">
        <div v-if="loading" class="text-lg text-gray-600">Loading product...</div>
        <div v-else-if="product">
            <img :src="product.image" :alt="product.name" class="max-w-full h-auto mb-5 rounded-lg shadow-md" />
            <h1 class="text-4xl font-bold text-gray-800 mb-4">{{ product.name }}</h1>
            <p class="text-lg text-gray-600 my-2.5">{{ product.description }}</p>
            <p class="text-2xl font-bold text-gray-800 my-5">Price: ${{ product.price }}</p>
            <div class="mt-5 flex justify-center gap-4">
                <button 
                    @click="addToCart(product)" 
                    class="px-5 py-2.5 bg-green-500 text-white border-none rounded cursor-pointer text-base font-semibold hover:bg-green-600 transition"
                >
                    Add to Cart
                </button>
                <button 
                    @click="addToWishlist(product)" 
                    class="px-5 py-2.5 bg-orange-500 text-white border-none rounded cursor-pointer text-base font-semibold hover:bg-orange-600 transition"
                >
                    Add to Wishlist
                </button>
            </div>
        </div>
        <div v-else class="text-lg text-gray-600">Product not found.</div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useWishlistStore } from '../stores/Wishlist'

const route = useRoute()
const router = useRouter()
const wishlist = useWishlistStore()
const product = ref(null)
const loading = ref(true)

const fetchProduct = async (id) => {
    const mockProducts = [
        { id: 1, name: 'Sample Product 1', price: 29.99, description: 'A great product.', image: 'https://via.placeholder.com/300' },
        { id: 2, name: 'Sample Product 2', price: 49.99, description: 'Another awesome item.', image: 'https://via.placeholder.com/300' }
    ]
    product.value = mockProducts.find(p => p.id == id) || null
    loading.value = false
}

const addToCart = (prod) => {
    alert(`${prod.name} added to cart!`)
}

const addToWishlist = (prod) => {
    wishlist.addItem(prod)
    router.push('/wishlist')
}

onMounted(() => {
    const productId = route.params.id
    if (productId) {
        fetchProduct(productId)
    } else {
        loading.value = false
    }
})
</script>