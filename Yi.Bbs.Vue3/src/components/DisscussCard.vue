<template>
  <el-badge :value="props.badge ?? ''" class="box-card">
    <el-card shadow="never" :style="{ 'border-color': discuss.color }">
      <el-row>
        <!-- 头部 -->
        <el-col :span="24" class="card-header">
          <AvatarInfo :userInfo="discuss.user" :time="discuss.creationTime" />
        </el-col>

        <!-- 身体 -->

        <el-col :span="18">
          <el-row>
            <el-col v-if="discuss.isBan" :span="24" class="item item-title">
              <el-link size="100" :underline="false" style="color: #f56c6c">{{
                discuss.title
              }}</el-link></el-col
            >

            <el-col v-else :span="24" class="item item-title">
              <el-link
                size="100"
                :underline="false"
                @click="enterDiscuss(discuss.id)"
                >{{ discuss.title }}</el-link
              ></el-col
            >

            <el-col :span="24" class="item item-description">{{
              discuss.introduction
            }}</el-col>
            <el-col :span="24" class="item item-tag"
              ><el-tag v-for="i in 4" :key="i">教程</el-tag></el-col
            >
          </el-row>
        </el-col>

        <el-col :span="6" style="display: flex; justify-content: center">
          <el-image
            :preview-src-list="[getUrl(discuss.cover)]"
            v-if="discuss.cover"
            :src="getUrl(discuss.cover)"
            style="width: 100px; height: 100px"
          />
        </el-col>

        <!-- 底部 -->
        <el-col :span="24" class="item item-bottom" style="margin-bottom: 0">
          <el-space :size="10" :spacer="spacer">
            <div class="item-description">
              {{ discuss.creationTime }}
            </div>
            <AgreeInfo :data="discuss" />
            <!-- 
                        <el-button text @click="agree">
                            <el-icon v-if="discuss.isAgree" color="#409EFF">
                                <CircleCheckFilled />
                            </el-icon>
                            <el-icon v-else color="#1E1E1E">
                                <Pointer />
                            </el-icon> 点赞:{{ discuss.agreeNum ?? 0 }}</el-button>
                        <el-button icon="Star" text>
                            收藏</el-button> -->

            <el-button icon="View" text>
              浏览数:{{ discuss.seeNum ?? 0 }}</el-button
            >
          </el-space>
        </el-col>
      </el-row>
    </el-card>
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
  isAgree: false,
  agreeNum: 0,
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
  // id:'',
  // title:"",
  // introduction:"",
  // creationTime:"",
  // user:{},
  // color:"",
  // seeNum:0,
  // agreeNum:0,
  // isAgree:""
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
  discuss.value = props.isAgree;
  discuss.value = props.agreeNum;
});
</script>
<style scoped>
.el-card {
  border: 2px solid white;
}

.item-bottom .el-icon {
  margin-right: 0.4rem;
}

.card-header {
  display: flex;
  margin-bottom: 1.5rem;
  align-items: center;
}

.item {
  font-size: 14px;
  margin-bottom: 18px;
}

.box-card {
  width: 100%;
  min-height: 15rem;
  /* right: calc(1px + var(--el-badge-size)/ 2) !important; */
  /* top: 0 !important;  */
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
</style>
