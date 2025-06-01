<template>
  <!-- 动态数据下拉选择框 -->
  <el-select v-model="value" :value-key="servicekey" filterable remote clearable :placeholder="placeholder"
    :loading="loading" :remote-method="remoteMethod" @change="handleChange"
    @clear="handleClear">
    <el-option v-for="item in options" :key="item[servicekey]" :label="item[servicelabel]" :value="item" />
  </el-select>
</template>

<script setup name="SelectDataTag">
import { ref } from 'vue';
import request from '@/utils/request.js'

const props = defineProps({
  placeholder: {
    type: String,
    default: '请输入关键字',
  },
  /** 动态服务名称 */
  servicename: {
    type: String,
    required: true,
  },
  /** 指定响应数据的key */
  servicekey: {
    type: String,
    required: true,
  },
  /** 指定响应数据的label */
  servicelabel: {
    type: String,
    required: true,
  },
  /** 记录表格渲染行索引 */
  index: {
    type: [String, Number],
    default: 0,
  }
});

const emits = defineEmits(["change", "clear"]);

const list = ref([]);
const options = ref([]);
const value = ref([]);
const loading = ref(false);

function getSelectDataList(query) {
  return request({
    url: '/' + props.servicename + '/select-data-list?keywords=' + query,
    method: 'get',
  });
}

function remoteMethod(query) {
  options.value = [];
  if (query) {
    loading.value = true;

    setTimeout(() => {
      getSelectDataList(query).then(
        (response) => {
          list.value = response.data.items;
          options.value = list.value;

          loading.value = false;
        }
      );
    }, 300);
  }
}

function handleChange(data) {
  emits("change", data || [], props.index);
}

function handleClear() {
  emits("clear");
}

</script>
