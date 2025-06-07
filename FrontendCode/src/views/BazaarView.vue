<template>
  <Navbar />
  <div class="market-page" >
    <h2 class="market-title"> 近期市集 </h2>
    <button class="carousel-button left" @click="prevSlide">
      <img src="@/assets/wy/leftslide.png" alt="左翻按钮" class="icon"/>
    </button>
    <div class="carousel-container">
      
      <div class="carousel-slide" :style="slideStyle">
        <div v-for="(market, index) in markets" :key="index" class="carousel-item">
          <div class="image-container">
            <img :src="market.image.imageUrl" class="carousel-image" @click="goToMerchantShowcase(market.marketId,market.theme,market.image.imageUrl,market.detail)" />
            <div class="overlay">
              <span class="end-time">至{{ formatDate(market.endTime) }}</span>
            </div>
          </div>
          <div class="market-info">
            <h3>{{ market.theme }}</h3>
            <p style="font-family: 'Regular';">{{ market.detail }}</p>
          </div>
        </div>
      </div>
    </div>
    <button class="carousel-button right" @click="nextSlide">
      <img src="@/assets/wy/rightslide.png" alt="右翻按钮" class="icon"/>
    </button>
  </div>
</template>

<script setup lang="ts">
import Navbar from '../components/Navbar.vue';
import { ref, onMounted, computed } from 'vue';
import router from '@/router';
import axiosInstance from '../components/axios';

const markets = ref([]);
const currentIndex = ref(0);

// 获取市集数据
const fetchMarkets = async () => {
  try {
    const response = await axiosInstance.get('/Administrator/GetAllMarket');
    markets.value = response.data;
    console.log("市集数据",markets.value);
  } catch (error) {
    console.error('获取市集数据时出错:', error);
  }
};

onMounted(() => {
  fetchMarkets();

  const savedIndex = localStorage.getItem('currentMarketIndex');
  if (savedIndex !== null) {
    currentIndex.value = parseInt(savedIndex, 10); // 恢复保存的索引
  }
});

// 上一页
const prevSlide = () => {
  currentIndex.value = (currentIndex.value - 2 + markets.value.length) % markets.value.length;
};

// 下一页
const nextSlide = () => {
  currentIndex.value = (currentIndex.value + 2) % markets.value.length;
};

// 跳转到市集商品页面
const goToMerchantShowcase = (marketId,theme,posterImg,detail) => {
  localStorage.setItem('selectedMarketId', marketId);
  localStorage.setItem('selectedMarkettheme', theme);
  localStorage.setItem('selectedMarketposterImg', posterImg);
  localStorage.setItem('selectedMarketdetail', detail);
  localStorage.setItem('currentMarketIndex', currentIndex.value); // 保存当前索引
  router.push('/bazaarmerchandise');
};

// 滑动样式
const slideStyle = computed(() => ({
  width: `${markets.value.length * 50}%`,
  transform: `translateX(-${(currentIndex.value / markets.value.length) * 100}%)`,
}));

// 日期格式化
const formatDate = (dateString) => {
  const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
  return new Date(dateString).toLocaleDateString('zh-CN', options);
};
</script>

<style scoped>
/* 市集页面背景颜色和整体布局 */
.market-page {
  /* background-color: #bdaead; */
  background-image: url('@/assets/wy/background.jpg'); background-size: cover; background-position: center; height: 100%; overflow-x: hidden;
  padding: 20px;
  text-align: center;
  height: 100%;
}

h2 {
  font-size: 28px;
  margin-top: 20px;
  margin-bottom: 20px;
}
.market-title{
  /* font-family: 'Long Cang', cursive; */
  /* font-family: 'Noto Serif SC', serif; */
  font-family: 'Regular';
  /* font-weight:bold; */
  font-size: 35px; /* 可根据需要调整字体大小 */
  color: #333; /* 可根据需要调整颜色 */
}

/* 轮播容器样式 */
.carousel-container {
  position: relative;
  width: 100%;
  overflow: hidden;
  max-width: 1200px;
  margin: 0 auto;
}

.carousel-slide {
  display: flex;
  transition: transform 0.5s ease;
}

/* 每个市集项的样式 */
.carousel-item {
  width: 50%; /* 每个市集占50%宽度 */
  padding: 20px;
  box-sizing: border-box;
  text-align: left;
}

.image-container {
  position: relative;
  width: 100%;
  height: 350px;
  /* margin-bottom: 10px; */
}

.carousel-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  cursor: pointer;
}

/* 海报右上角的日期覆盖层样式 */
.overlay {
  position: absolute;
  top: 0;
  right: 0;
  background-color: #a61b29;
  color: white;
  padding: 5px 10px;
  font-size: 14px;
}

.market-info {
  background-color: white;
  padding: 10px;
  /* border-radius: 5px; */
  /* box-shadow: 0px 0px 5px rgba(0,0,0,0.1); 添加轻微的阴影效果 */
}

.market-info h3 {
  margin: 0;
  font-size: 18px;
  color: #333;
}

.market-info p {
  margin: 5px 0 0;
  font-size: 14px;
  color: #666;
}

/* 左右翻页按钮样式 */
.carousel-button {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background-color: transparent;
  border: none;
  cursor: pointer;
  z-index: 10;
}

.carousel-button.left {
  left: 25px;
}

.carousel-button.right {
  right: 25px;
}

.icon{
  height: 110px;
}
</style>
