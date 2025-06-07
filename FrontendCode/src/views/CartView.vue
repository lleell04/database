<!-- 买家用户的收藏夹页面 -->
<template>
    <Navbar />
    <div class="main-container">
        <aside class="sidebar-category">
            <div role="tree" class="sidebar">
                <div
                v-for="category in categories"
                :key="category.id"
                :id="'category-' + category.id"
                role="treeitem"
                tabindex="0"
                :class="{ selected: selectedCategory === category.id }"
                class="category"
                @click="filter(category)"
                >
                {{ category.name }}
                </div>
            </div>
        </aside>

        <Loading v-show="isLoading" />

        <div v-show="!isLoading" class="display">
            <div v-if="selectedCategory==1" >
              <div v-if="paginatedProducts.length!==0">
                <div class="display-items1">
                    <div 
                      v-for="product in paginatedProducts" 
                      :key="product.productId" 
                      class="product-item"
                      @click="handleProductClick(product.productId)"
                    >
                      <img :src="product.productPic.imageUrl" :alt="product.productId" class="product-image" />
                      <h2>{{ product.productName }}</h2>
                      <p>价格: ¥{{ product.productPrice }}</p>
                    </div>
                </div>
                <div class="pagination">
                    <button @click="productPageChange(currentPage1 - 1)" :disabled="currentPage1 === 1">上一页</button>
                    <span>第 {{ currentPage1 }} 页 / 共 {{ productsPages }} 页</span>
                    <button @click="productPageChange(currentPage1 + 1)" :disabled="currentPage1 === productsPages">下一页</button>
                </div>
              </div>
              <div v-else>
                <span style="font-family: Arial, sans-serif; font-size: 20px; display: block; margin-bottom: 13px;">
                暂无收藏商品
                </span>
              </div>
            </div>
            <div v-else>
                <div class="display-items2">
                  <div v-if="paginatedStores.length!==0">
                    <div 
                      v-for="store in paginatedStores" 
                      :key="store.storeId" 
                      class="store-container"
                      @click="handleStoreClick(store.storeId)"
                    >
                      <div class="store-content">
                        <div class="store-header">
                          <div class="store-info">
                            <img :src="'data:image/png;base64,' + store.storePic" alt="Store Image" class="store-image" />
                            <!-- <img :src="store.storePic" alt="Store Image" class="store-image" /> -->
                            <h2 class="store-name">{{ store.storeName }}</h2>
                            <p class="store-rating">评分: {{ store.storeScore }}</p>
                          </div>
                        </div>
                        <div class="store-products">
                          <div v-for="product in store.products" :key="product.productId" class="product-item">
                            <!-- <img :src="'data:image/png;base64,' + product.productPic" alt="Product Image" class="product-image" /> -->
                            <img :src="product.productPic.imageUrl" alt="Product Image" class="product-image" />
                            <p class="product-price">{{ product.productName }}</p>
                            <p class="product-price">¥{{ product.productPrice }}</p>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="pagination">
                        <button @click="storePageChange(currentPage2 - 1)" :disabled="currentPage2 === 1">上一页</button>
                        <span>第 {{ currentPage2 }} 页 / 共 {{ storesPages }} 页</span>
                        <button @click="storePageChange(currentPage2 + 1)" :disabled="currentPage2 === storesPages">下一页</button>
                    </div>
                  </div>
                  <div v-else>
                    <span style="font-family: Arial, sans-serif; font-size: 20px; display: block; margin-bottom: 13px;">
                    暂无收藏店铺
                    </span>
                  </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import Navbar from '../components/Navbar.vue';
import Loading from '../views/templates/4.vue';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

const router = useRouter();
const userId =localStorage.getItem('userId');
const isLoading = ref(false);

const categories = ref([
    { id: 1, name: '收藏商品' },
    { id: 2, name: '收藏店铺' },
]);

const selectedCategory = ref(1);
const filter = (category) => {
    selectedCategory.value = category.id;
    console.log(` ${category.name} 被点击`);
    if(selectedCategory==1){
        fetchProducts();
    }
    else{
        fetchStores();
    }
}

//////商品相关
const Products = reactive([]);
const message01 = ref('');
const fetchProducts = async () => {
  try {
    const response = await axiosInstance.post('/Favourite/GetFavoriteProducts', {
      "userId": userId
    });

    response.data.forEach(product => {
      if(!product.saleOrNot){
        //product.productPic = `data:image/png;base64,${product.productPic}`;
        Products.push(product);
      }
    });
    message01.value = '已获取收藏商品数据';
    isLoading.value=false;

  } catch (error) {
    if (error.response) {
      message01.value = error.response.data;
    } else {
      message01.value = '获取数据失败';
    }
    // isLoading.value=false;
    // if(response.status!=404)
    //  ElMessage.info("获取数据失败，请稍后再试");
  }
  console.log(message01.value);
};

// 设置表格页面大小及当前页数
const pageSize1 = ref(9);
const currentPage1 = ref(1);

const totalProducts = computed(() => Products.length);
const productsPages = computed(() => Math.ceil(totalProducts.value / pageSize1.value));

