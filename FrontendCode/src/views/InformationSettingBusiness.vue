<!-- 设置界面的账号信息商家 -->
<template>
    <div class="container">
      <SettingSidebar />
      <div class="main-content">
        <SettingHeader />
        <hr>
        <div class="content">
          <div class="main-container">
        
                <h2>店铺基础信息管理</h2>
                <div class="user-info">
                    <el-card class="custom-card-width">
                        <el-form :model="businessInfo" label-width="80px" class="user-form">
                            <el-form-item label="店铺名" :rules="[{ required: true, message: '请输入店铺名', trigger: 'blur' }]">
                                <el-input v-model="businessInfo.username" placeholder="请输入店铺名"></el-input>
                            </el-form-item>
                            <el-form-item label="商家地址" :rules="[{ required: true, message: '请输入商家地址', trigger: 'blur' }]">
                                <el-input v-model="businessInfo.address" placeholder="请输入商家地址"></el-input>
                            </el-form-item>
                            <div class="form-footer">
                                <el-button type="primary" @click="updateBusinessInfo">保存</el-button>
                            </div>
                        </el-form>
                    </el-card>
                </div>

                <h2>店铺头像简介上传</h2>
                <div class="user-info">
                    <el-card class="custom-card-width">
                        <el-form :model="userimades"  ref="form" class="user-form">
                            <el-form-item label="上传头像" prop="image">
                                <img v-if="userimades.ima" :src="userimades.ima" alt="当前图片" style="width: 40px; height: 40px;border-radius: 50%;" />
                                <input type="file" @change="handleFile" accept="image/*" />
                            </el-form-item>
                            <el-form-item 
                                label="上传简介" 
                                prop="description"
                                :rules="[
                                    { required: true, message: '请输入简介', trigger: 'blur' },
                                    { validator: validateDescription, trigger: 'blur' }
                                ]">
                                <el-input v-model="userimades.descri" maxlength="20" show-word-limit></el-input>
                            </el-form-item>
                        </el-form>
                        <div class="form-footer">
                            <el-button type="primary" @click="handleUpload">上传</el-button>
                        </div>
                    </el-card>
                </div>

                <h2>店铺认证资料管理</h2>
                    <div class="user-info">
                        <el-card class="custom-card-width">
                            <div v-if="certificationStatus === '请求上传'">
                                <el-form :model="certificationUp" ref="form" class="user-form">
                                    <el-form-item label="认证资料图片上传(.jpg)" prop="image">
                                    <div v-for="(image, index) in certificationUp.images" :key="index" style="margin-bottom: 10px; position: relative; display: inline-block;">
                                        <img :src="image" alt="当前图片" style="width: 40px; height: 40px; display: block;margin-right: 8px" />
                                        <span class="close" @click="removeImage(index)">&times;</span>
                                    </div>
                                    <input type="file" @change="handleFileChange" accept=".jpg,.png" />
                                    </el-form-item>
                                    <el-form-item label="认证资料文字描述" prop="description">
                                    <el-input v-model="certificationUp.description"></el-input>
                                    </el-form-item>
                                </el-form>
                                <div class="form-footer">
                                    <el-button type="primary" @click="handleCertificationUpload">上传认证资料(图片和文字)</el-button>
                                </div>
                            </div>
                            <div v-else-if="certificationStatus === '正在审核'">
                            <p>您的认证资料正在审核中，请稍等。</p>
                            </div>
                            <div v-else-if="certificationStatus === '审核通过'">
                            <p>您的认证资料已通过审核。</p>
                            <div v-for="(image, index) in certificationUp.images" :key="index" style="margin-bottom: 2px;">
                                <img :src="image" alt="认证图片" style="width: 40px; height: 40px;" />
                            </div>
                            <p>相关文字描述：{{ certificationUp.description }}</p>
                            </div>
                        </el-card>
                    </div>

            </div>
        </div>
      </div>
    </div>
    
</template>
    
<script>
import SettingSidebar from '../components/SettingSidebarbusiness.vue'
import SettingHeader from '../components/SettingHeader.vue'
import { ElButton, ElForm, ElFormItem, ElInput} from 'element-plus';
import { Close } from '@element-plus/icons-vue';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';

