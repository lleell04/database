<!-- 管理员页面的举报管理 -->
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
        <div class="report-management">
          <h2>举报管理</h2>
          <div class='table-content'>
            <el-table :data="paginatedData">
                <el-table-column prop="type" label="举报类型" width="170"></el-table-column>
                <el-table-column prop="buyerAccountId" label="举报者账号" width="210"></el-table-column>
                <el-table-column prop="reportingTime" label="举报时间" width="330"></el-table-column>
                <el-table-column label="审核状态" width="170">
                  <template #default="scope">
                    <div v-if="scope.row.auditResults === null">
                      未审核
                    </div>
                    <div v-else>
                      {{ scope.row.auditResults }}
                    </div>
                  </template>
                </el-table-column>
                <el-table-column label="操作" width="400">
                <template #default="scope">
                  <el-button @click="search(scope.row.reportId)" type="primary">举报详情</el-button>
                  <el-button @click="auditReport(scope.row.reportId,'删除')" :disabled="scope.row.auditResults !== null" type="danger">删除</el-button>
                  <el-button @click="auditReport(scope.row.reportId,'忽略')" :disabled="scope.row.auditResults !== null" type="warning">忽略</el-button>
                </template>
              </el-table-column>
            </el-table>

            <el-dialog v-model="dialogVisible" title="举报详情" width="60%" >
              <div v-if="selectedDetail">
                <div v-if="selectedDetail.type=='帖子'">
                  <p>举报者账号: {{ selectedDetail.buyerAccountId }}</p>
                  <p>举报时间: {{ selectedDetail.reportingTime }}</p>
                  <p>举报原因: {{ selectedDetail.reportingReason }}</p>
                  <hr>
                  <p style="font-weight: bold; font-size:16px">帖子内容</p>
                  <p>帖子发布时间: {{ selectedDetail.postTime }}</p>
                  <p>帖子标题: {{ selectedDetail.postTitle }}</p>
                  <p>帖子内容: {{ selectedDetail.postContent }}</p>
                  <div v-if="selectedDetail.postImages" v-for="image in selectedDetail.postImages" :key="image.imageId">
                    <img :src="image.imageUrl" alt="Post Image" style="width: 70%; object-fit: cover;"/>
                  </div>
                </div>
                <div v-else>
                  <p>举报者账号: {{ selectedDetail.buyerAccountId }}</p>
                  <p>举报时间: {{ selectedDetail.reportingTime }}</p>
                  <p>举报原因: {{ selectedDetail.reportingReason }}</p>
                  <hr>
                  <p style="font-weight: bold; font-size:16px">评论内容</p>
                  <p>评论发布时间: {{ selectedDetail.postTime }}</p>
                  <p>评论内容: {{ selectedDetail.postContent }}</p>
                </div>
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
import { ElTable, ElTableColumn, ElPagination, ElButton, ElDialog, ElMessage, ElLoading } from 'element-plus';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

const userId =localStorage.getItem('userId');
const role=localStorage.getItem('role');

const records = reactive([]);
const message01 = ref('');

const fetchRecords = async () => {
  //加载缓冲
  const loadingInstance = ElLoading.service({
    lock: true,
    text: '正在加载...',
    target: '.table-content',
  });

  try {
    // const response = await axiosInstance.get('/Administrator/GetPostReport');
    // records.splice(0, records.length, ...response.data);

    //获取举报帖子数据
    const postReportsResponse = await axiosInstance.get('/Administrator/GetPostReport');
    const postReports = postReportsResponse.data.map(report => ({
      ...report,
      type: '帖子', //添加类型标识
      postTime: report.postTime.split('.')[0], //截断秒的小数部分
      reportingTime: report.reportingTime.split('.')[0]
    }));

    //获取举报评论数据
    const commentReportsResponse = await axiosInstance.get('/Administrator/GetCommentReport');
    const commentReports = commentReportsResponse.data.map(report => ({
      ...report,
      type: '评论',
      postTime: report.postTime.split('.')[0],
      reportingTime: report.reportingTime.split('.')[0]
    }));
    const allReports = [...postReports, ...commentReports];

    //排序
    allReports.sort((a, b) => {
      if (a.auditResults === null && b.auditResults !== null) return -1;
      if (a.auditResults !== null && b.auditResults === null) return 1;
      return 0;
    });
    records.splice(0, records.length, ...allReports);

    message01.value = '已获取举报记录数据';
    console.log(records.values);
  } catch (error) {
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
onMounted(() => {
  fetchRecords();
});

// 页面大小及当前页数
const pageSize = ref(8);
const currentPage = ref(1);

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

// dialog（详情窗口）相关变量
const dialogVisible = ref(false);
const selectedDetail = reactive({});

const search = (id) => {
  const report = records.find(report => report.reportId === id);
  if (report) {
    Object.assign(selectedDetail, report);
    dialogVisible.value = true;
  }
};

const message02 = ref('');
const auditReport = async (reportId,auditResult) => {
  try {
    const response = await axiosInstance.put('/Administrator/AuditReport', {
      "reportId": reportId,
      "auditResult": auditResult,
      "adminId": userId
    });
    message02.value = response.data;
    fetchRecords();
  } catch (error) {
    if (error.response) {
      message02.value = error.response.data;
    } else if (error.request) {
      message02.value = '请求未收到响应';
    } else {
      message02.value = '操作失败';
    }
  }
  ElMessage.info(message02.value);
  console.log(message02.value);
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

.report-management {
  background-color: #ffffff;
  padding: 0 10px;
  text-align: left;
}

.table-content {
  width: 100%;
}

.pagination-container {
  display: flex;
  justify-content: center;
}

.pagination {
  max-width: 100%;
}
</style>
