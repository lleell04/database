<!-- 商家界面订单管理的顶栏 -->
<template>
    <div class="CommodityTopbar">
      <button @click="changeView(0)" id="Button1" class="Ty">
        <span class="Buttondes">全部订单</span>
      </button> 
      <button @click="changeView(2)" id="Button2" class="Ty">
        <span class="Buttondes">待发货订单</span>
      </button>
      <button @click="changeView(3)" id="Button3" class="Ty">
        <span class="Buttondes">运输中订单</span>
      </button>
      <button @click="changeView(4)" id="Button4" class="Ty">
        <span class="Buttondes">已送达订单</span>
      </button>
      <button @click="changeView(5)"id="Button5" class="Ty">
        <span class="Buttondes">待退货订单</span>
      </button>
      <button @click="changeView(6)" id="Button6" class="Ty">
        <span class="Buttondes">已退货订单</span>
      </button>
    </div>
  </template>
  
  <script>
  export default {
    methods: {
      changeView(viewType) {
        this.$emit('changeView', viewType);
      }
    }
  };
  </script>

<style scoped>
.CommodityTopbar {
    background-color: #dfcdc7;
    height: 4vh;
    display: flex;
    position: fixed;
    top: 6vh;
    left: 150px; 
    right: 0;
    bottom: 0;
}

#Button1 {
  position: fixed;
  top: 7vh;
  left: 160px; 
  right: 0;
  bottom: 0;
    width: 150px;
    height: 3vh; 
    border-radius: 20px;
}
#Button2 {
  position: fixed;
  top: 7vh;
  left: 320px;
  right: 0;
  bottom: 0;
  width: 150px;
    height: 3vh; 
    border-radius: 20px;
}
#Button3 {
  position: fixed;
  top: 7vh;
  left: 480px; 
  right: 0;
  bottom: 0;
    width: 150px;
    height: 3vh; 
    border-radius: 20px;
}
#Button4 {
  position: fixed;
  top: 7vh;
  left: 640px;
  right: 0;
  bottom: 0;
  width: 150px;
    height: 3vh; 
    border-radius: 20px;
}
#Button5 {
  position: fixed;
  top: 7vh;
  left: 800px; 
  right: 0;
  bottom: 0;
    width: 150px;
    height: 3vh; 
    border-radius: 20px;
}
#Button6 {
  position: fixed;
  top: 7vh;
  left: 960px;
  right: 0;
  bottom: 0;
  width: 150px;
    height: 3vh; 
    border-radius: 20px;
}
.Ty {
    background-color: #a13232;
    border-color: #a13232;
}

.Ty:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}

.Buttondes {
    padding: 1vh;
}

</style>