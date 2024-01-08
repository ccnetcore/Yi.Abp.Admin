<script setup>
import { ref } from "vue";

const props = defineProps(["items", "header", "text", "hideDivider", "height"]);
const height = ref(props.height + "px");
</script>

<template>
  <el-card class="box-card" shadow="never">
    <template #header>
      <div class="card-header">
        <span>{{ props.header }}</span>
        <el-link :underline="false" type="primary">{{ props.text }}</el-link>
      </div>
    </template>

    <slot name="content" />
    <el-scrollbar :height="height">
      <div v-for="(item, i) in props.items">
        <div class="text item">
          <slot name="item" v-bind="item" />
        </div>
        <el-divider
          v-if="i != props.items.length - 1 && hideDivider == undefined"
        />
      </div>
    </el-scrollbar>
  </el-card>
</template>

<style scoped>
.el-divider {
  margin: 0.2rem 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.text {
  font-size: 14px;
}

.item {
  margin: 1rem 0;
}

.box-card {
  width: 100%;
  height: v-bind(height);
  padding-bottom: 10px;
}
</style>
