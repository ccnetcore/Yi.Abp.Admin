<template>
       <el-button text @click="agree">
                            <el-icon v-if="data.isAgree" color="#409EFF">
                                <CircleCheckFilled />
                            </el-icon>
                            <el-icon v-else color="#1E1E1E">
                                <Pointer />
                            </el-icon> 点赞:{{ data.agreeNum ?? 0 }}</el-button>
    </template>
    <script setup>
import {onMounted,reactive,watch} from 'vue'
import { operate } from '@/apis/agreeApi'


//'isAgree','agreeNum','id'
const props = defineProps([ 'data'])

watch(()=>props,(n)=>{
    data.id=n.data.id;
    data.isAgree=n.data.isAgree;
    data.agreeNum=n.data.agreeNum;
},{deep:true})


const data=reactive({
    id:'',
    isAgree:false,
    agreeNum:0
})
// onMounted(()=>{
  
// })
//点赞操作
const agree = async () => {
    const response = await operate(data.id)
    const res = response.data;
    //提示框，颜色区分
    if (res.isAgree) {
        data.isAgree = true;
        data.agreeNum += 1;
        ElMessage({
            message: res.message,
            type: 'success',
        })
    }
    else {
        data.isAgree = false;
        data.agreeNum-= 1;
        ElMessage({
            message: res.message,
            type: 'warning',
        })
    }
}
</script>