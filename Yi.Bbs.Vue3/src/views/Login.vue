<template>
  <div class="login-wrapper">
    <h1>{{ configStore.name }}-登录</h1>
    <div class="login-form">
      <el-form ref="loginFormRef" :model="loginForm" :rules="rules">
        <div class="username form-item">
          <el-form-item prop="userName">
            <span>使用账号</span>
            <input
              type="text"
              class="input-item"
              v-model="loginForm.userName"
            />
          </el-form-item>
        </div>
        <div class="password form-item">
          <el-form-item prop="password">
            <span>密码</span>
            <input
              type="password"
              class="input-item"
              v-model="loginForm.password"
            />
          </el-form-item>
        </div>
      </el-form>
      <!-- <RouterLink to="/register"> 没有账号？前往注册</RouterLink> -->
      <button class="login-btn" @click="login(loginFormRef)">登 录</button>
      <button class="login-btn" @click="guestlogin">访客</button>
    </div>

    <div class="divider">
      <span class="line"></span>
      <span class="divider-text">其他方式登录</span>
      <span class="line"></span>
    </div>

    <div class="other-login-wrapper">
      <div class="other-login-item">
        <img src="@/assets/login_images/QQ.png" alt="" />
      </div>
      <div class="other-login-item">
        <img src="@/assets/login_images/WeChat.png" alt="" />
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, reactive } from "vue";
import { useRouter, useRoute } from "vue-router";
import useUserStore from "@/stores/user.js";
import useConfigStore from "@/stores/config";
import useAuths from "@/hooks/useAuths";
const configStore = useConfigStore();
const userStore = useUserStore();
const router = useRouter();
const route = useRoute();

const { loginFun } = useAuths();

const loginFormRef = ref();
const rules = reactive({
  userName: [{ required: true, message: "请输入账号名", trigger: "blur" }],
  password: [{ required: true, message: "请输入密码", trigger: "blur" }],
});
const loginForm = reactive({
  userName: "",
  password: "",
  uuid: "",
  code: "",
});
const guestlogin = async () => {
  const redirect = route.query?.redirect ?? "/index";
  router.push(redirect);
};
const login = async (formEl) => {
  if (!formEl) return;
  await formEl.validate((valid) => {
    if (valid) {
      try {
        loginFun(loginForm);
      } catch (error) {
        ElMessage({
          message: error.message,
          type: "error",
          duration: 2000,
        });
      }
    }
  });
};
</script>
<style src="@/assets/styles/login.scss" scoped></style>
