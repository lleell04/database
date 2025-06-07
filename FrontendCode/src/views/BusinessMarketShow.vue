<template>
  <div class="CommodityShow">
    <div class="SearchContainer">
      <el-input v-model="searchTopic" placeholder="请输入市集主题" style="display: inline-block;"></el-input>
      <el-button type="primary" @click="filterMarkets">搜索</el-button>
    </div>
    <!-- 市集表格 -->
    <div class="TableContainer">
      <el-table :data="currentPageData" class="CommodityTable" height="760">
        <el-table-column type="index" />
        <el-table-column prop="marketId" label="市集ID"></el-table-column>
        <el-table-column prop="theme" label="市集主题"></el-table-column>
        <el-table-column prop="startTime" label="开始时间"></el-table-column>
        <el-table-column prop="endTime" label="结束时间"></el-table-column>
        <el-table-column label="操作">
          <template #default="scope">
            <el-button-group>
              <el-button size="mini" type="primary" icon="check" @click="handleCheck(scope.row)">查看</el-button>
              <el-button
                size="mini"
                :type="scope.row.isStoreParticipating ? 'success' : 'primary'"
                :icon="scope.row.isStoreParticipating ? 'check' : 'circle'"
                @click="toggleParticipation(scope.row)"
              >
                {{ scope.row.isStoreParticipating ? '已参与' : '未参与' }}
              </el-button>
            </el-button-group>
          </template>
        </el-table-column>
      </el-table>
    </div>  
    
    <!-- 市集详情页面 -->
    <div v-if="dialogVisible" class="SettingPopUp">
      <div v-if="currentMarket">
        <div class="container-block">
          <img src="@/assets/mmy/blue_background.jpg">
          <div class="inner-block">
            <div class="slider-top-right">
              <div class="picture-and-text">
                <transition class="animate__animated animate__fadeInDown">
                  <img class="picture" :src="currentMarket.posterImg" alt="MarketPoster">
                </transition>
                <transition class="animate__animated animate__fadeInUp">
                  <div class="text">
                    <span class="close" @click="dialogVisible = false">&times;</span>
                    <br>
                    <h2> {{ currentMarket.theme }}</h2>
                    <p style="text-align: end;font-size: 15px;	font-family: 'Noto Serif SC', serif;margin-bottom:3px;"> {{ currentMarket.startTime }}-{{ currentMarket.endTime }}</p>
                    <p style="font-size: 16px;">{{ currentMarket.detail }}</p>
                  </div>
                </transition>
              </div>
            </div>
          </div>
        </div> 
      </div>
    </div>

    <!-- 翻页 -->
    <div class="paginationContainer">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="totalMarkets"
        layout="total, prev, pager, next, jumper"
        @current-change="handlePageChange"
      ></el-pagination>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import axiosInstance from '../components/axios';
import imageA from '@/assets/setting.svg';
import 'animate.css';
import { ElSelect, ElOption, ElButton ,ElMessage} from 'element-plus';
import 'element-plus/dist/index.css';

