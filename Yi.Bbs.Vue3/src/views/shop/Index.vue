<script setup lang="ts">
import ShopCard from "./components/ShopCard.vue";
import {getShopList,getAccountInfo,postBuy} from "@/apis/shopApi.js";
import {onMounted, reactive, ref} from "vue";
import { ElMessage } from 'element-plus'
const  buyForm=reactive({
  goodsId:"",
  contactInformation:""
});
const shopList=ref([]);
const accountInfo=ref({});
const dialogFormVisible=ref(false);
onMounted(async ()=>{
await initData();
})
const  initData=async ()=>{
  const shopListData= await getShopList({skipCount: 0, maxResultCount: 100});
  shopList.value=shopListData.data.items;
  const accountInfoData= await getAccountInfo();
  accountInfo.value=accountInfoData.data;
}

const currentGoods=ref({});
//点击购买商品
const clickBuy=(data)=>{
  dialogFormVisible.value=true;
  currentGoods.value=data;
}
//确认购买
const confirmBuy=async()=>{
  buyForm.goodsId=currentGoods.value.id;
  await postBuy(buyForm);
  dialogFormVisible.value = false;
  await initData();
  ElMessage({
    message: '申请购买成功',
    type: 'success',
  })

}
</script>
<template>
  <div class="content-body">

    <div class="title">
      意社区-商城
    </div>
    <div class="header">
      <span >你的钱钱
       <el-tooltip effect="dark" content="社区通用货币，成功购买后将扣减（来源：通过社区活动获取）" placement="top-start">
             <el-icon><InfoFilled/></el-icon>
        </el-tooltip>
        ：{{accountInfo.money}}</span>
      
      
      <span>你的价值
         <el-tooltip effect="dark" content="数字藏品账号价值，成功购买后不会扣减（来源：通过社区小程序数字藏品获取）" placement="top-start">
             <el-icon><InfoFilled/></el-icon>
        </el-tooltip>
        ：{{accountInfo.value}}</span>
      
      <span> 你的积分
         <el-tooltip effect="dark" content="邀请码积分，成功购买后将会扣减（来源：通过社区小程序个人中心邀请获取）" placement="top-start">
             <el-icon><InfoFilled/></el-icon>
        </el-tooltip>
        ：{{accountInfo.points}}</span>
    </div>
    
      <el-row :gutter="30" class="shop">
        <el-col :span="6" :xs="12" :sm="12" :md="8" :lg="6" :xl="6" v-for="item in shopList"  :key="item.id">
          <ShopCard :data="item" :realData="accountInfo" @clickBuy="clickBuy"/>
        </el-col>
      </el-row>



    <el-dialog  title="申请购买" v-model="dialogFormVisible">
      <el-form :model="buyForm">
        <el-form-item label="联系方式（微信号）" :label-width="200">
          <el-input v-model="buyForm.contactInformation" autocomplete="off"></el-input>
        </el-form-item>
       
      </el-form>
      <template #footer>
        <p>后续，如您符合申请条件，官方将定期将会与您联系</p>
        <el-button @click="dialogFormVisible = false">取 消</el-button>
        <el-button type="primary" @click="confirmBuy()">确定无误，申请购买</el-button>
      </template>
    </el-dialog>
    
  </div>

</template>
<style scoped>
.header span {
  margin-left: 50px;
}

.el-col {
  margin: 10px 0;
}

.content-body {
  margin-bottom: 40px;
  margin-top: 20px;
  padding: 20px;
  background-color: #ffffff;
  width: 60%;

  .title {
    display: flex;
    justify-content: center;
    font-size: 40px;
    margin-bottom: 20px;
  }

  .header {
    display: flex;
    justify-content: center;
    margin-bottom: 40px;
  }

  .shop {
    display: flex;
  }
}
</style>
