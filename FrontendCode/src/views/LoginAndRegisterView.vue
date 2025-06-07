
<!-- npm install animate.css --save -->
<!-- el-put中，v-model用来双向绑定数据，clearable提供清除按钮，show-password可以调节可见性 -->
<template>
  <div class="base">
    <div class="loginAndRegist">
      <!-- 登录部分 -->
      <div class="loginArea" >
        <transition
                    name="animate__animated animate__bounce"
                    enter-active-class="animate__fadeInUp"
                    leave-active-class="animate__zoomOut"
                    appear
            >
            <div class="login_head" v-show="isLogin">
              <h2>瑕宝阁</h2>
              <p>专注于微瑕非物质文化遗产产品交易</p>
            </div>

        </transition>
        <transition
                    name="animate__animated animate__bounce"
                    enter-active-class="animate__fadeInUp"
                    leave-active-class="animate__zoomOut"
                    appear
            >
            <div class="loginForm" v-show="isLogin">
          <el-input 
          v-model="loginEmail" 
          placeholder="请输入邮箱" 
          type="email" 
          clearable 
          style="width: 215px;
          margin-left: 10px;
          height:40px;
          margin-bottom: 30px;"
        ></el-input>
          <el-input 
          v-model="password" 
          placeholder="请输入密码" 
          show-password 
          clearable 
          style="width: 215px;
          margin-left: 10px;
          height:40px;
          margin-bottom: 15px"
          @keyup.enter="login"
        ></el-input>
        <div>
          <label class="forget-password" @click="handleForgetPassword">忘记密码?</label>
          <!-- 两功能测试 -->
          <!-- <label class="forget-password" @click="show_protected">点击访问受保护数据</label> -->
          <!-- <label class="forget-password" @click="log_out">点击登出</label> -->

        </div>
          <el-button type="success" round 
          style="background-color: #82111f; 
          letter-spacing: 5px; 
          width: 150px; 
          margin-left: 10px;
          margin-top:20px;
          color:#FFFFFF" 
          @click="login">登录</el-button>
        </div>
         </transition>
      </div>
      <!-- 注册部分 -->
      <div class="registerArea">
        <transition
                    name="animate__animated animate__bounce"
                    enter-active-class="animate__fadeInUp"
                    leave-active-class="animate__zoomOut"
                    appear
            >
            <div class="registForm" v-show="!isLogin">
      <el-radio-group v-show="!isChangePsw" v-model="userType" style="margin-left: 10px;margin-bottom:0px">
        <el-radio label="买家">买家</el-radio>
        <el-radio label="商家">商家</el-radio>
      </el-radio-group>
      <!-- 用来调整位置的空白部分 -->
      <div v-show="isChangePsw">&nbsp;</div>
      <el-input 
        v-model="registerEmail" 
        placeholder="请输入邮箱" 
        clearable 
        type="email"
        style="width: 250px;margin-left: 10px;margin-top: 20px"
      ></el-input>
      <div >
      <el-input 
      v-model="verificationCode" 
      placeholder="请输入验证码" 
      clearable 
      style="width: 120px;margin-left: 10px;margin-top: 20px;"></el-input>
      <el-button type="success" round 
      :disabled="isButtonDisabled"
      style="background-color: #82111f; 
      /*letter-spacing调整间距 */
      font-size:auto;
      width: 120px; 
      margin-left: 10px;
      margin-top: 20px;
      " 
      @click="getVerificationCode">{{buttonText}}</el-button>
    </div>

      <el-input 
        v-model="newPassword" 
        placeholder="请输入新密码" 
        clearable 
        show-password
        style="width: 250px;margin-left: 10px;margin-top: 20px"
      ></el-input>
      <el-input v-show="!isChangePsw"
        v-model="confirmPassword" 
        placeholder="请再次输入新密码" 
        clearable 
        show-password
        style="width: 250px;margin-left: 10px;margin-top: 20px"
        @keyup.enter="register"
      ></el-input>
      <el-input v-show="isChangePsw"
        v-model="confirmPassword" 
        placeholder="请再次输入新密码" 
        clearable 
        show-password
        style="width: 250px;margin-left: 10px;margin-top: 20px"
        @keyup.enter="changPsw"
      ></el-input>

      <el-button v-show="!isChangePsw" type="success" round style="background-color: #82111f; letter-spacing: 5px; width: 250px; margin-left: 10px;margin-top: 30px" @click="register" >
        注册
      </el-button>
      <el-button v-show="isChangePsw" type="success" round style="background-color: #82111f; letter-spacing: 5px; width: 250px; margin-left: 10px;margin-top: 30px" @click="changPsw" >
        修改密码
      </el-button>
      </div>
          </transition>
      
     </div>
     <!-- 动态页面 -->
    <div id="aaa" class="showInfo"
              :style="{
             borderTopRightRadius:styleObj.bordertoprightradius,
             borderBottomRightRadius:styleObj.borderbottomrightradius,
             borderTopLeftRadius:styleObj.bordertopleftradius,
             borderBottomLeftRadius:styleObj.borderbottomleftradius,
             right:styleObj.rightDis
            }">
            <!-- 登录时的遮挡页 -->
            <div v-show="isLogin" class="button-container">
            <el-button type="success" 
            style="background-color:#f7f4ed; 
            /* border: 1px solid #ffffff; */
            color:#82111f;
            " round @click="changeToRegist">点击注册</el-button>
            </div>
           <!-- 注册或修改密码时的遮挡页 -->
            <div v-show="!isLogin" class="button-container" >
                <el-button type="success"  
                style="background-color:#f7f4ed;
                /* border: 1px solid #ffffff; */
                color:#82111f;
                " 
                round 
                @click="changeToLogin">返回登录</el-button>
           </div>
        </div>
     </div>
    </div>
