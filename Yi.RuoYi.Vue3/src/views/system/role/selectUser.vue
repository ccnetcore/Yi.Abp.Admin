<template>
   <!-- 授权用户 -->
   <el-dialog title="选择用户" v-model="visible" width="800px" top="5vh" append-to-body>
      <el-form :model="queryParams" ref="queryRef" :inline="true">
         <el-form-item label="用户名称" prop="userName">
            <el-input
               v-model="queryParams.userName"
               placeholder="请输入用户名称"
               clearable
               @keyup.enter="handleQuery"
            />
         </el-form-item>
         <el-form-item label="手机号码" prop="phone">
            <el-input
               v-model="queryParams.phone"
               placeholder="请输入手机号码"
               clearable
               @keyup.enter="handleQuery"
            />
         </el-form-item>
         <el-form-item>
            <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
            <el-button icon="Refresh" @click="resetQuery">重置</el-button>
         </el-form-item>
      </el-form>
      <el-row>
         <el-table @row-click="clickRow" ref="refTable" :data="userList" @selection-change="handleSelectionChange" height="260px">
            <el-table-column type="selection" width="55"></el-table-column>
            <el-table-column label="用户名称" prop="userName" :show-overflow-tooltip="true" />
            <el-table-column label="用户昵称" prop="nick" :show-overflow-tooltip="true" />
            <el-table-column label="邮箱" prop="email" :show-overflow-tooltip="true" />
            <el-table-column label="手机" prop="phone" :show-overflow-tooltip="true" />
            <el-table-column label="状态" align="center" prop="state">
               <template #default="scope">
                  <dict-tag :options="sys_normal_disable" :value="scope.row.state" />
               </template>
            </el-table-column>
            <el-table-column label="创建时间" align="center" prop="creationTime" width="180">
               <template #default="scope">
                  <span>{{ parseTime(scope.row.creationTime) }}</span>
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
      </el-row>
      <template #footer>
         <div class="dialog-footer">
            <el-button type="primary" @click="handleSelectUser">确 定</el-button>
            <el-button @click="visible = false">取 消</el-button>
         </div>
      </template>
   </el-dialog>
</template>

<script setup name="SelectUser">
import { authUserSelectAll, unallocatedUserList } from "@/api/system/role";

const props = defineProps({
  roleId: {
    type: [Number, String]
  }
});

const { proxy } = getCurrentInstance();
const { sys_normal_disable } = proxy.useDict("sys_normal_disable");

const userList = ref([]);
const visible = ref(false);
const total = ref(0);
const userIds = ref([]);

const queryParams = reactive({
  skipCount: 1,
  maxResultCount: 10,
  roleId: undefined,
  userName: undefined,
  phone: undefined
});

// 显示弹框
function show() {
  queryParams.roleId = props.roleId;
  getList();
  visible.value = true;
}
/**选择行 */
function clickRow(row) {
  proxy.$refs["refTable"].toggleRowSelection(row);
}
// 多选框选中数据
function handleSelectionChange(selection) {
  userIds.value = selection.map(item => item.id);
}
// 查询表数据
function getList() {
  unallocatedUserList(queryParams.roleId,queryParams).then(res => {
    userList.value = res.data.items;
    total.value = res.data.total;
  });
}
/** 搜索按钮操作 */
function handleQuery() {
  queryParams.skipCount = 1;
  getList();
}
/** 重置按钮操作 */
function resetQuery() {
  proxy.resetForm("queryRef");
  handleQuery();
}
const emit = defineEmits(["ok"]);
/** 选择授权用户操作 */
function handleSelectUser() {
  const roleId = queryParams.roleId;

  const uIds = userIds.value;
  if (uIds == []) {
    proxy.$modal.msgError("请选择要分配的用户");
    return;
  }
  
  authUserSelectAll({ roleId: roleId, userIds:uIds }).then(res => {
    if (res.statusCode == 200) {
      proxy.$modal.msgSuccess("成功");
      visible.value = false;
      emit("ok");
    }
  });
}

defineExpose({
  show,
});
</script>