export default {
    name:'InformationSettingBusiness',
    components:{
        SettingSidebar,
        SettingHeader,
        Close,
    },
    data() {
        return {
            //认证资料
            certificationStatus: '请求上传', // '请求上传', '正在审核', '审核通过'
            certificationUp: {
                imageFiles: [], // 存储多个 File 对象
                images: [], // 存储多个图片的 URL
                description: '', // 认证资料描述
            },
            passStatus:false,
            rules: {
                image: [
                { required: true, message: '请提供认证图片', trigger: 'change' }
                ],
                description: [
                { required: true, message: '请提供认证文字描述', trigger: 'blur' }
                ]
            },
            //基础信息
            businessInfo: {
                username: '',
                address: '',
                email: ''
            },
            //头像简介
            userimades:{
                ima:'',
                descri:'',
            },
            loading: false,
            error: null,
        };
    },
    created() {
    this.updateBusinessInfo();  
   },
    methods: {
        //处理认证资料上传
        handleFileChange(event) {
            const file = event.target.files[0];
            if (file) {
                const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];
                if (validTypes.includes(file.type)) {
                // 添加 File 对象到数组
                this.certificationUp.imageFiles.push(file);
                // 生成并添加图片 URL 到数组
                this.certificationUp.images.push(this.generateImageURL(file));
                } else {
                this.$message.error('请上传正确格式的图片 (.jpg 或 .png)');
                }
            } else {
                this.$message.error('未选择任何文件');
            }
        },
        //处理前端图片的展示
        generateImageURL(file) {
            return URL.createObjectURL(file);
        },
        //处理头像上传
        handleFile(event) {
            const file = event.target.files[0];
            if (file) {
                console.log('选中的文件:', file);
                this.userimades.file = file;
                this.userimades.ima = URL.createObjectURL(file);
                console.log('userfile',file,'userima',this.userimades.ima);
            } else {
                console.log('文件选择失败或无效');
            }
        },
        //获取头像简介
        async fetchImageAndText(id) {
            try {
                console.log(id,'!');
                const response = await axiosInstance.post('/UserInfo/GetPhotoAndDescribtion', {
                id
                });

                const { describtion, photo } = response.data;
                console.log('1:',this.userimades.ima);
                console.log('2:',this.userimades.descri);
                this.userimades.ima = photo.imageUrl;
                this.userimades.descri = describtion;

                console.log('获取到的头像和文字描述:', this.userimades);
            } catch (error) {
                console.error('获取头像和简介失败:', error);
                this.$message.error('获取头像和简介描述失败，请稍后再试');
            }
        },
        //上传头像简介
        async handleUpload() {
    try {


        const formData = new FormData();
        const Photo = this.userimades.file;
        const Describtion = this.userimades.descri;
        const Id = localStorage.getItem('userId');

        if (this.userimades.file) {
            formData.append('Photo', Photo); // 确保这是 File 对象
        }
        formData.append('Id', Id);
        formData.append('Describtion', Describtion);
        

        // 打印 FormData 内容
        for (const pair of formData.entries()) {
            console.log(`${pair[0]}:`, pair[1]);
        }

        // const response = await axiosInstance.put('/UserInfo/SetPhotoAndDescribtion', formData);

        const response = await axiosInstance.put('/UserInfo/SetPhotoAndDescribtion',formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });

        if (response.status === 200) {
            this.$message.success('上传成功，请刷新网页以查看最新状态');
        } else {
            this.$message.error(`上传失败: ${response.data.message}`);
        }
    } catch (error) {
        console.error('请求失败: 头像简介上传', error.response ? error.response.data : error.message);
        this.$message.error('请求失败，请稍后再试');
    }
},
       // 上传认证资料
        async handleCertificationUpload() {
        try {
            if (!this.certificationUp.imageFiles.length) {
            this.$message.error('请至少上传一张图片');
            return;
            }

            const storeId = localStorage.getItem('userId');
            if (!storeId || !this.certificationUp.description) {
            this.$message.error('请提供图片和认证资料');
            return;
            }

            const formData = new FormData();

            // 循环添加所有图片文件到 FormData
            this.certificationUp.imageFiles.forEach((file) => {
            formData.append('Photo', file); // 改成 'Photo'
            });

            formData.append('Authentication', this.certificationUp.description); // 添加认证资料描述
            formData.append('storeId', storeId); // 将 storeId 作为 body 参数传递

            const response = await axiosInstance.post('/StoreFront/SubmitAuthentication', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
            });

            if (response.status === 200) {
            this.$message.success('认证资料上传成功，请刷新网页以查看最新状态');
            } else {
            this.$message.error(`上传失败: ${response.data.message}`);
            }
        } catch (error) {
            console.error('认证资料上传失败', error);
            this.$message.error('请求失败，请稍后再试');
        }
        },
        removeImage(index) {
        this.certificationUp.imageFiles.splice(index, 1);  // 删除对应的 File 对象
        this.certificationUp.images.splice(index, 1);  // 删除对应的图片 URL
        },
        //获取商家信息
        async getUserInfo(userId) {
            try {
                const response = await axiosInstance.get(`/Account/get_user_message/${userId}`);
                
                if (response.data.message === '用户查找成功！') { // 根据实际响应内容调整
                this.businessInfo = {
                    username: response.data.target_user.useR_NAME,
                    // gender: response.data.target_user.gender,
                    email: response.data.target_user.email,
                    address:response.data.target_user.address
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
        //更新商家信息
        async updateBusinessInfo() {
            const storeId = localStorage.getItem('userId'); 
            this.loading = true;
            this.error = null;
            
            // 确保用户名和地址不为空
            if (!this.businessInfo.username || !this.businessInfo.address) {
                // this.$message.error('所有字段都必须填写');
                return;
            }
            const requestBody = {
                accountId: storeId || null, // 如果 storeId 不存在，传递 null
                userName: this.businessInfo.username || null, // 如果 username 不存在，传递 null
                storeName: this.businessInfo.username || null, // 如果 username 也作为 storeName 使用
                address: this.businessInfo.address || null // 如果 address 不存在，传递 null
            };


            try {
                const response = await axiosInstance.post('/Account/modify_seller_message', requestBody, {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                console.log('Update response:', response.data); // 打印响应数据
                this.$message.success('商家信息更新成功');
            } catch (error) {
                this.$message.error('更新商家信息失败');
                console.error('Error updating business info:', error);
            } finally {
                this.loading = false;
            }
        },
        //获取商家信息
        // async fetchStoreName() {
        //     const storeId = localStorage.getItem('userId'); 
        //     this.loading = true;
        //     this.error = null;
        //     this.storeScoreName = null;

        //     try {
        //         const response = await axiosInstance.get('/StoreFront/UpdateStoreScore', {
        //         params: { storeId }
        //         });
        //         this.businessInfo.username = response.data.storeName;
        //     } catch (error) {
        //         this.error = 'Failed to fetch store score';
        //         console.error('Error fetching store score:', error);
        //     } finally {
        //         this.loading = false;
        //     }
        // },
        //获取认证资料状态
        async checkCertificationStatus() {
            const storeId =  localStorage.getItem('userId');  // 替换为实际的 storeId
            try {
                const response = await axiosInstance.get('/StoreFront/UpdateStoreAuth', {
                params: {
                    storeId
                }
                });

                this.passStatus = response.data; // 获取返回的认证状态
                console.log('认证状态:', this.passStatus); // 输出认证状态
                if(this.passStatus==0){
                this.certificationStatus='请求上传'
                }
                else if(this.passStatus==1){
                this.certificationStatus='正在审核'
                }
                else if(this.passStatus==3){
                this.certificationStatus='审核通过'
                await this.fetchCertificationImageAndText(storeId);
                }
                else if(this.passStatus==2){
                this.certificationStatus='请求上传'
                this.$message.warning('管理员拒绝了你的请求，请重新上传认证材料');
                }
            } catch (error) {
                console.error('获取认证状态失败:', error);
                this.$message.error('获取认证状态失败，请稍后再试');
            }
        },
        // 获取认证资料图片和文字描述
        async fetchCertificationImageAndText(storeId) {
            try {
                const response = await axiosInstance.get('/StoreFront/GetStoreAuthImg', {
                    params: { storeId }
                });

                const { authentication, photo } = response.data;
                // 处理 photo 中的 imageUrl 字段
                this.certificationUp.images = [photo.imageUrl]; // 存储图片 URL 到数组
                this.certificationUp.description = authentication; // 存储认证资料文字描述

                console.log('获取到的认证图片和文字描述:', this.certificationUp);
            } catch (error) {
                console.error('获取认证图片和文字描述失败:', error);
                this.$message.error('获取认证图片和文字描述失败，请稍后再试');
            }
        }
    },
    mounted() {          
        const userId = localStorage.getItem('userId');
        this.getUserInfo(userId);
        // this.fetchStoreName();
        this.checkCertificationStatus(); 
        this.fetchImageAndText(userId);
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


 .user-info { 
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

.close {
  position: absolute;
  top: -8px;
  right: 0px;
  color: red;
  font-size: 20px;
  cursor: pointer;
}
  
</style>