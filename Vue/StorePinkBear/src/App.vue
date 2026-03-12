<script setup>
import { computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import Cookies from 'js-cookie';

const router = useRouter();
const route = useRoute();

const userRole = computed(() => Cookies.get('user_role'));
const userFio = computed(() => Cookies.get('user_fio'));
const isAuthenticated = computed(() => !!userRole.value);

const logout = () => {
  Cookies.remove('user_role');
  Cookies.remove('user_id');
  Cookies.remove('user_fio');
  
  router.push('/auth');
};

const goToCatalog = () => router.push('/');
const goToCart = () => router.push('/cart');
const goToOrders = () => router.push('/orders');
</script>

<template>
  <div id="app">
    <header v-if="isAuthenticated && route.path !== '/auth'" class="main-header">
      <div class="header-content">
        <div class="logo" @click="goToCatalog">
          Pink Bear Store
        </div>

        <nav class="navigation">
          <button @click="goToCatalog">Каталог</button>

          <button v-if="userRole === 'авторизованный клиент'" 
              @click="goToCart"
            > Корзина </button>
  
          
          <button 
            v-if="userRole === 'менеджер' || userRole === 'администратор'" 
            @click="goToOrders"
          >
            Заказы
          </button>
        </nav>

        <div class="user-block">
          <span class="user-info">
            <span class="user-name">{{ userFio }}</span>
            <span class="user-role">{{ userRole }}</span>
          </span>
          <button class="logout-btn" @click="logout">Выйти</button>
        </div>
      </div>
    </header>

    <main class="container">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
  </div>
</template>

<style>
body {
  margin: 0;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background-color: #fffafb;
  color: #333;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.user-name {
  font-weight: 600;
  font-size: 15px;
  line-height: 1.4;
  color: white;
}

.user-role {
  font-size: 15px;
  color: rgba(255, 255, 255, 0.9);
  padding: 2px 8px;
  border-radius: 12px;
  margin-top: 4px;
  text-align: center;
}


.main-header {
  background-color: #ffb6c1;
  color: white;
  padding: 10px 0;
  box-shadow: 0 2px 10px rgba(255, 182, 193, 0.4);
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
}

.logo {
  font-size: 1.5rem;
  font-weight: bold;
  cursor: pointer;
}

.navigation button {
  background: transparent;
  border: 1px solid white;
  color: white;
  padding: 6px 15px;
  margin-right: 10px;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.3s;
}

.navigation button:hover {
  background: white;
  color: #ffb6c1;
}

.user-block {
  display: flex;
  align-items: center;
  gap: 15px;
}

.user-info {
  font-size: 14px;
}

.user-info small {
  opacity: 0.8;
  font-style: italic;
}

.logout-btn {
  background: #ff85a1;
  border: none;
  color: white;
  padding: 6px 12px;
  border-radius: 5px;
  cursor: pointer;
}

.logout-btn:hover {
  background: #ff4d6d;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>