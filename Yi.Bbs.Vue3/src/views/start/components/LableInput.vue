<script setup>
import { ref } from 'vue';


const model = defineModel()

</script>
<template>
    <div>
      
        <input type="text" v-model="model"  />
        <label>项目名称</label>
  </div>
</template>
<style lang="scss" scoped>
label {
    position: relative;
    top: -69px;
    left: 20px;
    display: inline;
    background-color: #fff;
    color: rgb(33, 37, 40);
}

input {
    border: 2px solid #eee;
    height: 80px;
    background-color: #FAFAFA;
    color: #3b3a57;
    font-size: 1.3rem !important;
    font-weight: 400;
    padding: 26px 30px 0 30px;
    font-size: x-large;
    border-radius: 8px;
    width: 100%;
}
input:hover {
    border: 2px solid #409EFF;
 background-color:#FFFFFF; /* 鼠标悬停时的边框颜色 */
}
</style>