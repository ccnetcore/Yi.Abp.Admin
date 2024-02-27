<template>
    <div class="container">
        <div class="top">

        <h4>当前等级: {{userInfo.level}}-{{ userInfo.levelName }}</h4>
        <h4>当前钱钱: {{userInfo.money}}</h4>
        <div class="title">
            <div class="left">当前等级经验:</div>
            <div class="right"> 
                <el-progress  :percentage="(userInfo.experience/nextExperience).toFixed(2)*100" :stroke-width="15" striped  striped-flow />
                <div>{{userInfo.experience}}/{{ nextExperience }}</div>
            </div>
        </div>
      
        </div>

        <div class="bottom">
            <el-input-number v-model="moneyNum" :min="1" :max="10000" />
            
            <el-button @click="onUpgradeClick" type="primary">升级</el-button>

            <span>所需钱钱：{{ moneyNum }}</span>
        </div>


        <el-table :data="levelData" border style="width: 100%"  >
    <el-table-column prop="currentLevel" label="等级" width="80" align="center" />
    <el-table-column prop="name" label="称号" width="180" align="center" />
    <el-table-column prop="minExperience" label="所需经验"  width="180" align="center" />
    <el-table-column prop="nick" label="其他"  align="center" />
  </el-table>


    </div>
    
</template>
<script  setup>
import {getList,upgrade} from '@/apis/levelApi.js'
import {getBbsUserProfile} from '@/apis/userApi.js'
import { ref,onMounted, reactive,computed  } from 'vue'
import  useAuths  from '@/hooks/useAuths.js';
import useUserStore from "@/stores/user";
const { isLogin } = useAuths();
const userInfo=ref({});
const currentUserInfo=useUserStore();
const levelData =ref([]);
const moneyNum=ref(1);

const query=reactive({
    skipCount:0,
    maxResultCount:20
})

const nextExperience=computed(()=>{
   return levelData.value?.filter(x=>x.currentLevel==userInfo.value.level+1)[0]?.minExperience
})

const onUpgradeClick=async ()=>{
    await upgrade(moneyNum.value);
    await loadLevelData();
    await loadUserInfoData();
}

onMounted(async () => {
   await loadLevelData();
await loadUserInfoData();
})

const loadLevelData=  async () => {
    moneyNum.value=1;
   const {data:{items}} = await getList(query);
    levelData.value = items;
}
const loadUserInfoData=async()=>{
    if(isLogin)
   {
        const {data}= await getBbsUserProfile(currentUserInfo.id);
        userInfo.value=data;
   }
}
</script>
<style lang="scss" scoped >
.container{
padding: 20px 20px;
.title{
    display: flex;
    .left{
        width: 15%;
    }
    .right{
        width: 60%;
    }
}
.bottom{
    margin: 20px 0px;
        .el-button{
            margin-left: 10px;
            margin-right: 10px;
        }
}
}
</style>>