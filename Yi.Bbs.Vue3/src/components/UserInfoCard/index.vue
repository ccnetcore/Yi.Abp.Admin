<template>
  <div v-if="modelValue" class="userInfo-card" ref="cardRef">
    <!-- 个人信息内容 -->
    <div class="info">
      <h2>温海靖</h2>
      <p>XXX</p>
      <!-- 其他个人信息... -->
    </div>
  </div>
</template>

<script setup>
import { ref, computed, nextTick, defineProps, defineEmits } from "vue";

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false,
  },
  actOnRef: {},
});
const emit = defineEmits("update:modelValue");
const cardRef = ref(null);
const avatarRef = computed(() => props.actOnRef);

nextTick(() => {
  document.addEventListener("mouseup", (e) => {
    // 如果点击的是按钮   则不执行下面的操作
    if (avatarRef.value && cardRef.value) {
      if (cardRef.value.contains(e.target)) {
        return;
      }
      if (!cardRef.value.contains(e.target)) {
        // 点击的区域不包含在弹窗区域之内就关闭弹窗
        emit("update:modelValue", false);
      }
    }
  });
});
</script>

<style scoped>
.userInfo-card {
  width: 100px;
  height: 100px;
  position: absolute;
  top: 0;
  left: 0;
  background-color: pink;
  /* 卡片样式... */
}
</style>
