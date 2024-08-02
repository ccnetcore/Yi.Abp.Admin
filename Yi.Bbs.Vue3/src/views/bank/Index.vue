<template>
  <div class="bank-body">
    <h2>小心谨慎选择银行机构，确保资金安全</h2>
    <div>
      <ExchangeRate :option="statisOptions" />
      <div class="div-show">
        <p class="p-rate">当前实时利息：<span>{{currentRate}}%</span>（可获取投入的百分之{{currentRate}}的本金）</p>
        <el-button type="primary" @click="applying()"><el-icon>
            <AddLocation />
          </el-icon>申领银行卡</el-button>
      </div>
    </div>
    <el-divider />
    <div>
      <el-row :gutter="20" v-if="bankCardList.length > 0">

        <el-col :span=8 v-for="item in bankCardList" :key="item.id">
          <BankCard :card="item" @handlerDeposit="handlerDeposit(item.id)" @handlerDraw="sendDraw(item.id)"></BankCard>
        </el-col>
      </el-row>
      <div v-else> <el-alert title="当前暂未拥有银行卡，请申请！" type="info" center :closable="false" /></div>
    </div>

    <el-dialog v-model="depositDialogVisible" title="输入您的存款金额，最低不能少于50，该卡最大上限100" width="600">
      <el-form :model="depositForm" ref="depositFormRef"  >
        <el-form-item label="存入金额" prop="number">
          <el-input-number v-model="depositForm.number" :min="50" :max="100"  autocomplete="off"/>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="depositDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="sendDeposit">
            确认存入该卡中
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>
<script setup>
import BankCard from "./components/BankCard.vue"
import ExchangeRate from "./components/ExchangeRateChart.vue"
import { getBankCardList, applyingBankCard, getInterestList, depositMoney,drawMoney } from '@/apis/bankApi'
import useAuths from '@/hooks/useAuths.js';
import { computed, ref, onMounted,reactive } from "vue";
import useUserStore from "@/stores/user";
const { isLogin } = useAuths();
const bankCardList = ref([]);
const interestList = ref([]);
const depositForm = ref({
  cardId: null,
  number: 50
});
const userStore=useUserStore();

const depositFormRef = ref(null)

const depositDialogVisible = ref(false);

const refreshData = async () => {

  if (isLogin) {
    const { data } = await getBankCardList();
    bankCardList.value = data;
  }

  const { data: data2 } = await getInterestList();
  interestList.value = data2;
}



onMounted(async () => {
  await refreshData();
})

//申请银行卡
const applying = async () => {

  ElMessageBox.confirm(
    '现在要向行长申领银行卡吗？行长会根据你的【等级】信誉可为你开通不同数量的银行卡',
    '提交申请',
    {
      confirmButtonText: '申请',
      cancelButtonText: '取消',
      type: 'warning',
    }
  )
    .then(async () => {
      await applyingBankCard();
      ElMessage({
        type: 'success',
        message: '领取成功',
      })
      await refreshData();
    });
  //刷新一下

}

//打开存款面板
const handlerDeposit = (cardId) => {
  depositForm.value.cardId = cardId;
  depositDialogVisible.value = true;
}
//进行提款操作
const sendDraw=async (cardId)=>{
  ElMessageBox.confirm(
    '确定现在进行提款吗？如果提前提款，将只能获取存入的本金',
    '提款',
    {
      confirmButtonText: '确认提款',
      cancelButtonText: '取消',
      type: 'warning',
    }
  )
    .then(async () => {
      await drawMoney(cardId)
      await refreshData();
      await userStore.getInfo();
      ElMessage({
        type: 'success',
        message: '钱钱提款成功',
      })
    });


}

//进行存款操作
const sendDeposit = async () => {
  await depositMoney(depositForm.value.cardId, depositForm.value.number);
  depositDialogVisible.value=false;
      await refreshData();
     await userStore.getInfo();
      ElMessage({
        type: 'success',
        message: '钱钱提款成功',
      })
}

const getHours=(timeString)=>{
  const date = new Date(timeString);
  return date.getHours();
}

const currentRate=computed(()=>{
 return ((interestList.value.map(x=>x.value).slice(-1)[0])*100).toFixed(2);
})



const statisOptions = computed(() => {

  return {
    xAxis: {
      data:interestList.value.map(x=>getHours(x.creationTime)+"时")
    },
    series: {
      data: interestList.value.map(x=>(x.value*100))
    },
  };
});
</script>
<style scoped lang="scss">
.bank-body {
  padding: 20px 30px;
}

h2 {
  text-align: center;

}

.div-show {
  display: flex;
  justify-content: space-between;

  .p-rate {
    span {
      font-weight: 600;
      font-size: larger;
    }

  }
}
</style>