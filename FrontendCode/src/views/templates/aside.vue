<script setup lang="ts">
import { ref} from 'vue';
import { ElMenu} from 'element-plus';
import 'element-plus/dist/index.css';
import Navbar from '../../components/Navbar.vue';

const activeIndex = ref('1');
const option=ref(1);
const menuItems = ref([
  { index: 1, title: '全部' },
  { index: 2, title: '待付款' },
  { index: 3, title: '已付款' },
  { index: 4, title: '运输中' },
  { index: 5, title: '已送达' },
  { index: 6, title: '待退货' },
  { index: 7, title: '已退货' },
]);
const menuChange = (index:any) => {
  activeIndex.value = index.toString();
  option.value = parseInt(index);
};
</script>

<template>
    <el-aside width="18vh" style="background-color:  #82111f; ">
  <div class="big-title" style="display: flex; justify-content: center; align-items: center; width: 100%;">
    <img class="image" src="/src/assets/czw/aside.svg" alt="Original Image"  />
    <span>订单</span>
    <img class="flipped-image" src="/src/assets/czw/aside.svg" alt="Flipped Image" />
  </div>
  <div style="width: 100%;">
    <el-menu
      :default-active="activeIndex"
      mode="vertical"
      background-color="#ffffff"
      text-color="black"
      active-text-color="#d42517"
      @select="menuChange"
      style="width: 100%;"
    >
      <el-menu-item 
        v-for="item in menuItems" 
        :key="item.index" 
        :index="item.index.toString()"
        
      >
        <span class="el-icon-menu">{{ item.title }}</span>
      </el-menu-item>
    </el-menu>
  </div>
</el-aside>
</template>

<style scoped>
.image {
    width: 30px; /* 根据需求调整大小 */
    height: auto;
    /* transform: rotate(-90deg);*/
  }
  
  .flipped-image {
    width: 30px; /* 确保与第一张图一致 */
    height: auto;
    transform: scaleX(-1); /* 使用CSS进行垂直翻转 */

  }
  .big-title{
    font-size:4vh;  /* 将字体大小设置为32像素 */
    font-weight: bold; /* 可选：让文字加粗 */
    text-align: center; /* 可选：使文字水平居中 */
    margin: 20px 0; /* 可选：设置上下的外边距 */
    font-family: "华文中宋", sans-serif;
    color: #ffffff; /* 设置文字颜色为白色 */
  }
  .el-menu {
    position: relative;
    overflow: hidden;
  }
  
  .el-menu-item {
    position: relative;
    transition: background-color 0.3s ease;
  }
  
  .el-menu-item::before {
    content: '';
    position: absolute;
    left: 0;
    top: 0;
    width: 0;
    height: 100%;
    background-color: #bdaead;
    z-index: 0;
    transition: width 0.3s ease;
  }
  
  .el-menu-item:hover::before {
    width: 100%;
  }
  
  .el-menu-item > span {
    position: relative;
    z-index: 1; /* 确保文本始终在滑块上方 */
    transition: color 0.3s ease;
  }
  
  .el-menu-item:hover {
    color: #ffffff;
  }
  .el-icon-menu{
    font-size:2vh;
    font-weight: bold; /* 可选：让文字加粗 */  
    font-family: "华文中宋", sans-serif;
  }
</style>