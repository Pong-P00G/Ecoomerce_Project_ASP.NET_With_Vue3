<script setup>
import { ref, computed, onMounted } from 'vue'
import {
    Users,
    Plus,
    Search,
    Edit2,
    Trash2,
    X,
    Loader2,
    Filter,
    Download,
    Eye,
    AlertCircle,
    CheckCircle,
    Activity,
    RefreshCw
} from 'lucide-vue-next'
import { authAPI } from '../../api/authApi.js'
import api from '../../api/api.js'

// State
const users = ref([])
const loading = ref(false)
const error = ref(null)
const success = ref(null)
const searchTerm = ref('')
const selectedRole = ref('all')
const selectedStatus = ref('all')
const sortBy = ref('name')
const sortOrder = ref('asc')

// Modal states
const showAddModal = ref(false)
const showEditModal = ref(false)
const showViewModal = ref(false)
const currentUser = ref(null)

// Form data
const formData = ref({
    name: '',
    email: '',
    role: '',
    status: '',
    phone: '',
    department: ''
})

// Roles (can be fetched from API)
const roles = ref([
    'Admin',
    'Customer',
])

// Computed properties
const filteredUsers = computed(() => {
    let filtered = users.value

    // Search filter
    if (searchTerm.value) {
        filtered = filtered.filter(u =>
            u.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
            u.email.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
            u.department?.toLowerCase().includes(searchTerm.value.toLowerCase())
        )
    }

    // Role filter
    if (selectedRole.value !== 'all') {
        filtered = filtered.filter(u => u.role === selectedRole.value)
    }

    // Status filter
    if (selectedStatus.value !== 'all') {
        filtered = filtered.filter(u => u.status === selectedStatus.value)
    }

    // Sorting
    filtered = [...filtered].sort((a, b) => {
        let aVal = a[sortBy.value]
        let bVal = b[sortBy.value]

        if (typeof aVal === 'string') {
            aVal = aVal.toLowerCase()
            bVal = bVal.toLowerCase()
        }

        if (sortOrder.value === 'asc') {
            return aVal > bVal ? 1 : -1
        } else {
            return aVal < bVal ? 1 : -1
        }
    })

    return filtered
})

const stats = computed(() => ({
    total: users.value.length,
    active: users.value.filter(u => u.status === 'active').length,
    inactive: users.value.filter(u => u.status === 'inactive').length,
    admins: users.value.filter(u => u.role === 'Admin').length
}))

// Methods
const fetchUsers = async () => {
    try {
        loading.value = true
        error.value = null

        const response = await api.get('/users')
        users.value = (response.data || []).map(u => ({
            id: u.userId,
            name: u.fullName || `${u.firstName} ${u.lastName}`.trim(),
            email: u.email,
            role: u.roleName || (u.roleId === 1 ? 'Admin' : 'Customer'),
            status: u.isActive ? 'active' : 'inactive',
            phone: 'N/A',
            department: 'General',
            createdAt: new Date(u.createdAt).toLocaleDateString()
        }))

    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to load users'
        console.error('Error fetching users:', err)
    } finally {
        loading.value = false
    }
}

const openAddModal = () => {
    formData.value = {
        name: '',
        email: '',
        role: '',
        status: '',
        phone: '',
        department: ''
    }
    showAddModal.value = true
}

const openEditModal = (user) => {
    currentUser.value = user
    formData.value = { 
        ...user,
        role: user.role // Ensure role is mapped correctly
    }
    showEditModal.value = true
}

const openViewModal = (user) => {
    currentUser.value = user
    showViewModal.value = true
}

const closeModals = () => {
    showAddModal.value = false
    showEditModal.value = false
    showViewModal.value = false
    currentUser.value = null
}

