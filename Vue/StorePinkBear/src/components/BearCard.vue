<script setup>
import { computed } from 'vue'; 
const props = defineProps({
  product: {
    type: Object,
    required: true
  },
  userRole: {
    type: String,
    default: ''
  }
});
defineEmits(['add-to-cart']);

const finalPrice = computed(() => {
  const cost = props.product.tovarCost ?? 0;
  const sale = props.product.tovarSale ?? 0;
  if (sale > 0) {
    return cost - (cost * sale / 100);
  }
  return cost;
});

const cardStatusCostClass = computed(() => {
  if (props.product.tovarSale > 15) return 'high-sale'; 
  return '';
});

const cardStatusCountClass = computed(() => {
  if (props.product.tovarCount === 0) return 'out-of-stock';
  return '';
});
</script>

<template>
  <div class="bear-card" :class="cardStatusCostClass">
    <div class="image-wrapper">
      <img 
        :src="product.tovarPhoto ? `/${product.tovarPhoto}` : '/placeholder.png'" 
        :alt="product.tovarTitle">
    </div>
    <div class="info">
      <h3 :class="cardStatusCountClass">{{ product.tovarTitle }}</h3>
      <p class="desc">{{ product.tovarDescription }}</p>
      <div class="price-row">
        <div class="price-container">
          <template v-if="product.tovarSale > 0">
            <span class="old-price">{{ product.tovarCost.toLocaleString() }} ₽</span>
            <span class="current-price">{{ finalPrice.toLocaleString() }} ₽</span>
          </template>
          <span v-else class="current-price">{{ product.tovarCost.toLocaleString() }} ₽</span>
        </div>
        <button 
          v-if="userRole === 'авторизованный клиент'" 
          @click.stop="$emit('add-to-cart', product)"
        >В корзину</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.bear-card {
  background: white;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(255, 182, 193, 0.2);
  transition: all 0.3s;
  border: 2px solid transparent;
}

.high-sale {
  background-color: #2E8B57 !important;
  color: white;
}
.high-sale h3, .high-sale .desc { color: white; }

.out-of-stock {
  background-color: #ADD8E6 !important; 
}

.image-wrapper img { width: 100%; height: 200px; object-fit: contain; background: white; }
.info { padding: 15px; }

.price-container { display: flex; flex-direction: column; }

.old-price {
  text-decoration: line-through;
  color: red;
  font-size: 0.9rem;
}

.current-price {
  font-weight: bold;
  color: #333; 
  font-size: 1.2rem;
}

.high-sale .current-price { color: white; }

.price-row { display: flex; justify-content: space-between; align-items: center; margin-top: 10px; }

button { background: #ffb6c1; border: none; padding: 8px 15px; border-radius: 20px; color: white; cursor: pointer; }
button:hover { background: #ff85a1; }

.stock-label { font-size: 0.8rem; font-weight: bold; color: #005a8d; margin-top: 5px; }
</style>