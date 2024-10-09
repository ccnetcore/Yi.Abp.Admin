<template>
  <div style="width: 1300px">
    <div class="body-div">
      <el-form
        label-width="120px"
        :model="editForm"
        label-position="left"
        :rules="rules"
        ref="ruleFormRef"
      >
        <el-form-item label="类型：">
          <el-radio-group v-model="radio">
            <el-radio-button label="discuss" :disabled="artType !== 'discuss'"
              >主题</el-radio-button
            >
            <el-radio-button label="article" :disabled="artType !== 'article'"
              >文章</el-radio-button
            >
            <el-radio-button label="plate" :disabled="artType !== 'plate'"
              >板块</el-radio-button
            >
            <el-radio-button label="orther" :disabled="artType !== 'orther'"
              >其他</el-radio-button
            >
          </el-radio-group>
        </el-form-item>

        <el-form-item label="权限：" v-if="radio == 'discuss'">
          <el-radio-group v-model="perRadio">
            <el-radio-button label="Public">公开</el-radio-button>
            <el-radio-button label="Oneself">仅自己可见</el-radio-button>
            <el-radio-button label="User">部分用户可见</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item
          label="可见用户："
          v-if="radio == 'discuss' && perRadio == 'User'"
        >
          <UserSelectInfo v-model="editForm.permissionUserIds" />
        </el-form-item>

        <el-form-item
          v-if="radio == 'article'"
          label="子文章名称："
          prop="name"
        >
          <el-input placeholder="请输入" v-model="editForm.name" />
        </el-form-item>
        <el-form-item v-else label="标题：" prop="title">
          <el-input placeholder="请输入" v-model="editForm.title" />
        </el-form-item>
        <el-form-item label="描述：" prop="introduction">
          <el-input placeholder="请输入" v-model="editForm.introduction" />
        </el-form-item>
        <el-form-item label="内容：" prop="content">
          <MavonEdit
            height="30rem"
            v-model="editForm.content"
            :codeStyle="codeStyle"
          />
        </el-form-item>
        <el-form-item label="封面：" v-if="radio == 'discuss'">

          <el-image
              v-if="dialogImageUrl"
              :src="getUrl"
              style="width: 178px; height: 178px"
              class="avatar"
          />
          
          <!-- 主题封面选择 -->
          <el-upload
            class="avatar-uploader"
            :action="fileUploadUrl"
            :show-file-list="false"
            :on-success="onSuccess"
          >
            <el-icon  class="avatar-uploader-icon"><Plus /></el-icon>
          </el-upload>
        </el-form-item>
        <el-form-item label="标签：" prop="types">
          <el-input placeholder="请输入" v-model="editForm.types" />
        </el-form-item>
        <el-form-item>
          <el-button
            @click="submit(ruleFormRef)"
            class="submit-btn"
            type="primary"
            >提交</el-button
          ></el-form-item
        >
      </el-form>
      <div class="import-content" v-show="radio == 'article'">
        <div class="text">上传类型：</div>
        <el-select
          v-model="currentType"
          placeholder="请选择"
          style="width: 120px"
        >
          <el-option
            v-for="item in typeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
        <el-button
          type="primary"
          :icon="Download"
          :loading="importLoading"
          @click="handleImport"
          class="import-btn"
          >导入文章</el-button
        >
      </div>
    </div>
    <!-- 文件弹框 -->
    <div>
      <input
        v-show="false"
        ref="fileRef"
        type="file"
        multiple
        @change="getFile"
      />
    </div>
  </div>
</template>
<script setup>
import MavonEdit from "@/components/MavonEdit.vue";
import UserSelectInfo from "@/components/UserSelectInfo.vue";
import {ref, reactive, onMounted, computed} from "vue";
import { useRoute, useRouter } from "vue-router";
import { Plus, Download } from "@element-plus/icons-vue";

import {
  add as discussAdd,
  update as discussUpdate,
  get as discussGet,
} from "@/apis/discussApi.js";

import {
  add as articleAdd,
  update as articleUpdate,
  get as articleGet,
  importArticle,
} from "@/apis/articleApi.js";

//数据定义
const route = useRoute();
const router = useRouter();
const perRadio = ref("Public");
const radio = ref(route.query.artType);
const codeStyle = "atom-one-dark";

// 用于禁用判断
const artType = ref(route.query.artType);

//封面完整显示的url
const fileUploadUrl = `${import.meta.env.VITE_APP_BASEAPI}/file`;
//封面的url
const dialogImageUrl = ref("");

//文件上传成功后
const onSuccess = (response) => {
  dialogImageUrl.value = response[0].id;
};

//封面url
const getUrl=computed(()=>{
  return `${import.meta.env.VITE_APP_BASEAPI}/file/${dialogImageUrl.value}`;
})


//整个页面上的表单
const editForm = reactive({
  title: "",
  types: "",
  introduction: "",
  content: "",
  name: "",
  permissionUserIds: [],
});

//组装主题内容： 需要更新主题信息
const discuss = {};

//组装文章内容：需要添加的文章信息
const article = {};