</template>

<script setup>
import 'animate.css';
import { ref ,reactive} from 'vue';
import 'element-plus/dist/index.css';
import { ElInput, ElButton ,ElMessage} from 'element-plus';
import axiosInstance from '../components/axios';
import { useRouter } from 'vue-router';
const router = useRouter();
// 输入变量
const userType = ref('买家');
const loginEmail = ref('');
const registerEmail = ref('');
const password = ref('');
const newPassword=ref('');
const confirmPassword = ref('');
const verificationCode = ref('');
//后端返回验证码
const realVerificationCode=ref('');
//验证码倒计时相关
const countdownTime = 60;
const timeLeft = ref(countdownTime);//剩余时间
const buttonText = ref('获取验证码');
const isButtonDisabled = ref(false);
//控制变量
const isLogin=ref(true);
const isChangePsw=ref(false);//是否在修改密码
//el展示信息
const message = ref('');

// 设置遮挡页的四角和位置
const styleObj = ref({
  bordertoprightradius: '15px',
  borderbottomrightradius: '15px',
  bordertopleftradius: '0px',
  borderbottomleftradius: '0px',
  rightDis: '0px'
});
const validateEmail = (email) => {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(email);
};
// 重置变量
const clearData=()=>{
  userType.value='买家';
  loginEmail.value='';
  registerEmail.value='';
  password.value='';
  newPassword.value='';
  confirmPassword.value='';
  verificationCode.value='';
  realVerificationCode.value='';
}
///////////////////////通信最终版
 const login = async () => {
    if(!loginEmail.value||!password.value){
      ElMessage.error('请保证不为空');
    } 
    else if(!validateEmail(loginEmail.value)){
      ElMessage.error('邮箱格式不正确');
    }
    else{
      try {
        const response = await axiosInstance.post('/Account/login', {
          "username": loginEmail.value,
          "password": password.value,
        });
        message.value = response.data.message;
        ElMessage.success(message.value);
        console.log(`response.data.userId${response.data.userId}`);
        //本地存储用户id
        localStorage.setItem('userId',response.data.userId);
        localStorage.setItem('role',response.data.role);
        if(response.data.role=='买家'){
          router.push('/home');
        }else if(response.data.role=='商家'){
          router.push('/merchantpage');
        }else{
          router.push('/merchant-certification');
        }  
      } catch (error) {
        if (error.response) {
          message.value = error.response.data.message;
        } else {
          message.value = '登录失败';
        }
        ElMessage.error(message.value);
        }
        message.value='';
      }
 };
