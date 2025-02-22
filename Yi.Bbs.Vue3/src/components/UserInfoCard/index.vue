<template>
  <!-- 悬浮卡片 -->
  <el-popover
    placement="right"
    :width="300"
    :show-arrow="false"
    trigger="hover"
    popper-class="userCard"
    v-if="!props.isSelf"
  >
    <template #reference>
      <el-avatar :size="30" :src="iconUrl"  />
    </template>
    <div class="top">
      <div class="left">
        <div class="image">
          <img :src="iconUrl" alt="" @click="gotoProfile(userInfo.userName)" style="cursor: pointer;" />
        </div>
      </div>
      <div class="right">
        <div class="userInfo">
          <div class="name">{{ userInfo.nick }}</div>
          <div class="level">
            <el-tag effect="light" type="success" v-if="userInfo.level"
              >{{ userInfo.level }}-{{ userInfo.levelName }} 等级</el-tag
            >
          </div>
        </div>
        <div class="website">
          <div class="icon">
            <img src="@/assets/box/github-icon.png" alt="" />
          </div>
          <div class="icon">
            <img src="@/assets/box/website-icon.png" alt="" />
          </div>
          <div class="icon">
            <img src="@/assets/box/gitee-icon.png" alt="" />
          </div>
        </div>
        <div class="btn">
          <el-button type="primary" icon="Plus">关注</el-button>
        </div>
      </div>
    </div>
    <div class="line"></div>
    <div class="bottom">
      <div class="header">
        <div class="score">钱钱：{{ userInfo.money }}</div>
        <div class="status">
          <span>状态：</span>
          <UserLimitTag :userLimit="userInfo.userLimit"/>
        </div>
      </div>
      <div class="hobby">
        <span>关注：</span>
        <el-tag type="info">C#</el-tag>
        <el-tag type="info">前端</el-tag>
        <el-tag type="info">Python</el-tag>
        <el-tag type="info">算法</el-tag>
      </div>
    </div>
  </el-popover>
</template>

<script setup name="UserInfoCard">
import { computed } from "vue";
import { useRouter } from "vue-router";
import UserLimitTag from "../UserLimitTag.vue";
const props = defineProps({
  // 用户信息
  userInfo: {
    type: Object,
    default: () => {},
  },
  // icon地址
  iconUrl: {
    type: String,
    default: () => "",
  },
});
const router = useRouter();
const userInfo = computed(() => props.userInfo);

const gotoProfile=(userName)=>{
  router.push(`/profile/${userName}`);

}
</script>

<style scoped lang="scss">
.userCard {
  .top {
    display: flex;
    width: 100%;
    height: 100px;
    .left {
      width: 80px;
      .image {
        width: 80px;
        height: 80px;
        img {
          width: 100%;
          height: 100%;
        }
      }
    }
    .right {
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      flex: 1;
      margin-left: 20px;
      .userInfo {
        width: 100%;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        .level {
          margin: 0 10px;
        }
      }
      .website {
        display: flex;
        .icon {
          cursor: pointer;
          width: 20px;
          height: 20px;
          margin-right: 10px;
          img {
            width: 100%;
            height: 100%;
          }
        }
      }
    }
  }
  .line {
    margin: 20px 0;
    border-top: 1px solid #eee;
  }
  .bottom {
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    width: 100%;
    height: 100px;
    .header {
      display: flex;
      align-items: center;
      .status {
        margin-left: 50px;
      }
    }
    .hobby {
      display: flex;
      align-items: center;
      > .el-tag {
        margin-right: 10px;
      }
    }
  }
}
</style>
