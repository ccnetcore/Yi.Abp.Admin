<template>
  <div class="friend-box">
    <div class="left">
      <div class="icon"><img :src="userImageSrc" alt="" /></div>
    </div>
    <div class="center">
      <div class="top">
        <div class="name">
          <el-tooltip
            class="box-item"
            effect="dark"
            :content="friendData.userName"
            placement="top"
          >
            {{ friendData.userName }}
          </el-tooltip>
        </div>
        <el-tag effect="light" :type="userLimit.type">
          {{ userLimit.label }}
        </el-tag>
        <el-tag effect="light" type="success"
          >等级{{ friendData.level }}</el-tag
        >
      </div>
    </div>
    <div class="right">
      <div class="follow">
        <el-icon class="el-icon--right"><Plus /></el-icon>
        <div class="text">关注</div>
      </div>
    </div>
  </div>
</template>

<script setup name="RecommendFriend">
import { defineProps, computed } from "vue";

const props = defineProps({
  friendData: {
    type: Array,
    default: () => [],
  },
});

const statusTypeList = [
  {
    label: "正常",
    value: "Normal",
    type: "success",
  },
  {
    label: "危险",
    value: "Dangerous",
    type: "warning",
  },
  {
    label: "已禁止",
    value: "Ban",
    type: "danger",
  },
];

const getStatusInfo = (type) => {
  return statusTypeList.filter((item) => item.value === type)[0];
};

const userLimit = computed(() => getStatusInfo(props.friendData.userLimit));
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
      height: 100%;
      display: flex;
      align-items: center;
      .name {
        width: 50px;
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
</style>
