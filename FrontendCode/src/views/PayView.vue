<template>
  <Loading v-show="isLoading||(!isLoading&&isAlipayLoading)" />
  <div v-show="!isLoading&&!isAlipayLoading" class="Pcontainer">
      <div ></div>
      <div class="procedure" >
          <div class="procedure1">
              <img src="@/assets/mmy/number1.svg" class="number"/>
              <div class="text">1.确认订单信息</div>
          </div>
          <div class="line">———————</div>
          <div class="procedure2">
              <img src="@/assets/mmy/number2.svg" class="number"/>
              <div class="text">&nbsp2.支付&nbsp&nbsp</div>
          </div>
          <div class="line" v-show="isPaid===true">———————</div>
          <div class="procedure3" v-show="isPaid===true">
              <img src="@/assets/mmy/number3.svg" class="number"/>
              <div class="text-active">3.查看订单信息</div>
          </div>
      </div>
      <div class="address">
        <div class="highlight-text">收货信息</div>
        <div class="addressInfo">
          <img class="addressImage" src="@/assets/mmy/location.svg">
          <div class="text1">寄送至&#8201;&#8201;&#8201;&#8201;</div>
          <div class="customerAddress">{{ customer.address }}&#8201;&#8201;({{ customer.name }}&#8201;收)&#8201;&#8201;</div>
          <el-button v-show='isPaid===false'
              @click="openDialog"
              class="changeAddress"
              style="
              font-size: 20px;
              border-radius: 5px;
              border: 2px solid #a61b29;
              color:#a61b29;
              cursor: pointer;
              width: auto;
              height:32px;
              margin-top:5px;
              padding-left:20px;
              padding-right:20px;
              right:40px;
              position: absolute" 
          >修改地址</el-button>
        </div>
          <el-dialog 
              v-model="dialogVisible"
              width="35%"
              :style="{borderRadius:'15px'}"
              >
              <div class="dialog-content">
                  
                  <div class="dialog-line">
                      <div class="dialog-text">&#8201;收&#8201;件&#8201;人&#8201; </div>
                      <el-input 
                      v-model="customer.name" 
                      placeholder="请输入收件人姓名" 
                      clearable 
                      style="width: 350px;
                      height:40px;
                      font-size:17px;"
                      ></el-input>
                  </div>

                  <div class="dialog-line">
                      <div class="dialog-text">所在地区 </div>
                      <el-cascader :options='options' 
                      v-model='selectedOptions'
                      placeholder="省/市/区（县）"
                      class="custom-cascader"
                      style="width: 350px;"
                       @change='addressChange' 
                       ></el-cascader>
                   </div>
                  <div class="dialog-line">
                      <div class="dialog-text">详细地址 </div>
                      <el-input 
                      v-model="address2" 
                      placeholder="请输入详细地址（具体到小区、门牌号等）" 
                      clearable 
                      style="width: 350px;
                      height:40px;
                      font-size:17px;"
                      ></el-input>
                  </div>
              </div>
              <span slot="footer" class="dialog-footer">
              <el-button 
              style="border: 2px solid rgba(0,0,0,0.4);
              width: 20%;
              height:40px;
              font-size:20px;">取 消</el-button>
              <el-button 
               @click="handleSave"
               style="border: 2px solid rgba(0,0,0,0.4);
               background-color:rgba(0,0,0,0.4);
               color:white;
               width: 20%;
               height:40px;
               font-size:20px;">确 定</el-button>
              </span>
              </el-dialog>
             
      </div>
      <div class="orderInfo">
          <div class="storeArea">
              <img src="@/assets/mmy/store-active.svg" class="storeImage" @click="enterStore">
              <div class="text2" @click="enterStore">{{ product.storeName }}&#8201;&#8201;></div>
          </div>
          <div class="productArea">
              <img :src="product.pictures?product.pictures[0].imageUrl:''" class="productImage" alt="图片加载失败">
              <div class="orderDetail">
                  <div class="text-p">{{ product.name }}</div>
                  <div class="text-p2">￥{{ product.price }}</div>
                  <div class="text-p1">订单编号:&#8201;&#8201;{{ order.id }}</div>
                  <div class="text-p1">创建时间:&#8201;&#8201;{{ order.createTime }}</div>
              </div>
          </div>
      </div>
      <div class="price" v-show="isPaid===false">
        <div class="storeArea">
              <img src="@/assets/mmy/price.svg" class="storeImage" @click="enterStore">
              <div class="text-price1" >价格明细</div>
          </div>
          <div class="text-price">商品原价：&#8201;&#8201;{{ (product.price) }}元</div>
          <div class="text-price">活动折扣：&#8201;&#8201;{{ product.discountPrice===1?'本商品未参与活动': '* '+product.discountPrice*100+'%'}}</div>
          <div class="text-price">折后价格：&#8201;&#8201;{{ parseFloat((product.price*product.discountPrice).toFixed(2)) }}元</div>
          <div v-show="creditPrice!==0">
              <div class="useCredit">
                  <div class="text-price-active">是否使用积分</div>
                  <el-radio-group  v-model="isUseCredits" style="margin-left: 10px;margin-bottom:0px" @change="priceCalculate">
                  <el-radio label="yes">是</el-radio>
                  <el-radio label="no">否</el-radio>
                  </el-radio-group>
              </div>
              <div class="text-price-active-small" >当前积分{{ customer.credits }},可抵扣{{ creditPrice }}元</div>
          </div>
          <div class="text-price" v-show="creditPrice===0">积分抵扣：当前积分{{ customer.credits }},可抵扣{{ creditPrice }}元</div>
          <div class="text-price-active">价格合计：{{ product.finalPrice }}元</div>
          <div class="pay">
          <el-button
              @click="checkPay"
              class="payMoney"
              style="
              font-size: 20px;
              border-radius: 5px;
              background-color: #a61b29;
              color:white;
              cursor: pointer;
              width: auto;
              height:43px;
              right:60px;
              bottom:20px;
              position: absolute" 
          >支付￥{{ product.finalPrice }}</el-button>
          <el-dialog 
              v-model="payVisible"
              width="20%"
              :style="{borderRadius:'15px'}"
              >
              <div v-show="isPaySuccess===true">
                  <p class="text_pay_isSuccess">支付成功</p>
                  <p class="text_pay">获得积分：&#8201;&#8201;{{ bonusCredits }}</p>
                  <p class="text_pay">积分余额：&#8201;&#8201;{{ finalCredits }}</p>
              </div>
              <div v-show="isPaySuccess===false">
                  <p class="text_pay_isSuccess">支付失败</p>
                  <p class="text_pay">钱包余额不足，请及时充值</p>
              </div>

          </el-dialog>
      </div>
      </div>
      <div class="price" v-show="isPaid===true">
        <div class="storeArea">
              <img src="@/assets/mmy/price.svg" class="storeImage" @click="enterStore">
              <div class="text-price1" >价格明细</div>
          </div>
          <div class="text-price">商品原价：&#8201;&#8201;{{ (product.price) }}元</div>
          <div class="text-price">活动折扣：&#8201;&#8201;{{ product.discountPrice===1?'本商品未参与活动': '* '+product.discountPrice*100+'%'}}</div>
          <div class="text-price">折后价格：&#8201;&#8201;{{ parseFloat((product.price*product.discountPrice).toFixed(2)) }}元</div>
          <div class="text-price-active">积分抵扣：&#8201;&#8201;{{ parseFloat((product.price*product.discountPrice).toFixed(2))-product.finalPrice }}元</div>
          <div class="text-price-active">价格合计：{{ product.finalPrice }}元</div>
      </div>
      <el-dialog 
              v-model="payChoiceVisible"
              width="20%"
              :style="{borderRadius:'15px'}"
              >
            <h2>请选择支付方式</h2>
            <el-radio-group  v-model="payWay" 
            style="display:flex;
            flex-direction: column;
            margin-left: 10px;
            margin-bottom:0px">
                <el-radio label="wallet">钱包</el-radio>
                <el-radio label="alipay">支付宝</el-radio>
            </el-radio-group>
            <el-button 
              @click="openPay"
              style="
              font-size: 16px;
              border-radius: 5px;
              border: 2px solid #a61b29;
              background-color: #a61b29;
              color:#FFF;
              cursor: pointer;
              width: auto;
              height:32px;
              margin-top:5px;
              padding-left:20px;
              padding-right:20px;
              right:40px;" 
          >确认支付</el-button>
          </el-dialog>
  </div>
