<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../stores/Auth'
import { profileApi } from '../api/profileApi'
import { useRouter } from 'vue-router'
import {
    User,
    Mail,
    Lock,
    MapPin,
    CreditCard,
    Heart,
    Save,
    Plus,
    Edit2,
    Trash2,
    X,
    Loader2,
    CheckCircle,
    AlertCircle,
    Camera,
    Eye,
    EyeOff,
    Star
} from 'lucide-vue-next'

const authStore = useAuthStore()
const router = useRouter()

const loading = ref(false)
const saving = ref(false)
const error = ref(null)
const success = ref(null)

// Active tab
const activeTab = ref('profile')

// Profile data
const profileData = ref({
    name: '',
    email: '',
    phone: '',
    avatar: null
})

// Password change
const passwordData = ref({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
})
const showPasswords = ref({
    current: false,
    new: false,
    confirm: false
})

// Addresses
const addresses = ref([])
const showAddressModal = ref(false)
const editingAddress = ref(null)
const addressForm = ref({
    street: '',
    city: '',
    state: '',
    zipCode: '',
    country: '',
    isDefault: false
})

// Payment methods
const paymentMethods = ref([])
const showPaymentModal = ref(false)
const editingPayment = ref(null)
const paymentForm = ref({
    cardNumber: '',
    cardHolder: '',
    expiryMonth: '',
    expiryYear: '',
    cvv: '',
    isDefault: false
})

// Wishlist
const wishlistItems = ref([])

// Fetch data
const fetchProfile = async () => {
    try {
        loading.value = true
        error.value = null
        const response = await profileApi.getProfile()
        const data = response.data.data || response.data
        profileData.value = {
            name: data.name || data.username || '',
            email: data.email || '',
            phone: data.phone || '',
            avatar: data.avatar || null
        }
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to load profile'
        // Fallback to auth store
        profileData.value = {
            name: authStore.user?.name || authStore.user?.username || '',
            email: authStore.user?.email || '',
            phone: authStore.user?.phone || '',
            avatar: authStore.user?.avatar || null
        }
    } finally {
        loading.value = false
    }
}

const fetchAddresses = async () => {
    try {
        const response = await profileApi.getAddresses()
        addresses.value = response.data.data || response.data || []
    } catch (err) {
        console.error('Failed to load addresses:', err)
        addresses.value = []
    }
}

const fetchPaymentMethods = async () => {
    try {
        const response = await profileApi.getPaymentMethods()
        paymentMethods.value = response.data.data || response.data || []
    } catch (err) {
        console.error('Failed to load payment methods:', err)
        paymentMethods.value = []
    }
}

const fetchWishlist = async () => {
    try {
        const response = await profileApi.getWishlist()
        wishlistItems.value = response.data.data || response.data || []
    } catch (err) {
        console.error('Failed to load wishlist:', err)
        wishlistItems.value = []
    }
}

// Profile update
const handleAvatarChange = async (event) => {
    const file = event.target.files[0]
    if (!file) return

    if (!file.type.startsWith('image/')) {
        error.value = 'Please select an image file'
        return
    }

    try {
        saving.value = true
        error.value = null
        const response = await profileApi.uploadAvatar(file)
        profileData.value.avatar = response.data.data?.avatar || response.data.avatar
        success.value = 'Avatar updated successfully!'
        setTimeout(() => success.value = null, 3000)
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to upload avatar'
    } finally {
        saving.value = false
    }
}

const handleUpdateProfile = async () => {
    try {
        saving.value = true
        error.value = null
        await profileApi.updateProfile({
            name: profileData.value.name,
            email: profileData.value.email,
            phone: profileData.value.phone
        })
        success.value = 'Profile updated successfully!'
        setTimeout(() => success.value = null, 3000)
        await fetchProfile()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to update profile'
    } finally {
        saving.value = false
    }
}

