<script setup>
import { ref } from 'vue';
import { authApi } from '../../api/authApi';
import { useRouter } from 'vue-router';
import { 
  User, 
  Mail, 
  Lock, 
  Eye, 
  EyeOff, 
  ArrowRight, 
  Loader2, 
  ShieldCheck,
  UserCircle,
  AlertCircle,
  CheckCircle2,
  ChevronDown
} from 'lucide-vue-next';

const router = useRouter();

const formData = ref({
    username: '',
    firstname:'',
    lastname:'',
    email: '',
    password: '',
    confirmPassword: '',
    role: 'customer'
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
        const response = await authApi.register({
            username: formData.value.username,
            firstname: formData.value.firstname,
            lastname: formData.value.lastname,
            email: formData.value.email,
            password: formData.value.password,
            role: formData.value.role
        });
        
        if (response.data.token) {
            localStorage.setItem('authToken', response.data.token);
            if (response.data.refreshToken) {
                localStorage.setItem('refreshToken', response.data.refreshToken);
            }
        }
        if (response.data.user) {
            localStorage.setItem('user', JSON.stringify(response.data.user));
        }
        
        const userRole = response.data.user?.role || response.data.role || formData.value.role;
        if (userRole === 'admin') {
            await router.push('/admin/dashboard');
        } else {
            await router.push('/');
        }
    } catch (err) {
        error.value = err.response?.data?.message || err.message || 'Registration failed';
    } finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="min-h-screen flex items-center justify-center p-6 inset-0 bg-[radial-gradient(#e5e7eb_1px,transparent_1px)] bg-size-[18px_18px]">
        <div class="w-full max-w-125">
            <div class="text-center mb-10">
                <div class="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-indigo-600 text-white shadow-lg shadow-indigo-200 mb-4 transform rotate-6 hover:rotate-0 transition-transform duration-300">
                    <UserCircle class="w-10 h-10" />
                </div>
                <h1 class="text-3xl font-bold text-gray-900 tracking-tight">Create Account</h1>
                <p class="text-gray-500 mt-2 font-medium">Join the AlieeShop ecosystem today</p>
            </div>
            <div class="bg-white rounded-32px shadow-xl shadow-gray-200/50 border border-gray-100 p-10 relative overflow-hidden">
                <div class="absolute top-0 left-0 w-32 h-32 bg-indigo-50 rounded-full -ml-16 -mt-16 opacity-50"></div>
                <div v-if="error" class="bg-rose-50 border border-rose-100 text-rose-600 p-4 rounded-2xl mb-8 flex items-center gap-3 animate-shake relative z-10">
                    <AlertCircle class="w-5 h-5 shrink-0" />
                    <span class="text-sm font-semibold">{{ error }}</span>
                </div>
                <div class="space-y-6 relative z-10">
                    <div class="grid grid-cols-2 gap-4">
                        <div>
                            <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">First Name</label>
                            <input v-model="formData.firstname" type="text" placeholder="John"
                                class="w-full px-4 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-medium placeholder:text-gray-400 transition-all outline-none" />
                        </div>
                        <div>
                            <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">Last Name</label>
                            <input v-model="formData.lastname" type="text" placeholder="Doe"
                                class="w-full px-4 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-medium placeholder:text-gray-400 transition-all outline-none" />
                        </div>
                    </div>
                    <div>
                        <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">Username</label>
                        <div class="relative group">
                            <div class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-600 transition-colors">
                                <User class="w-4 h-4" />
                            </div>
                            <input v-model="formData.username" type="text" placeholder="johndoe"
                                class="w-full pl-12 pr-4 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-medium placeholder:text-gray-400 transition-all outline-none" />
                        </div>
                    </div>
                    <div>
                        <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">Email Address</label>
                        <div class="relative group">
                            <div class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-600 transition-colors">
                                <Mail class="w-4 h-4" />
                            </div>
                            <input v-model="formData.email" type="email" placeholder="john@example.com"
                                class="w-full pl-12 pr-4 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-medium placeholder:text-gray-400 transition-all outline-none" />
                        </div>
                    </div>
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">Password</label>
                            <div class="relative group">
                                <div class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-600 transition-colors">
                                    <Lock class="w-4 h-4" />
                                </div>
                                <input v-model="formData.password" :type="showPassword ? 'text' : 'password'" placeholder="••••••••"
                                    class="w-full pl-12 pr-10 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-medium placeholder:text-gray-400 transition-all outline-none" />
                                <button type="button" @click="showPassword = !showPassword" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors">
                                    <component :is="showPassword ? EyeOff : Eye" class="w-4 h-4" />
                                </button>
                            </div>
                        </div>
                        <div>
                            <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">Confirm</label>
                            <div class="relative group">
                                <div class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-600 transition-colors">
                                    <ShieldCheck class="w-4 h-4" />
                                </div>
                                <input v-model="formData.confirmPassword" :type="showConfirmPassword ? 'text' : 'password'" placeholder="••••••••"
                                    class="w-full pl-12 pr-10 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-medium placeholder:text-gray-400 transition-all outline-none" />
                                <button type="button" @click="showConfirmPassword = !showConfirmPassword" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors">
                                    <component :is="showConfirmPassword ? EyeOff : Eye" class="w-4 h-4" />
                                </button>
                            </div>
                        </div>
                    </div>
                    <div>
                        <label class="block text-[10px] font-bold text-gray-400 uppercase tracking-widest mb-2 ml-1">User Role</label>
                        <div class="relative group">
                            <div class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-600 transition-colors pointer-events-none">
                                <ShieldCheck class="w-4 h-4" />
                            </div>
                            <select v-model="formData.role"
                                class="w-full pl-12 pr-10 py-3.5 bg-gray-50 border-none rounded-2xl focus:ring-2 focus:ring-indigo-600/20 text-gray-900 font-bold appearance-none transition-all outline-none cursor-pointer">
                                <option value="customer">Customer</option>
                                <option value="admin">Admin</option>
                            </select>
                            <div class="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
                                <ChevronDown class="w-4 h-4" />
                            </div>
                        </div>
                    </div>
                    <div class="flex items-start gap-3 px-1">
                        <label class="relative flex items-center mt-1 cursor-pointer group">
                            <input v-model="acceptTerms" type="checkbox" class="peer sr-only" />
                            <div class="w-5 h-5 border-2 border-gray-200 rounded-lg group-hover:border-indigo-400 peer-checked:bg-indigo-600 peer-checked:border-indigo-600 transition-all duration-200"></div>
                            <div class="absolute inset-0 flex items-center justify-center text-white scale-0 peer-checked:scale-100 transition-transform duration-200">
                                <CheckCircle2 class="w-3.5 h-3.5" />
                            </div>
                        </label>
                        <span class="text-xs font-bold text-gray-500 leading-relaxed">
                            I accept the 
                            <RouterLink to="/terms" class="text-indigo-600 hover:text-indigo-700">
                              Terms of Protocol
                              <span class="text-xs font-bold text-gray-500">and</span>
                              <span class="text-indigo-600 hover:text-indigo-700"> Privacy Directives</span>
                            </RouterLink>
                        </span>
                    </div>
                    <button @click="handleRegister" :disabled="loading"
                        class="w-full bg-gray-900 text-white py-4 rounded-2xl font-bold hover:bg-gray-800 active:scale-[0.98] transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-gray-200 flex items-center justify-center gap-2 group">
                        <Loader2 v-if="loading" class="w-5 h-5 animate-spin" />
                        <span v-else class="flex items-center gap-2">
                            Initialize Account
                            <ArrowRight class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                        </span>
                    </button>
                </div>
            </div>

            <div class="mt-8 text-center">
                <p class="text-sm font-bold text-gray-400 tracking-wide uppercase">
                    Already registered?
                    <RouterLink to="/login" class="text-indigo-600 hover:text-indigo-700 ml-2 transition-colors">
                        Sign In
                    </RouterLink>
                </p>
            </div>
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