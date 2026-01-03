 <script setup>
import { ref } from 'vue';
import { authApi } from '../../api/authApi';
import { useRouter } from 'vue-router';

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
        const response = await authApi.forgotPassword({
            email: email.value
        });
        success.value = response.data?.message || 'Password reset link has been sent to your email!';
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
  <div class="min-h-screen bg-[#f9fafb] relative flex items-center justify-center px-4">
    <!-- dotted background -->
    <div class="absolute inset-0 bg-[radial-gradient(#e5e7eb_1px,transparent_1px)] background-size:18px_18px"></div>

    <div class="relative w-full max-w-md">
      <!-- Header -->
      <div class="text-center mb-8">
        <div class="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-xl bg-indigo-600 text-white shadow">
          <svg class="h-7 w-7" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
          </svg>
        </div>

        <h1 class="text-2xl font-semibold text-gray-900">Forgot Password</h1>
        <p class="mt-1 text-sm text-gray-500">
          Recover access to your account
        </p>
      </div>

      <!-- Card -->
      <div class="rounded-2xl bg-white p-8 shadow-lg">
        <!-- Success -->
        <div v-if="emailSent && success"
          class="mb-6 rounded-lg border border-green-200 bg-green-50 px-4 py-3 text-sm text-green-700">
          {{ success }}
        </div>

        <!-- Error -->
        <div v-if="error" class="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600">
          {{ error }}
        </div>

        <!-- Form -->
        <div v-if="!emailSent" class="space-y-6">
          <div>
            <p class="mb-2 text-xs font-semibold uppercase tracking-wide text-gray-400">
              Identity
            </p>
            <div class="relative">
              <input v-model="email" type="email" placeholder="Email address" @keyup.enter="handleSubmit"
                class="w-full rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-sm text-gray-900 placeholder-gray-400 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/20 outline-none" />
            </div>
          </div>

          <button @click="handleSubmit" :disabled="loading"
            class="flex w-full items-center justify-center gap-2 rounded-xl bg-gray-900 py-3 text-sm font-semibold text-white transition hover:bg-gray-800 disabled:opacity-50">
            {{ loading ? 'Sending...' : 'Send reset link' }}
            <span>→</span>
          </button>

          <div class="text-center">
            <button @click="goToLogin" class="text-sm font-medium text-indigo-600 hover:text-indigo-700">
              Back to login
            </button>
          </div>
        </div>

        <!-- After Email Sent -->
        <div v-else class="space-y-4">
          <button @click="goToLogin"
            class="w-full rounded-xl bg-gray-900 py-3 text-sm font-semibold text-white hover:bg-gray-800">
            Back to Login
          </button>

          <button @click="resendEmail" :disabled="loading"
            class="w-full rounded-xl border border-gray-300 py-3 text-sm font-semibold text-gray-700 hover:bg-gray-50 disabled:opacity-50">
            {{ loading ? 'Resending...' : 'Resend Email' }}
          </button>
        </div>
      </div>
      <!-- Footer -->
      <p class="mt-6 text-center text-xs text-gray-500">
        Need help?
        <RouterLink to="/contact-support" class="font-medium text-indigo-600 hover:text-indigo-700">
          Contact support
        </RouterLink>
      </p>
    </div>
  </div>
</template>