<!-- 买家用户首页中的主页分类 -->
<script setup lang="ts">
import { useRouter } from 'vue-router';
import Navbar from '../components/Navbar.vue';

const router = useRouter();

const blocks = [
  { id: 1, title: '服装', bgImage: 'url(src/assets/Kind1.jpg)', gridArea: '1 / 1 / 2 / 3', category: '1' },
  { id: 2, title: '首饰', bgImage: 'url(src/assets/Kind2.png)', gridArea: '1 / 3 / 2 / 4', category: '2' },
  { id: 3, title: '家具', bgImage: 'url(src/assets/Kind3.png)', gridArea: '2 / 1 / 3 / 2', category: '3' },
  { id: 4, title: '工艺品', bgImage: 'url(src/assets/Kind4.png)', gridArea: '2 / 2 / 3 / 4', category: '4' },
  { id: 5, title: '小物件', bgImage: 'url(src/assets/Kind5.png)', gridArea: '3 / 1 / 4 / 3', category: '5' },
  { id: 6, bgImage: 'url(src/assets/Kind5.png)', gridArea: '3 / 3 / 4 / 3', class: 'disabled' },
];

const navigateToCategory = (category: string) => {
  console.log('Navigating to category:', category); // 调试输出
  router.push(`/merchandise/${category}`);
};
</script>

<template>
  <!-- <Navbar /> -->
  <div class="container">
    <div class="sidebar">
      <h2>商品分类</h2>
    </div>
    <div class="block-container">
      <div
        v-for="block in blocks"
        :key="block.id"
        :style="{ backgroundImage: block.bgImage, gridArea: block.gridArea }"
        :class="['block', block.class]"
        @click="!block.class && navigateToCategory(block.category)"
      >
        {{ block.title }}
      </div>
    </div>
  </div>
</template>

<style scoped>
/* html, body {
  height: 100%;
  margin: 0;
  overflow: hidden;
} */

.container {
  display: flex;
  height: 100vh;
}

.background {
  position: fixed;
  top: 60px;
  width: 100%;
  height: calc(100vh - 60px);
  background-image: url('src/assets/Background.png');
  background-size: cover;
  background-position: center;
  z-index: -1;
}

.sidebar {
  width: 200px;
  padding: 20px;
  background-color: #f5f5f5;
  border-right: 1px solid #ccc;
}

.sidebar h2 {
  writing-mode: vertical-rl;
  text-align: center;
  font-family: 'Arial', sans-serif;
  font-size: 24px;
}

.sidebar ul {
  list-style: none;
  padding: 0;
  width: 100%;
}

.block-container {
  flex: 4;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  grid-template-rows: repeat(3, 150px);
  grid-gap: 10px;
  padding: 20px;
}

.block {
  background-size: cover;
  background-position: center;
  border: 1px solid #ccc;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 18px;
  text-align: center;
  animation: fadeIn 1s ease-in-out;
  cursor: url('src/assets/cursor.png') 16 16, auto;
}

.block:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.block:active {
  transform: translateY(0);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.block:nth-child(odd) {
  animation: moveUp 1s forwards;
}

.disabled {
  pointer-events: none;
  transform: none !important;
  box-shadow: none !important;
}

.block.disabled:hover {
  transform: none !important;
  box-shadow: none !important;
}

.block:nth-child(even) {
  animation: moveDown 1s forwards;
}

@keyframes moveRight {
  from {
    transform: translateX(100%);
  }
  to {
    transform: translateX(0);
  }
}

@keyframes moveLeft {
  from {
    transform: translateX(-100%);
  }
  to {
    transform: translateX(0);
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateX(50px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}
</style>
