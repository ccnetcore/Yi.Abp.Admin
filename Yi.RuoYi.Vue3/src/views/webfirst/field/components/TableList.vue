<template>
      <h3 class="title">数据表选择</h3>
      <el-menu
        default-active="2"
        class="el-menu-vertical-demo"
      >
      <el-menu-item  v-for="(item,i) in dataList" :index="'data-'+i" :key="i" @click="menuClick(item)">
          <el-icon><setting /></el-icon>
          <span>{{item.name}}</span>
      </el-menu-item>
  </el-menu>
</template>

<script lang="ts" setup>
import useTable from '../hooks/useTable'
const emit = defineEmits(['selectTable'])

function menuClick(item) {
  emit('selectTable',item)
}
const {dataList}=useTable();
</script>
<style scoped>
.el-menu-item{
  justify-content: center;
  max-height: 1000px;
}
.title
{
  text-align: center;
}
</style>