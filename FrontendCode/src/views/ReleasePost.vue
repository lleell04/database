<!-- 论坛的发布帖子页面 -->
<template>
  <!-- <div class="container">
    <el-header>
      <button :style="{ backgroundImage: `url(${buttons[1].background})`, 
      backgroundColor: buttons[1].backgroundColor }" @click="buttonClick(buttons[1])" class="back_button" 
      @mouseover="changeButtonColor(buttons[1], true)" @mouseout="changeButtonColor(buttons[1], false)"></button>
      发布新帖
    </el-header>
    <el-input
type="text"
placeholder="请输入标题(不能为空)"
v-model="inputTitle"
maxlength="10"
show-word-limit
></el-input>
<div class="spacer"></div> 
  <el-input
type="textarea"
:autosize="{ minRows: 10, maxRows: 24}"
placeholder="请输入正文(不能为空)"
v-model="inputContent"
class="input_content">
</el-input>
</div>
<div>
    <input type="file" @change="handleFileUpload" multiple>
    <button :style="{ backgroundImage: `url(${buttons[2].background})`, 
      backgroundColor: buttons[2].backgroundColor }" @click="confirm()" class="upload_button" 
      @mouseover="changeButtonColor(buttons[2], true)" @mouseout="changeButtonColor(buttons[2], false)"></button>
    <div class="image-container">
      <div v-for="(file, index) in files" :key="index" >
        <img :src="getFileUrl(file)" class="image-preview">
      </div>
    </div>
  </div> -->
  <div class="post-container">
    <div class="header">
      <!-- <button :style="{ backgroundImage: `url(${buttons[1].background})`, 
      backgroundColor: buttons[1].backgroundColor }" @click="buttonClick(buttons[1])" class="back_button" ></button> -->
       <img class="back_button" src="@/assets/czw/back.svg" alt="back" @click="buttonClick(1)"  />
      <p>发布新帖</p>
    </div>
    <div class="content">
      <el-input
type="text"
placeholder="请输入标题(不能为空)"
v-model="inputTitle"
maxlength="15"
show-word-limit
></el-input>
<div class="spacer"></div> 
  <el-input
type="textarea"
:autosize="{ minRows: 10, maxRows: 24}"
placeholder="请输入正文(不能为空)"
v-model="inputContent"
class="input_content">
</el-input>
</div>
<!-- <div>
  <div class="upload-container">
    <div class="upload-button-container">
      <label>上传图片</label>
      <input type="file" @change="handleFileUpload" multiple>
    </div>
  </div>
  <div class="image-container">
    <div v-for="(file, index) in files" :key="index" class="image-wrapper">
      <img :src="getFileUrl(file)" class="image-preview">
      <span class="delete-button" @click="removeImage(index)">×</span>
    </div>
  </div>
</div> -->
<div class="image-upload-container">
    <div class="image-upload-grid">
      <div v-for="(file, index) in files" :key="index" class="image-preview">
        <img :src="getImageURL(file)" class="image-thumb">
        <span class="delete-button" @click="removeImage(index)">×</span>
      </div>
      <div class="upload-button-container" @click="triggerFileInput">
        <div class="center-content">
          <span class="plus-icon">+</span>
        </div>
      </div>
    </div>
    <input
      type="file"
      ref="fileInput"
      @change="handleFileUpload"
      multiple
      class="hidden-file-input" />
  </div>
<!-- <div class="publish-button-container">
      <el-button type="danger" @click="confirm()">发布</el-button>
    </div> -->
    <div class="publish-button-container">
  <el-button 
    :class="{'btn-custom': !isPublishing}"
    :type="isPublishing ? 'info' : 'danger'"
    :disabled="isPublishing"
    @click="confirm"
  >
    {{ isPublishing ? '发布中' : '发布' }}
  </el-button>
</div>
  </div>
</template>



<script setup lang="ts">
import { ref, reactive} from 'vue';
import { ElInput,ElMessage,ElButton } from 'element-plus';
import 'element-plus/dist/index.css';
import router from '@/router';
import axiosInstance from '../components/axios';

const inputTitle = ref('');
const inputContent = ref('');
const hover=ref(false);
const isPublishing=ref(false);

// const buttons = reactive([
//   { id: 1, text: 'Button 1', background: '@/assets/czw/picture+.svg', backgroundColor: 'transparent' },
//   { id: 2, text: 'Button 2', background: '@/assets/czw/back.svg', backgroundColor: 'transparent' },
//   { id: 3, text: 'Button 2', background: '@/assets/czw/confirm.svg', backgroundColor: 'transparent' },
// ]);


const files = ref([]);
const fileInput = ref(null);
const triggerFileInput = () => {
      if (fileInput.value) {
        fileInput.value.click();
      }
    };

const handleFileUpload = (event) => {
  const selectedFiles = event.target.files;
  for (let i = 0; i < selectedFiles.length; i++) {
    files.value.push(selectedFiles[i]);
  }
};
const removeImage = (index) => {
  files.value.splice(index, 1);
};
const getImageURL = (file) => {
  return URL.createObjectURL(file);
};

