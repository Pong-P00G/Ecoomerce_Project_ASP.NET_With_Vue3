<script setup>
import { reactive, ref } from "vue";

const success = ref(false);

const form = reactive({
    name: "",
    email: "",
    message: "",
});

const submitForm = () => {
    console.log("Support Message:", form);

    // Reset form
    form.name = "";
    form.email = "";
    form.message = "";

    success.value = true;
    setTimeout(() => (success.value = false), 3000);
};
</script>

<template>
    <div class="min-h-screen flex items-center justify-center dotted-bg px-4">
        <div class="bg-white/90 backdrop-blur rounded-2xl shadow-xl w-full max-w-lg p-8">
            <h2 class="text-2xl font-bold text-gray-800 mb-2">Contact Support</h2>
            <p class="text-gray-500 mb-6">
                Need help? Send us a message and we’ll get back to you.
            </p>

            <form @submit.prevent="submitForm" class="space-y-4">
                <!-- Name -->
                <div>
                    <label class="block text-sm font-medium text-gray-700">Name</label>
                    <input v-model="form.name" type="text" required
                        class="mt-1 w-full rounded-xl border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-black"
                        placeholder="Your name" />
                </div>

                <!-- Email -->
                <div>
                    <label class="block text-sm font-medium text-gray-700">Email</label>
                    <input v-model="form.email" type="email" required
                        class="mt-1 w-full rounded-xl border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-black"
                        placeholder="you@example.com" />
                </div>

                <!-- Message -->
                <div>
                    <label class="block text-sm font-medium text-gray-700">Message</label>
                    <textarea v-model="form.message" rows="4" required
                        class="mt-1 w-full rounded-xl border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-black"
                        placeholder="Describe your issue..."></textarea>
                </div>

                <!-- Button -->
                <button type="submit" class="w-full bg-black text-white py-2 rounded-xl hover:bg-gray-800 transition">
                    Send Message
                </button>

                <p v-if="success" class="text-green-600 text-sm text-center">
                    Message sent successfully!
                </p>
            </form>
        </div>
    </div>
</template>

<style scoped>
/* Dotted background */
.dotted-bg {
    background-color: #f9fafb;
    background-image: radial-gradient(#d1d5db 1px, transparent 1px);
    background-size: 20px 20px;
}
</style>
