<template>
  <div class="friend-box">
    <div class="left">
      <UserInfoCard :userInfo="friendData" :iconUrl="userImageSrc" />
    </div>
    <div class="center">
      <div class="top">
        <div class="name">
          <el-tooltip
            class="box-item"
            effect="dark"
            :content="friendData.nick"
            placement="top"
          >
            {{ friendData.nick }}
          </el-tooltip>
        </div>




        <!-- <el-tag effect="light" type="success"
          >{{ friendData.level }}-{{friendData.levelName}} 等级</el-tag
        > -->

        <UserLimitTag :userLimit="friendData.userLimit" />
      </div>
    </div>
    <div class="right">
      <div class="follow">
       
        <div class="follow-text"> <el-icon class="el-icon--right"><Plus /></el-icon>关注</div>
      </div>
    </div>
  </div>
</template>

<script setup name="RecommendFriend">
import { computed } from "vue";
import UserInfoCard from "@/components/UserInfoCard/index.vue";
import UserLimitTag from "@/components/UserLimitTag.vue";
const props = defineProps({
  friendData: {
    type: Array,
    default: () => [],
  },
});

const userImageSrc = computed(() => {
  if (props.friendData.icon) {
    return import.meta.env.VITE_APP_BASEAPI + "/file/" + props.friendData.icon;
  } else {
    return "acquiesce.png";
  }
});
</script>

<style lang="scss">
.friend-box {
  width: 100%;
  height: 50px;
  display: flex;
  justify-content: space-around;
  .left {
    display: flex;
    align-items: center;
    flex: 1;
    .icon {
      width: 30px;
      height: 30px;
      img {
        width: 100%;
        height: 100%;
        border-radius: 50%;
      }
    }
  }
  .center {
    flex: 4;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    padding: 0 10px;
    .top {
      height: 100%;
      display: flex;
      align-items: center;
      .name {
        width: 100px;
        color: #252933;
        margin-left: 5px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
      > .el-tag {
        margin: 0 10px;
      }
    }
  }
  .right {
    flex: 2;
    display: flex;
    align-items: center;
    .follow {
      display: flex;
      justify-content: flex-start;
      align-items: center;
      font-size: 16px;
      color: #1171ee;
    }
  }
}
.follow-text
{
  font-size: small;
  cursor: pointer;
}
</style>