const handleChangePassword = async () => {
    if (passwordData.value.newPassword !== passwordData.value.confirmPassword) {
        error.value = 'New passwords do not match'
        return
    }

    if (passwordData.value.newPassword.length < 6) {
        error.value = 'Password must be at least 6 characters'
        return
    }

    try {
        saving.value = true
        error.value = null
        await profileApi.changePassword({
            currentPassword: passwordData.value.currentPassword,
            newPassword: passwordData.value.newPassword
        })
        success.value = 'Password changed successfully!'
        setTimeout(() => success.value = null, 3000)
        passwordData.value = {
            currentPassword: '',
            newPassword: '',
            confirmPassword: ''
        }
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to change password'
    } finally {
        saving.value = false
    }
}

// Address management
const openAddressModal = (address = null) => {
    editingAddress.value = address
    if (address) {
        addressForm.value = { ...address }
    } else {
        addressForm.value = {
            street: '',
            city: '',
            state: '',
            zipCode: '',
            country: '',
            isDefault: false
        }
    }
    showAddressModal.value = true
}

const closeAddressModal = () => {
    showAddressModal.value = false
    editingAddress.value = null
    addressForm.value = {
        street: '',
        city: '',
        state: '',
        zipCode: '',
        country: '',
        isDefault: false
    }
}

const handleSaveAddress = async () => {
    try {
        saving.value = true
        error.value = null
        if (editingAddress.value) {
            await profileApi.updateAddress(editingAddress.value.id, addressForm.value)
            success.value = 'Address updated successfully!'
        } else {
            await profileApi.addAddress(addressForm.value)
            success.value = 'Address added successfully!'
        }
        setTimeout(() => success.value = null, 3000)
        closeAddressModal()
        await fetchAddresses()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to save address'
    } finally {
        saving.value = false
    }
}

const handleDeleteAddress = async (id) => {
    if (!confirm('Are you sure you want to delete this address?')) return

    try {
        saving.value = true
        error.value = null
        await profileApi.deleteAddress(id)
        success.value = 'Address deleted successfully!'
        setTimeout(() => success.value = null, 3000)
        await fetchAddresses()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to delete address'
    } finally {
        saving.value = false
    }
}

const handleSetDefaultAddress = async (id) => {
    try {
        saving.value = true
        await profileApi.setDefaultAddress(id)
        success.value = 'Default address updated!'
        setTimeout(() => success.value = null, 3000)
        await fetchAddresses()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to set default address'
    } finally {
        saving.value = false
    }
}

// Payment method management
const openPaymentModal = (payment = null) => {
    editingPayment.value = payment
    if (payment) {
        paymentForm.value = {
            cardNumber: payment.cardNumber || '',
            cardHolder: payment.cardHolder || '',
            expiryMonth: payment.expiryMonth || '',
            expiryYear: payment.expiryYear || '',
            cvv: '',
            isDefault: payment.isDefault || false
        }
    } else {
        paymentForm.value = {
            cardNumber: '',
            cardHolder: '',
            expiryMonth: '',
            expiryYear: '',
            cvv: '',
            isDefault: false
        }
    }
    showPaymentModal.value = true
}

const closePaymentModal = () => {
    showPaymentModal.value = false
    editingPayment.value = null
    paymentForm.value = {
        cardNumber: '',
        cardHolder: '',
        expiryMonth: '',
        expiryYear: '',
        cvv: '',
        isDefault: false
    }
}

const formatCardNumber = (value) => {
    const v = value.replace(/\s+/g, '').replace(/[^0-9]/gi, '')
    const matches = v.match(/\d{4,16}/g)
    const match = matches && matches[0] || ''
    const parts = []
    for (let i = 0, len = match.length; i < len; i += 4) {
        parts.push(match.substring(i, i + 4))
    }
    if (parts.length) {
        return parts.join(' ')
    } else {
        return v
    }
}

