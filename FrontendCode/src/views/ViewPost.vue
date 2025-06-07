<!-- 论坛的查看具体帖子页面 -->
<script setup lang="ts">
import { ref, reactive,} from 'vue';
import {  ElButton, ElInput, ElCarousel, ElCarouselItem, ElImage,ElRow,ElCol,ElMessage, } from 'element-plus';
import 'element-plus/dist/index.css';
import router from '@/router';
import 'animate.css';
import axiosInstance from '../components/axios';
import Loading from '../views/templates/4.vue';


// 假设这些数据是从服务器获取的
const dialogVisible =ref(false);
const inputComment = ref('');
const postIdString = localStorage.getItem('postId');
const way = localStorage.getItem('way');
const postId=ref('');
const subId=ref('');
const reason=ref('');
const reason_else=ref('');
const makeCommentId=ref('');
const isLoading=ref(true);
console.log('postIdString:', postIdString);
console.log('way:', way);
let allSubComments = [];
// 检查获取的值是否是有效的 JSON 字符串
if (postIdString) {
  try {
    postId.value = JSON.parse(JSON.stringify(postIdString)); // 将数字转换为字符串再解析
    console.log("成功ID");
  } catch (error) {
    console.error('Error parsing JSON:', error);
  }
}
interface Comment {
  id: string;
  pic:string;
  author: string;
  time: string;
  content: string;
  isSubMakeComment: boolean,
  subCollapsed: boolean,
  dialogVisible_sub:boolean,
  subComments:Array<subComment>,
}
const post = ref<{
  title: string,
  author: string,
  content:string,
  time: string,
  liked:boolean,
  isMakeComment: boolean,
  collapsed: boolean,
  likeCount: string,
  commentCount: string,
  reason: string,
  reason_else: string,
  comments: Array<Comment>,
  images: Array<string>
}>({
  title: '',
  author: '',
  content:'',
  time: '',
  liked:false,
  isMakeComment: false,
  collapsed: false,
  likeCount: '0',
  commentCount: '0',
  reason: '',
  reason_else: '',
  comments: [],
  images: [],
});
interface subComment {
  fatherId:string;
  id: string;
  pic:string;
  author: string;
  time: string;
  content: string;
}
const subComments = ref<{
  sub: Array<subComment>
}>({
  sub:[]
});
const message = ref('');
function fetchPost (){
  console.log("正在获取");
  axiosInstance.get(`/Post/get_a_post/${ postId.value}`)
    .then(response => {
      const data = response.data;
      if (data && data.data) {
        const postData = data.data;
        const images = postData.images || [];
        post.value =reactive( {
          title: postData.postTitle || '',
          author: postData.authorName || '匿名',
          content: postData.postContent || '',
          time: postData.releaseTime,
          liked:data.liked||false,
          isMakeComment: false,
          collapsed: true,
          likeCount: postData.numberOfLikes || '0',
          commentCount: postData.numberOfComments || '0',
          reason: '', // 初始化为''
          reason_else: '', // 初始化为''
          comments: postData.comments.map(comment => ({
            id: comment.commentId || '',
            pic:comment.authorPhoto.imageUrl,
            author: comment.authorName || '匿名',
            time:comment.commentTime,
            content: comment.commentContent || '',
            isSubMakeComment: false,
            subCollapsed: true,
            dialogVisible_sub:false,
          })),
          images: images.map(image => image.imageUrl) // 提取每个图片的 URL
        });
         // 恢复 collapsed 状态
         const state = localStorage.getItem('state');
        if (state !== null) {
          post.value.collapsed = (state === 'true');
        }

        console.log("成功获取");
       
        // 恢复每个 comment.subCollapsed 状态
        const commentsState = JSON.parse(localStorage.getItem('commentsState') || '{}');
        post.value.comments.forEach(comment => {
          if (commentsState[comment.id] !== undefined) {
            comment.subCollapsed = commentsState[comment.id];
          }
        });
        getSub();
        
        console.log("加载完毕");

      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      message.value = "失败";
    })
    .finally(() => {
          isLoading.value = false; // 更新加载状态，无论成功或失败都会执行这一步
    });
}

fetchPost(); 
function fetchComment () {
  console.log("正在获取二级");
  axiosInstance.get(`/Post/get_sub_comments/${ subId.value}`)
    .then(response => {
      const data = response.data;
       if (data && data.data) {
        subComments.value = {
          sub: data.data.map(comment => ({
            fatherId: comment.commentedCommentId,
            id:comment.commentId,
            pic:comment.authorPhoto.imageUrl,
            author: comment.authorName || '匿名',
            time: comment.commentTime || '',
            content: comment.commentContent || '',
          })),
        };
        console.log("成功获得二级评论");
        allSubComments.push(...subComments.value.sub);
        console.log(allSubComments);
        
      } else {
        console.error('Invalid data format.');
      }
    })
    .catch(error => {
      message.value = "失败二级";
    });
}
function getSub()
{
  post.value.comments.forEach(comment => {
    subId.value=comment.id;
    fetchComment();
  });
}
const toggleReplies = (post) => {
  post.collapsed = !post.collapsed;
};
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
function reply() {
  post.value.isMakeComment = true;
}
function replySub(id) {
  let found = false;
  post.value.comments.forEach(comment => {
    if (!found && comment.id === id) {
      comment.isSubMakeComment = true;
      found=true; 
    }
  });
}
function submitReply (){
  post.value.isMakeComment=false;
  const formData = new FormData();
      formData.append('postId', postId.value);
      formData.append('commentContext', inputComment.value);
      axiosInstance.post('/Post/add_comment_to_post', formData, {
      }).then(response => {
        ElMessage({
        message: '评论成功',
        type: 'success',
        });
        console.log("评论成功");
        // location.reload();
        localStorage.setItem('state', String(post.value.collapsed));
      const commentsState = post.value.comments.reduce((acc, comment) => {
        acc[comment.id] = comment.subCollapsed;
        return acc;
      }, {});
      localStorage.setItem('commentsState', JSON.stringify(commentsState));
        fetchPost();
        
        inputComment.value='';
      }).catch(error => {
        console.error(error);
      });
}
function submit(id){
  let found = false;
  post.value.comments.forEach(comment => {
    if (!found && comment.id === id) {
      comment.isSubMakeComment = false;
      found=true; 
    }
  });
  makeCommentId.value=id;
  submitSubReply();
};
function submitSubReply(){
  const formData = new FormData();
      formData.append('commentId', makeCommentId.value);
      formData.append('commentContext', inputComment.value);
      axiosInstance.post('/Post/add_comment_to_comment', formData, {
      }).then(response => {
        ElMessage({
        message: '评论成功',
        type: 'success',
        });
        console.log("评论成功");
        // location.reload();
        localStorage.setItem('state', String(post.value.collapsed));
      const commentsState = post.value.comments.reduce((acc, comment) => {
        acc[comment.id] = comment.subCollapsed;
        return acc;
      }, {});
      localStorage.setItem('commentsState', JSON.stringify(commentsState));
        fetchPost();
        inputComment.value='';
      }).catch(error => {
        console.error(error);
      });
}
function cancel() {
  post.value.isMakeComment = false;
}
function cancelSub(id) {
  let found = false;
  post.value.comments.forEach(comment => {
    if (!found && comment.id === id) {
      comment.isSubMakeComment = false;
      found=true; 
    }
  });
}
function resetForm() {
  post.value.reason = '';
  post.value.reason_else = '';
}
function submitReason(){
  const formData = new FormData();
      formData.append('postId',postId.value );
      if (post.value.reason=="其他") {
        if(post.value.reason_else!='')
        formData.append('reportReason', post.value.reason_else);
        else{
          ElMessage({
        message: '不能为空',
        type: 'error',
        });
        return;
        }
      } else if(post.value.reason!='') {
        formData.append('reportReason', post.value.reason);
      }
      else{
        ElMessage({
        message: '不能为空',
        type: 'error',
        });
        return;
      }
      axiosInstance.post('/Post/report_post', formData, {
      }).then(response => {
        ElMessage({
        message: '举报成功',
        type: 'success',
        });
        resetForm(); 
        dialogVisible.value=false;
        console.log(response.data);

      }).catch(error => {
        console.error(error);
      });
}
function submitReplyReason(reply){
  const formData = new FormData();
      formData.append('commentId',reply.id );
      if (post.value.reason!="其他") {
        formData.append('reportReason', post.value.reason);
      } else  {
        formData.append('reportReason', post.value.reason_else);
      }
      axiosInstance.post('/Post/report_comment', formData, {
      }).then(response => {
        ElMessage({
        message: '举报成功',
        type: 'success',
        });
        resetForm(); 
        reply.dialogVisible_sub=false;
        console.log(response.data,"评论举报");
      }).catch(error => {
        console.error(error);
      });
}
// const button = reactive([
//   { id: 1, text: 'like', background: likeB, backgroundColor: 'transparent' },
//   { id: 2, text: 'like', background: replyB, backgroundColor: 'transparent' },
//   { id: 3, text: 'like', background: backB, backgroundColor: 'transparent' },
//   { id: 4, text: 'liked', background: likedB, backgroundColor: 'transparent' },
//   { id: 5, text: 'liked', background: showReplyB, backgroundColor: 'transparent' },
//   { id: 6, text: 'liked', background: hideReplyB, backgroundColor: 'transparent' },
// ]);
function like(){
  if(post.value.liked==false){
  const formData = new FormData();
      formData.append('postId',postId.value );
      axiosInstance.post('/Post/like_post', formData, {
      }).then(response => {
        console.log(response.data);
        ElMessage({
        message: '点赞成功',
        type: 'success',
        });
        // location.reload();
        localStorage.setItem('state', String(post.value.collapsed));
      const commentsState = post.value.comments.reduce((acc, comment) => {
        acc[comment.id] = comment.subCollapsed;
        return acc;
      }, {});
      localStorage.setItem('commentsState', JSON.stringify(commentsState));
        fetchPost();
      }).catch(error => {
        console.error(error);
      });
      post.value.liked=true;
  }
  else{
    const formData = new FormData();
      formData.append('postId',postId.value );
      axiosInstance.post('/Post/unlike_post', formData, {
      }).then(response => {
        console.log(response.data);
        ElMessage({
        message: '取消点赞',
        type: 'success',
        });
        // location.reload();
        localStorage.setItem('state', String(post.value.collapsed));
      const commentsState = post.value.comments.reduce((acc, comment) => {
        acc[comment.id] = comment.subCollapsed;
        return acc;
      }, {});
      localStorage.setItem('commentsState', JSON.stringify(commentsState));
        fetchPost();
      }).catch(error => {
        console.error(error);
      });
      post.value.liked=false;
  }
}

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
const buttonClick = (id) => {
  if(id==3&&way=='forum'){
    router.push('/forum'); // 跳转回 /forum 页面
  }
  if(id==3&&way=='message'){
    router.push('/messageview'); // 跳转回 /forum 页面
  }
};
function toggleSubComments(fatherId) {
  let found = false;
  post.value.comments.forEach(comment => {
    if (!found && comment.id === fatherId) {
      comment.subCollapsed = !comment.subCollapsed;
      found=true; 
    }
  });
};
function getSubComments(fatherId) {
      return allSubComments.filter(comment => comment.fatherId === fatherId);
};
function hasSubComments(fatherId) {
    return allSubComments.some(comment => comment.fatherId === fatherId);
};
function highlightPost(event) {
  // event.currentTarget.style.backgroundColor = ' #bdaead ';;
};
function resetPost(event) {
      event.currentTarget.style.backgroundColor = 'initial';
}
const handleChange = (currentIndex) => {
      console.log("Current carousel item index:", currentIndex);
    };

</script>

<template>
   <Loading v-show="isLoading" />
  <div v-show="!isLoading" class="big-container">
    <div class="header">
      <!-- <button :style="{ backgroundImage: `url(${button[2].background})`, 
      backgroundColor: button[2].backgroundColor }" @click="buttonClick(button[2])" class="back_button" ></button> -->
      <img class="back_button" src="@/assets/czw/back.svg" alt="back" @click="buttonClick(3)"  />
      <p>帖子详情</p>
    </div>
    <el-main>
    <div class="post-container" v-if="post.images.length > 0">
    <el-row :gutter="130">
      <el-col :span="12">
        <div v-if="post.images.length > 0">
        <el-carousel  @change="handleChange"> 
          <el-carousel-item v-for="image in post.images" :key="image">
            <div class="image-container">
            <el-image 
              style="width: 100%; height: 100%;"
              :src="image"
              alt="示例图片"
              fit="contain"></el-image></div>
          </el-carousel-item>
        </el-carousel>
        </div>
      </el-col>
      <el-col :span="12">
        <div class="post-content">
          <div class="fixed-text-area">
          <p class="post-title">{{post.title}}</p>
          <p class="post_content">{{post.content}}</p>
        </div>
          <div class="post-details">
            <div class="author">
            <span>{{ post.time }}</span>
            <div class="post-actions">
            <!-- <button v-if="post.liked==false" :style="{ backgroundImage: `url(${button[0].background})`, 
        backgroundColor: button[0].backgroundColor }" class="like_button" @click="like()"></button> -->
        <img v-if="post.liked==false" class="like_button" src="@/assets/czw/like.svg" alt="like post"  @click="like()"  />
        <!-- <button v-if="post.liked" :style="{ backgroundImage: `url(${button[3].background})`, 
        backgroundColor: button[3].backgroundColor }" class="like_button" @click="like()"></button>{{ post.likeCount }} -->
        <img  v-if="post.liked" class="like_button" src="@/assets/czw/liked.svg" alt="liked post"  @click="like()"  />{{ post.likeCount }} 
        <!-- <button :style="{ backgroundImage: `url(${button[1].background})`, 
        backgroundColor: button[1].backgroundColor }" class="like_button" @click="reply()"></button>{{ post.commentCount }} -->
        <img class="like_button" src="@/assets/czw/reply.svg" alt="liked post"  @click="reply()"  />{{ post.commentCount }}
          </div>
        </div>
            <div class="author">
            <span>作者:{{ post.author }}</span>
             <span>
            <el-button type="text" @click="dialogVisible = true" class="buttons">举报</el-button></span></div>
            <el-dialog
