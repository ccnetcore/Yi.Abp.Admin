<template>
  <router-view />
</template>

<script setup>
import useSettingsStore from '@/store/modules/settings'
import { handleThemeStyle } from '@/utils/theme'
import useUserStore from '@/store/modules/user'
import { storeToRefs } from 'pinia';
import signalR from '@/utils/signalR'

const {token}=storeToRefs(useUserStore());

onMounted(async () => {
  await  signalR.init(`main`);
  nextTick(() => {
    // 初始化主题样式
    handleThemeStyle(useSettingsStore().theme)
  })
})


//这里还需要监视token的变化，重新进行signalr连接
watch(()=>token.value,async (newValue,oldValue)=>{
  await  signalR.init(`main`);
})

</script>