const handleSavePayment = async () => {
    try {
        saving.value = true
        error.value = null
        const data = {
            ...paymentForm.value,
            cardNumber: paymentForm.value.cardNumber.replace(/\s/g, '')
        }
        if (editingPayment.value) {
            await profileApi.updatePaymentMethod(editingPayment.value.id, data)
            success.value = 'Payment method updated successfully!'
        } else {
            await profileApi.addPaymentMethod(data)
            success.value = 'Payment method added successfully!'
        }
        setTimeout(() => success.value = null, 3000)
        closePaymentModal()
        await fetchPaymentMethods()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to save payment method'
    } finally {
        saving.value = false
    }
}

const handleDeletePayment = async (id) => {
    if (!confirm('Are you sure you want to delete this payment method?')) return

    try {
        saving.value = true
        error.value = null
        await profileApi.deletePaymentMethod(id)
        success.value = 'Payment method deleted successfully!'
        setTimeout(() => success.value = null, 3000)
        await fetchPaymentMethods()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to delete payment method'
    } finally {
        saving.value = false
    }
}

const handleSetDefaultPayment = async (id) => {
    try {
        saving.value = true
        await profileApi.setDefaultPaymentMethod(id)
        success.value = 'Default payment method updated!'
        setTimeout(() => success.value = null, 3000)
        await fetchPaymentMethods()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to set default payment method'
    } finally {
        saving.value = false
    }
}

const handleRemoveFromWishlist = async (productId) => {
    try {
        saving.value = true
        await profileApi.removeFromWishlist(productId)
        success.value = 'Item removed from wishlist!'
        setTimeout(() => success.value = null, 3000)
        await fetchWishlist()
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to remove item'
    } finally {
        saving.value = false
    }
}

const maskCardNumber = (number) => {
    if (!number) return ''
    const cleaned = number.replace(/\s/g, '')
    if (cleaned.length < 4) return cleaned
    return '**** **** **** ' + cleaned.slice(-4)
}

onMounted(async () => {
    await fetchProfile()
    await fetchAddresses()
    await fetchPaymentMethods()
    await fetchWishlist()
})
</script>

