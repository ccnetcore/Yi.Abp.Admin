import editForm from "../form.vue";
import { message } from "@/utils/message";
import { addDialog } from "@/components/ReDialog";
import { reactive, ref, onMounted, h, toRaw } from "vue";
import type { FormItemProps } from "../utils/types";
import type { PaginationProps } from "@pureadmin/table";
import { deviceDetection } from "@pureadmin/utils";
import {
  addConfig,
  delConfig,
  getConfig,
  getConfigList,
  updateConfig
} from "@/api/system/config";

export function useConfig() {
  const form = reactive({
    configName: "",
    configKey: "",
    configType: ""
  });

  const pagination = reactive<PaginationProps>({
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true
  });

  /** 高亮当前权限选中行 */
  function rowStyle({ row: { id } }) {
    return {
      cursor: "pointer",
      background: id === curRow.value?.id ? "var(--el-fill-color-light)" : ""
    };
  }

  const curRow = ref();
  const formRef = ref();
  const dataList = ref([]);
  const loading = ref(true);

  const columns: TableColumnList = [
    {
      label: "参数主键",
      prop: "id",
      width: 180,
      align: "left"
    },
    {
      label: "参数名称",
      prop: "configName",
      width: 180,
      align: "left"
    },
    {
      label: "参数键名",
      prop: "configKey",
      width: 180,
      align: "left"
    },
    {
      label: "参数键值",
      prop: "configValue",
      width: 180,
      align: "left"
    },
    {
      label: "系统内置",
      prop: "configType",
      width: 180,
      align: "left"
    },
    {
      label: "备注",
      prop: "remark",
      minWidth: 120,
      align: "left"
    },
    {
      label: "创建时间",
      prop: "creationTime",
      width: 180,
      align: "left"
    },
    {
      label: "操作",
      fixed: "right",
      width: 210,
      slot: "operation"
    }
  ];

  function handleSelectionChange(val) {
    console.log("handleSelectionChange", val);
  }

  function resetForm(formEl) {
    if (!formEl) return;
    formEl.resetFields();
    onSearch();
  }

  async function onSearch() {
    loading.value = true;
    const { data } = await getConfigList(toRaw(form));
    dataList.value = data.items;
    pagination.total = data.totalCount;
    loading.value = false;
  }

  async function openDialog(title = "新增", row?: FormItemProps) {
    let data: any = null;
    if (title == "修改") {
      const response = await getConfig(row?.id);
      data = response.data;
    }
    addDialog({
      title: `${title}参数`,
      props: {
        formInline: {
          configName: data?.configName ?? "",
          configKey: data?.configKey ?? "",
          configValue: data?.configValue ?? "",
          configTYpe: data?.configType ?? "",
          remark: data?.remark ?? ""
        }
      },
      width: "40%",
      draggable: true,
      fullscreen: deviceDetection(),
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(editForm, { ref: formRef }),
      beforeSure: (done, { options }) => {
        const FormRef = formRef.value.getRef();
        const curData = options.props.formInline as FormItemProps;
        function chores() {
          message(`您 ${title} 了参数名称为 ${curData.configName} 的这条数据`, {
            type: "success"
          });
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(async valid => {
          if (valid) {
            // 表单规则校验通过
            if (title === "新增") {
              // 实际开发先调用新增接口，再进行下面操作
              console.log("新增参数");
              await addConfig(curData);
              chores();
            } else {
              // 实际开发先调用修改接口，再进行下面操作
              curData.id = row.id;
              curData.creationTime = row.creationTime;
              await updateConfig(row.id, curData);
              chores();
            }
          }
        });
      }
    });
  }

  async function handleDelete(row) {
    await delConfig([row.id]);
    message(`您删除了参数名称为 ${row.configName} 的这条数据`, {
      type: "success"
    });
    onSearch();
  }

  onMounted(() => {
    onSearch();
  });

  return {
    form,
    loading,
    columns,
    dataList,
    /** 搜索 */
    onSearch,
    /** 重置 */
    resetForm,
    /** 新增、修改部门 */
    openDialog,
    /** 删除部门 */
    handleDelete,
    pagination,
    handleSelectionChange,
    rowStyle
  };
}
