<template>
  <el-form ref="userRef" :model="user" :rules="rules" label-width="80px">
    <el-form-item label="账号" prop="userName">
      <el-input v-model="user.userName" disabled />
    </el-form-item>
    <el-form-item label="用户昵称" prop="nick">
      <el-input v-model="user.nick" maxlength="30" :disabled="isDisable"/>
    </el-form-item>
    <el-form-item label="手机号码" prop="phone">
      <el-input v-model="user.phone" maxlength="11" :disabled="isDisable" />
    </el-form-item>
    <el-form-item label="邮箱" prop="email">
      <el-input v-model="user.email" maxlength="50" :disabled="isDisable" />
    </el-form-item>
    <el-form-item label="性别">
      <el-radio-group v-model="user.sex" :disabled="isDisable">
        <el-radio :label="'Male'">男</el-radio>
        <el-radio :label="'Woman'">女</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item v-show="!isDisable">
      <el-button type="primary" @click="submit">保存</el-button>
      <el-button type="danger" @click="close">关闭</el-button>
    </el-form-item>
  </el-form>
</template>

<script setup>
import { updateUserProfile } from "@/apis/userApi";
import useUserStore from "@/stores/user";
import { ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const props = defineProps({
  user: {
    type: Object,
  },
  isDisable:{
    type:Boolean,
    default:true
  }
});

const userRef = ref(null);

const rules = ref({
  nick: [{ required: true, message: "用户昵称不能为空", trigger: "blur" }],
  email: [
    { required: true, message: "邮箱地址不能为空", trigger: "blur" },
    {
      type: "email",
      message: "请输入正确的邮箱地址",
      trigger: ["blur", "change"],
    },
  ],
  phone: [
    { required: true, message: "手机号码不能为空", trigger: "blur" },
    {
      pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
      message: "请输入正确的手机号码",
      trigger: "blur",
    },
  ],
});

/** 提交按钮 */
function submit() {
  userRef.value.validate(async (valid) => {
    if (valid) {
      const response = await updateUserProfile(props.user);
      if (response.status == 200) {
        ElMessage({
          type: "success",
          message: "用户信息修改成功",
        });
        // 更新用户信息
        await useUserStore().getInfo(); // 用户信息
      }
    }
  });
}
/** 关闭按钮 */
function close() {
  router.push("/index");
}
</script>