// //////////////////////////访问受保护数据测试
// const show_protected = async () => {
//   try {
//     const response = await axiosInstance.get('/Account/protected');
//     message.value = response.data.message;
//   } catch (error) {
//     if (error.response) {
//       message.value = error.response.data.message;
//     } else {
//       message.value = '访问受保护数据失败';
//     }
//   }
//   console.log(message.value);
// };
// ////////////////////////////////////////////登出功能测试，后续可加在别的地方
const log_out = async () => {
  try {
    const response = await axiosInstance.post('/Account/logout');
    message.value = response.data.message;
  } catch (error) {
    if (error.response) {
      message.value = error.response.data.message;
    } else {
      message.value = '登出账号失败';
    }
  }
  console.log(message.value);
};
const isRegistered=async()=>{
  console.log(`进入isRegistered`);
  try {
      const response = await axiosInstance.get(`/Account/check_register/${encodeURIComponent(registerEmail.value)}`);
      return false;
    } catch (error) {
      console.log('该邮箱已被注册');
    }
    message.value='';
    return true;
}
// 获取验证码前先检查该邮箱是否已经注册
// 根据当前在注册还是修改密码来决定是否可以发送验证码
const getVerificationCode= async () =>{
  if(isButtonDisabled.value) return;
  console.log(registerEmail.value);   
  if(!registerEmail.value){
    ElMessage.error('请保证邮箱不为空');
  }else{
    const isR=await isRegistered();
    const canSend =(isChangePsw.value==true? isR:!isR);
    console.log(`canSend is ${canSend}`);
    console.log(` isChangePsw is ${isChangePsw.value}`);
    if(!canSend&&isChangePsw.value==true){
      ElMessage.error('该邮箱尚未注册，请先注册！');
    }else if(!canSend&&isChangePsw.value==false){
      ElMessage.error('邮箱已存在！无法重复注册！');
    }else if(canSend){
      startCountdown();
      try {
        console.log(`开始发送验证码`);
        const response = await axiosInstance.get(`/Account/send_verification_code/${encodeURIComponent(registerEmail.value)}`);
        realVerificationCode.value=response.data.verificationCode;

        ElMessage.success('验证码发送成功');
      } catch (error) {
        if (error.response) {
          message.value = error.response.data;
        } else {
          message.value = '获取验证码失败，请检查邮箱后重试！';
        }
        ElMessage.error( '获取验证码失败，请检查邮箱后重试！');
      }
      message.value='';
    }
  }
}
const startCountdown = () => {
  isButtonDisabled.value = true; // 禁用按钮
  buttonText.value = `${countdownTime}s后再次发送`;
  timeLeft.value = countdownTime;

  const intervalId = setInterval(() => {
    timeLeft.value -= 1;
    buttonText.value = `${timeLeft.value}s后可再次发送`;

    if (timeLeft.value <= 0) {
      clearInterval(intervalId); // 清除定时器
      buttonText.value = '获取验证码';
      isButtonDisabled.value = false; // 启用按钮
    }
  }, 1000);
};

const register= async () =>{
  console.log('在注册');
  if(!registerEmail.value||!newPassword.value||!confirmPassword.value||!verificationCode.value){
        ElMessage.error('请保证不为空');
      } else if(!validateEmail(registerEmail.value)){
        ElMessage.error('邮箱格式不正确');
      }else if(newPassword.value!==confirmPassword.value){
        ElMessage.error("两次输入密码不一致")
      }else if(!verificationCode.value||verificationCode.value!==realVerificationCode.value){
        ElMessage.error("验证码错误");
      }else{
        try {
          const response = await axiosInstance.post('/Account/register',{
            'Email':registerEmail.value,
            'Password':newPassword.value,
            'Role':userType.value});
            message.value=response.data.message;
            ElMessage.success('注册成功');
            changeToLogin();
        } catch (error) {
          if (error.response) {
            message.value = error.response.data;
          } else {
            message.value = '注册失败';
          }
          ElMessage.error(message.value);
        }
        message.value='';
      }
}
const changPsw=async ()=>{
  console.log('在改密码');
  if(!registerEmail.value||!newPassword.value||!confirmPassword.value||!verificationCode.value){
        ElMessage.error('请保证不为空');
      } else if(!validateEmail(registerEmail.value)){
        ElMessage.error('邮箱格式不正确');
      }else if(newPassword.value!==confirmPassword.value){
        ElMessage.error("两次输入密码不一致")
      }else if(!verificationCode.value||verificationCode.value!==realVerificationCode.value){
        ElMessage.error("验证码错误");
      }else{
          try {
          const response = await axiosInstance.post('/Account/password_reset',{
            'username':registerEmail.value,
            'password':newPassword.value,
            });
            message.value=response.data.message;
            ElMessage.success(message.value);
            changeToLogin();
        } catch (error) {
          if (error.response) {
            message.value = error.response.data;
          } else {
            message.value = '重置密码失败';
          }
          ElMessage.error(message.value);
        }
        message.value='';

      }
}
//指场景的切换
const handleForgetPassword=()=>{
  isChangePsw.value=true;
  changeToRegist();
}
function changeToRegist() {
  styleObj.value.bordertoprightradius = '0px';
  styleObj.value.borderbottomrightradius = '0px';
  styleObj.value.bordertopleftradius = '15px';
  styleObj.value.borderbottomleftradius = '15px';
  styleObj.value.rightDis = '50%';
  isLogin.value = !isLogin.value;
  clearData();
}
function changeToLogin() {
  isChangePsw.value=false;
  styleObj.value.bordertoprightradius = '15px';
  styleObj.value.borderbottomrightradius = '15px';
  styleObj.value.bordertopleftradius = '0px';
  styleObj.value.borderbottomleftradius = '0px';
  styleObj.value.rightDis = '0px';
  isLogin.value = !isLogin.value;
  clearData();
}
</script>

