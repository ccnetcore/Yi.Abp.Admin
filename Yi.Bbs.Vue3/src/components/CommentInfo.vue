<template>
  <el-tabs v-model="activeName" @tab-click="handleClick">
    <el-tab-pane label="评论" name="comment"></el-tab-pane>
    <el-tab-pane label="相关内容" name="interrelated"></el-tab-pane>
  </el-tabs>

  <div class="total">
    <div style="align-self: center">共{{ total }}个评论</div>
    <div>
      <el-radio-group v-model="selectRadio">
        <el-radio-button label="new" name="new">最新</el-radio-button>
        <el-radio-button label="host" name="host">最热</el-radio-button>
      </el-radio-group>
    </div>
  </div>

  <el-divider />
  <div>
    <WangEditor v-model="topContent" height="220px" />
    <el-button
      @click="addTopComment"
      type="primary"
      class="btn-top-comment"
      :disabled="!isAddComment"
      >发表评论</el-button
    >
    <el-button class="btn-top-comment">其他</el-button>

    <el-divider />
  </div>

  <!-- 开始评论主体 -->

  <div v-for="item in commentList" :key="item.id" class="comment1">
    <AvatarInfo :userInfo="item.createUser" />
    <div class="content" v-html="item.content">
    </div>
    <span class="time"> {{ item.creationTime }} </span>
    <span class="pointer"
      ><el-icon>
        <Pointer />
      </el-icon>
      0</span
    >
    <el-button
      type="primary"
      @click="replay(item.createUser.nick, item.id, item.id)"
      size="large"
      text
      v-hasPer="['bbs:comment:add']"
      >回复</el-button
    >
    <el-button
      type="danger"
      @click="delComment(item.id)"
      size="large"
      text
      v-hasPer="['bbs:comment:remove']"
      >删除</el-button
    >
    <div v-show="replayId == item.id" class="input-reply">
      <el-input
        v-model="form.content"
        :placeholder="placeholder"
        :rows="3"
        type="textarea"
      ></el-input>
      <div class="btn-reply">
        <el-button
          @click="addComment"
          type="primary"
          v-hasPer="['bbs:comment:add']"
          >回复</el-button
        >
      </div>
    </div>

    <!-- 开始子评论主体 -->
    <div v-for="children in item.children" :key="children.id" class="comment2">
      <div style="display: flex">
        <AvatarInfo :userInfo="children.createUser" />
        <span style="align-self: center; color: #606266">
          回复@{{ children.commentedUser.nick }}</span
        >
      </div>
      <div class="content" v-html="children.content">
      </div>
      <span class="time">{{ children.creationTime }} </span>
      <span class="pointer">
        <el-icon> <Pointer /> </el-icon>0</span
      >
      <el-button
        type="primary"
        @click="replay(children.createUser.nick, children.id, item.id)"
        size="large"
        text
        v-hasPer="['bbs:comment:add']"
        >回复</el-button
      >
      <el-button
        type="danger"
        @click="delComment(children.id)"
        size="large"
        text
        v-hasPer="['bbs:comment:remove']"
        >删除</el-button
      >
      <div v-show="replayId == children.id" class="input-reply">
        <el-input
          v-model="form.content"
          :placeholder="placeholder"
          :rows="3"
          type="textarea"
        ></el-input>
        <div class="btn-reply">
          <el-button
            @click="addComment"
            type="primary"
            v-hasPer="['bbs:comment:add']"
            >回复</el-button
          >
        </div>
      </div>
    </div>

    <el-divider />
  </div>
  <el-empty
    v-show="commentList.length <= 0"
    description="评论空空如也，快来抢占沙发~"
  />
</template>
<script setup>
import { onMounted, reactive, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { getListByDiscussId, add, del } from "@/apis/commentApi.js";
import AvatarInfo from "./AvatarInfo.vue";
import { getPermission } from "@/utils/auth";
import WangEditor from "./WangEditor.vue"

const props = defineProps({
  isComment: {
    type: Boolean,
    default: false,
  },
});
const { isHasPermission: isAddComment } = getPermission(
  "bbs:comment:add",
  props.isComment
);

//数据定义
const route = useRoute();
const router = useRouter();
const commentList = ref([]);
const query = reactive({});
const topContent = ref("");
//当前回复id
const replayId = ref("");
//回复文本框
const placeholder = ref("");

//选择类型：评论
const activeName = ref("comment");
//选择 最新
const selectRadio = ref("new");
//评论总数
const total = ref(0);
const form = reactive({
  content: "",
  discussId: route.params.discussId,
  query,
  parentId: 0,
  rootId: 0,
});
onMounted(async () => {
  await loadComment();
});
const loadComment = async () => {
  topContent.value = "";
  form.content = "";
  const response = await getListByDiscussId(route.params.discussId, query);
  commentList.value = response.data.items;
//处理换行问题

commentList.value.forEach(x=>x.content=x.content.replace(/\n/g, "<br/>"))
  total.value = response.data.total;
};

const addTopComment = async () => {
  form.parentId = "00000000-0000-0000-0000-000000000000";
  form.rootId = "00000000-0000-0000-0000-000000000000";
  form.content = topContent.value;
  await addComment();
};
const addComment = async () => {
  if (form.content.length <= 0) {
    ElMessage.error("输入评论不能为空!");
    return;
  }
  await add(form);
  await loadComment();
  ElMessage({
    message: "评论发表成功!",
    type: "success",
  });
};
const delComment = async (ids) => {
  ElMessageBox.confirm(`确定是否删除编号[${ids}]的评论吗?`, "警告", {
    confirmButtonText: "确认",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    await del(ids);
    await loadComment();
    ElMessage({
      message: "评论已删除!",
      type: "success",
    });
  });
};
const replay = async (parentUserName, parentId, rootId) => {
  replayId.value = parentId;
  form.parentId = parentId;
  form.rootId = rootId;
  placeholder.value = `回复@${parentUserName}`;
};

//切换 评论、相关内容
const handleClick = () => {};
</script>
<style scoped>
.input-reply {
  margin-top: 1rem;
}

.btn-reply {
  margin: 1rem 0;
  display: flex;
  justify-content: end;
}

.comment1 .pointer {
  margin: 0 0 0 1rem;
}

.time {
  color: #8c8c8c;
}

.content {
  margin: 1rem 0;
}

.total {
  display: flex;

  justify-content: space-between;
}

.comment2 {
  margin-left: 3rem;
}

.el-divider {
  margin: 1rem 0;
}

.btn-top-comment {
  margin-top: 0.5rem;
  margin-right: 1rem;
}
</style>
