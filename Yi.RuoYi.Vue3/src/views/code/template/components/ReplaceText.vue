<template>
  <el-row>
    <el-col :offset="6" :span="8">
      <el-input v-model="oldStr" placeholder="输入需替换内容"></el-input>
    </el-col>
    <el-col :span="8">
      <el-input v-model="newStr" placeholder="输入替换后内容"></el-input>
    </el-col>

    <el-col :span="2" class="btn">
      <el-button @click="replace" type="primary" :disabled="oldStr==''||newStr==''">替换字符串</el-button>
    </el-col>
  </el-row>
</template>

<script setup>
const props = defineProps(['text'])
const emit = defineEmits(['handleText'])
const oldStr=ref("");
const newStr=ref("");
const replace=()=>{
  const resultStr=  props.text.replace(new RegExp(oldStr.value,'g'),newStr.value);
  emit("handleText",resultStr);
}
</script>

<style scoped>
.btn{
    justify-content: right;
    display: flex;
}
</style>
