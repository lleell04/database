<!-- 商家界面的订单管理的内容界面 -->
<template>
  <div class="CommodityShow">
    <div class="SearchContainer">
      <el-input v-model="searchOrder" placeholder="请输入订单ID（在全部订单中搜索）" style="display: inline-block;"></el-input>
      <el-button type="primary" @click="searchOrderById" >搜索</el-button>
      <el-input v-model="searchTime" placeholder="根据创建时间筛选（在全部订单中搜索）" style="display: inline-block;"></el-input>
      <el-button type="primary" @click="searchOrderByTime">筛选</el-button>
    </div>
    
    <!-- 表格 -->
    <div class="TableContainer">
      <el-table :data="currentPageData" class="CommodityTable" height="760">
        <el-table-column type="index" />
        <el-table-column prop="id" label="订单ID" width="150"></el-table-column>
        <el-table-column prop="creationTime" label="创建时间" width="150"></el-table-column>
        <el-table-column prop="orderStatus" label="订单状态" width="150"></el-table-column>
        <el-table-column prop="orderPrice" label="订单金额" width="150"></el-table-column>
        <el-table-column prop="practicalPrice" label="买家实付金额" width="150"></el-table-column>
        <el-table-column label="买家是否申请退货">
          <template #default="scope">
            <span>{{ scope.row.returnRequested ? '是' : '否' }}</span>
            <span class="space"></span>
            <el-button
              size="mini"
              :type="scope.row.returnStatus === '待同意' ? 'success' : 'primary'"
              :disabled="scope.row.returnStatus === '已同意' || !scope.row.returnRequested"
              @click="handleReturnRequest(scope.row)"
            >
              {{ scope.row.returnStatus }}
            </el-button>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="scope">
            <el-button-group>
              <el-button size="mini" type="primary" @click="handleCheck(scope.row)">买家评价</el-button>
              <el-button size="mini" type="primary" @click="handleCheckTwo(scope.row)">快递</el-button>
            </el-button-group>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 买家评价 -->
    <div v-if="dialogVisible" class="SettingPopUp">
      <div v-if="currentProduct" class="SettingContent">
        <span class="close" @click="dialogVisible = false">&times;</span>
        <p>买家评分: {{ currentProduct.score }}</p>
        <p>评价内容: {{ currentProduct.remark }}</p>
      </div>
    </div>
    

    <!-- 快递单号 -->
<div v-if="dialogVisibleTwo" class="SettingPopUp">
  <div v-if="currentProduct" class="SettingContent">
    <span class="close" @click="dialogVisibleTwo = false">&times;</span>
    <template v-if="currentProduct.deliveryNumber">
      <p>快递单号: {{ currentProduct.deliveryNumber }}</p>
    </template>
    <template v-else>
      <el-form @submit.prevent="updateDeliveryNumber">
        <el-form-item label="输入快递单号">
          <el-input v-model="currentProduct.deliveryNumberInput"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="updateDeliveryNumber">提交</el-button>
        </el-form-item>
      </el-form>
    </template>
  </div>
</div>

    <!-- 翻页 -->
    <div class="paginationContainer">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="totalProducts"
        layout="total, prev, pager, next, jumper"
        @current-change="handlePageChange"
      ></el-pagination>
    </div>
  </div>

</template>

<script>
import { ref, computed, onMounted } from 'vue';
import { ElSelect, ElOption, ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElButtonGroup, ElMessage } from 'element-plus';
import axiosInstance from '../components/axios';
import 'element-plus/dist/index.css';

