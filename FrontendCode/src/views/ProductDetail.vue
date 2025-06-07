<template>

  <Loading v-show="isLoading" />

  <div v-show="!isLoading" class="PDcontainer">
    <div class="header1" v-show="role==='买家'">
      <Navbar  />
    </div>
    <div class="header2" v-show="role==='商家'">
      <el-button 
              @click="enterSellerHome"
              style="display: flex;
              align-items: center;
              justify-content: center;
              font-size: 21px;
              border-radius: 5px;
              border: 2px solid #FFFFFF;
              background-color:#a61b29;
              color:#FFFFFF;
              cursor: pointer;
              width: auto;"
            >
            返回首页
        </el-button>
    </div>
    <div class="storeContent">
        <img :src="product.storeAvatar?product.storeAvatar.imageUrl:''" class="Avatar" />
        <div class="storeName">&nbsp{{ product.storeName }}</div>
        <div class="storeScore">评分：{{  product.score}}</div>
        <!-- 店铺收藏按钮，与商品收藏差不多 -->
        <el-button v-show="role==='买家'"
              @click="starStore"
              class="starStore-button"
              style="display: flex;
              align-items: center;
              justify-content: center;
              font-size: 21px;
              border-radius: 5px;
              border: 2px solid #a61b29;
              color:#a61b29;
              cursor: pointer;
              width: auto;
              height:65%;
              position: absolute;
              right:20%;" 

            >
            <img v-show="product.isStoreStared" 
            src="@/assets/mmy/stared.svg"
            alt="star"
            class="icon"/>
            <img v-show="!product.isStoreStared" 
            src="@/assets/mmy/star.svg"
            alt="star"
            class="icon"/>
            {{ product.isStoreStared ? '已收藏' : '收&nbsp藏' }}
        </el-button>
        <!-- 进入店铺按钮 -->
        <el-button
              class="enterStore-button"
              @click="enterStore"
              style="display: flex;
              align-items: center;
              justify-content: center;
              font-size: 21px;
              border-radius: 5px;
              border: 2px solid #a61b29;
              color:#a61b29;
              cursor: pointer;
              width: auto;
              height:65%;
              position: absolute;
              right:5%;" 
            >
            <img  
            src="@/assets/mmy/store-active.svg"
            class="icon"/>
            进入店铺 
        </el-button>
      </div>
    <div class="productContent">
      <div class="image">
        <div class="thumbnails">
          <div
          v-for="(item, index) in (Array.isArray(productImages) ? productImages.slice(0, 4) : [])"
            :key="index"
            class="thumbnail"
            :class="{ active: currentIndex === index }"
            @click="setCurrentImage(index)"
          >
            <img :src="item.imageUrl" :alt="`缩略图 ${index + 1}`" />
          </div>
        </div>
        <!-- 右侧的大图预览 -->
        <div class="preview">
          <img v-if="currentImage && currentImage.imageUrl" :src="currentImage.imageUrl" :alt="`预览图 ${currentIndex + 1}`" />
        </div>
        <!-- <img
          :src="`data:image/png;base64,${product.picture}`"
          class="image_inside"
        />
        <div class="overlay">细节描述：{{ product.description }}</div> -->
      </div>
      <div class="text">
        <div v-show="product.discountPrice===1" class="price">￥{{ product.price }}</div>
        <div v-show="product.discountPrice!==1" class="doubleprice">
          <div class="discountPrice">￥{{ product.price }}</div>
          <div class="price">￥{{ product.finalPrice }}</div>
        </div>
        <div class="name">{{ product.name }}</div>
        <div class="description">（细节描述）{{ product.description }}</div>
        <div class="store">来自 {{ product.storeName }} | 100%非遗正品保证</div>
        <div class="fromwhere">
          <div class="from1">发货地</div>
          <div class="from2">{{ product.fromWhere }}</div>
        </div>
        <div class="baozhang">
          <div class="baozhang1">保&nbsp障</div>
          <div class="baozhang2">假一赔十&nbsp七天无理由退货 </div>
        </div>
        <div class="baozhang">
          <div class="baozhang1">数&nbsp量</div>
          <div class="baozhang2">{{isAbleBuy===true?'1&nbsp有货':'0&nbsp无货'}} </div>
        </div>

        <div class="star_and_buy">
          <!-- 收藏 -->
          <el-button v-show="role==='买家'&&isAbleBuy"
          @click="starProduct"
          class="starProduct-button"
          style="display: flex;
          align-items: center;
          justify-content: center;
          font-size: 21px;
          border-radius: 5px;
          border: 2px solid #a61b29;
          color:#a61b29;
          cursor: pointer;
          width: auto;
          height:50px;
          right:30%;
          bottom:0%;
          position: absolute" 
        >
        <img v-show="product.isProductStared" 
        src="@/assets/mmy/stared.svg"
        alt="star"
        class="icon"/>
        <img v-show="!product.isProductStared" 
        src="@/assets/mmy/star.svg"
        alt="star"
        class="icon"/>
        {{ product.isProductStared ? '已收藏' : '收&nbsp藏' }}
        </el-button>
        <!-- 购买 -->
        <el-button type="success" 
        v-show="role==='买家'&&isAbleBuy"
        @click="enterPay"
        style="background-color: #a61b29; 
        letter-spacing: 5px; 
        font-size: 22px;
        border: 2px solid #a61b29;
        width: 25%; 
        height:50px;
        right: 5%;
        bottom:0%;
        position: absolute" 
        > 购买</el-button>
        </div>
      </div>
    </div> 
    <div class="productAndCommentContent">
        <!-- 侧边导航栏 -->
        <div class="sidebar">
          <div class="nav-item" @click="showProducts">店铺商品</div>
          <div class="nav-item" @click="showComments">店铺评论</div>
        </div>
        <!-- 内容区域 -->
        <div class="main-content">
                    <!-- 内容区域内的跳转按钮 -->
            <el-button 
            @click="handleButtonClick"
            v-show="activeSection==='comments'&&isRemarksNull===false"
            class="enter-button"
            style="
                font-size: 21px;
                border-radius: 5px;
                border: 2px solid #a61b29;
                color:#a61b29;
                cursor: pointer;
                width: auto;
                height:35px;
                ">查看全部评价</el-button>
            <el-button 
            @click="handleButtonClick"
            v-show="activeSection!=='comments'"
            class="enter-button"
            style="
                font-size: 21px;
                border-radius: 5px;
                border: 2px solid #a61b29;
                color:#a61b29;
                cursor: pointer;
                width: auto;
                height:35px;
                ">查看全部商品</el-button>

            <div v-if="activeSection === 'preview'" class="preview-section">
                <div 
                  class="displayProductItem"
                  v-for="product in displayProducts" 
                  :key="product.productId" 
                  @click="handleProductClick(product.productId)"
                  >
                    <img :src="product.productPics.length>0?product.productPics[0].imageUrl:null" alt="图片缺失" class="product-image" />
                    <div class="product-text">
                      <h2>{{ product.productName }}</h2>
                      <p>价格: ¥{{ product.productPrice }}</p>
                    </div> 
                </div>
            </div>
            <div class="remarks-loading" v-show="activeSection === 'comments'&&isRemarksLoading===true">
              <Loading/>
            </div>
            <div v-if="activeSection === 'comments'&&isRemarksLoading!==true" class="shop-remarks">
              <div v-show="isRemarksNull===true" class="remarksNull">该店铺暂无评论</div>
              <div v-show="isRemarksNull===false" class="remarks-content">
                <div 
                  v-for="remark in remarks" 
                  :key="remark.orderId" 
                  class="remarks">
                  <div class="remark-header">
                    <div class='remark-avatar'>
                      <img :src="remark.buyerAvatar" :alt="buyerAvatar" class='remark-avatar'/>
                    </div>
                    <div class="remark-buyerName" style="font-size: 16px; font-weight: bold; "> {{ remark.buyerName }} </div>
                  </div>
                  <div class="remark-content">
                    <div class="remark-score" style="font-size: 16px; text-align: left;">
                      评分：{{ remark.orderScore }}
                    </div>
                    <div class="remark-text" style="font-size: 16px; text-align: left;">
                      评价：{{ remark.orderRemark }}
                    </div>
                    <div class="splitLine"></div>
                  </div>
              </div>
            </div>
          </div>
      </div>
    </div>
    <div class="detailContent">
      <h2>图文详情</h2>
      <div
            v-for="(item, index) in imagesWithDescriptions"
            :key="index"
            class="image-and-text"
          >
            <p>{{ item.description }}</p>
            <img :src="item.url" :alt="`缩略图 ${index + 1}`" />
          </div>

    </div>
  </div>
