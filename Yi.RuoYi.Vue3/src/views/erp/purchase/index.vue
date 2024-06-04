<template>
  <div class="app-container">
    <el-form
      :model="queryParams"
      ref="queryRef"
      :inline="true"
      v-show="showSearch"
      label-width="90px"
    >
      <el-form-item label="供应商名称" prop="name">
        <el-input
          v-model="queryParams.name"
          placeholder="请输入供应商名称"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
          prop="name"
        />
      </el-form-item>
      <el-form-item label="采购单编号" prop="code">
        <el-input
          v-model="queryParams.code"
          placeholder="请输入采购单编号"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
          prop="code"
        />
      </el-form-item>

      <el-form-item label="采购员" prop="buyer">
        <el-input
          v-model="queryParams.code"
          placeholder="请输入采购员"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
          prop="buyer"
        />
      </el-form-item>
      <!-- <el-form-item label="状态" prop="isDeleted">
            <el-select
              v-model="queryParams.isDeleted"
              placeholder="状态"
              clearable
              style="width: 240px"
            >
              <el-option
                v-for="dict in sys_normal_disable"
                :key="dict.value"
                :label="dict.label"
                :value="dict.value"
              />
            </el-select>
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
          v-hasPermi="['erp:purchase:add']"
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
          v-hasPermi="['erp:purchase:edit']"
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
          v-hasPermi="['erp:purchase:remove']"
          >删除</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="warning"
          plain
          icon="Download"
          @click="handleExport"
          v-hasPermi="['erp:purchase:export']"
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
      <el-table-column label="采购单号" align="center" prop="code" />

      <el-table-column
        label="供应商"
        align="center"
        prop="supplierName"
        :show-overflow-tooltip="true"
      />

      <el-table-column
        label="需求时间"
        align="center"
        prop="needTime"
        :show-overflow-tooltip="true"
      />

      <el-table-column
        label="采购员"
        align="center"
        prop="buyer"
        :show-overflow-tooltip="true"
      />

      <el-table-column
        label="总共金额"
        align="center"
        prop="totalMoney"
        :show-overflow-tooltip="true"
      />

      <el-table-column
        label="已支付金额"
        align="center"
        prop="paidMoney"
        :show-overflow-tooltip="true"
      />

      <el-table-column
        label="采购状态"
        align="center"
        prop="purchaseState"
        :show-overflow-tooltip="true"
      >
    
      <template #default="scope">
        <el-tag>{{purchaseStateComputed(scope.row.purchaseState)}}</el-tag>
       </template>

    </el-table-column>

      <el-table-column
        label="备注"
        align="center"
        prop="remarks"
        :show-overflow-tooltip="true"
      />
      <!-- <el-table-column label="状态" align="center" prop="isDeleted">
            <template #default="scope">
              <dict-tag
                :options="sys_normal_disable"
                :value="scope.row.isDeleted"
              />
            </template>
          </el-table-column> -->
      <!-- <el-table-column
            label="备注"
            align="center"
            prop="remark"
            :show-overflow-tooltip="true"
          />
          <el-table-column
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
            @click="handleGet(scope.row)"
            v-hasPermi="['erp:purchase:edit']"
            >查看</el-button
          >

          <el-button
            link
            icon="Edit"
            @click="handleUpdate(scope.row)"
            v-hasPermi="['erp:purchase:edit']"
            >修改</el-button
          >

          <el-button
            link
            icon="Delete"
            @click="handleDelete(scope.row)"
            v-hasPermi="['erp:purchase:remove']"
            >结束</el-button
          >
          <el-button
            link
            icon="Delete"
            @click="handleDelete(scope.row)"
            v-hasPermi="['erp:purchase:remove']"
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
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="采购单编码" prop="code">
              <el-input v-model="form.code" placeholder="请输入采购单编码" />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="供应商" prop="supplierName">
              <el-select
                v-model="form.supplierId"
                filterable
                placeholder="请选择供应商"
              >
                <el-option
                  v-for="item in supplierList"
                  :key="item.id"
                  :label="item.name"
                  :value="item.id"
                />
              </el-select>
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="采购单员" prop="buyer">
              <el-input v-model="form.buyer" placeholder="请输入采购单员" />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="需求时间" prop="needTime">
              <el-date-picker
                v-model="form.needTime"
                type="date"
                placeholder="选择一个日期"
                size="default"
              />
            </el-form-item>
          </el-col>

          <el-col :offset="8" :span="8">
            <el-form-item label="总金额">
              {{ showTotalMoney }}
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="物料信息">
              <el-table :data="form.purchaseDetails" border style="width: 100%">
                <el-table-column width="90">
                  <template #default="scope">
                    <el-button
                      @click="delMaterialRow(scope.$index)"
                      icon="Delete"
                      type="danger"
                      size="small"
                      >删除</el-button
                    >
                  </template>
                </el-table-column>
                <el-table-column type="index" label="序号" width="60" />
                <el-table-column prop="materialName" label="物料" width="180" />
                <el-table-column prop="materialUnit" label="单位" width="180" />
                <el-table-column prop="unitPrice" label="单价" width="180">
                  <template #default="scope">
                    <el-input v-model="scope.row.unitPrice" />
                  </template>
                </el-table-column>
                <el-table-column
                  prop="totalNumber"
                  label="采购数量"
                  width="180"
                >
                  <template #default="scope">
                    <el-input v-model="scope.row.totalNumber" />
                  </template>
                </el-table-column>
                <el-table-column prop="remarks" label="备注">
                  <template #default="scope">
                    <el-input v-model="scope.row.remarks" />
                  </template>
                </el-table-column>
              </el-table>
              <el-button
                class="form-add-btn"
                type="primary"
                icon="CirclePlus"
                plain
                @click="materialHandleAdd"
                >添加物料</el-button
              >
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="备注" prop="remarks">
          <el-input
            v-model="form.remarks"
            type="textarea"
            placeholder="请输入内容"
            :rows="5"
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

    <!-- ---------------------这里是新增物料的对话框--------------------- -->
    <el-dialog
      title="添加物料"
      v-model="openMaterial"
      width="800px"
      append-to-body
    >
      <el-form
        :model="queryMaterialParams"
        ref="queryMaterialRef"
        :inline="true"
        label-width="70px"
      >
        <el-form-item label="物料名称" prop="name">
          <el-input
            v-model="queryMaterialParams.name"
            placeholder="请输入物料名称"
            clearable
            style="width: 150px"
            @keyup.enter="handleMaterialQuery"
            prop="name"
          />
        </el-form-item>
        <el-form-item label="物料编码" prop="code">
          <el-input
            v-model="queryMaterialParams.code"
            placeholder="请输入物料编码"
            clearable
            style="width: 150px"
            @keyup.enter="handleMaterialQuery"
            prop="code"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleMaterialQuery"
            >搜索</el-button
          >
          <el-button icon="Refresh" @click="resetMaterialQuery">重置</el-button>
        </el-form-item>
        <!-- 物料表单 -->
        <el-table
          style="width: 100%"
          :data="materialList"
          @selection-change="materialHandleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column property="code" label="物料编码" />
          <el-table-column property="name" label="物料信息" />
          <el-table-column property="unitName" label="物料单位" />
        </el-table>
        <pagination
          v-show="materialTotal > 0"
          :total="Number(materialTotal) "
          v-model:page="queryMaterialParams.skipCount"
          v-model:limit="queryMaterialParams.maxResultCount"
          @pagination="getMaterialList"
        />
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitMaterialForm"
            >确 定</el-button
          >
          <el-button @click="materialCancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- ---------------------这里是采购订单详情对话框--------------------- -->
    <el-dialog
      title="订单详情"
      v-model="openDetails"
      width="1200px"
      append-to-body
    >
      <el-form
        ref="dataDetailsRef"
        :model="form"
        :rules="rules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="物料" prop="code">
              <el-input  placeholder="请输入物料" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="创建时间" style="width: 308px">
              <el-date-picker
                value-format="YYYY-MM-DD"
                type="daterange"
                range-separator="-"
                start-placeholder="开始日期"
                end-placeholder="结束日期"
              ></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="10">
            <el-form-item>
          <el-button type="primary" icon="Search" 
          >查询</el-button
        >
      </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="物料信息">
              <el-table :data="form.purchaseDetails" border style="width: 100%" >
                <el-table-column type="index" label="序号" width="60" align="center"  />
                <el-table-column prop="materialName" label="物料" width="180" align="center" />
                <el-table-column prop="materialUnit" label="单位" width="180" align="center" />
                <el-table-column prop="unitPrice" label="单价" width="180" align="center">
                </el-table-column>
                <el-table-column
                  prop="totalNumber"
                  label="采购数量"
                  width="180"
                  align="center"
                >
                </el-table-column>
                <el-table-column prop="remarks" label="备注" >
                </el-table-column>
              </el-table>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="订单备注" prop="remarks">
          {{ form.remarks }}
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
} from "@/api/erp/purchaseApi";
import { allData as supplierlAllData } from "@/api/erp/supplierApi";
import { listData as materialListData } from "@/api/erp/materialApi";
import { getListByPurchaseId } from "@/api/erp/purchaseDetailsApi";

