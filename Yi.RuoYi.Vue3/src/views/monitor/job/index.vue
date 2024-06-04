<template>
   <div class="app-container">
      <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="68px">
         <el-form-item label="任务名称" prop="jobName">
            <el-input
               v-model="queryParams.jobName"
               placeholder="请输入任务名称"
               clearable
               @keyup.enter="handleQuery"
            />
         </el-form-item>
         <el-form-item label="任务组名" prop="jobGroup">
                 <el-input
               v-model="queryParams.jobGroup"
               placeholder="请输入任务组名"
               clearable
               @keyup.enter="handleQuery"
            />
         </el-form-item>
         <el-form-item label="任务状态" prop="status">
            <el-select v-model="queryParams.status" placeholder="请选择任务状态" clearable>
               <el-option
                  v-for="dict in sys_job_status"
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
               v-hasPermi="['monitor:job:add']"
            >新增</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button
               type="success"
               plain
               icon="Edit"
               :disabled="single"
               @click="handleUpdate"
               v-hasPermi="['monitor:job:edit']"
            >修改</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button
               type="danger"
               plain
               icon="Delete"
               :disabled="multiple"
               @click="handleDelete"
               v-hasPermi="['monitor:job:remove']"
            >删除</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button
               type="warning"
               plain
               icon="Download"
               @click="handleExport"
               v-hasPermi="['monitor:job:export']"
            >导出</el-button>
         </el-col>
         <el-col :span="1.5">
            <el-button
               type="info"
               plain
               icon="Operation"
               @click="handleJobLog"
               v-hasPermi="['monitor:job:query']"
            >日志</el-button>
         </el-col>
         <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="jobList" @selection-change="handleSelectionChange">



         <el-table-column type="selection" width="55" align="center" />

         <el-table-column label="任务Id" width="100" align="center" prop="jobId" />

         <el-table-column label="任务组名" align="center" prop="groupName">
            <!-- <template #default="scope">
               <dict-tag :options="sys_job_group" :value="scope.row.jobGroup" />
            </template> -->
         </el-table-column>
         <el-table-column label="程序集" align="center" prop="assemblyName" :show-overflow-tooltip="true" />
                  <el-table-column label="类型" align="center" prop="jobType" :show-overflow-tooltip="true" />
   <el-table-column label="触发器参数" align="center" prop="triggerArgs" :show-overflow-tooltip="true" />
