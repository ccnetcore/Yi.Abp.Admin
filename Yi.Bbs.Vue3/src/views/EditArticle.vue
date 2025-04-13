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
            >主题
            </el-radio-button>
            <el-radio-button label="article" :disabled="artType !== 'article'"
            >文章
            </el-radio-button
            >
            <el-radio-button label="plate" :disabled="artType !== 'plate'"
            >板块
            </el-radio-button
            >
            <el-radio-button label="orther" :disabled="artType !== 'orther'"
            >其他
            </el-radio-button
            >
          </el-radio-group>
        </el-form-item>

        <el-form-item label="主题类型：" v-if="radio == 'discuss'">
          <el-radio-group v-model="editForm.discussType">
            <el-radio-button label="Article">基础</el-radio-button>
            <el-radio-button label="Reward">有偿悬赏</el-radio-button>
            <el-radio-button label="Test" :disabled="true">问答</el-radio-button>
            <el-radio-button label="Test" :disabled="true">投票</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="悬赏金额：" v-if="editForm.discussType == 'Reward'">
          <div class="reward-div">
            <p style="color: red">通过”悬赏主题“发布问题，双方达成一致并解决问题后，建议有偿提供RMB给解决人员</p>
            <p style="color: red">社区只提供解决问题平台，不参与任何交易，请自行联系</p>
          <div>
            <span>最少RMB：</span>
            <el-input-number style="margin-right: 40px;" v-model="editForm.rewardData.minValue" :min="10">
            </el-input-number>

            <span>最多RMB：</span>
            <el-input-number v-model="editForm.rewardData.maxValue" :min="10">
            </el-input-number>
          </div>
          
    </div>
    </el-form-item>
        <el-form-item label="联系方式："  v-if="editForm.discussType == 'Reward'">
          <el-input v-model="editForm.rewardData.contact"  placeholder="输入你的联系方式（例如微信号/QQ号）" />
        </el-form-item>

        <el-form-item label="权限：" v-if="radio == 'discuss'">
      <el-radio-group v-model="perRadio">
        <el-radio-button label="Public">公开</el-radio-button>
        <el-radio-button label="Role">所选角色可见</el-radio-button>
      </el-radio-group>
    </el-form-item>
    <el-form-item
        label="可见角色："
        v-if="radio == 'discuss' && perRadio == 'Role'"
    >
      <el-input-tag
          v-model="editForm.permissionRoleCodes"
          placeholder="请输入角色code"
          aria-label="按下回车，可选择多个"
      />
    </el-form-item>

    <el-form-item
        v-if="radio == 'article'"
        label="子文章名称："
        prop="name"
    >
      <el-input placeholder="请输入" v-model="editForm.name"/>
    </el-form-item>
    <el-form-item v-else label="标题：" prop="title">
      <el-input placeholder="请输入" v-model="editForm.title"/>
    </el-form-item>
    <el-form-item label="描述：" prop="introduction">
      <el-input placeholder="请输入" v-model="editForm.introduction"/>
    </el-form-item>
    <el-form-item label="内容：" prop="content">
      <MavonEdit
          height="30rem"
          v-model="editForm.content"
          :codeStyle="codeStyle"
      />
    </el-form-item>
    <el-form-item label="首页：" v-if="radio == 'discuss'">

      <el-image
          v-if="dialogImageUrl"
          :src="getUrl"
          style="width: 178px; height: 178px"
          class="avatar"
      />

      <!-- 主题首页选择 -->
      <el-upload
          class="avatar-uploader"
          :action="fileUploadUrl"
          :show-file-list="false"
          :on-success="onSuccess"
      >
        <el-icon class="avatar-uploader-icon">
          <Plus/>
        </el-icon>
      </el-upload>
    </el-form-item>
    <el-form-item label="分类标签：" prop="discussLable" v-if="radio == 'discuss'">
      <el-select
          value-key="id"
          v-model="selectLabelList"
          multiple
          filterable
          remote
          reserve-keyword
          placeholder="请选择合适的文章标签"
          :remote-method="remoteMethod"
          :loading="labelLoading"
          style="width: 435px"
      >
        <el-option
            v-for="item in labelListData"
            :key="item.id"
            :label="item.name"
            :value="item"
        />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button
          @click="submit(ruleFormRef)"
          class="submit-btn"
          type="primary"
      >提交
      </el-button
      >
    </el-form-item
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
      >导入文章
      </el-button
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
import {ref, reactive, onMounted, computed} from "vue";
import {useRoute, useRouter} from "vue-router";
import {Plus, Download} from "@element-plus/icons-vue";

