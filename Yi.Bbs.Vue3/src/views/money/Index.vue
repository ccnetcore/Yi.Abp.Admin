<script setup>
import AwardPedestal from "./components/AwardPedestal.vue";
import AvatarInfo from "@/components/AvatarInfo.vue";
import { onMounted, reactive, ref, computed } from "vue";
import {
  getRankingPoints,
} from "@/apis/analyseApi.js";
const pointList = ref([]);
const total = ref(0);
const moneyQuery = reactive({ skipCount: 1, maxResultCount: 30 });

const isFirstPage = computed(() => {
  return moneyQuery.skipCount == 1;
})

const pointListFilter=computed(() => {
  //如果是第一页，去掉前3个
  if(moneyQuery.skipCount == 1)
  {
    return  pointList.value.slice(3);
  }
return pointList.value;
})


//初始化
onMounted(async () => {
  await initData();
});

const initData = async () => {
  const { data: pointData } = await getRankingPoints(moneyQuery);
  pointList.value = pointData.items;;
  total.value = pointData.totalCount
}
//分页事件
const changePage = async (currentPage) => {
  await initData();
}
</script>

<template>
  <div class="content-body">
    <AwardPedestal v-show="isFirstPage" :goldUserInfo="pointList[0]" :silverUserInfo="pointList[1]"
      :bronzeUserInfo="pointList[2]" />
    <div v-for="item in pointListFilter" :key="item.id" class="list-div">
      <div class="list-left">
        <span> {{ item.order }}</span>
        <AvatarInfo :userInfo="item" :isSelf="false" />
        <span class="money">
          {{ item.money }}
        </span>
      </div>
      <div class="list-right">
        关注
      </div>

    </div>
    <el-pagination background layout="total, sizes, prev, pager, next, jumper" :total="total"
      :page-sizes="[10, 30, 50, 100]" v-model:current-page="moneyQuery.skipCount"
      v-model:page-size="moneyQuery.maxResultCount" @current-change="changePage" @size-change="changePage" />
  </div>
</template>
<style scoped lang="scss">
.el-pagination {

  padding: 10px;
  display: flex;
  justify-content: center;
}

.content-body {
  margin-bottom: 40px;
  margin-top: 20px;
  padding: 20px;
  background-color: #ffffff;
}

.list-div {
  justify-content: space-between;
  border-radius: 4px;
  display: flex;
  background-color: #ffffff;
  height: 80px;
  width: 850px;
  cursor: pointer;
  padding: 16px 12px;

  .list-left {
    display: flex;

    span {
      margin-right: 20px;
      color: #515767;
      font-size: 1.5rem;
      font-weight: 600;
      line-height: 2rem;
      display: flex;
      align-content: center;
      flex-wrap: wrap;
    }

    .money {
      font-size: 1.0rem;
      color: #ff0000;
    }
  }
}

.list-div:hover {
  background-color: #f7f8fa;
}
</style>
