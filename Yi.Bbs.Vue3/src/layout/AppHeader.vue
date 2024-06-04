<template>
  <div class="header">
    <div class="logo" @click="enterIndex">
      <div class="image">
        <img class="img-icon" src="@/assets/common/icons/logo.ico" />
      </div>
      <div class="text">{{ configStore.name }}</div>
    </div>
    <div class="tab">
      <el-menu :default-active="activeIndex" mode="horizontal" :ellipsis="false" @select="handleSelect">
        <el-menu-item index="1" @click="enterIndex">主页</el-menu-item>

        <el-menu-item index="2" @click="enterStart"
        style="color: red;font-weight: bolder;font-size: large;"
        
        >开始</el-menu-item>

        <el-sub-menu index="3">
          <template #title>学习</template>
          <el-menu-item index="3-1">前端</el-menu-item>
          <el-menu-item index="3-2">后端</el-menu-item>
          <el-menu-item index="3-3">运维</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="4">
          <template #title>问答</template>
          <el-menu-item index="4-1">前端</el-menu-item>
          <el-menu-item index="4-2">后端</el-menu-item>
          <el-menu-item index="4-3">运维</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </div>
    <div class="search-bar">
      <el-input style="width: 300px" v-model="searchText" placeholder="全站搜索" clearable prefix-icon="Search">
        <template #append>
          <el-button type="primary" plain @click="search">搜索</el-button>
        </template>
      </el-input>
    </div>
    <div class="user">


      <div class="money" v-if="isLogin">钱钱：<span>{{ money }}</span></div>
      <el-dropdown trigger="click">
        <AvatarInfo :size="30" :isSelf="true" />

        <template #dropdown>


          <el-dropdown-menu v-if="isLogin">
            <el-dropdown-item>你的钱钱：{{ money }}</el-dropdown-item>
            <el-dropdown-item @click="enterProfile">进入个人中心</el-dropdown-item>
            <el-dropdown-item @click="enterActivity">进入活动页面</el-dropdown-item>
            <el-dropdown-item @click="logout">登出</el-dropdown-item>
          </el-dropdown-menu>
          <el-dropdown-menu v-else>
            <el-dropdown-item @click="toLogin">去登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <div class="notice">
        <el-dropdown trigger="click" :max-height="500">
          <el-badge :value="noticeStore.noticeForNoReadCount">
            <el-button type="primary">
              <el-icon :size="15">
                <Bell />
              </el-icon>
            </el-button>
          </el-badge>
          <template #dropdown>
            <el-dropdown-menu>

              <el-dropdown-item class="notice-oper" style="justify-content: space-between;">
                <el-button type="primary" @click="fetchNoticeData">刷新</el-button>
                <el-button type="warning" @click="hanldeReadClick">一键已读</el-button>
              </el-dropdown-item>


              <el-dropdown-item v-for="(item, index) in noticeList" :key="index">
                <div v-if="item.isRead" class="notice-msg" v-html="item.message"></div>

                <el-badge is-dot v-else >
                  <div class="notice-msg" v-html="item.message"></div>
                </el-badge>
         
           
              </el-dropdown-item>



     
            </el-dropdown-menu>
          </template>

        </el-dropdown>
      </div>

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
import { Bell } from '@element-plus/icons-vue'
import AvatarInfo from "@/components/AvatarInfo.vue";
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import useUserStore from "@/stores/user.js";
import useConfigStore from "@/stores/config";
import useAuths from "@/hooks/useAuths";
import { Session } from "@/utils/storage";
import { storeToRefs } from 'pinia'
import useNoticeStore from "@/stores/notice";
import { ElMessage } from "element-plus";
import { getList as getNoticeList, read as noticeRead } from "@/apis/bbsNoticeApi"
const { isLogin, clearStorage } = useAuths();
//消息通知存储
const noticeStore = useNoticeStore();
const { noticeList } = storeToRefs(noticeStore);
const configStore = useConfigStore();
const router = useRouter();
const route = useRoute();
const userStore = useUserStore();
const { money } = storeToRefs(userStore)
const activeIndex = ref("1");
const searchText = ref("");
const noticeForNoReadCount=computed(()=>{
 return noticeList.value.filter(x => x.isRead ==false).length;
})
//加载初始化离线消息
onMounted(async () => {
  await fetchNoticeData();
})
const fetchNoticeData = async () => {
  const { data } = await getNoticeList({maxResultCount:20});
  noticeStore.setNotices(data.items);
}


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
  router.push(`/profile/${userStore.userName}`);
};
const enterActivity = () => {
  router.push(`/activity`);
}
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
///一键已读
const hanldeReadClick=async ()=>{

 await noticeRead();
 await fetchNoticeData();

 ElMessage({
        message: `全部已读`,
        type: "success",
      });
}

const enterStart=()=>{
alert("即将发布Yi.Abp.Tool,官方脚手架工具集，敬请期待！")
}

</script>


<style scoped lang="scss">
.money {

  font-size: small;
  color: #FED055;
  margin: 0 5px;

  span {
    font-weight: 600;
  }
}

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

.notice {
  margin: 0 5px;
  &-oper {
display: flex;
justify-content: space-between;
width: 100%;
}
  &-msg {
    white-space: wrap !important;
    width: 400px;
    padding: 6px;
    font-size: 14px;
    line-height: 24px;
    border-bottom: 1px solid #f0e9e9;
  }
}
</style>
