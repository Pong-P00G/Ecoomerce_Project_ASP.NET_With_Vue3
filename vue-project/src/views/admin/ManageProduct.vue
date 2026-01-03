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
  <div class="min-h-screen bg-linear-to-br from-gray-50 to-gray-100">
    <!-- Error Banner -->
    <Transition name="slide-down">
      <div v-if="error" class="fixed top-4 left-1/2 -translate-x-1/2 z-50 w-full max-w-2xl px-4">
        <div class="bg-red-50 border-2 border-red-200 rounded-2xl p-4 shadow-xl flex items-center gap-3">
          <AlertCircle class="w-5 h-5 text-red-600 shrink-0" />
          <div class="flex-1">
            <p class="text-sm font-semibold text-red-900">Error</p>
            <p class="text-xs text-red-700 mt-0.5">{{ error }}</p>
          </div>
          <button @click="error = null; fetchProducts()"
            class="text-sm font-medium text-red-700 hover:text-red-900 underline">
            Retry
          </button>
          <button @click="error = null" class="text-red-400 hover:text-red-600">
            <X class="w-4 h-4" />
          </button>
        </div>
      </div>
    </Transition>

    <!-- Success Message -->
    <Transition name="slide-down">
      <div v-if="success" class="fixed top-4 right-4 z-50 max-w-md">
        <div class="bg-emerald-50 border-2 border-emerald-200 text-emerald-800 px-6 py-4 rounded-2xl shadow-xl flex items-center gap-3">
          <CheckCircle class="w-5 h-5 shrink-0" />
          <span class="flex-1 font-medium">{{ success }}</span>
          <button @click="success = null" class="text-emerald-600 hover:text-emerald-800 transition-colors">
            <X class="w-4 h-4" />
          </button>
        </div>
      </div>
    </Transition>

    <!-- Header -->
    <header class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4">
          <div>
            <h1 class="text-3xl font-bold text-gray-900 flex items-center gap-3">
              <div class="bg-blue-600 p-2 rounded-xl">
                <Package class="w-6 h-6 text-white" />
              </div>
              Product Management
            </h1>
            <p class="text-sm text-gray-600 mt-2">Manage inventory, pricing, and stock levels</p>
          </div>
          <button @click="fetchProducts"
            class="self-start sm:self-auto flex items-center gap-2 px-5 py-2.5 text-sm font-medium text-blue-700 bg-blue-50 hover:bg-blue-100 rounded-xl transition-all hover:shadow-md"
            :disabled="loading">
            <RefreshCw :class="['w-4 h-4', loading && 'animate-spin']" />
            <span>{{ loading ? 'Refreshing...' : 'Refresh' }}</span>
          </button>
        </div>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div v-if="loading && products.length === 0" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-5 gap-6">
          <div v-for="i in 5" :key="i" class="bg-white p-6 rounded-2xl shadow-sm animate-pulse">
            <div class="h-4 bg-gray-200 rounded w-24 mb-3"></div>
            <div class="h-8 bg-gray-200 rounded w-16"></div>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div v-else class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white rounded-2xl shadow-sm p-6 hover:shadow-lg transition-all border-2 border-transparent hover:border-blue-100">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Total Products</p>
              <p class="text-3xl font-bold text-gray-900 mt-2">{{ stats.total }}</p>
            </div>
            <div class="w-14 h-14 bg-blue-100 rounded-xl flex items-center justify-center">
              <Package class="w-7 h-7 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white rounded-2xl shadow-sm p-6 hover:shadow-lg transition-all border-2 border-transparent hover:border-emerald-100">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-semibold text-gray-500 uppercase tracking-wider">In Stock</p>
              <p class="text-3xl font-bold text-emerald-600 mt-2">{{ stats.inStock }}</p>
            </div>
            <div class="w-14 h-14 bg-emerald-100 rounded-xl flex items-center justify-center">
              <CheckCircle class="w-7 h-7 text-emerald-600" />
            </div>
          </div>
        </div>

        <div class="bg-white rounded-2xl shadow-sm p-6 hover:shadow-lg transition-all border-2 border-transparent hover:border-amber-100">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Low Stock</p>
              <p class="text-3xl font-bold text-amber-600 mt-2">{{ stats.lowStock }}</p>
            </div>
            <div class="w-14 h-14 bg-amber-100 rounded-xl flex items-center justify-center">
              <AlertCircle class="w-7 h-7 text-amber-600" />
            </div>
          </div>
        </div>

        <div class="bg-white rounded-2xl shadow-sm p-6 hover:shadow-lg transition-all border-2 border-transparent hover:border-rose-100">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Out of Stock</p>
              <p class="text-3xl font-bold text-rose-600 mt-2">{{ stats.outOfStock }}</p>
            </div>
            <div class="w-14 h-14 bg-rose-100 rounded-xl flex items-center justify-center">
              <X class="w-7 h-7 text-rose-600" />
            </div>
          </div>
        </div>

        <div class="bg-linear-to-br from-blue-600 to-blue-700 rounded-2xl shadow-lg p-6 hover:shadow-xl transition-all">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-semibold text-blue-100 uppercase tracking-wider">Total Value</p>
              <p class="text-2xl font-bold text-white mt-2">${{ stats.totalValue.toLocaleString() }}</p>
            </div>
            <div class="w-14 h-14 bg-white/20 rounded-xl flex items-center justify-center">
              <DollarSign class="w-7 h-7 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Actions Bar -->
      <div class="bg-white rounded-2xl shadow-sm mb-8 p-6 border border-gray-200">
        <div class="flex flex-col lg:flex-row gap-4">
          <!-- Search -->
          <div class="flex-1">
            <div class="relative">
              <Search class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 w-5 h-5" />
              <input v-model="searchTerm" type="text"
                placeholder="Search by name, category, or SKU..."
                class="w-full pl-12 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all" />
            </div>
          </div>

          <!-- Filters & Actions -->
          <div class="flex flex-wrap gap-3">
            <select v-model="selectedCategory"
              class="px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 text-sm font-medium">
              <option value="all">All Categories</option>
              <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
            </select>

            <select v-model="selectedStock"
              class="px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 text-sm font-medium">
              <option value="all">All Stock Levels</option>
              <option value="in-stock">In Stock</option>
              <option value="low-stock">Low Stock</option>
              <option value="out-of-stock">Out of Stock</option>
            </select>

            <button @click="exportToCSV"
              class="flex items-center gap-2 px-5 py-3 bg-gray-50 text-gray-700 border border-gray-200 rounded-xl hover:bg-gray-100 transition-all font-semibold text-sm">
              <Download class="w-4 h-4" />
              Export
            </button>

            <button @click="openAddModal"
              class="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition-all shadow-lg shadow-blue-200 font-semibold text-sm">
              <Plus class="w-4 h-4" />
              Add Product
            </button>
          </div>
        </div>
      </div>

      <!-- Products Table -->
      <div class="bg-white rounded-2xl shadow-sm overflow-hidden border border-gray-200">
        <!-- Empty State -->
        <div v-if="filteredProducts.length === 0 && !loading" class="text-center py-16">
          <div class="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <Package class="w-10 h-10 text-gray-400" />
          </div>
          <p class="text-gray-600 font-semibold text-lg mb-2">No products found</p>
          <p class="text-sm text-gray-500 mb-6">Try adjusting your filters or add a new product</p>
          <button @click="searchTerm = ''; selectedCategory = 'all'; selectedStock = 'all'"
            class="px-6 py-2.5 text-sm font-semibold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors">
            Clear Filters
          </button>
        </div>

        <!-- Table -->
        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="bg-gray-50 border-b border-gray-200">
                <th class="px-6 py-4 text-left text-xs font-bold text-gray-600 uppercase tracking-wider">Product</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-gray-600 uppercase tracking-wider">SKU</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-gray-600 uppercase tracking-wider">Category</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-gray-600 uppercase tracking-wider">Stock</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-gray-600 uppercase tracking-wider">Price</th>
                <th class="px-6 py-4 text-right text-xs font-bold text-gray-600 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-100">
              <tr v-for="product in filteredProducts" :key="product.id"
                class="hover:bg-gray-50 transition-colors group">
                <td class="px-6 py-4">
                  <div class="flex items-center gap-3">
                    <div class="w-12 h-12 rounded-xl bg-linear-to-br from-blue-50 to-blue-100 flex items-center justify-center shrink-0">
                      <Package class="w-6 h-6 text-blue-600" />
                    </div>
                    <div class="min-w-0">
                      <p class="font-semibold text-gray-900 truncate">{{ product.name }}</p>
                      <p class="text-xs text-gray-500 truncate">{{ product.description || 'No description' }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-gray-600 font-medium">{{ product.sku }}</td>
                <td class="px-6 py-4">
                  <span class="px-3 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs font-semibold uppercase">
                    {{ product.category }}
                  </span>
                </td>
                <td class="px-6 py-4">
                  <div class="flex items-center gap-2">
                    <div class="flex-1 h-2 bg-gray-100 rounded-full overflow-hidden max-w-80px">
                      <div class="h-full transition-all"
                        :class="{
                          'bg-rose-500': getStockStatus(product).color === 'red',
                          'bg-amber-500': getStockStatus(product).color === 'orange',
                          'bg-emerald-500': getStockStatus(product).color === 'green'
                        }"
                        :style="{ width: Math.min((product.stock / 100) * 100, 100) + '%' }">
                      </div>
                    </div>
                    <span class="text-xs font-bold"
                      :class="{
                        'text-rose-600': getStockStatus(product).color === 'red',
                        'text-amber-600': getStockStatus(product).color === 'orange',
                        'text-emerald-600': getStockStatus(product).color === 'green'
                      }">
                      {{ product.stock }}
                    </span>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm font-bold text-gray-900">${{ product.price.toFixed(2) }}</td>
                <td class="px-6 py-4">
                  <div class="flex justify-end gap-2">
                    <button @click="openViewModal(product)"
                      class="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-all">
                      <Eye class="w-4 h-4" />
                    </button>
                    <button @click="openEditModal(product)"
                      class="p-2 text-gray-400 hover:text-emerald-600 hover:bg-emerald-50 rounded-lg transition-all">
                      <Edit2 class="w-4 h-4" />
                    </button>
                    <button @click="handleDeleteProduct(product.id)"
                      class="p-2 text-gray-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition-all">
                      <Trash2 class="w-4 h-4" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Add/Edit Product Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showAddModal || showEditModal"
          class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4"
          @click.self="closeModals">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-3xl max-h-[90vh] overflow-hidden">
            <div class="p-6 border-b border-gray-200 bg-linear-to-r from-blue-600 to-blue-700">
              <div class="flex justify-between items-center">
                <h3 class="text-2xl font-bold text-white">
                  {{ showAddModal ? 'Add New Product' : 'Edit Product' }}
                </h3>
                <button @click="closeModals" class="p-2 hover:bg-white/10 rounded-lg transition-colors">
                  <X class="w-5 h-5 text-white" />
                </button>
              </div>
            </div>

            <div class="p-6 overflow-y-auto max-h-[calc(90vh-140px)]">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Product Name *</label>
                  <input v-model="formData.name" type="text" required
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="e.g., Premium Widget" />
                </div>

                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">SKU *</label>
                  <input v-model="formData.sku" type="text" required
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="e.g., WDG-001" />
                </div>

                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Category *</label>
                  <input v-model="formData.category" type="text" required
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="e.g., Electronics" />
                </div>

                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Supplier</label>
                  <input v-model="formData.supplier" type="text"
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="e.g., ABC Corp" />
                </div>

                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Stock Quantity *</label>
                  <input v-model.number="formData.stock" type="number" min="0" required
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="0" />
                </div>

                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Minimum Stock</label>
                  <input v-model.number="formData.minStock" type="number" min="0"
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="10" />
                </div>

                <div>
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Price *</label>
                  <input v-model.number="formData.price" type="number" step="0.01" min="0" required
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                    placeholder="0.00" />
                </div>

                <div class="md:col-span-2">
                  <label class="block text-sm font-semibold text-gray-700 mb-2">Description</label>
                  <textarea v-model="formData.description" rows="4"
                    class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all resize-none"
                    placeholder="Enter product description..."></textarea>
                </div>
              </div>
            </div>

            <div class="p-6 border-t border-gray-200 bg-gray-50 flex gap-3">
              <button @click="closeModals"
                class="px-6 py-3 bg-white border border-gray-300 text-gray-700 rounded-xl hover:bg-gray-50 transition-all font-semibold">
                Cancel
              </button>
              <button @click="showAddModal ? handleAddProduct() : handleUpdateProduct()"
                :disabled="loading || !formData.name || !formData.sku || !formData.category"
                class="flex-1 bg-blue-600 text-white py-3 rounded-xl hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all font-semibold flex items-center justify-center gap-2 shadow-lg shadow-blue-200">
                <Loader2 v-if="loading" class="w-5 h-5 animate-spin" />
                <span>{{ loading ? 'Saving...' : (showAddModal ? 'Add Product' : 'Update Product') }}</span>
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- View Product Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showViewModal && currentProduct"
          class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4"
          @click.self="closeModals">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl overflow-hidden">
            <div class="p-6 border-b border-gray-200 bg-linear-to-r from-gray-800 to-gray-900">
              <div class="flex justify-between items-center">
                <h3 class="text-2xl font-bold text-white">Product Details</h3>
                <button @click="closeModals" class="p-2 hover:bg-white/10 rounded-lg transition-colors">
                  <X class="w-5 h-5 text-white" />
                </button>
              </div>
            </div>

            <div class="p-6">
              <div class="grid grid-cols-2 gap-6">
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Product Name</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">{{ currentProduct.name }}</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">SKU</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">{{ currentProduct.sku }}</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Category</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">{{ currentProduct.category }}</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Supplier</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">{{ currentProduct.supplier || 'N/A' }}</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Stock Quantity</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">{{ currentProduct.stock }} units</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Minimum Stock</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">{{ currentProduct.minStock || 0 }} units</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Price</p>
                  <p class="text-lg font-semibold text-gray-900 mt-1">${{ currentProduct.price.toFixed(2) }}</p>
                </div>
                <div>
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Status</p>
                  <span :class="[
                    'inline-block px-3 py-1 text-sm font-bold rounded-full mt-1',
                    getStockStatus(currentProduct).color === 'green' ? 'bg-emerald-100 text-emerald-700' :
                    getStockStatus(currentProduct).color === 'orange' ? 'bg-amber-100 text-amber-700' :
                    'bg-rose-100 text-rose-700'
                  ]">
                    {{ getStockStatus(currentProduct).text }}
                  </span>
                </div>
                <div class="col-span-2">
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Description</p>
                  <p class="text-base text-gray-900 mt-2">{{ currentProduct.description || 'No description available' }}</p>
                </div>
                <div class="col-span-2 pt-4 border-t border-gray-200">
                  <p class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Total Value</p>
                  <p class="text-2xl font-bold text-blue-600 mt-1">
                    ${{ (currentProduct.stock * currentProduct.price).toFixed(2) }}
                  </p>
                </div>
              </div>
            </div>

            <div class="p-6 border-t border-gray-200 bg-gray-50 flex gap-3">
              <button @click="closeModals"
                class="px-6 py-3 bg-white border border-gray-300 text-gray-700 rounded-xl hover:bg-gray-50 transition-all font-semibold">
                Close
              </button>
              <button @click="openEditModal(currentProduct); showViewModal = false"
                class="flex-1 bg-blue-600 text-white py-3 rounded-xl hover:bg-blue-700 transition-all font-semibold shadow-lg shadow-blue-200">
                Edit Product
              </button>
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