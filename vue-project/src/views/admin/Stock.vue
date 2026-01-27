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
import { productAPI } from '../../api/productsApi'

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
        // Use getAllProducts instead of getProducts, and request larger page size
        const response = await productAPI.getAllProducts({ pageSize: 100 })
        
        products.value = (response.data.items || []).map(p => ({
            id: p.productId,
            name: p.productName,
            category: p.categories?.[0]?.categoryName || 'Uncategorized',
            sku: p.sku,
            price: p.basePrice,
            stock: p.stock
        }))
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

        await productAPI.updateStock(currentProduct.value.id, newStockValue.value)

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
    <div class="min-h-screen bg-slate-50/50 p-6 sm:p-8 lg:p-12">
    <div class="max-w-[1600px] mx-auto">
      <!-- Header Section -->
      <header class="mb-10 flex flex-col md:flex-row md:items-center md:justify-between gap-6">
        <div class="flex items-center gap-5">
          <div class="w-14 h-14 bg-gradient-to-br from-slate-800 to-slate-900 rounded-2xl flex items-center justify-center shadow-lg shadow-slate-200 rotate-3">
            <Database class="w-7 h-7 text-white" />
          </div>
          <div>
            <h1 class="text-2xl font-black text-slate-900 tracking-tight">Stock Inventory</h1>
            <p class="text-[10px] text-slate-400 mt-1 font-black uppercase tracking-[0.2em]">Real-time Warehouse Monitoring</p>
          </div>
        </div>
        <button @click="fetchProducts"
          class="flex items-center gap-2.5 px-6 py-3 bg-white border border-slate-200 rounded-xl text-slate-600 hover:bg-slate-50 transition-all font-black text-xs uppercase tracking-widest shadow-sm active:scale-95 disabled:opacity-50"
          :disabled="loading">
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': loading }" />
          Synchronize
        </button>
      </header>

      <div v-if="loading && products.length === 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
        <div v-for="i in 4" :key="i" class="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm animate-pulse">
          <div class="w-10 h-10 bg-slate-50 rounded-xl mb-4"></div>
          <div class="h-6 bg-slate-50 rounded-lg w-16 mb-2"></div>
          <div class="h-3 bg-slate-50 rounded-lg w-24"></div>
        </div>
      </div>
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
        <div v-for="(stat, idx) in [
          { label: 'Unique SKUs', value: stats.totalItems, icon: Package, color: 'blue' },
          { label: 'Low Levels', value: stats.lowStock, icon: AlertCircle, color: 'amber' },
          { label: 'Out of Stock', value: stats.outOfStock, icon: TrendingDown, color: 'rose' },
          { label: 'Inventory Value', value: '$' + stats.totalValue.toLocaleString(), icon: Database, color: 'emerald' }
        ]" :key="idx" 
          class="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm hover:shadow-md transition-all duration-300 group">
          <div class="flex items-center gap-4">
            <div :class="`w-12 h-12 rounded-2xl flex items-center justify-center bg-${stat.color}-50 text-${stat.color}-600 group-hover:scale-110 transition-transform duration-300` ">
              <component :is="stat.icon" class="w-6 h-6" />
            </div>
            <div>
              <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">{{ stat.label }}</p>
              <p class="text-2xl font-black text-slate-900 tracking-tight">{{ stat.value }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm mb-10 flex flex-col md:flex-row gap-4 items-center">
        <div class="flex-1 w-full relative group">
          <Search class="absolute left-5 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-300 group-focus-within:text-blue-600 transition-colors" />
          <input v-model="searchTerm" type="text" placeholder="Find by product name or SKU code..."
            class="w-full pl-12 pr-5 py-3.5 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 focus:ring-4 focus:ring-blue-50/50 transition-all font-medium text-slate-600 text-sm outline-none" />
        </div>
        <div class="flex gap-3 w-full md:w-auto">
          <div class="relative flex-1 md:flex-none">
            <select v-model="selectedStockStatus"
              class="appearance-none pl-10 pr-10 py-3.5 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all text-[11px] font-black uppercase tracking-widest text-slate-700 outline-none cursor-pointer w-full">
              <option value="all">Full Catalog</option>
              <option value="healthy">Healthy Stock</option>
              <option value="low">Low Inventory</option>
              <option value="out">Critical (0)</option>
            </select>
            <Filter class="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" />
          </div>
        </div>
      </div>

      <!-- Inventory Table -->
      <div class="bg-white rounded-[48px] border border-slate-100 shadow-sm overflow-hidden">
        <div v-if="loading && products.length > 0" class="p-8 text-center bg-slate-50/50">
          <div class="flex items-center justify-center gap-3">
            <Loader2 class="w-5 h-5 animate-spin text-blue-600" />
            <span class="text-xs font-black text-slate-400 uppercase tracking-widest">Refreshing Records...</span>
          </div>
        </div>
        <div>
          <!-- Mobile View (Cards) -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 p-4 lg:hidden">
            <div v-for="product in filteredProducts" :key="product.id" 
              class="bg-white rounded-2xl p-5 border border-slate-100 shadow-sm flex flex-col gap-4">
              
              <div class="flex items-center gap-4">
                <div class="w-14 h-14 rounded-2xl bg-slate-50 flex items-center justify-center shrink-0 border border-slate-100">
                   <Package class="w-7 h-7 text-slate-300" />
                </div>
                <div class="min-w-0">
                   <p class="text-sm font-bold text-slate-900 line-clamp-2 leading-tight">{{ product.name }}</p>
                   <div class="flex items-center gap-2 mt-1">
                     <span class="text-[10px] font-mono text-slate-400 bg-slate-100 px-1.5 py-0.5 rounded">{{ product.sku || 'N/A' }}</span>
                     <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider">{{ product.category }}</span>
                   </div>
                </div>
              </div>

              <div class="bg-slate-50 rounded-xl p-4 border border-slate-100/50">
                 <div class="flex justify-between items-end mb-2">
                    <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Current Level</p>
                    <p :class="`text-xl font-black ${product.stock === 0 ? 'text-rose-600' : product.stock <= 10 ? 'text-amber-600' : 'text-emerald-600'}`">
                      {{ product.stock }}
                    </p>
                 </div>
                 <div class="w-full h-2 bg-slate-200 rounded-full overflow-hidden">
                    <div class="h-full transition-all duration-1000"
                      :class="product.stock === 0 ? 'bg-rose-500' : product.stock <= 10 ? 'bg-amber-500' : 'bg-emerald-500'"
                      :style="{ width: Math.min((product.stock / 100) * 100, 100) + '%' }"></div>
                 </div>
              </div>

              <div class="flex items-center justify-between pt-2">
                 <div>
                    <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest">Unit Price</p>
                    <p class="text-sm font-black text-slate-900">${{ product.price.toLocaleString() }}</p>
                 </div>
                 <button @click="openUpdateModal(product)"
                    class="flex items-center gap-2 px-4 py-3 bg-slate-900 text-white rounded-xl font-bold text-xs shadow-lg shadow-slate-200 hover:bg-slate-800 transition-all active:scale-95">
                    <Edit2 class="w-4 h-4" />
                    Adjust
                 </button>
              </div>
            </div>
          </div>

          <div class="hidden lg:block overflow-x-auto px-4">
          <table class="w-full border-separate border-spacing-y-4">
            <thead>
              <tr>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">
                  <button @click="toggleSort('name')" class="flex items-center gap-2 hover:text-slate-900 transition-colors">
                    Product Info
                    <ArrowUpDown class="w-3 h-3" />
                  </button>
                </th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">SKU</th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">
                  <button @click="toggleSort('price')" class="flex items-center gap-2 hover:text-slate-900 transition-colors">
                    Unit Price
                    <ArrowUpDown class="w-3 h-3" />
                  </button>
                </th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">
                  <button @click="toggleSort('stock')" class="flex items-center gap-2 hover:text-slate-900 transition-colors">
                    Availability
                    <ArrowUpDown class="w-3 h-3" />
                  </button>
                </th>
                <th class="px-8 py-6 text-right text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Management</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="product in filteredProducts" :key="product.id"
                class="group hover:bg-slate-50 transition-all duration-300 rounded-[32px]">
                <td class="px-8 py-6 first:rounded-l-[32px]">
                  <div class="flex items-center gap-6">
                    <div class="w-16 h-16 rounded-[24px] bg-white shadow-sm border border-slate-100 flex items-center justify-center shrink-0 group-hover:scale-105 transition-transform duration-500">
                      <Package class="w-7 h-7 text-slate-200" />
                    </div>
                    <div class="min-w-0">
                      <p class="text-lg font-black text-slate-900 truncate tracking-tight">{{ product.name }}</p>
                      <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest mt-1">{{ product.category }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-8 py-6">
                  <span class="font-mono text-xs font-black text-slate-500 tracking-wider bg-slate-100 px-3 py-1.5 rounded-xl uppercase">{{ product.sku || 'N/A' }}</span>
                </td>
                <td class="px-8 py-6 text-lg font-black text-slate-900 tracking-tight">${{ product.price.toLocaleString() }}</td>
                <td class="px-8 py-6">
                  <div class="flex items-center gap-5">
                    <div class="w-32 h-2.5 bg-slate-100 rounded-full overflow-hidden shrink-0">
                      <div class="h-full transition-all duration-1000"
                        :class="product.stock === 0 ? 'bg-rose-500' : product.stock <= 10 ? 'bg-amber-500' : 'bg-emerald-500'"
                        :style="{ width: Math.min((product.stock / 100) * 100, 100) + '%' }"></div>
                    </div>
                    <span :class="`text-xs font-black uppercase tracking-tighter ${product.stock === 0 ? 'text-rose-600' : product.stock <= 10 ? 'text-amber-600' : 'text-emerald-600'}`">
                      {{ product.stock }} Units
                    </span>
                  </div>
                </td>
                <td class="px-8 py-6 last:rounded-r-[32px] text-right">
                  <button @click="openUpdateModal(product)"
                    class="p-3.5 text-slate-400 hover:text-blue-600 hover:bg-white hover:shadow-lg rounded-2xl transition-all active:scale-90 border border-transparent hover:border-slate-100"
                    title="Update Stock Count">
                    <Edit2 class="w-5 h-5" />
                  </button>
                </td>
              </tr>
              <tr v-if="filteredProducts.length === 0 && !loading">
                <td colspan="5" class="px-8 py-32 text-center">
                  <div class="w-32 h-32 bg-slate-50 rounded-[40px] flex items-center justify-center mx-auto mb-8 text-slate-100">
                    <Database class="w-16 h-16" />
                  </div>
                  <h3 class="text-2xl font-black text-slate-900 tracking-tight">Empty Inventory</h3>
                  <p class="text-slate-400 mt-2 font-bold max-w-sm mx-auto">No products were found matching your current filter parameters.</p>
                  <button @click="searchTerm = ''; selectedStockStatus = 'all'"
                    class="mt-8 px-10 py-4 bg-slate-900 text-white rounded-[20px] font-black uppercase tracking-widest text-xs hover:bg-slate-800 transition-all active:scale-95">
                    Clear Search Filters
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        </div>

        <div v-if="loading && products.length === 0" class="p-32 flex flex-col items-center justify-center gap-6">
          <div class="relative">
            <div class="w-20 h-20 border-4 border-slate-100 rounded-full"></div>
            <div class="w-20 h-20 border-4 border-blue-600 rounded-full border-t-transparent animate-spin absolute top-0"></div>
          </div>
          <p class="text-xs font-black text-slate-400 uppercase tracking-[0.2em]">Synchronizing Catalog...</p>
        </div>
      </div>

      <!-- Quick Update Modal -->
      <Teleport to="body">
        <Transition name="modal">
          <div v-if="showUpdateModal" class="fixed inset-0 z-[100] flex items-center justify-center p-6">
            <div class="fixed inset-0 bg-slate-900/60 backdrop-blur-xl" @click="showUpdateModal = false"></div>
            <div class="relative bg-white rounded-[48px] shadow-2xl w-full max-w-md overflow-hidden transform transition-all scale-100">
              <div class="px-10 py-8 border-b border-slate-50 flex items-center justify-between bg-white shrink-0">
                <h3 class="text-2xl font-black text-slate-900 tracking-tight leading-none">Stock Audit</h3>
                <button @click="showUpdateModal = false" class="p-3 hover:bg-slate-50 rounded-2xl transition-all text-slate-300">
                  <X class="w-6 h-6" />
                </button>
              </div>
              <div class="p-10">
                <div class="mb-10">
                  <div class="flex items-center gap-6 p-6 bg-slate-50 rounded-[32px] border border-slate-100/50">
                    <div class="w-16 h-16 rounded-[20px] bg-white flex items-center justify-center shadow-sm">
                      <Package class="w-8 h-8 text-slate-200" />
                    </div>
                    <div class="min-w-0">
                      <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Selected Item</p>
                      <p class="text-lg font-black text-slate-900 truncate tracking-tight">{{ currentProduct?.name }}</p>
                    </div>
                  </div>
                </div>
                <div class="mb-10">
                  <label class="block text-[10px] font-black text-slate-400 uppercase tracking-[0.2em] mb-4 ml-2">New Unit Count</label>
                  <div class="relative group">
                    <Database class="absolute left-6 top-1/2 -translate-y-1/2 w-6 h-6 text-slate-300 group-focus-within:text-blue-600 transition-colors" />
                    <input v-model.number="newStockValue" type="number" min="0"
                      class="w-full pl-16 pr-6 py-6 bg-slate-50 border-2 border-transparent rounded-[24px] focus:bg-white focus:border-blue-100 transition-all text-2xl font-black text-slate-900 outline-none" />
                  </div>
                  <div class="mt-4 flex items-center gap-3 px-4 text-slate-400">
                    <AlertCircle class="w-4 h-4 shrink-0" />
                    <p class="text-[10px] font-bold uppercase tracking-widest leading-relaxed">Overwrite current stock quantity in the database.</p>
                  </div>
                </div>
                <div class="flex gap-4">
                  <button @click="showUpdateModal = false"
                    class="flex-1 py-5 bg-white border-2 border-slate-100 text-slate-700 rounded-[20px] font-black text-xs uppercase tracking-widest hover:bg-slate-50 transition-all">
                    Cancel
                  </button>
                  <button @click="handleUpdateStock"
                    class="flex-1 py-5 bg-slate-900 text-white rounded-[20px] font-black text-xs uppercase tracking-widest hover:bg-slate-800 transition-all flex items-center justify-center gap-3 shadow-xl shadow-slate-200"
                    :disabled="updating">
                    <Loader2 v-if="updating" class="w-5 h-5 animate-spin" />
                    Commit Update
                  </button>
                </div>
              </div>
            </div>
          </div>
        </Transition>
      </Teleport>
    </div>
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
