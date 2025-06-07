<!-- 管理员页面的商家认证 -->
<template>
  <div v-show="role!=='管理员'">
    <p style="margin-top: 100px; font-weight: bold; font-size: 16px;">请登录管理员账号！！</p>
    <router-link :to="{ name: 'LoginAndRegister' }">点击此处跳转登录界面...</router-link>
  </div>
  <div class="container1" v-show="role==='管理员'">
    <AdminSidebarMenu />
    <div class="main-content">
      <AdminHeaderSec />
      <div class="content">
        <hr>
        <div class="container">
          <h2>审核商家认证申请</h2>
          <div class="table-content">
            <el-table :data="paginatedData">
              <el-table-column prop="storeId" label="商家账号"></el-table-column>
              <el-table-column prop="status" label="申请状态"></el-table-column>
              <el-table-column label="操作">
                <template #default="scope">
                  <el-button @click="search(scope.row.storeId)" type="primary">查看详情</el-button>
                  <el-button @click="updateRecord(scope.row.storeId,true)" :disabled="scope.row.status !== '待审核'" type="primary">通过申请</el-button>
                  <el-button @click="updateRecord(scope.row.storeId,false)" :disabled="scope.row.status !== '待审核'" type="danger">拒绝申请</el-button>
                </template>
              </el-table-column>
            </el-table>

            <el-dialog v-model="dialogVisible" title="申请详情" width="60%" >
              <div v-if="selectedDetail">
                <p>商家账号: {{ selectedDetail.storeId }}</p>
                <p>审核状态: {{ selectedDetail.status }}</p>
                <p>申请详情: {{ selectedDetail.authentication }}</p>
                <!-- <img :src="selectedDetail.photo" alt="authentication" v-if="selectedDetail.photo" style="width: 90%; object-fit: cover;" /> -->
                <img :src="selectedDetail.photo.imageUrl" :alt="`申请图片 ${selectedDetail.photo.imageId}`" class="detail-image" v-if="selectedDetail.photo" style="width: 90%; object-fit: cover;" />
              </div>
            </el-dialog>

            <div class="pagination-container">
              <div class="pagination">
                <div style="text-align: center;">
                  <span>共 {{ totalItems }} 条</span>
                </div>          
                <el-pagination
                  background
                  layout="prev, pager, next"
                  :total="totalItems"
                  :page-size="pageSize"
                  @current-change="handlePageChange"
                  :current-page="currentPage">
                </el-pagination>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  
</template>

<script setup>
import AdminSidebarMenu from '../components/AdminSidebarMenu.vue'
import AdminHeaderSec from '../components/AdminHeaderSec.vue'
import { reactive, ref, computed, onMounted  } from 'vue';
import { ElTable, ElTableColumn, ElPagination, ElButton, ElMessage, ElDialog, ElLoading } from 'element-plus';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

const userId =localStorage.getItem('userId');
// const userId ='1';
const role=localStorage.getItem('role');

const records = reactive([]);
const message = ref('');
const fetchRecords = async () => {
  //加载缓冲
  const loadingInstance = ElLoading.service({
    lock: true,
    text: '正在加载...',
    target: '.table-content',
  });

  try {
    const response = await axiosInstance.post('/Administrator/GetAllAuthentication', {
      "adminId": userId
    });

    records.splice(0, records.length, ...response.data);
    message.value = '已获取申请数据';
    console.log(records);
  } catch (error) {
    if (error.response) {
      message.value = error.response.data;
    } else {
      message.value = '获取数据失败';
    }
    ElMessage.error('获取数据失败，请稍后再试');
  }

  loadingInstance.close();
  console.log(message.value);
};
onMounted(() => {
  fetchRecords();
});


const message1 = ref('');
const updateRecord = async (storeId,result) => {
  try {
    const response = await axiosInstance.put('/Administrator/UpdateStoreAuthentication', {
      "storeId": storeId,
      "result": result,
      "adminId": userId
    });
    message1.value = response.data;
    fetchRecords();
  } catch (error) {
    if (error.response) {
      message1.value = error.response.data;
    } else if (error.request) {
      message1.value = '请求未收到响应';
    } else {
      message1.value = '操作失败';
    }
  }
  ElMessage.info(message1.value);
  console.log(message1.value);
};

// 设置表格页面大小及当前页数
const pageSize = ref(9);
const currentPage = ref(1);

// const totalItems = computed(() => dataArr.length);
const totalItems = computed(() => records.length);

// 处理并分页后的数据
const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  // return dataArr.slice(start, end);
  return records.slice(start, end);
});

// 切换页面
const handlePageChange = (page) => {
  currentPage.value = page;
};

// dialog（详情窗口）相关变量
const dialogVisible = ref(false);
const selectedDetail = reactive({});

const search = (id) => {
  const store = records.find(store => store.storeId === id);
  if (store) {
    Object.assign(selectedDetail, store);
    // selectedDetail.photo = `data:image/png;base64,${selectedDetail.photo}`;
    dialogVisible.value = true;
  }
};

</script>

<style scoped>
h2 {
  display: block;
  font-size: 1.5em;
  margin-block-start: 0.83em;
  margin-block-end: 0.83em;
  margin-inline-start: 0px;
  margin-inline-end: 0px;
  font-weight: bold;
  unicode-bidi: isolate;
}
.container1 {
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
  text-align: left;
  color: #000000;
}
.container {
  background-color: #ffffff;
  padding: 0 10px;
}

.pagination-container {
  display: flex;
  justify-content: center;
}

.pagination {
  max-width: 100%;
}

</style>
