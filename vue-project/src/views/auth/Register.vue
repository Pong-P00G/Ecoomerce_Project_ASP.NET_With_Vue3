<script setup>
import { ref } from 'vue';
import { authAPI } from '../../api/authApi';
import { useRouter } from 'vue-router';
import { RouterLink } from 'vue-router';
import {
    User,
    Mail,
    Lock,
    Eye,
    EyeOff,
    ArrowRight,
    Loader2,
    ShieldCheck,
    AlertCircle,
    CheckCircle2,
    ChevronDown,
    Sparkles
} from 'lucide-vue-next';

const router = useRouter();

const formData = ref({
    username: '',
    firstname: '',
    lastname: '',
    email: '',
    password: '',
    confirmPassword: '',
    roleId: 'customer'
});

const showPassword = ref(false);
const showConfirmPassword = ref(false);
const error = ref('');
const loading = ref(false);
const acceptTerms = ref(false);

const validateForm = () => {
    if (!formData.value.username || !formData.value.email || !formData.value.password || !formData.value.confirmPassword) {
        error.value = 'Please fill in all required fields';
        return false;
    }
    if (!formData.value.firstname || !formData.value.lastname) {
        error.value = 'First name and last name are required';
        return false;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(formData.value.email)) {
        error.value = 'Invalid email address format';
        return false;
    }
    if (formData.value.username.length < 3) {
        error.value = 'Username must be at least 3 characters';
        return false;
    }
    if (formData.value.password.length < 8) {
        error.value = 'Password must be at least 8 characters';
        return false;
    }
    if (formData.value.password !== formData.value.confirmPassword) {
        error.value = 'Passwords do not match';
        return false;
    }
    if (!acceptTerms.value) {
        error.value = 'Please accept terms and conditions';
        return false;
    }
    return true;
};

