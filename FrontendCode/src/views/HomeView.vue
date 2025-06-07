<template>
  <swiper
      :direction="'vertical'"
      :slidesPerView="1"
      :mousewheel="true"
      :speed="1000"
      :modules="[Mousewheel]"
      class="mySwiper"
      @slideChange="onSlideChange"
  >
      <swiper-slide>
      <div class="page1">
      <Navbar class="narbar"/>
      <div  class="LRcontainer">
      <img
          src="@/assets/mmy/arrow_left.svg"
          class="arrow-button1"
          @click="setAd((currentAdIndex - 1 + numAds) % numAds)"
          @mouseenter="stopTimer"
          @mouseleave="startTimer"
      />
      <img
          src="@/assets/mmy/arrow_right.svg"
          class="arrow-button2"
          @click="setAd((currentAdIndex + 1) % numAds)"
          @mouseenter="stopTimer"
          @mouseleave="startTimer"
      />
      <div class="ad">
          <img
          v-show="currentAdIndex === 0"
          src="@/assets/mmy/ad1.png"
          />
          <div class="ad2" v-show="currentAdIndex === 1">
            <img
            src="@/assets/mmy/ad2.png"
            />
            <div class="ad2_text" >
              <div class="column1">文房雅韵</div>
              <div class="column2">墨香千年</div>
            </div>
         </div>
         <div class="ad3" v-show="currentAdIndex === 2">
          <img
          src="@/assets/mmy/ad3.jpg"
          />
         </div>
      <div class="knotArea" @mouseenter="stopTimer" @mouseleave="startTimer">
      <img
          v-show="currentAdIndex !== 0"
          src= "@/assets/mmy/knot.svg"
          class="knot1"
          @click="setAd(0)"
      />
      <img
          v-show="currentAdIndex === 0"
          src= "@/assets/mmy/active_knot.svg"
          class="knot1"
          @click="setAd(0)"
      />
      <img
          v-show="currentAdIndex !== 1"
          src="@/assets/mmy/knot.svg"
          class="knot2"
          @click="setAd(1)"
      />
      <img
          v-show="currentAdIndex === 1"
          src="@/assets/mmy/active_knot.svg"
          class="knot2"
          @click="setAd(1)"
      />
      <img
          v-show="currentAdIndex !== 2"
          src="@/assets/mmy/knot.svg"
          class="knot3"
          @click="setAd(2)"
      />
      <img
          v-show="currentAdIndex === 2"
          src="@/assets/mmy/active_knot.svg"
          class="knot3"
          @click="setAd(2)"
      />
      </div>
      </div>  
      </div>
     </div>

      </swiper-slide>

      <swiper-slide>
      <div class="page2">
        
          <transition 
          v-show="activeSlideIndex === 1"
          appear
          class="animate__animated animate__slideInDown">
          <img src="@/assets/mmy/fan_up.png" class="fan_up">
          </transition>
          <transition 
          v-show="activeSlideIndex === 1"
          appear
          class="animate__animated animate__slideInUp">
          <img src="@/assets/mmy/fan_down.png" class="fan_down">
          </transition>
          <div class="text1">
            <h2>市场背景</h2>
            <p>&nbsp;&nbsp;由于非遗产品多为手工制作，制作过程中的微瑕疵不可避免。这些微瑕产品虽然具有独特的美感和个性，却在传统市场中难以得到充分认可，面临销售困难。</p>
            <p>&nbsp;&nbsp;同时，伴随消费者对环保和可持续发展的关注度增加，许多人开始接受并欣赏这些“有瑕疵的美”，认为这些微瑕产品展示了手工艺品的真实制作过程和独特魅力。然而，现有市场和销售渠道尚未充分发掘这一特点的市场潜力，大量微瑕非遗产品因缺乏适当的推广平台而无法找到理想的买家。因此，设立本交易平台能够精准匹配这些微瑕非遗产品与追求个性化的消费者，填补市场空白，提升微瑕产品的经济价值，并推动非遗文化在现代市场中的传播与发展。</p>
          </div>
          <div class="text2">
            <h2>产品定位</h2>
            <p>&nbsp;&nbsp;本平台作为支持微瑕非遗交易的电商平台，相比淘宝、天猫等成熟等电商平台，在产品定位策略上天然拥有精细化特点。我们深度挖掘并精准定位了微瑕非遗产品的独特价值与市场需求，旨在打造一个既尊重传统又拥抱创新，同时能够满足消费者个性化追求的文化消费空间，进而促进微瑕非遗交易的普及化以及微瑕非遗产品的接续流通，保证微瑕非遗市场的可持续发展。</p>
          </div>


      </div>
      </swiper-slide>

      <swiper-slide>
      <div  class="page3">
        <!-- -------------笨方法长盛不衰------------- -->
        <div class="sidebar">
          <div class="one-choice1" @click="navigateTo('/merchandise/1')">
            <img class="IconPage3" src="@/assets/mmy/工艺品.png">
            <p>商品</p>
          </div>

          <div class="one-choice2" @click="navigateTo('/bazaar')">
            <img class="IconPage3" src="@/assets/mmy/家具.png">
            <p>市集</p>
          </div>

          <div class="one-choice1" @click="navigateTo('/forum')">
            <img class="IconPage3" src="@/assets/mmy/服装.png">
            <p>论坛</p>
          </div>

          <div class="one-choice2" @click="navigateTo('/cart')">
            <img class="IconPage3" src="@/assets/mmy/首饰.png">
            <p>收藏夹</p>
          </div>

          <div class="one-choice1" @click="navigateTo('/ordercentre')">
            <img class="IconPage3" src="@/assets/mmy/小物件.png">
            <p>订单中心</p>
          </div>
        </div>

        <!-- ----------动画效果尝试失败---------- -->
        <!-- <div class="sidebar">
          <div class="one-choice1" @click="navigateTo('/merchandise/1')">
          <transition
            v-if="activeSlideIndex === 2"
            appear
            :name="'bounceInDown'"
            :style="{ animationDelay: `0.3s` }">
              <img class="IconPage3" src="@/assets/mmy/工艺品.png">
          </transition>
          <p>商品</p>
        </div>
        <div class="one-choice2" @click="navigateTo('/bazaar')">
            <transition
              v-if="activeSlideIndex === 2"
              appear
              :name="'bounceInDown'"
              :style="{ animationDelay: `0.6s` }">
                <img class="IconPage3" src="@/assets/mmy/家具.png">
            </transition>
            <p>市集</p>
        </div>
        <div class="one-choice1" @click="navigateTo('/forum')">
            <transition
              v-if="activeSlideIndex === 2"
              appear
              :name="'bounceInDown'"
              :style="{ animationDelay: `0.9s` }">
                <img class="IconPage3" src="@/assets/mmy/服装.png">
            </transition>
            <p>论坛</p>
        </div>
        <div class="one-choice2" @click="navigateTo('/cart')">
            <transition
              v-if="activeSlideIndex === 2"
              appear
              :name="'animate__bounceInDownanimate__animated animate__bounce'"
              :style="{ animationDelay: `1.2s` }">
                <img class="IconPage3" src="@/assets/mmy/首饰.png">
            </transition>
            <p>收藏夹</p>
        </div>
        <div class="one-choice1" @click="navigateTo('/ordercentre')">
            <transition
              v-if="activeSlideIndex === 2"
              appear
              :name="'bounceInDown'"
              :style="{ animationDelay: `1.5s` }">
                <img class="IconPage3" src="@/assets/mmy/小物件.png">
            </transition>
            <p>订单中心</p>
        </div>
        </div> -->
        <!-- -----------服务器无法识别该绝对路径，无法显示图标----------- -->
        <!-- <div class="sidebar">
          <div 
            v-for="(item, index) in menuItems" 
            :key="index"
            :class="index % 2 === 0 ? 'one-choice1' : 'one-choice2'"
            @click="navigateTo(item.link)"
          >
            <img class="IconPage3" :src="item.icon">
            <p>{{ item.text }}</p>
          </div>
      </div> -->

      <div class="foot">
          <p>指导教师：袁时金、穆斌</p>
          <p>机构官网：https://www.tongji.edu.cn</p>
          <div class="police">
          <img src="@/assets/mmy/公安.png" >
          <div class="text">京公网安备&nbspXXXXXXXXXXXXXX号&nbsp&nbsp京ICP备XXXXXXXX号-1</div>
          </div>
          <p>网站维护：同济大学软件学院微瑕非遗研究团队&nbsp&nbsp联系方式：123456789</p>
          <p>建议使用Edge、Chrome、Firefox浏览器，最佳分辨率1920×1080</p>
      </div>
      </div>
      </swiper-slide>
  </swiper>
