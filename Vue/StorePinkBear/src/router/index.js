import { createRouter, createWebHistory } from 'vue-router';
import Cookies from 'js-cookie';

import AuthPage from '../views/Auth.vue';
import CatalogPage from '../views/Catalog.vue';
import OrdersPage from '../views/Orders.vue';
import CartPage from '../views/CartPage.vue';

const routes = [
  {
    path: '/auth',
    name: 'Auth',
    component: AuthPage,
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    name: 'Catalog',
    component: CatalogPage,
    meta: { 
      requiresAuth: true, 
      allowedRoles: ['авторизованный клиент', 'менеджер', 'администратор', 'гость'] 
    }
  },
  {
    path: '/orders',
    name: 'Orders',
    component: OrdersPage,
    meta: { 
      requiresAuth: true, 
      allowedRoles: ['администратор'] 
    }
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  },
  {
    path: '/cart',
    name: 'Cart',
    component: CartPage,
    meta: { 
      requiresAuth: true, 
      allowedRoles: ['авторизованный клиент'] 
    }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const userRole = Cookies.get('user_role');
  const isAuthenticated = !!userRole;

  if (to.meta.requiresAuth && !isAuthenticated) {
    return next('/auth');
  }

  if (to.path === '/auth' && isAuthenticated) {
    return next('/');
  }

  if (to.meta.allowedRoles && !to.meta.allowedRoles.includes(userRole)) {
    console.warn(`Доступ запрещен для роли: ${userRole}`);
    return next('/');
  }

  next();
});

export default router;