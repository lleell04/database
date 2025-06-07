<template>
  <nav class="navbar" style="font-family: 'Regular';">
    <!-- <div class="line top-line"></div> -->
    <div class="navbar-top">
      <div class="navbar-brand">
        <img src="@/assets/logo1.png" alt="Logo" class="logo" />
        <span style="font-size: 20px;margin-left: 10px;font-family: 'Regular';">瑕宝阁</span>
      </div>
      <div class="navbar-right">
        <div class="date-weather" >
          <span style="font-size: 17px;">{{ lunarDate }}</span>
          <span style="font-size: 17px; margin-left: 10px;">{{ weather }}</span>
        </div>
        <!-- 使用动态图片链接 -->
        <img :src="userProfilePhoto" alt="Profile Photo" class="profile-photo" @click="openModal"/>
        <!-- <img src="@/assets/wy/profilephoto.jpg" alt="Profile Photo" class="profile-photo" @click="openModal"/> -->
      </div>
    </div>
    <div class="line"></div>
    <div class="navbar-bottom">
      <ul class="navbar-menu">
        <li class="navbar-item" v-for="item in menuItems" :key="item.text"  @click="handleMenuClick(item.link)">
          <router-link :to="item.link" class="nav-link" active-class="active">{{ item.text }}</router-link>
        </li>
      </ul>
      <div class="navbar-search">
        <div class="search-container">
          <select v-model="searchType" class="search-type">
            <option value="product">商品</option>
            <option value="vendor">商家</option>
          </select>
          <input type="text" v-model="searchText" placeholder="搜索..." class="search-input"  @keyup.enter="handleSearch"
          />
          <button class="search-button" @click="handleSearch">
            <el-icon style="color: white;font-size: 15px;"><Search /></el-icon>
          </button>
        </div>
      </div>

    </div>
    <!-- <div class="line bottom-line"></div> -->
    <!-- <UserModal ref="userModal" /> -->
  </nav>
</template>

