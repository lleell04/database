<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axiosInstance from '../components/axios';
import Navbar from '../components/Navbar.vue';
import { useRouter} from 'vue-router';

const router = useRouter();

const products = ref([]);
const loading = ref(true);  // 用于控制缓冲页面的显示
const errorMessage = ref(''); // 错误信息

//回车
const keyword = ref(localStorage.getItem('keyword') || ''); // 绑定输入框的关键字
const type = ref(localStorage.getItem('searchType') || '0'); // 搜索类型


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
      products.value = response.data;
      //products.value = Array(100).fill(response.data).flat(); // 将数据重复100次并平展成一个数组
      errorMessage.value = ''; // 清除错误信息
    } else {
      products.value = [];
      errorMessage.value = '没找到相关的宝贝...'; // 设置错误信息
    }
  } catch (error) {
    console.error('Error fetching stores:', error);
    errorMessage.value = '没找到相关的宝贝...'; // 设置错误信息
  } finally {
    if(response.data.length>12){
      loading.value = false;  // 数据获取完毕后关闭缓冲页面
    }
    
  }
};

const goToProductDetail = (productId: string) => {
  // console.log('Selected Store ID:', productId);
  localStorage.setItem('productIdOfDetail', productId);  // 存储 productId
  console.log('跳转至 /productdetail');
  router.push('/productdetail');  // 跳转到商品详情页
};

//回车搜索
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    fetchStores(keyword.value, type.value); // 按下回车键时执行搜索
  }
};

onMounted(() => {
  // const keyword = localStorage.getItem('keyword') || '';
  // const type = localStorage.getItem('searchType') || '0';
  // fetchStores(keyword, type);
  fetchStores(keyword.value, type.value);
 
});
</script>

<template>
  <Navbar />
  <!-- <div v-if="loading" class="loading-overlay">
    <div class="loading-spinner"></div>
    <p>搜索中，请稍候...</p>
  </div> -->
  <div class="totalpage"
  
  
  
  >
  
  
    <!-- <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div> -->
    <!-- 错误信息和图片 -->
    <div v-if="errorMessage" class="error-message-container">
      <div class="error-container">
        <img src="@/assets/wy/cry.jpeg" alt="Cry" class="error-image" />
        <div class="error-text">{{ errorMessage }}</div>
      </div>
      
    </div>
    <div v-else class="product-display">
      <div v-for="product in products" :key="product.productId" class="product-item" @click="goToProductDetail(product.productId)">
        <img :src="product.productPics.length ? product.productPics[0].imageUrl : ''" :alt="product.productName" class="product-image" />
        <div class="product-info">
          <p class="product-price">
            <span class="special-price">价格</span> ¥{{ product.productPrice }}
          </p>
          <h2 class="product-name">【{{ product.productName }}】
            <!-- {{ product.description }} -->
          </h2>
        </div>
      </div>
      <!-- 底部显示“已经到底了” -->
      <div v-if="!loading &&products.length > 0" class="end-of-list">
        <p>已经到底啦...</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.totalpage{
  background-image: url('@/assets/wy/background.jpg'); 
  background-size: cover; 
  background-position: center; 
  height: 100%; 
  overflow-x: hidden;
  font-family: 'Regular';

}
.product-display {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  padding-top: 20px;
  padding-left: 100px;
  padding-right: 100px;
  background-image: url('@/assets/wy/background.jpg'); background-size: cover; background-position: center; height: 100%; overflow-x: hidden;
  align-items: stretch; /* 使每个商品容器拉伸到相同高度 */
}

.product-item {
  width: calc(25% - 20px); /* 每行显示四个商品，减去间隙 */
  height:300px;
  padding: 20px;
  border: 1px solid #e7e7e7;
  border-radius: 10px;
  background-color: #fff;
  display: flex;
  flex-direction: column;
  align-items: center;
  cursor: pointer; /* 使鼠标在悬停时变成手型指针 */
  transition: border-color 0.3s ease; /* 添加平滑的边框颜色变化效果 */
}

.product-item:hover {
  border-color: #a61b29; /* 悬停时的边框颜色 */
}

.product-image {
  width: 258px; /* 固定宽度 */
  height: 170px; /* 固定高度 */
  object-fit: cover; /* 保持图片比例并裁剪超出部分 */
  /* object-fit: cover; */
  border-radius: 10px;
  margin-bottom: 10px;
}

.product-info {
  text-align: center;
  /* padding-bottom: 10px; */
}

.product-name {
  font-size: 16px;
  color: #333;
  margin-bottom: 10px;
}

.product-price {
  font-size: 18px;
  color: #a61b29;
  margin-bottom: 10px;
}

.special-price {
  background-color: #a61b29;
  color: #fff;
  padding: 2px 5px;
  border-radius: 5px;
}

.end-of-list {
  width: 100%;  /* 确保容器占满整个宽度 */
  text-align: center;
  margin: 20px 0;
  font-size: 24px;
  color: #a61b29;
  display: flex;
  justify-content: center; /* 水平居中 */
  align-items: center; /* 垂直居中 */
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