</template>
  
<script setup >
    import Navbar from '../components/Navbar.vue';
    import Loading from '../views/templates/4.vue';
    import { ref,onMounted,reactive} from 'vue';
    import 'element-plus/dist/index.css';
    import { ElButton ,ElMessage} from 'element-plus';
    import { useRouter } from 'vue-router';
    import axiosInstance from '../components/axios';
    
    //页面是否正在加载
    const isLoading=ref(true);
    const isRemarksLoading=ref(true);
    //从浏览器中获取数据
    const productId = localStorage.getItem('productIdOfDetail');
    // const userId="S00000025";
    // const role="商家";
    const userId =localStorage.getItem('userId');
    const role=localStorage.getItem('role');
    // 使用 useRoute 来访问路由参数
    const router=useRouter();

    const product = ref({
      name: '',
      pictures: [],
      price: 0,
      description: '',
      storeName: '',
      storeId: '',
      discountPrice: 0,
      fromWhere: '',
      score: 0,
      isProductStared: false,
      isStoreStared:false,
      storeAvatar:{},
      finalPrice:0,
      imageAndText:[]
    }) ;
    //商品图片显示部分
    const imagesWithDescriptions = ref([]);
    const productImages=ref([]);
    const currentIndex = ref(0);
    const currentImage = ref(productImages.value[currentIndex.value]);
    const setCurrentImage = (index) => {
      currentIndex.value = index;
      currentImage.value = productImages.value[index];
    };

    // const product=reactive([])
    //是否已经存在相关订单导致无法购买
    const isAbleBuy=ref(true);
    //处理商品和评论预览
    const activeSection = ref('preview');
    const displayProducts = reactive([]); 
    const remarks=reactive([]);
    const isRemarksNull=ref(false);          
    const message=ref('');
    onMounted(async () => {
      console.log(`当前登录用户id为${userId}`);
      isOrderExist();
      //获取商品信息
      try {
        const response = await axiosInstance.get(`/Shopping/GetProductDetails/`,{
          params:{
            userId:userId,
            productId:productId
          }
        });
        isLoading.value=false;
        product.value=response.data;
        product.value.finalPrice=parseFloat((product.value.discountPrice*product.value.price).toFixed(2));
        imagesWithDescriptions.value = product.value.imageAndText || [];
        console.log(`product is ${JSON.stringify(product.value,null,2)}`);
        // console.log(`imagesWithDescriptions is ${JSON.stringify(imagesWithDescriptions.value,null,2)}`);
        productImages.value=product.value.pictures || [];
        // console.log(`productImages.value is ${JSON.stringify(productImages.value, null, 2)}`)
        currentImage.value = productImages.value[0] || null;
        showProducts();
        // console.log('页面加载完成');
        ////////////////////////////////折扣测试
        // product.value.discountPrice=3;
        // console.log(product.value);
        } catch (error) {
          ElMessage.error('页面加载失败！');
        }
    })
    const isOrderExist=async()=>{
      try {
        const response = await axiosInstance.get(`/Shopping/IsOrderExist/`,{
          params:{
            productId:productId
          }
        });
        isAbleBuy.value=!response.data;
        // console.log(`isAbleBuy.value is ${isAbleBuy.value}`);
        } catch (error) {
        }
    }
    const enterStore=()=>{
      localStorage.setItem('storeIdOfDetail',product.value.storeId);
      router.push('/shopdetail');
    }
    const enterSellerHome=()=>{
      router.push('/businesshomepage');
    }
    const starProduct = async() => {
      if(product.value.isProductStared===false){
        // console.log('进入收藏');
        try {
          const response = await axiosInstance.post('/Favourite/BookmarkProduct', null, {
              params: {
                userId: userId,
                productId: productId
              }
            });

            console.log(response.data);
            if (response.status === 200) {
              product.value.isProductStared = !product.value.isProductStared;
            } else {
              console.error('Unexpected response status:', response.status);
            }
      } catch (error) {
        console.error('starProduct failed:', error);
      }
    }else{
        console.log('进入取消收藏');
        try {
          const response = await axiosInstance.delete('/Favourite/UnbookmarkProduct', {
            params: {
              userId: userId,
              productId: productId
            }
          });
          console.log(response.data);
          if (response.status === 200) {
            product.value.isProductStared = !product.value.isProductStared;
          } else {
            console.error('Unexpected response status:', response.status);
          }
        } catch (error) {
          console.error('unStarProduct failed:', error);
        }
      }
    };
    const starStore = async() => {
      if(product.value.isStoreStared===false){
        try {
          const response = await axiosInstance.post('Favourite/BookmarkStore',null,{
            params: {
                userId: userId,
                'storeId':product.value.storeId
              }});
            if (response.status === 200) {
              product.value.isStoreStared = !product.value.isStoreStared; 
            } else {
              console.error('Unexpected response status:', response.status);
            }
        } catch (error) {
          console.error('starStore failed:', error);
        }
      }else{
        try {
          const response = await axiosInstance.delete('/Favourite/UnbookmarkStore', {
            params: {
              userId: userId,
              storeId: product.value.storeId
            }
          });
          if (response.status === 200) {
            product.value.isStoreStared = !product.value.isStoreStared;
          } else {
            console.error('Unexpected response status:', response.status);
          }
        } catch (error) {
          console.error('unStarStore failed:', error);
        }
      }
    };
    const showComments = async() => {
      activeSection.value = 'comments';
      try {
        const response = await axiosInstance.get('/Shopping/GetStoreRemarks', {
          params: {
            storeId: product.value.storeId
          }
        });
        if (remarks.length > 0) {
          remarks.splice(0, remarks.length);
        }
        const limitedRemarks = response.data.slice(0, 3);
        if(limitedRemarks.length===0){
          isRemarksNull.value=true;
        }
        limitedRemarks.forEach(remark => {
          remark.buyerAvatar = `data:image/png;base64,${remark.buyerAvatar}`;
          remarks.push(remark);
        });
        console.log(`remarks is ${JSON.stringify(remarks, null, 2)}`)
        message.value = '已获取评论信息';
        isRemarksLoading.value=false;
        // console.log(remarks);
      } catch (error) {
        if (error.response) {
          message.value = error.response.data;
        } else {
          message.value = '获取评论信息失败';
        }
      }
      console.log(message.value);
    };
    const showProducts = async () => {
      activeSection.value='preview';
      console.log(`product.value.storeId${product.value.storeId}`);
      try {
      const response = await axiosInstance.get('/StoreViewProduct/GetProductsByStoreIdAndViewType', {
        params: {
          storeId: product.value.storeId,
          ViewType: 1
        }
      });
      if (displayProducts.length > 0) {
        displayProducts.splice(0, displayProducts.length);
      }
      //splice和slice不同
      // 只取返回数据的前三个商品
      const limitedProducts = response.data.slice(0, 3);
      limitedProducts.forEach(product => {
        // product.productPic = `data:image/png;base64,${product.productPic}`;
        displayProducts.push(product);
      });
      // console.log(`displayProducts is ${JSON.stringify(displayProducts, null, 2)}`)
    } catch (error) {
      if (error.response) {
        message.value = error.response.data;
      } else {
        message.value = '获取全部信息失败';
      }
    }
    console.log(message.value);
    };
    const handleButtonClick = () => {
      localStorage.setItem('storeIdOfDetail',product.value.storeId);
      router.push('/shopdetail');
    };
  const handleProductClick = (productId) => {
    console.log('正在被点击');
    console.log(`productId is ${productId}`);
    localStorage.setItem('productIdOfDetail',productId);
    // router.replace('/productdetail').catch(() => {});
    location.reload();
  };
  const enterPay=()=>{
    const productStr = JSON.stringify(product.value);//序列化对象
    router.push({path:'/pay',query:{product: productStr,
                                    isPaid:'false'
    }});
  }

