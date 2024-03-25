<template>
    <div class="card">
        <div class="card-body"
         :class="{ 'card-wait': cardData.bankCardState=='Wait',
        'card-unused': cardData.bankCardState=='Unused'}">
            银行卡-{{ cardData.id.substring(0,8) }}
            <p>当前余额：{{ cardData.storageMoney }}钱钱</p>
            <p>状态：{{ state }}</p>
            <p v-if="cardData.bankCardState=='Wait'">剩余提款时间: {{ time }}</p>
        </div>
        <div class="div-oper">
            <el-button v-if="cardData.bankCardState=='Unused'" @click="handlerDeposit">存款</el-button>
            <el-button v-if="cardData.bankCardState=='Wait'" type="danger" @click="handlerDraw(cardData.id)">提款</el-button>
        </div>
    </div>
</template>
<script setup>
import {computed,watch,ref} from 'vue'
const props = defineProps(['card'])
const emit =defineEmits(['handlerDeposit'])
const cardData=ref(props.card);
const bankCardStateEnum=[
{key:'Unused',value:"闲置中"},
{key:'Wait',value:"存款中"}
]
const state=computed(()=>{
   return bankCardStateEnum.filter(x=>x.key==cardData.value.bankCardState)[0].value
})

const time=computed(()=> {
    // 将传入的字符串转换为时间对象
    const inputDate = new Date(props.card.fulltermTime);
    
    // 获取当前时间
    const currentDate = new Date();
    
    // 计算时间差（以毫秒为单位）
    const timeDiff = inputDate.getTime() - currentDate.getTime();

    // 如果传入时间早于当前时间，则返回"已满"提示
    if (timeDiff <= 0) {
        return "0";
    }

    // 将时间差转换为天、小时、分钟和秒
    const days = Math.floor(timeDiff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((timeDiff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))+1;
    // 返回剩余时间字符串
    return `${days}天${hours}时`;
}
)

const handlerDraw=(cardId)=>{
    emit('handlerDraw',cardId)
}

//存款
const handlerDeposit=()=>{
    emit('handlerDeposit')
};

watch(
  () => props.card,
   (val, oldValue) => {
    cardData.value=props.card;
  },
  { immediate: true, deep: true }
);
</script>
<style scoped lang="scss">
.card-wait
{
    background-color:brown;
}
.card-unused
{ background-color: #FDC830;
   
}
.card {
    height: 140px;
    margin: 10px 0;
    border-radius: 15px;
    box-shadow: rgba(0, 0, 0, 0.2) 0px 4px 8px 0px;

    .card-body {
        padding: 5px;
        height: 100px;
        // background-color: #FDC830;
        color: #FFFFFF;
        border-radius: 15px 15px 0 0;
    }

    .div-oper {
        padding: 5px;
        padding-right: 10px;
        border-radius: 0 0 15px 15px;
        background-color: #FFFFFF;
        text-align: end;
        height: 40px;
    }
}
</style>