<template>
  <div class="app-container">
    <el-form
      :model="queryParams"
      ref="queryRef"
      :inline="true"
      v-show="showSearch"
      label-width="68px"
    >

       <el-form-item label="当前选择表："  label-width="120px" >
      
      <el-input v-if="props.table.name!=null" v-model="props.table.name" disabled />
     <span  v-else>无</span>
    </el-form-item>

      <el-form-item label="字段名称" prop="name">
        <el-input
          v-model="queryParams.name"
          placeholder="请输入字段名称"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery"
          >搜索</el-button
        >
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button
          type="primary"
          plain
          icon="Plus"
          @click="handleAdd"
          v-hasPermi="['system:config:add']"
          >新增</el-button
        >
        <!-- :disabled="props.table.name==null" -->
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="success"
          plain
          icon="Edit"
          :disabled="single"
          @click="handleUpdate"
          v-hasPermi="['system:config:edit']"
          >修改</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="danger"
          plain
          icon="Delete"
          :disabled="multiple"
          @click="handleDelete"
          v-hasPermi="['system:config:remove']"
          >删除</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="warning"
          plain
          icon="Download"
          @click="handleExport"
          v-hasPermi="['system:config:export']"
          >导出</el-button
        >
      </el-col>

      <right-toolbar
        v-model:showSearch="showSearch"
        @queryTable="getList"
      ></right-toolbar>
    </el-row>

    <el-table
      v-loading="loading"
      :data="dataList"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="名称" align="center" prop="name" />
      <el-table-column label="类型" align="center" prop="fieldType" >
    <template #default="scope">
  <el-tag>{{getFieldTypeEnum(scope.row.fieldType)}}</el-tag>
    </template>
          </el-table-column>

      <el-table-column label="长度" align="center" prop="length" >
        <template #default="scope">
           {{ scope.row.length==0?'-':scope.row.length }}
           </template>
      </el-table-column>

      <el-table-column label="是否必填" align="center" prop="isRequired" >
           <template #default="scope">
                  <el-switch disabled  v-model="scope.row.isRequired" />
           </template>
      </el-table-column>

      <el-table-column label="是否主键" align="center" prop="isKey" >
            <template #default="scope">
                  <el-switch disabled  v-model="scope.row.isKey" />
           </template>
                 </el-table-column>
      <el-table-column label="是否自增" align="center" prop="isAutoAdd" >
          <template #default="scope">
                  <el-switch disabled  v-model="scope.row.isAutoAdd" />
           </template>
                 </el-table-column>

      <el-table-column
        label="描述"
        align="center"
        prop="Description"
        :show-overflow-tooltip="true"
      />
      <el-table-column
        label="操作"
        align="center"
        width="150"
        class-name="small-padding fixed-width"
      >
        <template #default="scope">
          <el-button
            link
            icon="Edit"
            @click="handleUpdate(scope.row)"
            v-hasPermi="['system:config:edit']"
            >修改</el-button
          >
          <el-button
            link
            icon="Delete"
            @click="handleDelete(scope.row)"
            v-hasPermi="['system:config:remove']"
            >删除</el-button
          >
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="Number(total)"
      v-model:page="queryParams.skipCount"
      v-model:limit="queryParams.maxResultCount"
      @pagination="getList"
    />

    <!-- 添加或修改字段配置对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="dataRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入字段名称" />
        </el-form-item>
        <el-form-item label="类型" prop="fieldType">
          <el-select
            v-model="form.fieldType"
            class="m-2"
            placeholder="请选择一个类型"
            size="large"
          >
            <el-option
              v-for="item in fieldList"
              :key="item.value"
              :label="item.lable"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="长度" prop="length">
          <el-input v-model="form.length" placeholder="请输入字段长度" />
        </el-form-item>
        <el-form-item label="是否必须" prop="isRequired">
          <el-switch v-model="form.isRequired" />
        </el-form-item>

        <el-form-item label="是否主键" prop="isKey">
          <el-switch v-model="form.isKey" />
        </el-form-item>

        <el-form-item label="是否自增" prop="isAutoAdd">
          <el-switch v-model="form.isAutoAdd" />
        </el-form-item>

         <el-form-item label="字段排序" prop="orderNum">
            <el-input-number v-model="form.orderNum" />
        </el-form-item>


        <el-form-item label="字段描述" prop="description">
          <el-input
            v-model="form.description"
            type="textarea"
            placeholder="请输入字段描述"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import {
  listData,
  getData,
  addData,
  updateData,
  delData,
  getType
} from "@/api/code/fieldApi";
import { computed, onMounted, defineProps, ref } from "vue";