export default {
  components: {
    ElSelect,
    ElOption, 
    ElButton
  },
  setup() {
    const value = ref('');
    const dialogVisible = ref(false);
    const currentMarket = ref(null);
    const searchTopic = ref('');

    const markets = ref([]);
    const pageSize = 20;
    const currentPage = ref(1);
    const totalMarkets = computed(() => markets.value.length);

    const currentPageData = computed(() => {
      const start = (currentPage.value - 1) * pageSize;
      const end = Math.min(start + pageSize, totalMarkets.value);
      return markets.value.slice(start, end);
    });

    const handleCheck = (row) => {
      currentMarket.value = row;
      dialogVisible.value = true;
    };

    
    const toggleParticipation = async (row) => {
      const storeId = localStorage.getItem('userId'); // 替换为实际的 storeId
      console.log('Stored User ID:', localStorage.getItem('userId'));
      const inOrOut = !row.isStoreParticipating; // 反转参与状态

       // 检查当前时间是否在市集的开始时间和结束时间范围内
        const currentTime = new Date();
        const startTime = new Date(row.startTime);
        const endTime = new Date(row.endTime);
     
        if (currentTime < startTime || currentTime > endTime) {
          ElMessage({
            message: '市集不在有效时间范围内，无法改变参与状态。',
            type: 'warning'
          });
          return;
        }


      try {
        await axiosInstance.put('/StoreViewMarket/UpdateMarketStoreParticipation', {
          marketId: row.marketId,
          storeId: storeId,
          inOrOut: inOrOut,
        });

        // 更新本地状态
        row.isStoreParticipating = inOrOut;
      } catch (error) {
        console.error('更新市场参与状态失败:', error);
      }
      };

    const handlePageChange = (page) => {
      currentPage.value = page;
    };

//获取全部市集
const fetchMarkets = async () => {
  const storeId = localStorage.getItem('userId'); 

  try {
    const response = await axiosInstance.get('/StoreViewMarket/GetMarketsByStoreId', {
      params: {
        storeId: storeId,
      },
    });

    if (Array.isArray(response.data)) {
      const processedMarkets = response.data.map(market => {
        // 确保字段存在并转换正确
        const startTime = market.startTime ? new Date(market.startTime).toLocaleString() : 'N/A';
        const endTime = market.endTime ? new Date(market.endTime).toLocaleString() : 'N/A';

        // 获取 postimg 数组中的第一个图片 URL
        const posterImg = market.posterImg && market.posterImg.length > 0 ? market.posterImg[0].imageUrl : imageA;

        return {
          marketId: market.marketId || 'N/A',
          theme: market.theme || 'Unknown',
          startTime: startTime,
          endTime: endTime,
          detail: market.detail || 'No details available',
          posterImg: posterImg,
          isStoreParticipating: market.isStoreParticipating !== undefined ? market.isStoreParticipating : false,
        };
      });
      markets.value = processedMarkets; 
    } else {
      console.error('Unexpected response format:', response.data);
    }
  } catch (error) {
    console.error('Error fetching markets:', error);
  }
};
//根据市集主题搜索市集
const filterMarkets = () => {
  if (searchTopic.value.trim() !== '') {
    fetchMarketByTheme(searchTopic.value.trim());
  } else {
    fetchMarkets();
  }
};
const fetchMarketByTheme = async (theme) => {
  const storeId = localStorage.getItem('userId'); 
  try {
    const response = await axiosInstance.get('/StoreViewMarket/searchMarket', {
      params: {
        storeId: storeId,
        theme: theme || '', 
      }
    });

    if (Array.isArray(response.data)) {
      const processedMarkets = response.data.map(market => {
        const startTime = market.startTime ? new Date(market.startTime).toLocaleString() : 'N/A';
        const endTime = market.endTime ? new Date(market.endTime).toLocaleString() : 'N/A';

        // 获取 postimg 数组中的第一个图片 URL
        const posterImg = market.posterImg && market.posterImg.length > 0 ? market.posterImg[0].imageUrl : imageA;

        return {
          marketId: market.marketId || 'N/A',
          theme: market.theme || 'Unknown',
          startTime: startTime,
          endTime: endTime,
          detail: market.detail || 'No details available',
          posterImg: posterImg,
          isStoreParticipating: market.isStoreParticipating !== undefined ? market.isStoreParticipating : false,
        };
      });

      markets.value = processedMarkets; 
    } else {
      console.error('Unexpected response format:', response.data);
    }
  } catch (error) {
    markets.value = [];
    console.error('通过市集主题获取市集数据失败:', error);
  }
};

onMounted(() => {
  fetchMarkets();
});

return {
  value,
  dialogVisible,
  currentMarket,
  searchTopic,
  markets,
  pageSize,
  currentPage,
  totalMarkets,
  currentPageData,
  handleCheck,
  toggleParticipation,
  handlePageChange,
  filterMarkets,
};
},
};
</script>
  
<style scoped>
  .CommodityShow {
    position: fixed;
    top: 10vh;
    left: 150px; 
    right: 0;
    bottom: 0;
    background-color: #DFCDC7  ;
  }
  
  .TableContainer {
    margin: 10px;
    margin-top: 0; 
  }
  
  .SearchContainer {
    display: flex; 
    text-align: end;
    align-items: center;
    height: 60px;
    margin-right: 10px;
    /* background-color: rgb(164, 197, 181); */
  }
  
  .SettingPopUp {
  position: fixed;
  z-index: 1;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0,0,0,0.4);
  display: flex;
  justify-content: center;
  align-items: center;
}
.paginationContainer {
  display: inline-block;
}

