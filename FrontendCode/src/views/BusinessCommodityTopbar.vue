<!-- 商家界面商品管理的页面切换顶栏 -->
<template>
    <div class="CommodityTopbar">
      <button @click="changeView(1)" class="ButtonA" id="Button1">
        <span class="Buttondes">全部商品</span>
      </button>
      <button @click="changeView(2)" class="ButtonA" id="Button2">
        <span class="Buttondes">已售商品</span>
      </button>
      <button @click="changeView(3)" class="ButtonA" id="Button3">
        <span class="Buttondes">未售商品</span>
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

.ButtonA {
    background-color: #a13232;
    border-color: #a13232;
}

.ButtonA:hover {
    background-color: #8b2b2b;
    border-color: #8b2b2b;
}

.Buttondes {
    padding: 1vh;
}

</style>