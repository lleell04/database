<!-- 管理员页面的市集管理 -->
<template>
  <div v-show="role!=='管理员'">
    <p style="margin-top: 100px; font-weight: bold; font-size: 16px;">请登录管理员账号！！</p>
    <router-link :to="{ name: 'LoginAndRegister' }">点击此处跳转登录界面...</router-link>
  </div>
  <div class="container" v-show="role==='管理员'">
    <AdminSidebarMenu />
    <div class="main-content">
      <AdminHeaderSec />
      <div class="content">
        <hr>
        <div class="main-container">
          <div class="form-container">
            <h2>市集设置</h2>
            <el-form>
              <el-row :gutter="100">
                <el-col :span="7">
                  <el-form-item label="市集主题">
                    <el-input v-model="formData.topic"></el-input>
                  </el-form-item>
                </el-col>
                <el-col :span="7"> 
                  <el-form-item label="选择类型">
                    <el-select v-model="formData.selectedOptions" placeholder="将向以下类型商家发送邀请" style="width: 300px;" >
                      <el-option-group
                        v-for="group in categories"
                        :key="group.largeCategoryName"
                        :label="group.largeCategoryName"
                      >
                        <el-option
                          v-for="item in group.subCategories"
                          :key="item.subCategoryId"
                          :label="item.subCategoryName"
                          :value="item.subCategoryId"
                        />
                      </el-option-group>
                    </el-select>

                    <!-- <el-select v-model="formData.selectedOptions" multiple placeholder="将向以下类型商家发送邀请" style="width: 300px;" @change="handleOptionChange">
                      <el-option
                        v-for="item in options"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value">
                      </el-option>
                    </el-select> -->
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row :gutter="100">
                <el-col :span="7">
                  <el-form-item label="开始时间">
                    <el-input type="date" v-model="formData.start" placeholder="请选择日期"></el-input>
                  </el-form-item>
                </el-col>
                <el-col :span="7">
                  <el-form-item label="结束时间">
                    <el-input type="date" v-model="formData.end" placeholder="请选择日期"></el-input>
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row :gutter="100">
                <el-col :span="20"> 
                  <el-form-item label="市集信息">
                    <el-input type="textarea" v-model="formData.detail" style="width: 30%; min-height: 10px;" placeholder="请输入市集相关信息"> </el-input>
                  </el-form-item>
                </el-col>
              </el-row>
              <el-form-item label="图片上传">
                <input type="file" @change="handleFileUpload" accept=".png, .jpg">
              </el-form-item>
              <el-form-item>
                <el-button @click="handleSubmit" type="primary" >提交</el-button>
              </el-form-item>
            </el-form>      
          </div>


          <div class="table-container">
            <h2>市集管理</h2>
            <div class="table-content">
              <el-table :data="paginatedData">
                  <el-table-column prop="theme" label="市集主题"></el-table-column>
                  <el-table-column prop="startTime" label="开始时间"></el-table-column>
                  <el-table-column prop="endTime" label="结束时间"></el-table-column>
                  <el-table-column label="操作">
                    <template #default="scope">
                      <el-button @click="search(scope.row.marketId)" type="primary">查看详情</el-button>
                      <el-button @click="deleteMarket(scope.row.marketId)" type="danger">取消市集</el-button>
                    </template>
                  </el-table-column>
              </el-table>

              <el-dialog v-model="dialogVisible" title="市集详情" width="60%" >
                <div v-if="selectedDetail">
                  <p>主题: {{ selectedDetail.theme }}</p>
                  <p>开始时间: {{ selectedDetail.startTime }}</p>
                  <p>结束时间: {{ selectedDetail.endTime }}</p>
                  <p>详细信息: {{ selectedDetail.detail }}</p>
                  <!-- <img :src="selectedDetail.imageSrc" alt="Market Poster" v-if="selectedDetail.imageSrc" style="width: 90%; object-fit: cover;" /> -->
                  <img :src="selectedDetail.image.imageUrl" :alt="市集图片" v-if="selectedDetail.image" style="width: 90%; object-fit: cover;" />
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
  </div>
  
</template>
  
<script setup>
import AdminSidebarMenu from '../components/AdminSidebarMenu.vue'
import AdminHeaderSec from '../components/AdminHeaderSec.vue'
import { reactive, ref, computed, onMounted  } from 'vue';
import { ElTable, ElTableColumn, ElPagination, ElButton, ElDialog, ElForm, ElFormItem, ElInput, ElRow, ElCol, ElSelect, ElOption, ElMessage } from 'element-plus';
import { ElLoading, ElOptionGroup } from 'element-plus';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

