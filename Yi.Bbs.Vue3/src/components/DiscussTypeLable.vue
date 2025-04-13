
<script setup>
import {computed} from "vue";

const model = defineModel();
const items=[
  { type: 'primary', label: '主题',key:"Article" },
  { type: 'primary', label: '悬赏',key:"Reward"  }
];
const currentValue=computed(()=>{
  return items.filter(item=>item.key===model.value)[0];
})
</script>


<template>
  <el-tag
      v-if="currentValue"
      :type="currentValue?.type"
      effect="light"
  >
    {{ currentValue?.label }}
  </el-tag>
</template>
