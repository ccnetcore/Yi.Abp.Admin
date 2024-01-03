<template>
      <el-select
      style="width: 600px;"
        v-model="value"
        multiple
        filterable
        remote
        reserve-keyword
        placeholder="请输入用户账号（可多选）"
        remote-show-suffix
        :remote-method="remoteMethod"
        :loading="loading"
      >
        <el-option
          v-for="item in options"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        />
      </el-select>
</template>

<script  setup>
import { onMounted, ref,computed  } from 'vue'
import {listUser} from '@/apis/userApi'
const props = defineProps(['modelValue'])
const emit = defineEmits(['update:modelValue'])


//这个为可选择的列表，{value,label},value为用户id，label为账号名称（不可重复）
const options = ref([])

const value = computed({
  get() {
    return props.modelValue
  },
  set(value) {
    emit('update:modelValue', value)
  }
})


const loading = ref(false)

onMounted( async()=>{

 const response= await  listUser({ids:value.value.join()});
 const res=response.data.items;
   //下拉列表
   options.value = res
  .map((item) => {
    return { value: `${item.id}`, label: `用户:${item.userName}` }
  })
})


const loadUser=async(query)=>{
  const response= await listUser({userName:query});
  const res=response.data.items;
  //下拉列表
  options.value = res
  .map((item) => {
    return { value: `${item.id}`, label: `用户:${item.userName}` }
  })
}

const remoteMethod =async (query) => {
  if (query) {
    loading.value = true
   await loadUser(query);
   loading.value = false
  } else {
    options.value = []
  }
}

const states = [
  'Alabama',
  'Alaska',
  'Arizona',
  'Arkansas',
  'California',
  'Colorado',
  'Connecticut',
  'Delaware',
  'Florida',
  'Georgia',
  'Hawaii',
  'Idaho',
  'Illinois',
  'Indiana',
  'Iowa',
  'Kansas',
  'Kentucky',
  'Louisiana',
  'Maine',
  'Maryland',
  'Massachusetts',
  'Michigan',
  'Minnesota',
  'Mississippi',
  'Missouri',
  'Montana',
  'Nebraska',
  'Nevada',
  'New Hampshire',
  'New Jersey',
  'New Mexico',
  'New York',
  'North Carolina',
  'North Dakota',
  'Ohio',
  'Oklahoma',
  'Oregon',
  'Pennsylvania',
  'Rhode Island',
  'South Carolina',
  'South Dakota',
  'Tennessee',
  'Texas',
  'Utah',
  'Vermont',
  'Virginia',
  'Washington',
  'West Virginia',
  'Wisconsin',
  'Wyoming',
]
</script>