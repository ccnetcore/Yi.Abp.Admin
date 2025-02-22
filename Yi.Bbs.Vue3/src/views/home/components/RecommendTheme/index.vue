<template>
  <div class="theme-box">
    <div class="left">
      <div class="icon">
        <img src="@/assets/box/recommend.svg" alt="" />
      </div>
      <div class="text" @click="handleClickTheme(themeData?.id)">
        <el-tooltip
          class="box-item"
          effect="dark"
          :content="themeData.title"
          placement="top"
        >
          #{{ themeData.title }}
        </el-tooltip>
      </div>
    </div>
    <div class="right">
      <template v-if="seeNumLength > 4">
        <div class="icon">
          <img src="@/assets/box/browsingHistory.svg" alt="" />
        </div>
        <div class="text">
          <el-tooltip
            class="box-item"
            effect="dark"
            :content="themeData.seeNum"
            placement="top"
          >
            <div class="text">{{ themeData.seeNum }}</div>
          </el-tooltip>
        </div>
      </template>
      <template v-else>
        <div class="icon">
          <img src="@/assets/box/browsingHistory.svg" alt="" />
        </div>
        <div class="text">{{ themeData.seeNum }}</div>
      </template>
    </div>
  </div>
</template>

<script setup name="RecommendFriend">
import { ref } from "vue";
import { useRouter } from "vue-router";

const props = defineProps({
  themeData: {
    type: Array,
    default: () => [],
  },
});
const router = useRouter();

const seeNumLength = ref(props.themeData.seeNum.toString().length);

const handleClickTheme = (id) => {
  router.push(`/article/${id}`);
};
</script>

<style lang="scss">
.theme-box {
  width: 100%;
  height: 50px;
  display: flex;
  justify-content: space-around;
  .left {
    flex: 1;
    height: 100%;
    display: flex;
    align-items: center;

    .text {
      font-size: 16px;
      cursor: pointer;
      width: 230px;
      color: #252933;
      margin-left: 5px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      &:hover {
        color: #1171ee;
      }
    }
  }
  .right {
    width: 25%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: flex-start;
    .text {
      width: 30px;
      margin-left: 10px;
      cursor: pointer;
      color: #252933;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
  .icon {
    width: 25px;
    height: 25px;
    display: flex;
    align-items: center;
    img {
      width: 100%;
      height: 100%;
    }
  }
}
</style>
