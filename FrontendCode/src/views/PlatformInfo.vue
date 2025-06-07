<!-- 管理员页面的平台信息 -->
<template>
  <div class="container">
    <AdminSidebarMenu />
    <div class="main-content">
      <AdminHeaderSec />
      <div class="content">
        
        <hr>
        <div class="platform-info">
          <h2>平台信息</h2>
          
        </div>

      </div>
    </div>
  </div>
  
</template>

<script setup>
import AdminSidebarMenu from '../components/AdminSidebarMenu.vue'
import AdminHeaderSec from '../components/AdminHeaderSec.vue'

  const type = ''
  const keyword = ''

</script>

<style scoped>
h2 {
  display: block; /* 元素作为块级元素显示，占据其父容器的整个宽度 */
  font-size: 1.5em; /* 字体大小设置为1.5em，em是相对于父元素字体大小的单位 */
  margin-block-start: 0.83em; /* 元素上方的外边距（margin），以em为单位 */
  margin-block-end: 0.83em; /* 元素下方的外边距，以em为单位 */
  margin-inline-start: 0px; /* 元素左侧的外边距，设置为0 */
  margin-inline-end: 0px; /* 元素右侧的外边距，设置为0 */
  font-weight: bold; /* 字体加粗 */
  unicode-bidi: isolate; /* 设置双向文本的隔离行为，防止混合使用不同方向的文本对布局的影响 */
}
.container {
  display: flex;
  height: 100vh;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.content {
  flex: 1;
  padding: 0 15px;
}

.platform-info {
  background-color: #ffffff;
  padding: 0 10px;
  text-align: left; /* 确保文本对齐样式 */
  color: #000000; /* 确保文本颜色 */
}

form {
  display: flex;
  align-items: center;
}

label {
  margin-right: 10px;
}

select,
input {
  margin-right: 10px;
  padding: 5px;
}

button {
  padding: 5px 10px;
}
</style>
