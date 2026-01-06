<script setup>
import { ref } from 'vue';
import { RouterLink } from 'vue-router';
import {
    TrendingUp,
    Award,
    ShoppingBag,
    Star,
    Zap,
    Shield,
    Truck,
    Clock,
    Mail,
    Phone,
    MapPin,
    ArrowRight,
    Package,
    Users,
    Sparkles,
    Heart,
    Search,
    ChevronLeft,
    ChevronRight
} from 'lucide-vue-next';
import HeroCarousel from '../components/HeroCarousel.vue';
import ProductCarousel from '../components/ProductCarousel.vue';


const stats = ref([
    { icon: Users, value: '50K+', label: 'Happy Customers' },
    { icon: ShoppingBag, value: '100K+', label: 'Products Sold' },
    { icon: Award, value: '4.9', label: 'Average Rating' },
    { icon: Sparkles, value: '99%', label: 'Satisfaction Rate' },
]);

const categories = ref([
    {
        name: 'Electronics',
        image: 'https://images.unsplash.com/photo-1498049794561-7780e7231661?w=400&h=300&fit=crop',
        count: '1,234 Products',
        color: 'from-blue-500 to-indigo-600'
    },
    {
        name: 'Fashion',
        image: 'https://images.unsplash.com/photo-1445205170230-053b83016050?w=400&h=300&fit=crop',
        count: '2,156 Products',
        color: 'from-pink-500 to-rose-600'
    },
    {
        name: 'Sports',
        image: 'https://images.unsplash.com/photo-1461896836934-ffe607ba8211?w=400&h=300&fit=crop',
        count: '987 Products',
        color: 'from-orange-500 to-red-600'
    },
    {
        name: 'Home & Living',
        image: 'https://images.unsplash.com/photo-1484101403633-562f891dc89a?w=400&h=300&fit=crop',
        count: '1,543 Products',
        color: 'from-emerald-500 to-teal-600'
    },
]);

const newsletterEmail = ref('');
const handleNewsletterSubmit = () => {
    console.log('Newsletter subscription:', newsletterEmail.value);
    newsletterEmail.value = '';
};

const contactForm = ref({
    name: '',
    email: '',
    message: ''
});

const handleContactSubmit = () => {
    console.log('Contact form submitted:', contactForm.value);
    contactForm.value = { name: '', email: '', message: '' };
};

</script>

