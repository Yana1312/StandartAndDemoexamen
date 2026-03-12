<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import Cookies from 'js-cookie';

const orders = ref([]);
const loading = ref(true);
const editingOrder = ref(null);
const userRole = Cookies.get('user_role');
const API_BASE_URL = 'http://localhost:5265/api';

const editForm = ref({
  orderId: null,
  orderStatus: '',
  orderDateDelivery: '',
  orderPoint: '',
  items: []
});

const catalog = ref([]);
const selectedProduct = ref('');
const addQuantity = ref(1);

const points = ref([]);

const fetchOrders = async () => {
  loading.value = true;
  try {
    const response = await axios.get(`${API_BASE_URL}/Tovars/GetAllOrders`);
    orders.value = response.data;
  } catch (e) { 
    const errorMessage = e.response?.data?.message || e.response?.data || "Ошибка загрузки заказов";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  } finally {
    loading.value = false;
  }
};

const fetchCatalog = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/Tovars/GetAll`);
    catalog.value = response.data;
  } catch (e) { 
    const errorMessage = e.response?.data?.message || e.response?.data || "Ошибка загрузки каталога";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  }
};

const fetchPoints = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/Tovars/GetPickUpPoints`);
    points.value = response.data;
  } catch (e) { 
    const errorMessage = e.response?.data?.message || e.response?.data || "Ошибка загрузки пункта выдачи";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  }
};

const openEditModal = (order) => {
  editingOrder.value = order;
  
  let deliveryDate = '';
  if (order.orderDateDelivery) {
    const date = new Date(order.orderDateDelivery);
    deliveryDate = date.toISOString().split('T')[0];
  }
  
  editForm.value = {
    orderId: order.orderId,
    orderStatus: order.orderStatus,
    orderDateDelivery: deliveryDate,
    orderPoint: order.orderPoint,
    items: order.items.map(item => ({
      itemId: item.itemId,
      tovarId: item.product?.tovarId || item.itemId,
      tovarTitle: item.product?.tovarTitle || 'Товар',
      tovarCost: item.product?.tovarCost || 0,
      quantity: item.itemCount
    }))
  };
};

const updateItemQuantity = (itemId, newQuantity) => {
  const item = editForm.value.items.find(i => i.itemId === itemId);
  if (item) {
    item.quantity = parseInt(newQuantity) || 0;
    if (newQuantity == 0)
      editForm.value.items = editForm.value.items.filter(i => i.itemId !== itemId);
  }
};

const removeItem = (itemId) => {
  if (confirm('Убрать этот товар из заказа?')) {
    editForm.value.items = editForm.value.items.filter(i => i.itemId !== itemId);
  }
};

const addNewItem = () => {
  if (!selectedProduct.value) {
    alert('Выберите товар');
    return;
  }

  const product = catalog.value.find(p => p.tovarId === parseInt(selectedProduct.value));
  if (!product) return;

  const existingItem = editForm.value.items.find(i => i.tovarId === product.tovarId);
  
  if (existingItem) {
    existingItem.quantity += addQuantity.value;
  } else {
    const newItem = {
      itemId: `temp_${Date.now()}_${product.tovarId}`,
      tovarId: product.tovarId,
      tovarTitle: product.tovarTitle,
      tovarCost: product.tovarCost,
      quantity: addQuantity.value
    };
    editForm.value.items.push(newItem);
  }

  selectedProduct.value = '';
  addQuantity.value = 1;
};

const saveOrderChanges = async () => {
  try {
    const requestData = {
      orderDateDelivery: editForm.value.orderDateDelivery || null,
      orderPoint: editForm.value.orderPoint || null,
      orderStatus: editForm.value.orderStatus,
      items: editForm.value.items.map(item => ({
        tovarId: item.tovarId,
        quantity: item.quantity
      }))
    };

    console.log('Отправка данных:', requestData);

    const response = await axios.put(
      `${API_BASE_URL}/Tovars/UpdateOrder/${editForm.value.orderId}`, 
      requestData
    );

    if (response.data.success) {
      alert("Заказ успешно обновлен!");
      editingOrder.value = null;
      fetchOrders();
    }
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data || "Ошибка при сохранении";
    alert(`Ошибка: ${errorMessage}`);
    console.error("Детали ошибки:", e.response);
  }
};

const deleteOrder = async (orderId) => {
  if (!confirm('Вы уверены, что хотите удалить этот заказ? Это действие нельзя отменить.')) {
    return;
  }

  try {
    const response = await axios.delete(`${API_BASE_URL}/Tovars/DeleteOrder/${orderId}`);
    
    if (response.data.success) {
      alert('Заказ успешно удален');
      fetchOrders();
    }
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data || "Ошибка при удалении заказа";
    alert(`Ошибка: ${errorMessage}`);
    console.error("Детали ошибки:", e.response);
  }
};

const cancelEdit = () => {
  editingOrder.value = null;
};

