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
  RefreshCw,
  ChevronDown
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
const selectedProducts = ref([])

const toggleSelectAll = () => {
  if (selectedProducts.value.length === filteredProducts.value.length) {
    selectedProducts.value = []
  } else {
    selectedProducts.value = filteredProducts.value.map(p => p.id)
  }
}

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
  supplier: '',
  variants: [],
  images: [{ url: '', isMain: true }]
})

const addImage = () => {
  formData.value.images.push({ url: '', isMain: false })
}

const removeImage = (index) => {
  if (formData.value.images[index].isMain && formData.value.images.length > 1) {
    formData.value.images[0].isMain = true
  }
  formData.value.images.splice(index, 1)
  if (formData.value.images.length === 0) {
    addImage()
    formData.value.images[0].isMain = true
  }
}

const setMainImage = (index) => {
  formData.value.images.forEach((img, i) => (img.isMain = i === index))
}

const addVariant = () => {
  formData.value.variants.push({
    sku: formData.value.sku ? `${formData.value.sku}-${formData.value.variants.length + 1}` : '',
    price: formData.value.price || 0,
    stock: formData.value.stock || 0,
    isCollapsed: false,
    options: [
      { variant: 'Color', value: '' },
      { variant: 'Size', value: '' }
    ]
  })
}

const toggleVariantCollapse = (index) => {
  formData.value.variants[index].isCollapsed = !formData.value.variants[index].isCollapsed
}

const removeVariant = (index) => {
  formData.value.variants.splice(index, 1)
}

const addOption = (variantIndex) => {
  formData.value.variants[variantIndex].options.push({ variant: '', value: '' })
}

const removeOption = (variantIndex, optionIndex) => {
  formData.value.variants[variantIndex].options.splice(optionIndex, 1)
}

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
        const res = await productAPI.getAllProducts({ pageSize: 100 }) // Fetch more items
        
        products.value = (res.data.items || []).map(p => ({
            id: p.productId,
            name: p.productName,
            category: p.categories?.[0]?.categoryName || 'Uncategorized',
            description: p.description,
            stock: p.stock,
            price: p.basePrice,
            sku: p.sku,
            minStock: p.minStock,
            supplier: p.supplier,
            images: p.images?.[0]?.imageUrl || ''
        }))
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
        supplier: '',
        variants: [],
        images: [{ url: '', isMain: true }]
    }
    showAddModal.value = true
}

const openEditModal = async (product) => {
    try {
        loading.value = true
        const res = await productAPI.getProductsById(product.id)
        const p = res.data
        
        // Map images to objects
        const mappedImages = (p.images || []).map(img => ({
            url: img.imageUrl,
            isMain: img.isPrimary
        }))
        
        if (mappedImages.length === 0) {
            mappedImages.push({ url: '', isMain: true })
        }

        currentProduct.value = {
            id: p.productId,
            name: p.productName,
            category: p.categories?.[0]?.categoryName || 'Uncategorized',
            description: p.description,
            stock: p.stock,
            price: p.basePrice,
            sku: p.sku,
            minStock: p.minStock,
            supplier: p.supplier,
            images: p.images?.[0]?.imageUrl || '', // legacy for table display
            variants: p.variants || [],
            imageList: mappedImages
        }
        formData.value = { ...currentProduct.value, images: mappedImages }
        showEditModal.value = true
    } catch (err) {
        error.value = 'Failed to fetch product details'
        console.error(err)
    } finally {
        loading.value = false
    }
}

