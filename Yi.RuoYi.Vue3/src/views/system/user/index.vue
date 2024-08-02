<template>
   <div class="app-container">
      <el-row :gutter="20">
         <!--部门数据-->
         <el-col :span="4" :xs="24">
            <div class="head-container">
               <el-input v-model="deptName" placeholder="请输入部门名称" clearable prefix-icon="Search"
                  style="margin-bottom: 20px" />
            </div>
            <div class="head-container">
               <el-tree :data="deptOptions" :props="{ label: 'label', children: 'children' }"
                  :expand-on-click-node="false" :filter-node-method="filterNode" ref="deptTreeRef" highlight-current
                  default-expand-all @node-click="handleNodeClick" />
            </div>
         </el-col>
         <!--用户数据-->
         <el-col :span="20" :xs="24">
            <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="68px">
               <el-form-item label="用户名称" prop="userName">
                  <el-input v-model="queryParams.userName" placeholder="请输入用户名称" clearable style="width: 240px"
                     @keyup.enter="handleQuery" />
               </el-form-item>
               <el-form-item label="手机号码" prop="phone">
                  <el-input v-model="queryParams.phone" placeholder="请输入手机号码" clearable style="width: 240px"
                     @keyup.enter="handleQuery" />
               </el-form-item>
               <el-form-item label="状态" prop="state">
                  <el-select v-model="queryParams.state" placeholder="用户状态" clearable style="width: 240px">
                     <el-option v-for="dict in sys_normal_disable" :key="dict.value" :label="dict.label"
                        :value="JSON.parse( dict.value)" />
                  </el-select>
               </el-form-item>
               <el-form-item label="创建时间" style="width: 308px;">
                  <el-date-picker v-model="dateRange" value-format="YYYY-MM-DD" type="daterange" range-separator="-"
                     start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
               </el-form-item>
               <el-form-item>
                  <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
                  <el-button icon="Refresh" @click="resetQuery">重置</el-button>
               </el-form-item>
            </el-form>

            <el-row :gutter="10" class="mb8">
               <el-col :span="1.5">
                  <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['system:user:add']">新增
                  </el-button>
               </el-col>
               <el-col :span="1.5">
                  <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate"
                     v-hasPermi="['system:user:edit']">修改</el-button>
               </el-col>
               <el-col :span="1.5">
                  <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete"
                     v-hasPermi="['system:user:remove']">删除</el-button>
               </el-col>
               <el-col :span="1.5">
                  <el-button type="info" plain icon="Upload" @click="handleImport" v-hasPermi="['system:user:import']">
                     导入</el-button>
               </el-col>
               <el-col :span="1.5">
                  <el-button type="warning" plain icon="Download" @click="handleExport"
                     v-hasPermi="['system:user:export']">导出</el-button>
               </el-col>
               <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="userList" @selection-change="handleSelectionChange">
               <el-table-column type="selection" width="50" align="center" />
               <el-table-column label="用户编号" align="center" key="id" prop="id" v-if="columns[0].visible" />
               <el-table-column label="用户账号" align="center" key="userName" prop="userName" v-if="columns[1].visible"
                  :show-overflow-tooltip="true" />
               <el-table-column label="用户昵称" align="center" key="nick" prop="nick" v-if="columns[2].visible"
                  :show-overflow-tooltip="true" />
               <el-table-column label="部门" align="center" key="deptName" prop="deptName" v-if="columns[3].visible"
                  :show-overflow-tooltip="true" />
               <el-table-column label="手机号码" align="center" key="phone" prop="phone" v-if="columns[4].visible"
                />
               <el-table-column label="状态" align="state" key="state" v-if="columns[5].visible">
                  <template #default="scope">
                     <el-switch v-model="scope.row.state" :active-value=true :inactive-value=false
                        @change="handleStatusChange(scope.row)"></el-switch>
                  </template>
               </el-table-column>
               <el-table-column label="创建时间" align="center" prop="creationTime" v-if="columns[6].visible" width="160">
                  <template #default="scope">
                     <span>{{ parseTime(scope.row.creationTime) }}</span>
                  </template>
               </el-table-column>
               <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
                  <template #default="scope">
                     <el-tooltip content="修改" placement="top" v-if="scope.row.userName != 'cc'">
                        <el-button link icon="Edit" @click="handleUpdate(scope.row)"
                           v-hasPermi="['system:user:edit']"></el-button>
                     </el-tooltip>
                     <el-tooltip content="删除" placement="top" v-if="scope.row.userName != 'cc'">
                        <el-button link icon="Delete" @click="handleDelete(scope.row)"
                           v-hasPermi="['system:user:remove']"></el-button>
                     </el-tooltip>
                     <el-tooltip content="重置密码" placement="top" v-if="scope.row.userName != 'cc'">
                        <el-button link icon="Key" @click="handleResetPwd(scope.row)"
                           v-hasPermi="['system:user:resetPwd']"></el-button>
                     </el-tooltip>
                     <!-- <el-tooltip content="分配角色" placement="top" v-if="scope.row.userName != 'cc'">
                        <el-button link icon="CircleCheck" @click="handleAuthRole(scope.row)"
                           v-hasPermi="['system:user:edit']"></el-button>
                     </el-tooltip> -->
                  </template>
               </el-table-column>
            </el-table>
            <pagination v-show="total > 0"          :total="Number(total)" v-model:page="queryParams.skipCount"
               v-model:limit="queryParams.maxResultCount" @pagination="getList" />
         </el-col>
      </el-row>

      <!-- 添加或修改用户配置对话框 -->
      <el-dialog :title="title" v-model="open" width="600px" append-to-body>
         <el-form :model="form" :rules="rules" ref="userRef" label-width="80px">
            <el-row>
               <el-col :span="12">
                  <el-form-item label="用户昵称" prop="nick">
                     <el-input v-model="form.nick" placeholder="请输入用户昵称" maxlength="30" />
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="归属部门" prop="deptId">
                     <el-tree-select v-model="form.deptId" :data="deptOptions"
                        :props="{ value: 'id', label: 'label', children: 'children' }" value-key="id"
                        placeholder="请选择归属部门" check-strictly />
                  </el-form-item>
               </el-col>
            </el-row>
            <el-row>
               <el-col :span="12">
                  <el-form-item label="手机号码" prop="phone">
                     <el-input v-model="form.phone" placeholder="请输入手机号码" maxlength="11" />
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="邮箱" prop="email">
                     <el-input v-model="form.email" placeholder="请输入邮箱" maxlength="50" />
                  </el-form-item>
               </el-col>
            </el-row>
            <el-row>
               <el-col :span="12">
                  <el-form-item v-if="form.id == undefined" label="用户账号" prop="userName">
                     <el-input v-model="form.userName" placeholder="请输入用户账号" maxlength="30" />
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item v-if="form.id == undefined" label="用户密码" prop="password">
                     <el-input v-model="form.password" placeholder="请输入用户密码" type="password" maxlength="20"
                        show-password />
                  </el-form-item>
               </el-col>
            </el-row>
            <el-row>
               <el-col :span="12">
                  <el-form-item label="用户性别">
                     <el-select v-model="form.sex" placeholder="请选择">
                        <el-option v-for="dict in sys_user_sex" :key="dict.value" :label="dict.label" :value="dict.value "></el-option>
                     </el-select>
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="状态">
                     <el-radio-group v-model="form.state">
                        <el-radio v-for="dict in sys_normal_disable" :key="dict.value" :value="JSON.parse(dict.value)">{{dict.label}}</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
            </el-row>
            <el-row>
               <el-col :span="12">
                  <el-form-item label="岗位">
                     <el-select v-model="form.postIds" multiple placeholder="请选择">
                        <el-option v-for="item in postOptions" :key="item.id" :label="item.postName"
                           :value="item.id" :disabled="item.state == false"></el-option>
                     </el-select>
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="角色">
                     <el-select v-model="form.roleIds" multiple placeholder="请选择">
                        <el-option v-for="item in roleOptions" :key="item.id" :label="item.roleName" :value="item.id"
                           :disabled="item.state ==false"></el-option>
                     </el-select>
                  </el-form-item>
               </el-col>
            </el-row>
            <el-row>
               <el-col :span="24">
                  <el-form-item label="备注">
                     <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
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

      <!-- 用户导入对话框 -->
      <el-dialog :title="upload.title" v-model="upload.open" width="400px" append-to-body>
         <el-upload ref="uploadRef" :limit="1" accept=".xlsx, .xls" :headers="upload.headers"
            :action="upload.url" :disabled="upload.isUploading"
            :on-progress="handleFileUploadProgress" :on-success="handleFileSuccess" :auto-upload="false" drag>
            <el-icon class="el-icon--upload">
               <upload-filled />
            </el-icon>
            <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
            <template #tip>
               <div class="el-upload__tip text-center">
                  <!-- <div class="el-upload__tip">
                     <el-checkbox v-model="upload.updateSupport" />是否更新已经存在的用户数据
                  </div> -->
                  <span>仅允许导入xls、xlsx格式文件。</span>
                  <el-link type="primary" :underline="false" style="font-size:12px;vertical-align: baseline;"
                     @click="importTemplate">下载模板</el-link>
               </div>
            </template>
         </el-upload>
         <template #footer>
            <div class="dialog-footer">
               <el-button type="primary" @click="submitFileForm">确 定</el-button>
               <el-button @click="upload.open = false">取 消</el-button>
            </div>
         </template>
      </el-dialog>
   </div>