<script setup>
import { reactive, ref, onMounted, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
// import UserModal from '../views/UserCenter.vue';
import axios from 'axios';
import { getLunar } from 'chinese-lunar-calendar';
import { Search } from '@element-plus/icons-vue';
import axiosInstance from '../components/axios';
import chineseLunar from 'chinese-lunar';
// 引入UserModal组件
const userModal = ref(null);

// 打开模态框的函数
const openModal = () => {
  router.push(`/BuyerInformation`);
};

const router = useRouter();
const route = useRoute();

const searchType = ref('product'); // 搜索类型
const searchText = ref(''); // 搜索关键字
const currentDate = ref('');
const weather = ref('');
const lunarDate = ref('');
// 定义用于存储用户头像的 ref
const userProfilePhoto = ref('');
import defaultProfilePhoto from '@/assets/wy/profilephoto.jpg';



// const userId = 'U00000013';
const userId=localStorage.getItem('userId')|| 'we0';


const fetchUserProfilePhoto = async () => {
  const cachedUserId = localStorage.getItem('cachedUserId');
  const cachedProfilePhoto = localStorage.getItem(`userProfilePhoto_${userId}`);

  // 检查缓存的 userId 是否和当前 userId 相同
  if (cachedUserId === userId && cachedProfilePhoto) {
    userProfilePhoto.value = cachedProfilePhoto;
    return;  // 如果缓存的 userId 和当前用户一致，且有缓存的头像，直接使用
  }

  // 如果 userId 不同或者没有缓存头像，重新获取
  try {
    const response = await axiosInstance.post('/UserInfo/GetPhotoAndDescribtion', {
      id: userId,
    });
    const userInfo = response.data;

    if (userInfo && userInfo.photo && userInfo.photo.imageUrl) {
      userProfilePhoto.value = userInfo.photo.imageUrl;
      localStorage.setItem(`userProfilePhoto_${userId}`, userInfo.photo.imageUrl);  // 缓存头像
      localStorage.setItem('cachedUserId', userId);  // 记录当前 userId
    } else {
      userProfilePhoto.value = defaultProfilePhoto;
      localStorage.setItem(`userProfilePhoto_${userId}`, defaultProfilePhoto);  // 缓存默认头像
      localStorage.setItem('cachedUserId', userId);  // 记录当前 userId
    }
  } catch (error) {
    userProfilePhoto.value = defaultProfilePhoto;
    localStorage.setItem(`userProfilePhoto_${userId}`, defaultProfilePhoto);
    localStorage.setItem('cachedUserId', userId);  // 记录当前 userId
  }
};


const menuItems = reactive([
  { text: "首页", link: "/home" },
  { text: "商品", link: "/merchandise/1" },
  { text: "市集", link: "/bazaar" },
  { text: "论坛", link: "/forum" },
  { text: "收藏夹", link: "/cart" },
  { text: "订单中心", link: "/ordercentre" },
  // { text: "个人中心", link: "/personalcentre" },
]);
const handleMenuClick = (link) => {
    localStorage.removeItem('currentMarketIndex');
};


// 获取天气信息
const fetchWeather = async () => {
  const cachedUserId = localStorage.getItem('cachedUserId');
  const cachedWeather = localStorage.getItem(`weather_${userId}`);
  const lastWeatherFetchTime = localStorage.getItem('lastWeatherFetchTime');

  const currentTime = Date.now();
  const tenMinutes = 10 * 60 * 1000; // 10分钟的毫秒数
  console.log("currentTime - lastWeatherFetchTime",currentTime - lastWeatherFetchTime);

  // 检查缓存的 userId 是否和当前 userId 相同，并且检查距离上次获取天气的时间是否超过十分钟
  if (cachedUserId === userId && cachedWeather && lastWeatherFetchTime && (currentTime - lastWeatherFetchTime) < tenMinutes) {
    weather.value = cachedWeather;
    return;  // 如果缓存的 userId 和当前用户一致，且有缓存的天气信息，并且时间未超过十分钟，直接使用
  }

  // 如果 userId 不同、没有缓存天气，或距离上次获取天气超过十分钟，则重新获取
  try {
    const response = await axios.get('https://api.openweathermap.org/data/2.5/weather', {
      params: {
        q: 'Shanghai', 
        appid: '5f274af243427c3098128d11ecd97cd9', 
        lang: 'zh_cn' 
      }
    });
    
    weather.value = `${response.data.weather[0].description}`;
    localStorage.setItem(`weather_${userId}`, weather.value);  // 缓存天气信息
    localStorage.setItem('cachedUserId', userId);  // 记录当前 userId
    localStorage.setItem('lastWeatherFetchTime', currentTime);  // 记录获取天气的时间
    console.log("已重新获取天气");
  } catch (error) {
    console.error('获取天气信息失败', error);
  }
};


onMounted(() => {
  // 检查当前路由路径是否为 "/bazaarmerchandise"
  if (route.path === '/bazaarmerchandise') {
    document.querySelector('.navbar-menu li:nth-child(3) .nav-link').classList.add('active');
  }

  // 从本地存储中恢复搜索类型和关键字
  searchType.value = localStorage.getItem('searchType') === '1' ? 'vendor' : 'product';
  searchText.value = localStorage.getItem('keyword') || '';

  const year = new Date().getFullYear();
  const month = new Date().getMonth() + 1;
  const date = new Date().getDate();

  // 获取完整的农历信息
  const lunarInfo = getLunar(year, month, date);
  console.log('Lunar Info:', lunarInfo); // 打印查看lunarInfo对象的内容

  // 使用适当的属性组合显示完整的农历信息
  lunarDate.value = `${lunarInfo.lunarYear}${lunarInfo.dateStr}`;
  console.log('农历:', lunarDate.value);

  // fetchWeather();
  // fetchUserProfilePhoto(); // 组件挂载时获取用户头像

  const cachedUserId = localStorage.getItem('cachedUserId');
  const cachedProfilePhoto = localStorage.getItem(`userProfilePhoto_${userId}`);
  const cachedWeather = localStorage.getItem(`weather_${userId}`);

  // 如果缓存的 userId 和当前的 userId 一致，直接使用缓存
  if (cachedUserId === userId && cachedProfilePhoto) {
    userProfilePhoto.value = cachedProfilePhoto;
  } else {
    fetchUserProfilePhoto();  // 重新获取头像
  }

  fetchWeather();  // 重新获取天气
  

  /// 每小时更新日期和天气
  setInterval(() => {
    lunarDate.value = getLunar(year, month, date).dateStr;

    // 每10分钟检查是否需要重新获取天气
    fetchWeather();
  }, 600000); // 10分钟的间隔（600000毫秒）
  
});

watch(() => route.fullPath, () => {
  searchType.value = localStorage.getItem('searchType') === '1' ? 'vendor' : 'product';
  searchText.value = localStorage.getItem('keyword') || '';
});

const handleSearch = async () => {
  console.log('搜索类型:', searchType.value);
  console.log('搜索内容:', searchText.value);
  
  localStorage.setItem('searchType', searchType.value === 'product' ? '0' : '1');
  localStorage.setItem('keyword', searchText.value);

  if (searchType.value === 'product') {
    if (router.currentRoute.value.path !== '/searchproductshowcase') {
      router.push(`/searchproductshowcase`);
    } else {
      router.push('/home').then(() => router.push(`/searchproductshowcase`));
    }
  } else if (searchType.value === 'vendor') {
    if (router.currentRoute.value.path !== '/merchantshowcase') {
      router.push(`/merchantshowcase`);
    } else {
      router.push('/home').then(() => router.push(`/merchantshowcase`));
    }
  }
};
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

html, body {
  overflow-x: hidden; 
}

.navbar {
  width: 100%;
  background-color: #fff;
  position: relative;
}

.line {
  position: absolute;
  left: 0;
  width: 100vw;
  height: 1px;
  background-color: #e7e7e7;
}

.top-line {
  top: 0;
}

.bottom-line {
  bottom: 0;
}

.navbar-top,
.navbar-bottom {
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 20px;
}

.navbar-top {
  height: 60px;
  padding-left: 0px;
}

.navbar-bottom {
  width: 100%; /* 设置宽度为100% */
  margin: 0; /* 移除外边距 */
  padding-left: 300px; /* 导航栏文字 */
  background-color: white; /* 设置背景颜色为苋菜红 */
  display: flex;
  justify-content: flex-start;
  align-items: center;
  height: 60px;
}


.navbar-brand {
  display: flex;
  align-items: center;
  margin-left: 150px;
}

.logo {
  height: 40px;
  margin-right: 10px;
}

.navbar-right {
  display: flex;
  align-items: center;
  margin-right: 100px;
}

.date-weather {
  display: flex;
  align-items: center;
  margin-right: 20px;
}

.profile-photo {
  width: 50px;
  height: 50px;
  border-radius: 50%; /* 使头像为圆形 */
  margin-left: 50px;
  cursor: pointer; /* 鼠标悬停时变为可点击的状态 */
}


.icons i {
  margin-left: 10px;
  cursor: pointer;
}

.navbar-menu {
  margin-left: 20px;
  list-style: none;
  display: flex;
  gap: 70px;
  margin: 0;
  padding: 0;
}

.navbar-item {
  position: relative;
}

.navbar-item .nav-link {
  text-decoration: none;
  color: black; /* 字体颜色白色 */
  font-size: 22px;
  padding: 17px 8px;
  /* transition: background-color 0.3s ease, color 0.3s ease; */
  display: block;
  line-height: 20px; 
}


.navbar-item .nav-link.active,
.navbar-item .nav-link:hover {
  background-color: white; /* 鼠标悬停或点击时背景颜色 */
  border-bottom: 4px solid #a61b29;
}

.navbar-search {
  display: flex;
  align-items: center;
  padding-left: 80px; /* 增加左边距，使搜索框向右移动 */
}

.search-container {
  display: flex;
  align-items: center;
  /* background-color: #a61b29; */
  border: 1.5px solid #a61b29;
  padding-left: 5px;
  /* padding-right: -2px; */
  height: 38px;
  width: 240px;
  position: relative; /* 为了使按钮绝对定位 */
}

.search-type {
  height: 40px;
  background-color: transparent;
  border: none;
  padding: 0 4px;
  color: #a61b29;
  outline: none;
}

.search-input {
  height: 40px;
  border: none;
  outline: none;
  padding-left: 10px;
  color: #a61b29;
  background-color: transparent;
  flex-grow: 1;
}
.search-input::placeholder {
  color: #a61b29; /* 设置placeholder的字体颜色为白色 */
  opacity: 1; /* 确保颜色不透明 */
}
.search-type option {
  background-color: white; /* 背景为白色 */
  color: black; /* 下拉选项的字体颜色为黑色 */
}

.search-button {
  height: 36px;
  width: 40px;
  background-color: #a61b29;
  color: white;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  position: absolute; /* 绝对定位 */
  right: 0; /* 使按钮右对齐 */
}


</style>

