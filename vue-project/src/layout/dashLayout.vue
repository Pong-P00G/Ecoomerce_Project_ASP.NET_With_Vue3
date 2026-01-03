<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { 
  LayoutDashboard, 
  Package, 
  Users, 
  Database, 
  LogOut, 
  Menu, 
  X,
  ChevronRight,
  Settings,
  Bell
} from 'lucide-vue-next'
import { useAuthStore } from '../stores/Auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const isSidebarOpen = ref(true)
const navigation = [
  { name: 'Dashboard', href: '/admin/dashboard', icon: LayoutDashboard },
  { name: 'Products', href: '/admin/product', icon: Package },
  { name: 'Users', href: '/admin/user', icon: Users },
  { name: 'Stock', href: '/admin/stock', icon: Database },
]

const handleLogout = async () => {
  try {
    await authStore.logout()
    router.push('/login')
  } catch (error) {
    console.error('Logout failed:', error)
  }
}

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 flex">
    <!-- Sidebar -->
    <aside 
      :class="[
        'bg-white border-r border-gray-200 transition-all duration-300 flex flex-col',
        isSidebarOpen ? 'w-64' : 'w-20'
      ]"
    >
      <!-- Logo Area -->
      <div class="h-16 flex items-center justify-between px-6 border-b border-gray-100">
        <div v-if="isSidebarOpen" class="flex items-center gap-2">
          <div class="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
            <Package class="w-5 h-5 text-white" />
          </div>
          <span class="font-bold text-xl text-gray-900 tracking-tight">AlieeShop</span>
        </div>
        <div v-else class="w-full flex justify-center">
          <div class="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
            <Package class="w-5 h-5 text-white" />
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 overflow-y-auto py-6 px-3 space-y-1">
        <router-link
          v-for="item in navigation"
          :key="item.name"
          :to="item.href"
          :class="[
            'flex items-center gap-3 px-3 py-2.5 rounded-xl transition-all duration-200 group',
            route.path === item.href 
              ? 'bg-blue-50 text-blue-600' 
              : 'text-gray-500 hover:bg-gray-50 hover:text-gray-900'
          ]"
        >
          <component 
            :is="item.icon" 
            :class="[
              'w-5 h-5 shrink-0 transition-colors',
              route.path === item.href ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
            ]" 
          />
          <span v-if="isSidebarOpen" class="font-medium text-sm">{{ item.name }}</span>
          <ChevronRight 
            v-if="isSidebarOpen && route.path === item.href" 
            class="w-4 h-4 ml-auto" 
          />
        </router-link>
      </nav>

      <!-- User Profile & Footer Actions -->
      <div class="p-4 border-t border-gray-100 space-y-2">
        <button 
          v-if="isSidebarOpen"
          class="w-full flex items-center gap-3 px-3 py-2 text-gray-500 hover:bg-gray-50 rounded-xl transition-colors"
        >
          <Settings class="w-5 h-5" />
          <span class="text-sm font-medium">Settings</span>
        </button>
        
        <button 
          @click="handleLogout"
          :class="[
            'w-full flex items-center gap-3 px-3 py-2.5 rounded-xl transition-all duration-200 group',
            isSidebarOpen ? 'text-red-500 hover:bg-red-50' : 'text-gray-500 hover:bg-gray-50 justify-center'
          ]"
        >
          <LogOut class="w-5 h-5 shrink-0" />
          <span v-if="isSidebarOpen" class="font-medium text-sm">Logout</span>
        </button>

        <div v-if="isSidebarOpen" class="mt-4 px-3 py-3 bg-gray-50 rounded-2xl flex items-center gap-3">
          <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold text-sm">
            {{ authStore.user?.username?.substring(0, 2).toUpperCase() || 'AD' }}
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm font-semibold text-gray-900 truncate">{{ authStore.user?.username || 'Admin' }}</p>
            <p class="text-xs text-gray-500 truncate">{{ authStore.user?.role || 'Administrator' }}</p>
          </div>
        </div>
      </div>
    </aside>

    <!-- Main Content Area -->
    <div class="flex-1 flex flex-col h-screen overflow-hidden">
      <!-- Top Header -->
      <header class="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-8 sticky top-0 z-10">
        <div class="flex items-center gap-4">
          <button 
            @click="toggleSidebar" 
            class="p-2 hover:bg-gray-100 rounded-lg transition-colors text-gray-500"
          >
            <Menu v-if="!isSidebarOpen" class="w-5 h-5" />
            <X v-else class="w-5 h-5" />
          </button>
          <h1 class="text-lg font-semibold text-gray-800">
            {{ navigation.find(n => n.href === route.path)?.name || 'Admin Panel' }}
          </h1>
        </div>

        <div class="flex items-center gap-3">
          <button class="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-all relative">
            <Bell class="w-5 h-5" />
            <span class="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white"></span>
          </button>
          <div class="h-8 w-[1px] bg-gray-200 mx-2"></div>
          <div class="flex items-center gap-3 pl-2">
            <div class="text-right hidden sm:block">
              <p class="text-sm font-medium text-gray-900 leading-none">{{ authStore.user?.username || 'Admin' }}</p>
              <p class="text-[10px] text-gray-500 mt-1 uppercase tracking-wider font-bold">Online</p>
            </div>
            <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-blue-600 to-indigo-600 ring-2 ring-blue-50"></div>
          </div>
        </div>
      </header>

      <!-- Content -->
      <main class="flex-1 overflow-y-auto bg-gray-50/50">
        <router-view v-slot="{ Component }">
          <transition 
            name="fade" 
            mode="out-in"
          >
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Custom Scrollbar */
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: #cbd5e1;
}
</style>
