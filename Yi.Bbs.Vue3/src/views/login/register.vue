<script setup>

// 注册逻辑
import {computed, reactive, ref} from "vue";
import {getCodePhone} from "@/apis/accountApi";
import useAuths from "@/hooks/useAuths";
import {useRoute, useRouter} from "vue-router";
import useUserStore from "@/stores/user";
const { registerFun } = useAuths();
const router = useRouter();
const route = useRoute();
const registerFormRef = ref();
//验证码弹窗
const codeDialogVisible=ref(false);

// 获取图片验证码
const codeImageURL = computed(() => useUserStore().codeImageURL);
const codeUUid = computed(() => useUserStore().codeUUid);
// 确认密码
const passwordConfirm = ref("");
const registerForm = reactive({
  userName: "",
  phone: "",
  password: "",
  uuid: "",
  code: "",
  nick:""
});
const phoneForm=reactive({
  code:"",
  phone:"",
  uuid:codeUUid
});
const registerRules = reactive({
  nick: [
    { min: 2, message: "昵称需大于两位", trigger: "blur" },
  ],
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
        //注册成功返回登录
        handleSignInNow();
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

//点击验证码
const  handleGetCodeImage=()=>{
  useUserStore().updateCodeImage();
}

//点击手机发送短信
const  clickPhoneCaptcha=()=>{
  if (registerForm.phone !== "")
  {
    handleGetCodeImage();
    codeDialogVisible.value=true;
  }
  else {
    ElMessage({
      message: `请先输入手机号`,
      type: "warning",
    });
  }
}

//前往登录
const  handleSignInNow=()=>{
    router.push("/login");
}
const captcha = async () => {
  if (registerForm.phone!==""&&phoneForm.code!=="")
  {
    phoneForm.phone=registerForm.phone;
    const { data } = await getCodePhone(phoneForm);
    registerForm.uuid = data.uuid;
    codeDialogVisible.value=false;
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
  }
  else
  {
    ElMessage({
      message: `请先输入手机号`,
      type: "warning",
    });
  }
  
};
</script>

<template>
  <div class="container">
  <!-- 注册 -->
  <div class="div-content">
    <div class="div-right-register div-logo">
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

            <div style="display: flex;justify-content: space-between;margin: 0">
              <div class="input" style="width: 55%;margin: 0">
                <p>*登录账号</p>
                <el-form-item prop="userName">
                  <input type="text"    v-model.trim="registerForm.userName">
                </el-form-item>
              </div>

              <div class="input" style="width: 35%;margin: 0">
                <p>昵称</p>
                <el-form-item prop="nick">
                  <input type="text"    v-model.trim="registerForm.nick">
                </el-form-item>
              </div>
            </div>

            <div class="input" style="margin-top: 0">
              <p>*电话</p>
              <el-form-item prop="phone">
                <div class="phone-code">
                  <input class="phone-code-input" type="text" v-model.trim="registerForm.phone">
                  <button type="button" class="phone-code-btn" @click="clickPhoneCaptcha()">{{codeInfo}}</button>
                </div>
              </el-form-item>
            </div>
            <div class="input">
              <p>*短信验证码</p>
              <el-form-item prop="code" >
                <input :disabled="!isDisabledCode" type="text" v-model.trim="registerForm.code">
              </el-form-item>
            </div>
            <div class="input">
              <p>*密码</p>
              <el-form-item prop="password">
                <input :disabled="!isDisabledCode" type="password" v-model.trim="registerForm.password">
              </el-form-item>
            </div>
            <div class="input">
              <p>*确认密码</p>
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

    <el-dialog
        v-model="codeDialogVisible"
        title="发送短信"
        width="500"
        center
    >
      <div class="dialog-body">
      <img class="code-img" alt="加载中" @click="handleGetCodeImage"  :src="codeImageURL">

      <div class="input">
        <p>*图片验证码</p>
        <el-form-item prop="code" >
          <input type="text" v-model.trim="phoneForm.code">
        </el-form-item>
        <p style="color: red">由于国内短信严格程度在2025年5月连续加强3次，你的验证码有一定概率被运营商拦截</p>
        <p style="color: red">如果未收到验证码，请联系微信chengzilaoge520 站长进行手动创建</p>
      </div>
      </div>
      <template #footer>
        <div class="dialog-footer">
          <button @click="captcha" style="width:80% " type="button" class="phone-code-btn">确认发送</button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<style src="@/assets/styles/login.css" scoped>

</style>
<style scoped>
.dialog-body
{
  display: flex  !important;
  justify-content: center !important;
  padding: 0 !important;
}
.code-img{
  margin: 25px !important;
}
</style>