const openViewModal = async (product) => {
    try {
        loading.value = true
        const res = await productAPI.getProductsById(product.id)
        const p = res.data
        currentProduct.value = {
            id: p.productId,
            name: p.productName,
            category: p.categories?.[0]?.categoryName || 'Uncategorized',
            description: p.description,
            stock: p.stock,
            price: p.basePrice,
            sku: p.sku,
            minStock: p.minStock,
            supplier: p.supplier,
            images: p.images?.[0]?.imageUrl || '',
            variants: p.variants || []
        }
        showViewModal.value = true
    } catch (err) {
        error.value = 'Failed to fetch product details'
        console.error(err)
    } finally {
        loading.value = false
    }
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

        // Prepare image URLs with main image at index 0
        const mainImg = formData.value.images.find(img => img.isMain) || formData.value.images[0]
        const otherImgs = formData.value.images.filter(img => img !== mainImg)
        const imageUrls = [mainImg.url, ...otherImgs.map(img => img.url)].filter(Boolean)

        const payload = {
            productName: formData.value.name,
            basePrice: Number(formData.value.price),
            sku: formData.value.sku,
            description: formData.value.description || '',
            stock: Number(formData.value.stock),
            minStock: Number(formData.value.minStock || 0),
            supplier: formData.value.supplier || '',
            categoryNames: [formData.value.category],
            imageUrls: imageUrls,
            variants: formData.value.variants.length > 0 
                ? formData.value.variants.map(v => ({
                    sku: v.sku,
                    price: Number(v.price),
                    stockQuantity: Number(v.stock),
                    isActive: true,
                    options: v.options.map(opt => ({
                        variant: opt.variant,
                        value: opt.value
                    }))
                }))
                : [{
                    sku: formData.value.sku + '-STD',
                    price: Number(formData.value.price),
                    stockQuantity: Number(formData.value.stock),
                    isActive: true,
                    options: [{ variant: 'Standard', value: 'Default' }]
                }]
        }

        await productAPI.createProducts(payload)

        // Refetch all products
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

        // Prepare image URLs with main image at index 0
        const mainImg = formData.value.images.find(img => img.isMain) || formData.value.images[0]
        const otherImgs = formData.value.images.filter(img => img !== mainImg)
        const imageUrls = [mainImg.url, ...otherImgs.map(img => img.url)].filter(Boolean)

        const payload = {
            productName: formData.value.name,
            basePrice: Number(formData.value.price),
            sku: formData.value.sku,
            description: formData.value.description || '',
            stock: Number(formData.value.stock),
            minStock: Number(formData.value.minStock || 0),
            supplier: formData.value.supplier || '',
            imageUrls: imageUrls // Backend needs to be updated to handle this
        }

        await productAPI.updateProducts(currentProduct.value.id, payload)

        // Refetch all products
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

        <div v-else>
          <!-- Mobile View (Cards) -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 p-4 lg:hidden">
            <div v-for="product in filteredProducts" :key="product.id" 
              class="bg-white rounded-2xl p-4 border border-slate-100 shadow-sm flex flex-col gap-4 relative overflow-hidden group">
              
              <!-- Selection & ID -->
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <input type="checkbox" :value="product.id" v-model="selectedProducts"
                    class="w-4 h-4 rounded-lg border-slate-300 text-blue-600 focus:ring-blue-500/20" />
                  <span class="font-mono text-[10px] font-black text-slate-400 bg-slate-100 px-2 py-1 rounded-lg">#{{ product.id }}</span>
                </div>
                <div class="flex gap-1">
                   <button @click="openUpdateModal(product)" class="p-2 text-slate-400 hover:text-blue-600 bg-slate-50 rounded-lg">
                      <Edit2 class="w-4 h-4" />
                   </button>
                   <button @click="handleDeleteProduct(product.id)" class="p-2 text-slate-400 hover:text-rose-600 bg-slate-50 rounded-lg">
                      <Trash2 class="w-4 h-4" />
                   </button>
                </div>
              </div>

              <!-- Content -->
              <div class="flex gap-4">
                <div class="w-20 h-20 rounded-xl bg-slate-50 flex items-center justify-center shrink-0 border border-slate-100">
                   <img v-if="product.images" :src="product.images" class="w-full h-full object-cover rounded-xl" />
                   <Package v-else class="w-8 h-8 text-slate-300" />
                </div>
                <div class="min-w-0 flex-1">
                   <p class="text-sm font-bold text-slate-900 line-clamp-2 leading-tight">{{ product.name }}</p>
                   <p class="text-[10px] font-bold text-slate-400 uppercase tracking-wider mt-1">{{ product.category }}</p>
                   <p class="text-[10px] font-mono text-slate-400">{{ product.sku || 'NO-SKU' }}</p>
                </div>
              </div>

              <!-- Stats -->
              <div class="grid grid-cols-2 gap-2">
                 <div class="bg-slate-50 p-3 rounded-xl border border-slate-100/50">
                    <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest">Price</p>
                    <p class="text-base font-black text-slate-900">${{ product.price.toFixed(2) }}</p>
                 </div>
                 <div class="bg-slate-50 p-3 rounded-xl border border-slate-100/50">
                    <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest">Stock</p>
                    <div class="flex items-center gap-2">
                       <span :class="`w-2 h-2 rounded-full ${getStockStatus(product).color === 'green' ? 'bg-emerald-500' : getStockStatus(product).color === 'orange' ? 'bg-amber-500' : 'bg-rose-500'}`"></span>
                       <p :class="`text-base font-black ${getStockStatus(product).color === 'green' ? 'text-emerald-700' : getStockStatus(product).color === 'orange' ? 'text-amber-700' : 'text-rose-700'}`">{{ product.stock }}</p>
                    </div>
                 </div>
              </div>
            </div>
          </div>

          <!-- Desktop View (Table) -->
          <div class="hidden lg:block overflow-x-auto px-4">
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
                    <div class="flex items-center justify-between mb-3 ml-1">
                      <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest group-focus-within:text-blue-600 transition-colors">Visual Assets</label>
                      <button type="button" @click="addImage" class="text-[10px] font-black text-blue-600 uppercase tracking-widest hover:underline">+ Add URL</button>
                    </div>
                    <div class="space-y-4">
                      <div v-for="(img, idx) in formData.images" :key="idx" 
                        class="flex items-center gap-4 group/img relative bg-slate-50 p-3 rounded-2xl border-2 border-transparent focus-within:border-blue-100 transition-all">
                        <div class="w-12 h-12 rounded-xl bg-white border border-slate-100 flex items-center justify-center overflow-hidden shrink-0">
                          <img v-if="img.url" :src="img.url" class="w-full h-full object-cover" />
                          <Plus v-else class="w-4 h-4 text-slate-300" />
                        </div>
                        <input v-model="img.url" type="text"
                          class="flex-1 bg-transparent border-none focus:ring-0 font-bold text-slate-700 placeholder:text-slate-300 text-xs py-2"
                          placeholder="Image URL" />
                        
                        <div class="flex items-center gap-2">
                          <label :for="'main-' + idx" 
                            class="flex items-center gap-2 cursor-pointer px-3 py-1.5 rounded-lg transition-all"
                            :class="img.isMain ? 'bg-blue-600 text-white shadow-lg shadow-blue-200' : 'bg-white text-slate-400 hover:bg-slate-100'">
                            <input type="radio" :id="'main-' + idx" name="main-image" :checked="img.isMain" @change="setMainImage(idx)" class="hidden" />
                            <span class="text-[9px] font-black uppercase tracking-tighter">{{ img.isMain ? 'Main' : 'Set Main' }}</span>
                          </label>
                          <button @click="removeImage(idx)" v-if="formData.images.length > 1"
                            class="p-2 text-slate-300 hover:text-rose-500 transition-colors">
                            <X class="w-4 h-4" />
                          </button>
                        </div>
                      </div>
                    </div>
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

                <!-- Variants Section -->
                <div v-if="showAddModal" class="md:col-span-2 space-y-12 mt-8 border-t border-slate-100/50 pt-12">
                  <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-6">
                    <div>
                      <h4 class="text-2xl font-black text-slate-900 tracking-tight flex items-center gap-3">
                        <Plus class="w-6 h-6 text-blue-500" />
                        Product Variants
                      </h4>
                      <p class="text-xs font-bold text-slate-400 uppercase tracking-widest mt-1.5 ml-9">Configure multiple versions (Color, Size, etc.)</p>
                    </div>
                    <button type="button" @click="addVariant"
                      class="flex items-center gap-3 px-8 py-4 bg-slate-900 text-white rounded-2xl hover:bg-slate-800 transition-all font-black text-xs uppercase tracking-widest shadow-xl shadow-slate-200 active:scale-95">
                      <Plus class="w-5 h-5" />
                      Init New Variant
                    </button>
                  </div>

                  <div v-if="formData.variants.length > 0" class="grid grid-cols-1 gap-8">
                    <TransitionGroup name="list">
                      <div v-for="(variant, vIdx) in formData.variants" :key="vIdx" 
                        class="bg-white rounded-[40px] border-2 border-slate-100/50 relative group/variant hover:border-blue-100 hover:shadow-2xl hover:shadow-blue-50/50 transition-all duration-500 overflow-hidden">
                        
                        <!-- Header / Toggle -->
                        <div @click="toggleVariantCollapse(vIdx)" 
                          class="p-8 cursor-pointer flex items-center justify-between hover:bg-slate-50 transition-colors">
                          <div class="flex items-center gap-5">
                            <div class="w-10 h-10 bg-slate-900 rounded-xl flex items-center justify-center text-white text-xs font-black shrink-0">
                              {{ vIdx + 1 }}
                            </div>
                            <div>
                              <h5 class="text-sm font-black text-slate-900 uppercase tracking-widest leading-none">
                                {{ variant.sku || 'N/A' }} 
                                <span class="text-blue-500 ml-2">{{ variant.price ? '$' + variant.price : '' }}</span>
                              </h5>
                              <div v-if="variant.isCollapsed" class="flex flex-wrap gap-2 mt-2">
                                <span v-for="opt in variant.options" :key="opt.variant" 
                                  class="px-2 py-0.5 bg-slate-100 text-[9px] font-bold text-slate-500 rounded uppercase">
                                  {{ opt.variant }}: {{ opt.value || '?' }}
                                </span>
                              </div>
                              <p v-else class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mt-1">Expanding Configuration</p>
                            </div>
                          </div>
                          
                          <div class="flex items-center gap-3">
                            <button @click.stop="removeVariant(vIdx)" 
                              class="p-2.5 text-slate-300 hover:text-rose-500 hover:bg-rose-50 rounded-xl transition-all mr-2">
                              <Trash2 class="w-4 h-4" />
                            </button>
                            <div class="p-2.5 rounded-xl border border-slate-100 text-slate-400 transition-transform duration-300"
                              :class="{ 'rotate-180': !variant.isCollapsed }">
                              <ChevronDown class="w-4 h-4" />
                            </div>
                          </div>
                        </div>

                        <div v-if="!variant.isCollapsed" class="p-10 pt-0 relative z-10">
                          <!-- Background Accent -->
                          <div class="absolute top-0 right-0 w-32 h-32 bg-blue-50/50 rounded-bl-[100px] -mr-16 -mt-16 group-hover/variant:scale-150 transition-transform duration-700 pointer-events-none"></div>

                          <div class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-12 mt-4">
                            <div class="space-y-3">
                              <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest ml-1">Unique SKU</label>
                              <div class="relative group/input">
                                <input v-model="variant.sku" type="text"
                                  class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-mono font-bold text-slate-700 outline-none uppercase text-sm tracking-wider"
                                  placeholder="SKU-CODE" />
                              </div>
                            </div>
                            <div class="space-y-3">
                              <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest ml-1">Price Adjust ($)</label>
                              <div class="relative">
                                <span class="absolute left-6 top-1/2 -translate-y-1/2 font-black text-slate-300">$</span>
                                <input v-model.number="variant.price" type="number" step="0.01"
                                  class="w-full pl-12 pr-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-black text-slate-900 outline-none" />
                              </div>
                            </div>
                            <div class="space-y-3">
                              <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest ml-1">Available Qty</label>
                              <input v-model.number="variant.stock" type="number"
                                class="w-full px-6 py-4.5 bg-slate-50 border-2 border-transparent rounded-[20px] focus:bg-white focus:border-blue-100 transition-all font-black text-slate-900 outline-none" />
                            </div>
                          </div>

                          <div class="space-y-6">
                            <div class="flex items-center justify-between px-1">
                              <div>
                                <p class="text-[11px] font-black text-slate-900 uppercase tracking-[0.2em]">Attributes</p>
                                <p class="text-[10px] font-bold text-slate-400 mt-1">Define properties like Color or Size</p>
                              </div>
                              <button @click="addOption(vIdx)" 
                                class="flex items-center gap-2 px-5 py-2.5 bg-blue-50 text-blue-600 rounded-xl hover:bg-blue-100 transition-all font-black text-[10px] uppercase tracking-widest active:scale-95">
                                <Plus class="w-3.5 h-3.5" />
                                Add Attribute
                              </button>
                            </div>
                            
                            <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
                              <div v-for="(opt, oIdx) in variant.options" :key="oIdx" 
                                class="flex gap-4 items-end bg-white border border-slate-100 p-5 rounded-[28px] shadow-sm hover:shadow-md transition-all">
                                <div class="flex-1 space-y-2">
                                  <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest ml-1">Attribute Type</p>
                                  <select v-model="opt.variant"
                                    class="w-full px-4 py-3 bg-slate-50 border-2 border-transparent rounded-xl focus:bg-white focus:border-blue-50 transition-all font-bold text-slate-700 text-xs outline-none cursor-pointer">
                                    <option value="" disabled>Select Type</option>
                                    <option value="Color">Color</option>
                                    <option value="Size">Size</option>
                                    <option value="Material">Material</option>
                                    <option value="Style">Style</option>
                                    <option value="Weight">Weight</option>
                                    <option value="Dimensions">Dimensions</option>
                                  </select>
                                </div>
                                <div class="flex-1 space-y-2">
                                  <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest ml-1">Value</p>
                                  <input v-model="opt.value" type="text"
                                    class="w-full px-4 py-3 bg-slate-50 border-2 border-transparent rounded-xl focus:bg-white focus:border-blue-50 transition-all font-bold text-slate-700 text-xs outline-none"
                                    placeholder="e.g. Matte Black" />
                                </div>
                                <button @click="removeOption(vIdx, oIdx)" 
                                  class="p-3.5 text-slate-300 hover:text-rose-500 hover:bg-rose-50 rounded-xl transition-all">
                                  <X class="w-4 h-4" />
                                </button>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </TransitionGroup>
                  </div>
                  
                  <div v-else class="py-20 border-4 border-dashed border-slate-50 rounded-[48px] text-center flex flex-col items-center group cursor-pointer hover:border-blue-100 hover:bg-blue-50/10 transition-all" @click="addVariant">
                    <div class="w-20 h-20 bg-slate-50 rounded-[28px] flex items-center justify-center mb-6 group-hover:scale-110 transition-transform group-hover:rotate-12">
                      <Package class="w-10 h-10 text-slate-200 group-hover:text-blue-200" />
                    </div>
                    <p class="text-xl font-black text-slate-900 tracking-tight">No variants defined</p>
                    <p class="text-sm font-bold text-slate-400 mt-2 max-w-xs mx-auto">Click "Init New Variant" to start building your product matrix.</p>
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
            <div class="relative h-96 bg-slate-900 shrink-0 overflow-hidden">
              <div v-if="currentProduct.imageList && currentProduct.imageList.length > 0" class="absolute inset-0">
                <img :src="currentProduct.selectedViewImage || (currentProduct.imageList.find(img => img.isMain) || currentProduct.imageList[0]).url" 
                  class="w-full h-full object-cover opacity-80 transition-all duration-700" />
                <div class="absolute inset-0 bg-gradient-to-t from-slate-900 via-slate-900/20 to-transparent"></div>
              </div>

              <!-- Thumbnails Overlay -->
              <div class="absolute bottom-10 left-10 right-10 flex flex-col gap-6">
                 <div class="flex flex-wrap gap-3 overflow-x-auto pb-4 scrollbar-hide">
                    <div v-for="(img, idx) in currentProduct.imageList" :key="idx" 
                      @click="currentProduct.selectedViewImage = img.url"
                      class="w-16 h-16 rounded-2xl border-2 cursor-pointer transition-all overflow-hidden shrink-0 shadow-2xl"
                      :class="currentProduct.selectedViewImage === img.url || (!currentProduct.selectedViewImage && img.isMain) ? 'border-blue-500 scale-110' : 'border-white/20 hover:border-white/60'">
                      <img :src="img.url" class="w-full h-full object-cover" />
                    </div>
                 </div>

                 <div class="flex justify-between items-end">
                  <div>
                    <span class="px-3 py-1 bg-blue-600 text-white rounded-lg text-[9px] font-black uppercase tracking-widest mb-3 inline-block shadow-lg shadow-blue-500/30">
                      {{ currentProduct.category }}
                    </span>
                    <h3 class="text-4xl font-black text-white tracking-tight leading-none drop-shadow-lg">{{ currentProduct.name }}</h3>
                  </div>
                  <div class="text-right">
                    <p class="text-xs font-bold text-slate-300 uppercase tracking-widest drop-shadow-md">Pricing</p>
                    <p class="text-3xl font-black text-white mt-1 tracking-tight drop-shadow-lg">${{ currentProduct.price.toFixed(2) }}</p>
                  </div>
                </div>
              </div>

              <button @click="closeModals" class="absolute top-8 right-8 p-3 bg-white/10 hover:bg-white/20 text-white rounded-2xl backdrop-blur-md transition-all z-10">
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

              <div v-if="currentProduct.variants && currentProduct.variants.length > 0" class="mt-12 space-y-6">
                <div class="flex items-center justify-between px-1">
                  <p class="text-[11px] font-black text-slate-900 uppercase tracking-[0.2em]">Live Variants</p>
                  <span class="px-3 py-1 bg-blue-50 text-blue-600 rounded-lg text-[9px] font-black uppercase tracking-widest">
                    {{ currentProduct.variants.length }} Versions
                  </span>
                </div>
                
                <div class="grid grid-cols-1 gap-4">
                  <div v-for="variant in currentProduct.variants" :key="variant.productVariantId" 
                    class="bg-white border-2 border-slate-50 p-6 rounded-[32px] hover:border-blue-100 transition-all group/vitem">
                    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6">
                      <div class="flex items-center gap-5">
                        <div class="w-12 h-12 bg-slate-50 rounded-2xl flex items-center justify-center border border-slate-100 group-hover/vitem:bg-blue-50 group-hover/vitem:border-blue-100 transition-colors">
                          <Package class="w-5 h-5 text-slate-400 group-hover/vitem:text-blue-500" />
                        </div>
                        <div>
                          <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest leading-none mb-1.5">SKU: {{ variant.sku }}</p>
                          <div class="flex flex-wrap gap-2">
                            <div v-for="opt in variant.options" :key="opt.variant" 
                              class="px-3 py-1.5 bg-slate-50 border border-slate-100 rounded-xl text-[10px] font-bold text-slate-600 flex items-center gap-2">
                              <span class="text-slate-400 uppercase tracking-tighter">{{ opt.variant }}:</span>
                              <span class="text-slate-900">{{ opt.value }}</span>
                            </div>
                          </div>
                        </div>
                      </div>
                      <div class="flex items-center gap-4 bg-slate-50/50 p-2 rounded-[20px] border border-slate-50">
                        <div class="px-5 py-3 bg-white rounded-2xl shadow-sm border border-slate-100 text-center min-w-[100px]">
                          <p class="text-[8px] font-black text-slate-400 uppercase tracking-widest mb-0.5">Price</p>
                          <p class="text-sm font-black text-slate-900">${{ variant.price.toFixed(2) }}</p>
                        </div>
                        <div class="px-5 py-3 bg-white rounded-2xl shadow-sm border border-slate-100 text-center min-w-[100px]">
                          <p class="text-[8px] font-black text-slate-400 uppercase tracking-widest mb-0.5">Global Stock</p>
                          <p class="text-sm font-black text-blue-600">{{ variant.stockQuantity }} <span class="text-[9px] font-bold text-slate-400">Qty</span></p>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="space-y-4 mt-8">
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

.list-enter-active,
.list-leave-active {
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
}

.list-enter-from {
  opacity: 0;
  transform: translateY(30px) scale(0.95);
}

.list-leave-to {
  opacity: 0;
  transform: scale(0.9);
}

.custom-scrollbar::-webkit-scrollbar {
  width: 8px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #cbd5e1;
}
</style>