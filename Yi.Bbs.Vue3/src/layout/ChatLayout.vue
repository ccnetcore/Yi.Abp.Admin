<template>
    <div class="chat-body" >

<RouterView />
    </div>
</template>
<script setup>
import {onMounted, onUnmounted} from "vue";
import {start,close} from "@/hubs/chatHub.js";
onMounted( () => {
    start();
});
onUnmounted(()=>{
    close(); 
})
</script>
<style scoped>
.chat-body
{
    height: 100%;
    /* padding: 5%; */
    display: flex;
    justify-content: center;
    align-content: center;
    flex-wrap: wrap;
}
</style>
