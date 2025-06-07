<template>

  <div v-if="isUserCenterOpen" class="modal">
    <div class="modal-content">
      <!-- <span class="close" @click="closeModal">&times;</span> -->
      <!-- <UserCenter /> -->
       <div class="user-center">
        <!-- 添加右上角关闭按钮 -->
        <div class="close-btn" @click="closeUserCenter">&times;</div>
        <el-tabs v-model="activeTab">
          <el-tab-pane label="个人信息" name="userInfo">
            <div class="user-info">
              <el-form :model="userInfo" label-width="80px">
                <el-form-item label="用户名" :rules="[{ required: true, message: '请输入用户名', trigger: 'blur' }]">
                  <el-input v-model="userInfo.username" placeholder="请输入用户名"></el-input>
                </el-form-item>
                <el-form-item label="性别" :rules="[{ required: true, message: '请选择性别', trigger: 'change' }]">
                  <el-select v-model="userInfo.gender" placeholder="请选择性别">
                    <el-option label="男" value="male"></el-option>
                    <el-option label="女" value="female"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item label="年龄" :rules="[{ required: true, message: '请输入年龄', trigger: 'blur' }]">
                  <el-input v-model.number="userInfo.age" type="number" placeholder="请输入年龄"></el-input>
                </el-form-item>
                <el-form-item label="收货地址" :rules="[{ required: true, message: '请输入收货地址', trigger: 'blur' }]">
                  <el-input v-model="address.detail" placeholder="请输入收货地址"></el-input>
                </el-form-item>
                <el-form-item>
                  <el-button type="primary" @click="updateUserInfo">保存</el-button>
                </el-form-item>
              </el-form>
            </div>
          </el-tab-pane>

          <el-tab-pane label="消费积分" name="points">
            <div class="points">
              <el-card>
                <p>消费积分</p>
                <p>您当前的积分：{{ points }}</p>
              </el-card>
            </div>
          </el-tab-pane>

          <el-tab-pane label="钱包" name="wallet">
            <div class="wallet">
              <el-card>
                <h3>钱包</h3>
                <p>您的钱包余额：￥{{ balance }}</p>
                <el-form :model="recharge" label-width="80px">
                  <el-form-item label="充值金额">
                    <el-input v-model.number="recharge.amount" type="number" placeholder="请输入充值金额"></el-input>
                  </el-form-item>
                  <el-form-item>
                    <el-button type="primary" @click="rechargeBalance">充值</el-button>
                  </el-form-item>
                </el-form>
              </el-card>
            </div>
          </el-tab-pane>

          <el-tab-pane label="修改密码" name="account">
  <div class="account-info">
    <el-form :model="password" label-width="80px">
      <el-form-item label="新密码" :rules="[{ required: true, message: '请输入新密码', trigger: 'blur' }]">
        <el-input 
          v-model="password.new"
          :type="passwordVisibility.new ? 'text' : 'password'"
          placeholder="请输入新密码"
        >
          <template #suffix>
            <img 
              :src="currentImageTw"
              @click="toggleVisibilityTw()"
              class="password-visibility-toggle"
              alt="toggle visibility"
            />
          </template>
        </el-input>
      </el-form-item>
      <el-form-item label="确认密码" :rules="[{ required: true, message: '请确认新密码', trigger: 'blur' }]">
        <el-input 
          v-model="password.confirm"
          :type="passwordVisibility.confirm ? 'text' : 'password'"
          placeholder="请确认新密码"
        >
          <template #suffix>
            <img 
              :src="currentImageTh"
              @click="toggleVisibilityTh()"
              class="password-visibility-toggle"
              alt="toggle visibility"
            />
          </template>
        </el-input>
      </el-form-item>

      <!-- 添加发送验证码的部分 -->
      <el-form-item>
        <el-button type="primary" :disabled="isButtonDisabled" @click="getVerificationCode">
          {{buttonText}}
        </el-button>
      </el-form-item>
      <el-form-item label="验证码" :rules="[{ required: true, message: '请输入验证码', trigger: 'blur' }]">
        <el-input v-model="verificationCode" placeholder="请输入收到的验证码"></el-input>
      </el-form-item>
      
      <el-form-item>
        <el-button type="primary" @click="updateAccountInfo">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</el-tab-pane>

          <!-- 退出登录选项卡 -->
          <el-tab-pane label="退出登录" name="accountManagement">
            <div class="logout-section">
              <el-button type="danger" @click="logout">退出登录</el-button>
            </div>
          </el-tab-pane>


          <el-tab-pane label="头像和简介" name="userIn">
    <div>
      <el-form :model="userimades" :rules="rules" ref="form">
        <el-form-item label="上传头像(.jpg)" prop="image">
          <img v-if="userimades.ima" :src="userimades.ima" alt="当前图片" style="width: 200px; height: 200px;" />
          <input type="file" @change="handleFileChange" accept=".jpg" />
        </el-form-item>
        <el-form-item label="上传简介" prop="description">
          <el-input v-model="userimades.descri"></el-input>
        </el-form-item>
      </el-form>
      <el-button type="primary" @click="handleUpload">上传</el-button>
    </div>
  </el-tab-pane>
        </el-tabs>
      </div>
    </div>
  </div>

  
