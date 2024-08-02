<template>
  <div>
    <h2>谨记人生赌博，不如理性规划</h2>
    <h3>50钱钱一次,我的钱钱:{{ userInfo?.money ?? '未登录' }}</h3>
    <LuckyWheel class="wheel" ref="myLucky" width="500px" height="500px" :prizes="prizes" :blocks="blocks"
      :buttons="buttons" @start="startCallback" @end="endCallback" />
  </div>
</template>
  
<script setup>
import { ref, onMounted } from "vue";
import useAuths from '@/hooks/useAuths.js';
const { isLogin } = useAuths();
import { getBbsUserProfile } from '@/apis/userApi.js'
import { luckyWheel } from '@/apis/integralApi.js'
import useUserStore from "@/stores/user";
const userInfo = ref({});
const currentUserInfo=useUserStore();
onMounted(async () => {
  await loadUserInfoData();
})
const loadUserInfoData = async () => {
  if (isLogin) {
    const { data } = await getBbsUserProfile(currentUserInfo.id);
    userInfo.value = data;
  }
}

const blocks = [{ padding: '13px', background: '#617df2' }];
const prizes = [
  { fonts: [{ text: '啥也没有', top: '10%' }], background: '#e9e8fe' },
  { fonts: [{ text: '10', top: '10%' }], background: '#b8c5f2' },
  { fonts: [{ text: '30', top: '10%' }], background: '#e9e8fe' },
  { fonts: [{ text: '50', top: '10%' }], background: '#b8c5f2' },
  { fonts: [{ text: '60', top: '10%' }], background: '#e9e8fe' },
  { fonts: [{ text: '80', top: '10%' }], background: '#b8c5f2' },
  { fonts: [{ text: '90', top: '10%' }], background: '#e9e8fe' },
  { fonts: [{ text: '100', top: '10%' }], background: '#b8c5f2' },
  { fonts: [{ text: '200', top: '10%' }], background: '#e9e8fe' },
  { fonts: [{ text: '666', top: '10%' }], background: '#FAD400' }
];
const buttons = [{
  radius: '35%',
  background: '#8a9bf3',
  pointer: true,
  fonts: [{ text: '启动', top: '-10px' }]
}];

const myLucky = ref(null);
// 点击抽奖按钮会触发star回调
const startCallback = () => {
  var index =0;
  // 调用抽奖组件的play方法开始游戏
  if (!isLogin) {
    ElMessage({
      message: '请登录后启动！',
      type: 'warning',
    })
    return;
  }
  ElMessageBox.confirm(
    '每次启动需要消耗50钱钱，确定要启动吗?',
    '警告',
    {
      confirmButtonText: '启动',
      cancelButtonText: '放弃',
      type: 'warning',
    }
  )
    .then(async () => {
      myLucky.value.play()




      //等待3秒
      // 模拟调用接口异步抽奖
       setTimeout(() => {
        // 假设后端返回的中奖索引是0
        // 调用stop停止旋转并传递中奖索引
      }, 3000)

      try
      {
        const ddd=(await luckyWheel()).data;
      index= (await luckyWheel()).data;
      }
      catch
      {

      }
      finally{

        myLucky.value.stop(index)
      }


   
    })
    .catch(() => {
      ElMessage({
        type: 'info',
        message: '成功克制',
      })
    })

};
// 抽奖结束会触发end回调
const endCallback = async (prize) => {

  ElMessage({
    type: 'success',
    message: `恭喜你抽中了[${prize.fonts[0].text}]`,
  })
  await loadUserInfoData();
}
</script>
<style scoped>
.wheel {
  margin: auto;
  margin-top: 10%;
}

h2 {
  text-align: center;
}

h3 {
  text-align: center;
}
</style>