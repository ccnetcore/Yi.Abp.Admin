
<script setup>
import BookCard from './components/BookCard.vue'
import {getList} from '@/apis/discussApi'
import { ref, onMounted } from "vue";
import {useRouter} from "vue-router";
const discussList=ref([]);

//面试宝典板块id
const bookPlateId="d940818d-90ec-9dbe-b7af-3a0f935dac0a";
onMounted(async ()=>{
  const {data}=await getList({plateId:bookPlateId, skipCount:1,maxResultCount:100});
  discussList.value=data.items;
})
const router = useRouter();
const enterDiscuss = (id) => {
  router.push(`/article/${id}`);
};
</script>

<template>
  <div class="body">
    <p class="title">面试宝典</p>
    <div class="content">
      <el-row :gutter="20">
        <el-col @click="enterDiscuss(discussInfo.id)" v-for="discussInfo in discussList" :xs="12" :sm="8" :md="8" :lg="6" :xl="6">
          <BookCard :discuss="discussInfo"/>
        </el-col>
      </el-row>
      
    </div>
  </div>

</template>
<style scoped>
.el-col{
  cursor: pointer; /* 显示手型光标 */
  margin: 10px 0;
}
.body{
  width: 1200px;
  .title{
    margin-top: 0.5em;
    font-size: 24px;
    line-height: 1.2667;
    color: rgba(0, 0, 0, 0.88);
    font-weight: 600;
  }
  
}


</style>