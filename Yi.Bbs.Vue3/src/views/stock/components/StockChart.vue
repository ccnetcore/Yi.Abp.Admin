<template>
  <div class="stock-chart-container" ref="chartContainer"></div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import * as echarts from 'echarts/core';
import { LineChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent,
  MarkLineComponent,
  MarkPointComponent
} from 'echarts/components';
import { SVGRenderer } from 'echarts/renderers';

// 注册必要的组件
echarts.use([
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent,
  LineChart,
  SVGRenderer,
  MarkLineComponent,
  MarkPointComponent
]);

const props = defineProps({
  stockData: {
    type: Array,
    required: true
  }
});

const chartContainer = ref(null);
let myChart = null;

const initChart = () => {
  if (!chartContainer.value) return;
  
  myChart = echarts.init(chartContainer.value);
  
  const option = {
    backgroundColor: 'transparent',
    title: {
      text: '股票价格走势',
      textStyle: {
        color: '#e6edf3'
      }
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross',
        label: {
          backgroundColor: '#6a7985'
        }
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: props.stockData.map(item => item.date),
      axisLine: {
        lineStyle: {
          color: '#30363d'
        }
      },
      axisLabel: {
        color: '#8b949e'
      }
    },
    yAxis: {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#30363d'
        }
      },
      splitLine: {
        lineStyle: {
          color: '#21262d'
        }
      },
      axisLabel: {
        color: '#8b949e'
      }
    },
    series: [{
      name: '价格',
      type: 'line',
      stack: '总量',
      data: props.stockData.map(item => item.value),
      smooth: true,
      symbol: 'none',
      lineStyle: {
        width: 3,
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color: '#58a6ff' },
          { offset: 1, color: '#39d353' }
        ])
      },
      areaStyle: {
        opacity: 0.4,
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color: 'rgba(88, 166, 255, 0.5)' },
          { offset: 1, color: 'rgba(57, 211, 83, 0.1)' }
        ])
      }
    }]
  };
  
  myChart.setOption(option);
  
  window.addEventListener('resize', () => {
    myChart && myChart.resize();
  });
};

// 监听数据变化重新渲染图表
watch(() => props.stockData, () => {
  if (myChart) {
    myChart.setOption({
      xAxis: {
        data: props.stockData.map(item => item.date)
      },
      series: [{
        data: props.stockData.map(item => item.value)
      }]
    });
  }
}, { deep: true });

onMounted(() => {
  initChart();
});
</script>

<style scoped>
.stock-chart-container {
  width: 100%;
  height: 100%;
  min-height: 300px;
}

:deep(.el-empty) {
  padding: 20px 0;
  height: auto;
  max-height: 150px;
}

:deep(.el-empty__image) {
  width: 60px;
  height: 60px;
}

:deep(.el-empty__description) {
  margin-top: 10px;
}
</style> 