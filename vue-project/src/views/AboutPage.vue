<script setup>
import { ref } from 'vue';
import {
    Award,
    Target,
    Heart,
    Users,
    TrendingUp,
    Shield,
    Zap,
    Globe,
    Package,
    Star,
    Check
} from 'lucide-vue-next';

// Company Stats
const stats = ref([
    { icon: Users, value: '50K+', label: 'Happy Customers', color: 'from-blue-500 to-cyan-500' },
    { icon: Package, value: '100K+', label: 'Products Delivered', color: 'from-emerald-500 to-teal-500' },
    { icon: Globe, value: '25+', label: 'Countries Served', color: 'from-violet-500 to-purple-500' },
    { icon: Star, value: '4.9/5', label: 'Customer Rating', color: 'from-amber-500 to-orange-500' },
]);

// Core Values
const values = ref([
    {
        icon: Award,
        title: 'Quality First',
        description: 'We carefully curate every product to ensure premium quality and lasting value for our customers.',
        color: 'from-amber-500 to-orange-500'
    },
    {
        icon: Heart,
        title: 'Customer Care',
        description: 'Your satisfaction is our priority. We go above and beyond to provide exceptional service.',
        color: 'from-red-500 to-pink-500'
    },
    {
        icon: Shield,
        title: 'Trust & Security',
        description: 'Shop with confidence knowing your data and transactions are protected with enterprise-grade security.',
        color: 'from-blue-500 to-cyan-500'
    },
    {
        icon: Zap,
        title: 'Innovation',
        description: 'We continuously evolve to bring you the latest products and shopping experiences.',
        color: 'from-violet-500 to-purple-500'
    },
]);

// Team Members
const team = ref([
    {
        name: 'Sarah Johnson',
        role: 'Founder & CEO',
        image: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=400&h=400&fit=crop',
        bio: 'Visionary leader with 15+ years in e-commerce'
    },
    {
        name: 'Michael Chen',
        role: 'Chief Technology Officer',
        image: 'https://images.unsplash.com/photo-1556157382-97eda2f9e2bf?w=400&h=400&fit=crop',
        bio: 'Tech innovator passionate about seamless experiences'
    },
    {
        name: 'Emily Rodriguez',
        role: 'Head of Marketing',
        image: 'https://images.unsplash.com/photo-1580489944761-15a19d654956?w=400&h=400&fit=crop',
        bio: 'Creative strategist connecting brands with customers'
    },
    {
        name: 'David Kim',
        role: 'Operations Director',
        image: 'https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=400&h=400&fit=crop',
        bio: 'Logistics expert ensuring fast and reliable delivery'
    },
]);

// Milestones
const milestones = ref([
    { year: '2020', title: 'Company Founded', description: 'Started with a vision to revolutionize online shopping' },
    { year: '2021', title: 'Reached 10K Customers', description: 'Milestone achieved through quality and service' },
    { year: '2022', title: 'International Expansion', description: 'Expanded to 25+ countries worldwide' },
    { year: '2023', title: 'Award Recognition', description: 'Named Best E-commerce Platform of the Year' },
    { year: '2024', title: 'Innovation Launch', description: 'Introduced AI-powered shopping assistant' },
]);
</script>

