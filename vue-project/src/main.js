import './assets/style.css';
import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import router from './routes';
import { useAuthStore } from './stores/Auth'


const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
// Initialize auth
const authStore = useAuthStore();
authStore.initializeAuth();


app.mount('#app');