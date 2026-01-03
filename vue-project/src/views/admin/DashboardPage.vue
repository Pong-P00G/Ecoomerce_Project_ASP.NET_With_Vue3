<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  Users,
  Package,
  TrendingUp,
  AlertCircle,
  ArrowRight,
  Clock,
  ExternalLink,
  DollarSign,
  Activity,
  RefreshCw,
  AlertTriangle
} from 'lucide-vue-next'
import {useToast as $router} from "../../composables/useToast.js";

const loading = ref(false)
const error = ref(null)
const refreshing = ref(false)

// Data from backend
const users = ref([])
const products = ref([])
const dashboardData = ref(null)
const recentActivities = ref([])

// Preview state
const selectedItem = ref(null)
const previewType = ref('product') // 'product' or 'user'

// Format date helper
const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}

// Format relative time helper
const formatRelativeTime = (dateString) => {
  if (!dateString) return 'Unknown time'
  const date = new Date(dateString)
  const now = new Date()
  const diffInSeconds = Math.floor((now - date) / 1000)

  if (diffInSeconds < 60) return `${diffInSeconds} seconds ago`
  if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} minutes ago`
  if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hours ago`
  return `${Math.floor(diffInSeconds / 86400)} days ago`
}

// Computed properties
const stats = computed(() => {
  const revenue = dashboardData.value?.totalRevenue ||
    dashboardData.value?.revenue ||
    products.value.reduce((acc, p) => acc + (p.price || 0), 0) * 0.3 // Fallback estimate

  return {
    totalUsers: dashboardData.value?.totalUsers || users.value.length,
    totalProducts: dashboardData.value?.totalProducts || products.value.length,
    lowStock: products.value.filter(p => (p.stock || 0) < 10 && (p.stock || 0) > 0).length,
    outOfStock: products.value.filter(p => (p.stock || 0) === 0).length,
    totalRevenue: revenue,
    revenueTrend: dashboardData.value?.revenueTrend || '+12.5%',
    usersTrend: dashboardData.value?.usersTrend || '+3.2%',
    productsTrend: dashboardData.value?.productsTrend || '+18'
  }
})

