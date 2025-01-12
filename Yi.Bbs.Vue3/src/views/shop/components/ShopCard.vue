<template>
  <el-card  shadow="hover">
    <template #header>商品-物品A</template>
    <img
        src="https://ccnetcore.com/prod-api/wwwroot/logo.png"
        style="width: 100%"
     alt=""/>
    <ul>
      <li>所需钱钱（扣除）：666</li>
      <li>所需价值（拥有）：777</li>
      <li>所需积分（拥有）：888</li>
      <li>限制数量：1</li>
    </ul>
    <el-divider />
    <div class="bottom"> 
      <el-button type="success">申请购买</el-button>
    </div>
  </el-card>
</template>
<script setup lang="ts">
</script>
<style scoped lang="scss">
.bottom
{
  display: flex;
  justify-content: flex-end;
  margin-top: 20px;
}
</style>