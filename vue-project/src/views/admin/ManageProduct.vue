<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  Package,
  Plus,
  Search,
  Edit2,
  Trash2,
  X,
  Loader2,
  Download,
  Eye,
  AlertCircle,
  CheckCircle,
  DollarSign,
  RefreshCw
} from 'lucide-vue-next'
import { productAPI } from '../../api/productsApi.js'

// State
const products = ref([])
const loading = ref(false)
const error = ref(null)
const success = ref(null)
const searchTerm = ref('')
const selectedCategory = ref('all')
const selectedStock = ref('all')

// Modal states
const showAddModal = ref(false)
const showEditModal = ref(false)
const showViewModal = ref(false)
const currentProduct = ref(null)

// Form data
const formData = ref({
  Images:'',
  name: '',
  category: '',
  description: '',
  stock: 0,
  price: 0,
  sku: '',
  minStock: 0,
  supplier: ''
})

// Derived categories
const categories = computed(() => {
  const set = new Set(products.value.map(p => p.category).filter(Boolean))
  return Array.from(set)
})

// Computed properties
const filteredProducts = computed(() => {
  let filtered = products.value

  if (searchTerm.value) {
    const q = searchTerm.value.toLowerCase()
    filtered = filtered.filter(p =>
      p.name?.toLowerCase().includes(q) ||
      p.category?.toLowerCase().includes(q) ||
      p.sku?.toLowerCase().includes(q)
    )
  }

  if (selectedCategory.value !== 'all') {
    filtered = filtered.filter(p => p.category === selectedCategory.value)
  }

  if (selectedStock.value === 'in-stock') {
    filtered = filtered.filter(p => p.stock > 10)
  } else if (selectedStock.value === 'low-stock') {
    filtered = filtered.filter(p => p.stock > 0 && p.stock <= 10)
  } else if (selectedStock.value === 'out-of-stock') {
    filtered = filtered.filter(p => p.stock === 0)
  }

  return filtered
})

const stats = computed(() => ({
  total: products.value.length,
  inStock: products.value.filter(p => p.stock > 10).length,
  lowStock: products.value.filter(p => p.stock > 0 && p.stock <= 10).length,
  outOfStock: products.value.filter(p => p.stock === 0).length,
  totalValue: products.value.reduce((sum, p) => sum + (p.stock * p.price), 0)
}))

// Methods
const fetchProducts = async () => {
  try {
    loading.value = true
    error.value = null
    const res = await productAPI.getAllProducts()
    products.value = res.data || []
  } catch (err) {
    error.value = err.response?.data?.message || err.message || 'Failed to load products'
    console.error('Error fetching products:', err)
  } finally {
    loading.value = false
  }
}

const openAddModal = () => {
  formData.value = {
    Images: '',
    name: '',
    category: '',
    description: '',
    stock: 0,
    price: 0,
    sku: '',
    minStock: 0,
    supplier: ''
  }
  showAddModal.value = true
}

const openEditModal = (product) => {
  currentProduct.value = product
  formData.value = { ...product }
  showEditModal.value = true
}

const openViewModal = (product) => {
  currentProduct.value = product
  showViewModal.value = true
}

const closeModals = () => {
  showAddModal.value = false
  showEditModal.value = false
  showViewModal.value = false
  currentProduct.value = null
}

const handleAddProduct = async () => {
  try {
    loading.value = true
    error.value = null

    const payload = {
      images: formData.value.Images,
      name: formData.value.name,
      category: formData.value.category,
      description: formData.value.description || '',
      stock: Number(formData.value.stock),
      price: Number(formData.value.price),
      sku: formData.value.sku,
      minStock: Number(formData.value.minStock || 0),
      supplier: formData.value.supplier || ''
    }

    const response = await productAPI.createProducts(payload)

    // Refetch all products to ensure data consistency
    await fetchProducts()

    success.value = 'Product added successfully!'
    closeModals()
    setTimeout(() => (success.value = null), 3000)
  } catch (err) {
    error.value = err.response?.data?.message || err.response?.data || 'Failed to add product'
    console.error('Error adding product:', err)
  } finally {
    loading.value = false
  }
}

