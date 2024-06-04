<template>
  <div class="app-container">
    <el-form
      :model="queryParams"
      ref="queryRef"
      :inline="true"
      v-show="showSearch"
      label-width="100px"
    >
      <el-form-item label="模板名称" prop="name">
        <el-input
          v-model="queryParams.name"
          placeholder="请输入模板名称"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
          prop="name"
        />
      </el-form-item>
      <!-- <el-form-item label="模板编号" prop="code">
                <el-input v-model="queryParams.code" placeholder="请输入模板编号" clearable style="width: 240px"
                    @keyup.enter="handleQuery" prop="code" />
            </el-form-item> -->
      <!-- <el-form-item label="创建时间" style="width: 308px">
            <el-date-picker
              v-model="dateRange"
              value-format="YYYY-MM-DD"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item> -->
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
          v-hasPermi="['codeGen:template:add']"
          >新增</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="success"
          plain
          icon="Edit"
          :disabled="single"
          @click="handleUpdate"
          v-hasPermi="['codeGen:template:edit']"
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
          v-hasPermi="['codeGen:template:remove']"
          >删除</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="warning"
          plain
          icon="Download"
          @click="handleExport"
          v-hasPermi="['codeGen:template:export']"
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

      <!-----------------------这里开始就是数据表单的全部列------------------------>

      <el-table-column
        label="模板名称"
        align="center"
        prop="name"
        :show-overflow-tooltip="true"
      />
      <el-table-column
        label="生成路径"
        align="center"
        prop="buildPath"
        :show-overflow-tooltip="true"
      />

      <!-- <el-table-column label="备注" align="center" prop="remarks" :show-overflow-tooltip="true" /> -->
      <!-- <el-table-column label="状态" align="center" prop="isDeleted">
            <template #default="scope">
              <dict-tag
                :options="sys_normal_disable"
                :value="scope.row.isDeleted"
              />
            </template>
          </el-table-column> -->
      <el-table-column
        label="备注"
        align="center"
        prop="remarks"
        :show-overflow-tooltip="true"
      />
      <!-- <el-table-column
            label="创建时间"
            align="center"
            prop="createTime"
            width="180"
          >
            <template #default="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column> -->
      <el-table-column
        label="操作"
        align="center"
        class-name="small-padding fixed-width"
      >
        <template #default="scope">
          <el-button
            link
            icon="Edit"
            @click="handleUpdate(scope.row)"
            v-hasPermi="['codeGen:template:edit']"
            >修改</el-button
          >
          <el-button
            link
            icon="Delete"
            @click="handleDelete(scope.row)"
            v-hasPermi="['codeGen:template:remove']"
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

    <!-- ---------------------这里是新增和更新的对话框--------------------- -->
    <el-dialog :title="title" v-model="open" width="1200px" append-to-body>
      <el-form ref="dataRef" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="模板名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入模板名称" />
        </el-form-item>

        <el-form-item label="构建路径" prop="buildPath">
          
          <el-input v-model="form.buildPath" placeholder="请输入构建路径" />
          <el-button type="primary" @click="openDir(form.buildPath)">打开目录</el-button>
        </el-form-item>

        <!-- <el-form-item label="状态" prop="isDeleted">
              <el-radio-group v-model="form.isDeleted">
                <el-radio
                  v-for="dict in sys_normal_disable"
                  :key="dict.value"
                  :label="JSON.parse(dict.value)"
                  >{{ dict.label }}</el-radio
                >
              </el-radio-group>
            </el-form-item> -->

            <TempalteTip/>
        <el-form-item label="模板内容" prop="templateStr">
          <el-input
            v-model="form.templateStr"
            type="textarea"
            :rows="30"
            placeholder="请输入模板内容"
          ></el-input>
       
        </el-form-item>
        
        <ReplaceText style="margin-bottom: 15px;" :text="form.templateStr" @handleText='hanldeReplaceText'></ReplaceText>



        <el-form-item label="备注" prop="remarks">
          <el-input
            v-model="form.remarks"
            type="textarea"
            placeholder="请输入内容"
          ></el-input>
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
  delData,
  addData,
  updateData,
} from "@/api/code/templateApi";
import {openPath} from "@/api/code/codeGenApi";
import { ref } from "@vue/reactivity";
import ReplaceText from './components/ReplaceText'
import TempalteTip from './components/TempalteTip.vue'
const { proxy } = getCurrentInstance();
const { sys_normal_disable } = proxy.useDict("sys_normal_disable");

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
const data = reactive({
  form: {},
  queryParams: {
    skipCount: 1,
    maxResultCount: 10,
    name: undefined,
  },
  rules: {
    name: [{ required: true, message: "模板名称不能为空", trigger: "blur" }],
    buildPath: [
      { required: true, message: "构建路径不能为空", trigger: "blur" },
    ],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询列表 */
function getList() {
  loading.value = true;
  listData(proxy.addDateRange(queryParams.value, dateRange.value)).then(
    (response) => {
      dataList.value = response.data.items;
      total.value = response.data.totalCount;
      loading.value = false;
    }
  );
}
/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 表单重置 */
function reset() {
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
/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加模板";
}
/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.id);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}
/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const id = row.id || ids.value;
  getData(id).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = "修改模板";
  });
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs["dataRef"].validate((valid) => {
    if (valid) {
      if (form.value.id != undefined) {
        updateData(form.value.id, form.value).then((response) => {
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
  const delIds = row.id || ids.value;
  proxy.$modal
    .confirm('是否确认删除编号为"' + delIds + '"的数据项？')
    .then(function () {
      return delData(delIds);
    })
    .then(() => {
      getList();
      proxy.$modal.msgSuccess("删除成功");
    })
    .catch(() => {});
}
/** 导出按钮操作 */
function handleExport() {}


/** 处理字符串替换 */
function hanldeReplaceText(text)
{
    form.value.templateStr=text;
}
getList();

/** 打开目录 */
async function openDir(path)
{
  const response= await openPath(path);
  if(response.statusCode==200)
  {
    proxy.$modal.msgSuccess("目录打开成功");
  }
}
</script>