// Fetch dashboard data
const fetchDashboardData = async (showRefreshing = false) => {
  try {
    if (showRefreshing) {
      refreshing.value = true
    } else {
      loading.value = true
    }
    error.value = null

    const response = await dashboardApi.getAdminDashboard()
    dashboardData.value = response.data
    users.value = response.data.users || []
    products.value = response.data.products || []
    recentActivities.value = response.data.recentActivities || response.data.activities || []

    // Set initial preview
    if (products.value.length > 0) {
      selectedItem.value = products.value[0]
      previewType.value = 'product'
    } else if (users.value.length > 0) {
      selectedItem.value = users.value[0]
      previewType.value = 'user'
    } else {
      selectedItem.value = null
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'Failed to load dashboard data'
    console.error('Dashboard error:', err)

    // Fallback to sample data for preview if API fails
    if (products.value.length === 0 && users.value.length === 0) {
      products.value = [
        { id: 1, name: 'Premium Laptop Pro', category: 'Electronics', stock: 45, price: 1299, sku: 'LPT-001', description: 'High-performance laptop for professionals.' },
        { id: 2, name: 'Ergonomic Chair', category: 'Furniture', stock: 8, price: 299, sku: 'CHR-042', description: 'Stay comfortable during long work sessions.' }
      ]
      users.value = [
        { id: 1, name: 'John Doe', email: 'john@example.com', role: 'User', status: 'Active', joined: '2023-10-15' }
      ]
      recentActivities.value = [
        { id: 1, type: 'order', message: 'New order #1234 received', time: new Date().toISOString(), status: 'new' },
        { id: 2, type: 'user', message: 'New user registration: Sarah Connor', time: new Date(Date.now() - 15 * 60000).toISOString(), status: 'completed' },
        { id: 3, type: 'stock', message: 'Product "Wireless Mouse" is low in stock', time: new Date(Date.now() - 3600000).toISOString(), status: 'warning' },
        { id: 4, type: 'payment', message: 'Payment confirmed for order #1230', time: new Date(Date.now() - 3 * 3600000).toISOString(), status: 'completed' },
      ]
      if (products.value.length > 0) {
        selectedItem.value = products.value[0]
      }
    }
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

const refreshData = () => {
  fetchDashboardData(true)
}

const selectPreview = (item, type) => {
  selectedItem.value = item
  previewType.value = type
}

const clearActivities = () => {
  recentActivities.value = []
}

onMounted(() => {
  fetchDashboardData()
})
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8 max-w-1600px mx-auto">
    <!-- Error Banner -->
    <div v-if="error" class="mb-6 bg-red-50 border border-red-200 rounded-2xl p-4 flex items-center gap-3">
      <AlertTriangle class="w-5 h-5 text-red-600 shrink-0" />
      <div class="flex-1">
        <p class="text-sm font-semibold text-red-900">Error loading dashboard</p>
        <p class="text-xs text-red-700 mt-0.5">{{ error }}</p>
      </div>
      <button @click="fetchDashboardData()" class="text-sm font-medium text-red-700 hover:text-red-900 underline">
        Retry
      </button>
    </div>
    <div class="flex flex-col lg:flex-row gap-6 lg:gap-8">
      <!-- Main Dashboard Content -->
      <div class="flex-1 space-y-6 lg:space-y-8">
        <!-- Welcome Header -->
        <div class="flex flex-col sm:flex-row sm:items-end sm:justify-between gap-4">
          <div>
            <h2 class="text-2xl sm:text-3xl font-bold text-gray-900">Dashboard Overview</h2>
            <p class="text-gray-500 mt-1 text-sm sm:text-base">Welcome back! Here's what's happening today.</p>
          </div>
          <div class="flex items-center gap-3">
            <button @click="refreshData" :disabled="refreshing || loading"
              class="flex items-center gap-2 text-sm font-medium text-gray-600 bg-white border border-gray-200 px-4 py-2 rounded-full hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed">
              <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': refreshing }" />
              Refresh
            </button>
            <div class="flex items-center gap-2 text-sm font-medium text-blue-600 bg-blue-50 px-4 py-2 rounded-full">
              <Activity class="w-4 h-4" />
              Live Updates
            </div>
          </div>
        </div>
        <!-- Loading Skeleton -->
        <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-6">
          <div v-for="i in 4" :key="i" class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm animate-pulse">
            <div class="flex items-start justify-between">
              <div class="w-12 h-12 rounded-2xl bg-gray-200"></div>
              <div class="w-16 h-6 bg-gray-200 rounded-lg"></div>
            </div>
            <div class="mt-4 space-y-2">
              <div class="h-4 bg-gray-200 rounded w-24"></div>
              <div class="h-8 bg-gray-200 rounded w-32"></div>
            </div>
          </div>
        </div>
        <!-- Stats Grid -->
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-6">
          <div v-for="(stat, index) in [
            { label: 'Total Revenue', value: '$' + stats.totalRevenue.toLocaleString(), icon: DollarSign, color: 'blue', trend: stats.revenueTrend },
            { label: 'Active Users', value: stats.totalUsers, icon: Users, color: 'indigo', trend: stats.usersTrend },
            { label: 'Total Products', value: stats.totalProducts, icon: Package, color: 'purple', trend: stats.productsTrend },
            { label: 'Low Stock', value: stats.lowStock, icon: AlertCircle, color: 'amber', trend: stats.outOfStock + ' empty' }
          ]" :key="index"
            class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md transition-all duration-200 cursor-default">
            <div class="flex items-start justify-between">
              <div :class="`w-12 h-12 rounded-2xl flex items-center justify-center`"
                :style="{ backgroundColor: stat.color === 'blue' ? '#eff6ff' : stat.color === 'indigo' ? '#eef2ff' : stat.color === 'purple' ? '#f5f3ff' : '#fffbeb' }">
                <component :is="stat.icon" class="w-6 h-6"
                  :style="{ color: stat.color === 'blue' ? '#2563eb' : stat.color === 'indigo' ? '#4f46e5' : stat.color === 'purple' ? '#7c3aed' : '#d97706' }" />
              </div>
              <span
                :class="`text-xs font-bold px-2 py-1 rounded-lg ${stat.color === 'amber' ? 'bg-amber-50 text-amber-700' : 'bg-green-50 text-green-700'}`">
                {{ stat.trend }}
              </span>
            </div>
            <div class="mt-4">
              <p class="text-sm font-medium text-gray-500">{{ stat.label }}</p>
              <p class="text-2xl font-bold text-gray-900 mt-1">{{ stat.value }}</p>
            </div>
          </div>
        </div>
        <div class="grid grid-cols-1 xl:grid-cols-2 gap-6 lg:gap-8">
          <!-- Recent Products -->
          <div class="bg-white rounded-3xl border border-gray-100 shadow-sm overflow-hidden flex flex-col">
            <div class="p-6 border-b border-gray-50 flex items-center justify-between">
              <h3 class="font-bold text-gray-900 flex items-center gap-2">
                <Package class="w-5 h-5 text-blue-600" />
                Recent Products
              </h3>
              <router-link to="/admin/product"
                class="text-sm font-semibold text-blue-600 hover:underline flex items-center gap-1 transition-colors">
                View All
                <ArrowRight class="w-4 h-4" />
              </router-link>
            </div>
            <div class="p-2 grow overflow-y-auto max-h-500px">
              <div v-if="loading" class="space-y-1 p-2">
                <div v-for="i in 5" :key="i" class="flex items-center gap-4 p-3 rounded-2xl animate-pulse">
                  <div class="w-12 h-12 rounded-xl bg-gray-200"></div>
                  <div class="flex-1 space-y-2">
                    <div class="h-4 bg-gray-200 rounded w-3/4"></div>
                    <div class="h-3 bg-gray-200 rounded w-1/2"></div>
                  </div>
                  <div class="w-16 h-6 bg-gray-200 rounded"></div>
                </div>
              </div>
              <div v-else-if="products.length === 0" class="p-8 text-center">
                <Package class="w-12 h-12 text-gray-300 mx-auto mb-3" />
                <p class="text-sm text-gray-500">No products found</p>
              </div>
              <div v-else class="space-y-1">
                <div v-for="product in products.slice(0, 5)" :key="product.id"
                  @click="selectPreview(product, 'product')" :class="[
                    'flex items-center gap-4 p-3 rounded-2xl cursor-pointer transition-all',
                    selectedItem?.id === product.id && previewType === 'product' ? 'bg-blue-50 border-2 border-blue-200' : 'hover:bg-gray-50 border-2 border-transparent'
                  ]">
                  <div class="w-12 h-12 rounded-xl bg-gray-100 flex items-center justify-center text-gray-400 shrink-0">
                    <Package class="w-6 h-6" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="font-semibold text-gray-900 truncate">{{ product.name }}</p>
                    <p class="text-xs text-gray-500">{{ product.category || 'Uncategorized' }}</p>
                  </div>
                  <div class="text-right shrink-0">
                    <p class="font-bold text-gray-900">${{ (product.price || 0).toLocaleString() }}</p>
                    <p
                      :class="`text-[10px] font-bold uppercase ${(product.stock || 0) < 10 ? 'text-red-500' : 'text-green-500'}`">
                      {{ product.stock || 0 }} in stock
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- Recent Activity -->
          <div class="bg-white rounded-3xl border border-gray-100 shadow-sm overflow-hidden flex flex-col">
            <div class="p-6 border-b border-gray-50 flex items-center justify-between">
              <h3 class="font-bold text-gray-900 flex items-center gap-2">
                <Clock class="w-5 h-5 text-indigo-600" />
                Recent Activity
              </h3>
              <button v-if="recentActivities.length > 0" @click="clearActivities"
                class="text-sm font-semibold text-gray-400 hover:text-gray-600 transition-colors">
                Clear
              </button>
            </div>
            <div class="p-6 overflow-y-auto max-h-500px">
              <div v-if="loading" class="space-y-6">
                <div v-for="i in 4" :key="i" class="flex gap-4 animate-pulse">
                  <div class="w-10 h-10 rounded-full bg-gray-200 shrink-0"></div>
                  <div class="flex-1 space-y-2">
                    <div class="h-4 bg-gray-200 rounded w-3/4"></div>
                    <div class="h-3 bg-gray-200 rounded w-1/2"></div>
                  </div>
                </div>
              </div>
              <div v-else-if="recentActivities.length === 0" class="text-center py-8">
                <Clock class="w-12 h-12 text-gray-300 mx-auto mb-3" />
                <p class="text-sm text-gray-500">No recent activity</p>
              </div>
              <div v-else class="space-y-6">
                <div v-for="(activity, index) in recentActivities" :key="activity.id || index" class="flex gap-4">
                  <div class="relative">
                    <div :class="[
                      'w-10 h-10 rounded-full flex items-center justify-center shrink-0 transition-colors',
                      activity.status === 'new' ? 'bg-blue-100 text-blue-600' :
                        activity.status === 'warning' ? 'bg-amber-100 text-amber-600' : 'bg-gray-100 text-gray-600'
                    ]">
                      <Activity class="w-5 h-5" />
                    </div>
                    <div v-if="index < recentActivities.length - 1"
                      class="absolute top-8 left-1/2 -translate-x-1/2 w-0.5 h-6 bg-gray-100"></div>
                  </div>
                  <div class="flex-1 pt-0.5">
                    <p class="text-sm font-semibold text-gray-900">{{ activity.message }}</p>
                    <p class="text-xs text-gray-500 mt-0.5">{{ activity.time ? (activity.time.includes('ago') ?
                      activity.time : formatRelativeTime(activity.time)) : 'Unknown time' }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <!-- Right Side Preview Panel -->
      <div class="w-full lg:w-80 xl:w-96 shrink-0">
        <div class="sticky top-24 space-y-6">
          <div class="bg-white rounded-32px border border-gray-100 shadow-xl overflow-hidden">
            <!-- Preview Header -->
            <div class="h-32 bg-linear-to-br from-blue-600 to-indigo-700 relative">
              <div class="absolute -bottom-8 left-8 w-24 h-24 rounded-3xl bg-white shadow-lg p-1">
                <div class="w-full h-full rounded-2xl bg-gray-50 flex items-center justify-center">
                  <component :is="previewType === 'product' ? Package : Users" class="w-10 h-10 text-gray-300" />
                </div>
              </div>
            </div>
            <div class="pt-16 p-6 lg:p-8">
              <div v-if="!selectedItem" class="text-center py-8">
                <Package class="w-12 h-12 text-gray-300 mx-auto mb-3" />
                <p class="text-sm text-gray-500">Select an item to view details</p>
              </div>
              <div v-else>
                <div class="flex items-start justify-between">
                  <div class="flex-1 min-w-0">
                    <h4 class="text-xl lg:text-2xl font-bold text-gray-900 truncate">{{ selectedItem?.name || 'Select anyItem' }}</h4>
                    <p class="text-gray-500 mt-1 flex items-center gap-2 flex-wrap">
                      <span v-if="previewType === 'product'"
                        class="bg-blue-50 text-blue-700 text-[10px] font-bold px-2 py-0.5 rounded uppercase">
                        {{ selectedItem?.category || 'Uncategorized' }}
                      </span>
                      <span v-else
                        class="bg-purple-50 text-purple-700 text-[10px] font-bold px-2 py-0.5 rounded uppercase">
                        {{ selectedItem?.role || 'User' }}
                      </span>
                      <span class="text-gray-300">•</span>
                      <span class="text-xs font-medium truncate">{{ previewType === 'product' ? (selectedItem?.sku || 'N/A') : (selectedItem?.email || 'N/A') }}</span>
                    </p>
                  </div>
                  <button class="p-2 hover:bg-gray-100 rounded-xl transition-colors shrink-0 ml-2">
                    <ExternalLink class="w-5 h-5 text-gray-400" />
                  </button>
                </div>
                <!-- Details -->
                <div class="mt-6 lg:mt-8 space-y-6">
                  <div v-if="previewType === 'product'" class="grid grid-cols-2 gap-4">
                    <div class="bg-gray-50 p-4 rounded-2xl">
                      <p class="text-[10px] font-bold text-gray-400 uppercase tracking-wider">Price</p>
                      <p class="text-lg font-bold text-gray-900 mt-1">${{ (selectedItem?.price || 0).toLocaleString() }}
                      </p>
                    </div>
                    <div class="bg-gray-50 p-4 rounded-2xl">
                      <p class="text-[10px] font-bold text-gray-400 uppercase tracking-wider">Inventory</p>
                      <p
                        :class="['text-lg font-bold mt-1', (selectedItem?.stock || 0) < 10 ? 'text-red-500' : 'text-green-600']">
                        {{ selectedItem?.stock || 0 }} units
                      </p>
                    </div>
                  </div>
                  <div v-if="previewType === 'user'" class="space-y-4">
                    <div class="flex items-center justify-between p-4 bg-gray-50 rounded-2xl">
                      <span class="text-sm font-medium text-gray-500">Status</span>
                      <span :class="[
                        'px-3 py-1 rounded-full text-xs font-bold',
                        selectedItem?.status === 'Active' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                      ]">
                        {{ selectedItem?.status || 'Unknown' }}
                      </span>
                    </div>
                    <div class="flex items-center justify-between p-4 bg-gray-50 rounded-2xl">
                      <span class="text-sm font-medium text-gray-500">Joined</span>
                      <span class="text-sm font-bold text-gray-900">{{ formatDate(selectedItem?.joined) }}</span>
                    </div>
                  </div>
                  <div>
                    <h5 class="text-sm font-bold text-gray-900 mb-3">Quick Actions</h5>
                    <div class="grid grid-cols-1 gap-2">
                      <button
                        @click="previewType === 'product' ? $router.push(`/admin/product?edit=${selectedItem?.id}`) : $router.push(`/admin/user?edit=${selectedItem?.id}`)"
                        class="w-full flex items-center justify-center gap-2 py-3 bg-gray-900 text-white rounded-2xl font-bold hover:bg-gray-800 transition-colors">
                        Edit {{ previewType === 'product' ? 'Product' : 'User' }}
                      </button>
                      <button
                        class="w-full py-3 bg-white border border-gray-200 text-gray-700 rounded-2xl font-bold hover:bg-gray-50 transition-colors">
                        {{ previewType === 'product' ? 'View Inventory' : 'Reset Password' }}
                      </button>
                    </div>
                  </div>
                  <div v-if="previewType === 'product' && selectedItem?.description"
                    class="pt-4 border-t border-gray-100">
                    <p class="text-sm text-gray-500 leading-relaxed italic">
                      "{{ selectedItem.description }}"
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- Quick Stats Mini Card -->
          <div class="bg-linear-to-br from-indigo-600 to-purple-700 p-6 lg:p-8 rounded-32px text-white shadow-lg relative overflow-hidden">
            <TrendingUp class="absolute -right-4 -bottom-4 w-32 h-32 text-white/10" />
            <h5 class="font-bold text-indigo-100 text-sm lg:text-base">Monthly Growth</h5>
            <div class="mt-4 flex items-end gap-2">
              <p class="text-3xl lg:text-4xl font-bold">{{ dashboardData?.monthlyGrowth || '+24%' }}</p>
              <p class="text-indigo-200 text-xs lg:text-sm mb-1 pb-1">since last month</p>
            </div>
            <div class="mt-6 flex gap-1 h-16">
              <div v-for="(h, index) in (dashboardData?.growthChart || [40, 70, 45, 90, 65, 80, 100])" :key="index"
                 class="flex-1 bg-white/20 rounded-t-sm transition-all duration-300 hover:bg-white/30"
                :style="{ height: h + '%' }">
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Custom Scrollbar */
::-webkit-scrollbar {
  width: 4px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: #cbd5e1;
}
</style>
