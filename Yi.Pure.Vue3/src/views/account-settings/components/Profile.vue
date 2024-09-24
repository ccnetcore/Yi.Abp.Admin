<script setup lang="ts">
import { reactive, ref } from "vue";
import { formUpload } from "@/api/mock";
import { message } from "@/utils/message";
import type { FormInstance, FormRules } from "element-plus";
import ReCropperPreview from "@/components/ReCropperPreview";
import { createFormData, deviceDetection } from "@pureadmin/utils";
import uploadLine from "@iconify-icons/ri/upload-line";
import { useUserStoreHook } from "@/store/modules/user";
import {
  getUserProfile,
  updateUserIcon,
  updateUserProfile
} from "@/api/system/user";
import userAvatar from "@/assets/user.jpg";
import { getFileUrl } from "@/utils/file";
import { uploadFile } from "@/api/file";

defineOptions({
  name: "Profile"
});

const imgSrc = ref("");
const cropperBlob = ref();
const cropRef = ref();
const uploadRef = ref();
const isShow = ref(false);
const userInfoFormRef = ref<FormInstance>();

const userInfos = reactive({
  icon: "",
  nick: "",
  email: "",
  phone: "",
  introduction: ""
});

const rules = reactive<FormRules>({
  userName: [{ required: true, message: "用户名必填", trigger: "blur" }],
  nick: [{ required: true, message: "昵称必填", trigger: "blur" }]
});

function queryEmail(queryString, callback) {
  const emailList = [
    { value: "@qq.com" },
    { value: "@126.com" },
    { value: "@163.com" }
  ];
  let results = [];
  let queryList = [];
  emailList.map(item =>
    queryList.push({ value: queryString.split("@")[0] + item.value })
  );
  results = queryString
    ? queryList.filter(
        item =>
          item.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0
      )
    : queryList;
  callback(results);
}

const onChange = uploadFile => {
  const reader = new FileReader();
  reader.onload = e => {
    imgSrc.value = e.target.result as string;
    isShow.value = true;
  };
  reader.readAsDataURL(uploadFile.raw);
};

const handleClose = () => {
  cropRef.value.hidePopover();
  uploadRef.value.clearFiles();
  isShow.value = false;
};

const onCropper = ({ blob }) => (cropperBlob.value = blob);

const handleSubmitImage = () => {
  const formData = createFormData({
    files: new File([cropperBlob.value], "file")
  });
  uploadFile(formData)
    .then(async ({ status, data }) => {
      if (status == 200) {
        await updateUserIcon(data[0].id);
        useUserStoreHook().SET_AVATAR(data[0].id);
        message("更新头像成功", { type: "success" });
        handleClose();
      } else {
        message("更新头像失败");
      }
    })
    .catch(error => {
      message(`提交异常 ${error}`, { type: "error" });
    });
};

// 更新信息
const onSubmit = async (formEl: FormInstance) => {
  await formEl.validate(async (valid, fields) => {
    if (valid) {
      await updateUserProfile(userInfos);
      message("更新信息成功", { type: "success" });
    } else {
      console.log("error submit!", fields);
    }
  });
};
getUserProfile().then(res => {
  Object.assign(userInfos, res.data.user);
});
</script>

<template>
  <div
    :class="[
      'min-w-[180px]',
      deviceDetection() ? 'max-w-[100%]' : 'max-w-[70%]'
    ]"
  >
    <h3 class="my-8">个人信息</h3>
    <el-form
      ref="userInfoFormRef"
      label-position="top"
      :rules="rules"
      :model="userInfos"
    >
      <el-form-item label="头像">
        <el-avatar :size="80" :src="getFileUrl(userInfos.icon, userAvatar)" />
        <el-upload
          ref="uploadRef"
          accept="image/*"
          action="#"
          :limit="1"
          :auto-upload="false"
          :show-file-list="false"
          :on-change="onChange"
        >
          <el-button plain class="ml-4">
            <IconifyIconOffline :icon="uploadLine" />
            <span class="ml-2">更新头像</span>
          </el-button>
        </el-upload>
      </el-form-item>
      <el-form-item label="昵称" prop="nickname">
        <el-input v-model="userInfos.nick" placeholder="请输入昵称" />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-autocomplete
          v-model="userInfos.email"
          :fetch-suggestions="queryEmail"
          :trigger-on-focus="false"
          placeholder="请输入邮箱"
          clearable
          class="w-full"
        />
      </el-form-item>
      <el-form-item label="联系电话">
        <el-input
          v-model="userInfos.phone"
          placeholder="请输入联系电话"
          clearable
        />
      </el-form-item>
      <el-form-item label="简介">
        <el-input
          v-model="userInfos.introduction"
          placeholder="请输入简介"
          type="textarea"
          :autosize="{ minRows: 6, maxRows: 8 }"
          maxlength="56"
          show-word-limit
        />
      </el-form-item>
      <el-button type="primary" @click="onSubmit(userInfoFormRef)">
        更新信息
      </el-button>
    </el-form>
    <el-dialog
      v-model="isShow"
      width="40%"
      title="编辑头像"
      destroy-on-close
      :closeOnClickModal="false"
      :before-close="handleClose"
      :fullscreen="deviceDetection()"
    >
      <ReCropperPreview ref="cropRef" :imgSrc="imgSrc" @cropper="onCropper" />
      <template #footer>
        <div class="dialog-footer">
          <el-button bg text @click="handleClose">取消</el-button>
          <el-button bg text type="primary" @click="handleSubmitImage">
            确定
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>