</script>

 <style scoped>
div {
  user-select: none;
  outline: none; 
  cursor: default; 
}
.header1{
  width:100%;
}
.header2{
  width:100%;
  background-color: #a61b29;
  position: relative;
  padding:5px 0px 5px 10px;
}
.el-button:active{
box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2); /* 点击时的阴影效果 */
transform: scale(0.95); /* 点击时缩小效果 */
}
.el-button {
  font-family: 'Noto Serif SC', serif;
  font-weight: bold;
}

/* 容器样式 */
.PDcontainer {
  font-family: 'Noto Serif SC', serif;
  display: flex;
  align-items: center;/*这才是水平对齐 */
  flex-direction: column;
  /* 哇趣！！！！！！min-height！！！I love you!!*/
  min-height: 100vh;
  overflow-x: hidden; 
  padding-bottom:15px;
  background-image: url("../assets/mmy/background.jpg");
}
.productContent,.storeContent,.detailContent{
  background-color: #FFFFFF;
  width: 1050px; 
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); 
  box-sizing: border-box; 
  border-radius: 15px; 
  margin-top:15px;
}

.storeContent{
  padding: 10px 30px 10px 10px;
  position: relative;
  display: flex;
  flex-direction: row; 
  justify-items: center;
}
.productContent {
  min-height:450px;
  padding: 40px; 
  display: flex;
  position: relative;
  overflow: hidden;
  flex-direction: row; 
}
.productAndCommentContent {
  width: 1050px; 
  margin-top:15px;
  display: flex;
  flex-direction: row;
  position: relative;
}
.detailContent{
  /* min-height:300px; */
  padding:20px 40px 40px 20px;
  display: flex;
  flex-direction: column;
}
.image {
  width: 600px;
  height: 100%;
  border-radius: 15px;
  position: relative; /* 确保覆盖层绝对定位在此容器内 */
  display: flex;
  flex-direction: row;
}
/* .image_inside,.overlay {
  width: 100%;
  height: 100%;
  border-radius: 15px;
} */
/* .overlay{
  position: absolute;
  top: 0;
  left: 0;
  display: flex;
  align-items: center;
  justify-content: center;

  background: rgba(0, 0, 0, 0.3); 
  color: white;
  opacity: 0; 
  transition: opacity 0.3s; 
  padding: 80px;
  box-sizing: border-box; 
  text-align: center;
  z-index: 2; 
}
.image:hover .overlay {
  opacity: 1;
} */
.text{
  display: flex;
  flex-direction: column; 
  align-items: flex-start;
  width: 450px;
  height: auto;
  padding:0px 30px 30px 0px;
  position: relative; /* 使得按钮能定位在 text 内部 */
  box-sizing: border-box;
}
.name, .price,.store{
  margin-top: 5px;;
  text-align: left; /*文字左对齐*/ 
  width:100%;
}
.price,.dis_price{
  font-size: 30px;
  color:#a61b29;
}
.price{
  /* border:2px solid #232020; */
  width:40%;
  /* background-color: #CCCCCC; */
}
.dis_price{
  display: flex;
  align-items: center;
  justify-content: left;
  flex-direction: row; 
}
.dis{
  background-color: rgba(27, 25, 25,0.5);
  padding:5px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-top-left-radius:5px;
  border-bottom-left-radius:5px;

}
.dis,.original_and_dis_price{
  width:50%;
  height:100%;
}
.original_price{
  font-size: 20px;
  text-decoration: line-through;
  color:rgb(0, 0, 0);
  background-color: #CCCCCC;
  border-top-right-radius:5px;
}
.final_price{
  font-size:25px;
  background-color: rgba(0, 0, 0,0.3);
  overflow: hidden;
  padding:3px;
  box-sizing: border-box;
  text-align: left;
  border-bottom-right-radius:5px;
}
.name{
  font-size: 25px;
}
.store{
  border-radius:5px;
  border: 1px solid #ddd; /* 浅灰色边界 */
  padding: 5px; /* 为内容添加一些内边距，以便边界不会直接贴近内容 */
  box-sizing: border-box; /* 确保内边距和边界不会影响元素的宽度 */
  background-color: #a61b29;
  color:white;
  font-size: 20px;
  transition: transform 0.3s ease, box-shadow 0.3s ease; /* 平滑过渡效果 */
  will-change: transform;/*告诉浏览器将要发生变化 */
}

