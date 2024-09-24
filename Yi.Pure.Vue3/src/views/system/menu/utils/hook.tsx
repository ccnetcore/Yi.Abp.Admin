import editForm from "../form.vue";
import { handleTree } from "@/utils/tree";
import { message } from "@/utils/message";
import {
  addMenu,
  delMenu,
  getListMenu,
  getMenu,
  updateMenu
} from "@/api/system/menu";
import { transformI18n } from "@/plugins/i18n";
import { addDialog } from "@/components/ReDialog";
import { reactive, ref, onMounted, h } from "vue";
import type { FormItemProps } from "../utils/types";
import { useRenderIcon } from "@/components/ReIcon/src/hooks";
import { cloneDeep, isAllEmpty, deviceDetection } from "@pureadmin/utils";
import { menuTypeOptions } from "@/views/system/menu/utils/enums";

export function useMenu() {
  const form = reactive({
    menuName: "",
    menuSource: "Pure"
  });

  const formRef = ref();
  const dataList = ref([]);
  const loading = ref(true);

  const getMenuType = (type: string, text = false) => {
    switch (type) {
      case "Catalogue":
        return text ? "目录" : "primary";
      case "Menu":
        return text ? "菜单" : "warning";
      case "Component":
        return text ? "组件" : "info";
    }
  };

  const columns: TableColumnList = [
    {
      label: "菜单名称",
      prop: "menuName",
      align: "left",
      cellRenderer: ({ row }) => (
        <>
          <span class="inline-block mr-1">
            {h(useRenderIcon(row.menuIcon), {
              style: { paddingTop: "1px" }
            })}
          </span>
          <span>{transformI18n(row.menuName)}</span>
        </>
      )
    },
    {
      label: "菜单类型",
      prop: "menuType",
      width: 100,
      cellRenderer: ({ row, props }) => (
        <el-tag
          size={props.size}
          type={getMenuType(row.menuType)}
          effect="plain"
        >
          {getMenuType(row.menuType, true)}
        </el-tag>
      )
    },
    {
      label: "路由路径",
      prop: "router"
    },
    {
      label: "组件路径",
      prop: "component",
      formatter: ({ path, component }) =>
        isAllEmpty(component) ? path : component
    },
    {
      label: "权限标识",
      prop: "permissionCode"
    },
    {
      label: "排序",
      prop: "orderNum",
      width: 100
    },
    {
      label: "显示",
      prop: "isShow",
      formatter: ({ isShow }) => (isShow ? "否" : "是"),
      width: 100
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
    const data = (await getListMenu(form)).data.items; // 这里是返回一维数组结构，前端自行处理成树结构，返回格式要求：唯一id加父节点parentId，parentId取父节点id
    let newData = data;
    if (!isAllEmpty(form.menuName)) {
      // 前端搜索菜单名称
      newData = newData.filter(item =>
        transformI18n(item.title).includes(form.menuName)
      );
    }
    dataList.value = handleTree(newData); // 处理成树结构
    loading.value = false;
  }

  function formatHigherMenuOptions(treeList) {
    if (!treeList || !treeList.length) return;
    const newTreeList = [];
    for (let i = 0; i < treeList.length; i++) {
      treeList[i].title = transformI18n(treeList[i].title);
      formatHigherMenuOptions(treeList[i].children);
      newTreeList.push(treeList[i]);
    }
    return newTreeList;
  }

  async function openDialog(title = "新增", row?: FormItemProps) {
    let data: any = null;
    if (title == "修改") {
      data = (await getMenu(row.id)).data;
    }
    addDialog({
      title: `${title}菜单`,
      props: {
        formInline: {
          menuName: row?.menuName ?? "",
          menuType:
            data?.menuType == undefined
              ? 0
              : menuTypeOptions.findIndex(
                  option => option.value === row?.menuType
                ),
          higherMenuOptions: formatHigherMenuOptions(cloneDeep(dataList.value)),
          id: data?.id ?? "",
          parentId: data?.parentId ?? 0,
          router: data?.router ?? "",
          component: data?.component ?? "",
          orderNum: data?.orderNum ?? 0,
          menuIcon: data?.menuIcon ?? "",
          permissionCode: data?.permissionCode ?? "",
          showLink: data?.isShow ?? true,
          isLink: data?.isLink ?? false,
          state: data?.state ?? true,
          routerName: data?.routerName ?? "",
          menuSource: data?.menuSource ?? "Pure"
        }
      },
      width: "45%",
      draggable: true,
      fullscreen: deviceDetection(),
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(editForm, { ref: formRef }),
      beforeSure: (done, { options }) => {
        const FormRef = formRef.value.getRef();
        const curData = options.props.formInline as FormItemProps;
        function chores() {
          message(
            `您${title}了菜单名称为${transformI18n(curData.menuName)}的这条数据`,
            {
              type: "success"
            }
          );
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(async valid => {
          if (valid) {
            // 表单规则校验通过
            if (title === "新增") {
              // 实际开发先调用新增接口，再进行下面操作
              await addMenu(curData);
              chores();
            } else {
              // 实际开发先调用修改接口，再进行下面操作
              await updateMenu(row.id, curData);
              chores();
            }
          }
        });
      }
    });
  }

  async function handleDelete(row) {
    await delMenu([row.id]);
    message(`您删除了菜单名称为${transformI18n(row.menuName)}的这条数据`, {
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
    /** 新增、修改菜单 */
    openDialog,
    /** 删除菜单 */
    handleDelete,
    handleSelectionChange
  };
}