</template>

<script setup name="User">
import { getToken } from "@/utils/auth";
import { changeUserStatus, listUser, resetUserPwd, delUser, getUser, updateUser, addUser,getExportExcel } from "@/api/system/user";
import { roleOptionselect } from "@/api/system/role";
import { postOptionselect } from "@/api/system/post";
import { listDept } from "@/api/system/dept";


const router = useRouter();
const { proxy } = getCurrentInstance();
const { sys_normal_disable, sys_user_sex } = proxy.useDict("sys_normal_disable", "sys_user_sex");

const userList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const dateRange = ref([]);
const deptName = ref("");
const deptOptions = ref(undefined);
const initPassword = ref(undefined);
const postOptions = ref([]);
const roleOptions = ref([]);
/*** 用户导入参数 */
const upload = reactive({
   // 是否显示弹出层（用户导入）
   open: false,
   // 弹出层标题（用户导入）
   title: "",
   // 是否禁用上传
   isUploading: false,
   // 是否更新已经存在的用户数据
   updateSupport: 0,
   // 设置上传的请求头部
   headers: { Authorization: "Bearer " + getToken() },
   // 上传的地址
   url: import.meta.env.VITE_APP_BASE_API + "/user/import"
});
// 列显隐信息
const columns = ref([
   { key: 0, label: `用户编号`, visible: true },
   { key: 1, label: `用户名称`, visible: true },
   { key: 2, label: `用户昵称`, visible: true },
   { key: 3, label: `部门`, visible: true },
   { key: 4, label: `手机号码`, visible: true },
   { key: 5, label: `状态`, visible: true },
   { key: 6, label: `创建时间`, visible: true }
]);

