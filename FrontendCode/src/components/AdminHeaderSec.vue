<!-- 管理员页面的头部 -->
<template>
  <header class="header">
    <div class="logo">
      <img src="@/assets/logo1.png" alt="Logo" class="icon"/>
      <span style="font-size: 17px;">瑕宝阁</span>
    </div>
    <div class="user-info">
      <img src="@/assets_dxy/person_icon.svg" alt="Icon" class="icon">
      <el-dropdown trigger="click">
        <span style="font-size: 17px;" class="el-dropdown-link">
          管理员ID: {{ userId }} 
          <el-icon class="el-icon--right"><arrow-down /></el-icon>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item :icon="CircleClose" @click="logout()">退出登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </header>
</template>

<script setup>
import { ElDropdown, ElDropdownMenu, ElDropdownItem, ElIcon, ElMessage } from 'element-plus';
import { ref } from 'vue';
import { ArrowDown, CircleClose } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router';
import axiosInstance from '../components/axios';

const router = useRouter();

const userId =localStorage.getItem('userId');

const message = ref('');
const logout = async () => {
  try {
    const response = await axiosInstance.post('/Account/logout');
    console.log('logout response:', response.data);
    
    if (response.data.message === '登出账号成功！') {
      ElMessage.success('退出登录成功');
      //localStorage.removeItem('userToken'); 
      router.push('/loginandregister');
    } else {
      ElMessage.error(`退出登录失败: ${response.data.message}`);
    }
  } catch (error) {
    console.error('请求失败:', error);
    ElMessage.error('请求失败，请稍后再试');
  }
};

</script>
  
<style scoped>
.header {
  background-color: #ffffff;
  padding: 10px;
  display: flex;
  height: 70px;
}

.logo {
  display: flex;
  align-items: center;
}

.icon {
  height: 30px;
  margin-right: 10px;
}

.user-info {
  margin-left: auto;
  margin-right: 10px;
  display: flex;
  align-items: center;
}

</style>
