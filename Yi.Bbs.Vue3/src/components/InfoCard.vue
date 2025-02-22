<script setup>
import { ref } from "vue";

const props = defineProps(["items", "header", "text", "hideDivider", "height","isPadding"]);
const emit = defineEmits(['onClickText'])
const height = ref(props.height + "px");

const onClickText=()=>{
  emit('onClickText')
}
</script>

<template>
  <el-card class="box-card" shadow="never" :body-style="{padding: props.isPadding===false?'0px 0px':'20px 20px'}">
    <template #header>
      <div class="card-header">
        <span>{{ props.header }}</span>
        <el-link :underline="false" type="primary" @click="onClickText">{{ props.text }}</el-link>
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

::v-deep(.VisitsLineChart  .el-card__body){
  padding: 0 20px;
}
 .box-card-info {
  width: 100%;
  height: v-bind(height);
  padding-bottom: 10px;
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

</style>