.store:hover {
  transform: scale(1.005); /* 放大 5% */
  cursor: pointer;
}
.fromwhere,.baozhang{
  display: flex;
  align-items: center;
  justify-content: left;
  flex-direction: row; 
  margin-top: 5px;
}
.from1,.baozhang1{
  font-size: 20px;
  color:rgba(0,0,0,0.5);
}
.from2,.baozhang2{
  font-size: 20px;
  color:#000000;
  padding-left:20px ;
}
.starProduct-button:hover ,.starStore-button:hover,.enterStore-button:hover,.enter-button:hover{
  color: black; /* 鼠标悬停时文字颜色保持不变 */
  background-color: white; /* 鼠标悬停时背景颜色保持不变 */
  border: 2px solid black; /* 鼠标悬停时边框颜色保持不变 */
}
.starProduct-button .icon ,.starStore-button .icon,.enterStore-button .icon{
  width: 20px; /* 调整图标大小 */
  height: 20px; /* 调整图标大小 */
  margin-right: 8px; /* 图标与文本之间的间距 */
}
.storeName{
  font-size: 20px;
  font-weight:bold;
  /* padding-top:5px; */
}
.storeScore{
  font-size:20px;
  color:#a61b29;
  padding-left:20px;
  /* padding-top:5px; */
}