<template>
    <div class="min-h-screen bg-white">
        <!-- Hero Carousel -->
        <HeroCarousel class="mt-4"/>
        <!-- Shop by Category -->
        <section class="py-24">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="flex flex-col md:flex-row md:items-end justify-between mb-12 gap-6">
                    <div class="space-y-4">
                        <div class="inline-flex items-center gap-2 px-3 py-1 bg-gray-100 rounded-full text-gray-600 text-xs font-bold tracking-wider uppercase">
                            <Search class="w-3 h-3" />
                            Categories
                        </div>
                        <h2 class="text-4xl font-light text-gray-900 tracking-tight">
                            Shop by <span class="font-bold">Category</span>
                        </h2>
                        <p class="text-gray-500 max-w-md text-lg">
                            Curated collections for your lifestyle and needs.
                        </p>
                    </div>
                    <RouterLink to="/product" class="group flex items-center gap-2 text-gray-900 font-bold hover:text-violet-600 transition-colors">
                        View All Categories
                        <ArrowRight class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                    </RouterLink>
                </div>
                <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
                    <RouterLink v-for="category in categories" :key="category.name" to="/product"
                        class="group relative h-96 overflow-hidden rounded-3xl bg-gray-100 shadow-sm hover:shadow-2xl transition-all duration-700">
                        <img :src="category.image" :alt="category.name"
                            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-1000 grayscale-[0.2] group-hover:grayscale-0" />
                        <div class="absolute inset-0 bg-linear-to-t from-black/80 via-black/20 to-transparent opacity-60 group-hover:opacity-80 transition-opacity duration-500"></div>
                        <div class="absolute inset-0 flex flex-col justify-end p-8">
                            <div class="transform translate-y-4 group-hover:translate-y-0 transition-transform duration-500">
                                <p class="text-white/70 text-sm font-medium mb-1">{{ category.count }}</p>
                                <h3 class="text-2xl font-bold text-white mb-4">{{ category.name }}</h3>
                                <div class="h-1 w-12 bg-white/30 group-hover:w-full transition-all duration-500"></div>
                            </div>
                        </div>
                    </RouterLink>
                </div>
            </div>
        </section>
        <!-- Featured Products Carousel -->
        <section class="py-24 bg-gray-50/50">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-16 space-y-4">
                    <div class="inline-flex items-center gap-2 px-3 py-1 bg-amber-100/50 rounded-full text-amber-600 text-xs font-bold tracking-wider uppercase">
                        <TrendingUp class="w-3 h-3" />
                        Selected for you
                    </div>
                    <h2 class="text-4xl md:text-5xl font-light text-gray-900 tracking-tight">
                        Featured <span class="font-bold italic">Products</span>
                    </h2>
                    <div class="w-24 h-1 bg-amber-500 mx-auto rounded-full"></div>
                </div>
                <ProductCarousel />
            </div>
        </section>
        <!-- Stats Section -->
        <section class="py-24 bg-white border-y border-gray-100">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid grid-cols-2 md:grid-cols-4 gap-12">
                    <div v-for="stat in stats" :key="stat.label" class="flex flex-col items-center text-center space-y-4">
                        <div class="w-16 h-16 rounded-2xl bg-gray-50 flex items-center justify-center text-gray-400 group-hover:bg-violet-50 group-hover:text-violet-600 transition-colors duration-300">
                            <component :is="stat.icon" class="w-8 h-8" />
                        </div>
                        <div>
                            <div class="text-4xl font-black text-gray-900 mb-1 tracking-tight">{{ stat.value }}</div>
                            <div class="text-gray-500 font-medium uppercase text-xs tracking-widest">{{ stat.label }}</div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- About Section -->
        <section class="py-32 bg-white overflow-hidden">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid lg:grid-cols-2 gap-24 items-center">
                    <div class="relative">
                        <div class="absolute -top-12 -left-12 w-64 h-64 bg-violet-100 rounded-full blur-3xl opacity-50"></div>
                        <div class="absolute -bottom-12 -right-12 w-64 h-64 bg-amber-100 rounded-full blur-3xl opacity-50"></div>
                        
                        <div class="relative grid grid-cols-2 gap-4">
                            <img src="https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=400&h=600&fit=crop"
                                alt="About AlieeShop" class="rounded-3xl shadow-2xl w-full h-80 object-cover mt-12" />
                            <img src="https://images.unsplash.com/photo-1441986300917-64674bd600d8?w=400&h=600&fit=crop"
                                alt="Storefront" class="rounded-3xl shadow-2xl w-full h-80 object-cover" />
                        </div>
                        <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-white p-8 rounded-3xl shadow-2xl border border-gray-100 text-center">
                            <div class="text-4xl font-black text-gray-900">4.9/5</div>
                            <div class="flex justify-center gap-1 my-2">
                                <Star v-for="i in 5" :key="i" class="w-4 h-4 text-amber-400 fill-amber-400" />
                            </div>
                            <div class="text-sm text-gray-500 font-medium">Customer Satisfaction</div>
                        </div>
                    </div>
                    <div class="space-y-8">
                        <div class="inline-flex items-center gap-2 px-3 py-1 bg-violet-50 rounded-full text-violet-600 text-xs font-bold tracking-wider uppercase">
                            <Sparkles class="w-3 h-3" />
                            Since 2010
                        </div>
                        <h2 class="text-5xl font-light text-gray-900 leading-tight tracking-tight">
                            Elevating your <br />
                            <span class="font-bold italic">Shopping Experience</span>
                        </h2>
                        <p class="text-xl text-gray-500 leading-relaxed font-light">
                            At AlieeShop, we believe quality shouldn't be a luxury. We've spent over a decade curating the world's finest products, delivered with care to your doorstep.
                        </p>
                        <div class="grid sm:grid-cols-2 gap-6">
                            <div class="space-y-2">
                                <div class="w-10 h-10 bg-green-50 text-green-600 rounded-lg flex items-center justify-center">
                                    <Shield class="w-6 h-6" />
                                </div>
                                <h4 class="font-bold text-gray-900">Guaranteed Quality</h4>
                                <p class="text-gray-500 text-sm">Every product is hand-checked by our experts.</p>
                            </div>
                            <div class="space-y-2">
                                <div class="w-10 h-10 bg-blue-50 text-blue-600 rounded-lg flex items-center justify-center">
                                    <Truck class="w-6 h-6" />
                                </div>
                                <h4 class="font-bold text-gray-900">Worldwide Shipping</h4>
                                <p class="text-gray-500 text-sm">Fast, reliable delivery to over 150 countries.</p>
                            </div>
                        </div>
                        <div class="pt-4">
                            <RouterLink to="/about"
                                class="inline-flex items-center gap-4 text-gray-900 font-bold group">
                                <span class="relative">
                                    Our Story
                                    <span class="absolute -bottom-1 left-0 w-full h-0.5 bg-gray-900 scale-x-0 group-hover:scale-x-100 transition-transform origin-left"></span>
                                </span>
                                <div class="w-12 h-12 rounded-full border border-gray-200 flex items-center justify-center group-hover:bg-gray-900 group-hover:text-white transition-all duration-300">
                                    <ArrowRight class="w-5 h-5" />
                                </div>
                            </RouterLink>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="py-24 bg-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="relative bg-gray-900 rounded-[3rem] overflow-hidden p-12 md:p-24 text-center">
                    <div class="absolute top-0 left-0 w-full h-full">
                        <div class="absolute top-0 right-0 w-1/2 h-full bg-linear-to-bl from-violet-600/20 to-transparent"></div>
                        <div class="absolute bottom-0 left-0 w-1/2 h-full bg-linear-to-tr from-amber-600/10 to-transparent"></div>
                    </div>
                    <div class="relative z-10 max-w-2xl mx-auto space-y-8">
                        <h2 class="text-4xl md:text-6xl font-light text-white tracking-tight">
                            Stay in the <span class="font-bold italic text-violet-400">Loop</span>
                        </h2>
                        <p class="text-gray-400 text-lg md:text-xl font-light">
                            Subscribe to receive updates on new collections and exclusive offers.
                        </p>
                        <form @submit.prevent="handleNewsletterSubmit" class="relative max-w-md mx-auto">
                            <input v-model="newsletterEmail" type="email" placeholder="Your email address" required
                                class="w-full bg-white/5 border border-white/10 rounded-full px-8 py-5 text-white focus:outline-none focus:ring-2 focus:ring-gray-100 transition-all placeholder:text-gray-600" />
                            <button type="submit"
                                class="absolute right-2 top-2 bottom-2 px-8 bg-white text-gray-900 font-bold rounded-full hover:bg-violet-400 hover:text-white transition-all duration-300">
                                Join
                            </button>
                        </form>
                        <p class="text-xs text-gray-500 uppercase tracking-widest font-medium">
                            No spam, just quality updates.
                        </p>
                    </div>
                </div>
            </div>
        </section>
        <!-- Contact Section -->
        <section class="py-32 bg-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid lg:grid-cols-2 gap-24">
                    <div class="space-y-12">
                        <div class="space-y-4">
                            <h2 class="text-5xl font-light text-gray-900 tracking-tight">
                                Let's <span class="font-bold">Connect</span>
                            </h2>
                            <p class="text-gray-500 text-xl font-light">
                                Have a question or need assistance? Our team is here to help you.
                            </p>
                        </div>
                        <div class="space-y-8">
                            <div class="flex items-center gap-6 group">
                                <div class="w-14 h-14 rounded-2xl bg-gray-50 flex items-center justify-center text-gray-400 group-hover:bg-violet-50 group-hover:text-red-600 transition-all duration-300">
                                    <Phone class="w-7 h-7" />
                                </div>
                                <div>
                                    <div class="text-sm text-gray-400 uppercase tracking-widest font-bold mb-1">Call Us</div>
                                    <div class="text-xl font-bold text-gray-900">+855 (000) 000-000</div>
                                </div>
                            </div>
                            <div class="flex items-center gap-6 group">
                                <div class="w-14 h-14 rounded-2xl bg-gray-50 flex items-center justify-center text-gray-400 group-hover:bg-blue-50 group-hover:text-blue-600 transition-all duration-300">
                                    <Mail class="w-7 h-7" />
                                </div>
                                <div>
                                    <div class="text-sm text-gray-400 uppercase tracking-widest font-bold mb-1">Email Us</div>
                                    <div class="text-xl font-bold text-gray-900">alieeshop@example.com</div>
                                </div>
                            </div>
                            <div class="flex items-center gap-6 group">
                                <div class="w-14 h-14 rounded-2xl bg-gray-50 flex items-center justify-center text-gray-400 group-hover:bg-emerald-50 group-hover:text-emerald-600 transition-all duration-300">
                                    <MapPin class="w-7 h-7" />
                                </div>
                                <div>
                                    <div class="text-sm text-gray-400 uppercase tracking-widest font-bold mb-1">Visit Us</div>
                                    <div class="text-xl font-bold text-gray-900">123 St, Phnom Penh, Cambodia</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="bg-gray-50/50 rounded-[3rem] p-12 md:p-16">
                        <form @submit.prevent="handleContactSubmit" class="space-y-6">
                            <div class="grid md:grid-cols-2 gap-6">
                                <div class="space-y-2">
                                    <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">Name</label>
                                    <input v-model="contactForm.name" type="text" placeholder="John Doe" required
                                        class="w-full bg-white border-0 rounded-2xl px-6 py-4 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm" />
                                </div>
                                <div class="space-y-2">
                                    <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">Email</label>
                                    <input v-model="contactForm.email" type="email" placeholder="john@example.com" required
                                        class="w-full bg-white border-0 rounded-2xl px-6 py-4 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm" />
                                </div>
                            </div>
                            <div class="space-y-2">
                                <label class="text-xs font-bold uppercase tracking-widest text-gray-400 ml-4">Message</label>
                                <textarea v-model="contactForm.message" placeholder="How can we help?" rows="5" required
                                    class="w-full bg-white border-0 rounded-2xl px-6 py-4 focus:ring-2 focus:ring-violet-500 transition-all shadow-sm resize-none"></textarea>
                            </div>
                            <button type="submit"
                                class="w-full bg-gray-900 text-white font-bold py-5 rounded-2xl hover:bg-black transition-all duration-300 flex items-center justify-center gap-3 group shadow-xl">
                                Send Message
                                <ArrowRight class="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </section>
    </div>
</template>