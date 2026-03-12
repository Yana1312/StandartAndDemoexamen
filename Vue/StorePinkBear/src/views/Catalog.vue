<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import Cookies from 'js-cookie';
import BearCard from '../components/BearCard.vue';
import AdminEditModal from '../components/AdminEditModal.vue';

const isModalOpen = ref(false);
const editingProduct = ref(null);

const openEdit = (product) => {
  if (userRole === 'администратор') {
    editingProduct.value = product;
    isModalOpen.value = true;
  }
};

const openCreate = () => {
  editingProduct.value = null;
  isModalOpen.value = true;
};

const products = ref([]);
const categories = ref([]);
const suppliers = ref([]);
const manufacturers = ref([]);
const loading = ref(true);

const userRole = Cookies.get('user_role');
const userId = Cookies.get('user_id');

const searchQuery = ref('');
const selectedCategory = ref('');
const sortOrder = ref('count_asc');

const API_URL = 'http://localhost:5265/api/Tovars';

const fetchExtraData = async () => {
  try {
    const resS = await axios.get(`http://localhost:5265/api/Tovars/GetSuppliers`);
    const resM = await axios.get(`http://localhost:5265/api/Tovars/GetManufacturers`);
    suppliers.value = resS.data;
    manufacturers.value = resM.data;
  } catch (e)
    { const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка загрузки поставщиков и производителей";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response); }
};

onMounted(async () => {
  await fetchProducts();
  if (userRole === 'менеджер' || userRole === 'администратор') {
    await fetchCategories();
    await fetchExtraData();
  }
});

const fetchProducts = async () => {
  loading.value = true;
  try {
    const params = {};
    if (userRole === 'менеджер' || userRole === 'администратор') {
      if (searchQuery.value) params.search = searchQuery.value;
      if (selectedCategory.value) params.categoryId = selectedCategory.value;
      params.sortBy = sortOrder.value;
    }

    const response = await axios.get(`${API_URL}/GetAll`, { params });
    products.value = response.data;
  } catch (e) {
    if (e.response?.status === 404){
      products.value = [];
    }
    else {
      const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка авторизация";
      alert(`Внимание: ${errorMessage}`);
      console.error("Полная ошибка:", e.response);
    }
  } finally {
    loading.value = false;
  }
};

const fetchCategories = async () => {
  try {
    const res = await axios.get(`${API_URL}/GetCategories`);
    categories.value = res.data;
  } catch (e) { console.error(e); }
};

const addToBasket = async (product) => {
  try {
    await axios.post(`${API_URL}/AddToBasket`, null, {
      params: {
        userId: userId,
        tovarId: product.tovarId,
        quantity: 1
      }
    });
    alert(`Мишка ${product.tovarTitle} добавлен в корзину!`);
  } catch (e) {
    const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка авторизация";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);
  }
};
</script>

<template>
  <div class="catalog-container">
    <div class="header-box">
      
      <div v-if="userRole === 'менеджер' || userRole === 'администратор'" class="admin-tools">
        <input 
          v-model="searchQuery" 
          @input="fetchProducts" 
          type="text" 
          placeholder="Поиск по названию..."
        />
        
        <select v-model="selectedCategory" @change="fetchProducts">
          <option value="">Все категории</option>
          <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryId">
            {{ cat.categoryTitle }}
          </option>
        </select>

        <select v-model="sortOrder" @change="fetchProducts">
          <option value="count_asc">количество - по уменьшению</option>
          <option value="count_desc">количество - по увеличению</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="loader">Ищем самых милых мишек...</div>


    <button v-if="userRole === 'администратор'" @click="openCreate" class="add-btn">
      Добавить мишку
    </button>

    <h1>количество выведенных товаров - {{products.length}}</h1>

    <div class="grid">
      <div v-for="p in products" :key="p.tovarId" @click="openEdit(p)" class="clickable-card">
        <BearCard
          :userRole="userRole"
          :product="p" @add-to-cart="addToBasket" />
      </div>
    </div>

    <AdminEditModal 
      :show="isModalOpen" 
      :product="editingProduct" 
      :userRole="userRole"
      :categories="categories"
      :suppliers="suppliers"
      :manufacturers="manufacturers"
      @close="isModalOpen = false"
      @refresh="fetchProducts"
    />

    <div v-if="!loading && products.length === 0" class="empty">
      Товары не найдены!
    </div>
  </div>
</template>

<style scoped>
.catalog-container { padding: 20px 0; }
h1 { color: #ff85a1; text-align: center; margin-bottom: 30px; }

.admin-tools {
  display: flex;
  gap: 15px;
  background: white;
  padding: 15px;
  border-radius: 12px;
  margin-bottom: 30px;
  box-shadow: 0 4px 10px rgba(255, 182, 193, 0.1);
}

.add-btn {
  padding: 12px;
  background-color: #ffb6c1;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  margin:10px 0px;
  transition: background 0.3s;
}

.admin-tools input { flex: 2; padding: 10px; border: 1px solid #ffe4e9; border-radius: 8px; }
.admin-tools select { flex: 1; padding: 10px; border: 1px solid #ffe4e9; border-radius: 8px; }

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 30px;
}

.loader, .empty { text-align: center; padding: 50px; font-size: 1.2rem; color: #999; }
</style>