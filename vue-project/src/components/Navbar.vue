<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue';
import '../assets/Navbar.css';
import { RouterLink, useRoute } from 'vue-router';
import { ShoppingCart, Search, User, Heart, Menu, MenuSquare } from 'lucide-vue-next';
import Icons from '../assets/icons/icons.vue';

const route = useRoute();
const isMenuOpen = ref(false);
const isSearchOpen = ref(false);
const scrolled = ref(false);
const cartCount = ref(''); // Example cart count

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
  document.body.style.overflow = isMenuOpen.value ? 'hidden' : '';
};

const closeMenu = () => {
  isMenuOpen.value = false;
  document.body.style.overflow = '';
};

const toggleSearch = () => {
  isSearchOpen.value = !isSearchOpen.value;
};

const Navlinks = ref([
  { to: '/', label: 'Home',icon: 'Home', ariaLabel: 'Home' },
  { to: '/product', label: 'Product', icon: 'Box', ariaLabel: 'Browse Product' },
  { to: '/contact', label: 'Contact', icon: 'Phone', ariaLabel: 'Contact' },
  { to: '/about', label: 'About', icon: 'Info', ariaLabel: 'About' },
]);

const isActiveRoute = computed(() => (linkTo) =>
  route.path === linkTo || route.path.startsWith(linkTo + '/')
);

const handleScroll = () => {
  scrolled.value = window.scrollY > 20;
};

const handleEscapeKey = (event) => {
  if (event.key === 'Escape') {
    if (isMenuOpen.value) closeMenu();
    if (isSearchOpen.value) isSearchOpen.value = false;
  }
};

onMounted(() => {
  document.addEventListener('keydown', handleEscapeKey);
  window.addEventListener('scroll', handleScroll);
});

onUnmounted(() => {
  document.removeEventListener('keydown', handleEscapeKey);
  window.removeEventListener('scroll', handleScroll);
  document.body.style.overflow = '';
});

watch(route, () => {
  closeMenu();
});

</script>

