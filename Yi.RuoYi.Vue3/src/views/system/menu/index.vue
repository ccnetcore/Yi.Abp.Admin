<template>
   <div class="app-container">
      <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch">
         <el-form-item label="菜单名称" prop="menuName">
            <el-input
               v-model="queryParams.menuName"
               placeholder="请输入菜单名称"
               clearable
               @keyup.enter="handleQuery"
            />
         </el-form-item>
         <el-form-item label="状态" prop="state">
            <el-select v-model="queryParams.state" placeholder="菜单状态" clearable>
               <el-option
                  v-for="dict in sys_normal_disable"
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
               v-hasPermi="['system:menu:add']"
            >新增</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button 
               type="info"
               plain
               icon="Sort"
               @click="toggleExpandAll"
            >展开/折叠</el-button>
         </el-col>
         <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table
         v-if="refreshTable"
         v-loading="loading"
         :data="menuList"
         row-key="id"
         :default-expand-all="isExpandAll"
         :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      >
         <el-table-column prop="menuName" label="菜单名称" :show-overflow-tooltip="true" width="160"></el-table-column>
         <el-table-column prop="menuIcon" label="图标" align="center" width="100">
            <template #default="scope">
               <svg-icon :icon-class="scope.row.menuIcon+''" />
            </template>
         </el-table-column>
         <el-table-column prop="orderNum" label="排序" width="60"></el-table-column>
         <el-table-column prop="permissionCode" label="权限标识" :show-overflow-tooltip="true"></el-table-column>
         <el-table-column prop="router" label="路由" :show-overflow-tooltip="true"></el-table-column>
         <el-table-column prop="component" label="组件路径" :show-overflow-tooltip="true"></el-table-column>
         <el-table-column prop="state" label="状态" width="80">
            <template #default="scope">
               <dict-tag :options="sys_normal_disable" :value="scope.row.state" />
            </template>
         </el-table-column>
         <el-table-column label="创建时间" align="center" prop="creationTime">
            <template #default="scope">
               <span>{{ parseTime(scope.row.creationTime) }}</span>
            </template>
         </el-table-column>
         <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
            <template #default="scope">
               <el-button
                  link
                  icon="Edit"
                  @click="handleUpdate(scope.row)"
                  v-hasPermi="['system:menu:edit']"
               >修改</el-button>
               <el-button
                  link
                  icon="Plus"
                  @click="handleAdd(scope.row)"
                  v-hasPermi="['system:menu:add']"
               >新增</el-button>
               <el-button
                  link
                  icon="Delete"
                  @click="handleDelete(scope.row)"
                  v-hasPermi="['system:menu:remove']"
               >删除</el-button>
            </template>
         </el-table-column>
      </el-table>

      <!-- 添加或修改菜单对话框 -->
      <el-dialog :title="title" v-model="open" width="680px" append-to-body>
         <el-form ref="menuRef" :model="form" :rules="rules" label-width="100px">
            <el-row>
               <el-col :span="24">
                  <el-form-item label="上级菜单">
                     <el-tree-select
                        v-model="form.parentId"
                        :data="menuOptions"
                        :props="{ value: 'id', label: 'menuName', children: 'children' }"
                        value-key="id"
                        placeholder="选择上级菜单"
                        check-strictly
                     />
                  </el-form-item>
               </el-col>
               <el-col :span="24">
                  <el-form-item label="菜单类型" prop="menuType">
                     <el-radio-group v-model="form.menuType">
                        <el-radio value="Catalogue">目录</el-radio>
                        <el-radio value="Menu">菜单</el-radio>
                        <el-radio value="Component">按钮</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
               <el-col :span="24" v-if="form.menuType != 'Component'">
                  <el-form-item label="菜单图标" prop="menuIcon">
                     <el-popover
                        placement="bottom-start"
                        :width="540"
                        trigger="click"
                     >
                        <template #reference>
                           <el-input v-model="form.menuIcon" placeholder="点击选择图标" readonly>
                              <template #prefix>
                                 <svg-icon
                                    v-if="form.menuIcon"
                                    :icon-class="form.menuIcon+''"
                                    class="el-input__icon"
                                    style="height: 32px;width: 16px;"
                                 />
                                 <el-icon v-else style="height: 32px;width: 16px;"><search /></el-icon>
                              </template>
                           </el-input>
                        </template>
                        <icon-select ref="iconSelectRef" @selected="selected" />
                     </el-popover>
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="菜单名称" prop="menuName">
                     <el-input v-model="form.menuName" placeholder="请输入菜单名称" />
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="显示排序" prop="orderNum">
                     <el-input-number v-model="form.orderNum" controls-position="right" :min="0" />
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType != 'Component'">
                  <el-form-item>
                     <template #label>
                        <span>
                           <el-tooltip content="选择是外链则路由地址需要以`http(s)://`开头" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>是否外链
                        </span>
                     </template>
                     <el-radio-group v-model="form.isLink">
                        <el-radio :value=true>是</el-radio>
                        <el-radio :value=false>否</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col>

        
               <el-col :span="12" v-if="form.menuType != 'Component'">
                  <el-form-item prop="router">
                     <template #label>
                        <span>
                           <el-tooltip content="访问的路由地址，如：`user`，如外网地址需内链访问则以`http(s)://`开头" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           路由地址
                        </span>
                     </template>
                     <el-input v-model="form.router" placeholder="请输入路由地址" />
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType == 'Menu'">
                  <el-form-item prop="component">
                     <template #label>
                        <span>
                           <el-tooltip content="访问的组件路径，如：`system/user/index`，默认在`views`目录下" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           组件路径
                        </span>
                     </template>
                     <el-input v-model="form.component" placeholder="请输入组件路径" />
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType !='Catalogue'">
                  <el-form-item>
                     <el-input v-model="form.permissionCode" placeholder="请输入权限标识" maxlength="100" />
                     <template #label>
                        <span>
                           <el-tooltip content="控制器中定义的权限字符，如：@PreAuthorize(`@ss.hasPermi('system:user:list')`)" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           权限字符
                        </span>
                     </template>
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType == 'Menu'">
                  <el-form-item>
                     <el-input v-model="form.query" placeholder="请输入路由参数" maxlength="255" />
                     <template #label>
                        <span>
                           <el-tooltip content='访问路由的默认传递参数，如：`{"id": 1, "name": "ry"}`' placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           路由参数
                        </span>
                     </template>
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType == 'Menu'">
                  <el-form-item>
                     <template #label>
                        <span>
                           <el-tooltip content="选择是则会被`keep-alive`缓存，需要匹配组件的`name`和地址保持一致" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           是否缓存
                        </span>
                     </template>
                     <el-radio-group v-model="form.isCache">
                        <el-radio :value=true>缓存</el-radio>
                        <el-radio :value=false>不缓存</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType != 'Component'">
                  <el-form-item>
                     <template #label>
                        <span>
                           <el-tooltip content="选择隐藏则路由将不会出现在侧边栏，但仍然可以访问" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           显示状态
                        </span>
                     </template>
                     <el-radio-group v-model="form.isShow">
                        <el-radio
                           v-for="dict in sys_show_hide"
                           :key="dict.value"
                           :value="JSON.parse(dict.value)"
                        >{{ dict.label }}</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
               <el-col :span="12" v-if="form.menuType !='Component'">
                  <el-form-item>
                     <template #label>
                        <span>
                           <el-tooltip content="选择停用则路由将不会出现在侧边栏，也不能被访问" placement="top">
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                           菜单状态
                        </span>
                     </template>
                     <el-radio-group v-model="form.state">
                        <el-radio v-for="dict in sys_normal_disable" :key="dict.value" :value="JSON.parse(dict.value)">{{dict.label}}</el-radio>
                     </el-radio-group>
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