</template>

<script setup>
import { Swiper, SwiperSlide } from 'swiper/vue';
import 'swiper/css';
import { Mousewheel } from 'swiper/modules';
import Navbar from '../components/Navbar.vue';
// import HomeCategory from './HomeCategory.vue';
import { ref, onMounted, onBeforeUnmount ,computed} from 'vue';
import 'animate.css';
import { useRouter } from 'vue-router';

const currentAdIndex = ref(0);
// const menuItems = reactive([
//   { text: "商品", link: "/merchandise/1", icon: "/src/assets/mmy/工艺品.png" },
//   { text: "市集", link: "/bazaar", icon: "/src/assets/mmy/家具.png" },
//   { text: "论坛", link: "/forum", icon: "/src/assets/mmy/服装.png" },
//   { text: "收藏夹", link: "/cart", icon: "/src/assets/mmy/首饰.png" },
//   { text: "订单中心", link: "/ordercentre", icon: "/src/assets/mmy/小物件.png" }
// ]);
const numAds = 3;
const activeSlideIndex=ref(0);
let adTimer = null;  // 用于存储计时器 ID，这样写是为了知道数据类型

////////////////////////// 每2.5秒调用setAd函数
const startTimer = () => {
if (adTimer === null) {
  adTimer = setInterval(() => {
    setAd((currentAdIndex.value + 1) % numAds);
  }, 2500);
}
};
const stopTimer = () => {
if (adTimer !== null) {
  clearInterval(adTimer);
  adTimer = null;
}
};
onMounted(() => {
  startTimer();//内部调用计时器
  console.log('Swiper initialized');
});
onBeforeUnmount(() => {
  console.log('Swiper cleaned up');
});


