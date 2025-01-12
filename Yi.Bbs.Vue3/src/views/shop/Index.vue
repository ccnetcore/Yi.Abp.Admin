<template>
  <div class="content-body">

    <div class="title">
      意社区-商城
    </div>
    <div class="header">
      <span >你的钱钱
       <el-tooltip effect="dark" content="社区通用货币，（来源：通过社区活动获取）" placement="top-start">
             <el-icon><InfoFilled/></el-icon>
        </el-tooltip>
        ：50</span>
      
      
      <span>你的价值
         <el-tooltip effect="dark" content="数字藏品账号价值，（来源：通过社区小程序数字藏品获取）" placement="top-start">
             <el-icon><InfoFilled/></el-icon>
        </el-tooltip>
        ：70</span>
      
      <span> 你的积分
         <el-tooltip effect="dark" content="邀请码积分，（来源：通过社区小程序个人中心邀请获取）" placement="top-start">
             <el-icon><InfoFilled/></el-icon>
        </el-tooltip>
        ：40</span>
    </div>
    
    <div class="shop">
      <el-row :gutter="30">
        <el-col :span="6" v-for="i in 10">
          <ShopCard/>
        </el-col>
      </el-row>
    </div>
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
<script setup lang="ts">
import ShopCard from "./components/ShopCard.vue";
</script>