const handleUpdateProduct = async () => {
  try {
    loading.value = true
    error.value = null

    const payload = {
      name: formData.value.name,
      category: formData.value.category,
      description: formData.value.description || '',
      stock: Number(formData.value.stock),
      price: Number(formData.value.price),
      sku: formData.value.sku,
      minStock: Number(formData.value.minStock || 0),
      supplier: formData.value.supplier || ''
    }

    await productAPI.updateProducts(currentProduct.value.id, payload)

    // Refetch all products to ensure data consistency
    await fetchProducts()

    success.value = 'Product updated successfully!'
    closeModals()
    setTimeout(() => (success.value = null), 3000)
  } catch (err) {
    error.value = err.response?.data?.message || err.response?.data || 'Failed to update product'
    console.error('Error updating product:', err)
  } finally {
    loading.value = false
  }
}

const handleDeleteProduct = async (id) => {
  if (!confirm('Are you sure you want to delete this product? This action cannot be undone.')) return

  try {
    loading.value = true
    error.value = null
    await productAPI.hardDelete(id)
    products.value = products.value.filter(p => p.id !== id)
    success.value = 'Product deleted successfully!'
    setTimeout(() => (success.value = null), 3000)
  } catch (err) {
    error.value = err.response?.data?.message || err.response?.data || 'Failed to delete product'
    console.error('Error deleting product:', err)
  } finally {
    loading.value = false
  }
}

