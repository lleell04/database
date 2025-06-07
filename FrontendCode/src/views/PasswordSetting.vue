<!-- 设置界面的重置密码 -->
<template>
    <div class="container">
      <SettingSidebar />
      <div class="main-content">
        <SettingHeader />
        <hr>
        <div class="content">
          <div class="main-container">
            <h2>重置密码</h2>
            <div class="account-password">
                <el-card class="custom-card-width">
                    <el-form :model="password" label-width="80px" class="passwordForm">
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

                       
                        <el-form-item label="验证码" :rules="[{ required: true, message: '请输入验证码', trigger: 'blur' }]" >
                            <el-input v-model="verificationCode" placeholder="请输入收到的验证码"></el-input>
                        </el-form-item>

                        <div class="form-footer">
                            <el-form-item style="margin-right: 20px;">
                            <el-button type="primary" :disabled="isButtonDisabled" @click="getVerificationCode">
                            {{buttonText}}
                            </el-button>
                        </el-form-item>
                            <el-button type="primary" @click="updateAccountInfo">保存</el-button>
                        </div>
                    </el-form>
                </el-card>
                </div>
          </div>
          
        </div>
      </div>
    </div>
    
</template>
    
<script>
import SettingSidebar from '../components/SettingSidebar.vue'
import SettingHeader from '../components/SettingHeader.vue'
import eyeSvg from "@/assets/eye.svg"
import eyeInSvg from "@/assets/eyeIn.svg"
import { defineExpose } from 'vue';
import { reactive, ref, computed, onMounted  } from 'vue';
import { ElTable, ElTableColumn, ElPagination, ElButton, ElDialog, ElForm, ElFormItem, ElInput, ElRow, ElCol, ElSelect, ElOption, ElMessage } from 'element-plus';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

export default {
    name:'PasswordSetting',
    components:{
        SettingSidebar,
        SettingHeader
    },
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
            password: {
                new: '',
                confirm: ''
            },
            passwordVisibility: {
                new: false,
                confirm: false
            },
            businessInfo: {
                address: '',
            },
        };
    },
    computed: {
    currentImageTw() {
      return this.passwordVisibility.new ? eyeSvg : eyeInSvg;
    },
    currentImageTh() {
      return this.passwordVisibility.confirm ? eyeSvg : eyeInSvg;
    },
  },
    methods: {
        //获取验证码
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
        //更新密码
        async updateAccountInfo() {
            if ( this.password.new === '' || this.password.confirm === '') {
                this.$message.error('请填写所有必填项');
                return;
            };
            if (this.verificationCode !== this.serverVerificationCode) {
                this.$message.error('验证码错误');
                return;
            }

            await this.resetPassword();
        },
        async resetPassword() {
            const userId = localStorage.getItem('userId'); 
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
        //获取邮箱
        async getUserInfo(userId) {
            try {
                const response = await axiosInstance.get(`/Account/get_user_message/${userId}`);
                
                if (response.data.message === '用户查找成功！') { // 根据实际响应内容调整
                this.businessInfo = {
                    // username: response.data.target_user.useR_NAME,
                    // gender: response.data.target_user.gender,
                    email: response.data.target_user.email,
                    // address:response.data.target_user.address
                };
                
                // this.currentPass=response.data.target_user.password;
                } else {
                this.$message.error('获取用户信息失败');
                }
            } catch (error) {
                console.error('请求失败:获取商家信息', error);
                this.$message.error('请求失败，请稍后再试');
            }
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
         
         const userId = localStorage.getItem('userId');
         this.getUserInfo(userId);
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


 .passwordForm { 
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
.form-footer {
  display: flex;
  justify-content: flex-end;
}
  
</style>