<template>
  <div class="tabs">
    <div class="left">
      <div
        class="items"
        v-for="item in leftList"
        @click="handleClick(item)"
        :class="[modelValue === item.name ? 'active-tab' : '']"
      >
        {{ item.label }}
      </div>
    </div>
    <div class="center"></div>
    <div class="right">
      <template v-for="(item, index) in rightList">
        <div
          class="items"
          @click="handleClick(item)"
          :class="[modelValue === item.name ? 'active-tab' : '']"
        >
          {{ item.label }}
        </div>
        <div class="line" v-if="index < rightList.length - 1">|</div>
      </template>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  modelValue: {
    type: String,
    default: "",
  },
  tabList: {
    type: Array,
    default: () => [],
  },
});

const emit = defineEmits(["update:modelValue"], ["tab-change"]);

const leftList = computed(() =>
  props.tabList.filter((item) => item.position === "left")
);
const rightList = computed(() =>
  props.tabList.filter((item) => item.position === "right")
);
const handleClick = (item) => {
  emit("update:modelValue", item.name);
  emit("tab-change", item);
};
</script>

<style scoped lang="scss">
.tabs {
  display: flex;
  background-color: #fff;
  padding: 1rem;
  margin: 1rem 0rem;
  color: #8c8c8c;
  .left {
    width: 100px;
  }
  .center {
    flex: 3;
  }
  .right {
    width: 200px;
    display: flex;
    .line {
      margin: 0 10px;
    }
  }
  .items {
    cursor: pointer;
  }
}
.active-tab {
  color: #409eff;
}
</style>
