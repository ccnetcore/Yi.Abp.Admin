<template>
  <el-badge class="box-card">
    <el-card shadow="never" :style="{ 'border-color': discuss.color }"
             class="discuss-subscript"
    >
      <span class="recommend" v-if="discuss.discussType==='Reward'"> </span>
      <div class="card-header">
        <AvatarInfo :userInfo="discuss.user" />
      </div>
      
      <div style="display: flex;
        justify-content: space-between;">
        <div>
      <div v-if="discuss.isBan" class="item item-title">
        <el-link size="100" :underline="false" style="color: #f56c6c">{{
          discuss.title
        }}</el-link>
      </div>
      <div v-else class="item item-title">
        <el-link
          size="100"
          :underline="false"
          @click="enterDiscuss(discuss.id)"
          >{{ discuss.title }}</el-link
        >
      </div>

        <div class="item item-description">
          {{ discuss.introduction }}
        </div>
        </div>
        
      <div style="display: flex; margin-right: 40px;position: relative;
    top: -10px;">
        <el-image
          :preview-src-list="[getUrl(discuss.cover)]"
          v-if="discuss.cover"
          :src="getUrl(discuss.cover)"
          style="width: 90px; height: 90px"
        />
      </div>
      </div>
      <!-- 底部 -->
      <div class="item item-bottom">
        <div class="tag-list">
          
          <el-tag v-if="discuss.permissionRoleCodes.length>0" v-for="item in discuss.permissionRoleCodes"  effect="dark" type="danger" :key="item">{{item}}</el-tag>
          
          <el-tag v-if="discuss.title!=''&& discuss.lables.length===0">暂无标签</el-tag>
          
          <el-tag  v-if="discuss.lables.length>0" v-for="item in discuss.lables" :key="item.id">{{item.name}}</el-tag>
        </div>
        <el-space :size="10" :spacer="spacer">
          <div class="item-description">
            {{ discuss.creationTime }}
          </div>
          <AgreeInfo :data="discuss" />
          <el-button icon="View" text>
            浏览数:{{ discuss.seeNum ?? 0 }}</el-button
          >
        </el-space>
      </div>
    </el-card>
    <div class="pinned" v-if="props.badge">
      <div class="icon">
        <el-icon><Upload /></el-icon>
      </div>
      <div class="text">{{ props.badge ?? "" }}</div>
    </div>
  </el-badge>
</template>
<script setup>
import { h, ref, toRef, onMounted, reactive } from "vue";
import { useRouter } from "vue-router";
import AvatarInfo from "./AvatarInfo.vue";
import AgreeInfo from "./AgreeInfo.vue";
import { operate } from "@/apis/agreeApi";

const props = defineProps(["discuss", "badge"]);
const discuss = reactive({
  id: "",
  title: "",
  introduction: "",
  creationTime: "",
  user: {},
  color: "",
  seeNum: 0,
  agreeNum: 0,
  isAgree: false,
  cover: "",
  isBan: false,
  lables:[],
  permissionRoleCodes:[],
  discussType:""
});
const router = useRouter();
const spacer = h(ElDivider, { direction: "vertical" });
const enterDiscuss = (id) => {
  router.push(`/article/${id}`);
};
const getUrl = (str) => {
  return `${import.meta.env.VITE_APP_BASEAPI}/file/${str}`;
};

//点赞操作
const agree = async () => {
  const response = await operate(discuss.id);
  const res = response.data;
  //提示框，颜色区分
  if (res.isAgree) {
    discuss.isAgree = true;
    discuss.agreeNum += 1;
    ElMessage({
      message: res.message,
      type: "success",
    });
  } else {
    discuss.isAgree = false;
    discuss.agreeNum -= 1;
    ElMessage({
      message: res.message,
      type: "warning",
    });
  }
};
onMounted(() => {
  discuss.id = props.discuss.id;
  discuss.title = props.discuss.title;
  discuss.introduction = props.discuss.introduction;
  discuss.creationTime = props.discuss.creationTime;
  discuss.user = props.discuss.user;
  discuss.color = props.discuss.color;
  discuss.seeNum = props.discuss.seeNum;
  discuss.isAgree = props.discuss.isAgree;
  discuss.agreeNum = props.discuss.agreeNum;
  discuss.isBan = props.discuss.isBan;
  discuss.cover = props.discuss.cover;
  discuss.lables=props.discuss.lables;
  discuss.permissionRoleCodes=props.discuss.permissionRoleCodes;
  discuss.discussType=props.discuss.discussType;
});
</script>
<style scoped lang="scss">
.el-card {
  border: 2px solid white;
}

.item-bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0;
  .el-icon {
    margin-right: 0.4rem;
  }
}

.card-header {
  display: flex;
  align-items: center;
}

.item {
  font-size: 14px;
  margin: 5px 0;
}
.box-card {

  position: relative;
  width: 100%;
  /* right: calc(1px + var(--el-badge-size)/ 2) !important; */
  /* top: 0 !important;  */
  .pinned {
    display: flex;
    align-items: center;
    position: absolute;
    top: 10px;
    right: 10px;
    padding: 2px 15px;
    border-radius: 5px;
    color: rgb(8, 119, 229);
    background-color: #ecf5ff;
    font-size: 14px;
    .icon {
      display: flex;
      align-items: center;
      margin-right: 5px;
    }
  }
}

.item-title {
  /* font-size: var(--el-font-size-large); */
}

.item-description {
  font-size: var(--el-font-size-small);
  color: #8c8c8c;
}

.item .el-tag {
  margin-right: 1rem;
}

.ml-2 {
  margin-left: 1.2rem;
}

.el-link {
  font-size: initial;
  font-weight: 700;
}
:deep(.el-card__body) {
  padding: 10px 20px;
}

.discuss-subscript{
  position: relative;
  .recommend:before {
    cursor: pointer;
    content: "悬赏";
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
    content: "选择悬赏主题，帮助他人解决问题，赚取小费"; /* 鼠标悬浮时显示的文字 */
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
