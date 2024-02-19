<template>
  <div class="sign-box">
    <div class="menuList">
      <ActivityNav/>
    </div>
    <div class="page">
      <RouterView />
    </div>
  </div>
</template>

<script setup>
import ActivityNav from './ActivityNav.vue'
</script>

<style lang="scss" scoped>
.sign-box {
  display: flex;
  justify-content: space-between;
  width: 1300px;
  height: 100%;
  padding: 20px 0 10px;
  .menuList {
    width: 20%;
    height: 100%;
    background-color: #fff;
    border-radius: 5px;
  }
  .page {
    width: 75%;
    height: 100%;
    background-color: #fff;
    border-radius: 5px;
  }
}
</style>