</template>
<!-- 去掉lang="ts"就不会标红了 -->
<script setup >
import { ref ,computed,onMounted,watch} from 'vue';
import { ElDialog, ElButton ,ElMessage} from 'element-plus';
import { regionData, codeToText } from 'element-china-area-data';
import axiosInstance from '../components/axios';
import { useRouter } from 'vue-router';
import Loading from '../views/templates/4.vue';

//接收路由参数
import { useRoute } from 'vue-router';
//页面加载
const isLoading=ref(true);
const isAlipayLoading=ref(false);
const route = useRoute();
// const productId='555555';
const productid = localStorage.getItem('productIdOfDetail');
const userid =localStorage.getItem('userId');
const userId=userid?userid:'000000';
const productId=productid?productid:'555555';
const product=ref({});
// const product = computed(() => {
// const productStr = route.query.product ;
// return productStr ? JSON.parse(productStr) : null;
// });
//根据是否已经付款订单会显示不同状态
const isPaid = route.query.isPaid === 'true';
// const isPaid =false;
const order=ref({id:'',createTime:''});
//收货人相关信息
const customer=ref({name:'',
              address:'',
              credits:300});
const dialogVisible=ref(false);
const payVisible=ref(false);
const payChoiceVisible=ref(false);
const address1=ref('');
const address2=ref('');
const isUseCredits=ref('no');
//用了toFixed(2)就是string类型的了
const creditPrice=ref(0);
// 省市区信息
const options = ref(regionData);
const selectedOptions = ref(['110000', '110100', '110101']);
//后端回复信息
const message=ref('');
//支付后获得积分以及剩余积分
const bonusCredits=ref(0);
const finalCredits=ref(0);
//支付方式
const payWay=ref('wallet');
//是否支付成功
const isPaySuccess=ref(false);
////订单支付结束后（不管支付成功没成功）都跳转回原来的页面
const returnUrl=ref('');
const router=useRouter();
const routerPath=localStorage.getItem('routerPath');
watch(payVisible, (newValue, oldValue) => {
  if (newValue === false && oldValue === true) {
      // 当 `isPaySuccess` 变为 `false` 时执行操作
      console.log('支付失败，状态变为 false');
      // 跳转到指定页面
      const path = routerPath ? routerPath : '/home'; 
      router.push(path);
  }
});
onMounted(async () => {
  const productStr = route.query.product;
  if (productStr) {
    product.value = JSON.parse(productStr);
    console.log(`product.value is ${JSON.stringify(product.value, null, 2)}`)
    if(isPaid===false){
      product.value.finalPrice=parseFloat((product.value.price*product.value.discountPrice).toFixed(2));
    }
  }else {
    product.value = {}; // 确保对象是初始化的
  }
  // console.log(`product is ${JSON.stringify(product.value)}`);
  // console.log(`productId is ${productId}`);
  // console.log(`userId is ${userId}`);
  // console.log(`creditPrice is ${JSON.stringify(creditPrice, null, 2)}`);

  //////////可以删
  // if(isPaid==false){
  //   product.value.finalPrice=product.value.discountPrice;
  // }
  const formData = new FormData();
  formData.append('BuyerId',userId);
  formData.append('ProductId',productId);
  try {
      const response = await axiosInstance.post('Payment/AddOrders', formData,{
          headers: { 'Content-Type': 'multipart/form-data' }
     });
      order.value.id=response.data.orderId;
      order.value.createTime=response.data.createTime;
      customer.value.name=response.data.username;
      console.log(`customer.value.name is ${customer.value.name}`)
      if(customer.value.name==null){
          customer.value.name='未知收货人';
      }
      customer.value.address=response.data.address;
      if(customer.value.address==null){
          customer.value.address='未知地';
      }
      isLoading.value=false;
      customer.value.credits=response.data.credits;
      creditPrice.value=parseInt(Math.floor(customer.value.credits/100).toFixed(0));
  } catch (error) {
      console.log('订单生成失败');
   }
})
const openDialog=()=>{
  dialogVisible.value = true;
}
const handleSave = () => {
if(!address1.value||!address2.value){
    ElMessage.error('请保证地址不为空');
}else {
  dialogVisible.value = false;
  customer.value.address=address1.value+address2.value;
  console.log(`customer.value.address is ${customer.value.address}`);
}
};
// 将地址编号转换为汉字
const addressChange=()=> {
const arr=selectedOptions.value;
address1.value=codeToText[arr[0]]+codeToText[arr[1]]+codeToText[arr[2]];
console.log(address1);
}
//根据积分计算最终价格
const priceCalculate=()=>{
  // 确保 discountPrice 和 creditPrice 是数字进行计算
  const afterDiscount=parseFloat((product.value.price*product.value.discountPrice).toFixed(2));
  const credits = creditPrice.value;
  console.log(`isUseCredits.value is ${isUseCredits.value}`);
  if (isUseCredits.value === 'yes') {
      product.value.finalPrice =afterDiscount- credits; // 计算最终价格并保留两位小数
  } else {
      product.value.finalPrice = afterDiscount; // 如果不使用积分，最终价格就是折扣价格
  }
  if(product.value.finalPrice<0){
      product.value.finalPrice=0;
  }
  console.log(`product.value.finalPrice is ${product.value.finalPrice}`);

}
const enterStore=()=>{
      localStorage.setItem('storeIdOfDetail',product.value.storeId);
      router.push('/shopdetail');
}
const checkPay=()=>{
    if(customer.value.name==="未知收货人"||customer.value.address==="未知地"){
      ElMessage.error("请补充收货信息");
    }else{
      payChoiceVisible.value=true;
      // openPay();
    }
}
const aliPay=async()=>{

  const formData1 = new FormData();
    formData1.append('orderId',order.value.id);
    formData1.append('order_address',customer.value.address);
    formData1.append('username',customer.value.name);
    formData1.append('actual_pay', product.value.finalPrice);//使用积分后的价格
    formData1.append('total_pay',parseFloat((product.value.price*product.value.discountPrice).toFixed(2)));//打折后的价格
    //打印测试
      formData1.forEach((value, key) => {
        console.log(key, value);
     });
    try {
    const response = await axiosInstance.put('Payment/ConfirmOrders', formData1, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
    } catch (error) {
        if (error.response) {
        // 请求已发出，服务器返回了状态码
            console.error('响应错误状态码:', error.response.status);
            console.error('响应错误数据:', error.response.data);
            console.error('响应错误头:', error.response.headers);
        } else if (error.request) {
            // 请求已发出，但没有响应
            console.error('请求错误:', error.request);
        } else {
            // 其他错误
            console.error('错误信息:', error.message);
        }
  }

  const path = routerPath ? routerPath : '/home'; 
  // if(path==='/ordercentre'){
  //   returnUrl.value='http://47.97.5.21:17990/ordercentre';
  // }else{
  //   returnUrl.value='http://47.97.5.21:17990/merchandise/1';
  // }
  returnUrl.value='https://localhost:7262/ordercentre';
  console.log(`orderID is ${order.value.id}`);
  console.log(`actualPay is ${product.value.finalPrice}`);
  console.log(`returnUrl is ${returnUrl.value.toString()}`);
  const formData = new FormData();
  formData.append('orderID',order.value.id);
  formData.append('actualPay',product.value.finalPrice.toString());
  formData.append('returnUrl',returnUrl.value);

  try {
    const response = await axiosInstance.post('/Alipay',formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
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
  message.value='';
  }

//确认订单信息并完成支付
const openPay=async()=>{
  if(payWay.value==='wallet'){
    const formData = new FormData();
    formData.append('orderId',order.value.id);
    formData.append('order_address',customer.value.address);
    formData.append('username',customer.value.name);
    formData.append('actual_pay', product.value.finalPrice);//使用积分后的价格
    formData.append('total_pay',parseFloat((product.value.price*product.value.discountPrice).toFixed(2)));//打折后的价格
    formData.forEach((value, key) => {
        console.log(`${key}: ${value}`);
    });
    try {
    const response = await axiosInstance.put('Payment/ConfirmOrders', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
    bonusCredits.value = response.data.bonusCredits;
    finalCredits.value = response.data.credits;
    isPaySuccess.value=true;
    } catch (error) {
        isPaySuccess.value=false;
        if (error.response) {
        // 请求已发出，服务器返回了状态码
            console.error('响应错误状态码:', error.response.status);
            console.error('响应错误数据:', error.response.data);
            console.error('响应错误头:', error.response.headers);
        } else if (error.request) {
            // 请求已发出，但没有响应
            console.error('请求错误:', error.request);
        } else {
            // 其他错误
            console.error('错误信息:', error.message);
        }
        }
    message.value='';
    payChoiceVisible.value=false;
    payVisible.value=true;
  }else{
    isAlipayLoading.value=true;
    console.log(`isLoading||(!isLoading&&isAlipayLoading) is ${isLoading.value||(!isLoading.value&&isAlipayLoading.value)}`)
    aliPay();
  }
}

</script>

<style scoped>
div {
  user-select: none;
  outline: none; 
  cursor: default; 
}
:deep(.el-radio.is-checked .el-radio__inner) {
  border-color: #a61b29; /* 修改边框颜色 */
  background-color: #a61b29; /* 修改选中颜色 */
}
:deep(.el-radio .el-radio__label) {
  color: #a61b29; /* 修改文字颜色 */
  font-size: 16px; /* 修改文字大小 */
}
.Pcontainer {
  font-family: 'Noto Serif SC', serif;
background-color: rgba(204, 204, 204,0.4);
display: flex;
align-items: center;/*这才是水平对齐 */
flex-direction: column;
height: 100vh;
overflow-x: hidden; /*隐藏水平滚动条 */
overflow: auto;
background-image: url("../assets/mmy/background.jpg");

/* position:relative; */
}
.procedure{
margin-top:20px;
display: flex;
align-items: center;/*这才是水平对齐 */
flex-direction: row;
align-items: right;
}
.procedure1,.procedure2,.procedure3{
  display: flex;
  flex-direction: column;
  align-items: center;/*这才是水平对齐 */
}
.number{
width: 40px; /* 调整图标大小 */
height: 40px; /* 调整图标大小 */
margin-right: 8px; /* 图标与文本之间的间距 */
}
.text{
font-size:15px;
}
.text-active{
  font-size:15px;
  color:#a61b29;
}
.line{
  padding-bottom:15px ;
  padding-right:20px;
}
.address,.orderInfo,.price{
  background-color: #FFFFFF;
  width: 900px; 
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); /* 添加阴影效果（可选） */
  box-sizing: border-box; /* 使内边距和边框包含在宽度和高度内 */
  border-radius: 15px; 
  position:relative;
  margin-top:15px;
}
.orderInfo{
  padding:5px;
}
.address{
  display: flex;
  flex-direction: column;
  /* height:40px; */
  min-height:90px;
}
.addressInfo{
  display: flex;
  flex-direction: row;
  width:100%;
}
.addressImage{
  width:35px;
  height:30px;
  padding-top:5px;
}
.text1,.customerAddress{
  font-size:20px;
  padding-top:5px;
}
.changeAddress:hover{
  color: black; /* 鼠标悬停时文字颜色保持不变 */
  background-color: white; /* 鼠标悬停时背景颜色保持不变 */
  border: 2px solid black; /* 鼠标悬停时边框颜色保持不变 */
}
.dialog-content {
display: flex;
flex-direction: column; /* 使内容竖直排列 */
gap: 20px; 
}
.dialog-line{
  display: flex;
  justify-content: center; 
  flex-direction: row;
}
.dialog-text{
  padding-right:10px;
  padding-top:5px ;
  font-size:20px;
}
.dialog-footer {
display: flex;
justify-content: center; 
padding-top:25px;
}
.orderInfo{
  display: flex;
  flex-direction: column;
  height:250px;
  padding-left:20px;
}
.storeArea{
  display: flex;
  flex-direction: row;
  padding-top:10px;
}
.storeImage{
  width:30px;
  height:30px;
  padding-right:5px;
  cursor: pointer;
}
.productArea{
  margin-top:20px;
  min-height:150px;
  display: flex;
  flex-direction: row;
}
.productImage{
  width:125px;
  height:125px;
  border-radius: 5px;
  margin-left:30px;
  background-color: #a61b29;
  color:#FFFFFF;
}
.orderDetail{
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding-left:50px;
}
.text2{
  font-size:20px;
  margin-right:10px;
  cursor: pointer;
  color:#a61b29;
}
.text-p{
  font-size:23px;
}
.text-p1{
  font-size:20px;
  color:rgba(0,0,0,0.6);
}
.text-p2{
  font-size:19px;
  color:rgba(0,0,0,0.8);
}
.price{
  display: flex;
  flex-direction: column;
  min-height:250px;
  align-items: start; 
  padding:10px 0px 15px 20px;
  position:relative;
}
.highlight-text{
  width:100%;
  border-radius:15px 15px 0px 0px ;
  background-color: #a61b29;
  color:#FFFFFF;

  text-align: left;
  padding-left:20px;
  font-size:21px;
  padding-top: 5px;
  padding-bottom: 5px;
}
.text-price{
  padding-left:20px;
  font-size:20px;
  padding-bottom: 2px;
}
.text-price-active{
  padding-left:20px;
  font-size:20px;
  /* padding-bottom: 2px; */
  font-weight:bold;
  color:#a61b29;
}
.text-price-active-small{
  padding-left:60px;
  font-size:17px;
  padding-bottom: 2px;
  font-weight:bold;
  color:#000000;
}
.text-price1{
  font-size:20px;
  text-align: left;
  color:#a61b29;
  padding-bottom: 10px;
}
.useCredit{
  display: flex;
  flex-direction: row;
}

.text_pay_isSuccess{
  font-size:22px;
  margin-bottom: 10px;
}
.text_pay{
  margin-bottom: 7px;
  font-size:20px;
}

.payMoney:hover{
  transform: scale(1.02); /* Scale up by 20% */
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}
.loading-text{
font-size:22px;
}

</style>