const handleRegister = async () => {
    error.value = '';
    if (!validateForm()) {
        return;
    }
    loading.value = true;
    try {
        const response = await authAPI.register({
            username: formData.value.username,
            firstName: formData.value.firstname || null,
            lastName: formData.value.lastname || null,
            email: formData.value.email,
            password: formData.value.password,
            roleId: formData.value.roleId || 'customer'
        });

        // Registration successful - redirect to login
        if (response && response.status === 200 || response && response.status === 201) {
            // Optionally show success message or automatically redirect
            router.push('/login');
        }
    } catch (err) {
        console.error('Registration error:', err);
        if (err.code === 'ECONNABORTED' || err.message?.includes('timeout')) {
            error.value = 'Request timeout. Please check your connection and try again.';
        } else if (!err.response) {
            error.value = 'Network error. Please check if the server is running.';
        } else {
            // Handle validation errors and other API errors
            const errorMessage = err.response?.data?.message ||
                err.response?.data?.error ||
                (typeof err.response?.data === 'string' ? err.response.data : null) ||
                err.message ||
                'Registration failed. Please try again.';
            error.value = errorMessage;
        }
    } finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="min-h-screen bg-white flex items-center justify-center px-4 py-24">
        <div class="w-full max-w-2xl">
            <!-- Header -->
            <div class="text-center mb-12">
                <div
                    class="inline-flex items-center gap-2 px-3 py-1 bg-gray-100 rounded-full text-gray-600 text-xs font-bold tracking-wider uppercase mb-6">
                    <Sparkles class="w-3 h-3" />
                    Join Us
                </div>
                <h1 class="text-4xl md:text-5xl font-light text-gray-900 tracking-tight mb-4">
                    Create your <span class="font-bold italic">Account</span>
                </h1>
                <p class="text-gray-500 text-lg font-light">
                    Start your journey with AlieeShop today
                </p>
            </div>

            <!-- Card -->
            <div class="bg-gray-50/50 rounded-[3rem] p-12 md:p-16 shadow-sm border border-gray-100">
                <!-- Error -->
                <div v-if="error"
                    class="mb-6 flex items-center gap-3 rounded-2xl bg-red-50 border border-red-200 px-6 py-4">
                    <AlertCircle class="h-5 w-5 shrink-0 text-red-600" />
                    <span class="text-sm font-semibold text-red-800">{{ error }}</span>
                </div>

                <form @submit.prevent="handleRegister" class="space-y-6">
                    <!-- Name Fields -->
                    <div class="grid grid-cols-2 gap-6">
                        <div class="space-y-2">
                            <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                                First Name
                            </label>
                            <div class="relative">
                                <User class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                                <input v-model="formData.firstname" type="text" placeholder="John"
                                    class="w-full bg-white border-0 rounded-2xl pl-14 pr-6 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" />
                            </div>
                        </div>
                        <div class="space-y-2">
                            <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                                Last Name
                            </label>
                            <div class="relative">
                                <User class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                                <input v-model="formData.lastname" type="text" placeholder="Doe"
                                    class="w-full bg-white border-0 rounded-2xl pl-14 pr-6 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" />
                            </div>
                        </div>
                    </div>

                    <!-- Username -->
                    <div class="space-y-2">
                        <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                            Username
                        </label>
                        <div class="relative">
                            <User class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                            <input v-model="formData.username" type="text" placeholder="johndoe" required
                                class="w-full bg-white border-0 rounded-2xl pl-14 pr-6 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" />
                        </div>
                    </div>

                    <!-- Email -->
                    <div class="space-y-2">
                        <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                            Email Address
                        </label>
                        <div class="relative">
                            <Mail class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                            <input v-model="formData.email" type="email" placeholder="john@example.com" required
                                class="w-full bg-white border-0 rounded-2xl pl-14 pr-6 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" />
                        </div>
                    </div>

                    <!-- Password Fields -->
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div class="space-y-2">
                            <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                                Password
                            </label>
                            <div class="relative">
                                <Lock class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                                <input v-model="formData.password" :type="showPassword ? 'text' : 'password'"
                                    placeholder="••••••••" required
                                    class="w-full bg-white border-0 rounded-2xl pl-14 pr-14 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" />
                                <button type="button" @click="showPassword = !showPassword"
                                    class="absolute right-6 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors">
                                    <component :is="showPassword ? EyeOff : Eye" class="h-5 w-5" />
                                </button>
                            </div>
                        </div>
                        <div class="space-y-2">
                            <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                                Confirm Password
                            </label>
                            <div class="relative">
                                <ShieldCheck class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                                <input v-model="formData.confirmPassword"
                                    :type="showConfirmPassword ? 'text' : 'password'" placeholder="••••••••" required
                                    class="w-full bg-white border-0 rounded-2xl pl-14 pr-14 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" />
                                <button type="button" @click="showConfirmPassword = !showConfirmPassword"
                                    class="absolute right-6 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors">
                                    <component :is="showConfirmPassword ? EyeOff : Eye" class="h-5 w-5" />
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Role Selection -->
                    <div class="space-y-2">
                        <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
                            Account Type
                        </label>
                        <div class="relative">
                            <ShieldCheck
                                class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400 pointer-events-none" />
                            <select v-model="formData.roleId"
                                class="w-full bg-white border-0 rounded-2xl pl-14 pr-10 py-4 text-gray-900 font-bold appearance-none focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none cursor-pointer">
                                <option value="customer">Customer</option>
                                <option value="admin">Admin</option>
                            </select>
                            <ChevronDown
                                class="absolute right-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400 pointer-events-none" />
                        </div>
                    </div>

                    <!-- Terms Checkbox -->
                    <div class="flex items-start gap-3 px-1">
                        <label class="relative flex items-center mt-1 cursor-pointer group">
                            <input v-model="acceptTerms" type="checkbox" class="peer sr-only" required />
                            <div
                                class="w-5 h-5 border-2 border-gray-300 rounded-lg group-hover:border-gray-900 peer-checked:bg-gray-900 peer-checked:border-gray-900 transition-all duration-200">
                            </div>
                            <div
                                class="absolute inset-0 flex items-center justify-center text-white scale-0 peer-checked:scale-100 transition-transform duration-200">
                                <CheckCircle2 class="w-3 h-3" />
                            </div>
                        </label>
                        <span class="text-sm font-bold text-gray-600 leading-relaxed">
                            I accept the
                            <RouterLink to="/terms" class="text-gray-900 hover:text-violet-600 transition-colors">
                                Terms of Service
                            </RouterLink>
                            <span class="text-gray-600"> and </span>
                            <RouterLink to="/terms" class="text-gray-900 hover:text-violet-600 transition-colors">
                                Privacy Policy
                            </RouterLink>
                        </span>
                    </div>

                    <!-- Submit Button -->
                    <button type="submit" :disabled="loading"
                        class="w-full bg-gray-900 text-white font-bold py-5 rounded-2xl hover:bg-black transition-all duration-300 flex items-center justify-center gap-3 group shadow-xl disabled:opacity-50 disabled:cursor-not-allowed">
                        <Loader2 v-if="loading" class="h-5 w-5 animate-spin" />
                        <span v-else class="flex items-center gap-3">
                            Create Account
                            <ArrowRight class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                        </span>
                    </button>
                </form>
            </div>

            <!-- Footer -->
            <p class="mt-8 text-center text-sm font-bold text-gray-500">
                Already have an account?
                <RouterLink to="/login" class="font-bold text-gray-900 hover:text-violet-600 transition-colors ml-1">
                    Sign In
                </RouterLink>
            </p>
        </div>
    </div>
</template>