function onSlideChange(swiper) {
  activeSlideIndex.value = swiper.activeIndex;
  console.log(`activeSlideIndex.value is ${activeSlideIndex.value}`);
}

function setAd(index) {
  currentAdIndex.value = index;
}
const router = useRouter();

function navigateTo(link) {
  router.push(link);
}

document.querySelectorAll('.icon').forEach(icon => {
            icon.addEventListener('click', () => {
                icon.classList.add('shake');
                // Remove the class after animation ends
                setTimeout(() => {
                    icon.classList.remove('shake');
                }, 500); // Match the duration of the animation
            });
        });
</script>
  
<style scoped>

@import url('https://fonts.googleapis.com/css2?family=Ma+Shan+Zheng&display=swap');


div {
  user-select: none;
  outline: none; 
  cursor: default; 
}
#app {
height: 100%;
}
html,
body {
position: relative;
font-family: 'Ma Shan Zheng', cursive;
height: 100%;
}

body {
background: #eee;
font-family: 'Ma Shan Zheng', cursive;
/* font-family: Helvetica Neue, Helvetica, Arial, sans-serif; */
font-size: 14px;
color: #000;
margin: 0;
padding: 0;
}

.swiper {
width: 100%;
height: 100%;
}

.navbar {
  position: absolute;
  background-color: rgba(0, 0, 0, 0.3);
  color: #FFFFFF;
  z-index: 3;
}

/* Use :deep() to target the inner elements of the navbar */
:deep(.navbar-bottom) {
  background-color: rgba(0, 0, 0, 0.3) !important; 
}

:deep(.search-container) {
  background-color: rgba(0, 0, 0, 0.3) !important;
  color: #FFFFFF !important;
}

:deep(.search-input::placeholder) {
  color: #FFFFFF !important; 
}
:deep(.search-button) {
  background-color: rgba(0, 0, 0, 0.3) !important;
  color: #FFFFFF !important;
}
:deep(.navbar-item .nav-link.active),
:deep(.navbar-item .nav-link:hover) {
  background-color: rgba(0, 0, 0, 0.3) !important; 
  color: #FFFFFF !important; 
}
:deep(.navbar-item .nav-link){
  color: #FFFFFF !important; 
}
.mySwiper {
height: 100vh;  
width: 100%;    
}

.LRcontainer {
background-color: rgba(36, 63, 51);
height: 100vh;
display: flex;
flex-direction: column;
justify-content: center;
align-items: center;
position: relative; /* 添加这个以便子元素可以使用绝对定位 */
}

.ad,.ad2,.ad3 {
display: flex;
justify-content: center;
align-items: center;
width: 100%;
height: 100%;
overflow: hidden;
position: relative; /* 确保 .knotArea 绝对定位基于 .ad */
}

.ad img,.ad2 img ,.ad3 img{
width: 100%;
height: auto;
object-fit: cover; /* 保证图片自适应 */
z-index: 1;
}

