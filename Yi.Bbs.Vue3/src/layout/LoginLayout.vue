<template>
  <div class="box">
    <div class="content">
      <RouterView />
    </div>
  </div>
</template>
<!-- <style src="@/assets/styles/login.scss" scoped></style> -->

<style scoped lang="scss">
.box {
  width: 100vw;
  height: 100vh;
  .content {
    width: 100%;
    height: 100%;
  }
}
</style>