<el-table-column label="Job参数" align="center" prop="properties" :show-overflow-tooltip="true" /> 
   
               <el-table-column label="是否并行" align="center" prop="concurrent" :show-overflow-tooltip="true" /> 
                   <el-table-column label="最后执行时间" align="center" prop="lastRunTime" :show-overflow-tooltip="true" /> 
   
                   <el-table-column label="状态" align="center" prop="status" :show-overflow-tooltip="true" /> 
                <el-table-column label="描述" align="center" prop="description" :show-overflow-tooltip="true" /> 
         <!-- <el-table-column label="状态" align="center">
            <template #default="scope">
               <el-switch
                  v-model="scope.row.status"
                  active-value="0"
                  inactive-value="1"
                  @change="handleStatusChange(scope.row)"
               ></el-switch>
            </template>
         </el-table-column> -->
         <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
            <template #default="scope">
               <el-tooltip content="修改" placement="top">
                  <el-button
                     link
                     icon="Edit"
                     @click="handleUpdate(scope.row)"
                     v-hasPermi="['monitor:job:edit']"
                  ></el-button>
               </el-tooltip>
               <el-tooltip content="删除" placement="top">
                  <el-button
                     link
                     icon="Delete"
                     @click="handleDelete(scope.row)"
                     v-hasPermi="['monitor:job:remove']"
                  ></el-button>
               </el-tooltip>
               <el-tooltip content="执行一次" placement="top">
                  <el-button
                     link
                     icon="CaretRight"
                     @click="handleRun(scope.row)"
                     v-hasPermi="['monitor:job:changeStatus']"
                  ></el-button>
               </el-tooltip>
               <el-tooltip content="任务详细" placement="top">
                  <el-button
                     link
                     icon="View"
                     @click="handleView(scope.row)"
                     v-hasPermi="['monitor:job:query']"
                  ></el-button>
               </el-tooltip>
               <!-- <el-tooltip content="调度日志" placement="top">
                  <el-button
                     link
                     icon="Operation"
                     @click="handleJobLog(scope.row)"
                     v-hasPermi="['monitor:job:query']"
                  ></el-button>
               </el-tooltip> -->
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

      <!-- 添加或修改定时任务对话框 -->
      <el-dialog :title="title" v-model="open" width="800px" append-to-body>
         <el-form ref="jobRef" :model="form" :rules="rules" label-width="120px">
            <el-row>
               <el-col :span="12">
                  <el-form-item label="任务名称" prop="jobId">
                     <el-input v-model="form.jobId" placeholder="请输入任务名称" />
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="任务分组" prop="groupName">
                     <el-input v-model="form.groupName" placeholder="请输入任务分组" />
                  </el-form-item>
                    </el-col>
               <el-col :span="24">
                  <el-form-item prop="assemblyName">
                     <template #label>
                        <span>
                           调用程序集
                           <el-tooltip placement="top">
                              <template #content>
                                 <div>
                                    Bean调用示例：Yi.Furion.Application
                                    <!-- <br />Class类调用示例：com.ruoyi.quartz.task.RyTask.ryParams('ry')
                                    <br />参数说明：支持字符串，布尔类型，长整型，浮点型，整型 -->
                                 </div>
                              </template>
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                        </span>
                     </template>
                     <el-input v-model="form.assemblyName" placeholder="请输入调用程序集" />
                  </el-form-item>



                  <el-form-item prop="jobType">
                     <template #label>
                        <span>
                           job类名
                           <el-tooltip placement="top">
                              <template #content>
                                 <div>
                                    Bean调用示例：Yi.Furion.Application.Rbac.Job.TestJob
                                    <br />Class类调用示例：Yi.Furion.Application.Rbac.Job.TestJob
                                    <!-- <br />参数说明：支持字符串，布尔类型，长整型，浮点型，整型 -->
                                 </div>
                              </template>
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                        </span>
                     </template>
                     <el-input v-model="form.jobType" placeholder="请输入调用程序集下的job完整类名" />
                  </el-form-item>


                  <el-form-item prop="properties">
                     <template #label>
                        <span>
                           job任务参数
                           <el-tooltip placement="top">
                              <template #content>
                                 <div>
                                    参数示例：{"string":"obj","string","obj"}
                                    <br />调用示例：类型为字典key:string,value:obj
                                    <!-- <br />参数说明：支持字符串，布尔类型，长整型，浮点型，整型 -->
                                 </div>
                              </template>
                              <el-icon><question-filled /></el-icon>
                           </el-tooltip>
                        </span>
                     </template>
                     <el-input v-model="form.properties" placeholder="请输入任务参数" />
                  </el-form-item>


               </el-col>
               <el-col v-show="form.type=='Cron'" :span="24">
                  <el-form-item label="cron表达式" prop="cron">
                     <el-input v-model="form.cron" placeholder="请输入cron执行表达式">
                        <template #append>
                           <el-button type="primary" @click="handleShowCron">
                              生成表达式
                              <i class="el-icon-time el-icon--right"></i>
                           </el-button>
                        </template>
                     </el-input>
                  </el-form-item>
               </el-col>
                     <el-col v-show="form.type=='Millisecond'" :span="24">
                  <el-form-item label="定时毫秒间隔" prop="millisecond">
                     <el-input v-model="form.millisecond" placeholder="请输入毫秒间隔">
                     </el-input>
                  </el-form-item>
               </el-col>
               <el-col :span="24">
                  <el-form-item label="执行策略" prop="type">
                     <el-radio-group v-model="form.type">
                        <el-radio-button value="Cron">Cron表达式</el-radio-button>
                        <el-radio-button value="Millisecond">简单毫秒间隔</el-radio-button>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="是否并发" prop="concurrent">
                     <el-radio-group v-model="form.concurrent">
                        <el-radio-button :value=true>允许</el-radio-button>
                        <el-radio-button :value=false>禁止</el-radio-button>
                     </el-radio-group>
                  </el-form-item>
               </el-col>
               <!-- <el-col :span="12">
                  <el-form-item label="状态">
                     <el-radio-group v-model="form.status">
                        <el-radio
                           v-for="dict in sys_job_status"
                           :key="dict.value"
                           :label="dict.value"
                        >{{ dict.label }}</el-radio>
                     </el-radio-group>
                  </el-form-item>
               </el-col> -->
  

                          <el-col :span="24">
                  <el-form-item label="描述" prop="description">
                     <el-input v-model="form.description" placeholder="请输入任务描述" />
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
 
      <!-- 任务日志详细 -->
      <el-dialog title="任务详细" v-model="openView" width="700px" append-to-body>
         <el-form :model="form" label-width="120px">
            <el-row>
               <el-col :span="12">
                  <el-form-item label="任务Id：">{{ form.jobId }}</el-form-item>
                  <el-form-item label="任务分组：">{{ form.groupName }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="任务所在程序集：">{{ form.assemblyName }}</el-form-item>
                  <el-form-item label="任务完整类名：">{{ form.jobType }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="触发器参数：">{{ form.triggerArgs }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="下次执行时间：">{{ parseTime(form.nextRunTime) }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="最后执行时间：">{{ parseTime(form.lastRunTime) }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="已执行次数：">{{ form.numberOfRuns }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="任务状态：">{{ form.status }}</el-form-item>
               </el-col>
               <el-col :span="12">
                  <el-form-item label="是否并发：">
                     <div v-if="form.concurrent == true">允许</div>
                     <div v-else-if="form.concurrent == false">禁止</div>
                  </el-form-item>
               </el-col>
               <!-- <el-col :span="12">
                  <el-form-item label="执行策略：">
                     <div v-if="form.misfirePolicy == 0">默认策略</div>
                     <div v-else-if="form.misfirePolicy == 1">立即执行</div>
                     <div v-else-if="form.misfirePolicy == 2">执行一次</div>
                     <div v-else-if="form.misfirePolicy == 3">放弃执行</div>
                  </el-form-item>
               </el-col> -->
            </el-row>
         </el-form>
         <template #footer>
            <div class="dialog-footer">
               <el-button @click="openView = false">关 闭</el-button>
            </div>
         </template>
      </el-dialog>
   </div>
</template>

<script setup name="Job">
import { listJob, getJob, delJob, addJob, updateJob, runJob, changeJobStatus } from "@/api/monitor/job";
import { ref } from "vue";

const router = useRouter();
const { proxy } = getCurrentInstance();
const { sys_job_group, sys_job_status } = proxy.useDict("sys_job_group", "sys_job_status");

const jobList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const openView = ref(false);
const openCron = ref(false);
const expression = ref("");

const IsAdd=ref(true);

const data = reactive({
  form: {},
  queryParams: {
    skipCount: 1,
    maxResultCount: 10,
    JobId: undefined,
    jobGroup: undefined,
    status: undefined
  },
  rules: {
    jobId: [{ required: true, message: "任务Id不能为空", trigger: "blur" }],
    assemblyName: [{ required: true, message: "Job程序集不能为空", trigger: "blur" }],
    jobType: [{ required: true, message: "Job全类名不能为空", trigger: "blur" }]
  }
});

const { queryParams, form, rules } = toRefs(data);

/** 查询定时任务列表 */
function getList() {
  loading.value = true;
  listJob(queryParams.value).then(response => {
    jobList.value = response.data.items;
    total.value = response.data.totalCount;
    loading.value = false;
  });
}
/** 任务组名字典翻译 */
function jobGroupFormat(row, column) {
  return proxy.selectDictLabel(sys_job_group.value, row.jobGroup);
}
/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}
/** 表单重置 */
function reset() {
  form.value = {
   oldJobId:undefined,
    jobId: undefined,
    groupName: undefined,
    type: undefined,
    cron: undefined,
    millisecond: undefined,
    assemblyName: undefined,
    jobTypeFullName: undefined,
    concurrent: false,
    description:undefined,
   properties:undefined
  };
  IsAdd.value=true;
  proxy.resetForm("jobRef");
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
// 多选框选中数据
function handleSelectionChange(selection) {
  ids.value = selection.map(item => item.jobId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}
// 更多操作触发
function handleCommand(command, row) {
  switch (command) {
    case "handleRun":
      handleRun(row);
      break;
    case "handleView":
      handleView(row);
      break;
    case "handleJobLog":
      handleJobLog(row);
      break;
    default:
      break;
  }
}
// 任务状态修改
function handleStatusChange(row) {
  let text = row.status === "0" ? "启用" : "停用";
  proxy.$modal.confirm('确认要"' + text + '""' + row.jobName + '"任务吗?').then(function () {
    return changeJobStatus(row.jobId, row.status);
  }).then(() => {
    proxy.$modal.msgSuccess(text + "成功");
  }).catch(function () {
    row.status = row.status === "0" ? "1" : "0";
  });
}
/* 立即执行一次 */
function handleRun(row) {
  proxy.$modal.confirm('确认要立即执行一次"' + row.jobId + '"任务吗?').then(function () {
    return runJob(row.jobId, row.jobGroup);
  }).then(() => {
    proxy.$modal.msgSuccess("执行成功");})
  .catch(() => {});
}
/** 任务详细信息 */
function handleView(row) {
  getJob(row.jobId).then(response => {
    form.value = response.data;
    openView.value = true;
  });
}
/** cron表达式按钮操作 */
function handleShowCron() {
  expression.value = form.value.cronExpression;
  openCron.value = true;
}
/** 确定后回传值 */
function crontabFill(value) {
  form.value.cronExpression = value;
}
/** 任务日志列表查询 */
function handleJobLog(row) {
  const jobId = row.jobId || 0;
  router.push({ path: "/monitor/job-log/index", query: { jobId: jobId } });
}
/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加任务";
}
/** 修改按钮操作 */
function handleUpdate(row) {

  reset();
  IsAdd.value=false;
  const jobId = row.jobId || ids.value;

  getJob(jobId).then(response => {
    form.value = response.data;
    form.value.oldJobId=jobId;
    open.value = true;
    title.value = "修改任务";
  });
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs["jobRef"].validate(valid => {
   console.log(valid,"123");
    if (true) {
    
      if (IsAdd.value) {
        addJob(form.value).then(response => {
          proxy.$modal.msgSuccess("新增成功");
          open.value = false;
          getList();
        });
      } else {

        updateJob(form.value.oldJobId,form.value).then(response => {
          proxy.$modal.msgSuccess("修改成功");
          open.value = false;
          getList();
        });


      }
    }
  });
}
/** 删除按钮操作 */
function handleDelete(row) {
  const jobIds = row.jobId || ids.value;
  proxy.$modal.confirm('是否确认删除定时任务编号为"' + jobIds + '"的数据项?').then(function () {
    return delJob(jobIds);
  }).then(() => {
    getList();
    proxy.$modal.msgSuccess("删除成功");
  }).catch(() => {});
}
// /** 导出按钮操作 */
// function handleExport() {
//   proxy.download("monitor/job/export", {
//     ...queryParams.value,
//   }, `job_${new Date().getTime()}.xlsx`);
// }

getList();
</script>
