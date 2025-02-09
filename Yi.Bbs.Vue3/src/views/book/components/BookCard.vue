<script setup>

import { ref, onMounted } from "vue";

const props = defineProps([
  "discuss",
]);
const discuss=ref(props.discuss)
onMounted(()=>{
  
})
const getUrl = (str) => {
  return `${import.meta.env.VITE_APP_BASEAPI}/file/${str}`;
};
</script>
<template>
  <div class="card-body">
    <div class="left-logo">
      <el-image style="width: 40px; height: 40px"
                :src="getUrl(discuss.cover)"
                fit="fill">
        <template #error>
          <el-icon size="40px" color="#F0F2F5"><Picture /></el-icon>
        </template>
      </el-image>
 
    </div>
    <div class="right-content">

      <div class="content-top">
        {{discuss.title}}
      </div>
      <div class="content-bottom">
        {{discuss.introduction}}

      </div>

    </div>
  </div>
</template>
<style scoped>
.card-body {
  width: 100%;
  background-color: #FFFFFF;
  padding: 24px;
  border-radius: 8px;
  display: flex;
  .left-logo{

  }
  .right-content{
    display: flex;
    flex-direction: column;
    padding-left: 15px;
    overflow: hidden;
    .content-top{
      color: rgba(0, 0, 0, 0.88);
      font-weight: 600;
      font-size: 16px;
      margin-bottom: 8px;
    }
    .content-bottom{
      font-size: 14px;
      color: rgba(0, 0, 0, 0.45);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
}
</style>