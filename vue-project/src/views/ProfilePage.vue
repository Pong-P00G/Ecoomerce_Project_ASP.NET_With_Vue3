<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../stores/Auth'
import { profileApi } from '../api/profileApi'
import {
    User,
    Mail,
    Phone,
    MapPin,
    Calendar,
    Edit,
    Camera,
    Loader2,
    CheckCircle,
    AlertCircle
} from 'lucide-vue-next'

const authStore = useAuthStore()
const loading = ref(false)
const error = ref(null)
const success = ref(null)
const profile = ref(null)

const fetchProfile = async () => {
    try {
        loading.value = true
        error.value = null
        const response = await profileApi.getProfile()
        profile.value = response.data.data || response.data
    } catch (err) {
        error.value = err.response?.data?.message || 'Failed to load profile'
        // Fallback to auth store user
        profile.value = authStore.user
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    fetchProfile()
})
</script>

<template>
    <div class="min-h-screen bg-gray-50 py-8">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
            <!-- Header -->
            <div class="mb-8">
                <h1 class="text-3xl font-bold text-gray-900 flex items-center gap-3">
                    <User class="w-8 h-8 text-blue-600" />
                    My Profile
                </h1>
                <p class="text-gray-500 mt-2">View and manage your profile information</p>
            </div>

            <!-- Error Message -->
            <Transition name="slide-fade">
                <div v-if="error"
                    class="mb-6 bg-rose-50 border border-rose-200 rounded-2xl p-4 flex items-center gap-3">
                    <AlertCircle class="w-5 h-5 text-rose-600 shrink-0" />
                    <div class="flex-1">
                        <p class="text-sm font-semibold text-rose-900">Error</p>
                        <p class="text-xs text-rose-700 mt-0.5">{{ error }}</p>
                    </div>
                </div>
            </Transition>

            <!-- Loading State -->
            <div v-if="loading" class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-12">
                <div class="flex flex-col items-center justify-center gap-4">
                    <Loader2 class="w-8 h-8 animate-spin text-blue-600" />
                    <p class="text-gray-500 font-medium">Loading profile...</p>
                </div>
            </div>

            <!-- Profile Content -->
            <div v-else-if="profile" class="space-y-6">
                <!-- Profile Card -->
                <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm overflow-hidden">
                    <!-- Profile Header with Avatar -->
                    <div class="bg-linear-to-br from-blue-600 to-indigo-700 p-8 relative">
                        <div class="flex flex-col sm:flex-row items-center sm:items-end gap-6">
                            <div class="relative">
                                <div class="w-32 h-32 rounded-full bg-white p-1 shadow-xl">
                                    <div class="w-full h-full rounded-full bg-gray-100 flex items-center justify-center overflow-hidden">
                                        <img v-if="profile.avatar" :src="profile.avatar" :alt="profile.name" class="w-full h-full object-cover">
                                        <div v-else class="w-full h-full bg-linear-to-br from-blue-400 to-indigo-500 flex items-center justify-center">
                                            <span class="text-4xl font-bold text-white">
                                                {{ (profile.name || profile.username || 'U').charAt(0).toUpperCase() }}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="flex-1 text-center sm:text-left text-white">
                                <h2 class="text-3xl font-bold mb-2">{{ profile.name || profile.username || 'User' }}</h2>
                                <p class="text-blue-100 flex items-center justify-center sm:justify-start gap-2">
                                    <Mail class="w-4 h-4" />
                                    {{ profile.email || 'No email' }}
                                </p>
                            </div>
                            <router-link to="/settings" 
                                class="px-6 py-3 bg-white text-blue-600 rounded-xl font-semibold hover:bg-blue-50 transition-colors flex items-center gap-2">
                                <Edit class="w-4 h-4" />
                                Edit Profile
                            </router-link>
                        </div>
                    </div>

                    <!-- Profile Details -->
                    <div class="p-8">
                        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div class="flex items-start gap-4 p-4 bg-gray-50 rounded-2xl">
                                <div class="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center shrink-0">
                                    <User class="w-6 h-6 text-blue-600" />
                                </div>
                                <div>
                                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-1">Full Name</p>
                                    <p class="text-lg font-bold text-gray-900">{{ profile.name || profile.username || 'Not set' }}</p>
                                </div>
                            </div>

                            <div class="flex items-start gap-4 p-4 bg-gray-50 rounded-2xl">
                                <div class="w-12 h-12 bg-indigo-100 rounded-xl flex items-center justify-center shrink-0">
                                    <Mail class="w-6 h-6 text-indigo-600" />
                                </div>
                                <div>
                                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-1">Email Address</p>
                                    <p class="text-lg font-bold text-gray-900">{{ profile.email || 'Not set' }}</p>
                                </div>
                            </div>

                            <div v-if="profile.phone" class="flex items-start gap-4 p-4 bg-gray-50 rounded-2xl">
                                <div class="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center shrink-0">
                                    <Phone class="w-6 h-6 text-green-600" />
                                </div>
                                <div>
                                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-1">Phone Number</p>
                                    <p class="text-lg font-bold text-gray-900">{{ profile.phone }}</p>
                                </div>
                            </div>

                            <div class="flex items-start gap-4 p-4 bg-gray-50 rounded-2xl">
                                <div class="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center shrink-0">
                                    <Calendar class="w-6 h-6 text-purple-600" />
                                </div>
                                <div>
                                    <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-1">Member Since</p>
                                    <p class="text-lg font-bold text-gray-900">
                                        {{ profile.createdAt ? new Date(profile.createdAt).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' }) : 'N/A' }}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Quick Stats -->
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-6">
                        <div class="flex items-center justify-between mb-4">
                            <div class="w-12 h-12 bg-blue-50 rounded-2xl flex items-center justify-center">
                                <MapPin class="w-6 h-6 text-blue-600" />
                            </div>
                        </div>
                        <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Saved Addresses</p>
                        <p class="text-3xl font-bold text-gray-900">{{ profile.addresses?.length || 0 }}</p>
                    </div>

                    <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-6">
                        <div class="flex items-center justify-between mb-4">
                            <div class="w-12 h-12 bg-green-50 rounded-2xl flex items-center justify-center">
                                <CheckCircle class="w-6 h-6 text-green-600" />
                            </div>
                        </div>
                        <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Payment Methods</p>
                        <p class="text-3xl font-bold text-gray-900">{{ profile.paymentMethods?.length || 0 }}</p>
                    </div>

                    <div class="bg-white rounded-[32px] border border-gray-100 shadow-sm p-6">
                        <div class="flex items-center justify-between mb-4">
                            <div class="w-12 h-12 bg-purple-50 rounded-2xl flex items-center justify-center">
                                <User class="w-6 h-6 text-purple-600" />
                            </div>
                        </div>
                        <p class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Account Status</p>
                        <p class="text-3xl font-bold text-green-600">Active</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
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

