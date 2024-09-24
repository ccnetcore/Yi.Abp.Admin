<script setup>
import {getAssignmentList, getCanReceiveAssignment, acceptAssignment, receiveAssignment} from '@/apis/assignmentApi'
import {onMounted, reactive, ref} from "vue";
import AssignmentCard from "./components/AssignmentCard.vue"

const canReceiveAssignmentList = ref([]);

const assignmentList = ref([]);
const queryForm = reactive({
  assignmentQueryState: "Progress"
});

//当前选择table页
const currentTableSelect = ref("canAccept");

//切换tab
const changeClickTable = async (tabName) => {
  switch (tabName) {
    case "canAccept":
      const {data: canReceiveAssignmentListData} = await getCanReceiveAssignment();
      canReceiveAssignmentList.value = canReceiveAssignmentListData;
      return;
    case "progress":
      queryForm.assignmentQueryState = "Progress";
      break;
    case "end":
      queryForm.assignmentQueryState = "End";
      break;
  }
  const {data} = await getAssignmentList(queryForm);
  assignmentList.value = data;
}
onMounted(async () => {
  const {data: canReceiveAssignmentListData} = await getCanReceiveAssignment();
  canReceiveAssignmentList.value = canReceiveAssignmentListData;
});
//刷新数据
const refreshData = async () => {

  const {data: canReceiveAssignmentListData} = await getCanReceiveAssignment();
  canReceiveAssignmentList.value = canReceiveAssignmentListData;

  const {data} = await getAssignmentList(queryForm);
  assignmentList.value = data;
}

//接收任务
const onClickAcceptAssignment = async (item) => {
  await acceptAssignment(item.id);
  ElMessage({
    type: 'success',
    message: '接受任务成功',
  });
  await refreshData();
}

//领取奖励
const onClickReceiveAssignment = async (item) => {
  await receiveAssignment(item.id);
  ElMessage({
    type: 'success',
    message: '任务奖励领取成功',
  });
  await refreshData();
}

//切换tab
const changeTab = async (state) => {
  queryForm.assignmentQueryState = state;
  await refreshData();
}


</script>

<template>
  <div class="content-body">
    <el-tabs
        v-model="currentTableSelect"
        type="border-card"
        @tab-change="changeClickTable"
    >
      <el-tab-pane label="可接受" name="canAccept"/>
      <el-tab-pane label="已接受" name="progress"/>
      <el-tab-pane label="已结束" name="end"/>

      <div v-if="currentTableSelect==='canAccept'">
        <div v-for="item in canReceiveAssignmentList" class="assign-box" v-if="canReceiveAssignmentList.length>0">
          <AssignmentCard :isDefind="true" :data="item" @onClick="onClickAcceptAssignment"/>
        </div>
        <el-empty v-else description="暂时没有可领取的任务" />


      </div>
      <div v-else>
        <div v-for="item in assignmentList" class="assign-box" v-if="assignmentList.length>0">
          <AssignmentCard :isDefind="false" :data="item" @onClick="onClickReceiveAssignment"/>
        </div>
        <el-empty v-else description="暂时没有任务" />
      </div>
    </el-tabs>
  </div>
</template>
<style scoped lang="scss">
.content-body {

  padding: 30px;

  .assign-box {
    margin-bottom: 10px;
  }
}


</style>