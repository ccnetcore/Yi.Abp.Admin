<template>
  <div class="everyday-box">
    <h4>每日签到页持续coding中~~~</h4>
    <h5>当前已连续签到{{number}}天</h5>
    <el-button type="primary" @click="signInOnclic">点击完成签到</el-button>

    <el-calendar  v-model="signInRecordData">
      <template #date-cell="{ data }">
        <p :class="containSameDay(data.date) ? 'is-selected' : ''">
          {{ data.day.split("-").slice(1).join("月") + "日" }}
          <br/>
          {{containSameDay(data.date)? "已签到✔️" : "" }}
        </p>
      </template>
    </el-calendar>
  </div>
</template>

<script setup>
import { onMounted, ref, reactive, computed, nextTick, watch } from "vue";
import { signIn, signInRecord } from "@/apis/integralApi.js";
import useAuths from "@/hooks/useAuths";
const { isLogin } = useAuths();
const number=ref(0);
const signInData=ref([]);
const signInRecordData = ref(new Date());
const signInOnclic = async () => {
  const { data: data } = await signIn();

  ElMessage({
    message: `恭喜！运气爆棚，今日获得：${data.value} 钱钱`,
    type: "success",
  });
};

onMounted(async () => {
  //登录后才去查询签到记录
  if (isLogin) {
    const {  data:{signInItem,currentContinuousNumber} } = await signInRecord();
    number.value=currentContinuousNumber;
    signInData.value=signInItem;
  }
});

const containSameDay=(time)=>{
 return signInData.value.filter(x=>isSameDay(x.creationTime,time)).length==0?false:true;
}

//判断两个时间是否为同一天
const isSameDay=(time1, time2)=> {
  const date1 = new Date(time1);
  const date2 = new Date(time2);
  return (
    date1.getFullYear() === date2.getFullYear() &&
    date1.getMonth() === date2.getMonth() &&
    date1.getDate() === date2.getDate()
  );
}

</script>

<style lang="scss" scoped>
.everyday-box {
  width: 100%;
  height: 100%;
}
.is-selected {
  color: #1989fa;
}
</style>
