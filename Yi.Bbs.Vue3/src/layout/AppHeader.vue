<template>
  <div class="header">
    <div class="logo" @click="enterIndex">
      <div class="image">
        <img class="img-icon" src="@/assets/common/icons/logo.ico" />
      </div>
      <div class="text">{{ configStore.name }}</div>
    </div>
    <div class="tab">
      <el-menu
        :default-active="activeIndex"
        mode="horizontal"
        :ellipsis="false"
        @select="handleSelect"
      >
        <el-menu-item index="1" @click="enterIndex">主页</el-menu-item>
        <el-sub-menu index="2">
          <template #title>学习</template>
          <el-menu-item index="2-1">学习 one</el-menu-item>
          <el-menu-item index="2-2">学习 two</el-menu-item>
          <el-menu-item index="2-3">学习 three</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="3">
          <template #title>资源</template>
          <el-menu-item index="3-1">资源 one</el-menu-item>
          <el-menu-item index="3-2">资源 two</el-menu-item>
          <el-menu-item index="3-3">资源 three</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="4">
          <template #title>问答</template>
          <el-menu-item index="4-1">问答 one</el-menu-item>
          <el-menu-item index="4-2">问答 two</el-menu-item>
          <el-menu-item index="4-3">问答 three</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </div>
    <div class="search-bar">
      <el-input
        style="width: 300px"
        v-model="searchText"
        placeholder="全站搜索"
        clearable
        prefix-icon="Search"
      >
        <template #append>
          <el-button type="primary" plain @click="search">搜索</el-button>
        </template>
      </el-input>
    </div>
    <div class="user">
      <el-dropdown trigger="click">
        <AvatarInfo :size="30" :isSelf="true" />
        <template #dropdown>
          <el-dropdown-menu v-if="isLogin">
            <el-dropdown-item @click="enterProfile"
              >进入个人中心</el-dropdown-item
            >
            <el-dropdown-item @click="enterProfile">其他</el-dropdown-item>
            <el-dropdown-item @click="logout">登出</el-dropdown-item>
          </el-dropdown-menu>
          <el-dropdown-menu v-else="isLogin">
            <el-dropdown-item @click="toLogin">去登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
      <div class="gitee" @click="handleGitClick">
        <el-tooltip effect="dark" content="在gitee找到我们" placement="bottom">
          <img src="@/assets/common/icons/gitee.png" alt="" />
        </el-tooltip>
      </div>
      <div class="github" @click="handleGithubClick">
        <el-tooltip effect="dark" content="在github找到我们" placement="bottom">
          <img src="@/assets/common/icons/github.png" alt="" />
        </el-tooltip>
      </div>
    </div>
  </div>
</template>
<script setup>
import AvatarInfo from "@/components/AvatarInfo.vue";
import { ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import useUserStore from "@/stores/user.js";
import useConfigStore from "@/stores/config";
import useAuths from "@/hooks/useAuths";
import { Session } from "@/utils/storage";
import signalR from "@/utils/signalR";

const { isLogin, clearStorage } = useAuths();
const configStore = useConfigStore();
const router = useRouter();
const route = useRoute();
const userStore = useUserStore();
const activeIndex = ref("1");
const searchText = ref("");
const handleSelect = (key, keyPath) => {
  console.log(key, keyPath);
};
const logout = async () => {
  ElMessageBox.confirm(`确定登出系统吗?`, "警告", {
    confirmButtonText: "确认",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    //异步
    await userStore.logOut();
    await signalR.close();
    //删除成功后，跳转到主页
    router.push("/login");
    ElMessage({
      type: "success",
      message: "登出成功",
    });
  });
};
const enterIndex = () => {
  router.push("/index");
};
const enterProfile = () => {
  router.push("/profile");
};
const toLogin = () => {
  clearStorage();
  Session.set("currentPath", route.path);
  router.push("/login");
};
const search = () => {
  var routerPer = { path: `/discuss`, query: { q: searchText.value } };
  searchText.value = "";
  router.push(routerPer);
};

const handleGitClick = () => {
  window.open("https://gitee.com/ccnetcore/Yi");
};
const handleGithubClick = () => {
  window.open("https://github.com/ccnetcore/Yi.Abp.Admin");
};
</script>

<style scoped lang="scss">
.header {
  width: 1300px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.user {
  display: flex;
  align-items: center;

  .el-dropdown-link {
    cursor: pointer;
    display: flex;
    align-items: center;
  }
  .gitee,
  .github {
    cursor: pointer;
    width: 25px;
    height: 25px;
    margin-left: 15px;
    img {
      width: 100%;
      height: 100%;
    }
  }
}

.logo {
  cursor: pointer;
  display: flex;
  align-items: center;
  .image {
    width: 25px;
    height: 25px;
    img {
      width: 100%;
      height: 100%;
    }
  }
  .text {
    font-weight: bold;
    margin-left: 10px;
  }
}
.tab {
  .el-menu {
    height: 90%;
  }
  :deep(.el-menu--horizontal) {
    border-bottom: none;
  }
}
.flex-grow {
  flex-grow: 1;
}
.img-icon {
  margin-right: 0.5rem;
}
</style>