// const changeButtonColor = (button, isHovered) => {
//   if (isHovered) {
//     if(button.id==1)
//     button.backgroundColor = '#87c2a5'; // 设置鼠标悬停时的背景颜色
//     else if(button.id==2||button.id==3)
//     button.backgroundColor = '#5169e6'; 
//   } else {
//     button.backgroundColor = 'transparent'; // 恢复背景颜色为透明
//   }
// };

const buttonClick = (id) => {
  if(id==1){
    router.push('/forum'); // 跳转回 /forum 页面
  }
};
const confirm = async () => {
  if (!inputTitle.value) {
    ElMessage({
      message: '发布失败: 标题不能为空',
      type: 'error',
    });
    return;
  }
  isPublishing.value = true;
  const formData = new FormData();
      formData.append('PostTitle', inputTitle.value);
      formData.append('PostContent', inputContent.value);

      files.value.forEach((file, index) => {
        formData.append('Images', file); // 直接以 'Images' 为键添加文件到 FormData
    });


      axiosInstance.post('/Post/create_post', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      }).then(response => {
        console.log(response.data);
        ElMessage({
        message: '发布成功',
        type: 'success',
        });
        isPublishing.value = false;
        router.push('/forum'); 
      }).catch(error => {
        console.error(error);
      });
      console.log('上传图片:', files.value);
    
};
</script>


<style scoped>
.btn-custom {
  background-color: #82111f !important; /* 设置为你需要的颜色 */
  color: white !important; /* 设置文字颜色为白色 */
}
.image-upload-container {
  padding: 20px;
  margin-left: 14vh;
}

.image-upload-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.image-preview {
  width: 150px;
  height: 150px;
  border: 1px solid #e0e0e0;
  border-radius: 5px;
  overflow: hidden;
  position: relative;
}

.image-thumb {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.upload-button-container {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 150px;
  height: 150px;
  border: 1px dashed #c0c0c0;
  border-radius: 5px;
  cursor: pointer;
}

.upload-icon {
  font-size: 36px;
  color: #c0c0c0;
}

.hidden-file-input {
  display: none;
}
.image-wrapper {
  position: relative; /* 确保父容器是相对定位 */
  margin: 5px;
}

.delete-button {
  position: absolute;
  top: 5px;
  right: 5px;
  background: rgba(0, 0, 0, 0.5); /* 半透明背景以便更好显示 */
  color: white;
  border: none;
  border-radius: 50%; /* 圆形按钮 */
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}
.post-container {
  margin: 20px auto;
  width: 125vh;
  border: 1px solid #ddd;
  padding: 0px;
  border-radius: 10px; /* 圆角边框 */
  background: #fff;
}
.header {
  background-color: #82111f;
  color: #fff;
  padding: 10px;
  border-top-left-radius: 10px;
  border-top-right-radius: 10px;
  display: flex;
  align-items: center;
  font-size: larger;
}
.content{
  width:90vh;
  margin-top: 5vh;
  margin-left: 16vh;
  
}
.upload_button {
  width: 5vh;
  height:5vh;
  /* 按钮样式 */
  border: none;
  cursor: pointer;
  /* 其他样式 */
  background-repeat: no-repeat;
  background-size: cover;
  transition: background-color 0.3s ease; /* 添加过渡效果 */
  background-size: 100% 100%; /* 调整背景图像的尺寸 */
}
.spacer {
  height: 15px; /* 设置间隔的高度 */
}
.upload{
  margin-right: auto;
  margin-left: 25vh;
}
.el-header {
    background-color: #B3C0D1;
    color: #333;
    line-height: 60px;
    font-size: 5vh;
  }
.back_button {
      float: left;
      width: 5vh;
      height:5vh;
      background-repeat: no-repeat;
      background-size: cover;
      transition: background-color 0.3s ease; /* 添加过渡效果 */
      background-size: 100% 100%; /* 调整背景图像的尺寸 */
      border: none;
}
.image-preview {
  max-width: 200px;
  /* margin-top: 10px; */
}
.image-container {
    display: flex;
    flex-wrap: wrap; /* Allow images to wrap to the next line if there are many */
    gap: 10px; /* Add some space between images */
    margin-left: 16vh;
  }
.upload-container {
    display: flex;
    align-items: center;
    margin-top: 5vh;
    margin-left: 16vh;
  }
  .upload-button-container {
    display: flex;
    align-items: center;
  }
  .upload-button-container input[type="file"] {
    margin-left: 8px; /* Optional: Adjust the spacing between label and input */
  }
  .image-container {
    margin-top: 20px;
  }
  .publish-button-container {
    display: flex;
    justify-content: flex-end; /* Aligned to the right */
    margin-top: 20px;
    margin-right: 18vh;
    margin-bottom: 5vh;
  }
  .center-content {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  width: 100%;
}

.plus-icon {
  font-size: 24px;
  color: #A9A9A9;
}

</style>
