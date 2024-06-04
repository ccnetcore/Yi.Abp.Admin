<template>
  <div class="app-container">
    <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
      <el-form-item label="角色名称" prop="roleName">
        <el-input v-model="queryParams.roleName" placeholder="请输入角色名称" clearable style="width: 240px"
          @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="角色编号" prop="roleCode">
        <el-input v-model="queryParams.roleCode" placeholder="请输入角色编号" clearable style="width: 240px"
          @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="状态" prop="state">
        <el-select v-model="queryParams.state" placeholder="角色状态" clearable style="width: 240px">
          <el-option v-for="dict in sys_normal_disable" :key="dict.value" :label="dict.label" :value="dict.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="创建时间" style="width: 308px">
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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['system:role:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate"
          v-hasPermi="['system:role:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete"
          v-hasPermi="['system:role:remove']">删除</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="warning" plain icon="Download" @click="handleExport" v-hasPermi="['system:role:export']">导出
        </el-button>
      </el-col>
      <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <!-- 表格数据 -->
    <el-table v-loading="loading" :data="roleList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="50" align="center" />
      <el-table-column label="角色编号" prop="roleCode" />
      <el-table-column label="角色名称" prop="roleName" :show-overflow-tooltip="true" />
      <el-table-column label="权限字符" prop="roleCode" :show-overflow-tooltip="true" />
      <el-table-column label="显示顺序" prop="orderNum" />
      <el-table-column label="状态" align="center">
        <template #default="scope">
          <el-switch v-model="scope.row.state" :active-value="true" :inactive-value="false"
            @change="handleStatusChange(scope.row)"></el-switch>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="creationTime">
        <template #default="scope">
          <span>{{ parseTime(scope.row.creationTime) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-tooltip content="修改" placement="top" v-if="scope.row.roleId !== 1">
            <el-button link icon="Edit" @click=" handleUpdate(scope.row)" v-hasPermi="['system:role:edit']">
            </el-button>
          </el-tooltip>
          <el-tooltip content="删除" placement="top" v-if="scope.row.roleId !== 1">
            <el-button link icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['system:role:remove']">
            </el-button>
          </el-tooltip>
          <el-tooltip content="数据权限" placement="top" v-if="scope.row.roleId !== 1">
            <el-button link icon="CircleCheck" @click="handleDataScope(scope.row)"
              v-hasPermi="['system:role:edit']"></el-button>
          </el-tooltip>
          <!-- <el-tooltip content="分配用户" placement="top" v-if="scope.row.roleId !== 1">
            <el-button link icon="User" @click="handleAuthUser(scope.row)" v-hasPermi="['system:role:edit']">
            </el-button>
          </el-tooltip> -->
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="total > 0" :total="Number(total)" v-model:page="queryParams.skipCount" v-model:limit="queryParams.maxResultCount"
      @pagination="getList" />

    <!-- 添加或修改角色配置对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="roleRef" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="角色名称" prop="roleName">
          <el-input v-model="form.roleName" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item prop="roleCode">
          <template #label>
            <span>
              <el-tooltip content="控制器中定义的权限字符，如：@PreAuthorize(`@ss.hasRole('admin')`)" placement="top">
                <el-icon>
                  <question-filled />
                </el-icon>
              </el-tooltip>
              权限字符
            </span>
          </template>
          <el-input v-model="form.roleCode" placeholder="请输入权限字符" />
        </el-form-item>
        <el-form-item label="角色顺序" prop="orderNum">
          <el-input-number v-model="form.orderNum" controls-position="right" :min="0" />
        </el-form-item>
        <el-form-item label="状态">
          <el-radio-group v-model="form.state">
            <el-radio v-for="dict in sys_normal_disable" :key="dict.value" :value="JSON.parse(dict.value)">{{ dict.label
            }}</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="菜单权限">
          <el-checkbox v-model="menuExpand" @change="handleCheckedTreeExpand($event, 'menu')">展开/折叠</el-checkbox>
          <el-checkbox v-model="menuNodeAll" @change="handleCheckedTreeNodeAll($event, 'menu')">全选/全不选</el-checkbox>
          <el-checkbox v-model="form.menuCheckStrictly" @change="handleCheckedTreeConnect($event, 'menu')">父子联动
          </el-checkbox>
          <el-tree class="tree-border" :data="menuOptions" show-checkbox ref="menuRef" node-key="id"
            :check-strictly="!form.menuCheckStrictly" empty-text="加载中，请稍候"
            :props="{ label: 'label', children: 'children' }"></el-tree>
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配角色数据权限对话框 -->
    <el-dialog :title="title" v-model="openDataScope" width="500px" append-to-body>
      <el-form :model="form" label-width="80px">
        <el-form-item label="角色名称">
          <el-input v-model="form.roleName" :disabled="true" />
        </el-form-item>
        <el-form-item label="权限字符">
          <el-input v-model="form.roleCode" :disabled="true" />
        </el-form-item>
        <el-form-item label="权限范围">
          <el-select v-model="form.dataScope" @change="dataScopeSelectChange">
            <el-option v-for="item in dataScopeOptions" :key="item.value" :label="item.label" :value="item.value">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="数据权限" v-show="form.dataScope == 'CUSTOM'">
          <el-checkbox v-model="deptExpand" @change="handleCheckedTreeExpand($event, 'dept')">展开/折叠</el-checkbox>
          <el-checkbox v-model="deptNodeAll" @change="handleCheckedTreeNodeAll($event, 'dept')">全选/全不选</el-checkbox>
          <el-checkbox v-model="form.deptCheckStrictly" @change="handleCheckedTreeConnect($event, 'dept')">父子联动
          </el-checkbox>
          <el-tree class="tree-border" :data="deptOptions" show-checkbox default-expand-all ref="deptRef" node-key="id"
            :check-strictly="!form.deptCheckStrictly" empty-text="加载中，请稍候"
            :props="{ label: 'label', children: 'children' }"></el-tree>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitDataScope">确 定</el-button>
          <el-button @click="cancelDataScope">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Role">
import {
  addRole,
  changeRoleStatus,
  dataScope,
  delRole,
  getRole,
  listRole,
  updateRole,
} from "@/api/system/role";
import {
  roleMenuTreeselect,
  treeselect as menuTreeselect,
  listMenu,
} from "@/api/system/menu";
import { listDept, roleDeptTreeselect } from "@/api/system/dept";
const router = useRouter();
const { proxy } = getCurrentInstance();
const { sys_normal_disable } = proxy.useDict("sys_normal_disable");

const roleList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const dateRange = ref([]);
const menuOptions = ref([]);
const menuExpand = ref(true);
const menuNodeAll = ref(false);
const deptExpand = ref(true);
const deptNodeAll = ref(false);
const deptOptions = ref([]);
const openDataScope = ref(false);
const menuRef = ref(null);
const deptRef = ref(null);

/** 数据范围选项*/
const dataScopeOptions = ref([
  { value: 'ALL', label: "全部数据权限" },
  { value: 'CUSTOM', label: "自定数据权限" },
  { value: 'DEPT', label: "本部门数据权限" },
  { value: 'DEPT_FOLLOW', label: "本部门及以下数据权限" },
  { value: 'USER', label: "仅本人数据权限" },
]);

const data = reactive({
  form: {
  },
  queryParams: {
    skipCount: 1,
    maxResultCount: 10,
    roleName: undefined,
    roleCode: undefined,
    state: undefined,
  },
  rules: {
    roleName: [
      { required: true, message: "角色名称不能为空", trigger: "blur" },
    ],
    roleCode: [
      { required: true, message: "权限字符不能为空", trigger: "blur" },
    ],
    orderNum: [
      { required: true, message: "角色顺序不能为空", trigger: "blur" },
    ],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询角色列表 */
function getList() {
  loading.value = true;
  listRole(proxy.addDateRange(queryParams.value, dateRange.value)).then(
    (response) => {
      roleList.value = response.data.items;
      total.value = response.data.totalCount;
      loading.value = false;
    }
  );
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
/** 删除按钮操作 */
function handleDelete(row) {
  const roleIds = row.id || ids.value;
  proxy.$modal
    .confirm('是否确认删除角色编号为"' + roleIds + '"的数据项?')
    .then(function () {
      return delRole(roleIds);
    })
    .then(() => {
      getList();
      proxy.$modal.msgSuccess("删除成功");
    })
    .catch(() => { });
}
/** 导出按钮操作 */
function handleExport() {
  proxy.download(
    "system/role/export",
    {
      ...queryParams.value,
    },
    `role_${new Date().getTime()}.xlsx`
  );
}
/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.id);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}
/** 角色状态修改 */
function handleStatusChange(row) {
  let text = row.state === true ? "启用" : "停用";
  proxy.$modal
    .confirm('确认要"' + text + '""' + row.roleName + '"角色吗?')
    .then(function () {
      return changeRoleStatus(row.id, row.state);
    })
    .then(() => {
      proxy.$modal.msgSuccess(text + "成功");
    })
    .catch(function () {
      row.state = row.state === true ? false : true;
    });
}
/** 更多操作 */
function handleCommand(command, row) {
  switch (command) {
    case "handleDataScope":
      handleDataScope(row);
      break;
    case "handleAuthUser":
      handleAuthUser(row);
      break;
    default:
      break;
  }
}
/** 分配用户 */
function handleAuthUser(row) {
  router.push("/system/role-auth/user/" + row.id);
}
/** 查询菜单树结构 */
function getMenuTreeselect() {
  return listMenu().then((response) => {
    const options = [];
    response.data.items.forEach((m) => {
      options.push({
        id: m.id,
        label: m.menuName,
        parentId: m.parentId,
        children: m.children,
      });
    });
    menuOptions.value = proxy.handleTree(options);

    return response;
  });
}

/** 所有部门节点数据 */
function getDeptAllCheckedKeys() {
  // 目前被选中的部门节点
  let checkedKeys = deptRef.value.getCheckedKeys();
  // 半选中的部门节点
  let halfCheckedKeys = deptRef.value.getHalfCheckedKeys();
  checkedKeys.unshift.apply(checkedKeys, halfCheckedKeys);
  return checkedKeys;
}
/** 重置新增的表单以及其他数据  */
function reset() {
  if (menuRef.value != undefined) {
    menuRef.value.setCheckedKeys([]);
  }
  menuExpand.value = false;
  menuNodeAll.value = false;
  deptExpand.value = true;
  deptNodeAll.value = false;
  form.value = {

    id: undefined,
    roleName: undefined,
    roleCode: undefined,
    orderNum: 0,
    state: true,
    menuCheckStrictly: false,
    deptCheckStrictly: false,
    remark: undefined,
    dataScope: 0,
    menuIds: [],
    deptIds: [],
  };
  proxy.resetForm("roleRef");
}
/** 添加角色 */
function handleAdd() {
  reset();
  getMenuTreeselect();
  open.value = true;
  title.value = "添加角色";
}
/** 修改角色 */
 function handleUpdate(row) {
  reset();
  const roleId = row.id || ids.value;
  getRoleMenuTreeselect(roleId);
  getRole(roleId).then((response) => {
    form.value = response.data;
    form.value.orderNum = Number(form.value.orderNum);
    open.value = true;
    title.value = "修改角色";
  });
}
/** 根据角色ID查询菜单树结构 */
 function getRoleMenuTreeselect(roleId) {
  //1：获取该角色下的全部菜单id
  //2：获取全量菜单
    const menuTreeselect=getMenuTreeselect();
    menuTreeselect.then(()=>{
      nextTick(() => {
      roleMenuTreeselect(roleId).then((response) => {
    const menuIds = [];
    response.data.forEach((m) => {
      menuIds.push(m.id);
    });

      menuIds.forEach((v) => {
        menuRef.value.setChecked(v, true, false);
      });
      //这里是要一个当前用户已拥有的菜单的id
    });
  });

    })


}
/** 根据角色ID查询部门树结构 */
function getDeptTree(roleId) {
  return listDept().then((response) => {
    const selectList = [];
    response.data.items.forEach((res) => {
      selectList.push({
        id: res.id,
        label: res.deptName,
        parentId: res.parentId,
        orderNum: res.orderNum,
        children: [],
      });
    });
    deptOptions.value = proxy.handleTree(selectList, "id");

    let deptIds = [];
    roleDeptTreeselect(roleId).then((response) => {
      deptIds = response.data.map(x=>x.id);
      // nextTick(() => {
      if (deptRef.value) {
        deptRef.value.setCheckedKeys(deptIds);
      }
      // });
    });
  });
}
/** 树权限（展开/折叠）*/
function handleCheckedTreeExpand(value, type) {
  if (type == "menu") {
    let treeList = menuOptions.value;
    for (let i = 0; i < treeList.length; i++) {
      menuRef.value.store.nodesMap[treeList[i].id].expanded = value;
    }
  } else if (type == "dept") {
    let treeList = deptOptions.value;
    for (let i = 0; i < treeList.length; i++) {
      deptRef.value.store.nodesMap[treeList[i].id].expanded = value;
    }
  }
}
/** 树权限（全选/全不选） */
function handleCheckedTreeNodeAll(value, type) {
  if (type == "menu") {
    menuRef.value.setCheckedNodes(value ? menuOptions.value : []);
  } else if (type == "dept") {
    deptRef.value.setCheckedNodes(value ? deptOptions.value : []);
  }
}
/** 树权限（父子联动） */
function handleCheckedTreeConnect(value, type) {
  if (type == "menu") {
    form.value.menuCheckStrictly = value ? true : false;
  } else if (type == "dept") {
    form.value.deptCheckStrictly = value ? true : false;
  }
}
/** 所有菜单节点数据 */
function getMenuAllCheckedKeys() {
  // 目前被选中的菜单节点
  let checkedKeys = menuRef.value.getCheckedKeys();
  // 半选中的菜单节点
  let halfCheckedKeys = menuRef.value.getHalfCheckedKeys();
  checkedKeys.unshift.apply(checkedKeys, halfCheckedKeys);
  return checkedKeys;
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs["roleRef"].validate((valid) => {
    if (valid) {
      if (form.value.id != undefined) {
        form.value.menuIds = getMenuAllCheckedKeys();
        updateRole(form.value).then((response) => {
          proxy.$modal.msgSuccess("修改成功");
          open.value = false;
          getList();
        });
      } else {
        form.value.menuIds = getMenuAllCheckedKeys();
        addRole(form.value).then((response) => {
          proxy.$modal.msgSuccess("新增成功");
          open.value = false;
          getList();
        });
      }
    }
  });
}
/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}
/** 选择角色权限范围触发 */
function dataScopeSelectChange(value) {
  if (value !== "2") {
    deptRef.value.setCheckedKeys([]);
  }
}
/** 分配数据权限操作 */
function handleDataScope(row) {
  reset();
  getDeptTree(row.id);
  getRole(row.id).then((response) => {
    form.value = response.data;
    openDataScope.value = true;
    title.value = "分配数据权限";
  });
}
/** 提交按钮（数据权限） */
function submitDataScope() {
  if (form.value.id != undefined) {
    form.value.deptIds = getDeptAllCheckedKeys();
const data={
  roleId:form.value.id,
  deptIds:form.value.deptIds,
  dataScope:form.value.dataScope
}
console.log(data)
    dataScope(data).then((response) => {
      proxy.$modal.msgSuccess("修改成功");
      openDataScope.value = false;
      getList();
    });
  }
}
/** 取消按钮（数据权限）*/
function cancelDataScope() {
  openDataScope.value = false;
  reset();
}

getList();
</script>