const handleAddUser = async () => {
    try {
        loading.value = true
        error.value = null

        const names = formData.value.name.split(' ')
        const firstName = names[0]
        const lastName = names.length > 1 ? names.slice(1).join(' ') : 'User'

        const roleMapping = { 'Admin': 1, 'Customer': 2 }
        
        const userData = {
            username: formData.value.email.split('@')[0], // Generate username from email
            email: formData.value.email,
            password: 'Password123!', // Default temporary password
            firstName: firstName,
            lastName: lastName,
            roleId: roleMapping[formData.value.role] || 2,
            isActive: formData.value.status === 'active'
        }

        // API call
        const response = await api.post('/users', userData)
        
        // Refetch to get clean state
        await fetchUsers()
        
        success.value = 'User added successfully!'
        closeModals()
        setTimeout(() => success.value = null, 3000)
        } catch (err) {
            error.value = err.response?.data?.message || 'Failed to add user'
        } finally {
            loading.value = false
        }
}

const handleUpdateUser = async () => {
    try {
        loading.value = true
        error.value = null

        const names = formData.value.name.split(' ')
        const firstName = names[0]
        const lastName = names.length > 1 ? names.slice(1).join(' ') : ''

        const roleMapping = { 'Admin': 1, 'Customer': 2 }

        const userData = {
            firstName: firstName,
            lastName: lastName,
            email: formData.value.email,
            // Only send role if we are admin (implied by this page access)
            roleId: roleMapping[formData.value.role], 
            isActive: formData.value.status === 'active'
        }

        // API call
        await api.put(`/users/${currentUser.value.id}`, userData)

        // Refetch
        await fetchUsers()

        success.value = 'User updated successfully!'
        closeModals()
        setTimeout(() => success.value = null, 3000)

    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to update user'
    } finally {
        loading.value = false
    }
}

const handleDeleteUser = async (id) => {
    if (!confirm('Are you sure you want to delete this user?')) return

    try {
        loading.value = true
        error.value = null

        // API call
        const response = await api.delete(`/users/${id}`)

        users.value = users.value.filter(u => u.id !== id)

        success.value = 'User deleted successfully!'
        setTimeout(() => success.value = null, 3000)

    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to delete user'
    } finally {
        loading.value = false
    }
}