/* 侧边栏内部 */
.sidebar {
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); /* 添加阴影效果（可选） */
  box-sizing: border-box; /* 使内边距和边框包含在宽度和高度内 */
  border-radius: 15px; 
  width: 20%;
  background-color: #FFFFFF;
  color: #000000;
  margin-right: 10px;
  padding: 10px;
  border-radius:15px ;
}

.nav-item {
  font-size:18px;
  padding: 10px;
  cursor: pointer;
  color:#a61b29;
  font-weight:bold;
  border-bottom: 1px solid rgba(166, 27, 41,0.6);
}

.nav-item:hover {
  border-bottom: 2px solid rgba(166, 27, 41,1);
}

/* 主内容区域 */
.main-content {
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); /* 添加阴影效果（可选） */
  box-sizing: border-box; /* 使内边距和边框包含在宽度和高度内 */
  border-radius: 15px; 
  background-color: #FFFFFF;
  color: #000000;
  border-radius:15px ;
  width: 100%;
  padding: 10px;
  min-height:300px;

}
.loading-text{
  font-size:22px;
}
.Avatar{
  width: 30px; 
  height: 30px; 
  border-radius: 10px;
  object-fit: cover;
}
.doubleprice{
  position: relative;
  display: flex;
  flex-direction: row; 
  justify-items: center;
}
.discountPrice{
  text-decoration: line-through;
  font-size:15px;
  color:rgba(0,0,0,0.4);
}
.preview-section{
  padding-left:20px;
  padding-right:20px;
  display: flex;
  flex-wrap: wrap;
  /* gap: 20px; */
  justify-content: space-between;
  width:100%;
}
.displayProductItem{
  box-shadow: 0 0 10px rgba(166, 27, 41,0.1); 
  box-sizing: border-box; /* 使内边距和边框包含在宽度和高度内 */
  border-radius: 15px; 
  transition: transform 0.3s, box-shadow 0.3s;
  display: grid;
  width: 200px;
  height: 200px;
  margin-top:10px;
  margin-bottom:10px;
  padding:20px;
}
.displayProductItem:hover{
  transform: translateY(-2px);
}

