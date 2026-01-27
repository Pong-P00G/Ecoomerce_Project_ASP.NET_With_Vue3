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
  <div class="min-h-screen bg-[#F8FAFC] flex font-sans">
    <!-- Mobile Overlay -->
    <div 
      v-if="isSidebarOpen" 
      class="fixed inset-0 bg-slate-900/60 backdrop-blur-sm z-40 lg:hidden transition-opacity duration-300"
      @click="isSidebarOpen = false"
    ></div>

    <!-- Floating Sidebar (Desktop) / Fixed (Mobile) -->
    <aside 
      :class="[
        'fixed lg:sticky top-0 z-50 transition-all duration-500 cubic-bezier(0.4, 0, 0.2, 1) flex flex-col',
        'lg:h-[calc(100vh-2rem)] lg:my-4 lg:ml-4 rounded-[32px]', // Floating effect on desktop
        'bg-slate-900 text-white shadow-2xl shadow-slate-200/50 overflow-hidden',
        isSidebarOpen ? 'w-72 translate-x-0' : 'w-[88px] -translate-x-full lg:translate-x-0'
      ]"
    >
      <!-- Background Effects -->
      <div class="absolute top-0 right-0 w-64 h-64 bg-blue-500/10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2 pointer-events-none"></div>
      <div class="absolute bottom-0 left-0 w-64 h-64 bg-indigo-500/10 rounded-full blur-3xl translate-y-1/2 -translate-x-1/2 pointer-events-none"></div>

      <!-- Logic Header -->
      <div class="h-24 flex items-center px-6 shrink-0 relative z-10">
        <div class="flex items-center gap-4 overflow-hidden">
          <div class="w-10 h-10 bg-gradient-to-tr from-blue-600 to-indigo-600 rounded-xl flex items-center justify-center shrink-0 shadow-lg shadow-blue-900/40">
            <Package class="w-5 h-5 text-white" />
          </div>
          <div :class="['min-w-0 transition-opacity duration-300', isSidebarOpen ? 'opacity-100' : 'opacity-0 hidden']">
             <span class="block font-black text-lg tracking-tight leading-none text-white">AlieeShop</span>
             <span class="text-[10px] font-bold text-slate-400 uppercase tracking-[0.2em]">Admin</span>
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 overflow-y-auto py-6 px-4 space-y-2 custom-scrollbar relative z-10">
        <div v-if="isSidebarOpen" class="px-4 mb-4 transition-opacity duration-300">
          <p class="text-[10px] font-black text-slate-600 uppercase tracking-widest">Main Menu</p>
        </div>
        
        <router-link
          v-for="item in navigation"
          :key="item.name"
          :to="item.href"
          :class="[
            'flex items-center gap-4 px-4 py-4 rounded-2xl transition-all duration-300 group relative',
            route.path.startsWith(item.href)
              ? 'bg-blue-600 text-white shadow-xl shadow-blue-900/30 translate-x-1' 
              : 'text-slate-400 hover:bg-white/5 hover:text-white'
          ]"
        >
          <component 
            :is="item.icon" 
            :class="[
              'w-6 h-6 shrink-0 transition-all duration-300',
              route.path.startsWith(item.href) ? 'text-white scale-110' : 'text-slate-400 group-hover:text-white group-hover:scale-110'
            ]" 
          />
          <span :class="['font-bold text-sm tracking-wide whitespace-nowrap transition-all duration-300', isSidebarOpen ? 'opacity-100' : 'opacity-0 w-0 hidden']">{{ item.name }}</span>
          
          <!-- Active Glow -->
          <div 
            v-if="route.path.startsWith(item.href)" 
            class="absolute right-0 top-1/2 -translate-y-1/2 w-1.5 h-1.5 bg-white rounded-full mr-2 shadow-[0_0_8px_rgba(255,255,255,0.8)]"
          ></div>
        </router-link>
      </nav>

      <!-- User Profile & Footer -->
      <div class="px-4 pb-4 shrink-0 relative z-10 space-y-4">
        <!-- Collapse Toggle (Desktop) -->
        <button 
           @click="toggleSidebar"
           class="hidden lg:flex w-full items-center justify-center p-3 rounded-xl text-slate-400 hover:bg-white/5 hover:text-white transition-all border border-transparent hover:border-white/5 group"
        >
           <ChevronRight :class="['w-5 h-5 transition-transform duration-300', isSidebarOpen ? 'rotate-180' : '']" />
        </button>

        <div class="p-4 bg-white/5 backdrop-blur-md rounded-2xl border border-white/5 flex items-center gap-4 overflow-hidden">
          <div class="w-10 h-10 rounded-full bg-gradient-to-tr from-emerald-400 to-teal-500 flex items-center justify-center text-white font-black text-sm shrink-0 shadow-lg shadow-emerald-900/20">
            {{ authStore.user?.username?.substring(0, 2).toUpperCase() || 'AD' }}
          </div>
          <div :class="['min-w-0 flex-1 transition-opacity duration-300', isSidebarOpen ? 'opacity-100' : 'opacity-0 hidden']">
            <p class="text-sm font-bold text-white truncate">{{ authStore.user?.username || 'Admin' }}</p>
            <p class="text-[10px] font-medium text-slate-400 uppercase tracking-wider flex items-center gap-1.5">
              <span class="relative flex h-2 w-2">
                <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
                <span class="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"></span>
              </span>
              Online
            </p>
          </div>
          <button 
             @click="handleLogout"
             :class="['p-2 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-rose-500/10 transition-colors', isSidebarOpen ? '' : 'hidden']"
             title="Logout"
          >
             <LogOut class="w-4 h-4" />
          </button>
        </div>
      </div>
    </aside>

    <!-- Main Content Area -->
    <div class="flex-1 flex flex-col min-w-0 h-screen overflow-hidden">
      <!-- Mobile Header -->
      <header class="h-20 bg-white/80 backdrop-blur-xl border-b border-slate-100 flex items-center justify-between px-6 lg:hidden shrink-0 z-30 sticky top-0">
        <div class="flex items-center gap-4">
          <button 
            @click="isSidebarOpen = !isSidebarOpen" 
            class="p-3 bg-white border border-slate-200 rounded-xl text-slate-600 active:scale-95 shadow-sm"
          >
            <Menu v-if="!isSidebarOpen" class="w-6 h-6" />
            <X v-else class="w-6 h-6" />
          </button>
          <span class="font-black text-slate-900 tracking-tight text-lg">AlieeShop</span>
        </div>
        <div class="w-10 h-10 rounded-full bg-gradient-to-tr from-blue-600 to-indigo-600 flex items-center justify-center text-white font-bold text-sm shadow-md">
           {{ authStore.user?.username?.substring(0, 2).toUpperCase() || 'AD' }}
        </div>
      </header>

      <!-- Content -->
      <main class="flex-1 overflow-y-auto bg-[#F8FAFC] scroll-smooth p-4 lg:p-4">
        <router-view v-slot="{ Component }">
          <transition 
            name="scale-fade" 
            mode="out-in"
          >
            <div :key="route.path" class="w-full">
              <component :is="Component" />
            </div>
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<style scoped>
.scale-fade-enter-active,
.scale-fade-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.scale-fade-enter-from {
  opacity: 0;
  transform: scale(0.98) translateY(10px);
}

.scale-fade-leave-to {
  opacity: 0;
  transform: scale(0.98) translateY(-10px);
}

/* Custom Scrollbar */
.custom-scrollbar::-webkit-scrollbar {
  width: 0px; 
  background: transparent; 
}

.custom-scrollbar:hover::-webkit-scrollbar {
  width: 4px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.2);
}
</style>
