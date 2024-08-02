<script setup>
import { onMounted, ref } from "vue";
import Box from "./Box.vue"
const props = defineProps(["data", "width"]);
const emit = defineEmits(['onSelected','onSelectedValue']);
const selectData=ref(props.data.map(obj => ({ ...obj, isSelect: false })));
onMounted(()=>{
    selectData.value[0].isSelect=true;
})

const onSelected=(boxKey)=>{
    selectData.value.map(obj => {
  if (obj['key'] === boxKey) {
    emit('onSelected',obj);
    return obj.isSelect=true;
  } else {
    return obj.isSelect=false;
  }
});

}
/*
* [{name:"",key:"",value:""}]
*
*/
</script>
<template>
    <div class="select-box">
        <Box class="box" 
        :isSelect="item.isSelect" 
        :style="{ width: props.width }" 
        v-for="(item, index) in selectData"
        :key="item.key" :text="item.name"
        :boxKey="item.key"
        @onSelected="onSelected"
        />
    </div>
</template>
<style lang="scss" scoped>
.select-box {
    display: flex;

    .box {
        margin-right: 10px;
    }
}
</style>