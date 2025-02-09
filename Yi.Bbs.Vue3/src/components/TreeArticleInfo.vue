<template>
<!--  @node-click="handleNodeClick"-->
  <el-tree 
      empty-text="无子文章"
    :data="props.data == '' ? [] : props.data"
    :props="defaultProps"

    :expand-on-click-node="false"
    node-key="id"
    :default-expand-all="true"
    :highlight-current="true"
    :current-node-key="currentNodeKey"
  >
    <template #default="{ node, data }">
      <span class="custom-tree-node">
        <el-tooltip
          class="box-item"
          effect="dark"
          :content="data.name"
          placement="right"
        >
          <span @click="handleNodeClick(data)" class="title-name">{{ data.name }}</span>
        </el-tooltip>

        <span>
          <a
            style="color: #409eff; margin-left: 8px"
            @click="$emit('create', node, data)"
            v-if="isArticleUser && isAddArticle"
          >
            +
          </a>
          <a
            style="color: #409eff; margin-left: 8px"
            @click="$emit('update', node, data)"
            v-if="isArticleUser && isEditArticle"
          >
            编辑
          </a>
          <a
            style="color: #f56c6c; margin-left: 8px"
            @click="$emit('remove', node, data)"
            v-if="isArticleUser && isRemoveArticle"
          >
            删除
          </a>
        </span>
        
      </span>
    </template>
  </el-tree>
</template>
<script setup>
import { ref } from "vue";
import { getPermission } from "@/utils/auth";

const props = defineProps(["data", "currentNodeKey", "isArticleUser"]);
const emits = defineEmits(["handleNodeClick"]);

const currentNodeKey = props.currentNodeKey;
//数据定义
//子文章数据
// const articleData =ref([]);
//树形子文章选项
const defaultProps = {
  children: "children",
  label: "name",
};
// //子文章初始化
// const loadArticleData=async()=>
// {
//     articleData.value=  await  articleall(route.params.discussId);
// }
//点击事件
const handleNodeClick = (data) => {
  emits("handleNodeClick", data);
};
const { isHasPermission: isAddArticle } = getPermission("bbs:article:add");
const { isHasPermission: isEditArticle } = getPermission("bbs:article:update");
const { isHasPermission: isRemoveArticle } = getPermission("bbs:article:del");
</script>
<style scoped>
.custom-tree-node {

  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 14px;
  .title-name {
    width: 100%; /* 定义容器宽度 */
    white-space: nowrap; /* 不换行 */
    overflow: hidden; /* 溢出部分隐藏 */
    text-overflow: ellipsis; /* 显示省略号 */
  }
}
</style>
