<template>
 <div class="container">
  <!-- 登录 -->
        <div class="div-content"  v-if="isRegister">
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
                        <input type="checkbox">
                        <label>记住我</label>
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
            <div class="div-right">
<img class="div-img" src="@/assets/login.png"/>
            </div>

           
        </div>
  <!-- 注册 -->
        <div class="div-content" v-else>
            <div class="div-right-register">
              <img class="div-img" src="@/assets/login.png"/>
            </div>
            <div class="div-left-register">
                <div class="left-container">
                    <p class="title  register-title">Thank Join to Yi!</p>
                    <el-form
                class="registerForm"
                ref="registerFormRef"
                :model="registerForm"
                :rules="registerRules"
              >
                    <div class="input-content">
                        <div class="input">
                            <p>用户名</p>
                            <el-form-item prop="userName">
                            <input type="text"    v-model.trim="registerForm.userName">
                          </el-form-item>
                        </div>

                        <div class="input">
                            <p>电话</p>
                            <el-form-item prop="phone">
                            <div class="phone-code">
                                <input class="phone-code-input" type="text" v-model.trim="registerForm.phone">
                                <button type="button" class="phone-code-btn" @click="captcha()">{{codeInfo}}</button>
                            </div>
                            </el-form-item>
                        </div>
                        <div class="input">
                            <p>短信验证码</p>
                            <el-form-item prop="code" >
                            <input :disabled="!isDisabledCode" type="text" v-model.trim="registerForm.code">
                            </el-form-item>
                        </div>
                        <div class="input">
                            <p>密码</p>
                            <el-form-item prop="password">
                            <input :disabled="!isDisabledCode" type="password" v-model.trim="registerForm.password">
                            </el-form-item>
                        </div>
                        <div class="input">
                            <p>确认密码</p>
                            <el-form-item>
                            <input :disabled="!isDisabledCode" type="password" v-model.trim="passwordConfirm">
                            </el-form-item>
                        </div>
                 
                    </div>
                    </el-form>
                    <div class="left-btn">
                        <button type="button" class="btn-login" @click="register(registerFormRef)">注册</button>
                        <button type="button" class="btn-reg" @click="handleSignInNow">前往登录</button>
                    </div>
                </div>
            </div>
       
        </div>
        <div class="div-bottom">
                <span>备案：<span v-html="configStore.icp"></span></span>
                <span>站长：{{ configStore.author }}</span>
                <span @click="handleContact">联系我们</span>
                <span>关于本站</span>
                <span>建议反馈</span>
                <span>原创站点</span>
            </div>
    </div>
</template>
<script setup>
import { ref, reactive, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import useAuths from "@/hooks/useAuths";
import { getCodePhone } from "@/apis/accountApi";
import useUserStore from "@/stores/user";
import useConfigStore from "@/stores/config";

const configStore = useConfigStore();
const { loginFun, registerFun, loginSuccess } = useAuths();
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
const guestlogin = async () => {
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

// 注册逻辑
const isRegister = ref(true);
const registerFormRef = ref();
// 确认密码
const passwordConfirm = ref("");
const registerForm = reactive({
  userName: "",
  phone: "",
  password: "",
  uuid: "",
  code: "",
});
const registerRules = reactive({
  userName: [
    { required: true, message: "请输入用户名", trigger: "blur" },
    { min: 2, message: "用户名需大于两位", trigger: "blur" },
  ],
  phone: [{ required: true, message: "请输入手机号", trigger: "blur" }],
  code: [{ required: true, message: "请输入验证码", trigger: "blur" }],
  password: [
    { required: true, message: "请输入新的密码", trigger: "blur" },
    { min: 6, message: "密码需大于六位", trigger: "blur" },
  ],
});
const handleRegister = () => {
  isRegister.value = !isRegister.value;
};
const register = async (formEl) => {
  if (!formEl) return;
  await formEl.validate(async (valid) => {

    if (valid) {
      try {
        if (registerForm.password != passwordConfirm.value) {
          ElMessage.error("两次密码输入不一致");
          return;
        }
        await registerFun(registerForm);
        handleRegister();
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

//验证码
const codeInfo = ref("发送短信");
const isDisabledCode = ref(false);

const captcha = async () => {
  if (registerForm.phone !== "") {
    const { data } = await getCodePhone(registerForm.phone);
    registerForm.uuid = data.uuid;
    ElMessage({
      message: `已向${registerForm.phone}发送验证码，请注意查收`,
      type: "success",
    });
    isDisabledCode.value = true;
    let time = 60; //定义剩下的秒数
    let timer = setInterval(function () {
      if (time == 0) {
        //清除定时器和复原按钮
        clearInterval(timer);
        codeInfo.value = "发送验证码";
        time = 60; //这个10是重新开始
      } else {
        codeInfo.value = "剩余" + time + "秒";
        time--;
      }
    }, 1000);
  } else {
    ElMessage({
      message: `请先输入手机号`,
      type: "warning",
    });
  }
};

// 立即登录
const handleSignInNow = () => {
  isRegister.value = !isRegister.value;
};
// 获取图片验证码
const codeImageURL = computed(() => useUserStore().codeImageURL);
const handleGetCodeImage = () => {
  useUserStore().updateCodeImage();
};
onMounted(async () => {
  await useUserStore().updateCodeImage();
});

// 联系我们跳转对应页面
const handleContact = () => {
  router.push("/contact");
};

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
    window.close();
  }
});
</script>
<style src="@/assets/styles/login.css" scoped>
</style>