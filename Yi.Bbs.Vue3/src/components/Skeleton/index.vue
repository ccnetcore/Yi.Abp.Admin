<template>
  <el-col :span="24">
    <el-card shadow="never" class="card-box"> <el-skeleton /> </el-card>
  </el-col>
</template>

<script setup>
import { ref } from "vue";

const props = defineProps({
  isBorder: {
    type: Boolean,
    default: false,
  },
});
</script>

<style lang="scss">
.card-box {
  --el-card-border-color: v-bind(isBorder ? "#e4e7ed": "transparent");
}
</style>