const formTotalSum = computed(() => {
  return editForm.value.items.reduce((sum, item) => {
    const price = item.tovarCost || 0;
    return sum + (price * item.quantity);
  }, 0);
});

const canEditOrder = (order) => {
  return order.orderStatus !== 'Завершен' && order.orderStatus !== 'В корзине';
};

onMounted(() => {
  fetchOrders();
  fetchPoints();
  fetchCatalog();
});
</script>

<template>
  <div class="orders-page">
    <div class="header-section">
      <h1>Управление заказами</h1>
    </div>

    <div v-if="loading" class="loader">Загрузка данных...</div>

    <div v-else class="orders-list">
      <div v-for="order in orders" :key="order.orderId" class="order-card">
        <template v-if="editingOrder?.orderId !== order.orderId">
          <div class="order-header">
            <span class="order-number">Заказ №{{ order.orderId }}</span>
            <span :class="['status-badge', order.orderStatus]">{{ order.orderStatus }}</span>
          </div>

          <div class="order-info">
            <p><strong>Клиент:</strong> {{ order.userFull }}</p>
            <p><strong>Дата заказа:</strong> {{ order.orderDate }}</p>
            <p><strong>Дата доставки:</strong> {{ order.orderDateDelivery }}</p>
            <p><strong>Пункт выдачи:</strong> {{ order.pickUpPointAddress }}</p>
          </div>

          <table class="items-table">
            <thead>
              <tr>
                <th>Товар</th>
                <th>Кол-во</th>
                <th>Цена</th>
                <th>Сумма</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in order.items" :key="item.itemId">
                <td>{{ item.product.tovarTitle }}</td>
                <td>{{ item.itemCount }} шт.</td>
                <td>{{ item.product.tovarCost?.toLocaleString() }} ₽</td>
                <td>{{ item.totalPrice?.toLocaleString() }} ₽</td>
              </tr>
            </tbody>
          </table>

          <div class="order-footer">
            <div class="total">Итого: <strong>{{ order.totalSum?.toLocaleString() }} ₽</strong></div>
            
            <div class="actions">
              <button 
                v-if="userRole === 'администратор' && canEditOrder(order)" 
                @click="openEditModal(order)" 
                class="edit-btn"
                :title="!canEditOrder(order) ? 'Заказ нельзя редактировать' : ''"
              > Редактировать заказ
              </button>

              <button 
                v-if="userRole === 'администратор' && order.orderStatus === 'Завершен'" 
                @click="deleteOrder(order.orderId)" 
                class="delete-btn"
              > Удалить заказ
              </button>

              <span v-else-if="!canEditOrder(order)" class="disabled-note">
                (заказ нельзя редактировать)
              </span>
            </div>
          </div>
        </template>

        <template v-else>
          <div class="edit-header">
            <h3>Редактирование заказа №{{ order.orderCode }}</h3>
            <button @click="cancelEdit" class="close-btn">✕</button>
          </div>

          <div class="edit-form">
            <div class="form-row">
              <div class="form-group">
                <label>Статус:</label>
                <select v-model="editForm.orderStatus">
                  <option value="Новый">Новый</option>
                  <option value="В пути">В пути</option>
                  <option value="Готов к выдаче">Готов к выдаче</option>
                  <option value="Завершен">Завершен</option>
                </select>
              </div>

              <div class="form-group">
                <label>Дата доставки:</label>
                <input 
                  type="date" 
                  v-model="editForm.orderDateDelivery" 
                  :min="new Date().toISOString().split('T')[0]"
                  :max="new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0]"
                />
              </div>

              <div class="form-group">
                <label>Пункт выдачи:</label>
                <select v-model="editForm.orderPoint">
                  <option :value="null">Не выбрано</option>
                  <option v-for="point in points" :key="point.pointId" :value="point.pointId">
                    {{ point.fullAddress }}
                  </option>
                </select>
              </div>
            </div>

            <div class="items-section">
              <h4>Товары в заказе:</h4>
              
              <div v-for="item in editForm.items" :key="item.itemId" class="edit-item">
                <span class="item-title">{{ item.tovarTitle }}</span>
                
                <div class="item-controls">
                  <input 
                    type="number" 
                    :value="item.quantity"
                    @input="updateItemQuantity(item.itemId, $event.target.value)"
                    min="0"
                    class="item-count"
                  />
                  <span class="item-price">× {{ item.tovarCost?.toLocaleString() }} ₽</span>
                  <span class="item-total">= {{ (item.tovarCost * item.quantity).toLocaleString() }} ₽</span>
                  <button @click="removeItem(item.itemId)" class="remove-item-btn" title="Удалить">✕</button>
                </div>
              </div>

              <div class="add-item-section">
                <div class="add-item-form">
                  <select v-model="selectedProduct" class="product-select">
                    <option value="" disabled>Выберите товар...</option>
                    <option v-for="product in catalog" :key="product.tovarId" :value="product.tovarId">
                      {{ product.tovarTitle }} - {{ product.tovarCost?.toLocaleString() }} ₽
                    </option>
                  </select>
                  
                  <input 
                    type="number" 
                    v-model.number="addQuantity" 
                    min="1"
                    class="quantity-input"
                    placeholder="Кол-во"
                  />
                  
                  <button @click="addNewItem" class="add-btn">Добавить</button>
                </div>
              </div>
            </div>

            <div class="edit-footer">
              <div class="total">Итого: <strong>{{ formTotalSum.toLocaleString() }} ₽</strong></div>
              
              <div class="edit-actions">
                <button @click="saveOrderChanges" class="save-btn">Сохранить изменения</button>
                <button @click="cancelEdit" class="cancel-btn">Отмена</button>
              </div>
            </div>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>
