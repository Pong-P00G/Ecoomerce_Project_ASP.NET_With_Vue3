<script setup>
import { ref } from 'vue';
import { authAPI } from '../../api/authApi';
import { useRouter } from 'vue-router';
import { RouterLink } from 'vue-router';
import {
    Mail,
    ArrowRight,
    Loader2,
    AlertCircle,
    CheckCircle2,
    Sparkles,
    Lock
} from 'lucide-vue-next';

const router = useRouter();

const email = ref('');
const error = ref('');
const success = ref('');
const loading = ref(false);
const emailSent = ref(false);

const validateEmail = () => {
    if (!email.value) {
        error.value = 'Please enter your email address';
        return false;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email.value)) {
        error.value = 'Please enter a valid email address';
        return false;
    }
    return true;
};

const handleSubmit = async () => {
    error.value = '';
    success.value = '';
    if (!validateEmail()) {
        return;
    }
    loading.value = true;
    try {
        // Note: You'll need to implement forgotPassword in authAPI
        const response = await authAPI.forgotPassword({
            email: email.value
        });
        // For now, simulate success
        await new Promise(resolve => setTimeout(resolve, 1500));
        success.value = 'Password reset link has been sent to your email!';
        emailSent.value = true;
    } catch (err) {
        error.value = err.response?.data?.message || err.message || 'Failed to send reset link. Please try again.';
    } finally {
        loading.value = false;
    }
};

const resendEmail = async () => {
    emailSent.value = false;
    success.value = '';
    await handleSubmit();
};

const goToLogin = () => {
    router.push('/login');
};
</script>

<template>
  <div class="min-h-screen bg-white flex items-center justify-center px-4 py-24">
    <div class="w-full max-w-md">
      <!-- Header -->
      <div class="text-center mb-12">
        <div class="inline-flex items-center gap-2 px-3 py-1 bg-gray-100 rounded-full text-gray-600 text-xs font-bold tracking-wider uppercase mb-6">
          <Sparkles class="w-3 h-3" />
          Password Recovery
        </div>
        <h1 class="text-4xl md:text-5xl font-light text-gray-900 tracking-tight mb-4">
          Forgot your <span class="font-bold italic">Password?</span>
        </h1>
        <p class="text-gray-500 text-lg font-light">
          No worries, we'll send you reset instructions
        </p>
      </div>

      <!-- Card -->
      <div class="bg-gray-50/50 rounded-[3rem] p-12 md:p-16 shadow-sm border border-gray-100">
        <!-- Success -->
        <div v-if="emailSent && success" class="mb-6 flex items-center gap-3 rounded-2xl bg-green-50 border border-green-200 px-6 py-4">
          <CheckCircle2 class="h-5 w-5 shrink-0 text-green-600" />
          <span class="text-sm font-semibold text-green-800">{{ success }}</span>
        </div>

        <!-- Error -->
        <div v-if="error" class="mb-6 flex items-center gap-3 rounded-2xl bg-red-50 border border-red-200 px-6 py-4">
          <AlertCircle class="h-5 w-5 shrink-0 text-red-600" />
          <span class="text-sm font-semibold text-red-800">{{ error }}</span>
        </div>

        <!-- Form -->
        <div v-if="!emailSent" class="space-y-6">
          <div class="space-y-2">
            <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">
              Email Address
            </label>
            <div class="relative">
              <Mail class="absolute left-6 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input 
                v-model="email" 
                type="email" 
                placeholder="Enter your email address" 
                @keyup.enter="handleSubmit"
                required
                class="w-full bg-white border-0 rounded-2xl pl-14 pr-6 py-4 text-gray-900 placeholder-gray-400 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm outline-none" 
              />
            </div>
            <p class="text-xs text-gray-500 ml-4">
              We'll send a password reset link to this email
            </p>
          </div>

          <button 
            @click="handleSubmit" 
            :disabled="loading"
            class="w-full bg-gray-900 text-white font-bold py-5 rounded-2xl hover:bg-black transition-all duration-300 flex items-center justify-center gap-3 group shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Loader2 v-if="loading" class="h-5 w-5 animate-spin" />
            <span v-else class="flex items-center gap-3">
              Send Reset Link
              <ArrowRight class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
            </span>
          </button>

          <div class="text-center">
            <button 
              @click="goToLogin" 
              class="text-sm font-bold text-gray-500 hover:text-gray-900 transition-colors"
            >
              Back to Login
            </button>
          </div>
        </div>

        <!-- After Email Sent -->
        <div v-else class="space-y-6">
          <div class="text-center space-y-4">
            <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-green-50 text-green-600 mb-4">
              <Lock class="w-8 h-8" />
            </div>
            <p class="text-gray-500 text-sm font-light">
              Check your inbox and follow the instructions to reset your password.
            </p>
          </div>

          <button 
            @click="goToLogin"
            class="w-full bg-gray-900 text-white font-bold py-5 rounded-2xl hover:bg-black transition-all duration-300 flex items-center justify-center gap-3 group shadow-xl"
          >
            Back to Login
            <ArrowRight class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
          </button>

          <button 
            @click="resendEmail" 
            :disabled="loading"
            class="w-full border-2 border-gray-200 text-gray-900 font-bold py-5 rounded-2xl hover:bg-gray-50 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Loader2 v-if="loading" class="h-5 w-5 animate-spin mx-auto" />
            <span v-else>Resend Email</span>
          </button>
        </div>
      </div>

      <!-- Footer -->
      <p class="mt-8 text-center text-sm font-bold text-gray-500">
        Need help?
        <RouterLink to="/contact" class="font-bold text-gray-900 hover:text-violet-600 transition-colors ml-1">
          Contact Support
        </RouterLink>
      </p>
    </div>
  </div>
</template>
