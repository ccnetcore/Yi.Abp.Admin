<script setup>
import {computed, ref, watch} from "vue";
import { dayjs } from 'element-plus'
const props = defineProps(['data', 'isDefind'])
const cardData = ref(props.data);

const emit = defineEmits(['onClick'])
const onClick = () => {
  emit('onClick', cardData.value)
}
//监视组件传值变化
watch(() => props.data, (n, o) => {
  cardData.value = n;
})

//任务类型
const assignmentTypeEnum = {
  "Daily": {name:"每日任务",backgroundColor:"#fff"},
  "Weekly":{name:"每周任务",backgroundColor:"#fff"} ,
  "Novice": {name:"新手任务",backgroundColor:"#fff"}
}
const computedAssignmentState = computed(() => {

  if (props.isDefind) {
    return btnAssignmentStateEnum["CanReceive"];
  } else {
    return btnAssignmentStateEnum[cardData.value.assignmentState];
  }
});
const btnAssignmentStateEnum = {
  "CanReceive": {name: "接受任务", backgroundColor: "primary",isDisabled:false},
  "End": {name: "已完成", backgroundColor: "info",isDisabled:true},
  "Progress": {name: "正在进行", backgroundColor: "Default",isDisabled:true},
  "Completed": {name: "领取奖励", backgroundColor: "success",isDisabled:false},
  "Expired": {name: "已过期", backgroundColor: "info",isDisabled:true}
}
</script>

<template>
  <div class="card-box">
    <div class="left">
      <div class="left-type" :style="{backgroundColor:assignmentTypeEnum[cardData.assignmentType].backgroundColor}">{{ assignmentTypeEnum[cardData.assignmentType].name }}</div>
      <div class="content">
        <div class="content-title">
        <h2>{{ cardData.name }}</h2>
        <h4>{{ cardData.remarks }}</h4>
        </div>
        <div class="progress" v-if="props.isDefind===false">
        <el-progress
            :text-inside="true"
            :stroke-width="20"
            :percentage=" Math.round((cardData.currentStepNumber/cardData.totalStepNumber)*100)"

            status="success"
        />
       <span>{{cardData.currentStepNumber}}/{{cardData.totalStepNumber}}</span>
        </div>
      </div>

    </div>


    <div class="right">
      <div class="right-btn">
       
        <h5> {{cardData.expireTime ==null?"":"过期时间:"+dayjs(cardData.expireTime).format('YYYY年M月D日')}}</h5>
       <h5>奖励：<span style="color: #FF0000;font-weight: bolder ">{{cardData.rewardsMoneyNumber}}</span> 钱钱</h5>
        <el-button @click="onClick()" :disabled="computedAssignmentState.isDisabled" :type="computedAssignmentState.backgroundColor">
          {{ computedAssignmentState.name }}
        </el-button>
      </div>

    </div>


  </div>
</template>
<style scoped lang="scss">
.el-progress{
width: 450px;
}
.el-button {
  width: 100px;
  height: 40px;
  margin-top: 5px;
}

h2 {
  margin: 0 0 10px 0;
}

h4 {
  margin: 0 0 0 20px;
  display: flex;
  align-items: center;
}
h5 {
  margin: 0;
}


.card-box {
  padding: 10px;
  border: 2px solid #dcdfe6;
  display: flex;
  justify-content: space-between;
  height: 100px;
  width: 100%;
  align-content: center;
  flex-wrap: wrap;

  .right {
    display: flex;
    align-content: center;
    flex-wrap: wrap;

    .right-btn {
      text-align: right;
    }
  }

  .left {
    display: flex;

    align-content: center;
    flex-wrap: wrap;
    .content {
      margin-left: 30px;
      .content-title{
        display: flex;
      }
      .progress{
        display: flex;
      }
      span{
        margin: 0 10px;
      }
    }

    .left-type {
      border: 1px solid #dcdfe6;
      height: 60px;
      width: 100px;
      display: flex;
      align-content: center;
      flex-wrap: wrap;
      justify-content: center;
    }
  }
}

</style>