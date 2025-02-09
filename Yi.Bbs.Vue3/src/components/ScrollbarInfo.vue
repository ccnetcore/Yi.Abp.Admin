<template>
  <el-scrollbar>
    <div class="scrollbar-flex-content">
      <div v-for="item in recommendList" :key="item.id" class="scrollbar-item">
<!--        <el-tooltip-->
<!--          class="box-item"-->
<!--          effect="dark"-->
<!--          :content="item.dictLabel"-->
<!--          placement="top"-->
<!--          v-if="item.dictLabel.length > 5"-->
<!--        >-->
<!--          {{ item.dictLabel.slice(0, 5) + "..." }}-->
<!--        </el-tooltip>-->
        <span >
          {{ item.dictLabel }}
        </span>
      </div>
    </div>
  </el-scrollbar>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { getDictionaryList } from "@/apis/dictionaryApi.js";

const recommendList = ref([]);
onMounted(async () => {
  const { data } = await getDictionaryList("bbs_type_lable");
  recommendList.value = data;
});
</script>

<style scoped>
.scrollbar-flex-content {
  display: flex;
}
.scrollbar-item {
  padding: 0 15px;
  cursor: pointer;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  //width: 4rem;
  height: 2.6rem;
  margin: 0 0.2rem;
  text-align: center;
  border-radius: 4px;
  background-color: #fafafa;
  font-size: 14px;
}
</style>
