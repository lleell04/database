<!-- 商家首页折线图组件 -->
<template>
  <div>
    <br>
    <div ref="chart" style="width: 100%; height: 400px; margin-left: 60px;"></div>
  </div>
</template>

<script>
import { ref, onMounted, watch } from 'vue';
import * as echarts from 'echarts';

export default {
  name: 'OrderStatisticsChart',
  props: {
    data: {
      type: Array,
      required: true
    },
    labels: {
      type: Array,
      required: true
    }
  },
  setup(props) {
    const chart = ref(null);

    const renderChart = () => {
      const myChart = echarts.init(chart.value);

      const option = {
        title: {
          text: '最近七日订单数目'
        },
        tooltip: {
          trigger: 'axis'
        },
        xAxis: {
          type: 'category',
          data: props.labels
        },
        yAxis: {
          type: 'value'
        },
        series: [
          {
            name: '订单数目',
            type: 'line',
            data: props.data
          }
        ]
      };

      myChart.setOption(option);
    };

    onMounted(() => {
      renderChart();
    });

    watch([() => props.data, () => props.labels], () => {
      renderChart();
    });

    return {
      chart
    };
  }
};
</script>

<style scoped>
</style>