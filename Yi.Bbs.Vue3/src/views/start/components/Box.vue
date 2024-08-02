<script setup>
import { ref,watch } from 'vue';

const props = defineProps(["text", "isSelect", "boxKey"]);
const emit = defineEmits(['onSelected']);
const isSelectRef=ref(false);
const clickBox = () => {
    isSelectRef.value=true;
    emit('onSelected',props.boxKey)
}
watch(()=>props.isSelect,(n,o)=>{
    isSelectRef.value=n;
},{immediate:true})


</script>
<template>
    <div class="box" 
    :class="{ selected: isSelectRef }" 
    @click="clickBox">
        {{ props.text ?? "-" }}
    </div>
</template>
<style lang="scss" scoped>
.selected {
    border: 2px solid #409EFF !important;
    background-color: #FFFFFF !important;
}

.box {
    cursor: pointer;
    color: #292d33;
    background-color: #fafafa;
    padding: 18px !important;
    font-size: 13px !important;
    border: 2px solid transparent;
    font-weight: 500 !important;
    box-shadow: 1px 1px 2px rgba(11, 15, 33, .2) !important;
    border-radius: 8px;
    display: flex;
    justify-content: center;
}

.box:hover {
    border: 2px solid #409EFF;

}
</style>