<template>
   <div class="app-container">
      <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="68px">
         <el-form-item label="公告标题" prop="title">
            <el-input
               v-model="queryParams.title"
               placeholder="请输入公告标题"
               clearable
               @keyup.enter="handleQuery"
            />
         </el-form-item>
         <el-form-item label="类型" prop="type">
            <el-select v-model="queryParams.type" placeholder="公告类型" clearable>
               <el-option
                  v-for="dict in sys_notice_type"
                  :key="dict.value"
                  :label="dict.label"
                  :value="dict.value"
               />
            </el-select>
         </el-form-item>
         <el-form-item>
            <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
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
               v-hasPermi="['system:notice:add']"
            >新增</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button
               type="success"
               plain
               icon="Edit"
               :disabled="single"
               @click="handleUpdate"
               v-hasPermi="['system:notice:edit']"
            >修改</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button
               type="danger"
               plain
               icon="Delete"
               :disabled="multiple"
               @click="handleDelete"
               v-hasPermi="['system:notice:remove']"
            >删除</el-button>
         </el-col>
         <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="noticeList" @selection-change="handleSelectionChange">
         <el-table-column type="selection" width="55" align="center" />
         <el-table-column label="序号" align="center" prop="id" width="300" />
         <el-table-column
            label="公告标题"
            align="center"
            prop="title"
            :show-overflow-tooltip="true"
         />
         <el-table-column label="公告类型" align="center" prop="type" width="100">
            <template #default="scope">
               <dict-tag :options="sys_notice_type" :value="scope.row.type" />
            </template>
         </el-table-column>
         <el-table-column label="状态" align="center" prop="state" width="100">
            <template #default="scope">
               <el-tag type="info">{{sys_notice_state.filter(x=>x.value==scope.row.state)[0]?.label}}</el-tag>
            </template>
         </el-table-column>
         <el-table-column label="创建时间" align="center" prop="creationTime" width="100">
            <template #default="scope">
               <span>{{ parseTime(scope.row.creationTime, '{y}-{m}-{d}') }}</span>
            </template>
         </el-table-column>
         <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template #default="scope">
                            
               <el-button
                  link
                  icon="Promotion"
                  @click="handleOnlineSend(scope.row.id)"
                  v-hasPermi="['system:notice:edit']"
               >在线发送</el-button>

             
               <el-button
                  link
                  icon="Promotion"
                  @click="handleOfflineSend(scope.row.id)"
                  v-hasPermi="['system:notice:edit']"
               >离线发送</el-button>


               <el-button
                  link
                  icon="Edit"
                  @click="handleUpdate(scope.row)"
                  v-hasPermi="['system:notice:edit']"
               >修改</el-button>
               <el-button
                  link
                  icon="Delete"
                  @click="handleDelete(scope.row)"
                  v-hasPermi="['system:notice:remove']"
               >删除</el-button>
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

      <!-- 添加或修改公告对话框 -->
      <el-dialog :title="title" v-model="open" width="780px" append-to-body>
         <el-form ref="noticeRef" :model="form" :rules="rules" label-width="80px">
            <el-row>
               <el-col :span="12">
                  <el-form-item label="公告标题" prop="title">
                     <el-input v-model="form.title" placeholder="请输入公告标题" />
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="公告类型" prop="type">
                     <el-select v-model="form.type" placeholder="请选择">
                        <el-option
                           v-for="dict in sys_notice_type"
                           :key="dict.value"
                           :label="dict.label"
                           :value="dict.value"
                        ></el-option>
                     </el-select>
                  </el-form-item>
               </el-col>
               <el-col :span="24">
                  <el-form-item label="状态">
                     <el-radio-group v-model="form.state">
                        <el-radio
                           v-for="dict in sys_notice_state"
                           :key="dict.value"
                           :value="dict.value"
                        >{{ dict.label }}</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
               <el-col :span="24">
                  <el-form-item label="内容">
                     <el-input
                        :rows="6"
                        type="textarea"
                        placeholder="请输入内容"
                        v-model="form.content"
                     />
                  </el-form-item>
               </el-col>
            </el-row>
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

<script setup name="Notice">
import { sendOnlineNotice,sendOfflineNotice,listNotice, getNotice, delNotice, addNotice, updateNotice } from "@/api/system/notice";

const { proxy } = getCurrentInstance();
const sys_notice_state=[
{label:'启用',value:true},
{label:'停用',value:false}
];
const sys_notice_type=[
{label:'走马灯',value:'MerryGoRound'},
{label:'提示弹窗',value:'Popup'}
];
const noticeList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const data = reactive({
  form: {},
  queryParams: {
    skipCount: 1,
    maxResultCount: 10,
    title: undefined,
    createBy: undefined,
    state: undefined
  },
  rules: {
    title: [{ required: true, message: "公告标题不能为空", trigger: "blur" }],
    type: [{ required: true, message: "公告类型不能为空", trigger: "change" }]
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询公告列表 */
function getList() {
  loading.value = true;
  listNotice(queryParams.value).then(response => {
    noticeList.value = response.data.items;
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
    title: undefined,
    type: undefined,
    content: undefined,
    state: true
  };
  proxy.resetForm("noticeRef");
}
/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.skipCount = 1;
  getList();
}
/** 重置按钮操作 */
function resetQuery() {
  proxy.resetForm("queryRef");
  handleQuery();
}
/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map(item => item.id);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}
/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加公告";
}
/**修改按钮操作 */
function handleUpdate(row) {
  reset();
  const id = row.id || ids.value;
  getNotice(id).then(response => {
    form.value = response.data;
    open.value = true;
    title.value = "修改公告";
  });
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs["noticeRef"].validate(valid => {
    if (valid) {
      if (form.value.id != undefined) {
        updateNotice(form.value.id,form.value).then(response => {
          proxy.$modal.msgSuccess("修改成功");
          open.value = false;
          getList();
        });

      } else {
        addNotice(form.value).then(response => {
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
  const ids2 = row.id || ids.value
  proxy.$modal.confirm('是否确认删除公告编号为"' + ids2 + '"的数据项？').then(function() {
    return delNotice(ids2);
  }).then(() => {
    getList();
    proxy.$modal.msgSuccess("删除成功");
  }).catch(() => {});
}

const handleOnlineSend=async (id)=>{
  await sendOnlineNotice(id);
  proxy.$modal.msgSuccess("在线消息发送成功");

}
const handleOfflineSend=async (id)=>{
   await sendOfflineNotice(id);
   proxy.$modal.msgSuccess("离线消息发送成功");
}
getList();
</script>