.orders-page { padding: 20px 0; }

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

h1 { color: #ff85a1; }

.order-card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 4px 15px rgba(255, 182, 193, 0.15);
  border: 1px solid #ffe4e9;
}

.order-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 15px;
  border-bottom: 1px solid #eee;
  padding-bottom: 10px;
}

.order-number { font-weight: bold; font-size: 18px; color: #333; }

.status-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: bold;
}

.status-badge.Новый { background: #e3f2fd; color: #1976d2; }
.status-badge.В пути { background: #fff3e0; color: #f57c00; }
.status-badge.Готов к выдаче { background: #e8f5e9; color: #388e3c; }
.status-badge.Завершен { background: #f1f8e9; color: #2e7d32; }

.items-table {
  width: 100%;
  border-collapse: collapse;
  margin: 15px 0;
}

.items-table th { 
  text-align: left; 
  color: #999; 
  font-size: 13px; 
  padding-bottom: 10px; 
}

.items-table td { 
  padding: 8px 0; 
  border-top: 1px solid #f9f9f9; 
}

.order-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px dashed #ffb6c1;
}

.total { font-size: 18px; }
.actions { display: flex; gap: 10px; }

.edit-btn, .delete-btn {
  background: #4a90e2;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 20px;
  cursor: pointer;
}

.edit-btn:hover { background: #357abd; }

select {
  padding: 6px 12px;
  border-radius: 20px;
  border: 1px solid #ffb6c1;
  background-color: white;
}

.edit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.close-btn { background: none; border: none; font-size: 18px; cursor: pointer; color: #999; }

.form-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 15px;
  margin-bottom: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.form-group label { font-size: 13px; color: #666; }

.form-group input,
.form-group select {
  padding: 8px 12px;
  border: 1px solid #ffb6c1;
  border-radius: 6px;
  font-size: 14px;
}

.items-section {
  background: #f9f9f9;
  padding: 15px;
  border-radius: 8px;
  margin: 15px 0;
}

.edit-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px;
  background: white;
  border-radius: 6px;
  margin-bottom: 8px;
}

.edit-item span:first-child { flex: 2; }

.item-controls {
  display: flex;
  align-items: center;
  gap: 10px;
  flex: 3;
}

.item-count {
  width: 60px;
  padding: 5px;
  border: 1px solid #ffb6c1;
  border-radius: 4px;
  text-align: center;
}

.item-price { color: #666; min-width: 70px; }
.item-total { color: #ff85a1; font-weight: bold; min-width: 90px; }

.remove-item-btn {
  background: none;
  border: none;
  color: #ff6b6b;
  font-size: 16px;
  cursor: pointer;
  padding: 0 5px;
}

.remove-item-btn:hover { color: #ff0000; }

.add-item-form {
  display: flex;
  gap: 8px;
  margin-top: 15px;
}

.product-select { flex: 2; padding: 8px; border: 1px solid #ffb6c1; border-radius: 6px; }
.quantity-input { width: 70px; padding: 8px; border: 1px solid #ffb6c1; border-radius: 6px; }
.add-btn { background: #4a90e2; color: white; border: none; padding: 8px 15px; border-radius: 6px; cursor: pointer; }

.edit-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #ffb6c1;
}

.edit-actions { display: flex; gap: 10px; }
.save-btn { background: #ff85a1; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; }
.cancel-btn { background: #f0f0f0; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; }

.loader {
  text-align: center;
  padding: 40px;
  color: #ff85a1;
  font-size: 1.2rem;
}
</style>

// Вычисляемое свойство для отсортированных заказов (от новых к старым)
const sortedOrders = computed(() => {
  return [...orders.value].sort((a, b) => {
    const dateA = new Date(a.orderDate);
    const dateB = new Date(b.orderDate);
    return dateB - dateA; // От новых к старым (обратный порядок)
  });
});

/* Статистика по статусам заказов
const orderStatistics = computed(() => {
  const stats = {
    'Новый': 0,
    'В пути': 0,
    'Готов к выдаче': 0,
    'Завершен': 0,
    'В корзине': 0
  };
  
  orders.value.forEach(order => {
    if (stats.hasOwnProperty(order.orderStatus)) {
      stats[order.orderStatus]++;
    }
  });
  
  return stats;
});

// Общее количество заказов
const totalOrdersCount = computed(() => {
  return orders.value.length;
});
*/