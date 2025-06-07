<!-- 商家界面的订单管理总览界面 -->
<template>
  <div class="BusinessOrder">
    <Sidebar />
    <BusinessOrderTopbar @changeView="handleChangeView" />
    <div class="CommodityContent">
      <component :is="currentView" ref="currentView"></component>
    </div> 
  </div>
</template>

<script>
import BusinessOrderShow from './BusinessOrderShow.vue';
import BusinessOrderTopbar from './BusinessOrderTopbar.vue';
import Sidebar from './BusinessSidebar.vue'

export default {
  name: 'BusinessOrder',
  components: {
    BusinessOrderShow,
    BusinessOrderTopbar,
    Sidebar
  },
  data() {
    return {
      viewType: 1,
      currentView: 'BusinessOrderShow'
    }
  },
  methods: {
    handleChangeView(viewType) {
      this.viewType = viewType;
      this.$nextTick(() => {
        if (this.$refs.currentView && this.$refs.currentView.handleChange) {
          this.$refs.currentView.handleChange(viewType);
        } else {
          console.error('handleChange 方法在 currentView 组件中未找到');
        }
      });
    },
  }
};
</script>