<style scoped>
div {
  user-select: none;
  outline: none; 
  cursor: default; 
}
html, body {
  height: 100%;
  margin: 0;
  overflow: hidden; /* 禁用滚动条 */
}
.el-button:active{

  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2); /* 点击时的阴影效果 */
  transform: scale(0.95); /* 点击时缩小效果 */
}

:deep(.el-input__wrapper.is-focus) {
  box-shadow: 0 0 0 1px rgba(0,0,0,0.3);
}

:deep(.el-radio.is-checked .el-radio__inner) {
  border-color: hwb(353 7% 49%); /* 修改边框颜色 */
  background-color: #82111f; /* 修改选中颜色 */
}
:deep(.el-radio .el-radio__label) {
  color: #82111f; /* 修改文字颜色 */
  font-size: 16px; /* 修改文字大小 */
}
/* .el-radio__input.is-checked .el-radio__inner {
  border-color: #82111f; 
  background-color: #82111f; 
} */
.base {
  /* 华文宋 */
  font-family: 'Noto Serif SC', serif;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #D6CAC8 ; 
  background-image: url("../assets/mmy/background.jpg");
  background-size: cover; 
}
/* 背景覆盖层 */
.base::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background:rgba(189, 174, 173,0.5)  ; 
  z-index: 1; 
} 
.login_head h2{
    color:#82111f;
    margin-top:30px;
    margin-bottom:10px;
}
.login_head p{
  color:#82111f;
  padding-bottom:10px;
  border-bottom: 2px solid #82111f;
  margin-bottom:30px;
}
.title{
    width: 70%;
    height:25%;
    border-bottom: 1px solid #82111f;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #82111f;
    font-weight: bold;
    font-size: 24px;
    margin-bottom: 40px;
}
.forget-password{
  margin-left:150px;
  font-size: 13px; 
  cursor: pointer; 
  color:#82111f
}
.loginAndRegist {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
}

.loginArea, .registerArea {
  background-color: rgba(255, 255, 255, 0.8);
  height: 400px;
  width: 350px;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  overflow: hidden;
}
.loginArea {
  border-top-left-radius: 15px;
  border-bottom-left-radius: 15px;
}
.registerArea {
  border-top-right-radius: 15px;
  border-bottom-right-radius: 15px;
}
.registForm{
  margin-top: 30px;
}
.showInfo:hover{
    background-size: 100%;
    background-position: -50px -50px;
}
.showInfo{
    border-top-right-radius: 15px;
    border-bottom-right-radius: 15px;
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    height: 400px;
    width: 350px;
    z-index:2;
    top: 0;
    right: 0;
    background-image: url("../assets/mmy/red.jpg");
    background-size: 85%;
    /* 匀速过渡效果 */
    transition: 0.5s linear;
}
.showInfo:hover {
  background-size: 100%;
  background-position: -50px -50px;
}
.button-container {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
}
</style>
