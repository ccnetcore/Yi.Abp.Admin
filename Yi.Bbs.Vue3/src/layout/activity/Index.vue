<template>
  <div class="common-layout">
    <el-container class="common-container">
      <el-header
        class="common-header"
        ref="header"
        :class="[isFixed ? 'fixed' : '']"
      >
        <AppHeader />
      </el-header>
      <el-main class="common-main">
        <ActivityBody />
      </el-main>
    </el-container>
  </div>
</template>
<script setup>
import { ref, onMounted } from "vue";
import AppHeader from "../AppHeader.vue";
import ActivityBody from "./components/ActivityBody.vue";

const header = ref(null);
const isFixed = ref(false);

onMounted(() => {
  window.addEventListener("scroll", handleScroll);
});

const handleScroll = () => {
  const scrollTop =
    window.scrollY ||
    document.documentElement.scrollTop ||
    document.body.scrollTop;
  const currentEle = header.value.$el;
  if (scrollTop > currentEle.offsetTop) {
    isFixed.value = true;
  } else {
    isFixed.value = false;
  }
};
</script>

<style scoped lang="scss">
.common {
  &-layout {
    width: 100%;
    height: 100%;
  }
  &-container {
    width: 100%;
    height: 100%;
  }
  &-header {
    width: 100%;
    background-color: #fff;
    box-shadow: rgba(0, 0, 0, 0.1) -4px 9px 25px -6px;
    height: 60px;
    display: flex;
    justify-content: center;
  }
  &-main {
    display: flex;
    justify-content: center;
    width: 100%;
    height: calc(100% - 50px);
  }
}

.el-main {
  margin: 0;
  padding: 0;
  min-height: 10rem;
  background-color: #f0f2f5;
}

.fixed {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 99999;
}
</style>
