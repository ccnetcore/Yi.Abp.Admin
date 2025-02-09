<template>
  <div class="box">
    <div class="content">
      <RouterView />
      <div class="div-bottom">
        <span>备案：<span v-html="configStore.icp"></span></span>
        <span>站长：{{ configStore.author }}</span>
        <span v-if="!isIcp"><router-link to="/contact">联系我们</router-link></span>
        <span>关于本站</span>
        <span>建议反馈</span>
        <span>原创站点</span>
      </div>
    </div>
  </div>
  <StarBackground :speed="0.01" :number="4000"/>
</template>
<!-- <style src="@/assets/styles/login.scss" scoped></style> -->
<script setup>
import StarBackground from "@/components/StarBackground.vue";
import useConfigStore from "@/stores/config";
const  configStore=useConfigStore();
const isIcp=import.meta.env.VITE_APP_ICP==="true";
</script>
<style scoped lang="scss">
.box {
  width: 100vw;
  height: 100vh;
  .content {
    width: 100%;
    height: 100%;
  }
}
.div-bottom
{
  position: absolute;
  bottom: 30px;
  color: #CAD2D9;
  font-size: 12px;
  width: 100%;
  display: flex;
  justify-content: center;
  span{
    margin-right: 10px;
  }
  a {
    $link-color: red;
    color: $link-color;

    &.visited {
      color: $link-color;
    }

    &:hover {
      color: $link-color;
    }

    &:active {
      color: $link-color;
    }
  }
  
}
</style>