<script setup name="Menu">
import { addMenu, delMenu, getMenu, listMenu, updateMenu } from "@/api/system/menu";
import SvgIcon from "@/components/SvgIcon";
import IconSelect from "@/components/IconSelect";
import { ClickOutside as vClickOutside } from 'element-plus'

const { proxy } = getCurrentInstance();
const { sys_show_hide, sys_normal_disable } = proxy.useDict("sys_show_hide", "sys_normal_disable");

const menuList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const title = ref("");
const menuOptions = ref([]);
const isExpandAll = ref(false);
const refreshTable = ref(true);
const iconSelectRef = ref(null);

const guidEmpty="00000000-0000-0000-0000-000000000000";
const data = reactive({
  form: {

  },
  queryParams: {
    menuName: undefined,
    visible: true
  },
  rules: {
    menuName: [{ required: true, message: "菜单名称不能为空", trigger: "blur" }],
    orderNum: [{ required: true, message: "菜单顺序不能为空", trigger: "blur" }],
    router: [{ required: true, message: "路由地址不能为空", trigger: "blur" }]
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询菜单列表 */
function getList() {
  loading.value = true;
  console.log(queryParams.value,"queryParams.value");
  listMenu(queryParams.value).then(response => {
    menuList.value = proxy.handleTree(response.data.items, "id");
    loading.value = false;
  });
}
/** 查询菜单下拉树结构 */
function getTreeselect() {
  menuOptions.value = [];
  listMenu().then(response => {
    const menu = { id: guidEmpty, menuName: "主类目", children: [] };
    menu.children = proxy.handleTree(response.data.items, "id");
    menuOptions.value.push(menu);
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
    parentId: guidEmpty,
    menuName: undefined,
    menuIcon: undefined,
    menuType: 0,
    orderNum: 0,
    isLink: false,
    isCache: false,
    isShow: true,
    state: true
  };
  proxy.resetForm("menuRef");
}

/** 选择图标 */
function selected(name) {
  form.value.menuIcon = name;
}

/** 搜索按钮操作 */
function handleQuery() {
  getList();
}
/** 重置按钮操作 */
function resetQuery() {
  proxy.resetForm("queryRef");
  handleQuery();
}
/** 新增按钮操作 */
function handleAdd(row) {
  reset();
  getTreeselect();
  if (row != null && row.id) {
    form.value.parentId = row.id;
  } else {
    form.value.parentId = "00000000-0000-0000-0000-000000000000";
  }
  open.value = true;
  title.value = "添加菜单";
}
/** 展开/折叠操作 */
function toggleExpandAll() {
  refreshTable.value = false;
  isExpandAll.value = !isExpandAll.value;
  nextTick(() => {
    refreshTable.value = true;
  });
}
/** 修改按钮操作 */
async function handleUpdate(row) {
  reset();
  await getTreeselect();
  getMenu(row.id).then(response => {
    form.value = response.data;
    open.value = true;
    title.value = "修改菜单";
  });
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs["menuRef"].validate(valid => {
    if (valid) {
      if (form.value.id != undefined) {
        updateMenu(form.value).then(response => {
          proxy.$modal.msgSuccess("修改成功");
          open.value = false;
          getList();
        });
      } else {
        addMenu(form.value).then(response => {
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
  proxy.$modal.confirm('是否确认删除名称为"' + row.menuName + '"的数据项?').then(function() {
    return delMenu(row.id);
  }).then(() => {
    getList();
    proxy.$modal.msgSuccess("删除成功");
  }).catch(() => {});
}

getList();
</script>
