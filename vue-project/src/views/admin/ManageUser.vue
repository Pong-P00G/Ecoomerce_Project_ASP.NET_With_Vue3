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
    Activity
} from 'lucide-vue-next'
import authAPI from '../../api/authApi.js'

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
        users.value = response.data.users || []

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
    formData.value = { ...user }
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

        const userData = {
            name: formData.value.name,
            email: formData.value.email,
            role: formData.value.role,
            status: formData.value.status,
            phone: formData.value.phone,
            department: formData.value.department
        }

        // API call
        const response = await api.post('/users', userData)
        users.value.push(response.data.user)
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

        const userData = {
            name: formData.value.name,
            email: formData.value.email,
            role: formData.value.role,
            status: formData.value.status,
        }

        // API call
        await authAPI.put(`/users/${currentUser.value.id}`, userData)

        // Update locally
        const index = users.value.findIndex(u => u.id === currentUser.value.id)
        if (index !== -1) {
            users.value[index] = { ...users.value[index], ...userData }
        }
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
        const response = await authAPI.delete(`/users/${id}`)

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
    <div class="min-h-screen bg-gray-50">
        <!-- Error Banner -->
        <div v-if="error" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-6">
            <div class="bg-red-50 border border-red-200 rounded-2xl p-4 flex items-center gap-3">
                <AlertCircle class="w-5 h-5 text-red-600 shrink-0" />
                <div class="flex-1">
                    <p class="text-sm font-semibold text-red-900">Error loading users</p>
                    <p class="text-xs text-red-700 mt-0.5">{{ error }}</p>
                </div>
                <button @click="fetchUsers()" class="text-sm font-medium text-red-700 hover:text-red-900 underline">
                    Retry
                </button>
            </div>
        </div>
        <!-- Success Message -->
        <Transition name="slide-fade">
            <div v-if="success"
                class="fixed top-4 right-4 bg-emerald-50 border border-emerald-200 text-emerald-800 px-6 py-4 rounded-2xl shadow-xl z-50 max-w-md flex items-center gap-3">
                <CheckCircle class="w-5 h-5 shrink-0" />
                <span class="flex-1 font-medium">{{ success }}</span>
                <button @click="success = null" class="text-emerald-600 hover:text-emerald-800 transition-colors">
                    <X class="w-4 h-4" />
                </button>
            </div>
        </Transition>
        <!-- Header -->
        <header class="bg-white shadow-sm border-b border-gray-100">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
                <div class="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4">
                    <div>
                        <h1 class="text-3xl font-bold text-gray-900 flex items-center gap-3">
                            <Users class="w-8 h-8 text-blue-600" />
                            Manage Users
                        </h1>
                        <p class="text-sm text-gray-600 mt-2">Manage user accounts and permissions</p>
                    </div>
                    <button @click="fetchUsers"
                        class="self-start sm:self-auto flex items-center gap-2 px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors"
                        :disabled="loading">
                        <Loader2 v-if="loading" class="w-4 h-4 animate-spin" />
                        <span>{{ loading ? 'Refreshing...' : 'Refresh' }}</span>
                    </button>
                </div>
            </div>
        </header>

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
            <!-- Loading Skeleton -->
            <div v-if="loading && users.length === 0">
                <!-- Stats Cards Skeleton -->
                <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
                    <div v-for="i in 4" :key="i" class="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm animate-pulse">
                        <div class="flex items-center justify-between">
                            <div class="flex-1">
                                <div class="h-3 bg-gray-200 rounded w-24 mb-4"></div>
                                <div class="h-8 bg-gray-200 rounded w-20"></div>
                            </div>
                            <div class="w-12 h-12 rounded-2xl bg-gray-200"></div>
                        </div>
                    </div>
                </div>
                <!-- Actions Bar Skeleton -->
                <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm mb-8 p-6 animate-pulse">
                    <div class="flex flex-col lg:flex-row gap-4">
                        <div class="flex-1 h-12 bg-gray-200 rounded-2xl"></div>
                        <div class="flex gap-3 w-full lg:w-auto">
                            <div class="h-12 bg-gray-200 rounded-2xl w-32"></div>
                            <div class="h-12 bg-gray-200 rounded-2xl w-32"></div>
                            <div class="h-12 bg-gray-200 rounded-2xl w-32"></div>
                            <div class="h-12 bg-gray-200 rounded-2xl w-40"></div>
                        </div>
                    </div>
                </div>
                <!-- Table Skeleton -->
                <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm overflow-hidden animate-pulse">
                    <div class="p-8 space-y-4">
                        <div v-for="i in 5" :key="i" class="flex items-center gap-4 py-4 border-b border-gray-100">
                            <div class="w-12 h-12 rounded-2xl bg-gray-200"></div>
                            <div class="flex-1 space-y-2">
                                <div class="h-4 bg-gray-200 rounded w-48"></div>
                                <div class="h-3 bg-gray-200 rounded w-32"></div>
                            </div>
                            <div class="h-4 bg-gray-200 rounded w-32"></div>
                            <div class="h-4 bg-gray-200 rounded w-24"></div>
                            <div class="h-4 bg-gray-200 rounded w-20"></div>
                            <div class="h-4 bg-gray-200 rounded w-16"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
            <!-- Stats Cards -->
            <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
                <div
                    class="bg-white rounded-3xl border border-gray-100 shadow-sm p-6 hover:shadow-md transition-shadow">
                    <div class="flex items-center justify-between">
                        <div>
                            <p class="text-xs font-bold text-gray-400 uppercase tracking-wider">Total Users</p>
                            <p class="text-3xl font-bold text-gray-900 mt-2">{{ stats.total }}</p>
                        </div>
                        <div class="w-12 h-12 bg-blue-50 rounded-2xl flex items-center justify-center text-blue-600">
                            <Users class="w-6 h-6" />
                        </div>
                    </div>
                </div>

                <div
                    class="bg-white rounded-3xl border border-gray-100 shadow-sm p-6 hover:shadow-md transition-shadow">
                    <div class="flex items-center justify-between">
                        <div>
                            <p class="text-xs font-bold text-gray-400 uppercase tracking-wider">Active Users</p>
                            <p class="text-3xl font-bold text-emerald-600 mt-2">{{ stats.active }}</p>
                        </div>
                        <div
                            class="w-12 h-12 bg-emerald-50 rounded-2xl flex items-center justify-center text-emerald-600">
                            <CheckCircle class="w-6 h-6" />
                        </div>
                    </div>
                </div>

                <div
                    class="bg-white rounded-3xl border border-gray-100 shadow-sm p-6 hover:shadow-md transition-shadow">
                    <div class="flex items-center justify-between">
                        <div>
                            <p class="text-xs font-bold text-gray-400 uppercase tracking-wider">Inactive Users</p>
                            <p class="text-3xl font-bold text-rose-600 mt-2">{{ stats.inactive }}</p>
                        </div>
                        <div class="w-12 h-12 bg-rose-50 rounded-2xl flex items-center justify-center text-rose-600">
                            <AlertCircle class="w-6 h-6" />
                        </div>
                    </div>
                </div>

                <div
                    class="bg-white rounded-3xl border border-gray-100 shadow-sm p-6 hover:shadow-md transition-shadow">
                    <div class="flex items-center justify-between">
                        <div>
                            <p class="text-xs font-bold text-gray-400 uppercase tracking-wider">Admins</p>
                            <p class="text-3xl font-bold text-purple-600 mt-2">{{ stats.admins }}</p>
                        </div>
                        <div
                            class="w-12 h-12 bg-purple-50 rounded-2xl flex items-center justify-center text-purple-600">
                            <Activity class="w-6 h-6" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Actions Bar --> 
            <div class="bg-white text-gray-900 rounded-[32px] border border-gray-100 shadow-sm mb-8 p-6">
                <div class="flex flex-col lg:flex-row gap-4 items-center">
                    <!-- Search -->
                    <div class="flex-1 w-full">
                        <div class="relative group">
                            <Search
                                class="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5 group-focus-within:text-blue-600 transition-colors" />
                            <input v-model="searchTerm" type="text"
                                placeholder="Search users by name, email, or department..."
                                class="w-full pl-12 pr-4 py-3 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-blue-600/20 transition-all" />
                        </div>
                    </div>

                    <!-- Filters & Actions -->
                    <div class="flex flex-wrap gap-3 w-full lg:w-auto">
                        <select v-model="selectedRole"
                            class="grow lg:grow-0 px-4 py-3 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-blue-600/20 text-sm font-medium">
                            <option value="all">All Roles</option>
                            <option v-for="role in roles" :key="role" :value="role">{{ role }}</option>
                        </select>

                        <select v-model="selectedStatus"
                            class="grow lg:grow-0 px-4 py-3 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-blue-600/20 text-sm font-medium">
                            <option value="all">All Status</option>
                            <option value="active">Active</option>
                            <option value="inactive">Inactive</option>
                        </select>

                        <div class="flex gap-2 w-full lg:w-auto">
                            <button @click="exportToCSV"
                                class="flex-1 lg:flex-none flex items-center justify-center gap-2 px-6 py-3 bg-gray-50 text-gray-700 rounded-2xl hover:bg-gray-100 transition-colors font-bold text-sm">
                                <Download class="w-4 h-4" />
                                Export
                            </button>

                            <button @click="openAddModal"
                                class="flex-1 lg:flex-none flex items-center justify-center gap-2 px-6 py-3 bg-gray-900 text-white rounded-2xl hover:bg-gray-800 transition-colors font-bold text-sm shadow-lg shadow-gray-200">
                                <Plus class="w-4 h-4" />
                                Add User
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Users Table -->
            <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm overflow-hidden">
                <div v-if="filteredUsers.length === 0 && !loading" class="text-center py-20">
                    <div
                        class="w-20 h-20 bg-gray-50 rounded-full flex items-center justify-center mx-auto mb-4 text-gray-300">
                        <Users class="w-10 h-10" />
                    </div>
                    <p class="text-gray-500 font-medium text-lg mb-2">No users found matching your search</p>
                    <p class="text-sm text-gray-400 mb-4">Try adjusting your filters or search terms</p>
                    <button @click="searchTerm = ''; selectedRole = 'all'; selectedStatus = 'all'"
                        class="px-6 py-2 text-sm font-semibold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors">
                        Clear all filters
                    </button>
                </div>

                <div v-if="loading && users.length > 0" class="p-8 text-center">
                    <Loader2 class="w-6 h-6 animate-spin text-blue-600 mx-auto mb-2" />
                    <p class="text-sm text-gray-500">Refreshing users...</p>
                </div>

                <div v-else class="overflow-x-auto">
                    <table class="w-full text-left border-collapse">
                        <thead>
                            <tr class="bg-gray-50/50 border-b border-gray-100">
                                <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">User
                                    Details</th>
                                <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">
                                    Email</th>
                                <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">Role
                                </th>
                                <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">
                                    Department</th>
                                <th class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest">
                                    Status</th>
                                <th
                                    class="px-8 py-5 text-[10px] font-bold text-gray-400 uppercase tracking-widest text-right">
                                    Actions</th>
                            </tr>
                        </thead>
                        <tbody class="divide-y divide-gray-50">
                            <tr v-for="user in filteredUsers" :key="user.id"
                                class="hover:bg-gray-50/50 transition-colors group">
                                <td class="px-8 py-5">
                                    <div class="flex items-center gap-4">
                                        <div
                                            class="w-12 h-12 rounded-2xl bg-gray-100 flex items-center justify-center shrink-0 group-hover:bg-white transition-colors">
                                            <Users class="w-6 h-6 text-gray-400" />
                                        </div>
                                        <div>
                                            <p
                                                class="font-bold text-gray-900 group-hover:text-blue-600 transition-colors">
                                                {{ user.name }}</p>
                                            <p
                                                class="text-xs text-gray-500 mt-0.5 font-medium tracking-tight uppercase">
                                                {{ user.phone || 'No phone' }}</p>
                                        </div>
                                    </div>
                                </td>
                                <td class="px-8 py-5 text-sm text-gray-600 font-medium">{{ user.email }}</td>
                                <td class="px-8 py-5">
                                    <span
                                        class="px-3 py-1 bg-blue-50 text-blue-700 rounded-lg text-[10px] font-bold uppercase tracking-wider">
                                        {{ user.role }}
                                    </span>
                                </td>
                                <td class="px-8 py-5 text-sm text-gray-600 font-medium">{{ user.department }}</td>
                                <td class="px-8 py-5">
                                    <span :class="[
                                        'px-3 py-1 rounded-lg text-[10px] font-bold uppercase tracking-wider',
                                        user.status === 'active' ? 'bg-emerald-50 text-emerald-700' : 'bg-rose-50 text-rose-700'
                                    ]">
                                        {{ user.status }}
                                    </span>
                                </td>
                                <td class="px-8 py-5">
                                    <div
                                        class="flex justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                                        <button @click="openViewModal(user)"
                                            class="p-2.5 text-gray-400 hover:text-gray-900 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-gray-100">
                                            <Eye class="w-4 h-4" />
                                        </button>
                                        <button @click="openEditModal(user)"
                                            class="p-2.5 text-blue-400 hover:text-blue-600 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-gray-100">
                                            <Edit2 class="w-4 h-4" />
                                        </button>
                                        <button @click="handleDeleteUser(user.id)"
                                            class="p-2.5 text-rose-400 hover:text-rose-600 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-gray-100">
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

        <!-- Add User Modal -->
        <Teleport to="body">
            <Transition name="modal">
                <div v-if="showAddModal"
                    class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                    <div
                        class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col">
                        <div class="p-6 border-b border-gray-100 shrink-0">
                            <div class="flex justify-between items-center">
                                <h3 class="text-2xl font-bold text-gray-900">Add New User</h3>
                                <button @click="closeModals" class="p-2 hover:bg-gray-100 rounded-xl transition-colors">
                                    <X class="w-5 h-5 text-gray-500" />
                                </button>
                            </div>
                        </div>
                        <div class="p-6 overflow-y-auto flex-1">

                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Full Name *</label>
                                    <input v-model="formData.name" type="text"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter full name" required />
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Email *</label>
                                    <input v-model="formData.email" type="email"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter email address" required />
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Role *</label>
                                    <select v-model="formData.role"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        required>
                                        <option value="">Select role</option>
                                        <option v-for="role in roles" :key="role" :value="role">{{ role }}</option>
                                    </select>
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Status *</label>
                                    <select v-model="formData.status"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        required>
                                        <option value="">Select status</option>
                                        <option value="active">Active</option>
                                        <option value="inactive">Inactive</option>
                                    </select>
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Phone</label>
                                    <input v-model="formData.phone" type="tel"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter phone number" />
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Department</label>
                                    <input v-model="formData.department" type="text"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter department" />
                                </div>
                            </div>

                            <div class="flex gap-3 mt-6 pt-6 border-t border-gray-100">
                                <button @click="closeModals"
                                    class="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-colors font-semibold">
                                    Cancel
                                </button>
                                <button @click="handleAddUser"
                                    :disabled="loading || !formData.name || !formData.email || !formData.role || !formData.status"
                                    class="flex-1 bg-gray-900 text-white py-3 rounded-xl hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-semibold flex items-center justify-center gap-2">
                                    <Loader2 v-if="loading" class="w-4 h-4 animate-spin" />
                                    {{ loading ? 'Adding...' : 'Add User' }}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </Transition>
        </Teleport>

        <!-- View User Modal -->
        <Teleport to="body">
            <Transition name="modal">
                <div v-if="showViewModal && currentUser"
                    class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl overflow-hidden">
                        <div class="p-6 border-b border-gray-100">
                            <div class="flex justify-between items-center">
                                <h3 class="text-2xl font-bold text-gray-900">User Details</h3>
                                <button @click="closeModals" class="p-2 hover:bg-gray-100 rounded-xl transition-colors">
                                    <X class="w-5 h-5 text-gray-500" />
                                </button>
                            </div>
                        </div>
                        <div class="p-6">

                            <div class="space-y-4">
                                <div class="grid grid-cols-2 gap-4">
                                    <div>
                                        <p class="text-sm font-medium text-gray-500">Full Name</p>
                                        <p class="text-base text-gray-900 mt-1">{{ currentUser.name }}</p>
                                    </div>

                                    <div>
                                        <p class="text-sm font-medium text-gray-500">Email</p>
                                        <p class="text-base text-gray-900 mt-1">{{ currentUser.email }}</p>
                                    </div>

                                    <div>
                                        <p class="text-sm font-medium text-gray-500">Role</p>
                                        <p class="text-base text-gray-900 mt-1">{{ currentUser.role }}</p>
                                    </div>

                                    <div>
                                        <p class="text-sm font-medium text-gray-500">Status</p>
                                        <span :class="[
                                            'inline-block px-2 py-1 text-xs font-medium rounded-full mt-1',
                                            currentUser.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                                        ]">
                                            {{ currentUser.status }}
                                        </span>
                                    </div>

                                    <div>
                                        <p class="text-sm font-medium text-gray-500">Phone</p>
                                        <p class="text-base text-gray-900 mt-1">{{ currentUser.phone || 'Not provided'
                                            }}</p>
                                    </div>

                                    <div>
                                        <p class="text-sm font-medium text-gray-500">Department</p>
                                        <p class="text-base text-gray-900 mt-1">{{ currentUser.department ||
                                            'Notassigned' }}
                                        </p>
                                    </div>

                                    <div class="col-span-2">
                                        <p class="text-sm font-medium text-gray-500">Created At</p>
                                        <p class="text-base text-gray-900 mt-1">{{ currentUser.createdAt }}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="p-6 border-t border-gray-100 flex gap-3">
                            <button @click="closeModals"
                                class="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-colors font-semibold">
                                Close
                            </button>
                            <button @click="openEditModal(currentUser)"
                                class="flex-1 bg-gray-900 text-white py-3 rounded-xl hover:bg-gray-800 transition-colors font-semibold">
                                Edit User
                            </button>
                        </div>
                    </div>
                </div>
            </Transition>
        </Teleport>

        <!-- Edit User Modal -->
        <Teleport to="body">
            <Transition name="modal">
                <div v-if="showEditModal"
                    class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                    <div
                        class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col">
                        <div class="p-6 border-b border-gray-100 shrink-0">
                            <div class="flex justify-between items-center">
                                <h3 class="text-2xl font-bold text-gray-900">Edit User</h3>
                                <button @click="closeModals" class="p-2 hover:bg-gray-100 rounded-xl transition-colors">
                                    <X class="w-5 h-5 text-gray-500" />
                                </button>
                            </div>
                        </div>
                        <div class="p-6 overflow-y-auto flex-1">

                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Full Name *</label>
                                    <input v-model="formData.name" type="text"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter full name" required />
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Email *</label>
                                    <input v-model="formData.email" type="email"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter email address" required />
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Role *</label>
                                    <select v-model="formData.role"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        required>
                                        <option value="">Select role</option>
                                        <option v-for="role in roles" :key="role" :value="role">{{ role }}</option>
                                    </select>
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Status *</label>
                                    <select v-model="formData.status"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        required>
                                        <option value="">Select status</option>
                                        <option value="active">Active</option>
                                        <option value="inactive">Inactive</option>
                                    </select>
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Phone</label>
                                    <input v-model="formData.phone" type="tel"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter phone number" />
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">Department</label>
                                    <input v-model="formData.department" type="text"
                                        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter department" />
                                </div>
                            </div>

                            <div class="flex gap-3 mt-6 pt-6 border-t border-gray-100">
                                <button @click="closeModals"
                                    class="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-colors font-semibold">
                                    Cancel
                                </button>
                                <button @click="handleUpdateUser"
                                    :disabled="loading || !formData.name || !formData.email || !formData.role || !formData.status"
                                    class="flex-1 bg-gray-900 text-white py-3 rounded-xl hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-semibold flex items-center justify-center gap-2">
                                    <Loader2 v-if="loading" class="w-4 h-4 animate-spin" />
                                    {{ loading ? 'Updating...' : 'Update User' }}
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