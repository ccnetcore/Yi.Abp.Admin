<template>
    <p><el-icon><InfoFilled /></el-icon> 以下模板变量将会被替换：</p>
    <p v-for="(item,i) in text" :key="i">
     <el-text class="mx-1" type="primary">【{{item.oldValue}}】</el-text>替换为
     <el-text class="mx-1" type="Default">【{{item.newValue}}】</el-text>
    </p>
</template>
<script setup>
const text=[
{
    oldValue:"@model",
    newValue:"实体名字，小写开头"
},
{
    oldValue:"@Model",
    newValue:"实体名字，大写开头"
},
{
    oldValue:"@namespace",
    newValue:"命名空间"
},
{
    oldValue:"@field",
    newValue:"实体字段"
}
]
</script>
<style scoped>
.mx-1
{
    margin-right: 10px;
}
p{
    padding-left: 100px;
}
</style>