<template>
    <div class="min-h-screen bg-white">

        <!-- Hero Section -->
        <section
            class="relative bg-linear-to-br from-slate-900 via-slate-800 to-slate-900 text-white py-24 overflow-hidden">
            <!-- Decorative Elements -->
            <div class="absolute inset-0 opacity-10">
                <div class="absolute top-0 right-1/4 w-96 h-96 bg-violet-600 rounded-full blur-3xl"></div>
                <div class="absolute bottom-0 left-1/4 w-96 h-96 bg-purple-600 rounded-full blur-3xl"></div>
            </div>

            <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center space-y-6 max-w-4xl mx-auto">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-white/10 backdrop-blur-md rounded-full text-white text-sm font-semibold mb-4">
                        <Award class="w-4 h-4" />
                        About AlieeShop
                    </div>

                    <h1 class="text-5xl md:text-7xl font-black leading-tight">
                        Transforming the Way
                        <span
                            class="block bg-linear-to-r from-violet-400 to-purple-400 bg-clip-text text-transparent">
                            You Shop Online
                        </span>
                    </h1>

                    <p class="text-xl text-gray-300 leading-relaxed max-w-3xl mx-auto">
                        We're on a mission to create the most delightful shopping experience by combining quality
                        products,
                        exceptional service, and innovative technology.
                    </p>
                </div>
            </div>
        </section>

        <!-- Stats Section -->
        <section class="py-16 bg-linear-to-b from-gray-50 to-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid grid-cols-2 md:grid-cols-4 gap-8">
                    <div v-for="stat in stats" :key="stat.label" class="text-center space-y-4 group">
                        <div
                            :class="`inline-flex items-center justify-center w-16 h-16 bg-linear-to-br ${stat.color} rounded-2xl shadow-lg group-hover:scale-110 transition-transform duration-300`">
                            <component :is="stat.icon" class="w-8 h-8 text-white" />
                        </div>
                        <div class="text-4xl md:text-5xl font-black text-gray-900">{{ stat.value }}</div>
                        <div class="text-gray-600 font-medium">{{ stat.label }}</div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Our Story -->
        <section class="py-20">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="grid lg:grid-cols-2 gap-12 items-center">
                    <!-- Image -->
                    <div class="relative order-2 lg:order-1">
                        <div
                            class="absolute inset-0 bg-linear-to-br from-violet-600 to-purple-600 rounded-3xl transform rotate-3">
                        </div>
                        <img src="https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=800&h=600&fit=crop"
                            alt="Our Story" class="relative rounded-3xl shadow-2xl w-full h-500px object-cover" />
                    </div>

                    <!-- Content -->
                    <div class="space-y-6 order-1 lg:order-2">
                        <div
                            class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                            <Target class="w-4 h-4" />
                            Our Story
                        </div>

                        <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                            Building Trust Through
                            <span class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">
                                Quality & Service
                            </span>
                        </h2>

                        <div class="space-y-4 text-lg text-gray-700 leading-relaxed">
                            <p>
                                Founded in 2020, AlieeShop began with a simple idea: make online shopping better.
                                We noticed that customers wanted more than just products—they wanted an experience
                                built on trust, quality, and genuine care.
                            </p>

                            <p>
                                Today, we've grown into a global platform serving over 50,000 happy customers across
                                25+ countries. Our success comes from never compromising on our core values and always
                                putting customers first.
                            </p>

                            <p>
                                Every product we offer is carefully selected, every customer interaction is meaningful,
                                and every innovation we pursue is designed to make your life easier and more enjoyable.
                            </p>
                        </div>

                        <div class="flex flex-wrap gap-4 pt-4">
                            <div class="flex items-center gap-2 text-gray-700">
                                <div class="w-6 h-6 bg-green-100 rounded-full flex items-center justify-center">
                                    <Check class="w-4 h-4 text-green-600" />
                                </div>
                                <span class="font-semibold">Premium Quality</span>
                            </div>
                            <div class="flex items-center gap-2 text-gray-700">
                                <div class="w-6 h-6 bg-green-100 rounded-full flex items-center justify-center">
                                    <Check class="w-4 h-4 text-green-600" />
                                </div>
                                <span class="font-semibold">Fast Shipping</span>
                            </div>
                            <div class="flex items-center gap-2 text-gray-700">
                                <div class="w-6 h-6 bg-green-100 rounded-full flex items-center justify-center">
                                    <Check class="w-4 h-4 text-green-600" />
                                </div>
                                <span class="font-semibold">24/7 Support</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Core Values -->
        <section class="py-20 bg-linear-to-b from-gray-50 to-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-16 space-y-4">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                        <Heart class="w-4 h-4" />
                        Our Values
                    </div>
                    <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                        What <span
                            class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">We
                            Stand For</span>
                    </h2>
                    <p class="text-gray-600 max-w-2xl mx-auto text-lg">
                        Our core values guide everything we do and shape the experience we create for our customers
                    </p>
                </div>

                <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                    <div v-for="value in values" :key="value.title"
                        class="bg-white p-8 rounded-2xl shadow-sm hover:shadow-xl transition-all duration-300 border border-gray-100 group">
                        <div
                            :class="`inline-flex items-center justify-center w-14 h-14 bg-linear-to-br ${value.color} rounded-2xl shadow-lg group-hover:scale-110 transition-transform duration-300 mb-6`">
                            <component :is="value.icon" class="w-7 h-7 text-white" />
                        </div>
                        <h3 class="text-xl font-bold text-gray-900 mb-3">{{ value.title }}</h3>
                        <p class="text-gray-600 leading-relaxed">{{ value.description }}</p>
                    </div>
                </div>
            </div>
        </section>

        <!-- Timeline -->
        <section class="py-20">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-16 space-y-4">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                        <TrendingUp class="w-4 h-4" />
                        Our Journey
                    </div>
                    <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                        Milestones & <span
                            class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Achievements</span>
                    </h2>
                </div>

                <div class="relative">
                    <!-- Timeline Line -->
                    <div
                        class="hidden md:block absolute left-1/2 transform -translate-x-1/2 w-1 h-full bg-linear-to-b from-violet-600 to-purple-600">
                    </div>

                    <!-- Timeline Items -->
                    <div class="space-y-12">
                        <div v-for="(milestone, index) in milestones" :key="milestone.year" class="relative">
                            <div :class="index % 2 === 0 ? 'md:flex-row' : 'md:flex-row-reverse'"
                                class="flex flex-col md:flex items-center gap-8">
                                <!-- Content -->
                                <div class="flex-1" :class="index % 2 === 0 ? 'md:text-right' : 'md:text-left'">
                                    <div
                                        class="bg-white p-6 rounded-2xl shadow-sm hover:shadow-xl transition-all duration-300 border border-gray-100 inline-block">
                                        <div
                                            class="text-3xl font-black bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent mb-2">
                                            {{ milestone.year }}
                                        </div>
                                        <h3 class="text-xl font-bold text-gray-900 mb-2">{{ milestone.title }}</h3>
                                        <p class="text-gray-600">{{ milestone.description }}</p>
                                    </div>
                                </div>

                                <!-- Center Dot -->
                                <div class="hidden md:block relative z-10">
                                    <div
                                        class="w-6 h-6 bg-linear-to-br from-violet-600 to-purple-600 rounded-full border-4 border-white shadow-lg">
                                    </div>
                                </div>

                                <!-- Spacer -->
                                <div class="flex-1"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Team Section -->
        <section class="py-20 bg-linear-to-b from-gray-50 to-white">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div class="text-center mb-16 space-y-4">
                    <div
                        class="inline-flex items-center gap-2 px-4 py-2 bg-violet-100 rounded-full text-violet-600 text-sm font-semibold">
                        <Users class="w-4 h-4" />
                        Our Team
                    </div>
                    <h2 class="text-4xl md:text-5xl font-black text-gray-900">
                        Meet the <span
                            class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Dream
                            Team</span>
                    </h2>
                    <p class="text-gray-600 max-w-2xl mx-auto text-lg">
                        Passionate individuals working together to create exceptional shopping experiences
                    </p>
                </div>

                <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
                    <div v-for="member in team" :key="member.name" class="group">
                        <div class="relative overflow-hidden rounded-2xl mb-6">
                            <img :src="member.image" :alt="member.name"
                                class="w-full h-80 object-cover group-hover:scale-110 transition-transform duration-500" />
                            <div
                                class="absolute inset-0 bg-linear-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                            </div>
                        </div>
                        <h3 class="text-xl font-bold text-gray-900 mb-1">{{ member.name }}</h3>
                        <p class="text-violet-600 font-semibold mb-2">{{ member.role }}</p>
                        <p class="text-gray-600 text-sm">{{ member.bio }}</p>
                    </div>
                </div>
            </div>
        </section>

        <!-- CTA Section -->
        <section class="py-20">
            <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
                <div class="bg-linear-to-br from-violet-600 via-purple-600 to-indigo-600 rounded-3xl p-12 shadow-2xl">
                    <h2 class="text-4xl md:text-5xl font-black text-white mb-6">
                        Ready to Experience the Difference?
                    </h2>
                    <p class="text-xl text-white/90 mb-8">
                        Join thousands of satisfied customers and discover why AlieeShop is trusted worldwide
                    </p>
                    <div class="flex flex-col sm:flex-row gap-4 justify-center">
                        <a href="/product"
                            class="px-8 py-4 bg-white text-violet-600 font-bold rounded-xl hover:bg-gray-100 transition-all duration-300 hover:scale-105 shadow-lg">
                            Start Shopping
                        </a>
                        <a href="/contact"
                            class="px-8 py-4 bg-white/10 backdrop-blur-md text-white font-bold rounded-xl border-2 border-white/30 hover:bg-white/20 transition-all duration-300">
                            Contact Us
                        </a>
                    </div>
                </div>
            </div>
        </section>

    </div>
</template>

<style scoped>
/* Add any custom styles if needed */
</style>