.product-image{
  border-radius: 15px; 
  width:100%;
  height:auto;
  max-height:90px;
}
.product-item h2 {
  font-size: 18px;
  margin: 0 0 10px;
}

.product-item p {
  font-size: 16px;
  margin: 0 0 10px;
}

.product-text{
  align-self: end;
}
.product-text h2{
  font-size:18px;
}
.product-text p{
  font-size:auto;
}
.shop-remarks{
  background-color: #fff;
  border-radius: 16px;
  box-sizing: border-box;
  margin-top: 10px;
  padding: 8px;
  position: sticky;
  top: 10px;
  width: 100%;
  display: flex;
  justify-content: center;
}

.remarks{
  box-shadow: 0 0 10px rgba(166, 27, 41,0.1); /* 添加阴影效果（可选） */
  box-sizing: border-box; /* 使内边距和边框包含在宽度和高度内 */
  border-radius: 15px; 
  transition: transform 0.3s, box-shadow 0.3s;
  padding:10px;
  margin-bottom: 20px;
}

.remarks-content{
  width: 85%;
}
.remark-header{
  display: flex;
  flex-direction: row;
}
.remark-avatar{
  height: 40px;
  width: 40px;
  border-radius: 40px;
}

.remark-buyerName{
  margin-left: 20px;
  font-weight: 500;
  font-size: 14px;
  color: #11192d;
  line-height: 20px;
}

