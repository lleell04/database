<!-- 商家页面 -->
<template>
    <div style="display: flex;">
      <Sidebar @changeView="handleChangeView" />
      <div class="MainContent" style="border: 7px solid white;">
        <component :is="currentView"></component>
      </div> 
    </div>
  </template>
  
  <script>
  import Sidebar from './BusinessSidebar.vue';
  import BusinessHomePage from './BusinessHomePage.vue';
  import BusinessMarket from './BusinessMarket.vue';
  import BusinessOrder from './BusinessOrderManage.vue';
  import BusinessCommodity from './BusinessCommodity.vue';

  
  export default {
    components: {
      Sidebar,
      BusinessHomePage,
      BusinessMarket,
      BusinessOrder,
      BusinessCommodity,
    },
    data() {
      return {
        currentView: 'BusinessHomePage'
      }
    },
    methods: {
      handleChangeView(view) {
        this.currentView = view;
      }
    }
  }
  </script>
  
  <style scoped>
  #app {
    display: flex;
  }
  
  .MainContent {
    display: flex;
    position: fixed;
    bottom: 0;
    right: 0;
    left: 23vh;
    top:6vh;
    background-color:white;
  }
  </style>
  ————————————————
  