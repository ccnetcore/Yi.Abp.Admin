<template>
    <div class="login">
      <div class="login-box">
        <div class="left"></div>
        <div class="right">
          <div class="header-box">
            <div class="text" @click="guestlogin" v-if="isRegister">返回首页</div>
            <div class="text" @click="handleSignInNow" v-else>
              已有账号立即登录
            </div>
            <el-icon size="15"><DArrowRight /></el-icon>
          </div>
          <div class="top">
            <div class="title" v-if="isRegister">意社区登录 | SIGN IN</div>
            <div class="title" v-else>意社区注册 | REGISTER</div>
          </div>
          <div class="center">
            <div class="login-form">
              <el-form
                ref="loginFormRef"
                :model="loginForm"
                :rules="rules"
                v-if="isRegister"
              >
                <el-form-item label="用户名" class="title-item"></el-form-item>
                <el-form-item prop="userName">
                  <el-input
                    type="text"
                    v-model="loginForm.userName"
                    placeholder="请输入用户名"
                  />
                </el-form-item>
                <el-form-item label="密码" class="title-item"></el-form-item>
                <el-form-item prop="password">
                  <el-input
                    type="password"
                    v-model="loginForm.password"
                    placeholder="请输入密码"
                    show-password
                  />
                </el-form-item>
                <el-form-item label="验证码" class="title-item"></el-form-item>
                <div class="flex-between">
                  <el-col :span="18">
                    <el-form-item prop="phone">
                      <el-input
                        type="text"
                        v-model.trim="loginForm.code"
                        placeholder="请输入验证码"
                      />
                    </el-form-item>
                  </el-col>
                  <el-image
                    @click="handleGetCodeImage"
                    style="width: 120px; height: 40px; cursor: pointer"
                    :src="codeImageURL"
                    :fit="fit"
                  />
                </div>
              </el-form>
              <el-form
                class="registerForm"
                ref="registerFormRef"
                :model="registerForm"
                :rules="registerRules"
                v-else
              >
                <el-form-item label="用户名" class="title-item"></el-form-item>
                <el-form-item prop="userName">
                  <el-input
                    type="text"
                    v-model.trim="registerForm.userName"
                    placeholder="请输入用户名"
                  />
                </el-form-item>
                <el-form-item label="手机号" class="title-item"></el-form-item>
                <div class="flex-between">
                  <div class="item">
                    <el-form-item prop="phone">
                      <el-input
                        type="text"
                        v-model.trim="registerForm.phone"
                        placeholder="请输入手机号"
                      />
                    </el-form-item>
                  </div>
                  <el-button
                    type="primary"
                    @click="captcha"
                    :disabled="isDisabledCode"
                  >
                    {{ codeInfo }}
                  </el-button>
                </div>
                <el-form-item label="验证码" class="title-item"></el-form-item>
                <el-form-item prop="code">
                  <el-input
                    type="text"
                    v-model.trim="registerForm.code"
                    placeholder="请输入验证码"
                  />
                </el-form-item>
                <el-form-item label="新密码" class="title-item"></el-form-item>
                <el-form-item prop="password">
                  <el-input
                    type="password"
                    v-model.trim="registerForm.password"
                    placeholder="请输入新密码"
                  />
                </el-form-item>
                <el-form-item label="确认密码" class="title-item"></el-form-item>
                <el-form-item>
                  <el-input
                    type="password"
                    v-model.trim="passwordConfirm"
                    placeholder="请确认密码"
                    show-password
                  />
                </el-form-item>
              </el-form>
              <div class="link" v-if="isRegister">
                <div class="text" @click="handleRegister">没有账号？前往注册</div>
              </div>
              <div
                class="login-btn"
                @click="login(loginFormRef)"
                v-if="isRegister"
              >
                登 录
              </div>
              <div class="login-btn" @click="register(registerFormRef)" v-else>
                注 册
              </div>
            </div>
          </div>
          <div class="bottom" v-if="isRegister">
            <div class="title">
              <div>或者</div>
              <div>其他方式登录</div>
            </div>
            <div class="icon-list">
              <div class="icon" @click="handleQQLogin">
                <img src="@/assets/login_images/qq-setting.png" alt="" />
              </div>
              <div class="icon" @click="handleGiteeLogin">
                <img src="@/assets/login_images/gitee-setting.png" alt="" />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="login-footer">
        <div class="info">站长：{{ configStore.author }}</div>
        <div class="info btn" @click="handleContact">联系我们</div>
        <div class="info btn">关于本站</div>
        <div class="info btn">建议反馈</div>
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
  const codeInfo = ref("发送验证码");
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
        message: `清先输入手机号`,
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
  <style scoped lang="scss">
  .login {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
    background: url("@/assets/login_images/login_bg.jpg") no-repeat;
    &-box {
      display: flex;
      width: 70%;
      height: 80%;
      border-radius: 20px;
      background-color: #fff;
      box-shadow: 15px 15px 30px -10px rgba(0, 0, 0, 0.2),
        inset 20px 20px 15px rgba(255, 255, 255, 0.7);
      .left {
        width: 55%;
        height: 100%;
        display: flex;
        background: url("@/assets/login_images/welcome.jpg") no-repeat;
        background-size: 100% auto;
        background-position: 50%;
        border-right: 2px solid #eeefef;
      }
      .right {
        display: flex;
        flex-direction: column;
        width: 45%;
        padding: 40px 30px 40px 30px;
        border-radius: 20px;
        // color: #06035a;
  
        background-color: #fff;
        .header-box {
          cursor: pointer;
          margin-bottom: 20px;
          height: 10px;
          display: flex;
          justify-content: flex-end;
          align-items: center;
          color: #409eff;
        }
        .top {
          height: 40px;
          .title {
            font-size: 25px;
            font-weight: bold;
          }
          .text {
            margin-top: 10px;
          }
        }
        .center {
          flex: 1;
          .login-form {
            width: 100%;
            height: 100%;
            padding: 10px 0;
            display: flex;
            flex-direction: column;
            justify-content: space-around;
            .input-item {
              width: 100%;
              height: 45px;
              outline: none;
              border: 2px solid #dde0df;
              border-radius: 5px;
              padding: 0 10px;
              &:hover {
                outline: none;
              }
            }
            .login-btn {
              cursor: pointer;
              width: 100%;
              height: 45px;
              color: #fff;
              text-align: center;
              line-height: 50px;
              border-radius: 5px;
              background-color: #2282fe;
            }
            .link {
              margin-bottom: 10px;
              display: flex;
              justify-content: space-between;
              align-items: center;
              .text {
                cursor: pointer;
                color: #2282fe;
              }
            }
            .visitor {
              margin-top: 10px;
            }
            .registerForm {
              :deep(.el-form-item) {
                margin-bottom: 5px;
              }
            }
          }
        }
        .bottom {
          width: 100%;
          height: 150px;
          display: flex;
          flex-direction: column;
          justify-content: center;
          .title {
            > div {
              text-align: center;
              margin: 10px;
            }
          }
          .icon-list {
            margin-top: 10px;
            width: 100%;
            display: flex;
            justify-content: center;
            .icon {
              cursor: pointer;
              width: 25px;
              height: 25px;
              margin: 0 10px;
              img {
                width: 100%;
                height: 100%;
              }
            }
          }
        }
      }
    }
    &-footer {
      width: 70%;
      display: flex;
      justify-content: flex-end;
      margin-top: 20px;
      color: #eee;
      font-size: 14px;
      .info {
        margin: 0 10px;
      }
      .btn {
        cursor: pointer;
        &:hover {
          color: #fff;
        }
      }
    }
  
    :deep(.title-item) {
      margin-bottom: 0;
    }
    .flex-between {
      display: flex;
      justify-content: space-between;
      .item {
        flex: 1;
      }
    }
  }
  /*手机端CSS*/
  @media (max-width: 768px) {
    .login {
      background: url("@/assets/login_images/phone_login_bg.jpg") no-repeat;
      &-box {
        .left {
          display: none;
        }
        .right {
          flex: 1;
          .top {
            .title {
              font-size: 20px;
            }
            .text {
              margin-top: 10px;
            }
          }
        }
        .flex-between {
          display: flex;
          justify-content: space-between;
          .item {
            flex: 1;
          }
        }
      }
    }
  }
  </style>
  