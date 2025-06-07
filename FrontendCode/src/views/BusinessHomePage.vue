<!-- 商家首页 -->
<template>
  <Sidebar />
  <div class="Background">
    <hr>
    <div class="BlockOne">
      <span class="BlockThree">
        <el-card>
          <div style="color: #977c7c;font-weight: bold;font-size: 1.2em;">待办订单</div>
          <hr>
          <el-card class="box-card" >
            <span class="Number">
              <el-statistic :value="stats.waitingForShipment" />
            </span>
          </el-card>
          <el-card class="box-card-two">
            <span class="Number">
              <el-statistic :value="stats.waitingForReturn" />
            </span>
          </el-card>
          <br>
          <span class="Frame">待发货</span>
          <span>待售后</span>
        </el-card>
      </span>
      <span class="BlockThree">
        <el-card>
          <div style="color: #977c7c;font-weight: bold;font-size: 1.2em;">相关市集</div>
          <hr>
          <el-card class="box-card" >
            <span class="Number">
              <el-statistic :value="stats.inCount" />
            </span>
          </el-card>
          <el-card class="box-card-two">
            <span class="Number">
              <el-statistic :value="stats.outCount" />
            </span>
          </el-card>
          <br>
          <span class="Frame">已参与</span>
          <span>可报名</span>
        </el-card>
      </span>
      <span class="BlockThree">
        <el-card>
          <div style="color: #977c7c;font-weight: bold;font-size: 1.2em;">今日总览</div>
          <hr>
          <el-card class="box-card" >
            <span class="Number">
              <el-statistic :value="stats.orderCount" />
            </span>
          </el-card>
          <el-card class="box-card-two">
            <span class="Number">
              <el-statistic :value="stats.totalRevenue" />
            </span>
          </el-card>
          <br>
          <span class="FrameTwo">今日新订单</span>
          <span>今日营业额</span>
        </el-card>
      </span>
    </div>
    
    <div class="BlockTwo">
      <el-card class="OrderStatistics">
        <div style="color: #977c7c;font-weight: bold;font-size: 1.2em;">订单统计</div>
        <hr>
        <OrderStatisticsChart :data="orderData" :labels="lastSevenDays" />
      </el-card>
    </div>
  </div>
</template>

<script>
import OrderStatisticsChart from './OverviewView.vue';
import axiosInstance from '../components/axios';
import Sidebar from './BusinessSidebar.vue'
export default {
  components: {
    OrderStatisticsChart,
    Sidebar
  },
  data() {
    return {
      orderData: [],
      lastSevenDays: this.getLastSevenDays(),
      stats: {
        waitingForShipment: 0,
        waitingForReturn: 0,
        inCount: 0,
        outCount: 0,
        orderCount: 0,
        totalRevenue: 0,
      },
    };
  },
  methods: {
    //获取（待办订单、相关市集、今日总览）
    async fetchOrderStats() {
      const storeId = localStorage.getItem('userId'); 
      const today = this.formatDate(new Date());

      try {
        const response = await axiosInstance.get('/StoreFront/GetOrderStats', {
          params: {
            storeId: storeId,
            date: today,
          },
        });
        // 更新组件数据
        this.stats = response.data;
      } catch (error) {
        console.error('Error fetching order stats:', error);
      }
    },
    //获取七天前的日期
    getLastWeekDate() {
    const date = new Date();
    date.setDate(date.getDate() - 6); // 设置为今天之前的第七天
    return this.formatDate(date); // 格式化日期
    },
    //获取折线图数据
    async fetchWeeklyOrderCount() {
      const storeId = localStorage.getItem('userId'); // 替换为实际的 storeId
      const lastWeekDate = this.getLastWeekDate();
      try {
        const response = await axiosInstance.get('/StoreFront/GetWeeklyOrderCount', {
          params: {
            storeId: storeId,
            date: lastWeekDate,
          },
        });

        // 更新订单数据
        this.orderData = response.data.map(item => item.count);
        this.lastSevenDays = this.getLastSevenDays(); // 更新日期标签
        console.log('Response data:', response.data);

      } catch (error) {
        console.error('Error fetching weekly order count:', error);
      }
    },
    //获取过去七天的日期
    getLastSevenDays() {
      const days = [];
      const today = new Date();

      for (let i = 0; i < 7; i++) {
        const date = new Date();
        date.setDate(today.getDate() - i);
        const formattedDate = this.formatDate(date);
        days.push(formattedDate);
      }

      return days.reverse(); // 反转数组使日期按顺序排列
    },
    //格式化日期格式
    formatDate(date) {
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    },
  },
  mounted() {
    this.fetchOrderStats(); // 组件挂载后请求数据
    this.fetchWeeklyOrderCount(); // 请求一周订单数据
  },
};
</script>

<style scoped>
.Background {
  background-color: rgb(237,227,228);
  position: fixed;
  top: 6vh;
  left: 150px; 
  right: 0;
  bottom: 0;
  overflow: auto;
}

.BlockOne {
  display: flex;
  padding-left: 7%;
  padding-top: 3%;
}

.BlockTwo {
  display: flex;
  width: 90%;
  padding-left: 8%;
  padding-top: 3%;
}

.BlockThree {
  color: black;
  width:22%;
  /* background-color: rgb(226, 233, 233); */
  margin-right: 5%;
  padding:15px;
}

.Frame {
  padding-right: 80px;
}
  
.FrameTwo {
  padding-right: 50px;
}

.OrderStatistics {
  width:90%;
  height: 450px;
  padding:15px;
  color: black;
  background-color: rgb(235, 236, 226);
}

.box-card {
  width: 35%;
  height: 50%;
  margin-top: 10%;
  background-color:  #dcc8ca  ;
  display: inline-block;
}

.box-card-two {
  width: 35%;
  height:  50%;
  margin-left:10%;
  background-color: #dcc8ca  ;
  display: inline-block;
}

.Number {
  display: flex;
  justify-content: center;
  align-items: center; 
  height: 120px;
}
</style>