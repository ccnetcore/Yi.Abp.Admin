<template>
  <div class="discuss-box">
    <div class="header">
      <el-form :inline="true">
        <el-form-item label="标题：">
          <el-input v-model="query.title" placeholder="请输入标题"></el-input>
        </el-form-item>

        <el-form-item label="标签：">
          <el-input placeholder="搜索当下分类下的标签" />
        </el-form-item>
        <div class="form-right">
          <el-button @click="handleReset">重置</el-button>
          <el-button
            type="primary"
            @click="
              async () => {
                await loadDiscussList();
              }
            "
            >查询</el-button
          >
          <el-button
            @click="enterEditArticle"
            type="primary"
            :class="[!isEditArticle ? 'el-button--disabled' : '']"
            >发布主题</el-button
          >
          <el-dropdown>
            <span class="el-dropdown-link">
              展开
              <el-icon class="el-icon--right">
                <arrow-down />
              </el-icon>
            </span>

            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item>Action 1</el-dropdown-item>
                <el-dropdown-item>Action 2</el-dropdown-item>
                <el-dropdown-item>Action 3</el-dropdown-item>
                <el-dropdown-item disabled>Action 4</el-dropdown-item>
                <el-dropdown-item divided>Action 5</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-form>
    </div>
    <Tabs v-model="activeName" :tabList="tabList" @tab-change="handleClick" />
    <div class="div-item" v-for="i in topDiscussList" :key="i.id">
      <DisscussCard :discuss="i" badge="置顶" />
    </div>
    <template v-if="isDiscussFinished">
      <div class="div-item" v-for="i in discussList" :key="i.id">
        <DisscussCard :discuss="i" />
      </div>
    </template>
    <template v-else>
      <Skeleton :isBorder="true" />
    </template>
    <div>
      <el-pagination
        v-model:current-page="query.skipCount"
        v-model:page-size="query.maxResultCount"
        :page-sizes="[10, 20, 30, 50]"
        :background="true"
        layout="total, sizes, prev, pager, next, jumper"
        :total="total"
        @size-change="
          async (val) => {
            await loadDiscussList();
          }
        "
        @current-change="
          async (val) => {
            await loadDiscussList();
          }
        "
      />
    </div>

    <el-empty v-if="discussList.length == 0" description="空空如也" />
    <BottomInfo />
  </div>
</template>

<script setup>
import DisscussCard from "@/components/DisscussCard.vue";
import { getList } from "@/apis/discussApi.js";
import { ref, reactive, watch, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import BottomInfo from "@/components/BottomInfo.vue";
import { getPermission } from "@/utils/auth";
import useAuths from "@/hooks/useAuths";
import { Session } from "@/utils/storage";
import Skeleton from "@/components/Skeleton/index.vue";
import Tabs from "./components/tabs.vue";
import { cloneDeep } from "lodash";

const { getToken, clearStorage } = useAuths();
//数据定义
const route = useRoute();
const router = useRouter();
const activeName = ref("new");
//主题内容
const discussList = ref([]);
const isDiscussFinished = ref(false);
//置顶主题内容
const topDiscussList = ref([]);
const total = ref(100);
const query = reactive({
  skipCount: 1,
  maxResultCount: 10,
  title: "",
  isTop: false,
  plateId: route.params.plateId,
  type: activeName.value,
});

const handleClick = async (item) => {
  query.type = activeName.value;
  await loadDiscussList();
};

// onMounted(async () => {
//   if (route.query.q != undefined) {
//     query.title = route.query.q ?? "";
//     router.push("/discuss");
//   }
//   await loadDiscussList();
// });

// 重置
const handleReset = () => {
  query.skipCount = 1;
  query.maxResultCount = 10;
  query.title = "";
  query.plateId = route.params.plateId;
  query.type = activeName.value;
  loadDiscussList();
};

//加载discuss
const loadDiscussList = async () => {
  const response = await getList(query);
  isDiscussFinished.value = response.config.isFinish;
  discussList.value = response.data.items;
  total.value = Number(response.data.totalCount);

  //全查，无需参数
  const topParams = cloneDeep(query);
  topParams.isTop = true;
  const topResponse = await getList(topParams);
  topDiscussList.value = topResponse.data.items;
};

//进入添加主题页面
const { isHasPermission: isEditArticle } = getPermission(
  "bbs:discuss:add",
  route.params.isPublish === "false" ? false : true
);

const enterEditArticle = () => {
  const hasToken = getToken();
  if (isEditArticle) {
    //跳转路由
    var routerPer = {
      path: "/editArt",
      query: {
        operType: "create",
        artType: "discuss",
        plateId: route.params.plateId,
      },
    };
    router.push(routerPer);
  } else if (!hasToken) {
    ElMessageBox.confirm("登录后即可发布主题是否登录?", "提示", {
      confirmButtonText: "确认",
      cancelButtonText: "取消",
      type: "warning",
    }).then(() => {
      clearStorage();
      Session.set("currentPath", route.path);
      router.push("/login");
    });
  } else {
    ElMessage.warning("暂无发布权限!");
  }
};

watch(
  () => route.query.q,
  async (val) => {
    if (val) {
      query.title = val ?? "";
    }
    loadDiscussList();
  },
  { immediate: true }
);

const tabList = ref([
  { label: "全部文章", name: "suggest", position: "left" },
  { label: "最新", name: "new", position: "right" },
  { label: "推荐", name: "suggest", position: "right" },
  { label: "最热", name: "host", position: "right" },
]);
</script>
<style scoped lang="scss">
.discuss-box {
  width: 1300px;
  height: 100%;
  .el-pagination {
    margin: 2rem 0rem 2rem 0rem;
    justify-content: right;
  }
  /* .body-div {
  min-height: 1000px;
} */
  .el-dropdown-link {
    cursor: pointer;
    color: var(--el-color-primary);
    display: flex;
    align-items: center;
  }
  .header {
    background-color: #ffffff;
    padding: 1rem;
    margin: 1rem 0rem;
  }

  .header-tab {
    margin-bottom: 1rem;
  }
  .collapse-top {
    padding-left: 2rem;
  }
  .header .el-input {
  }
  .el-tabs {
    background-color: #ffffff;
    padding-left: 2rem;
  }
  .el-tabs >>> .el-tabs__header {
    margin-bottom: 0;
  }
  .div-item {
    margin-bottom: 1rem;
  }

  .el-form {
    --el-form-label-font-size: var(--el-font-size-base);
    display: flex;
    align-items: center;
  }
  .el-form-item {
    padding-top: 0.8rem;
  }
  .form-right {
    align-items: center;
    display: flex;
    margin-left: auto;
  }
  .form-right .el-button {
    margin-right: 0.6rem;
  }
  .header .el-input {
    width: 20rem;
  }
  .collapse-list >>> .el-collapse-item__header {
    border-bottom-color: #f0f2f5 !important;
  }

  .el-divider {
    margin: 0.5rem 0;
  }
}

/* 禁用状态下的样式 */
.el-button.el-button--disabled {
  opacity: 0.6;
  pointer-events: auto;
}
</style>