:draggable="true"
  v-model="dialogVisible" title="举报详情" width="460px" @close="resetForm">
  <el-select v-model="post.reason" placeholder="请选择原因">
        <el-option label="违反法律法规" value="违反法律法规"></el-option>
        <el-option label="不实信息" value="不实信息"></el-option>
        <el-option label="侵犯个人权益" value="侵犯个人权益"></el-option>
        <el-option label="扰乱社区环境" value="扰乱社区环境"></el-option>
        <el-option label="其他" value="其他"></el-option>
      </el-select>
      <div style="margin-top: 10px;"></div>
      <el-input v-if="post.reason === '其他' "v-model="post.reason_else" placeholder="请输入原因"></el-input>
  <el-divider></el-divider>
  <div slot="footer" class="dialog-footer">
    <el-button type="primary" @click="submitReason()">确 定</el-button>
    </div>
</el-dialog>
</div>
        </div>
      </el-col>
    </el-row>
  </div>



  <div class="post-container" v-else>
    <el-row :gutter="10">
      <el-col :span="24">
        <div class="post-content">
          <div class="fixed-text-area">
          <p class="post-title">{{post.title}}</p>
          <p class="post_content">{{post.content}}</p>
        </div>
          <div class="post-details">
            <div class="author">
            <span>{{ post.time }}</span>
            <div class="post-actions">
            <!-- <button v-if="post.liked==false" :style="{ backgroundImage: `url(${button[0].background})`, 
        backgroundColor: button[0].backgroundColor }" class="like_button" @click="like()"></button> -->
        <img v-if="post.liked==false" class="like_button" src="@/assets/czw/like.svg" alt="like post"  @click="like()"  />
        <!-- <button v-if="post.liked" :style="{ backgroundImage: `url(${button[3].background})`, 
        backgroundColor: button[3].backgroundColor }" class="like_button" @click="like()"></button>{{ post.likeCount }} -->
        <img  v-if="post.liked" class="like_button" src="@/assets/czw/liked.svg" alt="liked post"  @click="like()"  />{{ post.likeCount }} 
        <!-- <button :style="{ backgroundImage: `url(${button[1].background})`, 
        backgroundColor: button[1].backgroundColor }" class="like_button" @click="reply()"></button>{{ post.commentCount }} -->
        <img class="like_button" src="@/assets/czw/reply.svg" alt="liked post"  @click="reply()"  />{{ post.commentCount }}
          </div>
        </div>
            <div class="author">
            <span>作者:{{ post.author }}</span>
             <span>
            <el-button type="text" @click="dialogVisible = true" class="buttons">举报</el-button></span></div>
            <el-dialog
