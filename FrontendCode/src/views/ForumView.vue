<!-- 买家用户的论坛页面 -->
<script setup lang="ts">
import Navbar from '../components/Navbar.vue';
import { ref, computed } from 'vue';
import { ElPagination, ElInput} from 'element-plus';
import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import router from '@/router';
import axiosInstance from '../components/axios';
const search=ref('');
const way=ref('forum');
const storedPage = localStorage.getItem('currentPage');
const currentPage = ref(storedPage ? parseInt(storedPage, 10) : 1);
const storedSort = localStorage.getItem('sort');
const sort = ref(storedSort || 'time');
const pageSize=ref(12);
const totalPostNums = ref(); // 文章总数
const isSearching =ref(false);
const haveSearched=ref(true);
const posts = ref<Array<{ id: string, title: string, author: string, time: string ,like:string,comment:string,image:string}>>([]);
const message=ref('');
const fetchPost= async () =>{
      axiosInstance.post('/Post/get_all_posts', {
        'sortBy':sort.value,
        'page':currentPage.value,
        'pageSize':pageSize.value
      }).then(response => {
        const data = response.data;
        if (data && Array.isArray(data.posts)) {
          // 解构帖子数组
          posts.value = data.posts.map(post => ({
            id: post.postId || '',
            title: post.postTitle || '',
            author: post.authorName || '匿名',
            time: post.releaseTime,
            like:post.numberOfLikes || 0,
            comment:post.numberOfComments || 0,
            image: post.coverImageId.imageUrl,
          }));
          
          // 设置总帖子数量
          totalPostNums.value = data.totalPostNums || 0;
          message.value="成功";
        } else {
          console.error('Invalid data format.');
        }
      }).catch(error => {
        message.value="失败";
      });
}
const searchPost = async () => {
  axiosInstance.post('/Post/search_posts', {
        'keyWord':search.value,
        'sortBy':sort.value,
        'descending':true,
      })
    .then(response => {
        const data = response.data;
        if (data && Array.isArray(data.target_posts)) {
            posts.value = data.target_posts.map(post => ({
                id: post.postId || '',
                title: post.postTitle || '',
                author: post.authorName || '',
                time: post.releaseTime|| '',
                like: post.numberOfLikes || 0,
                comment: post.numberOfComments || 0,
                image: post.image.imageUrl,
            }));

            totalPostNums.value = data.amount || 0;
            message.value = "成功搜索";
            haveSearched.value=true;
        } else {
            console.error('Invalid data format.');
            haveSearched.value=false;
        }
    }).catch(error => {
        message.value = "失败";
        console.error(error);
        haveSearched.value=false;
    });
};
const displayedPosts=computed(()=> {
      // 根据当前页码和每页条数计算需要显示的文章列表
      if (isSearching.value==false||search.value=='') {
        fetchPost();
        // 当不处于搜索状态时，返回所有帖子
        return posts.value;
    } else {
      if( haveSearched.value==true){
        searchPost();
        // 当处于搜索状态时，根据当前页码和每页条数计算需要显示的文章列表
        const startIndex = (currentPage.value - 1) * pageSize.value;
        const endIndex = startIndex + pageSize.value;
        return posts.value.slice(startIndex, endIndex);
      }
    }
      //else{
    //     return posts.value.filter(post => 
    //   post.title.toLowerCase().includes(search.value) ||
    //   post.author.toLowerCase().includes(search.value) ||
    //   post.content.toLowerCase().includes(search.value)
    // );
    //   }
    })
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
// const buttons = reactive([
//   { id: 1, text: '发布', background: 'src/assets/czw/release.svg', backgroundColor: 'transparent' },
//   { id: 2, text: '按热度排序', background: 'src/assets/czw/hot.svg', backgroundColor: 'transparent' },
//   { id: 3, text: '按时间排序', background: 'src/assets/czw/time.svg', backgroundColor: 'transparent' },
//   { id: 4, text: '消息中心', background: 'src/assets/czw/ring.svg', backgroundColor: 'transparent' },
// ]);
// const changeButtonColor = (button, isHovered) => {
//   if (isHovered) {
//     button.backgroundColor = '#87c2a5'; // 设置鼠标悬停时的背景颜色
//   } else {
//     button.backgroundColor = 'transparent'; // 恢复背景颜色为透明
//   }
// };
function handleSizeChange(val) {
        console.log(`每页 ${val} 条`);
        pageSize.value = val;
        currentPage.value = 1; // 切换每页条数时，重置当前页码为第一页
      };
function  handleCurrentChange(val) {
        console.log(`当前页: ${val}`);
        currentPage.value = val;
        //displayedPosts.value;
      }

