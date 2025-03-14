<template>
  <div class="avatar">
    <div class="avatar-left">
      <div v-if="props.isSelf" class="avatar-center">
        <el-avatar :size="props.size" :src="iconUrl" />
        <div class="nick">{{ userInfo.nick }}</div>
      </div>
      <div v-if="!props.isSelf" class="avatar-card">
        <!-- 悬浮卡片 -->
        <userInfo-card :userInfo="userInfo" :iconUrl="iconUrl" />
        <div class="content">
          <div class="nick" :class="{ mt_1: props.time != 'undefined' }">
            <div class="text">{{ userInfo.nick }}</div>
            <div class="level">
              <el-tag round effect="light" type="success" v-if="userInfo.level">{{ userInfo.level
                }}-{{ userInfo.levelName }} 等级</el-tag>
            </div>
            <div class="status" v-if="userInfo.userLimit">

              <UserLimitTag :userLimit="userInfo.userLimit" />

            </div>
          </div>
          <div class="remarks" v-if="props.time">{{ props.time }}</div>
          <div class="remarks">
            <slot name="bottom" />
          </div>
        </div>
      </div>
      <div class="info" v-if="!props.isSelf">
        <el-tag class="ml-2" type="warning">V8</el-tag>
        <el-tag class="ml-2" type="danger">核心</el-tag>
      </div>
      <el-button v-if="props.showWatching" type="primary" size="small" icon="Plus">关注</el-button>
    </div>
  </div>
</template>
<script setup>
import useUserStore from "@/stores/user";
import { reactive, watch, onMounted, ref } from "vue";
import UserInfoCard from "./UserInfoCard/index.vue";
import UserLimitTag from "./UserLimitTag.vue";

//userInfo
//{icon,name,role,id},根据判断userInfo是否等于未定义，来觉得是当前登录用户信息，还是其他人信息
const props = defineProps([
  "size",
  "showWatching",
  "time",
  "userInfo",
  "isSelf",
]);
const userStore = useUserStore();
const userInfo = reactive({
  icon: "",
  nick: "",
  role: [],
  id: "",
  level: "",
  userLimit: "",
  userName: ""
});
const iconUrl = ref("/acquiesce.png");
const iconUrlHandler = (icon) => {
  if (
    userInfo.icon == null ||
    userInfo.icon == undefined ||
    userInfo.icon == ""
  ) {
    return "/acquiesce.png";
  } else {
    return import.meta.env.VITE_APP_BASEAPI + "/file/" + icon;
  }
};

watch(userStore, (n) => {
  Init();
});

watch(
  () => props,
  (n) => {
    Init();
  },
  { deep: true }
);

onMounted(() => {
  Init();
});

const Init = () => {
  //使用传入值
  if (props.userInfo != undefined) {
    userInfo.icon = props.userInfo.icon;
    userInfo.nick = props.userInfo.nick;
    userInfo.role = props.userInfo.role;
    userInfo.id = props.userInfo.id;
    userInfo.money = props.userInfo.money;
    userInfo.level = props.userInfo.level;
    userInfo.userLimit = props.userInfo.userLimit;
    userInfo.userName = props.userInfo.userName;
    userInfo.levelName = props.userInfo.levelName;
    iconUrl.value = iconUrlHandler(userInfo.icon);
  }

  //使用当前登录用户
  else {
    userInfo.icon = userStore.icon;
    userInfo.nick = userStore.name;
    userInfo.role = userStore.role;
    userInfo.id = userStore.id;
    iconUrl.value = userInfo.icon;
  }
};



</script>

<style lang="scss" scoped>
.mt_1 {
  margin-top: 0.5rem;
}

.info {
  flex: 1;
  margin-left: 1rem;
}

.info .el-tag {
  margin-right: 1rem;
}

.el-icon {
  color: white;
}

.avatar {
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  
}

.avatar-left,
.avatar-card {
  display: flex;
  justify-content: space-between;
  align-items: center;

  .content {
    margin-left: 10px;
  }

  .nick {
    display: flex;
    align-items: center;
    justify-content: space-between;
    font-weight: bold;

    >div {
      margin-right: 10px;
    }
  }
}

.avatar-center {
  display: flex;
  flex: 2;
}

.el-avatar {
  margin-right: 1rem;
  --el-avatar-bg-color: none;
}

.remarks {
  padding-top: 0.5rem;
  color: #8c8c8c;
}
</style>
