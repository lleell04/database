<!-- 论坛的消息中心 -->
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { ElMenu,ElMessage} from 'element-plus';
import 'element-plus/dist/index.css';
import axiosInstance from '../components/axios';
import router from '@/router';
const commentId=ref('');
const postId=ref('');
const afterReload=ref('');
afterReload.value = (localStorage.getItem('afterReload')!);
console.log("获取当前：",afterReload.value);
const option = ref<number>(afterReload.value !== null ? (Number(afterReload.value) || 1) : 1);
  console.log("实际当前：",option.value);
interface commentMessage {
  id:string;
  pic:string;
  author: string;
  time: string;
  content: string;
  postTitle:string;
  postId:string;
}
const way=ref('message');
const myCommentsMessage = ref<{
  comments: Array<commentMessage>
}>({
  comments:[]
});
const myCommentsMessageOld = ref<{
  comments: Array<commentMessage>
}>({
  comments:[]
});
const mySubCommentsMessage = ref<{
  comments: Array<commentMessage>
}>({
  comments:[]
});
const mySubCommentsMessageOld = ref<{
  comments: Array<commentMessage>
}>({
  comments:[]
});
const fetchComment = async () => {
  axiosInstance.get(`/Post/my_new_comments`)
    .then(response => {
      const data = response.data;
       if (data && data.data) {
        myCommentsMessage.value = {
          comments: data.data.map(comment => ({
            id:comment.commentId||'',
            pic:comment.authorPhoto.imageUrl,
            postId:comment.postId,
            author: comment.authorName || '匿名',
            time:comment.commentTime,
            content: comment.commentContent || '',
            postTitle:comment.postTitle||'',
          })),
        };
        console.log("成功获得评论");
      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      console.log("失败获得评论");
    });
}
fetchComment();
const fetchCommentOld = async () => {
  axiosInstance.get(`/Post/my_read_comments`)
    .then(response => {
      const data = response.data;
       if (data && data.data) {
        myCommentsMessageOld.value = {
          comments: data.data.map(comment => ({
            id:comment.commentId||'',
            pic:comment.authorPhoto.imageUrl,
            postId:comment.postId,
            author: comment.authorName || '匿名',
            time: comment.commentTime,
            content: comment.commentContent || '',
            postTitle:comment.postTitle||'',
          })),
        };
        console.log("成功获得评论过去");
      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      console.log("失败获得评论过去");
    });
}
fetchCommentOld();

interface likeMessage {
  id: string;
  time: string;
  postTitle:string;
}
const myLikeMessage = ref<{
  likes: Array<likeMessage>
}>({
  likes:[]
});
const fetchLike = async () => {
  axiosInstance.get(`/Post/my_liked_posts`)
    .then(response => {
      const data = response.data;
       if (data && data.data) {
        myLikeMessage.value = {
          likes: data.data.map(comment => ({
            id: comment.postId || '',
            time: comment.postReleaseTime,
            postTitle:comment.postTitle||'',
          })),
        };
        console.log("成功获得点赞");
      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      console.log("失败获得");
    });
}
fetchLike();
const myPosts = ref<Array<{ id: string, title: string, time: string ,like:string,comment:string,image:string}>>([]);
const getMyPost = async () => {
  axiosInstance.get(`/Post/get_my_posts`)
    .then(response => {
        const data = response.data;
        if (data && Array.isArray(data.target_posts)) {
            myPosts.value = data.target_posts.map(post => ({
                id: post.postId || '',
                title: post.postTitle || '',
                time: post.releaseTime,
                like: post.numberOfLikes || 0,
                comment: post.numberOfComments || 0,
                image:post.coverImageId.imageUrl,
            }));
            console.log("成功获得帖子");
        } else {
            console.error('Invalid data format.');
        }
    }).catch(error => {
        console.error(error);
    });
};
getMyPost();
function convertToReadableTime(isoTime) {
  if (!isoTime) return '';
  
  const date = new Date(isoTime);

  // 使用toLocaleString()格式化日期并根据需要自定义选项
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  });
}
const fetchSubComment = async () => {
  axiosInstance.get(`/Post/my_new_subcomments`)
    .then(response => {
      const data = response.data;
       if (data && data.data) {
        mySubCommentsMessage.value = {
          comments: data.data.map(comment => ({
            id:comment.commentId||'',
            pic:comment.authorPhoto.imageUrl,
            postId:comment.postId,
            author: comment.authorName || '匿名',
            time: comment.commentTime,
            content: comment.commentContent || '',
            postTitle:comment.commentedCommentId||'',
          })),
        };
        console.log("成功获得二级评论");
      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      console.log("失败获得评论");
    });
}
fetchSubComment();
const fetchSubCommentOld = async () => {
  axiosInstance.get(`/Post/my_read_subcomments`)
    .then(response => {
      const data = response.data;
       if (data && data.data) {
        mySubCommentsMessageOld.value = {
          comments: data.data.map(comment => ({
            id:comment.commentId||'',
            pic:comment.authorPhoto.imageUrl,
            postId:comment.postId,
            author: comment.authorName || '匿名',
            time: comment.commentTime,
            content: comment.commentContent || '',
            postTitle:comment.postTitle||'',
          })),
        };
        console.log("成功获得评论过去");
      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      console.log("失败获得评论过去");
    });
}
fetchSubCommentOld();
// const option=ref('news');

// function myNews(){
//      option.value='news';
// };
// function myLikes(){
//   option.value='likes';
// };
// function myComments(){
//   option.value='comments';
// };

// const button = reactive([
//   { id: 1, text: 'like', background: 'src/assets/czw/back.svg', backgroundColor: 'transparent' },

  
// ]);
// const changeButtonColor = (button, isHovered) => {
//   if (isHovered) {
//     if(button.id==1)
//     button.backgroundColor = '#87c2a5'; // 设置鼠标悬停时的背景颜色
//     else if(button.id==2)
//     button.backgroundColor = '#5169e6'; 
//   } else {
//     button.backgroundColor = 'transparent'; // 恢复背景颜色为透明
//   }
// };
// const buttonClick = (button) => {
//   if(button.id==1){
//     router.push('/forum'); // 跳转回 /forum 页面
//   }
// };
function markAsRead(postId,comment) {
 
  commentId.value=comment;
  console.log(commentId.value);
  isRead();
  postId.value=postId;
  viewPost(postId);
}
const isRead=async()=>{
  axiosInstance.post(`/Post/mark_comment/${ commentId.value}`).then(response => {
        console.log("成功已读");
        ElMessage({
        message: '已读',
        type: 'success',
        });
        // localStorage.setItem('afterReload', String(option.value));
        location.reload();
      }).catch(error => {
        console.log("失败已读");
      });
};
function viewPost(id,event)
{
  localStorage.setItem('postId', id);
  localStorage.setItem('way', way.value);
  localStorage.setItem('afterReload', String(option.value));
  router.push( 'viewpost'); // 跳转至 /viewpost 页面
};

const activeIndex = ref(option.value);
// const storedValue = localStorage.getItem('option');
// const option = ref(storedValue !== null ? parseInt(storedValue, 10) : 1);
const menuItems = ref([
  { index: 1, title: '我的帖子' },
  { index: 2, title: '我的点赞' },
  { index: 3, title: '回复我的帖子' },
  { index: 4, title: '回复我的评论' },
  { index: 5, title: '返回论坛' },

]);
const menuChange = (index:any) => {
  activeIndex.value = index.toString();
  option.value = parseInt(index);
  if(option.value==5){
    router.push('/forum'); // 跳转回 /forum 页面
  }
};

function deletePost(id,event)
{
  event.stopPropagation(); // 停止事件传播
  axiosInstance.delete('/Post/delete_post', {
      params: {
        postId:id
      }
   })
  .then(response => {
    console.log('删除:成功');
    ElMessage({
        message: '删除成功',
        type: 'success',
        });
    }).catch(error => {
      console.error('Error submitting return:', error);
    });
}
</script>

<template>
   <el-container>
    <el-aside width="18vh" style="background-color:  #82111f; ">
  <div class="big-title" style="display: flex; justify-content: center; align-items: center; width: 100%;">
    <img class="image" src="@/assets/czw/aside.svg" alt="Original Image"  />
    <span>消息</span>
    <img class="flipped-image" src="@/assets/czw/aside.svg" alt="Flipped Image" />
  </div>
  <div style="width: 100%;">
    <el-menu
      :default-active="activeIndex"
      mode="vertical"
      background-color="#ffffff"
      text-color="black"
      active-text-color="#d42517"
      @select="menuChange"
      style="width: 100%;"
    >
      <el-menu-item 
        v-for="item in menuItems" 
        :key="item.index" 
        :index="item.index.toString()"
        
      >
        <span class="el-icon-menu">{{ item.title }}</span>
      </el-menu-item>
    </el-menu>
  </div>
</el-aside>
   <el-container>
    <el-header style="text-align: left">
      <!-- <button :style="{ backgroundImage: `url(${button[0].background})`, 
        backgroundColor: button[0].backgroundColor }" @click="buttonClick(button[0])" class="back_button" 
        @mouseover="changeButtonColor(button[0], true)" @mouseout="changeButtonColor(button[0], false)"></button> -->
      <div v-if="option === 1" style="line-height: 6vh;">
        <span style="font-size: 2vh; color: #ffff; " >我的帖子</span>
      </div>
      <div v-if="option === 2" style="line-height: 6vh;">
        <span style="font-size: 2vh; color: #ffff;">我的点赞</span>
      </div>
      <div v-if="option === 3" style="line-height: 6vh;">
        <span style="font-size: 2vh; color: #ffff;">回复我的帖子</span>
      </div>
      <div v-if="option === 4" style="line-height: 6vh;">
        <span style="font-size: 2vh; color: #ffff;">回复我的评论</span>
      </div>
    </el-header>
    <el-menu>
      <div v-if="option === 1">
    <main class="main-content">
    <div class="post-display">
      <div v-for="(post, index) in myPosts" :key="index" class="post-item" @click="viewPost(post.id)">
        <img :src="post.image" alt="Post Image" class="post-image" />
        <a class="post-title">{{ post.title }}</a>
        <p>发布时间: {{ post.time }}</p>
        <el-button type="text" @click="deletePost(post.id,$event)" class="deleteButton">删除</el-button>
      </div>
    </div>
  </main>
      </div>

      <div v-if="option === 2">
        <main class="main-content">
        <div class="post-display">
      <div v-for="(like, index) in myLikeMessage.likes"  :key="index" class="post-item" @click="viewPost(like.id)">
        <a class="post-title">{{ like.postTitle }}</a>
        <p>点赞时间: {{ like.time }}</p>
      </div>
    </div>
  </main>
      </div>

      <div v-if="option === 3">
  <div class="container">
    <el-row>
      <el-col :span="24">
        <div v-for="(comment, index) in myCommentsMessage.comments" :key="index">
            <div class="avatar-container">
              <img :src="comment.pic" alt="user avatar" class="avatar">
            <span class="author">{{ comment.author }}</span>
            </div>
            <div class="comment-item">
            <div class="title" @click="markAsRead(comment.postId,comment.id)">回复了我的帖子：{{ comment.postTitle }}</div>
            <div class="comment-body">{{ comment.content }}</div>
            <div class="time">{{ comment.time }}</div>
          </div>
          <div class="bottom-border"></div>
        </div>
      </el-col>
    </el-row>
  </div>
  <div>
          <el-row class="dashed-line-container">
            <div class="dashed-line"></div>
            <div class="message-text">历史消息</div>
            <div class="dashed-line"></div>
          </el-row>
        </div>
        <div class="container">
    <el-row>
      <el-col :span="24">
        <div v-for="(comment, index) in myCommentsMessageOld.comments" :key="index"  >
          <div class="avatar-container">
            <img :src="comment.pic" alt="user avatar" class="avatar">
            <span class="author">{{ comment.author }}</span>
          </div>
          <div class="comment-item"> 
            <div class="title" @click="viewPost(comment.postId)">回复了我的帖子：{{ comment.postTitle }}</div>
            <div class="comment-body">{{ comment.content }}</div>
            <div class="time">{{ comment.time }}</div>
          </div>
          <div class="bottom-border"></div>
        </div>
      </el-col>
    </el-row>
  </div>
      </div>
      <div v-if="option === 4">
        <div class="container">
    <el-row>
      <el-col :span="24">
        <div v-for="(comment, index) in mySubCommentsMessage.comments" :key="index">
            <div class="avatar-container">
              <img :src="comment.pic" alt="user avatar" class="avatar">
            <span class="author">{{ comment.author }}</span>
            </div>
            <div class="comment-item">
            <div class="title" @click="markAsRead(comment.postId,comment.id)">回复了我的评论</div>
            <div class="comment-body">{{ comment.content }}</div>
            <div class="time">{{ comment.time }}</div>
          </div>
          <div class="bottom-border"></div>
        </div>
      </el-col>
    </el-row>
  </div>
  <el-row class="dashed-line-container">
            <div class="dashed-line"></div>
            <div class="message-text">历史消息</div>
            <div class="dashed-line"></div>
          </el-row>
        <div class="container">
    <el-row>
      <el-col :span="24">
        <div v-for="(comment, index) in mySubCommentsMessageOld.comments" :key="index">
            <div class="avatar-container">
              <img :src="comment.pic" alt="user avatar" class="avatar">
            <span class="author">{{ comment.author }}</span>
            </div>
            <div class="comment-item">
            <div class="title" @click="viewPost(comment.postId)">回复了我的评论</div>
            <div class="comment-body">{{ comment.content }}</div>
            <div class="time">{{ comment.time }}</div>
          </div>
          <div class="bottom-border"></div>
        </div>
      </el-col>
    </el-row>

      </div>
      </div>
  </el-menu>

  <el-main>


  </el-main>
</el-container>
</el-container>
    
</template>

<style scoped>
.avatar-container {
  margin-right: 1rem; /* 调整头像与内容之间的间距 */
  display: flex;
  flex-direction: row;
  margin-top: 1rem;
}

.avatar {
  width: 50px; /* 调整头像的大小 */
  height: 50px; /* 调整头像的大小 */
  border-radius: 50%; /* 使图像呈圆形 */
  object-fit: cover; /* 确保图像在容器内完整显示 */
}
.deleteButton{
  color: #82111f ;
}
.deleteButton:hover {
  color: #d42517; /* 可选：根据需要更改文字颜色 */
}
.author{
  font-size:large;
  /* color: #808080; */
  margin-top: 1vh;
  margin-left: 2vh;
}
.time{
  font-size: small;
  color: #808080;
}
.title{
  font-size: small;
  color: #82111f;
  cursor: pointer;
}
.container {
  width: 100vh; /* 你可以根据需要调整宽度 */
  margin: 0 auto; /* 居中显示 */
  padding: 10px; /* 内边距 */
  display: flex;
  flex-direction: column;
  align-items: start;
}
.comment-item div {
  display: flex;
  flex-direction: column;
  align-items: flex-start; /* 确保所有内容左对齐 */
}
.comment-item {
  padding: 10px 0;
}

.comment-body {
  font-size: 16px;
  margin: 5px 0;
}

.bottom-border {
  width: 100vh;
  border-bottom: 1px solid #ddd; /* 灰线颜色 */
  margin-top: 10px;
}
.dashed-line-container {
      display: flex;
      align-items: center;
      width: 95%;
      margin-top: 20px; /* Optional: Adjust the margin */
      margin-left: 4vh;
    }
    .dashed-line {
      flex-grow: 1;
      border-top: 1px dashed #000;
      position: relative;
      top: 1px;
    }
    .message-text {
      padding: 0 10px;
      background-color: white;
      font-size: 14px; /* Optional: Adjust font size */
      white-space: nowrap;
    }
.el-header {
    background-color: #82111f;
    color: #ffffff;
    line-height: 18vh;
    font-family: "华文中宋", sans-serif;
    font-size: larger;
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
.new-comment-dot {
  color: red; /* 设置红色点的样式 */
  margin-right: 5px; /* 距离评论内容一定距离 */
}
.image {
    width: 30px; /* 根据需求调整大小 */
    height: auto;
    /* transform: rotate(-90deg);*/
  }
  
  .flipped-image {
    width: 30px; /* 确保与第一张图一致 */
    height: auto;
    transform: scaleX(-1); /* 使用CSS进行垂直翻转 */

  }
  .big-title{
    font-size:4vh;  /* 将字体大小设置为32像素 */
    font-weight: bold; /* 可选：让文字加粗 */
    text-align: center; /* 可选：使文字水平居中 */
    margin: 20px 0; /* 可选：设置上下的外边距 */
    font-family: "华文中宋", sans-serif;
    color: #ffffff; /* 设置文字颜色为白色 */
  }
  .el-menu {
    position: relative;
    overflow: hidden;
  }
  
  .el-menu-item {
    position: relative;
    transition: background-color 0.3s ease;
  }
  
  .el-menu-item::before {
    content: '';
    position: absolute;
    left: 0;
    top: 0;
    width: 0;
    height: 100%;
    background-color: #bdaead;
    z-index: 0;
    transition: width 0.3s ease;
  }
  
  .el-menu-item:hover::before {
    width: 100%;
  }
  
  .el-menu-item > span {
    position: relative;
    z-index: 1; /* 确保文本始终在滑块上方 */
    transition: color 0.3s ease;
  }
  
  .el-menu-item:hover {
    color: #ffffff;
  }
  .el-icon-menu{
    font-size:2vh;
    font-weight: bold; /* 可选：让文字加粗 */  
    font-family: "华文中宋", sans-serif;
  }
  .main-content {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}

.main-content h1 {
  font-size: 24px;
  margin-bottom: 20px;
}
.post-title {
  color: #000; /* 默认标题颜色 */
  text-decoration: none;
  transition: color 0.2s ease; /* 动画过渡效果 */
  font-size: 20px;
}
.post-display {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.post-item {
  flex: 0 0 30%;
  background-color: #fff;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.post-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 10px rgba(0,0,0,0.15);
}

.post-image {
  width: 100%;
  height: auto;
  object-fit: cover;
  border-radius: 5px;
  margin-bottom: 10px;
}
.post-item p {
  font-size: 16px;
  margin: 0 0 10px;
}
</style>
