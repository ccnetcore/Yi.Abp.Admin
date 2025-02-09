<template>
  <el-menu default-active="1" class="el-menu-vertical-demo" @open="handleOpen" @close="handleClose" router>
    <el-menu-item index="1" :route="{ path: '/activity/sign' }">
      <el-icon>
        <Present />
      </el-icon>
      <span>每日签到</span>
    </el-menu-item>
    <el-menu-item index="2" :route="{ path: '/activity/level' }">
      <el-icon>
        <Ticket />
      </el-icon>
      <span>等级</span>
    </el-menu-item>
    <el-menu-item index="3" :route="{ path: '/activity/assignment' }">
      <el-icon>
        <Memo />
      </el-icon>
      <span>任务列表</span>
    </el-menu-item>
    <el-menu-item index="4" :route="{ path: '/activity/lucky' }">
      <el-icon>
        <HelpFilled />
      </el-icon>
      <span>大转盘</span>
    </el-menu-item>
    <el-menu-item index="5" :route="{ path: '/activity/bank' }">
      <el-icon>
        <CreditCard />
      </el-icon> <span>银行</span>
    </el-menu-item>
    <el-menu-item index="6" :route="{ path: '/money' }">
      <el-icon>
        <Money />
      </el-icon>
      <span>排行榜</span>
    </el-menu-item>
    <el-menu-item index="7" :route="{ path: '/shop' }">
      <el-icon>
        <ShoppingCart />
      </el-icon>
      <span>商城</span>
    </el-menu-item>
    <el-menu-item index="8" :route="{ path: '/dc' }">
      <el-icon><Trophy /></el-icon>
      <span>数字藏品</span>
    </el-menu-item>
    <el-menu-item index="9" :route="{ path: '/book' }">
      <el-icon><Memo /></el-icon>
      <span>面试宝典</span>
    </el-menu-item>
  </el-menu>

</template>
<script setup>
const handleOpen = (key, keyPath) => {
  //console.log(key, keyPath)
}
const handleClose = (key, keyPath) => {
 // console.log(key, keyPath)
}
</script>
<style lang="scss" scoped>
.el-menu-item {
  justify-content: center;
}

.el-menu {
  border-right-width: 0px !important;

}
</style>