//定义效验规则
const ruleFormRef = ref(null);
const rules = reactive({
  title: [
    { required: true, message: "请输入标题", trigger: "blur" },
    { min: 3, max: 40, message: "长度 3 到 20", trigger: "blur" },
  ],
  name: [{ required: true, message: "请输入子文章名称", trigger: "blur" }],
  content: [
    { required: true, message: "请输入内容", trigger: "blur" },
    { min: 10, message: "长度 大于 10", trigger: "blur" },
  ],
});
//提交按钮,需要区分操作类型
const submit = async (formEl) => {
  if (!formEl) return;
  await formEl.validate(async (valid, fields) => {
    if (valid) {
      //dicuss主题处理
      if (radio.value == "discuss") {
        discuss.title = editForm.title;
        discuss.types = editForm.types;
        discuss.introduction = editForm.introduction;
        discuss.content = editForm.content;
        discuss.plateId = discuss.plateId ?? route.query.plateId;
        discuss.cover = dialogImageUrl.value;
        discuss.permissionType = perRadio.value;

        discuss.permissionUserIds = editForm.permissionUserIds;
        //主题创建
        if (route.query.operType == "create") {
          const response = await discussAdd(discuss);

          ElMessage({
            message: `[${discuss.title}]主题创建成功！`,
            type: "success",
          });
          var routerPer = { path: `/article/${response.data.id}` };
          router.push(routerPer);
        }
        //主题更新
        else if (route.query.operType == "update") {
          await discussUpdate(route.query.discussId, discuss);

          ElMessage({
            message: `[${discuss.title}]主题更新成功！`,
            type: "success",
          });
          var routerPer = { path: `/article/${route.query.discussId}` };
          router.push(routerPer);
        }
      }

      //artcle文章处理
      else if (radio.value == "article") {
        //组装文章内容：需要添加的文章信息
        article.content = editForm.content;
        article.name = editForm.name;
        article.discussId = route.query.discussId;
        article.parentId = route.query.parentArticleId;
        //文章创建
        if (route.query.operType == "create") {
          const response = await articleAdd(article);
          ElMessage({
            message: `[${article.name}]文章创建成功！`,
            type: "success",
          });
          var routerPer = {
            path: `/article/${route.query.discussId}/${response.data.id}`,
          };
          router.push(routerPer);
        }
        //文章更新
        else if (route.query.operType == "update") {
          await articleUpdate(route.query.articleId, article);
          ElMessage({
            message: `[${article.name}]文章更新成功！`,
            type: "success",
          });
          var routerPer = {
            path: `/article/${route.query.discussId}/${route.query.articleId}`,
          };
          router.push(routerPer);
        }
      }
      //添加成功后跳转到该页面
      // var routerPer = { path: `/discuss/${discuss.plateId}` };
      // router.push(routerPer);
      // ruleFormRef.value.resetFields();
      // discuss.plateId = route.query.plateId;
    }
  });
};

onMounted(async () => {
  //如果是更新操作，需要先查询
  if (route.query.operType == "update") {
    //更新主题
    if (radio.value == "discuss") {
      await loadDiscuss();

      //更新文章
    } else if (radio.value == "article") {
      await loadArticle();
    }
  }
});
//加载主题
const loadDiscuss = async () => {
  const response = await discussGet(route.query.discussId);
  const res = response.data;
  editForm.content = res.content;
  editForm.title = res.title;
  editForm.types = res.types;
  editForm.introduction = res.introduction;
  discuss.plateId = res.plateId;
  dialogImageUrl.value = res.cover;
  perRadio.value = res.permissionType;
  editForm.permissionUserIds = res.permissionUserIds;
};
//加载文章
const loadArticle = async () => {
  const response = await articleGet(route.query.articleId);
  const res = response.data;
  editForm.content = res.content;
  editForm.name = res.name;
  editForm.discussId = res.discussId;
};

// 导入
let importLoading = ref(false);
const fileRef = ref(null);
const handleImport = async () => {
  fileRef.value.click();
};
const currentType = ref("Default");
const typeOptions = [
  {
    value: "Default",
    label: "默认",
  },
  {
    value: "VuePress",
    label: "VuePress",
  },
];
const getFile = async (e) => {
  importLoading.value = true;
  try {
    let formData = new FormData();
    for (let i = 0; i < e.target.files.length; i++) {
      formData.append("file", e.target.files[i]);
    }
    await importArticle(
      {
        discussId: route.query.discussId,
        articleParentId: route.query.parentArticleId,
        importType: currentType.value,
      },
      formData
    );
    ElMessage({
      message: `导入成功！`,
      type: "success",
    });
    importLoading.value = false;
    const routerPer = { path: `/article/${route.query.discussId}` };
    router.push(routerPer);
  } catch (error) {
    ElMessage.error(error.message);
    importLoading.value = false;
  }
};
</script>
<style scoped>
.submit-btn {
  width: 40%;
}

.body-div {
  position: relative;
  min-height: 1000px;
  background-color: #fff;
  margin: 1.5rem;
  padding: 1.5rem;
  .import-content {
    display: flex;
    align-items: center;
    position: absolute;
    top: 1.5rem;
    right: 1.5rem;
    .text {
      margin-right: 10px;
    }
  }
  .import-btn {
    margin-left: 10px;
  }
}

.avatar-uploader >>> .el-upload {
  border: 1px dashed var(--el-border-color);
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: var(--el-transition-duration-fast);
}

.avatar-uploader >>> .el-upload:hover {
  border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  text-align: center;
}
.el-upload {
}
</style>
