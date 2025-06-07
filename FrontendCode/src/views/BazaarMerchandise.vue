<template>
  <Navbar />
  <div class="market-page" style="font-family: 'Regular';">
    <!-- <div class="left-panel">
      <h2 class="market-title">
        <span class="market-back" @click="goBackToMarketList">市集</span>
        > {{ marketTheme.substring(0, 4) }}
      </h2>
      <div class="left-panel-bottom">
        <img src="@/assets/wy/market1.png" alt="市场图片1" class="market-image" style="position: absolute;top: 50px;"/>
        <p class="market-middle-text" >市集·{{ marketTheme }}</p>
        <img src="@/assets/wy/market2.png" alt="市场图片2" class="market-image" style="position: absolute;bottom: 50px;"/>
      </div>
    </div> -->
    
    <div class="right-panel">
      <div class="market-header">
        <img :src="marketPoster" alt="市场海报" class="market-poster" />
        <div class="market-info">
          <h2 class="market-title" style="font-family: 'Black';">
            <span class="market-back" @click="goBackToMarketList"></span>{{ marketTheme}}
          </h2>
          <p class="market-detail">{{ marketDetail }}</p>
        </div>
        
      </div>
      <div class="product-display">
        <div v-for="product in products" :key="product.id" class="product-item" @click="goToProductDetail(product.id)">
          <img :src="product.productPics.length ? product.productPics[0].imageUrl : ''"  :alt="product.name" class="product-image" />
          <div class="product-info">
            <p class="product-price">
              <span class="special-price">特卖价</span> ¥{{ Number(product.price).toFixed(1) }}
              <span class="original-price">¥{{Number(product.originalPrice).toFixed(1) }}</span>
              <span class="discount">{{ product.discount }}折</span>
            </p>
            <h2 class="product-name">【{{ product.tag }}】{{ product.name }} {{ product.description }}</h2>
          </div>
        </div>
        
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import Navbar from '../components/Navbar.vue';
import { ref, onMounted } from 'vue';
import axiosInstance from '../components/axios';
import { useRouter } from 'vue-router';

const router = useRouter();
const products = ref([]);
const marketTheme = localStorage.getItem('selectedMarkettheme');
const marketPoster = localStorage.getItem('selectedMarketposterImg');
const marketDetail = localStorage.getItem('selectedMarketdetail');

const fetchProducts = async (marketId) => {
  try {
    const response = await axiosInstance.post('/Market/GetProductsByMarket', {
      marketId: marketId
    });

    const data = response.data;
    products.value = data.map(item => ({
      id: item.productId,
      name: item.productName,
      tag: item.productTag,
      price: (item.productPrice * item.discount).toFixed(2),
      originalPrice: item.productPrice.toFixed(2),
      discount: (item.discount * 10).toFixed(1),
      description: item.description,
      productPics: item.productPics
    }));
    // products.value = Array(100).fill(products.value).flat(); // 将数据重复100次并平展成一个数组
  } catch (error) {
    console.error('Error fetching products:', error);
  }
};

const goToProductDetail = (productId: string) => {
  localStorage.setItem('productIdOfDetail', productId);
  router.push('/productdetail');
};
const goBackToMarketList = () => {
  router.push('/bazaar');
};
onMounted(() => {
  const marketId = localStorage.getItem('selectedMarketId');
  
  fetchProducts(marketId);
});
</script>

<style scoped>
.market-page {
  display: flex;
  /* background-color: #f7f4ed; */
  background-image: url('@/assets/wy/background.jpg'); 
  background-size: cover; 
  background-position: center; 
  height: 100%; 
  /* overflow-x: hidden; */
  overflow: hidden;
}

.left-panel {
  width: 14%;
  /* height: 100%; */
  /* background-color: #e0f7ff; */
  /* padding: 20px; */
  text-align: center;
  /* display: flex;  */
}
.left-panel-bottom {
  background-color: #bdaead;
  margin-left: 30px;
  width: 80%;
  height: 600px;
  display: flex;
  flex-direction: column;
  justify-content: center; /* 使文字垂直居中 */
  align-items: center; /* 使内容水平居中 */
  position: relative;
}

.market-title {
  font-size: 25px;
  color: #333;
  margin-bottom: 20px;
  margin-left: -50px;
  margin-top: 20px;
  /* cursor: pointer; */
}
.market-back:hover {
  color: #a61b29;
}

.market-image {
  width: 40%;
  /* position: absolute; */
  /* bottom: 50px; 固定距离底部边框50px */
}

.market-middle-text {
  font-size: 29px;
  color: #333;
  writing-mode: vertical-rl;
  text-orientation: upright;
  text-align: center;
  margin: 0; /* 移除外边距，确保居中 */
}

.right-panel {
  flex-grow: 1; /* 确保右侧面板占据剩余空间 */
  padding-top: 30px;
  overflow-y: scroll; /* 允许垂直滚动 */
  box-sizing: border-box; /* 包括内边距和边框在内的宽度 */
  padding-left: 100px;  /* 添加左侧间距 */
  padding-right: 100px; /* 添加右侧间距 */
}


.market-header {
  display: flex;
  background-color: #bdaead;
  /* align-items: center; */
  margin-bottom: 20px;
  box-shadow: 4px 4px 20px rgba(0, 0, 0, 0.1);

}

.market-poster {
  /* width: 500px; */
  height: 300px;
  /* padding-right: 20px; */
  /* padding: 15px; */
  /* margin-right: 20px; */
  /* border-radius: 10px; */
}
.market-info {
  display: flex;
  flex-direction: column; /* 使标题和描述垂直排列 */
  justify-content: flex-start;
  margin-top: 30px;
  padding-left: 30px;
  padding-right: 30px;
}
.market-detail {
  font-size: 20px;
  color: #555;
  margin-top: 20px;
}

.product-display {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.product-item {
  width: calc(25% - 20px);
  padding: 20px;
  border: 1px solid #e7e7e7;
  border-radius: 10px;
  background-color: #fff;
  display: flex;
  flex-direction: column;
  align-items: center;
  cursor: pointer;
  transition: border-color 0.3s ease;
}

.product-item:hover {
  border-color: #a61b29;
}

.product-image {
  width: 100%;
  height: 200px;
  object-fit: cover;
  border-radius: 10px;
  margin-bottom: 10px;
}

.product-info {
  text-align: center;
}

.product-name {
  font-size: 16px;
  color: #333;
  margin-bottom: 10px;
}

.product-price {
  font-size: 18px;
  color: #c21f30;
  margin-bottom: 10px;
}

.special-price {
  background-color: #a61b29;
  color: #fff;
  padding: 2px 5px;
  border-radius: 5px;
}

.original-price {
  text-decoration: line-through;
  margin-left: 5px;
  color: #999;
}

.discount {
  margin-left: 5px;
  color: #c21f30;
}
</style>