:draggable="true"
  v-model="dialogVisible" title="举报详情" width="460px" @close="resetForm">
  <el-select v-model="post.reason" placeholder="请选择原因">
        <el-option label="违反法律法规" value="1"></el-option>
        <el-option label="不实信息" value="2"></el-option>
        <el-option label="侵犯个人权益" value="3"></el-option>
        <el-option label="扰乱社区环境" value="4"></el-option>
        <el-option label="其他" value="5"></el-option>
      </el-select>
      <div style="margin-top: 10px;"></div>
      <el-input v-if="post.reason === '5' "v-model="post.reason_else" placeholder="请输入原因"></el-input>
  <el-divider></el-divider>
  <div slot="footer" class="dialog-footer">
    <el-button type="primary" @click="submitReason()">确 定</el-button>
    </div>
</el-dialog>
</div>
        </div>
      </el-col>
    </el-row>
  </div>


    <el-container class="post-content">
      <div v-if="post.isMakeComment" class="make_comment">
        <el-input
        class="input_comment"
          type="textarea"
         :autosize="{ minRows: 2, maxRows: 4}"
          placeholder="请输入"
          v-model="inputComment">
          </el-input>
          <el-button class="custom-confirm-button"  @click="submitReply()">确认</el-button>
          <el-button class="custom-cancel-button" @click="cancel()">取消</el-button>
      </div>
      <!-- <button @click="toggleReplies(post)" class="show_reply">显示/隐藏回复</button> -->
      <el-button type="text" v-if="post.collapsed==false" @click="toggleReplies(post)" class="show_reply">隐藏回复</el-button>
