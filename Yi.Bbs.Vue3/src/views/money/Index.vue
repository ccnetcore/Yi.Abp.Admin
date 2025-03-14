<script setup>
import AwardPedestal from "./components/AwardPedestal.vue";
import AvatarInfo from "@/components/AvatarInfo.vue";
import { onMounted, reactive, ref, computed } from "vue";
import {
  getMoneyTop,
  getValueTop,
  getPointsTop
} from "@/apis/analyseApi.js";
const pointList = ref([]);
const total = ref(0);
const moneyQuery = reactive({ skipCount: 1, maxResultCount: 30 });
const tabSelect=ref("money");
const isFirstPage = computed(() => {
  return moneyQuery.skipCount === 1;
})

const pointListFilter=computed(() => {
  //如果是第一页，去掉前3个
  if(moneyQuery.skipCount === 1)
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
  switch (tabSelect.value)
  {
    case "money":
      const { data: pointData } = await getMoneyTop(moneyQuery);
      pointList.value = pointData.items;
      total.value = pointData.totalCount
      break;
    case "value":
      const { data: pointData2 } = await getValueTop(moneyQuery);
      pointData2.items.forEach(item => {item.money=item.value})
      pointList.value = pointData2.items;
      total.value = pointData2.totalCount
      break;
    case "points":
      const { data: pointData3 } = await getPointsTop(moneyQuery);
      pointData3.items.forEach(item => {item.money=item.points})
      pointList.value = pointData3.items;
      total.value = pointData3.totalCount
      break;
  }
 
}
//分页事件
const changePage = async (currentPage) => {
  await initData();
}

//切换tab
const handleClickTabs=async (pane) => {

  tabSelect.value=pane.paneName;
  moneyQuery.skipCount = 1;
  await initData();
}
</script>

<template>
  <div class="content-body">
    <div class="header-banner">
      <h1 class="main-title">{{ tabSelect === 'money' ? '钱钱排行榜' : tabSelect === 'value' ? '价值排行榜' : '积分排行榜' }}</h1>
    </div>
    <el-tabs v-model="tabSelect" @tab-click="handleClickTabs" class="tabs">
      <el-tab-pane label="钱钱" name="money"></el-tab-pane>
      <el-tab-pane label="价值" name="value"></el-tab-pane>
      <el-tab-pane label="积分" name="points"></el-tab-pane>
    </el-tabs>
    
    <transition name="fade">
      <AwardPedestal v-show="isFirstPage" :goldUserInfo="pointList[0]" :silverUserInfo="pointList[1]"
        :bronzeUserInfo="pointList[2]" />
    </transition>
    
    <div class="list-container">
      <transition-group name="list" tag="div">
        <div v-for="(item, index) in pointListFilter" :key="item.id" class="list-div" :style="{'--delay': `${index * 0.05}s`}">
          <div class="list-left">
            <span class="rank-number" :class="{'top-ten': item.order <= 10}"> {{ item.order }}</span>
            <AvatarInfo :userInfo="item" :isSelf="false" />
            <span class="money">
              {{ item.money }}
            </span>
          </div>
          <div class="list-right">
            <el-button type="primary" size="small" class="follow-btn">关注</el-button>
          </div>
        </div>
      </transition-group>
    </div>
    
    <el-pagination background layout="total, sizes, prev, pager, next, jumper" :total="total"
      :page-sizes="[10, 30, 50, 100]" v-model:current-page="moneyQuery.skipCount"
      v-model:page-size="moneyQuery.maxResultCount" @current-change="changePage" @size-change="changePage" class="pagination" />
  </div>
</template>
<style scoped lang="scss">
.header-banner {
  background: linear-gradient(135deg, #6a11cb 0%, #2575fc 100%);
  padding: 30px 0;
  border-radius: 10px;
  margin-bottom: 30px;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
}

.main-title {
  color: white;
  text-align: center;
  font-size: 2.5rem;
  margin: 0;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.5s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.list-container {
  max-width: 900px;
  margin: 0 auto;
}

.list-enter-active, .list-leave-active {
  transition: all 0.5s ease;
}
.list-enter-from {
  opacity: 0;
  transform: translateX(-30px);
}
.list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}

.el-pagination {
  padding: 20px;
  display: flex;
  justify-content: center;
  margin-top: 20px;
}

.content-body {
  margin-bottom: 40px;
  margin-top: 20px;
  padding: 20px;
  background-color: #ffffff;
  border-radius: 10px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.05);
}

.list-div {
  justify-content: space-between;
  border-radius: 8px;
  display: flex;
  background-color: #ffffff;
  height: 80px;
  width: 100%;
  cursor: pointer;
  padding: 16px 20px;
  margin-bottom: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
  transition: all 0.3s ease;
  animation: slideIn 0.5s ease backwards;
  animation-delay: var(--delay);

  .list-left {
    display: flex;
    align-items: center;

    .rank-number {
      width: 40px;
      height: 40px;
      margin-right: 20px;
      color: #515767;
      font-size: 1.5rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      justify-content: center;
      position: relative;
      
      &.top-ten {
        color: white;
        background: linear-gradient(135deg, #ff9a9e 0%, #fad0c4 99%, #fad0c4 100%);
        border-radius: 50%;
        box-shadow: 0 4px 8px rgba(255, 154, 158, 0.3);
      }
    }

    .money {
      font-size: 1.2rem;
      color: #ff5252;
      font-weight: bold;
      margin-left: 15px;
    }
  }
  
  .list-right {
    display: flex;
    align-items: center;
    
    .follow-btn {
      transition: all 0.3s;
      background: linear-gradient(45deg, #667eea 0%, #764ba2 100%);
      border: none;
      
      &:hover {
        transform: translateY(-2px);
        box-shadow: 0 5px 10px rgba(118, 75, 162, 0.3);
      }
    }
  }
}

.list-div:hover {
  background-color: #f8f9ff;
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
}

.tabs :deep(.el-tabs__nav-wrap) {
  display: flex;
  justify-content: center !important;
}

.tabs :deep(.el-tabs__item) {
  font-size: 18px;
  padding: 0 30px;
  transition: all 0.3s;
  
  &.is-active {
    font-weight: bold;
    color: #6a11cb;
    transform: scale(1.05);
  }
  
  &:hover {
    color: #6a11cb;
  }
}

.tabs :deep(.el-tabs__active-bar) {
  background-color: #6a11cb;
  height: 3px;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.pagination {
  :deep(.el-pagination__sizes),
  :deep(.btn-prev),
  :deep(.btn-next),
  :deep(.el-pager li) {
    transition: all 0.3s;
    
    &:hover {
      transform: translateY(-2px);
    }
  }
  
  :deep(.el-pager li.active) {
    background-color: #6a11cb;
    color: white;
  }
}
</style>
