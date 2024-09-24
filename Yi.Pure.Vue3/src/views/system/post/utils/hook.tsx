import dayjs from "dayjs";
import editForm from "../form.vue";
import { message } from "@/utils/message";
import { ElMessageBox } from "element-plus";
import { usePublicHooks } from "../../hooks";
import { transformI18n } from "@/plugins/i18n";
import { addDialog } from "@/components/ReDialog";
import type { FormItemProps } from "../utils/types";
import type { PaginationProps } from "@pureadmin/table";
import { deviceDetection } from "@pureadmin/utils";
import {
  getPostList,
  addPost,
  updatePost,
  delPost,
  getPost,
  updatePostStatus
} from "@/api/system/post";

import { reactive, ref, onMounted, h, toRaw } from "vue";

export function usePost() {
  const form = reactive({
    postName: "",
    postCode: "",
    state: true,
    skipCount: 1,
    maxResultCount: 10
  });
  const curRow = ref();
  const formRef = ref();
  const dataList = ref([]);
  const treeData = ref([]);
  const isShow = ref(false);
  const loading = ref(true);
  const isLinkage = ref(false);
  const treeSearchValue = ref();
  const switchLoadMap = ref({});
  const isExpandAll = ref(false);
  const isSelectAll = ref(false);
  const { switchStyle } = usePublicHooks();
  const pagination = reactive<PaginationProps>({
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true
  });
  const columns: TableColumnList = [
    {
      label: "岗位编号",
      prop: "id",
      width: 300
    },
    {
      label: "岗位名称",
      prop: "postName"
    },
    {
      label: "岗位标识",
      prop: "postCode"
    },
    {
      label: "状态",
      cellRenderer: scope => (
        <el-switch
          size={scope.props.size === "small" ? "small" : "default"}
          loading={switchLoadMap.value[scope.index]?.loading}
          v-model={scope.row.state}
          active-value={true}
          inactive-value={false}
          active-text="已启用"
          inactive-text="已停用"
          inline-prompt
          style={switchStyle.value}
          onChange={() => onChange(scope as any)}
        />
      ),
      minWidth: 90
    },
    { label: "排序", prop: "orderNum" },
    {
      label: "备注",
      prop: "remark",
      minWidth: 160
    },
    {
      label: "创建时间",
      prop: "creationTime",
      minWidth: 160,
      formatter: ({ creationTime }) =>
        dayjs(creationTime).format("YYYY-MM-DD HH:mm:ss")
    },
    {
      label: "操作",
      fixed: "right",
      width: 210,
      slot: "operation"
    }
  ];

  async function onChange({ row, index }) {
    ElMessageBox.confirm(
      `确认要<strong>${
        row.state === false ? "停用" : "启用"
      }</strong><strong style='color:var(--el-color-primary)'>${
        row.roleName
      }</strong>吗?`,
      "系统提示",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
        dangerouslyUseHTMLString: true,
        draggable: true
      }
    )
      .then(async () => {
        switchLoadMap.value[index] = Object.assign(
          {},
          switchLoadMap.value[index],
          {
            loading: true
          }
        );

        await updatePostStatus(row.id, row.state);

        switchLoadMap.value[index] = Object.assign(
          {},
          switchLoadMap.value[index],
          {
            loading: false
          }
        );
        message(`已${row.state === false ? "停用" : "启用"}${row.roleName}`, {
          type: "success"
        });
      })
      .catch(() => {
        row.state === false ? (row.state = true) : (row.state = false);
      });
  }

  async function handleDelete(row) {
    await delPost([row.id]);
    message(`您删除了角色名称为${row.roleName}的这条数据`, { type: "success" });
    onSearch();
  }

  function handleSizeChange(val: number) {
    form.maxResultCount = val;
    onSearch();
  }

  function handleCurrentChange(val: number) {
    form.skipCount = val;
    onSearch();
  }

  function handleSelectionChange(val) {
    console.log("handleSelectionChange", val);
  }

  async function onSearch() {
    loading.value = true;
    const { data } = await getPostList(toRaw(form));
    dataList.value = data.items;
    pagination.total = data.totalCount;
    loading.value = false;
  }

  const resetForm = formEl => {
    if (!formEl) return;
    formEl.resetFields();
    onSearch();
  };

  async function openDialog(title = "新增", row?: FormItemProps) {
    let data: any = null;
    if (title == "修改") {
      data = (await getPost(row?.id)).data;
    }
    addDialog({
      title: `${title}岗位`,
      props: {
        formInline: {
          postName: row?.postName ?? "",
          postCode: row?.postCode ?? "",
          remark: row?.remark ?? "",
          orderNum: data?.orderNum ?? 0
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
          message(`您${title}了岗位名称为${curData.postName}的这条数据`, {
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
              await addPost(curData);
              chores();
            } else {
              // 实际开发先调用修改接口，再进行下面操作
              await updatePost(row?.id, curData);
              chores();
            }
          }
        });
      }
    });
  }

  /** 高亮当前权限选中行 */
  function rowStyle({ row: { id } }) {
    return {
      cursor: "pointer",
      background: id === curRow.value?.id ? "var(--el-fill-color-light)" : ""
    };
  }

  /** 数据权限 可自行开发 */
  // function handleDatabase() {}

  const filterMethod = (query: string, node) => {
    return transformI18n(node.title)!.includes(query);
  };

  onMounted(async () => {
    onSearch();
  });

  return {
    form,
    isShow,
    curRow,
    loading,
    columns,
    rowStyle,
    dataList,
    treeData,
    isLinkage,
    pagination,
    isExpandAll,
    isSelectAll,
    treeSearchValue,
    onSearch,
    resetForm,
    openDialog,
    handleDelete,
    filterMethod,
    transformI18n,
    handleSizeChange,
    handleCurrentChange,
    handleSelectionChange
  };
}
