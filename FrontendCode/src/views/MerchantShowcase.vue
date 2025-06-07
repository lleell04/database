<template>
  <Navbar />
  <!-- <div v-if="loading" class="loading-overlay">
    <div class="loading-spinner"></div>
    <p>搜索中，请稍候...</p>
  </div> -->
  <div style="background-image: url('@/assets/wy/background.jpg'); background-size: cover; background-position: center; height: 100%; overflow-x: hidden;">
    <!-- <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div> -->
    <div v-if="errorMessage" class="error-message-container">
      <div class="error-container">
        <img src="@/assets/wy/cry.jpeg" alt="Cry" class="error-image" />
        <div class="error-text">{{ errorMessage }}</div>
      </div>
      
    </div>
    <div v-else class="store-page" >
      <div v-for="store in stores" :key="store.storeId" class="store-container">
        <div class="store-content">
          <div class="store-header">
            <img :src="store.storePhoto.imageUrl" alt="Store Avatar" class="store-avatar" @click="goToStoreDetail(store.storeId)"/>
            <div class="store-info">
              <h2 class="store-name" @click="goToStoreDetail(store.storeId)">{{ store.storeName }}</h2>
              <p class="store-rating">评分: {{ (store.storeScore ) }}</p>
            </div>
          </div>
          <div class="store-products">
            <div v-for="product in store.homeProducts" :key="product.productId" class="product-item">
              <img :src="product.productPics.length ? product.productPics[0].imageUrl : ''" alt="Product Image" class="product-image" @click="goToProductDetail(product.productId)"/>
              <p class="product-price">¥{{ product.productPrice }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axiosInstance from '../components/axios';
import Navbar from '../components/Navbar.vue';
import { useRouter} from 'vue-router';

const router = useRouter();
const stores = ref([]);
const loading = ref(true);  // 用于控制缓冲页面的显示
const errorMessage = ref(''); // 错误信息

const fetchStores = async (keyword: string, type: string) => {
  try {
    const response = await axiosInstance.get('/NaviSearch/search', {
      params: {
        keyword: keyword,
        type: type
      }
    });
    console.log('返回数据', response.data);

    if (response.data && response.data.length > 0) {
      stores.value = response.data;
      // stores.value = Array(100).fill(response.data).flat(); // 将数据重复100次并平展成一个数组
      errorMessage.value = ''; // 清除错误信息
    } else {
      stores.value = [];
      errorMessage.value = '没找到相关的商家...'; // 设置错误信息
    }
  } catch (error) {
    console.error('Error fetching stores:', error);
    errorMessage.value = '没找到相关的商家...'; // 设置错误信息
  } finally {
    loading.value = false;  // 数据获取完毕后关闭缓冲页面
  }
};
// 定义跳转函数
const goToStoreDetail = (storeId: string) => {
  localStorage.setItem('storeIdOfDetail', storeId);  // 存储 storeId
  console.log("跳转至/shopdetail")
  router.push('/shopdetail');  // 跳转到店铺详情页
};

const goToProductDetail=(productId: string) => {
  localStorage.setItem('productIdOfDetail', productId);  // 存储 productId
  console.log("跳转至/productdetail")
  router.push('/productdetail');  // 跳转到店铺详情页
};

onMounted(() => {
  const keyword = localStorage.getItem('keyword') || '';
  const type = localStorage.getItem('searchType') || '1';
  fetchStores(keyword, type);
});
</script>

<style scoped>
.store-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding: 20px;
  align-items: center; /* 居中对齐 */
  background-image: url('@/assets/wy/background.jpg'); background-size: cover; background-position: center; height: 100%; overflow-x: hidden;
}

.store-container {
  background-color: white;
  border: 1px solid #e7e7e7;
  border-radius: 10px;
  padding: 20px;
  width: 80%; /* 调整宽度 */
  max-width: 1200px; /* 限制最大宽度 */
}

.store-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  width: 100%; /* 使用全宽 */
}

.store-header {
  display: flex;
  align-items: center; /* 头像和名称居中对齐 */
  gap: 10px; /* 调整头像和店铺名称之间的间距 */
  margin-bottom: 20px;
  width: 100%; 
}

.store-avatar {
  width: 60px; /* 头像大小 */
  height: 60px; /* 头像大小 */
  border-radius:2px;
  /* border-radius: 50%; 将头像裁剪为圆形 */
  object-fit: cover;
  cursor: pointer;
}

.store-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start; /* 确保左对齐 */
  width: 100%; /* 让店铺名称和评分都占据同样的宽度 */
}

.store-name {
  font-size: 24px; /* 调整为更大的字体 */
  font-weight: bold;
  margin: 0;
  cursor: pointer; /* 鼠标悬停时显示为指针 */
  text-align: left; /* 确保左对齐 */
}

.store-rating {
  font-size: 18px; /* 调整为更大的字体 */
  color: #888;
  text-align: left; /* 确保左对齐 */
}

.store-name:hover {
  color: #a61b29; /* 鼠标悬停时更改颜色 */
}



.store-products {
  display: grid;
  gap: 10px;
  grid-template-columns: repeat(4, 1fr); /* 每行显示四个商品 */
  justify-content: end; /* 将商品从右往左对齐 */
  direction: rtl; /* 设置内容从右往左排列 */
  width: 100%; /* 保持容器的宽度 */
}

.product-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  direction: ltr; /* 保持商品内部从左到右的布局 */
}



.product-image {
  width: 150px; /* 固定图片宽度 */
  height: 150px; /* 固定图片高度 */
  object-fit: cover;
  border-radius: 5px;
  cursor: pointer;
}
.product-image:hover {
  border-color: #a61b29;
}
.product-price {
  font-size: 16px;
  color: #333;
  margin-top: 5px;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.loading-spinner {
  border: 5px solid #f3f3f3;
  border-radius: 50%;
  border-top: 5px solid #3498db;
  width: 50px;
  height: 50px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message-container {
  background-color: white;
  height: 100%;
}
.error-container{
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 50px;
}
.error-image {
  width: 120px;
  height: auto;
  margin-bottom: 20px; /* 给图片和文字之间留出空隙 */
}

.error-text {
  font-size: 24px;
  color: #a61b29;
  text-align: center;
}
</style>