</template>

<script>
import eyeSvg from "@/assets/eye.svg"
import eyeInSvg from "@/assets/eyeIn.svg"
import axiosInstance from '../components/axios';
// import { ref } from 'vue';
import { ref, defineExpose } from 'vue';

export default {
  setup() {
    const isUserCenterOpen = ref(false);
    
    const closeUserCenter = () => {
      console.log("关闭按钮值",isUserCenterOpen)
      isUserCenterOpen.value = false;
      console.log("关闭按钮值",isUserCenterOpen)
    };

    // 使用 defineExpose 暴露属性和方法
    defineExpose({
      isUserCenterOpen,
      closeUserCenter
    });
    return { isUserCenterOpen, closeUserCenter };
  },
  data() {
    return {
    verificationCode: '',
    serverVerificationCode: '', // 用于存储后端返回的验证码
    isButtonDisabled: false,
    buttonText: '获取验证码',
    timeLeft: 0,
    countdownTime: 60, // 倒计时时间
      userimades:{
        ima:'',
        descri:'',
      },
      activeTab: 'userInfo',
      userInfo: {
        username: '',
        gender: '', 
        age: null 
      },
      points: 0,
      address: {
        detail: ''
      },
      balance: 0,
      password: {
        // current: '',
        new: '',
        confirm: ''
      },
      recharge: {
        amount: 0
      },
      currentPass:'',
      passwordVisibility: {
        current: false,
        new: false,
        confirm: false
      }
    };
  },
  computed: {
    currentImage() {
      return this.passwordVisibility.current ? eyeSvg : eyeInSvg;
    },
    currentImageTw() {
      return this.passwordVisibility.new ? eyeSvg : eyeInSvg;
    },
    currentImageTh() {
      return this.passwordVisibility.confirm ? eyeSvg : eyeInSvg;
    },
  },
  methods: {
    async getUserInfo(userId) {
      try {
        const response = await axiosInstance.get(`/Account/get_user_message/${userId}`);
        
        if (response.data.message === '用户查找成功！') {
          this.userInfo = {
            username: response.data.target_user.useR_NAME,
            gender: response.data.target_user.gender,
            age: response.data.target_user.age,
          };
          this.address = {
        detail: response.data.target_user.address
      };
        } else {
          this.$message.error('获取用户信息失败');
        }
      } catch (error) {
        console.error('请求失败:获取用户信息', error);
        this.$message.error('请求失败，请稍后再试');
      }
    },
    async updateUserInfo() {
      const userId = localStorage.getItem('userId');
  if (!this.userInfo.username || !this.userInfo.gender || this.userInfo.age === null || !this.address.detail) {
    this.$message.error('所有字段都必须填写');
    return;
  }

  // 验证年龄必须大于或等于0，并且是整数
  if (this.userInfo.age < 0 || !Number.isInteger(this.userInfo.age)) {
    this.$message.error('年龄必须是大于或等于0的整数');
    return;
  }

  try {
    const response = await axiosInstance.post('/Account/modify_buyer_message', {
      accountId: userId, // 替换为实际的 accountId
      userName: this.userInfo.username,
      gender: this.userInfo.gender,
      age: this.userInfo.age,
      address: this.address.detail
    });

    if (response.data.message === "用户信息修改成功！") { 
      this.$message.success('用户信息更新成功');
    } else {
      this.$message.error(`用户信息更新失败: ${response.data.detail}`);
    }
  } catch (error) {
    console.error('请求失败:', error); // 输出错误信息
    this.$message.error('请求失败，请稍后再试');
  }
},
async getWalletBalance(userId) {
  try {
    const response = await axiosInstance.get('/Payment/GetWalletBalance', {
      params: {
        accountID: userId
      }
    });

    console.log('API response:', response.data); // 输出完整的响应内容

    // 直接将返回的 response.data 视为余额
    if (typeof response.data === 'number') {
      this.balance = response.data;
    } else {
      this.$message.warning('未返回有效的余额信息');
      this.balance = 0; // 设置默认值为 0 或其他默认值
    }
  } catch (error) {
    console.error('请求失败:钱包数值获取', error);
    this.$message.error('请求失败，请稍后再试');
  }
},
async getBuyerCredits(userId) {
    try {
      // 调用接口获取买家消费积分
      const response = await axiosInstance.get('/Shopping/GetBuyerCredits', {
        params: {
          buyerId: userId // 替换为实际的买家账号ID
        }
      });

      // 输出响应内容以进行调试
      console.log('API response:', response.data);

      // 直接将返回的积分值赋给 points
      if (Number.isInteger(response.data)) {
        this.points = response.data;
      } else {
        this.$message.warning('未返回有效的积分信息');
      }
    } catch (error) {
      console.error('请求失败:获取消费积分', error);
      this.$message.error('请求失败，请稍后再试');
    }
  },
  async logout() {
      try {
        // 发送请求到后端接口进行退出登录
        const response = await axiosInstance.post('/Account/logout');

        // 在控制台输出响应内容以进行调试
        console.log('API response:', response.data);

        // 检查响应内容并反馈给用户
        if (response.data.message === '登出账号成功！') {
          this.$message.success('退出登录成功');
          localStorage.removeItem('userToken'); // 清除本地存储的用户信息
          this.$router.push('/loginandregister');
        } else {
          this.$message.error(`退出登录失败: ${response.data.message}`);
        }
      } catch (error) {
        console.error('请求失败:', error);
        this.$message.error('请求失败，请稍后再试');
      }
    },
    handleFileChange(event) {
  const file = event.target.files[0];
  if (file) {
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];
    if (validTypes.includes(file.type)) {
      const reader = new FileReader();
      reader.onload = (e) => {
        this.userimades.ima = e.target.result;
        console.log('Loaded image:', this.userimades.ima); // 输出图片数据
      };
      reader.readAsDataURL(file);
    } else {
      this.$message.error('请上传正确格式的图片 (.jpg 或 .png)');
    }
  }
},

async handleUpload() {
  try {
    if (!this.userimades.ima) {
      this.$message.error('请提供图片');
      return;
    }

    // 从图片数据中去除 Base64 前缀
    const Photo = this.userimades.ima.split(',')[1]; 
    const Describtion = this.userimades.descri;
    const Id = localStorage.getItem('userId'); 
    // const Id = 'U00000018'; 

    if (!Photo || !Describtion) {
      this.$message.error('请提供图片和认证资料');
      return;
    }

    const response = await axiosInstance.put('/UserInfo/SetPhotoAndDescribtion', {
      Id,
      Photo,
      Describtion,
    });

    if (response.status === 200) {
      this.$message.success('上传成功，请刷新网页以查看最新状态');

    } else {
      this.$message.error(`上传失败: ${response.data.message}`);
    }
  } catch (error) {
    console.error('请求失败:头像简介上传', error.response ? error.response.data : error.message);
    this.$message.error('请求失败，请稍后再试');
  }
},
async fetchImageAndText(id) {
  try {
    console.log(id,'!');
    const response = await axiosInstance.post('/UserInfo/GetPhotoAndDescribtion', {
      id
    });

    const { describtion, photo } = response.data;
    console.log('1:',this.userimades.ima);
    console.log('2:',this.userimades.descri);
    this.userimades.ima = `data:image/jpeg;base64,${photo}`;
    this.userimades.descri = describtion;

    console.log('获取到的头像和文字描述:', this.userimades);
  } catch (error) {
    console.error('获取头像和简介失败:', error);
    this.$message.error('获取头像和简介描述失败，请稍后再试');
  }
},
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

  try {
    const response = await axiosInstance.put('/Payment/RechargeWallet', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });

    if (response.status === 200 || response.data.success || response.data.code === 0) {
  this.$message.success('充值成功');
  this.balance += this.recharge.amount; // 充值成功后更新余额
} else {
  this.$message.error(`充值失败: ${response.data.message || '未知错误'}`);
}
  } catch (error) {
    console.error('请求失败:', error);
    this.$message.error('请求失败，请稍后再试');                               
  }

  this.recharge.amount = 0; // 重置充值金额
},

