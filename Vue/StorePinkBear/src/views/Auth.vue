<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import Cookies from 'js-cookie';

const router = useRouter();

const login = ref('');
const password = ref('');
const error = ref('');
const isLoading = ref(false);

const loginAsGuest = () => {
  Cookies.set('user_role', 'гость', { expires: 1 });
  Cookies.set('user_id', '1', { expires: 1 });
  router.push('/');
};

const handleLogin = async () => {
  error.value = '';
  isLoading.value = true;

  try {
    const BACKEND_URL = 'http://localhost:5265/api/Users/Authorization';

    const response = await axios.post(BACKEND_URL, null, {
      params: {
        email: login.value,
        password: password.value
      }
    });

    const user = response.data;

    if (user) {
      Cookies.set('user_role', user.userRole, { expires: 1 });
      Cookies.set('user_id', user.userId, { expires: 1 });
      
      const fio = `${user.userFirstname || ''} ${user.userName || ''}`.trim();
      Cookies.set('user_fio', fio, { expires: 1 });

      router.push('/');
    }
  } catch (e) {
    { const errorMessage = e.response?.data?.message || e.response?.data  || "Ошибка авторизация";
    alert(`Внимание: ${errorMessage}`);
    console.error("Полная ошибка:", e.response);}
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Вход в систему</h1>
      
      <form @submit.prevent="handleLogin">
        <div class="input-group">
          <label>Логин</label>
          <input 
            v-model="login" 
            type="text" 
            placeholder="Введите ваш логин" 
            :disabled="isLoading"
          />
        </div>

        <div class="input-group">
          <label>Пароль</label>
          <input 
            v-model="password" 
            type="password" 
            placeholder="Введите пароль" 
            :disabled="isLoading"
          />
        </div>

        <button type="submit" :disabled="isLoading">
          {{ isLoading ? 'Загрузка...' : 'Войти' }}
        </button>

        <div class="guest-section">
          <p class="guest-text">Или</p>
          <button type="button" @click="loginAsGuest" class="guest-btn">
            Войти как гость
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #fffafb;
}

.auth-card {
  background: white;
  padding: 40px;
  border-radius: 15px;
  box-shadow: 0 10px 25px rgba(255, 182, 193, 0.3);
  width: 100%;
  max-width: 400px;
  text-align: center;
}

.logo {
  font-size: 24px;
  margin-bottom: 10px;
}

h1 {
  color: #ff85a1;
  margin-bottom: 30px;
  font-size: 22px;
}

.input-group {
  text-align: left;
  margin-bottom: 20px;
}

.input-group label {
  display: block;
  margin-bottom: 5px;
  color: #666;
  font-size: 14px;
}

input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ffe4e9;
  border-radius: 8px;
  box-sizing: border-box;
}

input:focus {
  outline: none;
  border-color: #ffb6c1;
  box-shadow: 0 0 5px rgba(255, 182, 193, 0.5);
}

button {
  width: 100%;
  padding: 12px;
  background-color: #ffb6c1;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background 0.3s;
}

button:hover:not(:disabled) {
  background-color: #ff85a1;
}

button:disabled {
  background-color: #ffd9e0;
  cursor: not-allowed;
}

.error-msg {
  color: #d32f2f;
  background: #ffebee;
  padding: 10px;
  border-radius: 5px;
  margin-bottom: 20px;
  font-size: 14px;
}
</style>