<template>
  <header class="fixed top-0 left-0 right-0 z-50 transition-all duration-300" :class="scrolled ? 'shadow-lg' : ''"
    role="banner">
    <!-- Glassmorphism Background -->
    <div class="absolute inset-0 transition-all duration-300" :class="scrolled ? 'bg-white/80 backdrop-blur-xl border-b border-gray-200/50' : 'bg-linear-to-b from-white/60 to-white/40 backdrop-blur-md'"></div>
    <div class="relative max-w-7xl mx-auto">
      <div class="flex justify-between items-center px-4 sm:px-6 lg:px-8 py-4">
        <!-- Logo Section -->
        <RouterLink to="/" class="flex items-center gap-2 group" aria-label="AlieeShop Home">
          <span class="text-2xl font-bold bg-linear-to-r from-gray-900 to-gray-700 bg-clip-text text-transparent">
            Aliee<span class="bg-linear-to-r from-violet-600 to-purple-600 bg-clip-text text-transparent">Shop</span>
          </span>
        </RouterLink>
        <!-- Desktop Navigation -->
        <nav class="hidden lg:flex items-center space-x-1" role="navigation" aria-label="Main navigation">
          <RouterLink v-for="link in Navlinks" :key="link.label" :to="link.to"
            class="relative flex items-center gap-2 px-5 py-2.5 rounded-xl text-gray-700 font-medium text-sm transition-all duration-300 group"
            :class="isActiveRoute(link.to) ? 'text-violet-600' : 'hover:text-violet-600'" :aria-label="link.ariaLabel">
            <!-- Active Indicator -->
            <div v-if="isActiveRoute(link.to)"
              class="absolute inset-0 bg-linear-to-r from-violet-100 to-purple-100 rounded-xl"></div>
            <!-- Hover Effect -->
            <div class="absolute inset-0 bg-linear-to-r from-violet-50 to-purple-50 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
            <span class="relative z-10">{{ link.label }}</span>
            <!-- Active Dot -->
            <span v-if="isActiveRoute(link.to)"
              class="absolute -bottom-1 left-1/2 transform -translate-x-1/2 w-1 h-1 bg-violet-600 rounded-full"></span>
          </RouterLink>
        </nav>
        <!-- Desktop Actions -->
        <div class="hidden lg:flex items-center gap-3">
          <!-- Search Button -->
          <button @click="toggleSearch" class="p-2.5 rounded-xl hover:bg-gray-100 transition-all duration-300 group"
            aria-label="Search">
            <Search class="w-5 h-5 text-gray-700 group-hover:text-violet-600 transition-colors" />
          </button>
          <!-- Wishlist Button -->
          <button class="p-2.5 rounded-xl hover:bg-gray-100 transition-all duration-300 group" aria-label="Wishlist">
            <RouterLink to="/wishlist">
              <Heart class="w-5 h-5 text-gray-700 group-hover:text-red-500 transition-colors" />
            </RouterLink>
          </button>
          <!-- Cart Button with Badge -->
          <button class="relative p-2.5 rounded-xl hover:bg-gray-100 transition-all duration-300 group"
            aria-label="Shopping cart">
            <RouterLink to="/cart">
              <ShoppingCart class="w-5 h-5 text-gray-700 group-hover:text-violet-600 transition-colors" />
              <span v-if="cartCount > 0"
                class="absolute -top-1 -right-1 w-5 h-5 bg-linear-to-br from-violet-600 to-purple-600 text-white text-xs font-bold rounded-full flex items-center justify-center shadow-lg">
                {{ cartCount }}
              </span>
            </RouterLink>
          </button>
          <!-- Divider -->
          <div class="w-px h-6 bg-gray-300"></div>
          <!-- Login Button -->
          <RouterLink to="/login"
            class="relative px-6 py-2.5 text-sm font-semibold text-white rounded-xl overflow-hidden group">
            <div
              class="absolute inset-0 bg-linear-to-r from-violet-600 to-purple-600 transition-transform duration-300 group-hover:scale-105">
            </div>
            <div
              class="absolute inset-0 bg-linear-to-r from-violet-700 to-purple-700 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
            </div>
            <span class="relative flex items-center gap-2">
              <User class="w-4 h-4" />
              Login
            </span>
          </RouterLink>
        </div>
        <!-- Mobile Menu Button -->
        <button @click="toggleMenu" class="lg:hidden p-2 rounded-xl hover:bg-gray-100 transition-all duration-300"
          :aria-expanded="isMenuOpen" aria-controls="mobile-menu" aria-label="Toggle menu">
          <Icons name="Menu" class="w-6 h-6 text-gray-800" />
        </button>
      </div>
      <!-- Search Bar (Desktop) -->
      <transition name="slide-fade">
        <div v-if="isSearchOpen" class="hidden lg:block px-4 sm:px-6 lg:px-8 pb-4">
          <div class="relative max-w-2xl mx-auto">
            <input type="text" placeholder="Search products, categories, brands..."
              class="w-full px-5 py-3 pl-12 pr-4 rounded-xl border-2 border-gray-200 focus:border-violet-600 focus:outline-none transition-colors bg-white/80 backdrop-blur-sm"
              autofocus />
            <Search class="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          </div>
        </div>
      </transition>
    </div>
    <!-- Mobile Menu -->
    <transition name="mobile-menu">
      <div v-if="isMenuOpen" id="mobile-menu" class="lg:hidden fixed inset-0 z-50" aria-label="Mobile navigation"
        role="region">
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" @click="closeMenu" aria-hidden="true"></div>
        <!-- Sidebar -->
        <div class="absolute top-0 right-0 w-80 h-full bg-white shadow-2xl overflow-y-auto">
          <!-- Header -->
          <div
            class="flex justify-between items-center p-5 border-b border-gray-100 bg-linear-to-r from-violet-50 to-purple-50">
            <div class="flex items-center gap-2">
              <h2 class="text-lg font-bold text-gray-900">Menu</h2>
            </div>
            <button @click="closeMenu" class="p-2 rounded-lg hover:bg-white transition-colors"
              aria-label="Close navigation menu">
              <Icons name="X" class="w-6 h-6 text-gray-800" />
            </button>
          </div>
          <!-- Mobile Search -->
          <div class="p-4 border-b border-gray-100">
            <div class="relative">
              <input type="text" placeholder="Search..."
                class="w-full px-4 py-2.5 pl-10 rounded-lg border border-gray-200 focus:border-violet-600 focus:outline-none transition-colors" />
              <Search class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
            </div>
          </div>
          <!-- Navigation Links -->
          <nav class="p-4">
            <ul class="space-y-1">
              <li v-for="link in Navlinks" :key="link.label + '-mobile'">
                <RouterLink :to="link.to" class="flex items-center gap-3 p-3.5 rounded-xl transition-all duration-300"
                  :class="isActiveRoute(link.to)
                    ? 'bg-linear-to-r from-violet-100 to-purple-100 text-violet-600 shadow-sm'
                    : 'text-gray-700 hover:bg-gray-50'" @click="closeMenu" :aria-label="link.ariaLabel">
                  <div class="w-10 h-10 rounded-lg flex items-center justify-center transition-colors" :class="isActiveRoute(link.to)
                    ? 'bg-white shadow-sm'
                    : 'bg-gray-100'">
                    <Icons :name="link.icon" class="w-5 h-5" />
                  </div>
                  <span class="font-medium">{{ link.label }}</span>
                </RouterLink>
              </li>
            </ul>
          </nav>
          <!-- Mobile Actions -->
          <div class="p-4 space-y-3 border-t border-gray-100">
            <!-- Quick Actions -->
            <div class="grid grid-cols-3 gap-2">
              <button
                class="flex flex-col items-center gap-2 p-3 rounded-xl bg-gray-50 hover:bg-gray-100 transition-colors">
                <Heart class="w-5 h-5 text-gray-700" />
                <span class="text-xs font-medium text-gray-700">Wishlist</span>
              </button>
              <button
                class="flex flex-col items-center gap-2 p-3 rounded-xl bg-gray-50 hover:bg-gray-100 transition-colors relative">
                <ShoppingCart class="w-5 h-5 text-gray-700" />
                <span class="text-xs font-medium text-gray-700">Cart</span>
                <span v-if="cartCount > 0"
                  class="absolute top-1 right-1 w-4 h-4 bg-violet-600 text-white text-xs font-bold rounded-full flex items-center justify-center">
                  {{ cartCount }}
                </span>
              </button>
              <button
                class="flex flex-col items-center gap-2 p-3 rounded-xl bg-gray-50 hover:bg-gray-100 transition-colors">
                <User class="w-5 h-5 text-gray-700" />
                <span class="text-xs font-medium text-gray-700">Profile</span>
              </button>
            </div>
            <!-- Login Button -->
            <RouterLink to="/login" @click="closeMenu"
              class="flex items-center justify-center gap-2 w-full px-6 py-3.5 text-sm font-semibold text-white bg-linear-to-r from-violet-600 to-purple-600 rounded-xl hover:shadow-lg transition-all duration-300">
              <User class="w-4 h-4" />
              Login / Register
            </RouterLink>
          </div>
        </div>
      </div>
    </transition>
  </header>
</template>