<el-button type="text" v-if="post.collapsed==true" @click="toggleReplies(post)" class="show_reply">展开回复</el-button>
    </el-container>
    <el-container>
      <div v-if="!post.collapsed">
    <!-- <el-menu> -->
    <div v-for="(reply, index) in  post.comments" :key="index" class="comments">
      <div  @mouseover="highlightPost" @mouseout="resetPost">
        <!-- 用户头像 -->
      <div class="avatar-container">
        <img :src="reply.pic" alt="user avatar" class="avatar">
        <div>
      <div class="name-line">用户：{{ reply.author }}</div></div></div>
      <div class="replyContent">{{ reply.content}}</div>
      <div class="sub">
      <div class="user-line">{{ reply.time}}</div>
<!-- <div class="sub"> -->
  <el-button type="text" @click="replySub(reply.id)" class="buttons">回复</el-button>
<!-- <button :style="{ backgroundImage: `url(${button[1].background})`, 
        backgroundColor: button[1].backgroundColor }" class="sub_reply_button" @click="replySub(reply.id)"></button> -->
<el-button type="text" @click="reply.dialogVisible_sub = true" class="buttons">举报</el-button>
<el-dialog
:draggable="true"
  v-model="reply.dialogVisible_sub" title="举报详情" width="460px" @close="resetForm">
  <el-select v-model="post.reason" placeholder="请选择原因">
        <el-option label="违反法律法规" value="1"></el-option>
        <el-option label="不实信息" value="2"></el-option>
        <el-option label="侵犯个人权益" value="3"></el-option>
        <el-option label="扰乱社区环境" value="4"></el-option>
        <el-option label="其他" value="5"></el-option>
      </el-select>
      <div style="margin-top: 10px;"></div>
      <el-input v-if="post.reason === '5' "v-model="post.reason_else" placeholder="请输入原因"></el-input>
  <el-divider></el-divider>
  <div slot="footer" class="dialog-footer">
    <el-button type="primary" @click="submitReplyReason(reply)">确 定</el-button>
    </div>
