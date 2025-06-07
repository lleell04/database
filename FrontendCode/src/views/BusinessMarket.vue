<!-- 商家界面参与市集的总览界面 -->
<template>
  <div class="BusinessMarket">
    <Sidebar />
       <BusinessMarketTopbar @changeView="handleChangeView" />
       <div class="MarketContent">
       <component :is="currentView"></component>
       </div> 
  </div>
</template>

<script>
import BusinessMarketShow from './BusinessMarketShow.vue'
import BusinessMarketTopbar from './BusinessMarketTopbar.vue'
import Sidebar from './BusinessSidebar.vue'

export default {
   name:'BusinessOrder',
   components: {
   BusinessMarketShow,
   BusinessMarketTopbar,
   Sidebar
 },
 data() {
   return {
     currentView: 'BusinessMarketShow'
   }
 },
 methods: {
   handleChangeView(view) {
     this.currentView = view;
   }
 }
};
</script>
  