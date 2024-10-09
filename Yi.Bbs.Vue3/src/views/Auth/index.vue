<template>
  <div class="message">{{ message }}</div>
</template>

<script setup>
import { ref, watch } from "vue";
import { useRoute } from "vue-router";
import { authOtherLogin, authOtherBind } from "@/apis/auth.js";

const route = useRoute();

const code = ref(route.query.code);
const type = ref(route.query.state);

const message = ref("");
const scheme = ref("");
const authData = ref("");
const closeWindow = () => {
  setTimeout(() => {
    window.close();
  }, 2000);
};
watch(
  () => code.value,
  async (val) => {
    if (val) {
      // 使用正则表达式提取路由参数
      const regex = /\/auth\/([\w-]+)[?]?/;
      const result = regex.exec(route.fullPath);
      const authParam = result != null ? result[1] : null;
      switch (authParam) {
        case "gitee":
          scheme.value = "Gitee";
          break;
        case "qq":
          scheme.value = "QQ";
          break;
      }
      try {
        //state 0 代表使用第三方登录
        if (type.value === "0") {
          const { data } = await authOtherLogin({ code: val }, scheme.value);
          authData.value = data;
        } 
        //state 0 代表进行第三方绑定
        else if (type.value === "1") {
          const { data } = await authOtherBind({ code: val }, scheme.value);
          authData.value = data;
        }
      } catch (error) {
        if (error.status === 403) {
          closeWindow();
        }
      }
      window.opener.postMessage({
        authData: JSON.stringify(authData.value),
        type: scheme.value,
      });
      message.value = "授权成功";
      closeWindow();
    }
  },
  { immediate: true }
);
</script>

<style lang="scss">
.message {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  font-size: 20px;
}
</style>