export default {
  components: {
    ElSelect,
    ElOption,
    ElInput,
    ElButton,
    ElTable,
    ElTableColumn,
    ElPagination,
    ElButtonGroup
  },
  setup() {
    const selectedFilter = ref('');
    const dialogVisible = ref(false);
    const dialogVisibleTwo = ref(false);
    const currentProduct = ref(null);
    const orderStatus = ref(0);
    const searchOrder = ref('');
    const searchTime = ref('');
    const products = ref([]);

    //展示订单状态展示订单
    const fetchOrders = async () => {
      const storeId = localStorage.getItem('userId');  

      try {
        const response = await axiosInstance.get('/StoreOrder/GetOrders', {
          params: {
            storeId: storeId,
            orderStatus: orderStatus.value
          },
        });

        if (Array.isArray(response.data)) {
          const processedOrders = response.data
            .map(order => {

              // 确保字段存在并转换正确
              const creationTime = order.creatE_TIME ? new Date(order.creatE_TIME).toLocaleString() : 'N/A';
              let returnStatus = '';

              if (order.ordeR_STATUS === '待退货' && order.returN_OR_NOT) {
                returnStatus = '待同意';
              } else if (order.ordeR_STATUS === '已退货' && order.returN_OR_NOT) {
                returnStatus = '已同意';
              } else if (!order.returN_OR_NOT) {
                returnStatus = '待同意';
              }

              let orderStatusText = order.ordeR_STATUS;
              if (order.ordeR_STATUS === '已付款') {
                orderStatusText = '待发货';
              } else if (order.ordeR_STATUS === '待付款') {
                return null; // 过滤掉 "待付款" 的订单
              }

              return {
                id: order.ordeR_ID || 'N/A',
                creationTime: creationTime,
                orderStatus: orderStatusText || 'Unknown',
                orderPrice: order.totaL_PAY !== undefined ? order.totaL_PAY : 'N/A',
                practicalPrice: order.actuaL_PAY !== undefined ? order.actuaL_PAY : 'N/A',
                returnRequested: order.returN_OR_NOT != null ? order.returN_OR_NOT : false,
                returnStatus: returnStatus,
                remark: order.remark || 'No remarks',
                score: order.score !== undefined ? order.score : 'N/A',
                deliveryNumber: order.deliverY_NUMBER || '',
              };
            })
            .filter(order => order !== null); // 过滤掉 "待付款" 的订单

          console.log('Processed Orders:', processedOrders);
          products.value = processedOrders; // 确保 products 是 Vue 的响应式对象
        } else {
          console.error('Unexpected response format:', response.data);
        }
      } catch (error) {
        console.error('获取订单数据失败:', error);
      }
    };
    const handleChange = (viewTypeValue) => {
      orderStatus.value = viewTypeValue;
      fetchOrders();
    };
    //根据订单Id搜索订单
    const searchOrderById = () => {
      if (searchOrder.value.trim() !== '') {
        fetchOrderById(searchOrder.value.trim());
      } else {
        fetchOrders();
      }
    };
    const fetchOrderById = async (orderId) => {
      const storeId = localStorage.getItem('userId'); // 替换为实际的 storeId
      try {
        const response = await axiosInstance.get('/StoreOrder/GetOrderById', {
          params: {
            storeId: storeId,
            orderId: orderId
          }
        });

        if (response.data) {
          // 处理返回的订单数据
          const order = response.data;
          const creationTime = order.creatE_TIME ? new Date(order.creatE_TIME).toLocaleString() : 'N/A';
          let returnStatus = '';

          if (order.ordeR_STATUS === '待退货' && order.returN_OR_NOT) {
            returnStatus = '待同意';
          } else if (order.ordeR_STATUS === '已退货' && order.returN_OR_NOT) {
            returnStatus = '已同意';
          } else if (!order.returN_OR_NOT) {
            returnStatus = '待同意';
          }

          let orderStatusText = order.ordeR_STATUS;
          if (order.ordeR_STATUS === '已付款') {
            orderStatusText = '待发货';
          } else if (order.ordeR_STATUS === '待付款') {
            return null; // 过滤掉 "待付款" 的订单
          }

          const orderData = {
            id: order.ordeR_ID || 'N/A',
            creationTime: creationTime,
            orderStatus: orderStatusText || 'Unknown',
            orderPrice: order.totaL_PAY !== undefined ? order.totaL_PAY : 'N/A',
            practicalPrice: order.actuaL_PAY !== undefined ? order.actuaL_PAY : 'N/A',
            returnRequested: order.returN_OR_NOT != null ? order.returN_OR_NOT : false,
            returnStatus: returnStatus,
            remark: order.remark || 'No remarks',
            score: order.score !== undefined ? order.score : 'N/A',
            deliveryNumber: order.deliverY_NUMBER || 'N/A',
          };

          products.value = [orderData]; // 更新products为单个订单的数据
        } else {
          console.error('未找到订单');
        }
      } catch (error) {
        products.value = [];
        console.error('通过订单ID获取订单数据失败:', error);
      }
    };
    //根据创建时间筛选订单
    const searchOrderByTime = () => {
      if (searchTime.value.trim() !== '') {
        fetchOrderByTime(searchTime.value.trim());
      } else {
        fetchOrders();
      }
    };
    const fetchOrderByTime = async (date) => {
      const storeId =localStorage.getItem('userId');
      try {
        const response = await axiosInstance.get('/StoreOrder/GetOrdersByDate', {
          params: {
            storeId: storeId,
            date: date
          }
        });

        if (response.data && Array.isArray(response.data)) {
          const orders = response.data
            .map(order => {
              const creationTime = order.creatE_TIME ? new Date(order.creatE_TIME).toLocaleString() : 'N/A';
              let returnStatus = '';

              if (order.ordeR_STATUS === '待退货' && order.returN_OR_NOT) {
                returnStatus = '待同意';
              } else if (order.ordeR_STATUS === '已退货' && order.returN_OR_NOT) {
                returnStatus = '已同意';
              } else if (!order.returN_OR_NOT) {
                returnStatus = '待同意';
              }

              let orderStatusText = order.ordeR_STATUS;
              if (order.ordeR_STATUS === '已付款') {
                orderStatusText = '待发货';
              } else if (order.ordeR_STATUS === '待付款') {
                return null; // 过滤掉 "待付款" 的订单
              }

              return {
                id: order.ordeR_ID || 'N/A',
                creationTime: creationTime,
                orderStatus: orderStatusText || 'Unknown',
                orderPrice: order.totaL_PAY !== undefined ? order.totaL_PAY : 'N/A',
                practicalPrice: order.actuaL_PAY !== undefined ? order.actuaL_PAY : 'N/A',
                returnRequested: order.returN_OR_NOT != null ? order.returN_OR_NOT : false,
                returnStatus: returnStatus,
                remark: order.remark || 'No remarks',
                score: order.score !== undefined ? order.score : 'N/A',
                deliveryNumber: order.deliverY_NUMBER || 'N/A',
              };
            })
            .filter(order => order !== null); // 过滤掉 "待付款" 的订单

          products.value = orders; 
        } else {
          console.error('未找到订单');
        }
      } catch (error) {
        products.value = [];
        console.error('通过日期获取订单数据失败:', error);
      }
    };
    //查看买家评价
    const handleCheck = (row) => {
      currentProduct.value = row;
      dialogVisible.value = true;
    };
    //查看快递单号
    const handleCheckTwo = (row) => {
      currentProduct.value = { ...row, deliveryNumberInput: '' };
      dialogVisibleTwo.value = true;
    };
    //更新快递单号
    const updateDeliveryNumber = async () => {
      if (!currentProduct.value.deliveryNumberInput) {
        ElMessage({
          message: '请输入快递单号',
          type: 'warning'
        });
        return;
      }

      try {
        const storeId =localStorage.getItem('userId'); // 替换为实际的storeId
        const orderId = currentProduct.value.id;
        const deliveryNumber = currentProduct.value.deliveryNumberInput;

        // 发送请求到后端更新快递单号
        const response = await axiosInstance.put('/StoreOrder/UpdateDeliveryNumber', null, {
          params: {
            storeId: storeId,
            orderId: orderId,
            deliveryNumber: deliveryNumber
          }
        });

        if (response.status === 200) {
          ElMessage({
            message: '快递单号已更新',
            type: 'success'
          });
          currentProduct.value.deliveryNumber = deliveryNumber;
          dialogVisibleTwo.value = false;
          fetchOrders();
        } else {
          ElMessage({
            message: '更新快递单号失败',
            type: 'error'
          });
        }
      } catch (error) {
        console.error('更新快递单号失败:', error.response ? error.response.data : error.message);
        ElMessage({
          message: '更新快递单号失败: ' + error.message,
          type: 'error'
        });
      }
    };
    //同意退货请求
    const handleReturnRequest = async (row) => {
    if (row.returnRequested && row.returnStatus === '待同意') {
      try {
        const storeId = localStorage.getItem('userId'); // 替换为实际的 storeId 变量
        const orderId = row.id;

        // 发送请求到后端确认退货
        const response = await axiosInstance.put('/StoreOrder/ConfirmReturn', null, {
          params: {
            storeId: storeId,
            orderId: orderId,
          }
        });                               

        if (response.status === 200) {
          row.returnStatus = '已同意';
          ElMessage({
            message: '退货请求已成功确认。',
            type: 'success'
          });
          fetchOrders();
        } else {
          ElMessage({
            message: '退货请求确认失败',
            type: 'error'
          });
        }
      } catch (error) {
        console.error('确认退货时发生错误:', error.response ? error.response.data : error.message);
        ElMessage({
          message: '退货请求确认失败: ' + error.message,
          type: 'error'
        });
      }
    }
    };
    //翻页
    const pageSize = 20;
    const currentPage = ref(1);
    const totalProducts = computed(() => products.value.length);
    const currentPageData = computed(() => {
      const start = (currentPage.value - 1) * pageSize;
      const end = Math.min(start + pageSize, totalProducts.value);
      return products.value.slice(start, end);
    });
    const handlePageChange = (page) => {
      currentPage.value = page;
    };
    onMounted(() => {
      fetchOrders();
    });

    return {
      selectedFilter,
      dialogVisible,
      dialogVisibleTwo,
      currentProduct,
      orderStatus,
      searchOrder,
      products,
      handleCheck,
      handleCheckTwo,
      handleReturnRequest,
      pageSize,
      currentPage,
      totalProducts,
      currentPageData,
      handlePageChange,
      handleChange,
      orderStatus,
      searchOrderById,
      searchTime,
      searchOrderByTime,
      updateDeliveryNumber
    };
  }
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
  margin-left: 950px;
  display: flex;
  align-items: center;
  height: 60px;
  margin-right: 10px;
  /* background-color: rgb(164, 197, 181); */
}

.paginationContainer {
  display: inline-block;
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

.SettingContent {
  color: #065f43;
  background-color: #fefefe;
  display: inline-block;
  padding: 5vh;
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

.space {
  margin-left: 20px;
}

.el-button--primary {
    background-color: #a13232;
    border-color: #a13232;
}

.el-button--primary:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}
</style>