function handleTitleClick( id,event) {
  localStorage.setItem('postId', id);
  localStorage.setItem('way', way.value);
  localStorage.setItem('currentPage', String(currentPage.value));
  localStorage.setItem('sort', sort.value);
  router.push( 'viewpost'); // 跳转至 /viewpost 页面
}
function  handleSearch()
{
  isSearching.value=true;
}
const activeIndex = ref('1');
const option=ref(1);
const menuItems = ref([
  { index: 1, title: '发布帖子' },
  { index: 2, title: '消息中心' },
]);
const menuChange = (index:any) => {
  activeIndex.value = index.toString();
  option.value = parseInt(index);
  if (option.value=== 1) {
    router.push('/releasepost'); // 跳转至 /releasepost 页面
  }
 else  if (option.value === 2) {
  router.push('/messageview'); // 跳转至 /messageview 页面
  }
};

const sortBy = (newSort) => {
  sort.value = newSort;
  localStorage.setItem('sort', newSort);
};
</script>

<template>
  <Navbar />
  <!-- <h1 class="forum_title">论坛</h1> -->
  <el-container style="height:100vh; border: 1px solid #eee" class="all">
    <el-aside width="18vh" style="background-color:  #82111f; ">
  <div class="big-title" style="display: flex; justify-content: center; align-items: center; width: 100%;">
    <img class="image" src="@/assets/czw/aside.svg" alt="Original Image"  />
    <span>论坛</span>
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
    <el-main>
  <main class="main-content">
    <div class="buttons">
      <!-- <div class="color-block red-block">全部</div> -->
      <button class="button" @click="sortBy('time')" :class="{ active: sort=== 'time' }">排序按时间</button>
      <button class="button" @click="sortBy('likes')" :class="{ active: sort=== 'likes' }">排序按点赞</button>
      <el-input
            background-color="#ffffff"
            text-color="black"
            active-text-color="#d42517"
      class="search_input"
    placeholder="请输入关键词"
    v-model="search"
    @input="handleSearch">
    <i slot="suffix" class="el-input__icon el-icon-search"></i>
  </el-input>
    </div>
    <div class="post-display">
      <div v-for="(post, index) in displayedPosts" :key="index" class="post-item" @click.prevent="handleTitleClick(post.id)">
        <img :src="post.image" alt="Post Image" class="post-image" />
        <a class="post-title">{{ post.title }}</a>
        <p>作者：{{ post.author }}</p>
        <p>发布时间: {{ post.time }}</p>
      </div>
    </div>
  </main>
<!-- </div> -->
  <div style="display: flex; justify-content: center; margin-top: 1vh;">
  <el-pagination
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
      :current-page="currentPage"
      :page-sizes="[21,12]"
      :page-size="pageSize"
      layout="total, sizes, prev, pager, next, jumper"
      :total="totalPostNums">
    </el-pagination>
  </div>
    </el-main>
    </el-container>
  </el-container>
</template>
  
<style scoped>

.buttons {
  display: flex;
  justify-content: start;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px; /* 色块和内容之间的间距 */
}

.color-block {
  padding: 10px 20px;
  border-radius: 5px;
  background-color: #bdaead; /* 红色块背景色 */
  color: white;
  font-weight: bold;
  user-select: none; /* 禁止用户选择文本 */
}

.button {
  background: none;
  border: none;
  outline: none;
  color: grey;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  transition: color 0.3s;
}

.button:hover {
  color: black;
}

.button.active {
  color: #ff4d4d; /* 按钮点击后文字变色 */
}

.forum_title {
  height: 10vh; /* 根据需求设置标题的高度 */
  font-size: calc(10vh * 0.8);  /* 根据需求设置标题的字体大小 */
  /* background-color: #287658;  */
  font-family: Arial, sans-serif;
  display: flex;
  align-items: center;
  justify-content: center;
}

.main_part {
  display: flex;
  /* align-items: center;*/
  flex-direction: row; /* 设置横向排列 */
  /*border: 1px solid black; /* 添加边界样式 */ 
  /*padding: 10px; /* 添加内边距以增加边界与内容之间的间距 */
}
.search_input {
  width: 35vh;
  height: 3vh; /* 调整搜索框的高度 */
  font-size: calc(3vh * 0.6); /* 调整输入文字的大小 */
  left: 25%;
  margin-left: 48vh;
  /* 其他样式属性 */
}
.search_input .el-input__inner {
  background-color: red !important; /* 设置背景颜色为红色 */
  color: white !important; /* 设置文字颜色为白色 */
  border: none !important; /* 去掉边框 */
}

.search_input .el-input__inner::placeholder {
  color: rgba(255, 255, 255, 0.5) !important; /* 设置占位符颜色为半透明的白色 */
}
.post {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  margin: 10px 0;
  padding: 10px;
  border: none;
}
.all {
        overflow: hidden; /* 隐藏页面的滚动条 */
        background-image: url('@/assets/czw/bg2.svg');
  background-size: cover;
  background-repeat: no-repeat;
  background-attachment: fixed;
  background-position: center;
  height: 100vh; /* 确保背景图覆盖全页面高度 */
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