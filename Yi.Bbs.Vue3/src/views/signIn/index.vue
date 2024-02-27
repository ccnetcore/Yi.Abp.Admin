<template>
  <div class="everyday-box">
    <h4>每日签到</h4>
    <h5>
      <el-button class="btn-signin" type="primary" @click="signInOnclic">点击完成今日签到</el-button>当前已连续签到{{number}}天

      <el-tooltip >
        <template #content>
          1.每天随机3-10<br />
          2.连续签到每次累加多+1<br />
          3.随机到以9结尾，额外再获取1倍*2<br />
          4.每次签到最大上限为30
        </template>
 
       <span>说明<el-icon><WarningFilled /></el-icon></span>
      </el-tooltip>
 
    </h5>
    

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
 await loadSignInData();
};

const loadSignInData=async()=>{
  if (isLogin) {
    const {  data:{signInItem,currentContinuousNumber} } = await signInRecord();
    number.value=currentContinuousNumber;
    signInData.value=signInItem;
  }
}

onMounted(async () => {
  //登录后才去查询签到记录
 await loadSignInData();
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
  padding: 0px  30px;
  .btn-signin
{
  margin-right: 20px;
}
h5{
  color: #181818;
}
}
.is-selected {
  color: #1989fa;
}
</style>
