<template>
  <div class="point-box">
    <div class="left">
      <UserInfoCard :userInfo="pointsData" :iconUrl="userImageSrc" />
    </div>
    <div class="center">
      <div class="top">
        <el-tag effect="light" type="success"
          >{{pointsData.money }} 钱钱</el-tag
        >

        
        <!-- <UserLimitTag :userLimit="pointsData.userLimit" /> -->
      </div>
      
      <div class="bottom">
        <div class="name">
          <el-tooltip
            class="box-item"
            effect="dark"
            :content="pointsData.nick"
            placement="top"
          >
            {{ pointsData.nick }}
          </el-tooltip>
        </div>
      </div>
    </div>
    <div class="right">
      <div class="follow">
      
        <div class="follow-text">  <el-icon class="el-icon--right"><Plus /></el-icon>关注</div>
      </div>
    </div>
  </div>
</template>

<script setup name="PointsRanking">
import { computed } from "vue";
import UserInfoCard from "@/components/UserInfoCard/index.vue";
import UserLimitTag from "@/components/UserLimitTag.vue";
const props = defineProps({
  pointsData: {
    type: Array,
    default: () => [],
  },
});

const pointsData = computed(() => props.pointsData);

const userImageSrc = computed(() => {
  if (pointsData.value.icon) {
    return import.meta.env.VITE_APP_BASEAPI + "/file/" + pointsData.value.icon;
  } else {
    return "acquiesce.png";
  }
});
</script>

<style lang="scss">
.point-box {
  width: 100%;
  height: 50px;
  display: flex;
  justify-content: space-around;
  .left {
    flex: 1;
    .icon {
      width: 40px;
      height: 40px;
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
      > .el-tag {
        margin-right: 10px;
      }
    }
    .bottom {
      width: 180px;
      .name {
        cursor: pointer;
        width: 100%;
        color: #252933;
        margin-left: 5px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
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
{  cursor: pointer;
  font-size: small;
}
</style>
