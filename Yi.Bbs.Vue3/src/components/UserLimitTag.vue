<template>
  <el-tag round effect="light" :type="limitItem?.type">
    {{ limitItem?.label }}
  </el-tag>
</template>
<script setup>
import {  ref, watch } from "vue";
const props = defineProps(["userLimit"]);
const limitItem=ref({}) ;
const statusTypeList = [
  {
    label: "正常",
    value: "Normal",
    type: "success",
  },
  {
    label: "危险",
    value: "Dangerous",
    type: "warning",
  },
  {
    label: "已禁止",
    value: "Ban",
    type: "danger",
  },
];

watch(()=>props,()=>{
    limitItem.value = statusTypeList.filter((item) => item.value === props.userLimit)[0];
},  {immediate:true,deep:true});
</script>