// 处理并分页后的商品数据
const paginatedProducts = computed(() => {
  const start = (currentPage1.value - 1) * pageSize1.value;
  const end = start + pageSize1.value;
  return Products.slice(start, end);
});

// 切换分页页面
const productPageChange = (page) => {
    if (page >= 1 && page <= productsPages.value) {
        currentPage1.value = page;
    }
};

const handleProductClick = (productId) => {
    localStorage.setItem('productIdOfDetail',productId);
    router.push('/productdetail');
};


///// 店铺相关
const Stores = reactive([]);
const message02 = ref('');
const fetchStores = async () => {
  isLoading.value=true;
  try {
    const response = await axiosInstance.post('/Favourite/GetFavoriteStores', {
      "userId": userId
    });
    // Stores.splice(0, Stores.length, ...response.data);
    response.data.forEach(store => {
      Stores.push(store);
    });
    isLoading.value=false;
    message02.value = '已获取收藏店铺数据';
    console.log('收藏店铺数据：'+Stores.values);
  } catch (error) {
    if (error.response) {
      message02.value = error.response.data;
    } else {
      message02.value = '获取数据失败';
    }
    isLoading.value=false;
    // ElMessage.info("获取数据失败，请稍后再试");

  }
  console.log(message02.value);
};

// 设置表格页面大小及当前页数
const pageSize2 = ref(2);
const currentPage2 = ref(1);

const totalStores = computed(() => Stores.length);
const storesPages = computed(() => Math.ceil(totalStores.value / pageSize2.value));

// 处理并分页后的商品数据
const paginatedStores = computed(() => {
  const start = (currentPage2.value - 1) * pageSize2.value;
  const end = start + pageSize2.value;
  return Stores.slice(start, end);
});

// 切换页面
const storePageChange = (page) => {
    if (page >= 1 && page <= storesPages.value) {
        currentPage2.value = page;
    }
};

const handleStoreClick = (storeId) => {
    localStorage.setItem('storeIdOfDetail',storeId);
    router.push('/shopdetail');
};



onMounted(() => {
    fetchProducts();
});

</script>

<style scoped>
  
.main-container{
  /* align-items: center;
  background-color: #f4f6f8;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  min-height: 100vh;
  padding-bottom: 48px;
  position: relative; */
  
  display: flex;
  height: 100vh;
  /* background-color: #f8f8f8; */

}

.main-content{
  margin-top: 20px;
  width:100%;
  justify-content: center;
  box-sizing: border-box;
  align-items: flex-start;
  display: flex;
  justify-content: space-between;
}

.sidebar-category{
  /* background-color: #fff;
  border-radius: 16px;
  box-sizing: border-box;
  min-height: 70vh;
  margin-top: 16px;
  padding: 8px;
  position: sticky;
  top: 10px;
  width: 20%; */
  flex: 0 0 200px;
  background-color: #fff;
  padding: 20px;
  box-shadow: 2px 0 5px rgba(0,0,0,0.1);
}

.display{
  /* background-color: #fff;
  border-radius: 16px; */
  box-sizing: border-box;
  margin-top: 16px;
  margin-left: 16px;
  padding: 24px 0 0;
  width: 100%;
  min-height: 70vh;
}

.category {
  font-size: 16px;
  padding: 8px 12px;
  margin-bottom: 8px;
  cursor: pointer;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.category.selected {
  background-color: #a61b29;
  font-weight: bold;
  color:#ffffff;
}

.display-items1{
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  min-height: 480px;
}

.product-item {
  cursor: pointer;
  flex: 0 0 23%;
  background-color: #fff;
  padding: 10px;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
  transition: transform 0.3s, box-shadow 0.3s;
  height: 250px;
}

.product-image {
  max-height: 110px;
  width: 90%;
  object-fit: cover;
  border-radius: 5px;
  margin-bottom: 10px;
  margin-left: 10px;
  margin-right: 10px;
}

.product-item h2 {
  font-size: 18px;
  margin: 0 0 5px;
}

.product-item p {
  font-size: 16px;
  margin: 0 0 5px;
}

.pagination{
  margin-top: 20px; 
}

.display-items2{
  flex-wrap: wrap;
  gap: 20px;
  min-height: 480px;
}

.store-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding: 20px;
  align-items: center;
}

.store-container {
  border: 1px solid #e7e7e7;
  border-radius: 10px;
  padding: 20px;
  width: 97%;
  margin-right: 10px;
  margin-bottom: 10px;
  height: 240px;
  cursor: pointer;
}

.store-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  width: 100%;
}

.store-header {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
  margin-bottom: 20px;
}

.store-info {
  display: flex;
  flex-direction: column;
  margin: 15px;
}

.store-image{
  width: 120px;
  height: 120px;
  border-radius: 120px;
}

.store-name {
  font-size: 24px;
  font-weight: bold;
}

.store-rating {
  font-size: 18px;
  color: #888;
}

.store-products {
  display: flex;
  gap: 10px;
  margin-right: 50px;
  flex-wrap: wrap;
}

.product-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 150px;
  height: 210px;
}

.product-image {
  width: 150px;
  height: 150px;
  object-fit: cover;
  border-radius: 5px;
}

.product-price {
  font-size: 16px;
  color: #333;
  margin-top: 5px;
}

</style>