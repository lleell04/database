<!-- 设置界面的我的钱包 -->
<template>

     <!-- Loading 组件 -->
    <Loading v-show="isLoading" />

    <div class="container" v-show="!isLoading">
      <SettingSidebar />
      <div class="main-content">
        <SettingHeader />
        <hr> 
        <div class="content">
          <div class="main-container">
                <h2>我的余额</h2>
                <div class="wallet">
                    <el-card class="custom-card-width">
                        <p>您的钱包余额：￥{{ balance }}</p>
                    </el-card>
                </div>
                
                <h2>充值钱包</h2>
                <div class="wallet">
                    <el-card class="custom-card-width">
                        <el-form :model="recharge" label-width="80px" class="recharge-form">
                            <el-form-item label="充值金额" class="form-item">
                                <el-input v-model.number="recharge.amount" type="number" placeholder="请输入充值金额"></el-input>
                            </el-form-item>
                            <el-form-item >
                                <el-button type="primary" @click="rechargeBalance">充值</el-button>
                            </el-form-item>
                        </el-form>
                    </el-card>
                </div>
                
          </div>
          <div class="bottom">
          </div>
          
        </div>
      </div>
    </div>
    
</template>
    
<script>
import SettingSidebar from '../components/SettingSidebar.vue'
import SettingHeader from '../components/SettingHeader.vue'
import Loading from '../views/templates/4.vue';
import { reactive, ref, computed, onMounted  } from 'vue';
import { ElTable, ElTableColumn, ElPagination, ElButton, ElDialog, ElForm, ElFormItem, ElInput, ElRow, ElCol, ElSelect, ElOption, ElMessage } from 'element-plus';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

export default {
    name:'WalletSetting',
    components:{
        SettingSidebar,
        SettingHeader,
        Loading
    },
    data() {
        return {
            isLoading: false, // 控制加载状态
            recharge: {
                amount: 0
            },
            balance: 0,
        };
    },
    methods:{
        //获取钱包余额
        async getWalletBalance(userId) {
            try {
                const response = await axiosInstance.get('/Payment/GetWalletBalance', {
                params: {
                    accountID: userId
                }
                });

                if (typeof response.data === 'number') {
                    this.balance = response.data;
                } else {
                    this.$message.warning('未返回有效的余额信息');
                    this.balance = 0; 
                }
            } catch (error) {
                console.error('请求失败:钱包数值获取', error);
                this.$message.error('请求失败，请稍后再试');
            }
        },
        
        //充值钱包
        async rechargeBalance() {

            if (this.recharge.amount <= 0) {
                this.$message.error('充值金额必须大于零');
                return;
            }
            const userId = localStorage.getItem('userId'); // 获取当前用户 ID

            // 创建 FormData 对象
            const formData = new FormData();
            formData.append('BuyerId', userId); // 假设这是当前用户的 ID
            formData.append('Amount', this.recharge.amount);
            formData.append('returnUrl', 'https://localhost:7262/BuyerWallet');
            formData.forEach((value, key) => {
                console.log(key, value);
             });
                // 开始加载动画
                this.isLoading = true;
                this.$nextTick(() => {
                    console.log('Loading started');
                });
                // 刷新网页
                // window.location.reload();
                console.log('Loading started');
            try {
                const response = await axiosInstance.put('/Alipay/RechargeWallet', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
                });
                console.log(response.data);
                window.location.assign(response.data);

            } catch (error) {
                //检查是否重定向
                if (error.response&&error.response.status===302) {
                    const location=error.response.headers.location;
                    //手动处理重定向
                    window.location.href=location;
                } else {
                    console.error('error:',error.message);
                }
                // ElMessage.error(message.value);
            }
            finally {
            // 关闭加载页面
            this.recharge.amount = 0;
        }
        },
    },
    mounted() {
        const userId = localStorage.getItem('userId');  // 替换为实际的用户 ID
        if (userId) {
            this.getWalletBalance(userId);
        } else {
            this.$message.error('用户ID不存在,请登录后再试');
        }
  }
}
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
    color: #977c7c;
  }
  .container {
    display: flex;
    height: 100vh;
    /* background-color: rgb(248,210,210); */
    /* background-color: rgb(153,97,97); */
    background-color: rgb(237,227,228);
    /* background-color: rgb(239, 235, 235); */
  }
  
  .main-content {
    flex: 1;
    display: flex;
    flex-direction: column;
    color: #ffffff;
    
  }
  .content {
    flex: 1;
  }
  .main-container {
    text-align: left;
    color: #000000;
    margin-left: 140px;
    margin-top: 40px;
  }


.recharge-form {
  display: flex;
}

.form-item {
  flex: 1; 
}

 .wallet { 
  padding: 15px;
}

.el-button--primary {
    background-color: #a13232;
    border-color: #a13232;
}

.el-button--primary:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}
.custom-card-width {
    max-width: 1000px; /* 设置宽度为400px，您可以根据需要调整 */
    margin-left:0;
}
/* .bottom {
    width: 100%;
    height: 58%;
    background-color: #000000;
    background-image: url('@/assets/wy/back.webp');
    background-repeat: repeat;
    background-size: cover; 
  } */
</style>