const userId =localStorage.getItem('userId');
const role=localStorage.getItem('role');

const records = reactive([]);
const categories = reactive([]);
const message01 = ref('');

const fetchRecords = async () => {
  //加载缓冲
  const loadingInstance = ElLoading.service({
    lock: true,
    text: '正在加载...',
    target: '.table-content',
  });

  try {
    const response = await axiosInstance.get('/Administrator/GetAllMarket');
    records.splice(0, records.length, ...response.data);
    message01.value = '已获取市集数据';
    console.log(records);
  } catch (error) {
    loadingInstance.close();
    if (error.response) {
      message01.value = error.response.data;
    } else {
      message01.value = '获取数据失败';
    }
    ElMessage.error('获取数据失败，请稍后再试');
  }

  loadingInstance.close();
  console.log(message01.value);
};

const fetchCategories = async () => {
  try {
    const response = await axiosInstance.get('/Classification/GetAllCategories');
    categories.splice(0, categories.length, ...response.data.categories);
    message01.value = '已获取系统分类数据';
    console.log(categories);
  } catch (error) {
    if (error.response) {
      message01.value = error.response.data;
    } else {
      message01.value = '获取分类数据失败';
    }
    ElMessage.error('获取分类数据失败，请稍后再试');
  }
  console.log(message01.value);
};

onMounted(() => {
  fetchRecords();
  fetchCategories();
})

// 页面大小及当前页数
const pageSize = ref(4);
const currentPage = ref(1);

// dialog（详情窗口）相关变量
const dialogVisible = ref(false);
const selectedDetail = reactive({});

const totalItems = computed(() => records.length);

// 处理并分页后的数据
const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return records.slice(start, end);
});

const handlePageChange = (page) => {
  currentPage.value = page;
};

const search = (id) => {
  const market = records.find(market => market.marketId === id);
  if (market) {
    Object.assign(selectedDetail, market);
    // selectedDetail.imageSrc = `data:image/png;base64,${selectedDetail.posterImg}`;
    dialogVisible.value = true;
  }
};

const formData = reactive({
  topic: '',
  start: '',
  end: '',
  detail: '',
  selectedOptions:'',
  img:''
});

const handleFileUpload = (event) => {
  const file = event.target.files[0];
  formData.img = file;
};

// const handleOptionChange = (values) => {
//   formData.selectedOptions = values;
// };

const message02 = ref('');
const addMarket = async () => {
  const putData = new FormData();
  putData.append('theme', formData.topic);
  putData.append('option', formData.selectedOptions);
  putData.append('startTime', formData.start);
  putData.append('endTime', formData.end);
  putData.append('detail', formData.detail);
  putData.append('posterImg', formData.img);
  putData.append('adminId', userId);

  console.log('邀请商家分类id'+formData.selectedOptions);

  try {
    const response = await axiosInstance.put('/Administrator/AddMarket', putData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    message02.value = response.data;
    fetchRecords();
  } catch (error) {
    if (error.response) {
      message02.value = error.response.data;
    } else if (error.request) {
      message02.value = '请求未收到响应';
    } else {
      message02.value = '提交失败';
    }
  }
  ElMessage.info(message02.value);
  console.log(message02.value);
};

const handleSubmit = () => {
  for (const key in formData) {
    if (formData[key] === '') {
      ElMessage.error('表单不可有空');
      return;
    }
  }
  if (formData.end <= formData.start) {
    ElMessage.error('结束时间不能早于开始时间');
    return;
  } else {
    addMarket();
    formData.topic = '';
    formData.start = '';
    formData.end = '';
    formData.detail = '';
    formData.img = '';
    formData.selectedOptions = [];
  }
};

const message03 = ref('');
const deleteMarket = async (marketId) => {
  try {
    const response = await axiosInstance.put('/Administrator/DeleteMarket', {
      "marketId": marketId,
    });
    message03.value = response.data;
    fetchRecords();
  } catch (error) {
    if (error.response) {
      message03.value = error.response.data;
    } else if (error.request) {
      message03.value = '请求未收到响应';
    } else {
      message03.value = '操作失败';
    }
  }
  ElMessage.info(message03.value);
  console.log(message03.value);
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
.main-container {
  background-color: #ffffff;
  padding: 0 10px;
  text-align: left;
  color: #000000;
}

.pagination-container {
  display: flex;
  justify-content: center;
}


</style>
  