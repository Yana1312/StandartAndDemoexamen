<script setup>
import { ref, watch } from 'vue';
import axios from 'axios';

const props = defineProps({
  show: Boolean,
  product: Object,  
  categories: Array,
  suppliers: Array,
  manufacturers: Array
});

const emit = defineEmits(['close', 'refresh']);

const form = ref({
  tovarTitle: '',
  tovarCost: 0,
  tovarCount: 0,
  tovarDescription: '',
  tovarCategory: '',
  tovarManufactur: '',
  tovarSupplier: '', 
  tovarPhoto: ''
});

watch(() => props.product, (newVal) => {
  if (newVal) form.value = { ...newVal };
  else form.value = { tovarTitle: '', tovarCost: 0, tovarCount: 0, 
      tovarDescription: '', tovarCategory: '', 
      tovarManufactur: '', tovarSupplier: '', tovarPhoto: ''  };
}, { immediate: true });

const save = async () => {
  try {
    if (props.product?.tovarId) {
      await axios.put(`http://localhost:5265/api/Tovars/RedactTovar`, form.value);
    } else {
      await axios.post(`http://localhost:5265/api/Tovars/AddTovar`, form.value);
    }
    emit('refresh');
    emit('close');
  } catch (e)
   { const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка сохранения";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);}
};

const remove = async () => {
  if (!confirm("Удалить этого мишку навсегда?")) return;
  try {
    await axios.delete(`http://localhost:5265/api/Tovars/DeleteTovar/${props.product.tovarId}`);
    emit('refresh');
    emit('close');
  } catch (e) { const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка удаления";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);}
};
</script>

<template>
  <div v-if="show" class="modal-overlay" @click.self="emit('close')">
    <div class="modal-content">
      <h2>{{ props.product ? 'Редактирование' : 'Новый мишка' }}</h2>
      
      <div class="field-label">Название товара</div>
      <input v-model="form.tovarTitle" placeholder="Название">
      <div class="field-label">Цена</div>
      <input v-model.number="form.tovarCost" type="number" placeholder="Цена">
      <div class="field-label">Количество на складе</div>
      <input v-model.number="form.tovarCount" type="number" placeholder="Количество на складе">
      <div class="field-label">Описание</div>
      <textarea v-model="form.tovarDescription" placeholder="Описание"></textarea>
      
      <div class="field-label">Выберите категорию</div>
      <select v-model="form.tovarCategory">
        <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryId">
          {{ cat.categoryTitle }}
        </option>
      </select>

      <div class="field-label">Выберите производителя</div>
        <select v-model="form.tovarManufactur">
            <option v-for="m in manufacturers" :key="m.manufacturId" :value="m.manufacturId">
                {{ m.manufacturTitle }}
            </option>
        </select>

      <div class="field-label">Выберите поставщика</div>
        <select v-model="form.tovarSupplier">
            <option v-for="s in suppliers" :key="s.supplierId" :value="s.supplierId">
                {{ s.supplierTitle }}
            </option>
        </select>

      <div class="field-label">Фотография товара</div>
      <input v-model="form.tovarPhoto" placeholder="Имя файла картинки (из public)">

      <div class="actions">
        <button @click="save" class="save-btn">Сохранить</button>
        <button v-if="props.product" @click="remove" class="delete-btn">Удалить</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.field-label { font-weight: 600; color: #555; font-size: 14px;}
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-content { background: white; padding: 25px; border-radius: 15px; width: 400px; display: flex; flex-direction: column; gap: 10px; }
input, textarea, select { padding: 10px; border: 1px solid #ddd; border-radius: 8px; }
.actions { display: flex; gap: 10px; margin-top: 10px; }
.save-btn { background: #ff85a1; color: white; border: none; padding: 10px; border-radius: 8px; flex: 1; cursor: pointer; }
.delete-btn { background: #ff4d4d; color: white; border: none; padding: 10px; border-radius: 8px; cursor: pointer; }
</style>