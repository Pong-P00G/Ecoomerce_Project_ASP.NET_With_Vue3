<script setup>
import { ref, computed, onMounted } from 'vue'
import {
    Package,
    Search,
    Edit2,
    Loader2,
    AlertCircle,
    CheckCircle,
    TrendingDown,
    TrendingUp,
    RefreshCw,
    Filter,
    ArrowUpDown,
    X,
    Database
} from 'lucide-vue-next'
import productApi from '../../api/productsApi'
import api from '../../api/api'

// State
const products = ref([])
const loading = ref(false)
const error = ref(null)
const success = ref(null)
const searchTerm = ref('')
const selectedStockStatus = ref('all')
const sortBy = ref('name')
const sortOrder = ref('asc')

// Modal state
const showUpdateModal = ref(false)
const currentProduct = ref(null)
const newStockValue = ref(0)
const updating = ref(false)

// Stats computed properties
const stats = computed(() => {
    return {
        totalItems: products.value.length,
        lowStock: products.value.filter(p => p.stock > 0 && p.stock <= 10).length,
        outOfStock: products.value.filter(p => p.stock === 0).length,
        totalValue: products.value.reduce((sum, p) => sum + (p.stock * p.price), 0)
    }
})

// Filtered and sorted products
const filteredProducts = computed(() => {
    let filtered = products.value

    // Search filter
    if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase()
        filtered = filtered.filter(p =>
            p.name.toLowerCase().includes(term) ||
            (p.sku && p.sku.toLowerCase().includes(term))
        )
    }

    // Stock status filter
    if (selectedStockStatus.value === 'low') {
        filtered = filtered.filter(p => p.stock > 0 && p.stock <= 10)
    } else if (selectedStockStatus.value === 'out') {
        filtered = filtered.filter(p => p.stock === 0)
    } else if (selectedStockStatus.value === 'healthy') {
        filtered = filtered.filter(p => p.stock > 10)
    }

    // Sorting
    return [...filtered].sort((a, b) => {
        let aVal = a[sortBy.value === 'name' ? 'name' : sortBy.value]
        let bVal = b[sortBy.value === 'name' ? 'name' : sortBy.value]

        if (sortBy.value === 'stock' || sortBy.value === 'price') {
            return sortOrder.value === 'asc' ? aVal - bVal : bVal - aVal
        }

        aVal = (aVal || '').toString().toLowerCase()
        bVal = (bVal || '').toString().toLowerCase()

        if (sortOrder.value === 'asc') {
            return aVal > bVal ? 1 : -1
        } else {
            return aVal < bVal ? 1 : -1
        }
    })
})

// Methods
const fetchProducts = async () => {
    try {
        loading.value = true
        error.value = null
        const response = await productApi.getProducts()
        products.value = response.data || []
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to fetch products'
        console.error('Error fetching products:', err)
    } finally {
        loading.value = false
    }
}

const openUpdateModal = (product) => {
    currentProduct.value = product
    newStockValue.value = product.stock
    showUpdateModal.value = true
}

const handleUpdateStock = async () => {
    if (!currentProduct.value) return

    try {
        updating.value = true
        error.value = null

        await productApi.updateStock(currentProduct.value.id, newStockValue.value)

        // Update local state
        const index = products.value.findIndex(p => p.id === currentProduct.value.id)
        if (index !== -1) {
            products.value[index].stock = newStockValue.value
        }

        success.value = 'Stock updated successfully'
        showUpdateModal.value = false

        setTimeout(() => {
            success.value = null
        }, 3000)
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to update stock'
    } finally {
        updating.value = false
    }
}

const toggleSort = (field) => {
    if (sortBy.value === field) {
        sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
    } else {
        sortBy.value = field
        sortOrder.value = 'asc'
    }
}

const getStockBadgeClass = (stock) => {
    if (stock === 0) return 'bg-red-100 text-red-800'
    if (stock <= 10) return 'bg-yellow-100 text-yellow-800'
    return 'bg-green-100 text-green-800'
}

onMounted(fetchProducts)
</script>