import { ref } from "@vue/reactivity";
import { computed } from "@vue/runtime-core";
const { proxy } = getCurrentInstance();
const { sys_normal_disable } = proxy.useDict("sys_normal_disable");

//详情物料对话框
const openDetails = ref(false);

//添加物料对话框
const openMaterial = ref(false);
const materialList = ref([]);
const materialTotal = ref(0);
const materialMultipleSelection = ref([]);

//选择框供应商
const supplierList = ref([]);

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
  form: {
    supplierId: undefined,
    totalMoney: 0,
    purchaseDetails: [],
  },
  queryParams: {
    skipCount: 1,
    maxResultCount: 10,
    name: undefined,
    code: undefined,
    buyer: undefined,
  },
  queryMaterialParams: {
    skipCount: 1,
    maxResultCount: 10,
    name: undefined,
    code: undefined,
  },
  rules: {
    code: [{ required: true, message: "采购单编号不能为空", trigger: "blur" }],
    name: [{ required: true, message: "采购单名称不能为空", trigger: "blur" }],
  },
});

const { queryParams, form, queryMaterialParams, rules } = toRefs(data);
/** 查询列表 */
function getList() {
  loading.value = true;
  listData(proxy.addDateRange(queryParams.value, dateRange.value)).then(
    (response) => {
      dataList.value = response.data.data;
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
  title.value = "添加采购单";
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
    title.value = "修改采购单";
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

/** 查看详情 */
function handleGet(row) {
  getListByPurchaseId(row.id).then((response) => {
    form.value.purchaseDetails = response.data;
  });
  form.value.remarks=row.remarks;
  openDetails.value = true;
}

//-------------这里开始是物料对话框的数据-----------
/** 物料查询列表 */
function getMaterialList() {
  materialListData(
    proxy.addDateRange(queryMaterialParams.value, dateRange.value)
  ).then((response) => {
    materialList.value = response.data.data;
    materialTotal.value = response.data.totalCount;
  });
}
/** 表单改变选择 */
function materialHandleSelectionChange(select) {
  materialMultipleSelection.value = select;
}
/** 打开对话框 */
function materialHandleAdd() {
  getMaterialList();
  openMaterial.value = true;
}
/** 搜索 */
function handleMaterialQuery() {
  getMaterialList();
}
/**重置表单 */
function resetMaterialQuery() {
  proxy.resetForm("queryMaterialRef");
  handleMaterialQuery();
}
/** 提交物料表单 */
function submitMaterialForm() {
  if (materialMultipleSelection.value.length > 0) {
    const purchaseDetailsList = materialMultipleSelection.value.map((u) => {
      return { materialName: u.name, materialUnit: u.unitName };
    });

    form.value.purchaseDetails.push(...purchaseDetailsList);
    materialCancel();
  } else {
    proxy.$modal.msgError("请选择至少一个物料");
  }
}
/** 取消对话框 */
function materialCancel() {
  openMaterial.value = false;
}

/** 删除物料行 */
function delMaterialRow(index) {
  form.value.purchaseDetails.splice(index, 1);
}
// watch(data.form, (newValue, oldValue) => {
// console.log(newValue.purchaseDetails,999)
//       }
//       , { immdiate: true })
/** 计算属性：实时计算展示的价格 */
const showTotalMoney = computed(() => {
  let res = 0;
  form.value.purchaseDetails.forEach((details) => {
    if (details.unitPrice != undefined && details.totalNumber != undefined) {
      res += details.unitPrice * details.totalNumber;
    }
  });
  return res;
});

const  purchaseStateEnum=[

{key:"build",name:"新建"},
{key:"run",name:"进行中"},
{key:"complete",name:"已完成"},
{key:"end",name:"已结束"},
]
/** 计算属性：采购订单状态 */
const purchaseStateComputed= computed(()=>
(purchaseState) => { 
return  purchaseStateEnum[purchaseState].name
});
//-------------这里开始是供货商的数据-----------
/** 供货商查询列表 */
function getSupplierList() {
  supplierlAllData().then((response) => {
    supplierList.value = response.data;
  });
}
getList();
getSupplierList();
</script>
<style scoped>
.form-add-btn {
  width: 100%;
  margin-top: 10px;
}
</style>