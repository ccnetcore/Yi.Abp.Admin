import dayjs from "dayjs";
import { message } from "@/utils/message";
import { getKeyList } from "@pureadmin/utils";
import { usePublicHooks } from "@/views/system/hooks";
import type { PaginationProps } from "@pureadmin/table";
import { type Ref, reactive, ref, onMounted, toRaw } from "vue";
import { getLoginLoglist } from "@/api/log/loginLog";

export function useRole(tableRef: Ref) {
  const form = reactive({
    loginUser: "",
    state: "",
    creationTime: [],
    skipCount: 1,
    maxResultCount: 10
  });
  const dataList = ref([]);
  const loading = ref(true);
  const selectedNum = ref(0);
  const { tagStyle } = usePublicHooks();

  const pagination = reactive<PaginationProps>({
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true
  });
  const columns: TableColumnList = [
    {
      label: "勾选列", // 如果需要表格多选，此处label必须设置
      type: "selection",
      fixed: "left",
      reserveSelection: true // 数据刷新后保留选项
    },
    {
      label: "序号",
      prop: "id",
      minWidth: 90
    },
    {
      label: "登录用户",
      prop: "loginUser",
      minWidth: 100
    },
    {
      label: "登录 IP",
      prop: "loginIp",
      minWidth: 140
    },
    {
      label: "登录地点",
      prop: "loginLocation",
      minWidth: 140
    },
    {
      label: "操作系统",
      prop: "os",
      minWidth: 100
    },
    {
      label: "浏览器类型",
      prop: "browser",
      minWidth: 100
    },
    {
      label: "登录状态",
      prop: "state",
      minWidth: 100,
      cellRenderer: ({ props }) => (
        <el-tag size={props.size} style={tagStyle.value(1)}>
          {1 === 1 ? "成功" : "失败"}
        </el-tag>
      )
    },
    {
      label: "登录行为",
      prop: "logMsg",
      minWidth: 100
    },
    {
      label: "登录时间",
      prop: "creationTime",
      minWidth: 180,
      formatter: ({ creationTime }) =>
        dayjs(creationTime).format("YYYY-MM-DD HH:mm:ss")
    }
  ];
  function handleSizeChange(val: number) {
    form.maxResultCount = val;
    onSearch();
  }

  function handleCurrentChange(val: number) {
    form.skipCount = val;
    onSearch();
  }

  /** 当CheckBox选择项发生变化时会触发该事件 */
  function handleSelectionChange(val) {
    selectedNum.value = val.length;
    // 重置表格高度
    tableRef.value.setAdaptive();
  }

  /** 取消选择 */
  function onSelectionCancel() {
    selectedNum.value = 0;
    // 用于多选表格，清空用户的选择
    tableRef.value.getTableRef().clearSelection();
  }

  /** 批量删除 */
  function onbatchDel() {
    // 返回当前选中的行
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    // 接下来根据实际业务，通过选中行的某项数据，比如下面的id，调用接口进行批量删除
    message(`已删除序号为 ${getKeyList(curSelected, "id")} 的数据`, {
      type: "success"
    });
    tableRef.value.getTableRef().clearSelection();
    onSearch();
  }

  /** 清空日志 */
  function clearAll() {
    // 根据实际业务，调用接口删除所有日志数据
    message("已删除所有日志数据", {
      type: "success"
    });
    onSearch();
  }

  async function onSearch() {
    loading.value = true;
    const { data } = await getLoginLoglist(
      toRaw({
        ...form,
        startTime: form.creationTime[0],
        endTime: form.creationTime[1]
      })
    );
    dataList.value = data.items;
    pagination.total = data.totalCount;
    loading.value = false;
  }

  const resetForm = formEl => {
    if (!formEl) return;
    formEl.resetFields();
    onSearch();
  };

  onMounted(() => {
    onSearch();
  });

  return {
    form,
    loading,
    columns,
    dataList,
    pagination,
    selectedNum,
    onSearch,
    clearAll,
    resetForm,
    onbatchDel,
    handleSizeChange,
    onSelectionCancel,
    handleCurrentChange,
    handleSelectionChange
  };
}
