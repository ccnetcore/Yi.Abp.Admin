<template>
<StarBackground :speed="0.2" :number="1000"/>
  <div class="content-main">
    <RouterView />
  </div>

   
</template>
<script setup>
import StarBackground from "@/components/StarBackground.vue";
</script>
<style scoped>
.content-main {
  width: 100%;
  /* height: 100%; */
  display: flex;
  justify-content: center;
  /* min-height: 1150px; */
  /*background-color: #f0f2f5;*/
}
</style>