.ad2{
  background-image: url("../assets/mmy/texture2.png");
}
.ad3{
  background-color:#243569;
}
@font-face {
  font-family: 'SJxingshu';
  src: url('../assets/fonts/SJxingshu.ttf') format('truetype');
}
.ad2_text{
  display: flex;
  flex-direction: row;
  z-index: 3;

  color:white;
  position: fixed;
  top: 200px;
  left: 180px;
  /* gap:10px; */

  font-size: 70px;
  font-family: 'SJxingshu', serif;
}

.column1, .column2 {
    writing-mode: vertical-rl; /* 将文字垂直排列 */
}
.column1 {
}
.column2 {
  padding-top:150px;
}
.arrow-button1,
.arrow-button2,
.knot1,
.knot2,
.knot3 {
cursor: pointer;
z-index: 2;
}

.arrow-button1 {
width: 50px;
height: 50px;
position: fixed;
top: 50%;
left: 70px;
}

.arrow-button2 {
width: 50px;
height: 50px;
position: fixed;
top: 50%;
right: 70px;
}

.arrow-button2:hover,
.arrow-button1:hover {
box-shadow: 0 0 5px rgba(229, 215, 215, 0.5);
transform: scale(1.1);
}

.knotArea {
display: flex;
justify-content: center;
gap: 15px;
position: absolute;
bottom: 20px; /* 你可以调整这个值来改变位置 */
width: 10%;
}
.ad_discount {
width: 130vh;
display: block;
margin: auto;
}


.page2{
  overflow: hidden;
  width: 100%;
  height: 100%;
  position: relative;
  background-image: url("../assets/mmy/blue_background.jpg");
}
.fan_up{
position:absolute;
top: 0;
right: 50%;
margin-right: -95px;
width: 990px;
height: 472px;
z-index:1;
}
.fan_down{
position:absolute;
bottom: 0;
left: 50%;
margin-left: -260px;
width: 1222px;
height: 540px;
z-index:2;
}
.animate__animated.animate__slideInUp{
--animate-duration: 1.2s;
animation-delay: 0.5s;
}
.animate__animated.animate__slideInDown{
--animate-duration: 1.2s;
animation-delay: 0.5s;
}
.page3{
width:100%;
height:100%;
display:flex;
overflow: hidden;
/* flex-direction: column; */
justify-content: center;
background-image: url("../assets/mmy/background.jpg");
background-color: rgba(65, 33, 10, 0.2);
font-family: 'Noto Serif SC', serif;

}
.sidebar{
top:20%;
display: flex;
flex-direction: row;
position: relative;
width:1000px;
/* align-items: center; */

}
.one-choice1,
.one-choice2 {
position: relative;
width:200px;
height:178.34px;
}
.one-choice1{
background-image: url("../assets/mmy/红底.jpg");
}
.one-choice2{
background-image: url("../assets/mmy/黄底.jpg");
}
.IconPage3 {
position: absolute;
top: 10px; 
left: 50%; 
transform: translateX(-50%); 
z-index: 2;
width: 60%;
transition: transform 0.3s ease;
transform-origin: center;
cursor: pointer;
}
.IconPage3:hover{
  transform: translateX(-50%) scale(1.05); 
}
.one-choice1 p,
.one-choice2 p {
position: absolute;
font-family: 'Noto Serif SC', serif;
font-size:20px;
bottom: 10px; 
left: 50%; 
transform: translateX(-50%); 
color:#FFFFFF;
cursor: pointer;
z-index: 2;
}
.foot{
bottom:5%;
display: flex;
flex-direction: column;
position:absolute;
width:1000px;
align-items: center;
font-size: 16px;
}
.police{
display: flex;
flex-direction: row;
}
.text1,.text2{
  overflow: auto;
  z-index:4;
  position:fixed;
}
.text1{
  width: 42%;
  height:37%;
  right:20px;
  top:30px;
}
.text2{
  width: 37%;
  height:37%;
  left:40px;
  bottom:30px;
}
.text1 h2,.text2 h2{
  font-family: 'SJxingshu';
  color:white;
  font-size:30px;
}
.text1 p,.text2 p{
  font-family: 'Noto Serif SC', serif;
  color:white;
  font-size:20px;
  text-align: left;

}
.text1::-webkit-scrollbar {
  width: 10px;
}

.text1::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.text1::-webkit-scrollbar-thumb {
  background: #5f697a;
  border-radius: 10px;
}
.text2::-webkit-scrollbar {
  width: 10px;
}

.text2::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.text2::-webkit-scrollbar-thumb {
  background: #5f697a;
  border-radius: 10px;
}

</style>