<template>
    <div class="min-h-screen bg-gray-50 py-8">
        <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
            <!-- Header -->
            <div class="mb-8">
                <h1 class="text-3xl font-bold text-gray-900 flex items-center gap-3">
                    <User class="w-8 h-8 text-blue-600" />
                    Settings
                </h1>
                <p class="text-gray-500 mt-2">Manage your account settings and preferences</p>
            </div>

            <!-- Messages -->
            <Transition name="slide-fade">
                <div v-if="error"
                    class="mb-6 bg-rose-50 border border-rose-200 rounded-2xl p-4 flex items-center gap-3">
                    <AlertCircle class="w-5 h-5 text-rose-600 shrink-0" />
                    <div class="flex-1">
                        <p class="text-sm font-semibold text-rose-900">Error</p>
                        <p class="text-xs text-rose-700 mt-0.5">{{ error }}</p>
                    </div>
                    <button @click="error = null" class="text-rose-600 hover:text-rose-800">
                        <X class="w-4 h-4" />
                    </button>
                </div>
            </Transition>

            <Transition name="slide-fade">
                <div v-if="success"
                    class="mb-6 bg-emerald-50 border border-emerald-200 rounded-2xl p-4 flex items-center gap-3">
                    <CheckCircle class="w-5 h-5 text-emerald-600 shrink-0" />
                    <span class="flex-1 font-medium text-emerald-900">{{ success }}</span>
                    <button @click="success = null" class="text-emerald-600 hover:text-emerald-800">
                        <X class="w-4 h-4" />
                    </button>
                </div>
            </Transition>

            <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
                <!-- Sidebar Navigation -->
                <div class="lg:col-span-1">
                    <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-4 sticky top-24">
                        <nav class="space-y-2">
                            <button
                                @click="activeTab = 'profile'"
                                :class="[
                                    'w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium',
                                    activeTab === 'profile' ? 'bg-blue-50 text-blue-600' : 'text-gray-600 hover:bg-gray-50'
                                ]">
                                <User class="w-5 h-5" />
                                Profile
                            </button>
                            <button
                                @click="activeTab = 'addresses'"
                                :class="[
                                    'w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium',
                                    activeTab === 'addresses' ? 'bg-blue-50 text-blue-600' : 'text-gray-600 hover:bg-gray-50'
                                ]">
                                <MapPin class="w-5 h-5" />
                                Addresses
                            </button>
                            <button
                                @click="activeTab = 'payments'"
                                :class="[
                                    'w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium',
                                    activeTab === 'payments' ? 'bg-blue-50 text-blue-600' : 'text-gray-600 hover:bg-gray-50'
                                ]">
                                <CreditCard class="w-5 h-5" />
                                Payments
                            </button>
                            <button
                                @click="activeTab = 'wishlist'"
                                :class="[
                                    'w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium',
                                    activeTab === 'wishlist' ? 'bg-blue-50 text-blue-600' : 'text-gray-600 hover:bg-gray-50'
                                ]">
                                <Heart class="w-5 h-5" />
                                Wishlist
                            </button>
                        </nav>
                    </div>
                </div>

                <!-- Main Content -->
                <div class="lg:col-span-3 space-y-6">
                    <!-- Profile Tab -->
                    <div v-if="activeTab === 'profile'" class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-8">
                        <h2 class="text-2xl font-bold text-gray-900 mb-6">Edit Profile</h2>

                        <!-- Avatar Section -->
                        <div class="mb-8">
                            <label class="block text-sm font-medium text-gray-700 mb-3">Profile Picture</label>
                            <div class="flex items-center gap-6">
                                <div class="relative">
                                    <div class="w-24 h-24 rounded-full bg-gray-100 flex items-center justify-center overflow-hidden">
                                        <img v-if="profileData.avatar" :src="profileData.avatar" :alt="profileData.name" class="w-full h-full object-cover">
                                        <div v-else class="w-full h-full bg-linear-to-br from-blue-400 to-indigo-500 flex items-center justify-center">
                                            <span class="text-2xl font-bold text-white">
                                                {{ (profileData.name || 'U').charAt(0).toUpperCase() }}
                                            </span>
                                        </div>
                                    </div>
                                    <label class="absolute bottom-0 right-0 w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center cursor-pointer hover:bg-blue-700 transition-colors shadow-lg">
                                        <Camera class="w-4 h-4 text-white" />
                                        <input type="file" accept="image/*" @change="handleAvatarChange" class="hidden">
                                    </label>
                                </div>
                                <div>
                                    <p class="text-sm text-gray-600 mb-2">Upload a new profile picture</p>
                                    <p class="text-xs text-gray-400">JPG, PNG or GIF. Max size 2MB</p>
                                </div>
                            </div>
                        </div>

                        <!-- Profile Form -->
                        <div class="space-y-6">
                            <div>
                                <label class="block text-sm font-medium text-gray-700 mb-2">Full Name *</label>
                                <input v-model="profileData.name" type="text"
                                    class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                    placeholder="Enter your full name">
                            </div>

                            <div>
                                <label class="block text-sm font-medium text-gray-700 mb-2">Email Address *</label>
                                <input v-model="profileData.email" type="email"
                                    class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                    placeholder="Enter your email">
                            </div>

                            <div>
                                <label class="block text-sm font-medium text-gray-700 mb-2">Phone Number</label>
                                <input v-model="profileData.phone" type="tel"
                                    class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                    placeholder="Enter your phone number">
                            </div>

                            <button @click="handleUpdateProfile" :disabled="saving"
                                class="w-full sm:w-auto px-8 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2">
                                <Loader2 v-if="saving" class="w-4 h-4 animate-spin" />
                                <Save v-else class="w-4 h-4" />
                                {{ saving ? 'Saving...' : 'Save Changes' }}
                            </button>
                        </div>

                        <!-- Change Password Section -->
                        <div class="mt-12 pt-8 border-t border-gray-100">
                            <h3 class="text-xl font-bold text-gray-900 mb-6">Change Password</h3>
                            <div class="space-y-6">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-2">Current Password</label>
                                    <div class="relative">
                                        <input v-model="passwordData.currentPassword" 
                                            :type="showPasswords.current ? 'text' : 'password'"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent pr-12"
                                            placeholder="Enter current password">
                                        <button @click="showPasswords.current = !showPasswords.current"
                                            class="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                                            <Eye v-if="!showPasswords.current" class="w-5 h-5" />
                                            <EyeOff v-else class="w-5 h-5" />
                                        </button>
                                    </div>
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-2">New Password</label>
                                    <div class="relative">
                                        <input v-model="passwordData.newPassword" 
                                            :type="showPasswords.new ? 'text' : 'password'"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent pr-12"
                                            placeholder="Enter new password">
                                        <button @click="showPasswords.new = !showPasswords.new"
                                            class="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                                            <Eye v-if="!showPasswords.new" class="w-5 h-5" />
                                            <EyeOff v-else class="w-5 h-5" />
                                        </button>
                                    </div>
                                </div>

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-2">Confirm New Password</label>
                                    <div class="relative">
                                        <input v-model="passwordData.confirmPassword" 
                                            :type="showPasswords.confirm ? 'text' : 'password'"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent pr-12"
                                            placeholder="Confirm new password">
                                        <button @click="showPasswords.confirm = !showPasswords.confirm"
                                            class="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                                            <Eye v-if="!showPasswords.confirm" class="w-5 h-5" />
                                            <EyeOff v-else class="w-5 h-5" />
                                        </button>
                                    </div>
                                </div>

                                <button @click="handleChangePassword" :disabled="saving"
                                    class="w-full sm:w-auto px-8 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2">
                                    <Loader2 v-if="saving" class="w-4 h-4 animate-spin" />
                                    <Lock v-else class="w-4 h-4" />
                                    {{ saving ? 'Updating...' : 'Update Password' }}
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Addresses Tab -->
                    <div v-if="activeTab === 'addresses'" class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-8">
                        <div class="flex items-center justify-between mb-6">
                            <h2 class="text-2xl font-bold text-gray-900">Saved Addresses</h2>
                            <button @click="openAddressModal()"
                                class="px-6 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 transition-colors flex items-center gap-2">
                                <Plus class="w-4 h-4" />
                                Add Address
                            </button>
                        </div>

                        <div v-if="addresses.length === 0" class="text-center py-12">
                            <MapPin class="w-16 h-16 text-gray-300 mx-auto mb-4" />
                            <p class="text-gray-500 font-medium mb-2">No addresses saved</p>
                            <p class="text-sm text-gray-400 mb-4">Add your first address to get started</p>
                            <button @click="openAddressModal()"
                                class="px-6 py-2 text-sm font-semibold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors">
                                Add Address
                            </button>
                        </div>

                        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div v-for="address in addresses" :key="address.id"
                                :class="[
                                    'p-6 rounded-2xl border-2 transition-all',
                                    address.isDefault ? 'border-blue-500 bg-blue-50' : 'border-gray-100 bg-gray-50'
                                ]">
                                <div class="flex items-start justify-between mb-4">
                                    <div class="flex items-center gap-2">
                                        <MapPin class="w-5 h-5 text-blue-600" />
                                        <span v-if="address.isDefault"
                                            class="px-2 py-1 bg-blue-600 text-white text-xs font-bold rounded-full">Default</span>
                                    </div>
                                    <div class="flex gap-2">
                                        <button @click="openAddressModal(address)"
                                            class="p-2 text-gray-400 hover:text-blue-600 hover:bg-white rounded-lg transition-colors">
                                            <Edit2 class="w-4 h-4" />
                                        </button>
                                        <button @click="handleDeleteAddress(address.id)"
                                            class="p-2 text-gray-400 hover:text-rose-600 hover:bg-white rounded-lg transition-colors">
                                            <Trash2 class="w-4 h-4" />
                                        </button>
                                    </div>
                                </div>
                                <div class="space-y-1">
                                    <p class="font-semibold text-gray-900">{{ address.street }}</p>
                                    <p class="text-sm text-gray-600">{{ address.city }}, {{ address.state }} {{ address.zipCode }}</p>
                                    <p class="text-sm text-gray-600">{{ address.country }}</p>
                                </div>
                                <button v-if="!address.isDefault" @click="handleSetDefaultAddress(address.id)"
                                    class="mt-4 text-sm font-medium text-blue-600 hover:text-blue-700">
                                    Set as default
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Payment Methods Tab -->
                    <div v-if="activeTab === 'payments'" class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-8">
                        <div class="flex items-center justify-between mb-6">
                            <h2 class="text-2xl font-bold text-gray-900">Payment Methods</h2>
                            <button @click="openPaymentModal()"
                                class="px-6 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 transition-colors flex items-center gap-2">
                                <Plus class="w-4 h-4" />
                                Add Payment Method
                            </button>
                        </div>

                        <div v-if="paymentMethods.length === 0" class="text-center py-12">
                            <CreditCard class="w-16 h-16 text-gray-300 mx-auto mb-4" />
                            <p class="text-gray-500 font-medium mb-2">No payment methods saved</p>
                            <p class="text-sm text-gray-400 mb-4">Add a payment method for faster checkout</p>
                            <button @click="openPaymentModal()"
                                class="px-6 py-2 text-sm font-semibold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors">
                                Add Payment Method
                            </button>
                        </div>

                        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div v-for="payment in paymentMethods" :key="payment.id"
                                :class="[
                                    'p-6 rounded-2xl border-2 transition-all',
                                    payment.isDefault ? 'border-blue-500 bg-blue-50' : 'border-gray-100 bg-gray-50'
                                ]">
                                <div class="flex items-start justify-between mb-4">
                                    <div class="flex items-center gap-2">
                                        <CreditCard class="w-5 h-5 text-blue-600" />
                                        <span v-if="payment.isDefault"
                                            class="px-2 py-1 bg-blue-600 text-white text-xs font-bold rounded-full">Default</span>
                                    </div>
                                    <div class="flex gap-2">
                                        <button @click="openPaymentModal(payment)"
                                            class="p-2 text-gray-400 hover:text-blue-600 hover:bg-white rounded-lg transition-colors">
                                            <Edit2 class="w-4 h-4" />
                                        </button>
                                        <button @click="handleDeletePayment(payment.id)"
                                            class="p-2 text-gray-400 hover:text-rose-600 hover:bg-white rounded-lg transition-colors">
                                            <Trash2 class="w-4 h-4" />
                                        </button>
                                    </div>
                                </div>
                                <div class="space-y-2">
                                    <p class="font-mono text-lg font-bold text-gray-900">{{ maskCardNumber(payment.cardNumber) }}</p>
                                    <p class="text-sm text-gray-600">{{ payment.cardHolder }}</p>
                                    <p class="text-sm text-gray-600">Expires {{ payment.expiryMonth }}/{{ payment.expiryYear }}</p>
                                </div>
                                <button v-if="!payment.isDefault" @click="handleSetDefaultPayment(payment.id)"
                                    class="mt-4 text-sm font-medium text-blue-600 hover:text-blue-700">
                                    Set as default
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Wishlist Tab -->
                    <div v-if="activeTab === 'wishlist'" class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-8">
                        <div class="flex items-center justify-between mb-6">
                            <h2 class="text-2xl font-bold text-gray-900">My Wishlist</h2>
                            <router-link to="/wishlist"
                                class="px-6 py-3 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 transition-colors flex items-center gap-2">
                                View All
                            </router-link>
                        </div>

                        <div v-if="wishlistItems.length === 0" class="text-center py-12">
                            <Heart class="w-16 h-16 text-gray-300 mx-auto mb-4" />
                            <p class="text-gray-500 font-medium mb-2">Your wishlist is empty</p>
                            <p class="text-sm text-gray-400 mb-4">Start adding items you love!</p>
                            <router-link to="/product"
                                class="px-6 py-2 text-sm font-semibold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-colors inline-block">
                                Browse Products
                            </router-link>
                        </div>

                        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                            <div v-for="item in wishlistItems.slice(0, 6)" :key="item.id || item.productId"
                                class="group relative bg-gray-50 rounded-2xl border border-gray-100 overflow-hidden hover:shadow-lg transition-all">
                                <div class="aspect-square bg-gray-100 flex items-center justify-center">
                                    <img v-if="item.image || item.product?.image" 
                                        :src="item.image || item.product?.image" 
                                        :alt="item.name || item.product?.name"
                                        class="w-full h-full object-cover">
                                    <Heart v-else class="w-16 h-16 text-gray-300" />
                                </div>
                                <div class="p-4">
                                    <h3 class="font-bold text-gray-900 mb-2 truncate">
                                        {{ item.name || item.product?.name || 'Product' }}
                                    </h3>
                                    <p class="text-lg font-bold text-blue-600 mb-4">
                                        ${{ (item.price || item.product?.price || 0).toLocaleString() }}
                                    </p>
                                    <div class="flex gap-2">
                                        <router-link :to="`/product/${item.productId || item.id}`"
                                            class="flex-1 px-4 py-2 bg-gray-900 text-white rounded-xl font-semibold hover:bg-gray-800 transition-colors text-center text-sm">
                                            View
                                        </router-link>
                                        <button @click="handleRemoveFromWishlist(item.productId || item.id)"
                                            class="px-4 py-2 bg-rose-50 text-rose-600 rounded-xl font-semibold hover:bg-rose-100 transition-colors">
                                            <Trash2 class="w-4 h-4" />
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Address Modal -->
        <Teleport to="body">
            <Transition name="modal">
                <div v-if="showAddressModal" class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col">
                        <div class="p-6 border-b border-gray-100 shrink-0">
                            <div class="flex justify-between items-center">
                                <h3 class="text-2xl font-bold text-gray-900">
                                    {{ editingAddress ? 'Edit Address' : 'Add New Address' }}
                                </h3>
                                <button @click="closeAddressModal" class="p-2 hover:bg-gray-100 rounded-xl transition-colors">
                                    <X class="w-5 h-5 text-gray-500" />
                                </button>
                            </div>
                        </div>
                        <div class="p-6 overflow-y-auto flex-1">
                            <div class="space-y-4">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-2">Street Address *</label>
                                    <input v-model="addressForm.street" type="text"
                                        class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500"
                                        placeholder="Enter street address">
                                </div>
                                <div class="grid grid-cols-2 gap-4">
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">City *</label>
                                        <input v-model="addressForm.city" type="text"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500"
                                            placeholder="Enter city">
                                    </div>
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">State *</label>
                                        <input v-model="addressForm.state" type="text"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500"
                                            placeholder="Enter state">
                                    </div>
                                </div>
                                <div class="grid grid-cols-2 gap-4">
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">ZIP Code *</label>
                                        <input v-model="addressForm.zipCode" type="text"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500"
                                            placeholder="Enter ZIP code">
                                    </div>
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">Country *</label>
                                        <input v-model="addressForm.country" type="text"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500"
                                            placeholder="Enter country">
                                    </div>
                                </div>
                                <div class="flex items-center gap-2">
                                    <input v-model="addressForm.isDefault" type="checkbox" id="defaultAddress"
                                        class="w-4 h-4 text-blue-600 rounded focus:ring-blue-500">
                                    <label for="defaultAddress" class="text-sm font-medium text-gray-700">Set as default address</label>
                                </div>
                            </div>
                            <div class="flex gap-3 mt-6 pt-6 border-t border-gray-100">
                                <button @click="closeAddressModal"
                                    class="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-colors font-semibold">
                                    Cancel
                                </button>
                                <button @click="handleSaveAddress" :disabled="saving"
                                    class="flex-1 bg-gray-900 text-white py-3 rounded-xl hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-semibold flex items-center justify-center gap-2">
                                    <Loader2 v-if="saving" class="w-4 h-4 animate-spin" />
                                    <Save v-else class="w-4 h-4" />
                                    {{ saving ? 'Saving...' : 'Save Address' }}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </Transition>
        </Teleport>

        <!-- Payment Method Modal -->
        <Teleport to="body">
            <Transition name="modal">
                <div v-if="showPaymentModal" class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col">
                        <div class="p-6 border-b border-gray-100 shrink-0">
                            <div class="flex justify-between items-center">
                                <h3 class="text-2xl font-bold text-gray-900">
                                    {{ editingPayment ? 'Edit Payment Method' : 'Add New Payment Method' }}
                                </h3>
                                <button @click="closePaymentModal" class="p-2 hover:bg-gray-100 rounded-xl transition-colors">
                                    <X class="w-5 h-5 text-gray-500" />
                                </button>
                            </div>
                        </div>
                        <div class="p-6 overflow-y-auto flex-1">
                            <div class="space-y-4">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-2">Card Number *</label>
                                    <input v-model="paymentForm.cardNumber" type="text" maxlength="19"
                                        @input="paymentForm.cardNumber = formatCardNumber(paymentForm.cardNumber)"
                                        class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 font-mono"
                                        placeholder="1234 5678 9012 3456">
                                </div>
                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-2">Card Holder Name *</label>
                                    <input v-model="paymentForm.cardHolder" type="text"
                                        class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500"
                                        placeholder="John Doe">
                                </div>
                                <div class="grid grid-cols-3 gap-4">
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">Expiry Month *</label>
                                        <select v-model="paymentForm.expiryMonth"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500">
                                            <option value="">MM</option>
                                            <option v-for="month in 12" :key="month" :value="String(month).padStart(2, '0')">
                                                {{ String(month).padStart(2, '0') }}
                                            </option>
                                        </select>
                                    </div>
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">Expiry Year *</label>
                                        <select v-model="paymentForm.expiryYear"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500">
                                            <option value="">YYYY</option>
                                            <option v-for="year in 20" :key="year" :value="new Date().getFullYear() + year - 1">
                                                {{ new Date().getFullYear() + year - 1 }}
                                            </option>
                                        </select>
                                    </div>
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-2">CVV *</label>
                                        <input v-model="paymentForm.cvv" type="text" maxlength="4"
                                            class="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 font-mono"
                                            placeholder="123">
                                    </div>
                                </div>
                                <div class="flex items-center gap-2">
                                    <input v-model="paymentForm.isDefault" type="checkbox" id="defaultPayment"
                                        class="w-4 h-4 text-blue-600 rounded focus:ring-blue-500">
                                    <label for="defaultPayment" class="text-sm font-medium text-gray-700">Set as default payment method</label>
                                </div>
                            </div>
                            <div class="flex gap-3 mt-6 pt-6 border-t border-gray-100">
                                <button @click="closePaymentModal"
                                    class="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-colors font-semibold">
                                    Cancel
                                </button>
                                <button @click="handleSavePayment" :disabled="saving"
                                    class="flex-1 bg-gray-900 text-white py-3 rounded-xl hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-semibold flex items-center justify-center gap-2">
                                    <Loader2 v-if="saving" class="w-4 h-4 animate-spin" />
                                    <Save v-else class="w-4 h-4" />
                                    {{ saving ? 'Saving...' : 'Save Payment Method' }}
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
.modal-enter-active,
.modal-leave-active {
    transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
    opacity: 0;
}

.modal-enter-active > div,
.modal-leave-active > div {
    transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from > div,
.modal-leave-to > div {
    transform: scale(0.95);
    opacity: 0;
}

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