const data = reactive({
   form : {
   },
   queryParams: {
      skipCount: 1,
      maxResultCount: 10,
      userName: undefined,
      phone: undefined,
      state: true,
      deptId: undefined
   },
   rules: {
      userName: [{ required: true, message: "用户名称不能为空", trigger: "blur" }, { min: 2, max: 20, message: "用户名称长度必须介于 2 和 20 之间", trigger: "blur" }],
      nick: [{ required: true, message: "用户昵称不能为空", trigger: "blur" }],
      // password: [{ required: true, message: "用户密码不能为空", trigger: "blur" }, { min: 5, max: 20, message: "用户密码长度必须介于 5 和 20 之间", trigger: "blur" }],
      email: [{ type: "email", message: "请输入正确的邮箱地址", trigger: ["blur", "change"] }],
      phone: [{ pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/, message: "请输入正确的手机号码", trigger: "blur" }]
   }
});

const { queryParams, form, rules } = toRefs(data);

/** 通过条件过滤节点  */
const filterNode = (value, data) => {
   if (!value) return true;
   return data.label.indexOf(value) !== -1;
};
/** 根据名称筛选部门树 */
watch(deptName, val => {
   proxy.$refs["deptTreeRef"].filter(val);
});
/** 查询部门下拉树结构 */
function getDeptTree() {
   listDept().then(response => {
      const selectList = [];
      response.data.items.forEach(res => {
         selectList.push({ id: res.id, label: res.deptName, parentId: res.parentId, orderNum: res.orderNum, children: [] })
      }

      )
      deptOptions.value = proxy.handleTree(selectList, "id");;
   });
};
/** 查询用户列表 */
function getList() {
   loading.value = true;
   listUser(proxy.addDateRange(queryParams.value, dateRange.value)).then(res => {

      loading.value = false;
      userList.value = res.data.items;
      total.value = res.data.totalCount;
   });
};
/** 节点单击事件 */
function handleNodeClick(data) {
   queryParams.value.deptId = data.id;
   handleQuery();
};
/** 搜索按钮操作 */
function handleQuery() {
   queryParams.value.skipCount = 1;
   getList();
};
/** 重置按钮操作 */
function resetQuery() {
   proxy.resetForm("queryRef");


   // console.log(proxy.$refs["deptTreeRef"])
   queryParams.value.deptId=undefined;
   handleQuery();
};
/** 删除按钮操作 */
function handleDelete(row) {
   const userIds = row.id || ids.value;
   proxy.$modal.confirm('是否确认删除用户编号为"' + userIds + '"的数据项？').then(function () {
      return delUser(userIds);
   }).then(() => {
      getList();
      proxy.$modal.msgSuccess("删除成功");
   }).catch(() => { });
};
/** 导出按钮操作 */
function handleExport() {
   getExportExcel(proxy.addDateRange(queryParams.value, dateRange.value));
};
/** 用户状态修改  */
function handleStatusChange(row) {
   console.log(row.state);
   let text = row.state === true ? "启用" : "停用";
   proxy.$modal.confirm('确认要"' + text + '""' + row.userName + '"用户吗?').then(function () {
      return changeUserStatus(row.id, row.state);
   }).then(() => {
      proxy.$modal.msgSuccess(text + "成功");
   }).catch(function () {
      row.state = row.state === true ? false : true;
   });
};
/** 更多操作 */
function handleCommand(command, row) {
   switch (command) {
      case "handleResetPwd":
         handleResetPwd(row);
         break;
      case "handleAuthRole":
         handleAuthRole(row);
         break;
      default:
         break;
   }
};
/** 跳转角色分配 */
function handleAuthRole(row) {
   const userId = row.id;
   router.push("/system/user-auth/role/" + userId);
};
/** 重置密码按钮操作 */
function handleResetPwd(row) {
   proxy.$prompt('请输入"' + row.userName + '"的新密码', "提示", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      closeOnClickModal: false,
      inputPattern: /^.{5,20}$/,
      inputErrorMessage: "用户密码长度必须介于 5 和 20 之间",
   }).then(({ value }) => {
      resetUserPwd(row.id, value).then(response => {
         proxy.$modal.msgSuccess("修改成功，新密码是：" + value);
      });
   }).catch(() => { });
};
/** 选择条数  */
function handleSelectionChange(selection) {
   ids.value = selection.map(item => item.id);
   single.value = selection.length != 1;
   multiple.value = !selection.length;
};
/** 导入按钮操作 */
function handleImport() {
   upload.title = "用户导入";
   upload.open = true;
};
/** 下载模板操作 */
function importTemplate() {
   proxy.download("user/template");
};
/**文件上传中处理 */
const handleFileUploadProgress = (event, file, fileList) => {
   upload.isUploading = true;
};
/** 文件上传成功处理 */
const handleFileSuccess = (response, file, fileList) => {
   upload.open = false;
   upload.isUploading = false;
   proxy.$refs["uploadRef"].handleRemove(file);
   proxy.$alert("<div style='overflow: auto;overflow-x: hidden;max-height: 70vh;padding: 10px 20px 0;'>" + response.msg + "</div>", "导入结果", { dangerouslyUseHTMLString: true });
   getList();
};
/** 提交上传文件 */
function submitFileForm() {
   proxy.$refs["uploadRef"].submit();
};
/** 重置操作表单 */
function reset() {
   form.value = {
         userName: undefined,
         nick: undefined,
         password: undefined,
         phone: undefined,
         email: undefined,
         sex: undefined,
         state: true,
         remark: undefined,
      postIds: [],
      roleIds: [],
      deptId: undefined
   };
   proxy.resetForm("userRef");

   if (postOptions.value.length == 0 || roleOptions.value.length == 0) {
      roleOptionselect().then(response => {
         //岗位从另一个接口获取全量
         roleOptions.value = response.data.items;
      })
      postOptionselect().then(response => {
         postOptions.value = response.data.items;

      }

      )

   }




};
/** 取消按钮 */
function cancel() {
   open.value = false;
   reset();
};


/** 新增按钮操作 */
function handleAdd() {
   reset();

   open.value = true;
   title.value = "添加用户";
};
/** 修改按钮操作 */
function handleUpdate(row) {
   reset();
   const userId = row.id || ids.value;
   getUser(userId).then(response => {
     

      form.value = response.data;
      form.value.postIds=[];
response.data.posts.forEach(post => {
   form.value.postIds.push(post.id)
});

form.value.deptId= response.data.deptId;
form.value.roleIds=[];
response.data.roles.forEach(role => {
   form.value.roleIds.push(role.id)
});
open.value = true;
title.value = "修改用户";
form.value.password = null;



   });
};
/** 提交按钮 */
function submitForm() {
   proxy.$refs["userRef"].validate(valid => {
      if (valid) {
         if (form.value.id != undefined) {
            updateUser(form.value.id,form.value).then(async response => {
               proxy.$modal.msgSuccess("修改成功");
               open.value = false;
              await getList();
            });
         } else {
            addUser(form.value).then(async response => {
               proxy.$modal.msgSuccess("新增成功");
               open.value = false;
              await getList();
            });
         }
      }
   });
};
getDeptTree();
getList();

</script>
