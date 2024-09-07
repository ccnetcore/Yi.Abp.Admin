import dayjs from "dayjs";
import { message } from "@/utils/message";
import { reactive, ref, onMounted, toRaw } from "vue";
import type { PaginationProps } from "@pureadmin/table";
import { forceLogout, getOnlineList } from "@/api/monitor/online";

export function useRole() {
  const form = reactive({
    userName: "",
    skipCount: 1,
    maxResultCount: 10
  });
  const dataList = ref([]);
  const loading = ref(true);
  const pagination = reactive<PaginationProps>({
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true
  });
  const columns: TableColumnList = [
    {
      label: "序号",
      prop: "connnectionId",
      minWidth: 200
    },
    {
      label: "用户名",
      prop: "userName",
      minWidth: 100
    },
    {
      label: "登录 IP",
      prop: "ipaddr",
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
      label: "登录时间",
      prop: "creationTime",
      minWidth: 180,
      formatter: ({ creationTime }) =>
        dayjs(creationTime).format("YYYY-MM-DD HH:mm:ss")
    },
    {
      label: "操作",
      fixed: "right",
      slot: "operation"
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
    console.log(val);
  }

  async function handleOffline(row) {
    await forceLogout(row.connnectionId);
    message(`${row.userName}已被强制下线`, { type: "success" });
    onSearch();
  }

  async function onSearch() {
    loading.value = true;
    const { data } = await getOnlineList(toRaw(form));
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
    onSearch,
    resetForm,
    handleOffline,
    handleSizeChange,
    handleCurrentChange,
    handleSelectionChange
  };
}
