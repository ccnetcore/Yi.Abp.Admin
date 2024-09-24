<script setup lang="tsx">
import { ref } from "vue";
import "vue-json-pretty/lib/styles.css";
import VueJsonPretty from "vue-json-pretty";

const props = defineProps({
  data: {
    type: Array,
    default: () => []
  }
});

const columns = [
  {
    label: "IP 地址",
    prop: "operIp"
  },
  {
    label: "地点",
    prop: "operLocation"
  },
  {
    label: "所属模块",
    prop: "title"
  },
  {
    label: "请求时间",
    prop: "creationTime"
  },
  {
    label: "请求方法",
    prop: "requestMethod"
  },
  {
    label: "请求接口",
    prop: "method",
    copy: true
  }
];

const dataList = ref([
  {
    title: "响应体",
    name: "requestResult",
    data:
      (props.data[0] as any).requestResult == null
        ? ""
        : JSON.parse((props.data[0] as any).requestResult)
  },
  {
    title: "请求体",
    name: "requestParam",
    data:
      (props.data[0] as any).requestParam == null
        ? ""
        : JSON.parse((props.data[0] as any).requestParam)
  }
]);
</script>

<template>
  <div>
    <el-scrollbar>
      <PureDescriptions border :data="data" :columns="columns" :column="5" />
    </el-scrollbar>
    <el-tabs :modelValue="'requestResult'" type="border-card" class="mt-4">
      <el-tab-pane
        v-for="(item, index) in dataList"
        :key="index"
        :name="item.name"
        :label="item.title"
      >
        <el-scrollbar max-height="calc(100vh - 240px)">
          <vue-json-pretty v-model:data="item.data" />
        </el-scrollbar>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