<template>
    <div class="p-4 sm:p-6 lg:p-8 max-w-[1600px] mx-auto">
       <!-- Error Banner -->
       <div v-if="error" class="mb-6 bg-red-50 border border-red-200 rounded-2xl p-4 flex items-center gap-3">
            <AlertCircle class="w-5 h-5 text-red-600 shrink-0" />
            <div class="flex-1">
                <p class="text-sm font-semibold text-red-900">Error loading inventory</p>
                <p class="text-xs text-red-700 mt-0.5">{{ error }}</p>
            </div>
            <button @click="fetchProducts()" class="text-sm font-medium text-red-700 hover:text-red-900 underline">
                Retry
            </button>
        </div>
        <!-- Header Section -->
        <div class="mb-8 flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
                <h1 class="text-3xl font-bold text-gray-900 flex items-center gap-3">
                    <Database class="w-8 h-8 text-blue-600" />
                    Inventory & Stock
                </h1>
                <p class="text-gray-500 mt-1">Real-time monitoring and management of your product stock levels.</p>
            </div>
            <button @click="fetchProducts"
                class="flex items-center gap-2 px-6 py-3 bg-white border border-gray-100 rounded-2xl text-gray-700 hover:bg-gray-50 transition-all font-bold text-sm shadow-sm"
                :disabled="loading">
                <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': loading }" />
                Refresh Data
            </button>
        </div>

        <!-- Loading Skeleton -->
        <div v-if="loading && products.length === 0">
            <!-- Stats Overview Skeleton -->
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                <div v-for="i in 4" :key="i" class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm animate-pulse">
                    <div class="flex items-center justify-between mb-4">
                        <div class="p-3 bg-gray-200 rounded-2xl w-12 h-12"></div>
                    </div>
                    <div class="h-8 bg-gray-200 rounded w-20 mb-2"></div>
                    <div class="h-3 bg-gray-200 rounded w-32"></div>
                </div>
            </div>
            <!-- Filters Skeleton -->
            <div class="bg-white p-6 rounded-[32px] border border-gray-100 shadow-sm mb-8 animate-pulse">
                <div class="flex flex-col md:flex-row gap-4">
                    <div class="flex-1 h-12 bg-gray-200 rounded-2xl"></div>
                    <div class="h-12 bg-gray-200 rounded-2xl w-48"></div>
                </div>
            </div>
            <!-- Table Skeleton -->
            <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm overflow-hidden animate-pulse">
                <div class="p-8 space-y-4">
                    <div v-for="i in 5" :key="i" class="flex items-center gap-4 py-4 border-b border-gray-100">
                        <div class="w-12 h-12 rounded-2xl bg-gray-200"></div>
                        <div class="flex-1 space-y-2">
                            <div class="h-4 bg-gray-200 rounded w-48"></div>
                            <div class="h-3 bg-gray-200 rounded w-24"></div>
                        </div>
                        <div class="h-4 bg-gray-200 rounded w-24"></div>
                        <div class="h-4 bg-gray-200 rounded w-20"></div>
                        <div class="h-4 bg-gray-200 rounded w-32"></div>
                        <div class="h-4 bg-gray-200 rounded w-12"></div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Stats Overview -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <div class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
                <div class="flex items-center justify-between mb-4">
                    <div class="p-3 bg-blue-50 rounded-2xl">
                        <Package class="w-6 h-6 text-blue-600" />
                    </div>
                </div>
                <div class="text-3xl font-bold text-gray-900">{{ stats.totalItems }}</div>
                <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mt-1">Total Unique Items</div>
            </div>

            <div class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
                <div class="flex items-center justify-between mb-4">
                    <div class="p-3 bg-amber-50 rounded-2xl">
                        <AlertCircle class="w-6 h-6 text-amber-600" />
                    </div>
                </div>
                <div class="text-3xl font-bold text-gray-900">{{ stats.lowStock }}</div>
                <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mt-1">Low Stock Alert</div>
            </div>

            <div class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
                <div class="flex items-center justify-between mb-4">
                    <div class="p-3 bg-rose-50 rounded-2xl">
                        <TrendingDown class="w-6 h-6 text-rose-600" />
                    </div>
                </div>
                <div class="text-3xl font-bold text-gray-900">{{ stats.outOfStock }}</div>
                <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mt-1">Out of Stock</div>
            </div>

            <div class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
                <div class="flex items-center justify-between mb-4">
                    <div class="p-3 bg-emerald-50 rounded-2xl">
                        <TrendingUp class="w-6 h-6 text-emerald-600" />
                    </div>
                </div>
                <div class="text-3xl font-bold text-gray-900">${{ stats.totalValue.toLocaleString() }}</div>
                <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mt-1">Total Stock Value</div>
            </div>
        </div>

        <!-- Filters and Search -->
        <div
            class="bg-white text-gray-900 p-6 rounded-[32px] border border-gray-100 shadow-sm mb-8 flex flex-col md:flex-row gap-4 items-center">
            <div class="flex-1 w-full relative group">
                <Search
                    class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-blue-600 transition-colors" />
                <input v-model="searchTerm" type="text" placeholder="Search by product name or SKU..."
                    class="w-full pl-12 pr-4 py-3 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-blue-600/20 transition-all" />
            </div>
            <div class="flex gap-3 w-full md:w-auto">
                <div class="flex-1 md:flex-none flex items-center gap-2 px-4 py-1 bg-gray-50 rounded-2xl">
                    <Filter class="w-4 h-4 text-gray-400" />
                    <select v-model="selectedStockStatus"
                        class="bg-transparent border-none py-2 focus:ring-0 text-sm font-bold text-gray-700">
                        <option value="all">All Status</option>
                        <option value="healthy">In Stock (>10)</option>
                        <option value="low">Low Stock (1-10)</option>
                        <option value="out">Out of Stock (0)</option>
                    </select>
                </div>
            </div>
        </div>

        <!-- Alert Messages -->
        <Transition name="slide-fade">
            <div v-if="error"
                class="mb-8 p-4 bg-rose-50 border border-rose-200 rounded-2xl flex items-center gap-3 text-rose-800 font-medium shadow-sm">
                <AlertCircle class="w-5 h-5 shrink-0" />
                {{ error }}
            </div>
        </Transition>
        <Transition name="slide-fade">
            <div v-if="success"
                class="mb-8 p-4 bg-emerald-50 border border-emerald-200 rounded-2xl flex items-center gap-3 text-emerald-800 font-medium shadow-sm">
                <CheckCircle class="w-5 h-5 shrink-0" />
                {{ success }}
            </div>
        </Transition>

        <!-- Inventory Table -->
        <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm overflow-hidden">
            <div class="overflow-x-auto">
                <table class="w-full text-left border-collapse">
                    <thead>
                        <tr class="bg-gray-50/50 border-b border-gray-100">
                            <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">
                                <button @click="toggleSort('name')"
                                    class="flex items-center gap-1 hover:text-blue-600 transition-colors">
                                    Product Details
                                    <ArrowUpDown class="w-3 h-3" />
                                </button>
                            </th>
                            <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">SKU</th>
                            <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">
                                <button @click="toggleSort('price')"
                                    class="flex items-center gap-1 hover:text-blue-600 transition-colors">
                                    Price
                                    <ArrowUpDown class="w-3 h-3" />
                                </button>
                            </th>
                            <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">
                                <button @click="toggleSort('stock')"
                                    class="flex items-center gap-1 hover:text-blue-600 transition-colors">
                                    Stock Level
                                    <ArrowUpDown class="w-3 h-3" />
                                </button>
                            </th>
                            <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest text-right">
                                Actions
                            </th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-50">
                        <tr v-for="product in filteredProducts" :key="product.id"
                            class="hover:bg-gray-50/50 transition-colors group">
                            <td class="px-8 py-5">
                                <div class="flex items-center gap-4">
                                    <div
                                        class="w-12 h-12 rounded-2xl bg-gray-100 flex items-center justify-center group-hover:bg-white transition-colors">
                                        <Package class="w-6 h-6 text-gray-400" />
                                    </div>
                                    <div>
                                        <div
                                            class="font-bold text-gray-900 group-hover:text-blue-600 transition-colors">
                                            {{ product.name }}</div>
                                        <div
                                            class="text-[10px] font-bold text-gray-400 uppercase tracking-tight mt-0.5">
                                            {{ product.category }}</div>
                                    </div>
                                </div>
                            </td>
                            <td class="px-8 py-5 text-gray-500 font-mono text-xs font-bold uppercase tracking-wider">
                                {{ product.sku || 'N/A' }}
                            </td>
                            <td class="px-8 py-5 text-gray-900 font-bold">
                                ${{ product.price.toLocaleString() }}
                            </td>
                            <td class="px-8 py-5">
                                <div class="flex items-center gap-4">
                                    <div class="w-32 h-2 bg-gray-100 rounded-full overflow-hidden shrink-0">
                                        <div class="h-full transition-all duration-700 ease-out"
                                            :class="product.stock === 0 ? 'bg-rose-500' : product.stock <= 10 ? 'bg-amber-500' : 'bg-emerald-500'"
                                            :style="{ width: Math.min((product.stock / 100) * 100, 100) + '%' }"></div>
                                    </div>
                                    <span class="text-[10px] font-bold uppercase tracking-widest"
                                        :class="product.stock === 0 ? 'text-rose-600' : product.stock <= 10 ? 'text-amber-600' : 'text-emerald-600'">
                                        {{ product.stock }} units
                                    </span>
                                </div>
                            </td>
                            <td class="px-8 py-5 text-right">
                                <button @click="openUpdateModal(product)"
                                    class="p-2.5 text-gray-400 hover:text-blue-600 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-gray-100"
                                    title="Quick Update Stock">
                                    <Edit2 class="w-4 h-4" />
                                </button>
                            </td>
                        </tr>
                        <tr v-if="filteredProducts.length === 0 && !loading">
                            <td colspan="5" class="px-8 py-20 text-center">
                                <div
                                    class="w-20 h-20 bg-gray-50 rounded-full flex items-center justify-center mx-auto mb-4 text-gray-300">
                                    <Database class="w-10 h-10" />
                                </div>
                                <p class="text-gray-500 font-medium text-lg mb-2">No inventory items found</p>
                                <p class="text-sm text-gray-400 mb-4">Try adjusting your filters or search terms</p>
                                <button @click="searchTerm = ''; selectedStockStatus = 'all'"
                                    class="px-6 py-2 text-sm font-semibold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors">
                                    Clear all filters
                                </button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Loading State -->
            <div v-if="loading" class="p-20 flex flex-col items-center justify-center gap-4">
                <Loader2 class="w-10 h-10 text-blue-600 animate-spin" />
                <p class="text-gray-500 font-medium">Synchronizing inventory data...</p>
            </div>
        </div>

        <!-- Quick Update Modal -->
        <Teleport to="body">
            <Transition name="modal">
                <div v-if="showUpdateModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
                    <div class="fixed inset-0 bg-black/60 backdrop-blur-sm" @click="showUpdateModal = false"></div>
                    <div
                        class="relative bg-white rounded-[32px] shadow-2xl w-full max-w-md overflow-hidden transform transition-all">
                        <div class="px-8 py-6 border-b border-gray-50 flex items-center justify-between">
                            <h3 class="text-xl font-bold text-gray-900">Update Stock Level</h3>
                            <button @click="showUpdateModal = false"
                                class="p-2 hover:bg-gray-100 rounded-xl transition-colors text-gray-400">
                                <X class="w-5 h-5" />
                            </button>
                        </div>
                        <div class="p-8">
                            <div class="mb-8">
                                <div class="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
                                    <div
                                        class="w-12 h-12 rounded-xl bg-white flex items-center justify-center shadow-sm">
                                        <Package class="w-6 h-6 text-gray-400" />
                                    </div>
                                    <div>
                                        <p class="text-[10px] font-bold text-gray-400 uppercase tracking-wider">Product
                                        </p>
                                        <p class="text-gray-900 font-bold">{{ currentProduct?.name }}</p>
                                    </div>
                                </div>
                            </div>
                            <div class="mb-8">
                                <label
                                    class="block text-xs font-bold text-gray-400 uppercase tracking-widest mb-3">Adjust
                                    Quantity</label>
                                <div class="relative group">
                                    <Database
                                        class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-blue-600 transition-colors" />
                                    <input v-model.number="newStockValue" type="number" min="0"
                                        class="w-full pl-12 pr-4 py-4 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-blue-600/20 text-lg font-bold" />
                                </div>
                                <p class="mt-3 text-xs text-gray-500 font-medium flex items-center gap-2">
                                    <AlertCircle class="w-3.5 h-3.5" />
                                    Update total current stock count for this SKU.
                                </p>
                            </div>
                            <div class="flex gap-3">
                                <button @click="showUpdateModal = false"
                                    class="flex-1 py-4 bg-gray-50 text-gray-700 rounded-2xl font-bold hover:bg-gray-100 transition-colors">
                                    Cancel
                                </button>
                                <button @click="handleUpdateStock"
                                    class="flex-1 py-4 bg-gray-900 text-white rounded-2xl font-bold hover:bg-gray-800 transition-colors flex items-center justify-center gap-2 shadow-lg shadow-gray-200"
                                    :disabled="updating">
                                    <Loader2 v-if="updating" class="w-4 h-4 animate-spin" />
                                    Update Count
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </Transition>
        </Teleport>
    </div>
</template>

<style scoped>
/* Modal Transitions */
.modal-enter-active,
.modal-leave-active {
    transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
    opacity: 0;
}

.modal-enter-active>div:last-child,
.modal-leave-active>div:last-child {
    transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from>div:last-child,
.modal-leave-to>div:last-child {
    transform: scale(0.95);
    opacity: 0;
}

/* Slide Fade Transitions */
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
