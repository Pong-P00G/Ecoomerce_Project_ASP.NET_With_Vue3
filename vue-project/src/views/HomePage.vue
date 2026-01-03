<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
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

const testimonials = ref([
    {
        name: 'Sarah Johnson',
        role: 'Fashion Enthusiast',
        comment: 'Amazing quality and fast shipping! Will definitely shop again.',
        rating: 5,
        avatar: '👩',
        image: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=100&h=100&fit=crop'
    },
    {
        name: 'Michael Chen',
        role: 'Tech Lover',
        comment: 'Best online shopping experience. Great customer service!',
        rating: 5,
        avatar: '👨',
        image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100&h=100&fit=crop'
    },
    {
        name: 'Emma Davis',
        role: 'Home Decorator',
        comment: 'Found exactly what I was looking for. Highly recommended!',
        rating: 5,
        avatar: '👩‍🦰',
        image: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=100&h=100&fit=crop'
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
    <div class="min-h-screen bg-linear-to-b from-gray-50 to-white">
        <!-- Hero Carousel -->
        <HeroCarousel />
        <!-- Shop by Category -->
        <section class="py-20 bg-linear-to-br from-gray-50 to-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-12 space-y-4">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                        <Search class="w-4 h-4" />
                        Shop by Category
                    </div>
                    <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                        Explore Our <span
                            class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Collections</span>
                    </h2>
                    <p class="text-gray-600 max-w-2xl mx-auto text-lg">
                        Discover thousands of products across all categories
                    </p>
                </div>
                <div class="grid grid-cols-2 lg:grid-cols-4 gap-6">
                    <RouterLink v-for="category in categories" :key="category.name" to="/product"
                        class="group relative overflow-hidden rounded-2xl shadow-lg hover:shadow-2xl transition-all duration-500 aspect-4/5">
                        <img :src="category.image" :alt="category.name"
                            class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                        <div
                            :class="`absolute inset-0 bg-linear-to-t ${category.color} opacity-60 group-hover:opacity-70 transition-opacity`">
                        </div>
                        <div class="absolute inset-0 flex flex-col items-center justify-center text-white p-6">
                            <h3
                                class="text-2xl md:text-3xl font-black mb-2 transform group-hover:scale-110 transition-transform">
                                {{ category.name }}</h3>
                            <p class="text-sm md:text-base font-medium opacity-90">{{ category.count }}</p>
                            <div
                                class="mt-4 px-6 py-2 bg-white/20 backdrop-blur-md rounded-full text-sm font-semibold opacity-0 group-hover:opacity-100 transition-opacity">
                                Shop Now
                            </div>
                        </div>
                    </RouterLink>
                </div>
            </div>
        </section>
        <!-- Featured Products Carousel -->
        <section class="py-20 bg-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-12 space-y-4">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-amber-100 rounded-full text-amber-600 text-sm font-semibold">
                        <TrendingUp class="w-4 h-4" />
                        Trending Now
                    </div>
                    <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                        Featured <span
                            class="bg-linear-to-r from-amber-600 to-orange-600 bg-clip-text text-transparent">Products</span>
                    </h2>
                    <p class="text-gray-600 max-w-2xl mx-auto text-lg">
                        Handpicked items just for you
                    </p>
                </div>
                <!-- Product Carousel -->
                <ProductCarousel />
            </div>
        </section>
        <!-- Stats Section -->
        <section class="py-20 bg-linear-to-br from-violet-600 via-purple-600 to-indigo-600 relative overflow-hidden">
            <div class="absolute inset-0 opacity-10">
                <div class="absolute top-0 left-1/4 w-96 h-96 bg-white rounded-full blur-3xl"></div>
                <div class="absolute bottom-0 right-1/4 w-96 h-96 bg-white rounded-full blur-3xl"></div>
            </div>
            <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-12">
                    <h2 class="text-4xl md:text-5xl font-black text-white mb-4">
                        Trusted by Thousands
                    </h2>
                    <p class="text-white/80 text-lg">
                        Join our growing community of satisfied customers
                    </p>
                </div>
                <div class="grid grid-cols-2 md:grid-cols-4 gap-8">
                    <div v-for="stat in stats" :key="stat.label" class="text-center space-y-3 group">
                        <div
                            class="inline-flex items-center justify-center w-20 h-20 bg-white/20 backdrop-blur-md rounded-2xl group-hover:scale-110 group-hover:bg-white/30 transition-all duration-300">
                            <component :is="stat.icon" class="w-10 h-10 text-white" />
                        </div>
                        <div class="text-5xl md:text-6xl font-black text-white">{{ stat.value }}</div>
                        <div class="text-white/90 font-medium text-lg">{{ stat.label }}</div>
                    </div>
                </div>
            </div>
        </section>
        <!-- About Section -->
        <section class="py-20 bg-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid lg:grid-cols-2 gap-16 items-center">
                    <div class="relative order-2 lg:order-1">
                        <div
                            class="absolute -inset-4 bg-linear-to-br from-violet-600 to-purple-600 rounded-3xl transform rotate-3 opacity-20">
                        </div>
                        <img src="https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=800&h=600&fit=crop"
                            alt="About AlieeShop" class="relative rounded-3xl shadow-2xl w-full h-500px object-cover" />
                        <div class="absolute -bottom-8 -right-8 bg-white rounded-2xl shadow-2xl p-6 max-w-xs">
                            <div class="flex items-center gap-4">
                                <div
                                    class="w-16 h-16 bg-linear-to-br from-green-500 to-emerald-600 rounded-2xl flex items-center justify-center">
                                    <Award class="w-8 h-8 text-white" />
                                </div>
                                <div>
                                    <div class="text-3xl font-black text-gray-900">4.9/5</div>
                                    <div class="text-sm text-gray-600">Customer Rating</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="space-y-6 order-1 lg:order-2">
                        <div
                            class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                            <Award class="w-4 h-4" />
                            About Us
                        </div>
                        <h2 class="text-4xl md:text-5xl font-black text-gray-900 leading-tight">
                            Your Trusted <br />
                            <span
                                class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Shopping
                                Partner</span>
                        </h2>
                        <p class="text-lg text-gray-600 leading-relaxed">
                            At AlieeShop, we're passionate about bringing you the best products at unbeatable
                            prices.
                            With over 50,000 satisfied customers and counting, we've built a reputation for
                            quality,
                            reliability, and exceptional customer service.
                        </p>
                        <div class="space-y-4">
                            <div class="flex items-start gap-4 p-4 bg-green-50 rounded-xl">
                                <div
                                    class="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center shrink-0 mt-1">
                                    <span class="text-white font-bold text-lg">✓</span>
                                </div>
                                <div>
                                    <h4 class="font-bold text-gray-900 mb-1">Premium Quality</h4>
                                    <p class="text-gray-600 text-sm">Curated selection of authentic products</p>
                                </div>
                            </div>
                            <div class="flex items-start gap-4 p-4 bg-blue-50 rounded-xl">
                                <div
                                    class="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center shrink-0 mt-1">
                                    <span class="text-white font-bold text-lg">✓</span>
                                </div>
                                <div>
                                    <h4 class="font-bold text-gray-900 mb-1">Fast Delivery</h4>
                                    <p class="text-gray-600 text-sm">Quick shipping with real-time tracking</p>
                                </div>
                            </div>
                            <div class="flex items-start gap-4 p-4 bg-purple-50 rounded-xl">
                                <div
                                    class="w-8 h-8 bg-purple-500 rounded-full flex items-center justify-center shrink-0 mt-1">
                                    <span class="text-white font-bold text-lg">✓</span>
                                </div>
                                <div>
                                    <h4 class="font-bold text-gray-900 mb-1">Easy Returns</h4>
                                    <p class="text-gray-600 text-sm">30-day hassle-free return policy</p>
                                </div>
                            </div>
                        </div>
                        <RouterLink to="/about"
                            class="inline-flex items-center gap-2 px-6 py-3 bg-gray-900 text-white font-semibold rounded-xl hover:bg-gray-800 transition-colors">
                            Learn More About Us
                            <ArrowRight class="w-5 h-5" />
                        </RouterLink>
                    </div>
                </div>
            </div>
        </section>
        <!-- Testimonials Section -->
        <section class="py-20 bg-linear-to-b from-gray-50 to-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-12 space-y-4">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-amber-100 rounded-full text-amber-600 text-sm font-semibold">
                        <Star class="w-4 h-4 fill-amber-600" />
                        Testimonials
                    </div>
                    <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                        What Our <span
                            class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Customers
                            Say</span>
                    </h2>
                    <p class="text-gray-600 max-w-2xl mx-auto text-lg">
                        Don't just take our word for it
                    </p>
                </div>
                <div class="grid md:grid-cols-3 gap-8">
                    <div v-for="testimonial in testimonials" :key="testimonial.name"
                        class="bg-white p-8 rounded-2xl shadow-sm hover:shadow-xl transition-all duration-300 space-y-6 border border-gray-100">
                        <div class="flex items-center gap-1">
                            <Star v-for="i in testimonial.rating" :key="i"
                                class="w-5 h-5 text-yellow-400 fill-yellow-400" />
                        </div>
                        <p class="text-gray-700 leading-relaxed text-lg">
                            "{{ testimonial.comment }}"
                        </p>
                        <div class="flex items-center gap-4 pt-4 border-t border-gray-100">
                            <img :src="testimonial.image" :alt="testimonial.name"
                                class="w-14 h-14 rounded-full object-cover" />
                            <div>
                                <div class="font-bold text-gray-900 text-lg">{{ testimonial.name }}</div>
                                <div class="text-sm text-gray-600">{{ testimonial.role }}</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <!-- Newsletter Section -->
        <section class="py-20 bg-linear-to-br from-violet-600 via-purple-600 to-indigo-600 relative overflow-hidden">
            <div class="absolute inset-0 opacity-10">
                <div class="absolute top-1/4 left-1/4 w-96 h-96 bg-white rounded-full blur-3xl"></div>
                <div class="absolute bottom-1/4 right-1/4 w-96 h-96 bg-white rounded-full blur-3xl"></div>
            </div>
            <div class="relative max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center space-y-8">
                <div class="inline-flex items-center justify-center w-20 h-20 bg-white/20 backdrop-blur-md rounded-3xl">
                    <Mail class="w-10 h-10 text-white" />
                </div>
                <h2 class="text-4xl md:text-5xl font-black text-white">
                    Subscribe to Our Newsletter
                </h2>
                <p class="text-white/90 text-lg max-w-2xl mx-auto">
                    Get exclusive deals, new arrivals, and insider tips delivered straight to your inbox.
                </p>
                <form @submit.prevent="handleNewsletterSubmit" class="flex flex-col sm:flex-row gap-4 max-w-xl mx-auto">
                    <input v-model="newsletterEmail" type="email" placeholder="Enter your email address" required
                        class="flex-1 px-6 py-4 rounded-xl focus:outline-none focus:ring-4 focus:ring-white/50 transition-all text-gray-900" />
                    <button type="submit"
                        class="px-8 py-4 bg-white text-violet-600 font-bold rounded-xl hover:bg-gray-100 hover:scale-105 transition-all duration-300 shadow-xl">
                        Subscribe
                    </button>
                </form>
                <p class="text-sm text-white/70">
                    By subscribing, you agree to our Privacy Policy and consent to receive updates.
                </p>
            </div>
        </section>
        <!-- Contact CTA Section -->
        <section class="py-20 bg-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid lg:grid-cols-2 gap-12 items-center">
                    <div class="space-y-8">
                        <div
                            class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                            <Mail class="w-4 h-4" />
                            Get In Touch
                        </div>
                        <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                            Have Questions?<br />
                            <span
                                class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">We're
                                Here to Help</span>
                        </h2>
                        <p class="text-gray-600 text-lg leading-relaxed">
                            Our dedicated support team is available 24/7 to assist you with any questions or
                            concerns.
                        </p>
                        <div class="space-y-4">
                            <a href="tel:+1234567890"
                                class="flex items-center gap-4 text-gray-900 hover:text-violet-600 transition-colors group p-4 rounded-xl hover:bg-violet-50">
                                <div
                                    class="w-14 h-14 bg-linear-to-br from-blue-500 to-indigo-600 rounded-xl flex items-center justify-center group-hover:scale-110 transition-transform">
                                    <Phone class="w-7 h-7 text-white" />
                                </div>
                                <div>
                                    <div class="text-sm text-gray-500 font-medium">Phone</div>
                                    <div class="font-bold text-lg">+1 (234) 567-890</div>
                                </div>
                            </a>
                            <a href="mailto:support@alieeshop.com"
                                class="flex items-center gap-4 text-gray-900 hover:text-violet-600 transition-colors group p-4 rounded-xl hover:bg-violet-50">
                                <div
                                    class="w-14 h-14 bg-linear-to-br from-violet-500 to-purple-600 rounded-xl flex items-center justify-center group-hover:scale-110 transition-transform">
                                    <Mail class="w-7 h-7 text-white" />
                                </div>
                                <div>
                                    <div class="text-sm text-gray-500 font-medium">Email</div>
                                    <div class="font-bold text-lg">support@alieeshop.com</div>
                                </div>
                            </a>
                            <div class="flex items-center gap-4 text-gray-900 p-4">
                                <div
                                    class="w-14 h-14 bg-linear-to-br from-emerald-500 to-teal-600 rounded-xl flex items-center justify-center">
                                    <MapPin class="w-7 h-7 text-white" />
                                </div>
                                <div>
                                    <div class="text-sm text-gray-500 font-medium">Address</div>
                                    <div class="font-bold text-lg">123 Commerce St, NY 10001</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="bg-linear-to-br from-gray-50 to-white rounded-3xl p-8 shadow-xl border border-gray-100">
                        <h3 class="text-2xl font-bold text-gray-900 mb-6">Send us a message</h3>
                        <form @submit.prevent="handleContactSubmit" class="space-y-4">
                            <div>
                                <input v-model="contactForm.name" type="text" placeholder="Your Name" required
                                    class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors text-gray-900" />
                            </div>
                            <div>
                                <input v-model="contactForm.email" type="email" placeholder="Your Email" required
                                    class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors text-gray-900" />
                            </div>
                            <div>
                                <textarea v-model="contactForm.message" placeholder="Your Message" rows="4" required
                                    class="w-full px-4 py-3 border-2 border-gray-200 rounded-xl focus:border-violet-600 focus:outline-none transition-colors resize-none text-gray-900"></textarea>
                            </div>
                            <button type="submit"
                                class="w-full px-6 py-4 bg-linear-to-r from-violet-600 to-purple-600 text-white font-bold rounded-xl hover:shadow-2xl hover:shadow-violet-600/50 transition-all duration-300 flex items-center justify-center gap-2 group">
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