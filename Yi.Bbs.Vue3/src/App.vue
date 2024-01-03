<script setup></script>

<template>
  <el-config-provider :locale="locale">
    <RouterView />
  </el-config-provider>
</template>
<script setup>
import useConfigStore from "@/stores/config";
import { ElConfigProvider } from "element-plus";
import { onMounted } from "vue";

import zhCn from "element-plus/dist/locale/zh-cn.mjs";
const locale = zhCn;

const configStore = useConfigStore();

// 判断是否有loading有的话去掉
const loading = document.getElementById("Loading");
if (loading !== null) {
  document.body.removeChild(Loading);
}

//加载全局信息
onMounted(async () => {
  await configStore.getConfig();
});
</script>
<style scoped></style>