.SettingContent {
  background-color: #fefefe;
  display: inline-block;
  /* position: relative;
  width: 80%; 
  height: 60%; 
  background: #fff; 
  border-radius: 10px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3); 
  overflow: hidden; 
  display: flex;
  flex-direction: column;
  background-color: rgb(109,99,59); */
}

.close {
  color: #aaa;
  float: right;
  font-size: 28px;
  font-weight: bold;
}
 
.close:hover,
.close:focus {
  color: black;
  text-decoration: none;
  cursor: pointer;
}

.el-button--primary {
    background-color: #a13232;
    border-color: #a13232;
}

.el-button--primary:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}
  

.animate__animated.animate__fadeInUp{
	--animate-duration: 1.5s;
}
.animate__animated.animate__fadeInDown{
	--animate-duration: 1.5s;
}
.container-block {
    overflow: hidden;
    border-radius: 15px;
	color: #fff;
	display: inline-block;
	margin: 2rem auto;
	max-width: 60%;
	position: relative;	
	&::before {
		/* 特别修改1 */
        /* 企图用覆盖层的颜色改变原本背景色,并保留原本图案*/
		/* background-color: rgba(59, 69, 109,0.3); */
        /* background-color: rgba(255, 255, 255,0.3); */
        background-color: rgba(250, 13, 40, 0.3);
		bottom: 0;
		content: '';
		display: block;
		position: absolute;
		top: 0;
		width: 100%;
	}
	&:hover {
		.inner-block:before,
		.slider-top-right:after {
			height: 100%;
		}
		.inner-block:after,
		.slider-top-right:before {
			width: 100%;
		}
	}
	img {
		display: block;
		max-width: 100%;
	}
}

.slider-top-right:before,
.inner-block:after {
	height: 2px;
	transition: width .75s ease;
	width: 0%;
}

.slider-top-right:after,
.inner-block:before {
	height: 0%;
	transition: height .75s ease;
	width: 2px;
}

.inner-block:before,
.inner-block:after,
.slider-top-right:before,
.slider-top-right:after {
	background-color: #fff;
	content: '';
	display: block;
	position: absolute;
}

.inner-block {
	font-size: 2em;
	width: 90%;
	height: 90%;
	position: absolute;
	top: 0;
	left: 0;
	right: 0;
	bottom: 0;
	margin: auto;
	&:before {
		bottom: 0;
		left: 0;
	}
	&:after {
		bottom: 0;
		right: 0;
	}
}

.slider-top-right {
	position: relative;
	width: 100%;
	height: 100%;
	display: flex; /* 使用 Flexbox */
	justify-content: center; /* 水平居中 */
	align-items: center; /* 垂直居中 */
	&:before {
		top: 0;
		left: 0;
	}
	&:after {
		top: 0;
		right: 0;
	}
}
.picture-and-text{
	width: 90%;
	height:90%;
	display: flex; 
	flex-direction: row;
}
.picture{
	width:50%;
	border-radius: 20px;
}
.text{
	/* 华文宋 */
	width: 50%;
	font-family: 'Noto Serif SC', serif;
	font-size: 20px;
	padding: 10px 10px 20px 20px;
	overflow-y: auto; 
	max-height: 100%; /* 限制最大高度为父容器高度 */
	text-align: left;
	
}
/* 特别修改2 */
/* 自定义滚动条样式 */
.text::-webkit-scrollbar {
  width: 5px;
}

.text::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.text::-webkit-scrollbar-thumb {
  background: #5f697a;
  border-radius: 10px;
}
h2 {
    font-size: 1.5em;
    /* margin-block-start: 0.83em;
    margin-block-end: 0.83em;
    margin-inline-start: 0px;
    margin-inline-end: 0px; */
    font-weight: bold;
    unicode-bidi: isolate;
    color: #ecdada;
    text-align: center;
  }
  </style>