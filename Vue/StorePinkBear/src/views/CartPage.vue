<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import Cookies from 'js-cookie';

const cartItems = ref([]);
const orders = ref([]);
const loading = ref(true);
const points = ref([]); 
const selectedPoint = ref('');
const userId = Cookies.get('user_id');
const API_URL = 'http://localhost:5265/api/Tovars';

const fetchCart = async () => {
  if (!userId) return;
  loading.value = true;
  try {
    const response = await axios.get(`${API_URL}/GetBasketItems`, {
      params: { userId: userId }
    });
    cartItems.value = response.data;
    console.log('Корзина:', cartItems.value);
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка загрузки корзины";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  } finally {
    loading.value = false;
  }
};

const fetchOrders = async () => {
  if (!userId) return;
  loading.value = true;
  try {
    const response = await axios.get(`${API_URL}/GetUserOrders`, {
      params: { userId: userId }
    });
    orders.value = response.data;
    console.log('Заказы:', orders.value);
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка загрузки заказов";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  } finally {
    loading.value = false;
  }
};


const fetchPoints = async () => {
  try {
    const response = await axios.get(`${API_URL}/GetPickUpPoints`);
    points.value = response.data;
    console.log('Пункты выдачи:', points.value);
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка загрузки пунктов выдачи";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  }
};

const removeFromCart = async (tovarId) => {
  try {
    await axios.delete(`${API_URL}/RemoveFromBasket`, {
      params: { userId, tovarId }
    });
    fetchCart();
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка загрузки пунктов выдачи";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  }
};

const totalSum = computed(() => {
  return cartItems.value.reduce((sum, item) => {
    const price = item.product?.tovarCost || 0;
    const quantity = item.quantity || 1;
    return sum + (price * quantity);
  }, 0);
});

const checkout = async () => {
  if (!selectedPoint.value) {
    alert('Выберите пункт выдачи');
    return;
  }

  try {
    const orderData = {
      userId: Number(userId),
      pickUpPointId: Number(selectedPoint.value),
      items: cartItems.value.map(item => ({
        tovarId: item.tovarId,
        quantity: item.quantity || 1
      }))
    };

    console.log('Отправка заказа:', orderData);

    const response = await axios.post(`${API_URL}/CreateOrder`, orderData);

    if (response.data.success) {
      alert(`Заказ №${response.data.orderCode} успешно оформлен!`);
      cartItems.value = [];
      selectedPoint.value = '';
    }
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка загрузки пунктов выдачи";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  }
};

const formatPrice = (price) => {
  if (price === undefined || price === null) return '0';
  return Number(price).toLocaleString('ru-RU');
};

onMounted(() => {
  fetchCart();
  fetchOrders();
  fetchPoints(); 
});
</script>

<template>
  <div class="cart-container">
    <div v-for="item in orders" :key="item.orderId" class="cart-item">
        <div class="item-details">
          <h3>{{ item.orderCode  }}</h3>
          <h3>{{ item.orderStatus  }}</h3>
      </div>
    </div>

    <h1>Ваша корзина</h1>
    </div>

</template>

<style scoped>
.cart-container { 
  padding: 30px 20px; 
  max-width: 900px; 
  margin: 0 auto; 
}

h1 { 
  color: #ff85a1; 
  margin-bottom: 30px; 
}

.cart-item {
  display: flex;
  align-items: center;
  background: white;
  padding: 15px;
  border-radius: 12px;
  margin-bottom: 15px;
  box-shadow: 0 4px 10px rgba(255, 182, 193, 0.1);
}

.cart-img { 
  width: 80px; 
  height: 80px; 
  object-fit: cover; 
  border-radius: 8px; 
  margin-right: 20px; 
}

.item-details { 
  flex: 2; 
}

.item-details h3 { 
  margin: 0 0 5px 0; 
  font-size: 1.1rem; 
}

.item-details p { 
  margin: 0; 
  color: #ff85a1; 
  font-weight: bold; 
}

.item-quantity { 
  flex: 1; 
  text-align: center; 
}

.item-total { 
  flex: 1; 
  font-weight: bold; 
  color: #ff85a1; 
  text-align: right; 
  margin-right: 20px; 
}

.remove-btn { 
  background: none; 
  border: none; 
  color: #ffb6c1; 
  font-size: 1.2rem; 
  cursor: pointer; 
  padding: 5px 10px;
}

.remove-btn:hover { 
  color: #ff85a1; 
}

.cart-summary {
  margin-top: 30px;
  padding: 20px;
  background: #fff0f3;
  border-radius: 12px;
}

.delivery-selection {
  margin-bottom: 20px;
  text-align: left;
}

.delivery-selection label {
  display: block;
  margin-bottom: 8px;
  color: #666;
  font-weight: bold;
}

.point-dropdown {
  width: 100%;
  padding: 10px;
  border: 1px solid #ffb6c1;
  border-radius: 8px;
  background: white;
  font-size: 14px;
  outline: none;
}

.point-dropdown:focus {
  border-color: #ff85a1;
  box-shadow: 0 0 5px rgba(255, 133, 161, 0.3);
}

.checkout-btn {
  width: 100%;
  background: #ff85a1;
  color: white;
  border: none;
  padding: 15px;
  border-radius: 25px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  margin-top: 15px;
  transition: all 0.3s;
}

.checkout-btn:hover:not(:disabled) { 
  background: #ff6b8e; 
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(255, 133, 161, 0.3);
}

.checkout-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.empty-cart { 
  text-align: center; 
  padding: 50px; 
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(255, 182, 193, 0.1);
}

.back-link { 
  color: #ff85a1; 
  text-decoration: underline;
  display: inline-block;
  margin-top: 15px;
}

.loader {
  text-align: center;
  padding: 50px;
  color: #999;
}
</style>