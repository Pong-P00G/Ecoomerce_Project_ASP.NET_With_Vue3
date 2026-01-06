<script setup>
import { ref } from 'vue';
import {
    Mail,
    Phone,
    MapPin,
    Clock,
    Send,
    MessageSquare,
    Headphones,
    FileText,
    CheckCircle
} from 'lucide-vue-next';

// Form State
const formData = ref({
    name: '',
    email: '',
    phone: '',
    subject: '',
    message: ''
});

const formSubmitted = ref(false);
const isSubmitting = ref(false);

// Contact Information
const contactInfo = ref([
    {
        icon: Phone,
        title: 'Phone',
        details: '+1 (234) 567-890',
        description: 'Mon-Fri from 8am to 6pm',
        link: 'tel:+1234567890',
        color: 'from-blue-500 to-cyan-500'
    },
    {
        icon: Mail,
        title: 'Email',
        details: 'support@alieeshop.com',
        description: 'We reply within 24 hours',
        link: 'mailto:support@alieeshop.com',
        color: 'from-emerald-500 to-teal-500'
    },
    {
        icon: MapPin,
        title: 'Office',
        details: '123 Commerce St',
        description: 'Phnom Penh, Cambodia',
        link: 'https://maps.google.com',
        color: 'from-violet-500 to-purple-500'
    },
    {
        icon: Clock,
        title: 'Working Hours',
        details: 'Mon-Fri: 8am - 6pm',
        description: 'Sat-Sun: 10am - 4pm',
        link: null,
        color: 'from-amber-500 to-orange-500'
    },
]);

// Support Options
const supportOptions = ref([
    {
        icon: MessageSquare,
        title: 'Live Chat',
        description: 'Chat with our support team in real-time',
        action: 'Start Chat',
        color: 'from-blue-500 to-cyan-500'
    },
    {
        icon: Headphones,
        title: 'Call Support',
        description: 'Speak directly with our customer service',
        action: 'Call Now',
        color: 'from-emerald-500 to-teal-500'
    },
    {
        icon: FileText,
        title: 'Help Center',
        description: 'Browse our comprehensive FAQ and guides',
        action: 'Visit Help Center',
        color: 'from-violet-500 to-purple-500'
    },
]);

// Form Submission
const submitForm = async () => {
    isSubmitting.value = true;

    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1500));

    console.log('Form submitted:', formData.value);

    formSubmitted.value = true;
    isSubmitting.value = false;

    // Reset form
    formData.value = {
        name: '',
        email: '',
        phone: '',
        subject: '',
        message: ''
    };

    // Reset success message after 5 seconds
    setTimeout(() => {
        formSubmitted.value = false;
    }, 5000);
};
</script>