const { proxy } = getCurrentInstance();
const { sys_yes_no } = proxy.useDict("sys_yes_no");
const props = defineProps(["table"]);

const tableOptions = computed(() => props.table);

const dataList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const dateRange = ref([]);
const fieldList=ref([]);
const data = reactive({
  form: {
    tableId: props.table.id,
    orderNum:0
  },
  queryParams: {
    skipCount: 1,
    maxResultCount: 10,
    name: undefined,
    // props.table.id
  },
  rules: {
    name: [{ required: true, message: "字段名称不能为空", trigger: "blur" }],
    fieldType: [
      { required: true, message: "字段类型不能为空", trigger: "blur" },
    ],
    //  configValue: [{ required: true, message: "字段键值不能为空", trigger: "blur" }]
  },
});

const { queryParams, form, rules } = toRefs(data);

const getFieldTypeEnum=(field)=>{
  return field;
  // return fieldList.value.filter(x=>x.value==field)[0]?.lable??'未知';
}

watch(
  () => props,
  (val) => {
    getList();
  },
  { deep: true }
);

function getTypeList()
{
getType().then((response)=>{
fieldList.value=response.data;
});
}

/** 查询字段列表 */
function getList() {
  loading.value = true;
  listData({
    ...proxy.addDateRange(queryParams.value, dateRange.value),
    tableId: props.table.id,
  }).then((response) => {
    dataList.value = response.data.items;
    total.value = response.data.totalCount;
    loading.value = false;
  });
}
/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}
/** 表单重置 */
function reset() {
  form.value = {
    id: undefined,
    name: undefined,
    description: undefined,
    orderNum: undefined,
    length: undefined,
    fieldType: undefined,
    isRequired: false,
    isKey: false,
    isAutoAdd: false,
    tableId: props.table.id,
    orderNum:0
  };
  proxy.resetForm("dataRef");
}
/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.skipCount = 1;
  getList();
}
/** 重置按钮操作 */
function resetQuery() {
  dateRange.value = [];
  proxy.resetForm("queryRef");
  handleQuery();
}
/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.id);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}
/** 新增按钮操作 */
function handleAdd() {
   if(props.table.id==undefined)
   {
      proxy.$modal.msgWarning('请先选择一张表后再添加字段');
      return;
   }
  reset();
  open.value = true;
  title.value = "添加字段";
}
/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const configId = row.id || ids.value;
  getData(configId).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = "修改字段";
  });
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs["dataRef"].validate((valid) => {
    if (valid) {
      if (form.value.id != undefined) {
        updateData(form.value).then((response) => {
          proxy.$modal.msgSuccess("修改成功");
          open.value = false;
          getList();
        });
      } else {
        addData(form.value).then((response) => {
          proxy.$modal.msgSuccess("新增成功");
          open.value = false;
          getList();
        });
      }
    }
  });
}
/** 删除按钮操作 */
function handleDelete(row) {
  const configIds = row.id || ids.value;
  proxy.$modal
    .confirm('是否确认删除字段编号为"' + configIds + '"的数据项？')
    .then(function () {
      return delData(configIds);
    })
    .then(() => {
      getList();
      proxy.$modal.msgSuccess("删除成功");
    })
    .catch(() => {});
}
/** 导出按钮操作 */
function handleExport() {
  proxy.download(
    "system/config/export",
    {
      ...queryParams.value,
    },
    `config_${new Date().getTime()}.xlsx`
  );
}
/** 刷新缓存按钮操作 */
function handleRefreshCache() {
  refreshCache().then(() => {
    proxy.$modal.msgSuccess("刷新缓存成功");
  });
}
getTypeList();
getList();
</script>
