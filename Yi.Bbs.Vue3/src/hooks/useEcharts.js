import * as echarts from "echarts";
import { onMounted, onUnmounted, watch } from "vue";
import { debounce } from "@/utils/index";
import _ from "lodash";

// containerRef echarts实例   props 传入的值  baseOptions 图表初始化渲染
export default function useEcharts(containerRef, emits, props, baseOptions) {
  let chart = null;

  onMounted(() => {
    chart = echarts.init(containerRef.value);
    init();
    chart.on("click", function (param) {
      emits("chart-click", param);
    });
    //自适应不同屏幕时改变图表尺寸
    window.addEventListener("resize", cancalDebounce);
  });
  // 用于将echarts实例暴露出去使用
  const getChart = () => chart;
  onUnmounted(() => {
    chart && chart.dispose();
    window.removeEventListener("resize", cancalDebounce);
  });

  function init() {
    let option = _.cloneDeep(baseOptions);
    chart.setOption(option);
    chart.setOption(props.option);
  }

  function resize() {
    chart && chart.resize();
  }

  watch(
    () => props.option,
    (val) => {
      chart && chart.setOption(val);
    },
    { deep: true }
  );

  const cancalDebounce = debounce(resize, 500);

  return {
    getChart,
    resize,
  };
}
