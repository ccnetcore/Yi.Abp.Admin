<template>
  <el-row>
    <el-col>
      <el-card :body-style="{ padding: '0px' }" shadow="never">
        <img src="" class="image" />
        <div style="padding: 14px" class="plate-box">
          <span>{{ props.name }}</span>
          <div class="bottom">
            <time class="remarks">{{ props.introduction }}</time>
            <RouterLink :to="`/discuss/${props.id}/${props.isPublish}`">
              <el-button text class="button" type="primary"
                >进入<el-icon><CaretRight /></el-icon
              ></el-button>
            </RouterLink>
          </div>
          <span class="recommend" v-if="isPublish"> </span>
        </div>
      </el-card>
    </el-col>
  </el-row>
</template>

<script setup>
import { onMounted } from "vue";

const props = defineProps(["name", "introduction", "id", "isPublish"]);
</script>

<style scoped>
.remarks {
  font-size: 12px;
  color: #999;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.image {
  width: 100%;
  display: block;
}

.plate-box {
  position: relative;
  .recommend:before {
    cursor: pointer;
    content: "官方";
    position: absolute;
    top: -8px;
    right: -26px;
    z-index: 1;
    padding: 14px 22px 2px;
    background-color: #ff9900;
    transform: rotate(45deg);
    font-size: 12px;
    color: #ffffff;
  }
  .recommend:hover::after {
    width: 100px;
    content: "官方专属板块不得随意发布主题"; /* 鼠标悬浮时显示的文字 */
    position: absolute;
    top: 30px; /* 距离盒子顶部的距离 */
    left: 70%; /* 盒子中央位置 */
    transform: translateX(-50%); /* 水平居中 */
    padding: 5px 10px;
    background-color: #ff9900;
    color: #ffffff;
    font-size: 12px;
    border-radius: 5px;
    z-index: 9999;
    opacity: 0; /* 初始状态不透明 */
    transition: opacity 0.5s; /* 添加过渡效果 */
  }

  .recommend:hover::after {
    opacity: 1; /* 鼠标悬浮时完全显示 */
  }
}
</style>
