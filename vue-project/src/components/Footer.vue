<script setup>
import { ref } from 'vue';
import { RouterLink } from 'vue-router';
import {
    Mail,
    Phone,
    MapPin,
    Facebook,
    Instagram,
    Twitter,
    ArrowUp
} from 'lucide-vue-next';

const currentYear = new Date().getFullYear();

const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

const footerLinks = {
    shop: [
        { name: 'New Arrivals', to: '/product?filter=new' },
        { name: 'Best Sellers', to: '/product?filter=bestsellers' },
        { name: 'Sale', to: '/product?filter=sale' },
        { name: 'Gift Cards', to: '/gift-cards' },
    ],
    help: [
        { name: 'Track Order', to: '/track-order' },
        { name: 'Returns & Exchanges', to: '/returns' },
        { name: 'Shipping Info', to: '/shipping' },
        { name: 'FAQ', to: '/faq' },
    ],
    company: [
        { name: 'About Us', to: '/about' },
        { name: 'Contact', to: '/contact' },
        { name: 'Careers', to: '/careers' },
        { name: 'Press', to: '/press' },
    ],
};

const socialLinks = [
    { icon: Facebook, href: 'https://facebook.com', label: 'Facebook', color: 'hover:bg-blue-600' },
    { icon: Instagram, href: 'https://instagram.com', label: 'Instagram', color: 'hover:bg-gradient-to-br hover:from-purple-600 hover:to-pink-600' },
    { icon: Twitter, href: 'https://twitter.com', label: 'Twitter', color: 'hover:bg-sky-500' },
];

</script>

<template>
    <footer class="relative bg-linear-to-br from-slate-900 via-slate-800 to-slate-900 text-gray-300 overflow-hidden">
        <!-- Decorative Background Elements -->
        <div class="absolute inset-0 opacity-10">
            <div class="absolute top-0 left-1/4 w-96 h-96 bg-violet-600 rounded-full blur-3xl"></div>
            <div class="absolute bottom-0 right-1/4 w-96 h-96 bg-purple-600 rounded-full blur-3xl"></div>
        </div>
        <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <!-- Main Footer Content -->
            <div class="pt-16 pb-12">
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-12 gap-12 lg:gap-8">
                    
                    <!-- Brand Section -->
                    <div class="lg:col-span-4 space-y-6">
                        <RouterLink to="/" class="inline-flex items-center gap-2 group">
                            <span class="text-3xl font-bold text-white">
                                Aliee<span class="bg-linear-to-r from-violet-400 to-purple-400 bg-clip-text text-transparent">Shop</span>
                            </span>
                        </RouterLink>
                        <p class="text-gray-400 leading-relaxed text-sm">
                            Discover unique products that elevate your lifestyle. Quality, style, and innovation in every purchase.
                        </p>
                        <!-- Contact Info -->
                        <div class="space-y-3">
                            <a href="tel:+1234567890" class="flex items-center gap-3 text-sm hover:text-violet-400 transition-colors group">
                                <div class="w-9 h-9 rounded-lg bg-slate-800 flex items-center justify-center group-hover:bg-violet-600 transition-colors">
                                    <Phone class="w-4 h-4" />
                                </div>
                                <span>+1 (234) 567-890</span>
                            </a>
                            
                            <a href="mailto:support@alieeshop.com" class="flex items-center gap-3 text-sm hover:text-violet-400 transition-colors group">
                                <div class="w-9 h-9 rounded-lg bg-slate-800 flex items-center justify-center group-hover:bg-violet-600 transition-colors">
                                    <Mail class="w-4 h-4" />
                                </div>
                                <span>support@alieeshop.com</span>
                            </a>
                            
                            <div class="flex items-center gap-3 text-sm">
                                <div class="w-9 h-9 rounded-lg bg-slate-800 flex items-center justify-center">
                                    <MapPin class="w-4 h-4" />
                                </div>
                                <span>123 Commerce St, Phnom Penh</span>
                            </div>
                        </div>
                    </div>
                    <!-- Quick Links Sections -->
                    <div class="lg:col-span-2">
                        <h3 class="text-white font-bold text-sm uppercase tracking-wider mb-6 flex items-center gap-2">
                            <span class="w-8 h-0.5 bg-linear-to-r from-violet-600 to-purple-600"></span>
                            Shop
                        </h3>
                        <ul class="space-y-3">
                            <li v-for="link in footerLinks.shop" :key="link.name">
                                <RouterLink :to="link.to"
                                    class="text-sm hover:text-violet-400 hover:translate-x-1 transition-all duration-300 inline-block">
                                    {{ link.name }}
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                    <div class="lg:col-span-2">
                        <h3 class="text-white font-bold text-sm uppercase tracking-wider mb-6 flex items-center gap-2">
                            <span class="w-8 h-0.5 bg-linear-to-r from-violet-600 to-purple-600"></span>
                            Help
                        </h3>
                        <ul class="space-y-3">
                            <li v-for="link in footerLinks.help" :key="link.name">
                                <RouterLink :to="link.to"
                                    class="text-sm hover:text-violet-400 hover:translate-x-1 transition-all duration-300 inline-block">
                                    {{ link.name }}
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                    <div class="lg:col-span-2">
                        <h3 class="text-white font-bold text-sm uppercase tracking-wider mb-6 flex items-center gap-2">
                            <span class="w-8 h-0.5 bg-linear-to-r from-violet-600 to-purple-600"></span>
                            Company
                        </h3>
                        <ul class="space-y-3">
                            <li v-for="link in footerLinks.company" :key="link.name">
                                <RouterLink :to="link.to"
                                    class="text-sm hover:text-violet-400 hover:translate-x-1 transition-all duration-300 inline-block">
                                    {{ link.name }}
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                    <div class="lg:col-span-2">
                        <h3 class="text-white font-bold text-sm uppercase tracking-wider mb-6 flex items-center justify-center gap-2">
                            <span class="w-8 h-0.5 bg-linear-to-r from-violet-600 to-purple-600"></span>
                            Social Media
                        </h3>
                        <!-- Social Media Links -->
                        <div class="mt-12 flex justify-center gap-3">
                            <a v-for="social in socialLinks" :key="social.label" :href="social.href" target="_blank"
                                rel="noopener noreferrer" :aria-label="social.label"
                                class="w-12 h-12 rounded-xl bg-slate-800 flex items-center justify-center text-white transition-all duration-300 hover:scale-110 hover:text-white"
                                :class="social.color">
                                <component :is="social.icon" class="w-5 h-5" />
                            </a>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Bottom Bar -->
            <div class="py-8 border-t border-slate-700/50">
                <div class="flex flex-col lg:flex-row items-center justify-between gap-6">
                    
                    <!-- Copyright -->
                    <p class="text-sm text-gray-400 text-center lg:text-left">
                        © {{ currentYear }} AlieeShop. All rights reserved.
                    </p>
                </div>
            </div>
        </div>
        <!-- Animated linear Line -->
        <div class="absolute bottom-0 left-0 right-0 h-1 bg-linear-to-r from-violet-600 via-purple-600 to-violet-600 animate-gradient"></div>
    </footer>
</template>

<style scoped>
@keyframes gradient {
    0%, 100% {
        background-position: 0% 50%;
    }
    50% {
        background-position: 100% 50%;
    }
}

.animate-gradient {
    background-size: 200% 200%;
    animation: gradient 3s ease infinite;
}

/* Smooth scroll behavior */
html {
    scroll-behavior: smooth;
}
</style>