const exportToCSV = () => {
  const headers = ['ID', 'SKU', 'Name', 'Category', 'Stock', 'Price', 'Min Stock', 'Supplier']
  const rows = products.value.map(p => [
    p.id,
    p.sku,
    p.name,
    p.category,
    p.stock,
    p.price,
    p.minStock || 0,
    p.supplier || ''
  ])
  const csv = [headers, ...rows].map(r => r.join(',')).join('\n')
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `products-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

const getStockStatus = (product) => {
  if (product.stock === 0) return { text: 'Out of Stock', color: 'red' }
  if (product.stock <= (product.minStock || 10)) return { text: 'Low Stock', color: 'orange' }
  return { text: 'In Stock', color: 'green' }
}

onMounted(() => {
  fetchProducts()
})

</script>

<template>
  <div class="min-h-screen bg-slate-50/50">
    <!-- Error Banner -->
    <Transition name="slide-down">
      <div v-if="error" class="fixed top-8 left-1/2 -translate-x-1/2 z-[100] w-full max-w-2xl px-6">
        <div class="bg-white border border-rose-100 rounded-[32px] p-6 shadow-2xl shadow-rose-200/50 flex items-center gap-6">
          <div class="w-14 h-14 bg-rose-50 rounded-2xl flex items-center justify-center shrink-0">
            <AlertCircle class="w-7 h-7 text-rose-500" />
          </div>
          <div class="flex-1">
            <p class="text-sm font-black text-slate-900 uppercase tracking-widest">System Error</p>
            <p class="text-xs text-slate-500 mt-1 font-bold leading-relaxed">{{ error }}</p>
          </div>
          <div class="flex items-center gap-3">
            <button @click="error = null; fetchProducts()" class="p-3 bg-slate-900 text-white rounded-xl hover:bg-slate-800 transition-all shadow-sm">
              <RefreshCw class="w-5 h-5" />
            </button>
            <button @click="error = null" class="p-3 bg-slate-100 text-slate-400 rounded-xl hover:bg-slate-200 transition-all">
              <X class="w-5 h-5" />
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Success Message -->
    <Transition name="slide-down">
      <div v-if="success" class="fixed top-8 right-8 z-[100] max-w-md">
        <div class="bg-slate-900 border border-slate-800 text-white px-8 py-5 rounded-[24px] shadow-2xl flex items-center gap-4">
          <div class="w-10 h-10 bg-emerald-500 rounded-xl flex items-center justify-center shrink-0">
            <CheckCircle class="w-6 h-6 text-white" />
          </div>
          <span class="flex-1 font-bold text-sm tracking-tight">{{ success }}</span>
          <button @click="success = null" class="text-slate-400 hover:text-white transition-colors p-1">
            <X class="w-5 h-5" />
          </button>
        </div>
      </div>
    </Transition>

    <!-- Header -->
    <header class="bg-white border-b border-slate-100 sticky top-0 z-40">
      <div class="max-w-[1600px] mx-auto px-6 sm:px-8 lg:px-12 py-8">
        <div class="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-8">
          <div class="flex items-center gap-6">
            <div class="w-16 h-16 bg-slate-900 rounded-[24px] flex items-center justify-center shadow-xl shadow-slate-200">
              <Package class="w-8 h-8 text-white" />
            </div>
            <div>
              <h1 class="text-3xl font-black text-slate-900 tracking-tight">Products</h1>
              <p class="text-sm text-slate-400 mt-1 font-bold uppercase tracking-widest">Inventory Control</p>
            </div>
          </div>
          <div class="flex items-center gap-3">
            <button @click="fetchProducts"
              class="flex items-center gap-3 px-6 py-3.5 text-sm font-black text-slate-700 bg-white border-2 border-slate-100 rounded-2xl hover:bg-slate-50 transition-all active:scale-95 disabled:opacity-50"
              :disabled="loading">
              <RefreshCw :class="['w-5 h-5', loading && 'animate-spin']" />
              <span>{{ loading ? 'Syncing...' : 'Refresh' }}</span>
            </button>
            <button @click="openAddModal"
              class="flex items-center gap-3 px-8 py-4 bg-blue-600 text-white rounded-[20px] hover:bg-blue-700 transition-all shadow-xl shadow-blue-200 active:scale-95 font-black text-sm uppercase tracking-widest">
              <Plus class="w-5 h-5" />
              New Product
            </button>
          </div>
        </div>
      </div>
    </header>

    <main class="max-w-[1600px] mx-auto px-6 sm:px-8 lg:px-12 py-12">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-8 mb-12">
        <div v-for="(stat, idx) in [
          { label: 'Inventory', value: stats.total, icon: Package, color: 'blue', bg: 'bg-blue-50', text: 'text-blue-600' },
          { label: 'Healthy', value: stats.inStock, icon: CheckCircle, color: 'emerald', bg: 'bg-emerald-50', text: 'text-emerald-600' },
          { label: 'Low Stock', value: stats.lowStock, icon: AlertCircle, color: 'amber', bg: 'bg-amber-50', text: 'text-amber-600' },
          { label: 'Critical', value: stats.outOfStock, icon: X, color: 'rose', bg: 'bg-rose-50', text: 'text-rose-600' },
          { label: 'Asset Value', value: '$' + stats.totalValue.toLocaleString(), icon: DollarSign, color: 'slate', bg: 'bg-slate-900', text: 'text-white' }
        ]" :key="idx" 
          class="bg-white rounded-[32px] p-8 border border-slate-100 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300">
          <div class="flex items-center justify-between mb-6">
            <div :class="`w-14 h-14 ${stat.bg} rounded-2xl flex items-center justify-center transition-transform group-hover:rotate-12` ">
              <component :is="stat.icon" :class="`w-7 h-7 ${stat.text}`" />
            </div>
          </div>
          <p class="text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">{{ stat.label }}</p>
          <p class="text-3xl font-black text-slate-900 mt-2 tracking-tight">{{ stat.value }}</p>
        </div>
      </div>

      <!-- Actions Bar -->
      <div class="bg-white rounded-[40px] shadow-sm mb-12 p-8 border border-slate-100">
        <div class="flex flex-col lg:flex-row gap-6">
          <!-- Search -->
          <div class="flex-1">
            <div class="relative group">
              <Search class="absolute left-6 top-1/2 -translate-y-1/2 text-slate-300 w-6 h-6 group-focus-within:text-blue-600 transition-colors" />
              <input v-model="searchTerm" type="text"
                placeholder="Find product by name, category or SKU..."
                class="w-full pl-16 pr-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[24px] focus:bg-white focus:border-blue-100 focus:ring-4 focus:ring-blue-50 transition-all font-bold text-slate-600" />
            </div>
          </div>

          <!-- Filters -->
          <div class="flex flex-wrap gap-4">
            <select v-model="selectedCategory"
              class="px-8 py-4.5 bg-slate-50 border-2 border-transparent rounded-[24px] focus:bg-white focus:border-blue-100 transition-all font-black text-xs uppercase tracking-widest text-slate-700 outline-none cursor-pointer">
              <option value="all">Categories</option>
              <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
            </select>

            <select v-model="selectedStock"
              class="px-8 py-4.5 bg-slate-50 border-2 border-transparent rounded-[24px] focus:bg-white focus:border-blue-100 transition-all font-black text-xs uppercase tracking-widest text-slate-700 outline-none cursor-pointer">
              <option value="all">Stock Levels</option>
              <option value="in-stock">In Stock</option>
              <option value="low-stock">Low Stock</option>
              <option value="out-of-stock">Critical</option>
            </select>

            <button @click="exportToCSV"
              class="flex items-center gap-3 px-8 py-4.5 bg-slate-50 text-slate-700 rounded-[24px] hover:bg-slate-100 transition-all font-black text-xs uppercase tracking-widest active:scale-95">
              <Download class="w-5 h-5" />
              Export
            </button>
          </div>
        </div>
      </div>

      <!-- Products Table -->
      <div class="bg-white rounded-[48px] shadow-sm overflow-hidden border border-slate-100">
        <div v-if="loading && products.length === 0" class="p-20 space-y-8">
          <div v-for="i in 5" :key="i" class="flex items-center gap-8 animate-pulse">
            <div class="w-20 h-20 bg-slate-100 rounded-3xl"></div>
            <div class="flex-1 space-y-4">
              <div class="h-6 bg-slate-100 rounded-lg w-1/3"></div>
              <div class="h-4 bg-slate-100 rounded-lg w-1/4"></div>
            </div>
          </div>
        </div>

        <div v-else-if="filteredProducts.length === 0" class="text-center py-32">
          <div class="w-32 h-32 bg-slate-50 rounded-[40px] flex items-center justify-center mx-auto mb-8">
            <Package class="w-16 h-16 text-slate-200" />
          </div>
          <h3 class="text-2xl font-black text-slate-900 tracking-tight">No results found</h3>
          <p class="text-slate-400 mt-2 font-bold max-w-sm mx-auto">We couldn't find any products matching your current search or filter criteria.</p>
          <button @click="searchTerm = ''; selectedCategory = 'all'; selectedStock = 'all'"
            class="mt-8 px-10 py-4 bg-slate-900 text-white rounded-[20px] font-black uppercase tracking-widest text-xs hover:bg-slate-800 transition-all active:scale-95">
            Reset Filters
          </button>
        </div>

        <div v-else class="overflow-x-auto px-4">
          <table class="w-full border-separate border-spacing-y-4">
            <thead>
              <tr>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Product Info</th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">SKU</th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Category</th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Inventory</th>
                <th class="px-8 py-6 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Price</th>
                <th class="px-8 py-6 text-right text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="product in filteredProducts" :key="product.id"
                class="group hover:bg-slate-50 transition-all duration-300 rounded-[32px]">
                <td class="px-8 py-6 first:rounded-l-[32px]">
                  <div class="flex items-center gap-6">
                    <div class="w-20 h-20 rounded-[28px] bg-white shadow-sm border border-slate-100 flex items-center justify-center shrink-0 group-hover:scale-105 transition-transform duration-500 overflow-hidden">
                      <img v-if="product.images" :src="product.images" class="w-full h-full object-cover" />
                      <Package v-else class="w-8 h-8 text-slate-200" />
                    </div>
                    <div class="min-w-0">
                      <p class="text-lg font-black text-slate-900 truncate tracking-tight">{{ product.name }}</p>
                      <p class="text-xs font-bold text-slate-400 truncate mt-1 leading-relaxed">{{ product.description || 'No description provided' }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-8 py-6">
                  <span class="font-mono text-sm font-black text-slate-500 tracking-wider bg-slate-100 px-3 py-1.5 rounded-xl uppercase">{{ product.sku }}</span>
                </td>
                <td class="px-8 py-6">
                  <div class="inline-flex items-center gap-2 px-4 py-2 bg-slate-50 text-slate-700 rounded-2xl text-[10px] font-black uppercase tracking-widest border border-slate-100/50">
                    {{ product.category }}
                  </div>
                </td>
                <td class="px-8 py-6">
                  <div class="flex items-center gap-4">
                    <div class="w-24 h-2.5 bg-slate-100 rounded-full overflow-hidden shrink-0">
                      <div class="h-full transition-all duration-1000"
                        :class="{
                          'bg-rose-500': getStockStatus(product).color === 'red',
                          'bg-amber-500': getStockStatus(product).color === 'orange',
                          'bg-emerald-500': getStockStatus(product).color === 'green'
                        }"
                        :style="{ width: Math.min((product.stock / 100) * 100, 100) + '%' }">
                      </div>
                    </div>
                    <span class="text-xs font-black uppercase tracking-tighter"
                      :class="{
                        'text-rose-600': getStockStatus(product).color === 'red',
                        'text-amber-600': getStockStatus(product).color === 'orange',
                        'text-emerald-600': getStockStatus(product).color === 'green'
                      }">
                      {{ product.stock }} Qty
                    </span>
                  </div>
                </td>
                <td class="px-8 py-6 text-lg font-black text-slate-900 tracking-tight">${{ product.price.toFixed(2) }}</td>
                <td class="px-8 py-6 last:rounded-r-[32px]">
                  <div class="flex justify-end gap-3">
                    <button @click="openViewModal(product)"
                      class="p-3.5 text-slate-400 hover:text-slate-900 hover:bg-white hover:shadow-lg rounded-2xl transition-all active:scale-90 border border-transparent hover:border-slate-100">
                      <Eye class="w-5 h-5" />
                    </button>
                    <button @click="openEditModal(product)"
                      class="p-3.5 text-slate-400 hover:text-blue-600 hover:bg-white hover:shadow-lg rounded-2xl transition-all active:scale-90 border border-transparent hover:border-slate-100">
                      <Edit2 class="w-5 h-5" />
                    </button>
                    <button @click="handleDeleteProduct(product.id)"
                      class="p-3.5 text-slate-400 hover:text-rose-600 hover:bg-white hover:shadow-lg rounded-2xl transition-all active:scale-90 border border-transparent hover:border-slate-100">
                      <Trash2 class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>

    <!-- Form Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showAddModal || showEditModal"
          class="fixed inset-0 bg-slate-900/60 backdrop-blur-xl flex items-center justify-center z-[100] p-6"
          @click.self="closeModals">
          <div class="bg-white rounded-[48px] shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col scale-100 transition-transform">
            <div class="p-10 border-b border-slate-50 flex justify-between items-center shrink-0">
              <div class="flex items-center gap-6">
                <div class="w-14 h-14 bg-slate-900 rounded-2xl flex items-center justify-center">
                  <Package class="w-7 h-7 text-white" />
                </div>
                <div>
                  <h3 class="text-3xl font-black text-slate-900 tracking-tight">
                    {{ showAddModal ? 'Create Product' : 'Edit Product' }}
                  </h3>
                  <p class="text-xs font-bold text-slate-400 uppercase tracking-[0.2em] mt-1">Configure your product data</p>
                </div>
              </div>
              <button @click="closeModals" class="p-4 hover:bg-slate-50 rounded-2xl transition-all active:rotate-90 duration-300">
                <X class="w-7 h-7 text-slate-400" />
              </button>
            </div>

            <div class="p-10 overflow-y-auto grow custom-scrollbar">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-10">
                <div class="space-y-8">
                  <div class="group">
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3 ml-1 group-focus-within:text-blue-600 transition-colors">Identification</label>
                    <div class="space-y-4">
                      <input v-model="formData.name" type="text" required
                        class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-bold text-slate-700 outline-none"
                        placeholder="Product Name" />
                      <input v-model="formData.sku" type="text" required
                        class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-mono font-bold text-slate-700 outline-none uppercase tracking-wider"
                        placeholder="SKU Code" />
                    </div>
                  </div>

                  <div class="group">
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3 ml-1">Classification</label>
                    <div class="space-y-4">
                      <input v-model="formData.category" type="text" required
                        class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-bold text-slate-700 outline-none"
                        placeholder="Category" />
                      <input v-model="formData.supplier" type="text"
                        class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-bold text-slate-700 outline-none"
                        placeholder="Supplier Name" />
                    </div>
                  </div>

                  <div class="group">
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3 ml-1">Images</label>
                    <input v-model="formData.Images" type="text"
                      class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-bold text-slate-700 outline-none"
                      placeholder="Image URL" />
                  </div>
                </div>

                <div class="space-y-8">
                  <div class="group">
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3 ml-1">Inventory & Value</label>
                    <div class="grid grid-cols-2 gap-4">
                      <div class="space-y-2">
                        <p class="text-[9px] font-bold text-slate-400 ml-1">PRICE ($)</p>
                        <input v-model.number="formData.price" type="number" step="0.01" min="0" required
                          class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-black text-slate-900 outline-none" />
                      </div>
                      <div class="space-y-2">
                        <p class="text-[9px] font-bold text-slate-400 ml-1">TOTAL STOCK</p>
                        <input v-model.number="formData.stock" type="number" min="0" required
                          class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-black text-slate-900 outline-none" />
                      </div>
                    </div>
                    <div class="mt-4">
                      <p class="text-[9px] font-bold text-slate-400 ml-1 mb-2">MINIMUM ALERT LEVEL</p>
                      <input v-model.number="formData.minStock" type="number" min="0"
                        class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-black text-slate-900 outline-none" />
                    </div>
                  </div>

                  <div class="group">
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3 ml-1">Description</label>
                    <textarea v-model="formData.description" rows="6"
                      class="w-full px-6 py-5 bg-slate-50 border-2 border-transparent rounded-[24px] focus:bg-white focus:border-blue-100 transition-all font-bold text-slate-700 outline-none resize-none"
                      placeholder="Provide detailed information about this product..."></textarea>
                  </div>
                </div>
              </div>
            </div>

            <div class="p-10 border-t border-slate-50 bg-slate-50/50 flex gap-4 shrink-0">
              <button @click="closeModals"
                class="px-10 py-5 bg-white border-2 border-slate-200 text-slate-700 rounded-[20px] hover:bg-slate-50 transition-all font-black text-xs uppercase tracking-widest">
                Discard Changes
              </button>
              <button @click="showAddModal ? handleAddProduct() : handleUpdateProduct()"
                :disabled="loading || !formData.name || !formData.sku || !formData.category"
                class="flex-1 bg-slate-900 text-white py-5 rounded-[20px] hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed transition-all font-black text-xs uppercase tracking-widest flex items-center justify-center gap-3 shadow-xl shadow-slate-200">
                <Loader2 v-if="loading" class="w-5 h-5 animate-spin" />
                <span>{{ loading ? 'Processing...' : (showAddModal ? 'Confirm Creation' : 'Update Record') }}</span>
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- View Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showViewModal && currentProduct"
          class="fixed inset-0 bg-slate-900/60 backdrop-blur-xl flex items-center justify-center z-[100] p-6"
          @click.self="closeModals">
          <div class="bg-white rounded-[48px] shadow-2xl w-full max-w-3xl overflow-hidden flex flex-col scale-100 transition-transform">
            <div class="relative h-64 bg-slate-900 shrink-0">
              <div v-if="currentProduct.images" class="absolute inset-0">
                <img :src="currentProduct.images" class="w-full h-full object-cover opacity-60" />
                <div class="absolute inset-0 bg-gradient-to-t from-slate-900 via-transparent"></div>
              </div>
              <div class="absolute bottom-8 left-10 right-10 flex justify-between items-end">
                <div>
                  <span class="px-3 py-1 bg-blue-600 text-white rounded-lg text-[9px] font-black uppercase tracking-widest mb-3 inline-block">
                    {{ currentProduct.category }}
                  </span>
                  <h3 class="text-4xl font-black text-white tracking-tight leading-none">{{ currentProduct.name }}</h3>
                </div>
                <div class="text-right">
                  <p class="text-xs font-bold text-slate-400 uppercase tracking-widest">Pricing</p>
                  <p class="text-3xl font-black text-white mt-1 tracking-tight">${{ currentProduct.price.toFixed(2) }}</p>
                </div>
              </div>
              <button @click="closeModals" class="absolute top-8 right-8 p-3 bg-white/10 hover:bg-white/20 text-white rounded-2xl backdrop-blur-md transition-all">
                <X class="w-6 h-6" />
              </button>
            </div>

            <div class="p-10">
              <div class="grid grid-cols-3 gap-8 mb-10">
                <div class="bg-slate-50 p-6 rounded-3xl border border-slate-100/50">
                  <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest">Inventory Status</p>
                  <div class="flex items-center gap-3 mt-2">
                    <div :class="`w-3 h-3 rounded-full ${getStockStatus(currentProduct).color === 'green' ? 'bg-emerald-500 shadow-[0_0_12px_rgba(16,185,129,0.5)]' : getStockStatus(currentProduct).color === 'orange' ? 'bg-amber-500 shadow-[0_0_12px_rgba(245,158,11,0.5)]' : 'bg-rose-500 shadow-[0_0_12px_rgba(244,63,94,0.5)]'}`"></div>
                    <p class="text-lg font-black text-slate-900 tracking-tight">{{ currentProduct.stock }} <span class="text-xs font-bold text-slate-500">Units</span></p>
                  </div>
                </div>
                <div class="bg-slate-50 p-6 rounded-3xl border border-slate-100/50">
                  <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest">Identification</p>
                  <p class="text-lg font-black text-slate-900 mt-2 tracking-widest font-mono">{{ currentProduct.sku }}</p>
                </div>
                <div class="bg-slate-50 p-6 rounded-3xl border border-slate-100/50">
                  <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest">Total Asset Value</p>
                  <p class="text-lg font-black text-blue-600 mt-2 tracking-tight">${{ (currentProduct.stock * currentProduct.price).toFixed(2) }}</p>
                </div>
              </div>

              <div class="space-y-4">
                <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest ml-1">Product Description</p>
                <div class="bg-slate-50/50 p-8 rounded-[32px] border border-slate-100/50 min-h-[120px]">
                  <p class="text-slate-600 font-bold leading-relaxed italic">
                    "{{ currentProduct.description || 'This product does not have a detailed description yet.' }}"
                  </p>
                </div>
              </div>

              <div class="flex gap-4 mt-12">
                <button @click="closeModals"
                  class="px-10 py-5 bg-white border-2 border-slate-100 text-slate-700 rounded-[20px] hover:bg-slate-50 transition-all font-black text-xs uppercase tracking-widest">
                  Back to List
                </button>
                <button @click="openEditModal(currentProduct); showViewModal = false"
                  class="flex-1 bg-slate-900 text-white py-5 rounded-[20px] hover:bg-slate-800 transition-all font-black text-xs uppercase tracking-widest shadow-xl shadow-slate-200">
                  Modify Product Data
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
.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.3s ease;
}

.slide-down-enter-from {
  opacity: 0;
  transform: translateY(-20px) translateX(-50%);
}

.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-20px) translateX(-50%);
}

.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from > div,
.modal-leave-to > div {
  transform: scale(0.95);
}
</style>