.remark-content {
  margin-left: 55px;
  margin-bottom: 16px;
  display: flex;
  flex-direction: column;
  justify-content: left;
}

.splitLine{
  margin: 12px 0;
  height: 1px;
  background-color: #f0f3f5;
}

.thumbnails {
  display: flex;
  flex-direction: column;
  width:70px;

}
.thumbnail {
  cursor: pointer;
  border: 2px solid transparent;
  width: 100%;
  height:90px;
  overflow:hidden;
  padding: 3px;
  text-align: center;
  border-radius: 15px;
}
.thumbnail img {
  width: 100%;
  height: 100%;
  border-radius: 15px;
  object-fit: cover;
}
.thumbnail.active {
  border-color: #a61b29;
}
.preview {
  margin-left: 40px;
  background-color: #a61b29;
  border-radius: 15px;
}
.preview img {
  border-radius: 15px;
  width: 400px;
  height: 100%;
  object-fit: cover;
}
.description{
	font-family: 'Noto Serif SC', serif;
	font-size: 16px;
	overflow-y: auto; 
	max-height: 50px;
	text-align: left;
}
.detailContent h2{
  border-bottom:2px solid rgb(166, 27, 41,0.2);
  color:#a61b29;
  text-align: left;
  padding-left:40px;
}
.image-and-text img{
  width:80%;
  border-radius: 15px;
}
.image-and-text p{
  font-size:22px;
}
.remarksNull{
  font-size:40px;
  position:relative;
  top:80px;
  color:rgba(0,0,0,0.3);
}
.remarks-loading{
  padding-top:50px;
}
</style> 
  