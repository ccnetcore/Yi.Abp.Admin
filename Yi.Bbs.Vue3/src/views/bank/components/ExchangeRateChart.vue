<template>
    <div>
    <div class="rate v-chart"  ref="statis">
    </div>
   
    
    </div>
    </template>

<script setup >
  import useEcharts from "@/hooks/useEcharts";
  import { exchangeRateConfig } from "../hooks/exchangeRateConfig";
  import {ref} from 'vue';
  const props = defineProps({
    option: {
      type: Object,
      default: () => {},
    },
  });
  const emits = defineEmits([
    "chart-click", // 点击chart
  ]);
  
  let statis = ref(null);
  const { resize } = useEcharts(statis, emits, props, exchangeRateConfig);
  defineExpose({
    resize,
  });


</script>
<style scoped lang="scss">
.rate
{
height: 300px;

}
.v-chart {
    width: 100%;
  }

</style>