const exportToCSV = () => {
    const headers = ['ID', 'Name', 'Email', 'Role', 'Status', 'Created At']
    const rows = users.value.map(u => [
        u.id,
        u.name,
        u.email,
        u.role,
        u.status,
        u.createdAt
    ])

    const csv = [headers, ...rows].map(row => row.join(',')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv' })
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = 'users.csv'
    a.click()
}

const getStatusColor = (status) => {
    return status === 'active' ? 'green' : 'red'
}

// Lifecycle
onMounted(() => {
    fetchUsers()
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
          <button @click="fetchUsers" class="p-3 bg-slate-900 text-white rounded-xl hover:bg-slate-800 transition-all">
            <RefreshCw class="w-5 h-5" />
          </button>
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
    <header class="bg-white/80 backdrop-blur-md border-b border-slate-100 sticky top-0 z-40">
      <div class="max-w-[1600px] mx-auto px-6 sm:px-8 lg:px-12 py-6">
        <div class="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-8">
          <div class="flex items-center gap-6">
            <div class="w-14 h-14 bg-gradient-to-br from-slate-800 to-slate-900 rounded-[20px] flex items-center justify-center shadow-lg shadow-slate-200 rotate-3 group hover:rotate-0 transition-transform duration-300">
              <Users class="w-7 h-7 text-white" />
            </div>
            <div>
              <h1 class="text-2xl font-black text-slate-900 tracking-tight">User Management</h1>
              <p class="text-[10px] text-slate-400 mt-1 font-black uppercase tracking-[0.2em]">Administration Panel</p>
            </div>
          </div>
          <div class="flex items-center gap-3">
            <button @click="fetchUsers"
              class="flex items-center gap-2.5 px-5 py-3 text-xs font-black text-slate-600 bg-white border border-slate-200 rounded-xl hover:bg-slate-50 hover:border-slate-300 transition-all active:scale-95 disabled:opacity-50"
              :disabled="loading">
              <RefreshCw :class="['w-4 h-4', loading && 'animate-spin']" />
              <span>{{ loading ? 'Syncing...' : 'Refresh' }}</span>
            </button>
            <button @click="openAddModal"
              class="flex items-center gap-2.5 px-6 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition-all shadow-lg shadow-blue-200 active:scale-95 font-black text-xs uppercase tracking-widest">
              <Plus class="w-4 h-4" />
              Add New User
            </button>
          </div>
        </div>
      </div>
    </header>

    <main class="max-w-[1600px] mx-auto px-6 sm:px-8 lg:px-12 py-12">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <div v-for="(stat, idx) in [
          { label: 'Total Users', value: stats.total, icon: Users, color: 'blue' },
          { label: 'Active Users', value: stats.active, icon: CheckCircle, color: 'emerald' },
          { label: 'Inactive', value: stats.inactive, icon: AlertCircle, color: 'rose' },
          { label: 'Administrators', value: stats.admins, icon: Activity, color: 'indigo' }
        ]" :key="idx" 
          class="bg-white rounded-3xl p-6 border border-slate-100 shadow-sm hover:shadow-md transition-all duration-300 group">
          <div class="flex items-center gap-5">
            <div :class="`w-12 h-12 rounded-2xl flex items-center justify-center transition-colors duration-300 bg-${stat.color}-50 group-hover:bg-${stat.color}-100` ">
              <component :is="stat.icon" :class="`w-6 h-6 text-${stat.color}-600`" />
            </div>
            <div>
              <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">{{ stat.label }}</p>
              <p class="text-2xl font-black text-slate-900 tracking-tight">{{ stat.value }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Actions Bar -->
      <div class="bg-white rounded-3xl shadow-sm mb-12 p-6 border border-slate-100">
        <div class="flex flex-col lg:flex-row gap-4">
          <div class="flex-1">
            <div class="relative group">
              <Search class="absolute left-5 top-1/2 -translate-y-1/2 text-slate-300 w-5 h-5 group-focus-within:text-blue-600 transition-colors" />
              <input v-model="searchTerm" type="text"
                placeholder="Search users by name, email, or department..."
                class="w-full pl-12 pr-5 py-3.5 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 focus:ring-4 focus:ring-blue-50/50 transition-all font-medium text-slate-600 text-sm" />
            </div>
          </div>

          <div class="flex flex-wrap gap-3">
            <div class="relative">
              <select v-model="selectedRole"
                class="appearance-none pl-5 pr-10 py-3.5 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-bold text-[11px] uppercase tracking-widest text-slate-700 outline-none cursor-pointer">
                <option value="all">All Roles</option>
                <option v-for="role in roles" :key="role" :value="role">{{ role }}</option>
              </select>
              <Filter class="absolute right-4 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" />
            </div>

            <div class="relative">
              <select v-model="selectedStatus"
                class="appearance-none pl-5 pr-10 py-3.5 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-bold text-[11px] uppercase tracking-widest text-slate-700 outline-none cursor-pointer">
                <option value="all">Any Status</option>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
              </select>
              <Filter class="absolute right-4 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" />
            </div>

            <button @click="exportToCSV"
              class="flex items-center gap-2.5 px-6 py-3.5 bg-slate-900 text-white rounded-xl hover:bg-slate-800 transition-all font-bold text-[11px] uppercase tracking-widest active:scale-95 shadow-lg shadow-slate-200">
              <Download class="w-4 h-4" />
              Export CSV
            </button>
          </div>
        </div>
      </div>

      <!-- Users Table -->
      <div class="bg-white rounded-3xl shadow-sm overflow-hidden border border-slate-100">
        <div v-if="loading && users.length === 0" class="p-16 space-y-6">
          <div v-for="i in 5" :key="i" class="flex items-center gap-6 animate-pulse">
            <div class="w-12 h-12 bg-slate-50 rounded-xl"></div>
            <div class="flex-1 space-y-3">
              <div class="h-4 bg-slate-50 rounded-lg w-1/4"></div>
              <div class="h-3 bg-slate-50 rounded-lg w-1/6"></div>
            </div>
          </div>
        </div>

        <div v-else-if="filteredUsers.length === 0" class="text-center py-24">
          <div class="w-20 h-20 bg-slate-50 rounded-2xl flex items-center justify-center mx-auto mb-6">
            <Users class="w-10 h-10 text-slate-200" />
          </div>
          <h3 class="text-xl font-black text-slate-900 tracking-tight">No Results Found</h3>
          <p class="text-slate-400 mt-2 text-sm font-medium max-w-xs mx-auto">We couldn't find any users matching your current filters.</p>
        </div>

        <div v-else>
          <!-- Mobile View (Cards) -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 p-4 lg:hidden">
            <div v-for="user in filteredUsers" :key="user.id" 
              class="bg-white rounded-2xl p-5 border border-slate-100 shadow-sm flex flex-col gap-4">
              <div class="flex items-start justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 rounded-xl bg-slate-100 flex items-center justify-center shrink-0 border border-slate-200/50">
                    <span class="text-lg font-black text-slate-500">{{ user.name.charAt(0) }}</span>
                  </div>
                  <div>
                    <p class="text-sm font-bold text-slate-900 line-clamp-1">{{ user.name }}</p>
                    <p class="text-[11px] font-medium text-slate-400 line-clamp-1">{{ user.email }}</p>
                  </div>
                </div>
                <div class="flex flex-col items-end gap-1">
                   <span :class="`px-2 py-1 rounded-lg text-[10px] font-black uppercase tracking-widest border ${user.status === 'active' ? 'bg-emerald-50 text-emerald-700 border-emerald-100' : 'bg-rose-50 text-rose-700 border-rose-100'}`">
                    {{ user.status }}
                  </span>
                  <p class="text-[10px] font-bold text-slate-400">{{ user.role }}</p>
                </div>
              </div>
              
              <div class="flex items-center justify-between pt-4 border-t border-slate-50 mt-auto">
                 <p class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">{{ user.department || 'General' }}</p>
                 <div class="flex gap-1">
                    <button @click="openViewModal(user)" class="p-2 text-slate-400 hover:text-slate-900 bg-slate-50 rounded-lg">
                      <Eye class="w-4 h-4" />
                    </button>
                    <button @click="openEditModal(user)" class="p-2 text-slate-400 hover:text-blue-600 bg-slate-50 rounded-lg">
                      <Edit2 class="w-4 h-4" />
                    </button>
                    <button @click="handleDeleteUser(user.id)" class="p-2 text-slate-400 hover:text-rose-600 bg-slate-50 rounded-lg">
                      <Trash2 class="w-4 h-4" />
                    </button>
                 </div>
              </div>
            </div>
          </div>

          <!-- Desktop View (Table) -->
          <div class="hidden lg:block overflow-x-auto">
            <table class="w-full">
              <thead>
                <tr class="bg-slate-50/50 border-b border-slate-100">
                  <th class="px-8 py-5 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Member</th>
                  <th class="px-8 py-5 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Department</th>
                  <th class="px-8 py-5 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Role</th>
                  <th class="px-8 py-5 text-left text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Status</th>
                  <th class="px-8 py-5 text-right text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-50">
                <tr v-for="user in filteredUsers" :key="user.id"
                  class="group hover:bg-slate-50/80 transition-all duration-200">
                  <td class="px-8 py-5">
                    <div class="flex items-center gap-4">
                      <div class="w-10 h-10 rounded-xl bg-slate-100 flex items-center justify-center shrink-0 border border-slate-200/50 group-hover:scale-110 transition-transform duration-300">
                        <span class="text-sm font-black text-slate-500">{{ user.name.charAt(0) }}</span>
                      </div>
                      <div class="min-w-0">
                        <p class="text-sm font-bold text-slate-900 truncate tracking-tight">{{ user.name }}</p>
                        <p class="text-[11px] font-medium text-slate-400 truncate">{{ user.email }}</p>
                      </div>
                    </div>
                  </td>
                  <td class="px-8 py-5">
                    <p class="text-xs font-bold text-slate-600 uppercase tracking-wider">{{ user.department || 'General' }}</p>
                  </td>
                  <td class="px-8 py-5">
                    <span class="inline-flex items-center px-2.5 py-1 bg-blue-50 text-blue-700 rounded-lg text-[10px] font-black uppercase tracking-widest border border-blue-100">
                      {{ user.role }}
                    </span>
                  </td>
                  <td class="px-8 py-5">
                    <span :class="`inline-flex items-center px-2.5 py-1 rounded-lg text-[10px] font-black uppercase tracking-widest border ${user.status === 'active' ? 'bg-emerald-50 text-emerald-700 border-emerald-100' : 'bg-rose-50 text-rose-700 border-rose-100'}`">
                      {{ user.status }}
                    </span>
                  </td>
                  <td class="px-8 py-5">
                    <div class="flex justify-end gap-2">
                      <button @click="openViewModal(user)"
                        class="p-2 text-slate-400 hover:text-slate-900 hover:bg-white hover:shadow-sm rounded-lg transition-all border border-transparent hover:border-slate-100">
                        <Eye class="w-4 h-4" />
                      </button>
                      <button @click="openEditModal(user)"
                        class="p-2 text-slate-400 hover:text-blue-600 hover:bg-white hover:shadow-sm rounded-lg transition-all border border-transparent hover:border-slate-100">
                        <Edit2 class="w-4 h-4" />
                      </button>
                      <button @click="handleDeleteUser(user.id)"
                        class="p-2 text-slate-400 hover:text-rose-600 hover:bg-white hover:shadow-sm rounded-lg transition-all border border-transparent hover:border-slate-100">
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
    </main>

    <!-- Modals -->
    <Teleport to="body">
      <!-- Add/Edit Modal -->
      <Transition name="modal">
        <div v-if="showAddModal || showEditModal"
          class="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-[100] p-4 sm:p-6"
          @click.self="closeModals">
          <div class="bg-white rounded-3xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col transform transition-all">
            <div class="px-8 py-6 border-b border-slate-50 flex justify-between items-center shrink-0">
              <div class="flex items-center gap-4">
                <div class="w-10 h-10 bg-slate-900 rounded-xl flex items-center justify-center">
                  <Plus v-if="showAddModal" class="w-5 h-5 text-white" />
                  <Edit2 v-else class="w-5 h-5 text-white" />
                </div>
                <div>
                  <h3 class="text-xl font-black text-slate-900 tracking-tight">
                    {{ showAddModal ? 'New User' : 'Edit User' }}
                  </h3>
                  <p class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mt-0.5">Account Configuration</p>
                </div>
              </div>
              <button @click="closeModals" class="p-2 hover:bg-slate-50 rounded-lg transition-all">
                <X class="w-5 h-5 text-slate-400" />
              </button>
            </div>

            <div class="p-8 overflow-y-auto grow custom-scrollbar">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div class="space-y-5">
                  <div>
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2 ml-1">Full Name</label>
                    <input v-model="formData.name" type="text" required
                      class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-bold text-slate-700 outline-none text-sm"
                      placeholder="e.g. John Doe" />
                  </div>
                  <div>
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2 ml-1">Email Address</label>
                    <input v-model="formData.email" type="email" required
                      class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-bold text-slate-700 outline-none text-sm"
                      placeholder="e.g. john@example.com" />
                  </div>
                  <div>
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2 ml-1">Phone Number</label>
                    <input v-model="formData.phone" type="tel"
                      class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-bold text-slate-700 outline-none text-sm"
                      placeholder="e.g. +1 234 567 890" />
                  </div>
                </div>

                <div class="space-y-5">
                  <div>
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2 ml-1">Department</label>
                    <input v-model="formData.department" type="text"
                      class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-bold text-slate-700 outline-none text-sm"
                      placeholder="e.g. Marketing" />
                  </div>
                  <div>
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2 ml-1">System Role</label>
                    <select v-model="formData.role" required
                      class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-black text-xs uppercase tracking-widest text-slate-700 outline-none cursor-pointer">
                      <option value="">Select Role</option>
                      <option v-for="role in roles" :key="role" :value="role">{{ role }}</option>
                    </select>
                  </div>
                  <div>
                    <label class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2 ml-1">Account Status</label>
                    <select v-model="formData.status" required
                      class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:bg-white focus:border-blue-200 transition-all font-black text-xs uppercase tracking-widest text-slate-700 outline-none cursor-pointer">
                      <option value="">Select Status</option>
                      <option value="active">Active</option>
                      <option value="inactive">Inactive</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>

            <div class="p-8 border-t border-slate-50 bg-slate-50/50 flex gap-3 shrink-0">
              <button @click="closeModals"
                class="px-6 py-3 bg-white border border-slate-200 text-slate-600 rounded-xl hover:bg-slate-100 transition-all font-bold text-xs uppercase tracking-widest">
                Cancel
              </button>
              <button @click="showAddModal ? handleAddUser() : handleUpdateUser()"
                :disabled="loading || !formData.name || !formData.email || !formData.role || !formData.status"
                class="flex-1 bg-blue-600 text-white py-3 rounded-xl hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all font-black text-xs uppercase tracking-widest flex items-center justify-center gap-2 shadow-lg shadow-blue-100">
                <Loader2 v-if="loading" class="w-4 h-4 animate-spin" />
                <span>{{ loading ? 'Processing...' : (showAddModal ? 'Create User' : 'Update User') }}</span>
              </button>
            </div>
          </div>
        </div>
      </Transition>

      <!-- View User Modal -->
      <Transition name="modal">
        <div v-if="showViewModal && currentUser"
          class="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-[100] p-4 sm:p-6"
          @click.self="closeModals">
          <div class="bg-white rounded-3xl shadow-2xl w-full max-w-md overflow-hidden flex flex-col transform transition-all">
            <div class="p-8 text-center bg-slate-50 border-b border-slate-100 relative shrink-0">
              <button @click="closeModals" class="absolute right-4 top-4 p-2 hover:bg-white rounded-lg transition-all">
                <X class="w-5 h-5 text-slate-400" />
              </button>
              <div class="w-20 h-20 bg-white rounded-2xl flex items-center justify-center mx-auto mb-4 shadow-sm border border-slate-100">
                <Users class="w-10 h-10 text-slate-900" />
              </div>
              <h3 class="text-2xl font-black text-slate-900 tracking-tight leading-none mb-2">{{ currentUser.name }}</h3>
              <p class="text-[10px] font-black text-blue-600 uppercase tracking-[0.2em] bg-blue-50 inline-block px-3 py-1 rounded-full">{{ currentUser.role }}</p>
            </div>

            <div class="p-8 space-y-6">
              <div class="space-y-4">
                <div class="flex items-center justify-between py-2 border-b border-slate-50">
                  <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Email</span>
                  <span class="text-sm font-bold text-slate-700">{{ currentUser.email }}</span>
                </div>
                <div class="flex items-center justify-between py-2 border-b border-slate-50">
                  <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Status</span>
                  <span :class="`px-2.5 py-1 rounded-lg text-[10px] font-black uppercase tracking-widest border ${currentUser.status === 'active' ? 'bg-emerald-50 text-emerald-700 border-emerald-100' : 'bg-rose-50 text-rose-700 border-rose-100'}`">
                    {{ currentUser.status }}
                  </span>
                </div>
                <div class="flex items-center justify-between py-2 border-b border-slate-50">
                  <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Department</span>
                  <span class="text-sm font-bold text-slate-700">{{ currentUser.department || 'General' }}</span>
                </div>
                <div class="flex items-center justify-between py-2 border-b border-slate-50">
                  <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Phone</span>
                  <span class="text-sm font-bold text-slate-700">{{ currentUser.phone || 'N/A' }}</span>
                </div>
                <div class="flex items-center justify-between py-2 border-b border-slate-50">
                  <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Joined</span>
                  <span class="text-sm font-bold text-slate-700">{{ currentUser.createdAt }}</span>
                </div>
              </div>

              <div class="flex gap-3 pt-2">
                <button @click="closeModals"
                  class="flex-1 px-6 py-3 bg-white border border-slate-200 text-slate-600 rounded-xl hover:bg-slate-100 transition-all font-bold text-xs uppercase tracking-widest">
                  Close
                </button>
                <button @click="openEditModal(currentUser)"
                  class="flex-1 bg-slate-900 text-white py-3 rounded-xl hover:bg-slate-800 transition-all font-black text-xs uppercase tracking-widest shadow-lg shadow-slate-200">
                  Edit Profile
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

.modal-enter-active>div,
.modal-leave-active>div {
    transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from>div,
.modal-leave-to>div {
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
    transform: translateX(100%);
    opacity: 0;
}

.slide-fade-leave-to {
    transform: translateX(100%);
    opacity: 0;
}

/* Fade Transition */
.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}
</style>