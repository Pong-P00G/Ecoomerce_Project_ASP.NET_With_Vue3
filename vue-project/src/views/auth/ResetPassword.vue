<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/Auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const resetForm = ref({
    email: '',
    token: '',
    newPassword: '',
    confirmPassword: ''
})

const loading = ref(false)
const error = ref('')
const success = ref('')

onMounted(() => {
    resetForm.value.token = route.query.token || ''
    resetForm.value.email = route.query.email || ''
})

const handleResetPassword = async () => {
    try {
        loading.value = true
        error.value = ''
        success.value = ''

        if (resetForm.value.newPassword !== resetForm.value.confirmPassword) {
            error.value = 'Passwords do not match'
            return
        }
        await authStore.resetPassword(resetForm.value)
        success.value = 'Password reset successfully! Redirecting to login...'
        setTimeout(() => {
            router.push('/login')
        }, 2000)
    } catch (err) {
        error.value = err.message || 'Password reset failed'
    } finally {
        loading.value = false
    }
}
</script>

<template>
    <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
        <div class="max-w-md w-full space-y-8">
            <div>
                <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
                    Reset Your Password
                </h2>
            </div>
            <form class="mt-8 space-y-6" @submit.prevent="handleResetPassword">
                <div v-if="success" class="bg-green-50 border border-green-400 text-green-700 px-4 py-3 rounded">
                    {{ success }}
                </div>
                <div v-if="error" class="bg-red-50 border border-red-400 text-red-700 px-4 py-3 rounded">
                    {{ error }}
                </div>
                <div class="space-y-4">
                    <div>
                        <label for="newPassword" class="block text-sm font-medium text-gray-700">
                            New Password
                        </label>
                        <input id="newPassword" v-model="resetForm.newPassword" type="password" required
                            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md" />
                    </div>
                    <div>
                        <label for="confirmPassword" class="block text-sm font-medium text-gray-700">
                            Confirm Password
                        </label>
                        <input id="confirmPassword" v-model="resetForm.confirmPassword" type="password" required
                            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md" />
                    </div>
                </div>
                <button type="submit" :disabled="loading"
                    class="w-full py-2 px-4 border border-transparent rounded-md text-white bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50">
                    {{ loading ? 'Resetting...' : 'Reset Password' }}
                </button>
            </form>
        </div>
    </div>
</template>