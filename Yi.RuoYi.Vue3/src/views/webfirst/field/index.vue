<template>
    <el-row>
    <el-col  :span="4"> <TableList @selectTable="selectTable"></TableList></el-col>
    <el-col  :span="20">   <FieldList class="fileList" :table="table"></FieldList></el-col>
    </el-row>
</template>
<script setup>
import { ref ,watch} from 'vue'
import FieldList from './components/FieldList.vue'
import TableList from './components/TableList.vue'
const table=ref({});
const options = ref({})
const selectTable=(table)=>{
options.value = table
}
watch(() => options.value,val => {
    table.value = val
},{deep:true})
</script>
<style scoped>
.fileList{

   
}
.tableList{

}
</style>