</el-dialog>
<div>
<el-button type="text" v-if="reply.subCollapsed==false" @click="toggleSubComments(reply.id)" class="buttons">隐藏回复</el-button>
<el-button type="text" v-if="reply.subCollapsed==true" @click="toggleSubComments(reply.id)" class="buttons">展开回复</el-button>
<!-- </div> -->
</div>
</div>
      </div>
      <el-container>
      <div v-if="!reply.subCollapsed">
    <!-- <el-menu> -->
      <div v-for="(subComment, subIndex) in getSubComments(reply.id)" :key="subIndex" class="subComments">
      <div  @mouseover="highlightPost" @mouseout="resetPost">
          <!-- 用户头像 -->
      <div class="avatar-container">
        <img :src="reply.pic" alt="user avatar" class="avatar">
        <div>
      <div class="name-line">用户：{{ subComment.author }}</div></div></div>
      <div class="replyContent">{{ subComment.content}}</div>
      <div class="user-line">{{ subComment.time }}</div>
      </div>
      </div>
  <!-- </el-menu> -->
    </div>
</el-container>
<el-container>
      <div v-if="reply.isSubMakeComment" class="make_comment">
        <el-input
        class="input_sub_comment"
          type="textarea"
         :autosize="{ minRows: 2, maxRows: 4}"
          placeholder="请输入"
          v-model="inputComment">
          </el-input>
          <el-button class="custom-confirm-button"  @click="submit(reply.id)">确认</el-button>
          <el-button  class="custom-cancel-button" @click="cancelSub(reply.id)">取消</el-button>
      </div>
    </el-container>