import {
  add as discussAdd,
  update as discussUpdate,
  get as discussGet,
} from "@/apis/discussApi.js";

import {
  getAllList as getLableAllList
} from "@/apis/discussLableApi"

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

//远程文章标签
const labelListData = ref([]);
const labelLoading = ref(false);
const selectLabelList = ref([]);
//远程下拉框查询文章标签
const remoteMethod = async (query) => {
  labelLoading.value = true
  const {data} = await getLableAllList({name: query});
  labelLoading.value = false
  labelListData.value = data.items;
}

//文件上传成功后
const onSuccess = (response) => {
  dialogImageUrl.value = response[0].id;
};

//封面url
const getUrl = computed(() => {
  return `${import.meta.env.VITE_APP_BASEAPI}/file/${dialogImageUrl.value}`;
})


//整个页面上的表单
const editForm = reactive({

  discussType: "Article",
  title: "",
  introduction: "",
  content: "",
  name: "",
  permissionRoleCodes: [],
  discussLableIds: [],
  rewardData: {
    minValue: 10,
    maxValue: 200,
    contact:""
  }
});

//组装主题内容： 需要更新主题信息
const discuss = {};

//组装文章内容：需要添加的文章信息
const article = {};

//定义效验规则
const ruleFormRef = ref(null);
const rules = reactive({
  title: [
    {required: true, message: "请输入标题", trigger: "blur"},
    {min: 3, max: 40, message: "长度 3 到 20", trigger: "blur"},
  ],
  name: [{required: true, message: "请输入子文章名称", trigger: "blur"}],
  content: [
    {required: true, message: "请输入内容", trigger: "blur"},
    {min: 10, message: "长度 大于 10", trigger: "blur"},
  ],
});
//提交按钮,需要区分操作类型
const submit = async (formEl) => {
  if (!formEl) return;
  await formEl.validate(async (valid, fields) => {
    if (valid) {
      //dicuss主题处理
      if (radio.value == "discuss") {
        discuss.discussType=editForm.discussType;
        discuss.discussLableIds = selectLabelList.value.map((item) => item.id);
        discuss.title = editForm.title;
        discuss.introduction = editForm.introduction;
        discuss.content = editForm.content;
        discuss.plateId = discuss.plateId ?? route.query.plateId;
        discuss.cover = dialogImageUrl.value;
        discuss.permissionType = perRadio.value;
        discuss.permissionRoleCodes = editForm.permissionRoleCodes;
        //悬赏还需要新增参数
        if (editForm.discussType == 'Reward')
        {
          discuss.rewardData= editForm.rewardData;
          if (editForm.rewardData.contact=="")
          {
            ElMessage.error("悬赏的联系方式不能为空!");
            return;
          }
          if (editForm.rewardData.maxValue<editForm.rewardData.minValue)
          {
            ElMessage.error("悬赏设置的最多金额不能小于最少金额");
            return;
          }
        }
          //主题创建
        if (route.query.operType == "create") {
          const response = await discussAdd(discuss);

          ElMessage({
            message: `[${discuss.title}]主题创建成功！`,
            type: "success",
          });
          var routerPer = {path: `/article/${response.data.id}`};
          router.push(routerPer);
        }
        //主题更新
        else if (route.query.operType == "update") {
          discuss.discussLableIds = selectLabelList.value.map((item) => item.id);
          await discussUpdate(route.query.discussId, discuss);

          ElMessage({
            message: `[${discuss.title}]主题更新成功！`,
            type: "success",
          });
          var routerPer = {path: `/article/${route.query.discussId}`};
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
  editForm.introduction = res.introduction;
  editForm.discussLableIds = res.discussLableIds;
  editForm.permissionRoleCodes = res.permissionRoleCodes;

  //编辑状态，已选择的就是全部
  labelListData.value = res.lables;
  selectLabelList.value = res.lables;

  discuss.plateId = res.plateId;
  dialogImageUrl.value = res.cover;
  perRadio.value = res.permissionType;

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
    const routerPer = {path: `/article/${route.query.discussId}`};
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

.reward-div {
  display: flex;
  flex-direction: column;
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
