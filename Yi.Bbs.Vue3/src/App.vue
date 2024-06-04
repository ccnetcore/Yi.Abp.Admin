<script setup></script>

<template>
  <el-config-provider :locale="locale">
    <RouterView />
  </el-config-provider>
</template>
<script setup>
import mainHub from "@/hubs/mainHub.js";
import noticeSignalR from "@/hubs/noticeHub.js";
import bbsNoticeSignalR from "@/hubs/bbsNoticeHub.js";
import useConfigStore from "@/stores/config";
import { ElConfigProvider } from "element-plus";
import useUserStore from "@/stores/user.js";
import { onMounted, watch, computed } from "vue";
const userStore = useUserStore();
import zhCn from "element-plus/dist/locale/zh-cn.mjs";
const locale = zhCn;
const configStore = useConfigStore();
const token = computed(() => useUserStore().token);
// 判断是否有loading有的话去掉
const loading = document.getElementById("Loading");
if (loading !== null) {
  document.body.removeChild(Loading);
}

//加载全局信息
onMounted(async () => {
  await configStore.getConfig();

    //如果登录了，再连接消息通知
    bbsNoticeSignalR();
  noticeSignalR();


});

watch(
  () => token,
   (val, oldValue) => {
    //console.log("token发生改变");
    if (val) {
      mainHub();
    }
  },
  { immediate: true, deep: true }
);
</script>
<style scoped></style>
