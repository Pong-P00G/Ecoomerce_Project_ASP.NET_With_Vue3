<script setup>
import { ref } from 'vue';
import { authApi } from '../../api/authApi';
import { useRouter } from 'vue-router';
import {
  Mail,
  Lock,
  Eye,
  EyeOff,
  ArrowRight,
  Loader2,
  User,
  ShieldCheck,
  AlertCircle, CheckCircle2
} from 'lucide-vue-next';

const router = useRouter();

const emailOrUsername = ref('');
const password = ref('');
const rememberMe = ref(false);
const showPassword = ref(false);
const error = ref('');
const loading = ref(false);

const handleLogin = async () => {
    error.value = '';
    
    if (!emailOrUsername.value || !password.value) {
        error.value = 'Please fill in all fields';
        return;
    }
    
    loading.value = true;
    
    try {
        const response = await authApi.login({
            emailOrUsername: emailOrUsername.value,
            password: password.value
        });
        
        // Store token based on remember me preference
        if (response.data.token) {
            if (rememberMe.value) {
                localStorage.setItem('authToken', response.data.token);
                if (response.data.refreshToken) {
                    localStorage.setItem('refreshToken', response.data.refreshToken);
                }
            } else {
                sessionStorage.setItem('authToken', response.data.token);
                if (response.data.refreshToken) {
                    sessionStorage.setItem('refreshToken', response.data.refreshToken);
                }
            }
        }
        
        // Store user data if needed
        if (response.data.user) {
            localStorage.setItem('user', JSON.stringify(response.data.user));
        }
        // Success message
        alert('Login successful!');
        // Redirect based on user role
        const userRole = response.data.user?.role || response.data.role;
        
        if (userRole === 'admin') {
            router.push('/admin/dashboard');
        } else if (userRole === 'customer') {
            router.push('/');
        } else {
            router.push('/');
        }
        
    } catch (err) {
        error.value = err.response?.data?.message || err.message || 'Login failed. Please try again.';
    } finally {
        loading.value = false;
    }
};
</script>

<template>
  <div class="min-h-screen bg-[#f9fafb] relative flex items-center justify-center px-4">
    <!-- dotted background -->
    <div class="absolute inset-0 bg-[radial-gradient(#e5e7eb_1px,transparent_1px)] bg-size-[18px_18px]"></div>
    <div class="relative w-full max-w-md">
      <!-- Header -->
      <div class="text-center mb-8">
        <div class="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-xl bg-indigo-600 text-white shadow">
          <ShieldCheck class="h-7 w-7" />
        </div>
        <h1 class="text-2xl font-semibold text-gray-900">Welcome Back</h1>
        <p class="mt-1 text-sm text-gray-500">
          Elevate your experience with AlieeShop
        </p>
      </div>
      <!-- Card -->
      <div class="rounded-2xl bg-white p-8 shadow-lg">
        <!-- Error -->
        <div v-if="error" class="mb-6 flex items-center gap-2 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600">
          <AlertCircle class="h-5 w-5 shrink-0" />
          {{ error }}
        </div>
        <div class="space-y-6">
          <!-- Identity -->
          <div>
            <p class="mb-2 text-xs font-semibold uppercase tracking-wide text-gray-400">
              Identity
            </p>
            <div class="relative">
              <User class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input v-model="emailOrUsername" type="text" placeholder="Email or Username" @keyup.enter="handleLogin"
                  class="w-full rounded-xl border border-gray-200 bg-gray-50 px-12 py-3 text-sm text-gray-900 placeholder-gray-400 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/20 outline-none"/>
            </div>
          </div>
          <!-- Security -->
          <div>
            <div class="mb-2 flex items-center justify-between">
              <p class="text-xs font-semibold uppercase tracking-wide text-gray-400">
                Security
              </p>
            </div>
            <div class="relative">
              <Lock class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input v-model="password" :type="showPassword ? 'text' : 'password'" placeholder="Password" @keyup.enter="handleLogin"
                  class="w-full rounded-xl border border-gray-200 bg-gray-50 px-12 py-3 text-sm text-gray-900 placeholder-gray-400 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/20 outline-none"/>
              <button type="button" @click="showPassword = !showPassword" class="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                <component :is="showPassword ? EyeOff : Eye" class="h-5 w-5" />
              </button>
            </div>
            <RouterLink to="/forgot-password" class="text-xs font-medium text-indigo-600 hover:text-indigo-700">
              Forgot Password?
            </RouterLink>
          </div>
          <!-- Remember -->
          <div class="flex items-center gap-3 px-1">
            <label class="relative flex items-center mt-1 cursor-pointer group">
              <input v-model="rememberMe" type="checkbox" class="peer sr-only" />
              <div class="w-5 h-5 border-2 border-gray-200 rounded-lg group-hover:border-indigo-400 peer-checked:bg-indigo-600 peer-checked:border-indigo-600 transition-all duration-200"></div>
              <div class="absolute inset-0 flex items-center justify-center text-white scale-0 peer-checked:scale-100 transition-transform duration-200">
                <CheckCircle2 class="w-3.5 h-3.5" />
              </div>
            </label>
            <span class="text-sm font-medium text-gray-600 peer-checked:text-gray-900">
                Remember Me
            </span>
          </div>

          <!-- Button -->
          <button @click="handleLogin" :disabled="loading"
              class="flex w-full items-center justify-center gap-2 rounded-xl bg-gray-900 py-3 text-sm font-semibold text-white transition hover:bg-gray-800 disabled:opacity-50">
            <Loader2 v-if="loading" class="h-5 w-5 animate-spin" />
            <span v-else class="flex items-center gap-2">
              Authenticate
              <ArrowRight class="h-4 w-4" />
            </span>
          </button>
        </div>
      </div>
      <!-- Footer -->
      <p class="mt-6 text-center text-sm font-bold text-gray-500">
        New to the platform?
        <RouterLink to="/register" class="font-medium text-indigo-600 hover:text-indigo-700">
          Sign Up
        </RouterLink>
      </p>
    </div>
  </div>
</template>


<style scoped>
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-4px); }
  75% { transform: translateX(4px); }
}

.animate-shake {
  animation: shake 0.4s ease-in-out 0s 2;
}
</style>