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
import { dashboardApi } from '../../api/dashboardApi.js';
import { useRouter } from 'vue-router'

const router = useRouter()

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
    
    // Map users to ensure consistent property access
    users.value = (response.data.users || []).map(u => ({
      ...u,
      id: u.userId,
      role: u.role || 'User'
    }))

    // Map products to flat structure expected by template
    products.value = (response.data.products || []).map(p => ({
      ...p,
      id: p.productId,
      name: p.productName,
      price: p.basePrice,
      category: p.categories?.[0]?.categoryName || 'General' // Flatten category
    }))

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
  <div class="min-h-screen bg-slate-50/50 p-4 sm:p-6 lg:p-8">
    <div class="max-w-[1600px] mx-auto">
      <!-- Error Banner -->
      <Transition name="slide-down">
        <div v-if="error" class="mb-8 bg-rose-50 border border-rose-100 rounded-3xl p-5 flex items-center gap-4 shadow-sm">
          <div class="p-3 bg-white rounded-2xl shadow-sm">
            <AlertTriangle class="w-6 h-6 text-rose-500 shrink-0" />
          </div>
          <div class="flex-1">
            <p class="text-sm font-bold text-slate-900">System Notification</p>
            <p class="text-xs text-slate-500 mt-0.5">{{ error }}</p>
          </div>
          <button @click="fetchDashboardData()" class="px-5 py-2.5 bg-slate-900 text-white rounded-xl hover:bg-slate-800 transition-all text-sm font-bold shadow-sm">
            Retry
          </button>
        </div>
      </Transition>

      <div class="flex flex-col lg:flex-row gap-8">
        <!-- Main Dashboard Content -->
        <div class="flex-1 space-y-8">
          <!-- Welcome Header -->
          <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
            <div>
              <h2 class="text-3xl font-extrabold text-slate-900 tracking-tight">Dashboard</h2>
              <p class="text-slate-500 mt-1 font-medium">Monitoring your store's performance in real-time.</p>
            </div>
            <div class="flex items-center gap-3">
              <button @click="refreshData" :disabled="refreshing || loading"
                class="flex items-center gap-2 text-sm font-bold text-slate-700 bg-white border border-slate-200 px-5 py-2.5 rounded-2xl hover:bg-slate-50 transition-all shadow-sm active:scale-95 disabled:opacity-50">
                <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': refreshing }" />
                Sync Data
              </button>
              <div class="flex items-center gap-2 text-sm font-bold text-emerald-600 bg-emerald-50 px-5 py-2.5 rounded-2xl border border-emerald-100/50">
                <div class="w-2 h-2 rounded-full bg-emerald-500 animate-pulse"></div>
                Live
              </div>
            </div>
          </div>

          <!-- Stats Grid -->
          <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-6">
            <div v-for="i in 4" :key="i" class="bg-white p-6 rounded-[32px] border border-slate-100 shadow-sm animate-pulse">
              <div class="flex items-start justify-between mb-4">
                <div class="w-12 h-12 rounded-2xl bg-slate-100"></div>
                <div class="w-16 h-6 bg-slate-100 rounded-lg"></div>
              </div>
              <div class="space-y-3">
                <div class="h-4 bg-slate-100 rounded w-24"></div>
                <div class="h-8 bg-slate-100 rounded w-32"></div>
              </div>
            </div>
          </div>
          <div v-else class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-6">
            <div v-for="(stat, index) in [
              { label: 'Total Revenue', value: '$' + stats.totalRevenue.toLocaleString(), icon: DollarSign, color: 'blue', trend: stats.revenueTrend, bg: 'bg-blue-50', text: 'text-blue-600' },
              { label: 'Active Users', value: stats.totalUsers, icon: Users, color: 'indigo', trend: stats.usersTrend, bg: 'bg-indigo-50', text: 'text-indigo-600' },
              { label: 'Total Products', value: stats.totalProducts, icon: Package, color: 'purple', trend: stats.productsTrend, bg: 'bg-purple-50', text: 'text-purple-600' },
              { label: 'Stock Alerts', value: stats.lowStock, icon: AlertCircle, color: 'amber', trend: stats.outOfStock + ' out', bg: 'bg-amber-50', text: 'text-amber-600' }
            ]" :key="index"
              class="group bg-white p-6 rounded-[32px] border border-slate-100 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300">
              <div class="flex items-start justify-between">
                <div :class="`w-14 h-14 rounded-2xl flex items-center justify-center transition-colors ${stat.bg}`">
                  <component :is="stat.icon" :class="`w-7 h-7 ${stat.text}`" />
                </div>
                <div :class="`px-3 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-wider ${stat.color === 'amber' ? 'bg-rose-50 text-rose-600' : 'bg-emerald-50 text-emerald-600'}`">
                  {{ stat.trend }}
                </div>
              </div>
              <div class="mt-6">
                <p class="text-sm font-bold text-slate-400 uppercase tracking-widest">{{ stat.label }}</p>
                <p class="text-3xl font-black text-slate-900 mt-1 tracking-tight">{{ stat.value }}</p>
              </div>
            </div>
          </div>

          <div class="grid grid-cols-1 xl:grid-cols-2 gap-8">
            <!-- Recent Products -->
            <div class="bg-white rounded-[32px] border border-slate-100 shadow-sm overflow-hidden flex flex-col">
              <div class="p-8 border-b border-slate-50 flex items-center justify-between">
                <div>
                  <h3 class="font-black text-slate-900 text-lg flex items-center gap-3">
                    <div class="p-2 bg-blue-50 rounded-lg">
                      <Package class="w-5 h-5 text-blue-600" />
                    </div>
                    Recent Products
                  </h3>
                </div>
                <router-link to="/admin/product"
                  class="group text-sm font-bold text-blue-600 hover:text-blue-700 flex items-center gap-2 transition-colors">
                  View Inventory
                  <ArrowRight class="w-4 h-4 transition-transform group-hover:translate-x-1" />
                </router-link>
              </div>
              <div class="p-4 grow overflow-y-auto max-h-[500px]">
                <div v-if="loading" class="space-y-2">
                  <div v-for="i in 5" :key="i" class="flex items-center gap-4 p-4 rounded-2xl animate-pulse">
                    <div class="w-14 h-14 rounded-2xl bg-slate-100"></div>
                    <div class="flex-1 space-y-3">
                      <div class="h-4 bg-slate-100 rounded w-3/4"></div>
                      <div class="h-3 bg-slate-100 rounded w-1/2"></div>
                    </div>
                  </div>
                </div>
                <div v-else-if="products.length === 0" class="py-20 text-center">
                  <div class="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center mx-auto mb-4">
                    <Package class="w-10 h-10 text-slate-200" />
                  </div>
                  <p class="text-slate-400 font-bold">No products available</p>
                </div>
                <div v-else class="space-y-2">
                  <div v-for="product in products.slice(0, 5)" :key="product.id"
                    @click="selectPreview(product, 'product')" :class="[
                      'flex items-center gap-4 p-4 rounded-2xl cursor-pointer transition-all duration-200 border-2',
                      selectedItem?.id === product.id && previewType === 'product' ? 'bg-blue-50/50 border-blue-100 shadow-sm' : 'hover:bg-slate-50 border-transparent'
                    ]">
                    <div class="w-14 h-14 rounded-2xl bg-white shadow-sm flex items-center justify-center text-slate-300 shrink-0 border border-slate-100">
                      <Package class="w-7 h-7" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <p class="font-bold text-slate-900 truncate">{{ product.name }}</p>
                      <p class="text-xs font-bold text-slate-400 uppercase tracking-tighter mt-0.5">{{ product.category || 'General' }}</p>
                    </div>
                    <div class="text-right shrink-0">
                      <p class="font-black text-slate-900">${{ (product.price || 0).toLocaleString() }}</p>
                      <div :class="`inline-flex items-center mt-1 px-2 py-0.5 rounded-lg text-[10px] font-black uppercase tracking-tighter ${(product.stock || 0) < 10 ? 'bg-rose-50 text-rose-600' : 'bg-emerald-50 text-emerald-600'}`">
                        {{ product.stock || 0 }} Unit
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Recent Activity -->
            <div class="bg-white rounded-[32px] border border-slate-100 shadow-sm overflow-hidden flex flex-col">
              <div class="p-8 border-b border-slate-50 flex items-center justify-between">
                <h3 class="font-black text-slate-900 text-lg flex items-center gap-3">
                  <div class="p-2 bg-indigo-50 rounded-lg">
                    <Clock class="w-5 h-5 text-indigo-600" />
                  </div>
                  System Logs
                </h3>
                <button v-if="recentActivities.length > 0" @click="clearActivities"
                  class="text-xs font-black text-slate-400 hover:text-rose-500 uppercase tracking-widest transition-colors">
                  Clear Logs
                </button>
              </div>
              <div class="p-8 overflow-y-auto max-h-[500px]">
                <div v-if="loading" class="space-y-8">
                  <div v-for="i in 4" :key="i" class="flex gap-6 animate-pulse">
                    <div class="w-12 h-12 rounded-full bg-slate-100 shrink-0"></div>
                    <div class="flex-1 space-y-3">
                      <div class="h-4 bg-slate-100 rounded w-3/4"></div>
                      <div class="h-3 bg-slate-100 rounded w-1/4"></div>
                    </div>
                  </div>
                </div>
                <div v-else-if="recentActivities.length === 0" class="py-20 text-center">
                  <div class="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center mx-auto mb-4">
                    <Activity class="w-10 h-10 text-slate-200" />
                  </div>
                  <p class="text-slate-400 font-bold">No recent activities</p>
                </div>
                <div v-else class="space-y-8">
                  <div v-for="(activity, index) in recentActivities" :key="activity.id || index" class="flex gap-6 relative">
                    <div v-if="index < recentActivities.length - 1"
                      class="absolute top-12 left-6 -translate-x-1/2 w-px h-8 bg-slate-100"></div>
                    <div class="relative shrink-0">
                      <div :class="[
                        'w-12 h-12 rounded-2xl flex items-center justify-center shadow-sm border border-white',
                        activity.status === 'new' ? 'bg-blue-500 text-white' :
                          activity.status === 'warning' ? 'bg-amber-500 text-white' : 'bg-slate-900 text-white'
                      ]">
                        <component :is="activity.type === 'order' ? DollarSign : activity.type === 'user' ? Users : activity.type === 'stock' ? AlertTriangle : Activity" class="w-5 h-5" />
                      </div>
                    </div>
                    <div class="flex-1 pt-1">
                      <p class="text-sm font-bold text-slate-900 leading-snug">{{ activity.message }}</p>
                      <p class="text-xs font-bold text-slate-400 uppercase tracking-widest mt-1.5 flex items-center gap-2">
                        <Clock class="w-3 h-3" />
                        {{ activity.time ? (activity.time.includes('ago') ? activity.time : formatRelativeTime(activity.time)) : 'Now' }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Side Preview Panel -->
        <div class="w-full lg:w-96 shrink-0">
          <div class="sticky top-8 space-y-8">
            <div class="bg-white rounded-[40px] border border-slate-100 shadow-xl shadow-slate-200/50 overflow-hidden group">
              <!-- Preview Header -->
              <div class="h-40 bg-slate-900 relative overflow-hidden">
                <div class="absolute inset-0 bg-gradient-to-br from-blue-600/20 to-indigo-600/20"></div>
                <div class="absolute -bottom-10 -right-10 w-40 h-40 bg-white/5 rounded-full blur-3xl"></div>
                <div class="absolute -bottom-12 left-10 w-32 h-32 rounded-[32px] bg-white shadow-2xl p-1.5 transition-transform group-hover:scale-105 duration-500">
                  <div class="w-full h-full rounded-[24px] bg-slate-50 flex items-center justify-center">
                    <component :is="previewType === 'product' ? Package : Users" class="w-12 h-12 text-slate-200" />
                  </div>
                </div>
              </div>
              <div class="pt-24 p-10">
                <div v-if="!selectedItem" class="text-center py-10">
                  <p class="text-slate-400 font-bold">Select an item to preview</p>
                </div>
                <div v-else>
                  <div class="flex items-start justify-between">
                    <div class="flex-1 min-w-0">
                      <h4 class="text-2xl font-black text-slate-900 truncate tracking-tight">{{ selectedItem?.name || 'Unknown' }}</h4>
                      <div class="flex items-center gap-2 mt-2">
                        <span :class="`px-2.5 py-1 rounded-lg text-[10px] font-black uppercase tracking-widest ${previewType === 'product' ? 'bg-blue-50 text-blue-600' : 'bg-indigo-50 text-indigo-600'}`">
                          {{ previewType === 'product' ? (selectedItem?.category || 'General') : (selectedItem?.role || 'User') }}
                        </span>
                        <span class="text-slate-300 font-light">|</span>
                        <span class="text-xs font-bold text-slate-400 truncate">{{ previewType === 'product' ? (selectedItem?.sku || 'NO-SKU') : (selectedItem?.email || 'no-email') }}</span>
                      </div>
                    </div>
                  </div>

                  <!-- Details -->
                  <div class="mt-10 space-y-8">
                    <div v-if="previewType === 'product'" class="grid grid-cols-2 gap-4">
                      <div class="bg-slate-50 p-5 rounded-3xl border border-slate-100/50">
                        <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Pricing</p>
                        <p class="text-xl font-black text-slate-900 mt-1">${{ (selectedItem?.price || 0).toLocaleString() }}</p>
                      </div>
                      <div class="bg-slate-50 p-5 rounded-3xl border border-slate-100/50">
                        <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">In Stock</p>
                        <p :class="['text-xl font-black mt-1', (selectedItem?.stock || 0) < 10 ? 'text-rose-500' : 'text-emerald-600']">
                          {{ selectedItem?.stock || 0 }} <span class="text-xs uppercase">Qty</span>
                        </p>
                      </div>
                    </div>

                    <div v-if="previewType === 'user'" class="space-y-4">
                      <div class="flex items-center justify-between p-5 bg-slate-50 rounded-3xl border border-slate-100/50">
                        <span class="text-xs font-black text-slate-400 uppercase tracking-widest">Account Status</span>
                        <span :class="`px-3 py-1 rounded-xl text-[10px] font-black uppercase tracking-widest ${selectedItem?.status === 'Active' ? 'bg-emerald-50 text-emerald-600' : 'bg-slate-100 text-slate-500'}`">
                          {{ selectedItem?.status || 'Inactive' }}
                        </span>
                      </div>
                      <div class="flex items-center justify-between p-5 bg-slate-50 rounded-3xl border border-slate-100/50">
                        <span class="text-xs font-black text-slate-400 uppercase tracking-widest">Member Since</span>
                        <span class="text-sm font-bold text-slate-900">{{ formatDate(selectedItem?.joined) }}</span>
                      </div>
                    </div>

                    <div class="space-y-3">
                      <button
                        @click="previewType === 'product' ? router.push(`/admin/product?edit=${selectedItem?.id}`) : router.push(`/admin/user?edit=${selectedItem?.id}`)"
                        class="w-full py-4 bg-slate-900 text-white rounded-[20px] font-black uppercase tracking-widest text-xs hover:bg-slate-800 transition-all shadow-lg shadow-slate-200 active:scale-[0.98]">
                        Modify Profile
                      </button>
                      <button
                        class="w-full py-4 bg-white border-2 border-slate-100 text-slate-700 rounded-[20px] font-black uppercase tracking-widest text-xs hover:bg-slate-50 transition-all active:scale-[0.98]">
                        {{ previewType === 'product' ? 'View Full Analytics' : 'Security Settings' }}
                      </button>
                    </div>

                    <div v-if="previewType === 'product' && selectedItem?.description" class="pt-6 border-t border-slate-100">
                      <p class="text-xs font-bold text-slate-400 uppercase tracking-widest mb-3">Description</p>
                      <p class="text-sm text-slate-500 leading-relaxed italic">
                        "{{ selectedItem.description }}"
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Growth Card -->
            <div class="bg-indigo-600 p-8 rounded-[40px] text-white shadow-xl shadow-indigo-200 relative overflow-hidden group">
              <div class="absolute top-0 right-0 p-8">
                <TrendingUp class="w-8 h-8 text-white/20" />
              </div>
              <p class="text-[10px] font-black text-indigo-100 uppercase tracking-widest">Performance</p>
              <h5 class="text-2xl font-black mt-1">Growth Index</h5>
              <div class="mt-8 flex items-end gap-3">
                <p class="text-5xl font-black tracking-tighter">{{ dashboardData?.monthlyGrowth || '+24%' }}</p>
                <p class="text-indigo-100/60 text-xs font-bold uppercase tracking-widest mb-2">Net increase</p>
              </div>
              <div class="mt-10 flex items-end gap-1.5 h-16">
                <div v-for="(h, index) in (dashboardData?.growthChart || [40, 70, 45, 90, 65, 80, 100])" :key="index"
                  class="flex-1 bg-white/20 rounded-lg transition-all duration-500 group-hover:bg-white/40"
                  :style="{ height: h + '%' }">
                </div>
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
