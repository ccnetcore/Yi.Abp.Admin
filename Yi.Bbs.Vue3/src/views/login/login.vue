<template>
 <div class="container">
  <!-- 登录 -->
        <div class="div-content">
            <div class="div-left">
                <div class="left-container">
                    <p class="title title-1">Hello,<span @click="guestlogin">you can go to homepage >></span></p>
                    <p class="title title-2">Welcome to Yi!</p>
                    <el-form
                ref="loginFormRef"
                :model="loginForm"
                :rules="rules"
                >

                    <div class="input-content">
                        <div class="input">
                            <p>用户名</p>
                            <el-form-item prop="userName">
                            <input type="text" v-model="loginForm.userName">
                            </el-form-item>
                        </div>

                        <div class="input">
                            <p>密码</p>
                            <el-form-item prop="password">
                            <input type="password" v-model="loginForm.password">
                            </el-form-item>
                        </div>

                        <div class="input">
                            <p>验证码</p>
                            <el-form-item prop="code">
                            <div class="code">
                                <input class="code-input" type="text" v-model.trim="loginForm.code">
                                <img class="code-img" alt="加载中" @click="handleGetCodeImage"  :src="codeImageURL">
                            </div>
                          </el-form-item>
                        </div>
                    </div>
</el-form>
                    <div class="left-lable">
                      <div>
                        <input type="checkbox">
                      <label>记住我</label>
                      </div>
                       
                      <span class="right-forgot" @click="handleForgotPassword">忘记密码？点击找回</span>
                    </div>

                    <div class="left-btn">
                        <button type="button" class="btn-login" @click="login(loginFormRef)">登录</button>
                        <button type="button" class="btn-reg" @click="handleRegister">前往注册</button>
                    </div>
                    <div class="bottom-div">
                        <p>其他方式: <span @click="handleQQLogin"><img src="@/assets/login_images/qq-setting.png" alt="QQ" /></span> <span @click="handleGiteeLogin"><img src="@/assets/login_images/gitee-setting.png" alt="Gitee" /></span></p>
                    </div>
                </div>
            </div>
            <div class="div-right div-logo">
<img class="div-img" src="@/assets/login.png" alt=""/>
            </div>
        </div>

    </div>
</template>
<script setup>
import { ref, reactive, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import useAuths from "@/hooks/useAuths";
import useUserStore from "@/stores/user";
const { loginFun, loginSuccess } = useAuths();
const router = useRouter();
const route = useRoute();
const loginFormRef = ref();
const rules = reactive({
  userName: [{ required: true, message: "请输入用户名", trigger: "blur" }],
  password: [{ required: true, message: "请输入密码", trigger: "blur" }],
});
const loginForm = reactive({
  userName: "",
  password: "",
  uuid: "",
  code: "",
});
//前往注册
const handleRegister=()=>{
  router.push("/register");
}
//前往忘记密码
const handleForgotPassword = () => {
  router.push("/forgotPassword");
}
//直接进入首页
const guestlogin = () => {
  const redirect = route.query?.redirect ?? "/index";
  router.push(redirect);
};
const codeUUid = computed(() => useUserStore().codeUUid);
const login = async (formEl) => {
  if (!formEl) return;
  await formEl.validate((valid) => {
    if (valid) {
      try {
        loginForm.uuid = codeUUid.value;
        loginFun(loginForm);
      } catch (error) {
        console.log(error.message, "error.message");
        ElMessage({
          message: error.message,
          type: "error",
          duration: 2000,
        });
      }
    }
  });
};

// 获取图片验证码
const codeImageURL = computed(() => useUserStore().codeImageURL);
const handleGetCodeImage = () => {
  useUserStore().updateCodeImage();
};
onMounted(async () => {
  await useUserStore().updateCodeImage();
});


const handleQQLogin = () => {
  window.open(
    "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id=102087446&redirect_uri=https://ccnetcore.com/auth/qq&state=0&scope=get_user_info",
    undefined,
    "width=500,height=500,left=50,top=50"
  );
};

const handleGiteeLogin = () => {
  window.open(
    "https://gitee.com/oauth/authorize?client_id=949f3519969adc5cfe82c209b71300e8e0868e8536f3d7f59195c8f1e5b72502&redirect_uri=https%3A%2F%2Fccnetcore.com%2Fauth%2Fgitee&state=0&response_type=code",
    undefined,
    "width=500,height=500,left=50,top=50"
  );
};

window.addEventListener("message", async (e) => {
  const { authData, type } = e.data;
  console.log(authData, "传到登录页的值");
  if (authData) {
    await loginSuccess({ data: JSON.parse(authData) });
  }
});
</script>
<style src="@/assets/styles/login.css" scoped>

</style>