async getVerificationCode() {
    if (this.isButtonDisabled) return;

    if (!this.businessInfo.email) {
      this.$message.error('获取用户邮箱失败');
    } else {
      this.startCountdown();
      try {
        const response = await axiosInstance.get(`/Account/send_verification_code/${encodeURIComponent(this.businessInfo.email)}`);
        this.serverVerificationCode = response.data.verificationCode;
        this.$message.success('验证码已发送，请检查您的邮箱');
      } catch (error) {
        const message = error.response ? error.response.data : '获取验证码失败，请检查邮箱后重试！';
        this.$message.error(message);
      }
    }
  },
  startCountdown() {
    this.isButtonDisabled = true; // 禁用按钮
    this.buttonText = `${this.countdownTime}s后再次发送`;
    this.timeLeft = this.countdownTime;

    const intervalId = setInterval(() => {
      this.timeLeft -= 1;
      this.buttonText = `${this.timeLeft}s后可再次发送`;

      if (this.timeLeft <= 0) {
        clearInterval(intervalId); // 清除定时器
        this.buttonText = '获取验证码';
        this.isButtonDisabled = false; // 启用按钮
      }
    }, 1000);
  },
    async updateAccountInfo() {
  if (this.password.current === '' || this.password.new === '' || this.password.confirm === '') {
    this.$message.error('请填写所有必填项');
    return;
  };
  if (this.verificationCode !== this.serverVerificationCode) {
      this.$message.error('验证码错误');
      return;
    }
  // console.log(this.password.current);
  // console.log(this.currentPass);

  await this.resetPassword();
},
async resetPassword() {
  // if(this.password.current!=this.currentPass){
  //   this.$message.error('原密码不正确');
  //   return;
  // }
  const userId = localStorage.getItem('userId'); // 获取当前用户 ID
  if (this.password.new !== this.password.confirm) {
    this.$message.error('新密码和确认密码不匹配');
    return;
  }

  try {
    // 发送请求到后端重置密码
    const response = await axiosInstance.post('/Account/password_reset', {
      username:  userId,  // 使用当前用户的用户名
      password: this.password.new  // 使用新密码
    });

    // 在控制台输出响应内容以进行调试
    console.log('API response:', response);

    // 检查响应内容并反馈给用户
    if (response.data.message === "密码重置成功！") {
      this.$message.success('密码重置成功');
      this.password.current = '';
      this.password.new = '';
      this.password.confirm = '';
      this.verificationCode ='';
    } else {
      this.$message.error(`密码重置失败: ${response.data.message}`);
    }
  } catch (error) {
    console.error('请求失败:', error);
    this.$message.error('请求失败，请稍后再试');
  }
},
    toggleVisibility() {
      this.passwordVisibility.current = !this.passwordVisibility.current;
    },
    toggleVisibilityTw() {
      this.passwordVisibility.new = !this.passwordVisibility.new;
    },
    toggleVisibilityTh() {
      this.passwordVisibility.confirm = !this.passwordVisibility.confirm;
    },
    validateEmail(email) {
      const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@(([^<>()[\]\.,;:\s@"]+\.)+[^<>()[\]\.,;:\s@"]{2,})$/i;
      return re.test(email);
    }
  },
  mounted() {
    
    const userId = localStorage.getItem('userId');  // 替换为实际的用户 ID
    console.log('Stored User ID:', localStorage.getItem('userId'));
    if (userId) {
    this.getUserInfo(userId);
    this.getWalletBalance(userId);
    this.getBuyerCredits(userId); // 调用获取买家积分的方法
    this.fetchImageAndText(userId)
  } else {
    this.$message.error('用户ID不存在，请登录后再试');
  }
  }
};
</script>

<style scoped>
  .modal {
    display: flex;
    /* align-items: center; */
    justify-content: center;
    position: fixed;
    z-index: 1000;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    overflow: auto;
    background-color: rgba(0, 0, 0, 0.5);
  }
  
  .modal-content {
    margin-top: 200px;
    height: 45%;
    background-color: #fefefe;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  }

.password-visibility-toggle {
  cursor: pointer;
  width: 20px;
  height: 20px;
  margin-left: 10px;
}

.close-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  font-size: 40px;
  cursor: pointer;
  color: #333;
  margin-right: -30px;
  margin-top: -40px;
}

.user-center {
  /* height: 50%; */
  position: relative;
  padding: 20px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}
</style>