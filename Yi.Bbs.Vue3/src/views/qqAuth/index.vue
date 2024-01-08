<template>
  <div class="message">{{ message }}</div>
</template>

<script setup>
import { ref, watch } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();

const code = ref(route.query.code);

const message = ref("");
watch(
  () => code.value,
  (val) => {
    if (val) {
      message.value = "授权成功";
      // window.close();
    }
  },
  { immediate: true }
);
</script>

<style lang="scss">
.message {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  font-size: 20px;
}
</style>
