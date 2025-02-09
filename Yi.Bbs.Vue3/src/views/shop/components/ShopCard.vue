<script setup>
import {computed, ref, watch} from "vue";
import { dayjs } from 'element-plus'
const data=ref({});
const realData=ref({});
const props = defineProps([
  "data","realData"
]);
const emit = defineEmits(['clickBuy'])
watch(()=>props.data,(n)=>{
  data.value=n;
},{deep:true, immediate: true})

watch(()=>props.realData,(n)=>{
  realData.value=n;
},{deep:true, immediate: true})

const clickBuy=async ()=>{
  
  emit('clickBuy',data.value)
}

const isConformToRule=computed(()=>{
  if (data.value.isLimit===true)
  {
    return true;
  }
  if (realData.value.money>=data.value.needMoney
      &&realData.value.value>=data.value.needValue
      &&realData.value.points>=data.value.needPoints
      &&data.value.stockNumber>0)
  {
   return true; 
  }
  return false
  
})
</script>


<template >
  <div class="shop-card" >
  <el-card  shadow="hover">
    <template #header>{{data.name}}</template>
    <img
        :src="data.imageUrl"
        style="width: 100%"
     alt=""/>
    
    <ul>
      <li style="font-size: smaller;color: #7c8188;margin-bottom: 5px">简介：{{data.describe}}</li>
      <li :class="{'less-li': realData.money<data.needMoney}">所需钱钱：{{data.needMoney}}</li>
      <li :class="{'less-li': realData.value<data.needValue}">所需价值：{{data.needValue}}</li>
      <li :class="{'less-li': realData.points<data.needPoints}">所需积分：{{data.needPoints}}</li>
      <li >到期时间：{{dayjs(data.endTime).format("YYYY/MM/DD")}}</li>
      <li>限购数量：{{data.limitNumber}}</li>
      <li>剩余：{{data.stockNumber}}</li>
    </ul>
    <el-divider />
    <div class="bottom"> 
      
      <el-button v-if="!isConformToRule" :disabled="true" type="danger">条件不足</el-button>
      <el-button v-else :disabled="data.isLimit" type="success" @click="clickBuy">{{data.isLimit===true?"已申请":"申请购买"}} </el-button>
    </div>
  </el-card>
    </div>
</template>

<style scoped lang="scss">
.bottom
{
  display: flex;
  justify-content: flex-end;
  margin: 10px;
}
.less-li{
  color: red;
}
img{
  height: 100px;
  width: 100%;
  background-repeat: no-repeat;
  background-size: cover;
  background-position: center;
}
.shop-card :deep(.el-card__body) {
  padding: 0 2px 0 0!important;
}
.shop-card :deep(.el-card__header)
{
  display: flex;
  justify-content: center;
  padding: 5px;
  //color: red;
  font-weight: bolder;
  
}
ul{
  padding: 10px;
}
.shop-card :deep(.el-divider)
{
  margin: 0;
}
</style>