<template>
    <div class="min-h-screen bg-linear-to-br from-indigo-50 via-purple-50 to-pink-50">

        <!-- Hero Section -->
        <section
            class="relative bg-linear-to-r from-indigo-600 via-purple-600 to-pink-600 text-white py-28 overflow-hidden">
            <!-- Animated Background Shapes -->
            <div class="absolute inset-0 overflow-hidden">
                <div class="absolute -top-40 -right-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse"></div>
                <div class="absolute -bottom-40 -left-40 w-96 h-96 bg-white/10 rounded-full blur-3xl animate-pulse" style="animation-delay: 1s;"></div>
                <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-72 h-72 bg-white/5 rounded-full blur-2xl"></div>
            </div>

            <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center space-y-8 max-w-4xl mx-auto">
                    <div
                        class="inline-flex items-center gap-2 px-6 py-3 bg-white/20 backdrop-blur-xl rounded-full text-white text-sm font-bold shadow-lg border border-white/30 hover:bg-white/30 transition-all duration-300">
                        <Mail class="w-5 h-5" />
                        Let's Connect
                    </div>

                    <h1 class="text-6xl md:text-7xl font-extrabold leading-tight tracking-tight">
                        Get in Touch
                        <span class="block bg-linear-to-r from-yellow-300 via-pink-300 to-white bg-clip-text text-transparent mt-2">
                            We're Here to Help
                        </span>
                    </h1>

                    <p class="text-xl md:text-2xl text-indigo-100 leading-relaxed max-w-2xl mx-auto font-light">
                        Have questions? Need support? We're available 24/7 to assist you with anything you need.
                    </p>
                </div>
            </div>
        </section>

        <!-- Contact Information Cards -->
        <section class="py-16 -mt-16 relative z-10">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                    <div v-for="info in contactInfo" :key="info.title"
                        class="bg-white/90 backdrop-blur-sm p-8 rounded-3xl shadow-xl hover:shadow-2xl hover:-translate-y-2 transition-all duration-500 border-2 border-purple-100 group hover:border-purple-300">
                        <div
                            :class="`inline-flex items-center justify-center w-16 h-16 bg-linear-to-br ${info.color} rounded-2xl shadow-xl group-hover:scale-110 group-hover:rotate-6 transition-all duration-500 mb-5`">
                            <component :is="info.icon" class="w-8 h-8 text-white" />
                        </div>
                        <h3 class="text-xl font-extrabold text-gray-900 mb-3">{{ info.title }}</h3>
                        <a v-if="info.link" :href="info.link"
                            class="text-purple-600 font-bold hover:text-purple-700 transition-colors block mb-2 text-lg">
                            {{ info.details }}
                        </a>
                        <p v-else class="text-gray-900 font-bold mb-2 text-lg">{{ info.details }}</p>
                        <p class="text-sm text-gray-600 leading-relaxed">{{ info.description }}</p>
                    </div>
                </div>
            </div>
        </section>

        <!-- Main Content -->
        <section class="py-16">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid lg:grid-cols-2 gap-12">

                    <!-- Contact Form -->
                    <div>
                        <div class="mb-10">
                            <h2 class="text-4xl md:text-5xl font-extrabold text-gray-900 mb-4 leading-tight">
                                Send Us a <span
                                    class="bg-linear-to-r from-purple-600 via-pink-600 to-indigo-600 bg-clip-text text-transparent">Message</span>
                            </h2>
                            <p class="text-gray-600 text-lg leading-relaxed">
                                Fill out the form and our team will respond within 24 hours.
                            </p>
                        </div>

                        <!-- Success Message -->
                        <transition name="fade">
                            <div v-if="formSubmitted"
                                class="mb-8 p-5 bg-linear-to-r from-green-50 to-emerald-50 border-2 border-green-300 rounded-2xl flex items-center gap-4 shadow-lg">
                                <CheckCircle class="w-6 h-6 text-green-600 shrink-0" />
                                <p class="text-green-800 font-semibold text-lg">Thank you! Your message has been sent
                                    successfully.</p>
                            </div>
                        </transition>

                        <form @submit.prevent="submitForm" class="space-y-6">
                            <!-- Name -->
                            <div>
                                <label for="name" class="block text-sm font-bold text-gray-900 mb-2">
                                    Full Name <span class="text-red-500">*</span>
                                </label>
                                <input id="name" v-model="formData.name" type="text" required placeholder="John Doe"
                                    class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors" />
                            </div>

                            <!-- Email and Phone -->
                            <div class="grid md:grid-cols-2 gap-4">
                                <div>
                                    <label for="email" class="block text-sm font-bold text-gray-900 mb-2">
                                        Email Address <span class="text-red-500">*</span>
                                    </label>
                                    <input id="email" v-model="formData.email" type="email" required
                                        placeholder="john@example.com"
                                        class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors" />
                                </div>
                                <div>
                                    <label for="phone" class="block text-sm font-bold text-gray-900 mb-2">
                                        Phone Number
                                    </label>
                                    <input id="phone" v-model="formData.phone" type="tel" placeholder="+1 (234) 567-890"
                                        class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors" />
                                </div>
                            </div>

                            <!-- Subject -->
                            <div>
                                <label for="subject" class="block text-sm font-bold text-gray-900 mb-2">
                                    Subject <span class="text-red-500">*</span>
                                </label>
                                <select id="subject" v-model="formData.subject" required
                                    class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors appearance-none bg-white">
                                    <option value="" disabled>Select a subject</option>
                                    <option value="general">General Inquiry</option>
                                    <option value="order">Order Support</option>
                                    <option value="product">Product Question</option>
                                    <option value="technical">Technical Issue</option>
                                    <option value="partnership">Partnership Opportunity</option>
                                    <option value="feedback">Feedback</option>
                                </select>
                            </div>

                            <!-- Message -->
                            <div>
                                <label for="message" class="block text-sm font-bold text-gray-900 mb-2">
                                    Message <span class="text-red-500">*</span>
                                </label>
                                <textarea id="message" v-model="formData.message" required rows="6"
                                    placeholder="Tell us how we can help you..."
                                    class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors resize-none"></textarea>
                            </div>

                            <!-- Submit Button -->
                            <button type="submit" :disabled="isSubmitting"
                                class="w-full px-8 py-4 bg-linear-to-r from-violet-600 to-purple-600 text-white font-bold rounded-xl hover:shadow-2xl hover:shadow-violet-600/50 transition-all duration-300 flex items-center justify-center gap-2 group disabled:opacity-50 disabled:cursor-not-allowed">
                                <span v-if="!isSubmitting">Send Message</span>
                                <span v-else>Sending...</span>
                                <Send class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                            </button>

                            <p class="text-sm text-gray-500 text-center">
                                By submitting this form, you agree to our Privacy Policy
                            </p>
                        </form>
                    </div>

                    <!-- Right Column -->
                    <div class="space-y-8">

                        <!-- Map -->
                        <div class="bg-white rounded-2xl overflow-hidden shadow-lg border border-gray-100">
                            <div class="aspect-video bg-gray-200 relative">
                                <iframe
                                    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d125421.41243287174!2d104.8134!3d11.5564!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3109513dc76a6be3%3A0x9c010ee85ab525bb!2sPhnom%20Penh%2C%20Cambodia!5e0!3m2!1sen!2s!4v1234567890"
                                    width="100%" height="100%" style="border:0;" allowfullscreen="" loading="lazy"
                                    class="absolute inset-0"></iframe>
                            </div>
                            <div class="p-6">
                                <h3 class="text-xl font-bold text-gray-900 mb-2">Visit Our Office</h3>
                                <p class="text-gray-600">
                                    123 Commerce Street, Phnom Penh, Cambodia
                                </p>
                            </div>
                        </div>

                        <!-- Support Options -->
                        <div class="bg-white rounded-2xl p-6 shadow-lg border border-gray-100">
                            <h3 class="text-xl font-bold text-gray-900 mb-6">Other Ways to Reach Us</h3>
                            <div class="space-y-4">
                                <div v-for="option in supportOptions" :key="option.title"
                                    class="flex items-start gap-4 p-4 bg-gray-50 rounded-xl hover:bg-gray-100 transition-colors group cursor-pointer">
                                    <div
                                        :class="`w-12 h-12 flex-shrink- bg-linear-to-br ${option.color} rounded-xl flex items-center justify-center group-hover:scale-110 transition-transform duration-300`">
                                        <component :is="option.icon" class="w-6 h-6 text-white" />
                                    </div>
                                    <div class="flex-1">
                                        <h4 class="font-bold text-gray-900 mb-1">{{ option.title }}</h4>
                                        <p class="text-sm text-gray-600 mb-2">{{ option.description }}</p>
                                        <button
                                            class="text-sm font-semibold text-violet-600 hover:text-violet-700 transition-colors">
                                            {{ option.action }} →
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Quick Stats -->
                        <div
                            class="bg-linear-to-br from-violet-600 via-purple-600 to-indigo-600 rounded-2xl p-8 text-white shadow-2xl">
                            <h3 class="text-2xl font-bold mb-6">Why Contact Us?</h3>
                            <div class="space-y-4">
                                <div class="flex items-center gap-3">
                                    <div
                                        class="w-10 h-10 bg-white/20 backdrop-blur-md rounded-lg flex items-center justify-center">
                                        <CheckCircle class="w-5 h-5" />
                                    </div>
                                    <div>
                                        <div class="font-bold">Fast Response Time</div>
                                        <div class="text-sm text-white/80">Average reply in under 2 hours</div>
                                    </div>
                                </div>
                                <div class="flex items-center gap-3">
                                    <div
                                        class="w-10 h-10 bg-white/20 backdrop-blur-md rounded-lg flex items-center justify-center">
                                        <CheckCircle class="w-5 h-5" />
                                    </div>
                                    <div>
                                        <div class="font-bold">Expert Support</div>
                                        <div class="text-sm text-white/80">Knowledgeable team ready to help</div>
                                    </div>
                                </div>
                                <div class="flex items-center gap-3">
                                    <div
                                        class="w-10 h-10 bg-white/20 backdrop-blur-md rounded-lg flex items-center justify-center">
                                        <CheckCircle class="w-5 h-5" />
                                    </div>
                                    <div>
                                        <div class="font-bold">24/7 Availability</div>
                                        <div class="text-sm text-white/80">We're here whenever you need us</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </section>

        <!-- FAQ Preview -->
        <section class="py-16 bg-gray-50">
            <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
                <h2 class="text-3xl md:text-4xl font-black text-gray-900 mb-4">
                    Need Quick <span
                        class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Answers?</span>
                </h2>
                <p class="text-gray-600 mb-8 text-lg">
                    Check out our FAQ section for instant answers to common questions
                </p>
                <a href="/faq"
                    class="inline-flex items-center gap-2 px-8 py-4 bg-linear-to-r from-violet-600 to-purple-600 text-white font-bold rounded-xl hover:shadow-2xl hover:shadow-violet-600/50 hover:scale-105 transition-all duration-300">
                    Visit Help Center
                    <FileText class="w-5 h-5" />
                </a>
            </div>
        </section>

    </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}
</style>