</div>
  <!-- </el-menu> -->
</div>
</el-container>
</el-main>
  </div>
</template>

<style scoped>
.custom-confirm-button {
    background-color: #B71C1C;
    border-color: #B71C1C;
    color: white;
    margin-left: 2vh;
}

.custom-cancel-button {
    background-color: #bdaead ;
    border-color: #bdaead    ;
    color: white;
    
}
.avatar-container {
  margin-right: 1rem; /* 调整头像与内容之间的间距 */
  display: flex;
  flex-direction: row;
}

.avatar {
  width: 50px; /* 调整头像的大小 */
  height: 50px; /* 调整头像的大小 */
  border-radius: 50%; /* 使图像呈圆形 */
  object-fit: cover; /* 确保图像在容器内完整显示 */
}
.image-container {
    position: relative;
    overflow: hidden;
    width: 100%;
    height: 100%;
}
.el-carousel {
  width: 100%;
  height: 100%;
}
/* .image-container img {
    width: 100%;
    height: 100%;
    transition: transform 0.3s ease;
} */
.buttons{
  margin-left: 2vh;
  color: #82111f ;
}
.buttons:hover {
  color: #d42517; /* 可选：根据需要更改文字颜色 */
}
/* .el-header {
    background-color: #B3C0D1;
    color: #333;
    text-align: center;
    line-height: 60px;
    margin-left: 14vh;
  width:150vh;
} */
.big-container {
  margin: 20px auto;
  width: 165vh;
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
.data{
  display: flex;
  justify-content: flex-end;
  margin-right: 120px;
}
.like_button{
  width: 25px;
  height: 25px;
  /* 按钮样式 */
  border: none;
  cursor: pointer;
  border-radius: 4px;
  /* 其他样式 */
  background-repeat: no-repeat;
  background-size: cover;
  transition: background-color 0.3s ease; /* 添加过渡效果 */
  background-size: 100% 100%; /* 调整背景图像的尺寸 */
  margin-right: 3px;
  margin-left: 5px;
}
.sub_reply_button{
  width: 20px;
  height: 20px;
  /* 按钮样式 */
  border: none;
  cursor: pointer;
  border-radius: 4px;
  /* 其他样式 */
  background-repeat: no-repeat;
  background-size: cover;
  transition: background-color 0.3s ease; /* 添加过渡效果 */
  background-size: 100% 100%; /* 调整背景图像的尺寸 */
  margin-right: 3px;
  margin-left: 5px;
}
.report_button {
  margin-left: 5px;
  
}
.report_button .el-button {
  background-color: #ffffff;
  color: #a61b29  ;
  padding: 8px 10px;
  border-radius: 4px;
  border:none;
}
.hh{
  display: flex;
  flex-direction: row;
  margin-left: 12vh;
}
.show_reply{
  display: flex;
  justify-content: flex-start;
  margin-left: 12vh;
  background-color: #ffffff;
  color: #82111f ;
  border:none;
}
.show_reply:hover {
  color: #d42517; /* 可选：根据需要更改文字颜色 */
}
.input_comment{
  width: 80vh;       /* 设置输入框宽度 */
}
.input_sub_comment{
  width: 80vh;
  margin-left:-12vh;
}
.comments{
  margin-left: 12vh;
  width:100vh;
  text-align: left;
  margin-bottom: 1vh;
  margin-top: 1vh;
}
.subComments{
  margin-left: 12vh;
  width:88vh;
}
.make_comment{
  display: flex;
  justify-content: flex-start;
  display: flex;
  justify-content: flex-start;
  margin-left:12vh;
  align-items: flex-end;
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

.sub{
  display: flex;
  flex-direction: row;
}
.user-line {
  font-size: 14px; /* 调整字体大小 */
  margin-top: 0.5vh;
  color:#8a8888;
}
.name-line {
  font-size: 14px; /* 调整字体大小 */
  margin-top: 0.5vh;
  margin-left: 2vh;
  color:#f55454;

}
.replyContent{
  font-size: 18px; /* 调整字体大小 */
  margin-left: 8vh;
  margin-top: -2vh;
}

/* --------------------------------------------------------------------- */
.post-container {
  margin-left: 12vh;
  width:138vh;
  border: none;
  padding: 20px;
  background: #fff;
  
}
.post-content {
  display: flex;
  flex-direction: column;
}
.post_content{
  margin-top:2vh ;
  font-size:2vh ;
}
.post-title {
  display: flex;
  flex-direction: column;
  font-size:3vh ;
}
.author{
  display: flex;
  flex-direction: column;
}
.fixed-text-area { /* 固定大小的显示区域 */
  height: 300px; 
  overflow-y: auto; /* 内容溢出时自动滚动 */
  border-bottom: 1px solid #ddd; /* 底部灰色横线 */
  padding-bottom: 10px; /* 文字内容和横线之间的间距 */
}
.post-details {
  display: flex;
  justify-content: space-between;
  margin: 10px 0;
}
.post-actions {
  display: flex;
  align-items: center;
}
.post-actions span {
  margin-left: 10px;
}
.report-link {
  cursor: pointer;
  color: #f56c6c;
}
    
</style>