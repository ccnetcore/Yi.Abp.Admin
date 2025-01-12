<template>
  <div ref="box">
    <v-form-designer ></v-form-designer>
  </div>
</template>

<script setup lang="ts">
import {ref,onMounted} from "vue";

const box = ref<Element>()
onMounted(() =>
{
  box.value?.firstChild?.classList.add("not-margin")
  box.value?.firstChild?.children[0].remove()
})
</script>

<style lang="scss" scoped>
div {
  margin: 0; /* 如果页面出现垂直滚动条，则加入此行CSS以消除之 */
}
.not-margin{
  margin: 0 !important;
}
</style>