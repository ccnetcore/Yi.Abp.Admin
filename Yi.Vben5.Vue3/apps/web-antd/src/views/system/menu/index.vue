<script setup lang="ts">
import type { VbenFormProps } from '@vben/common-ui';

import type { VxeGridProps } from '#/adapter/vxe-table';
import type { Menu } from '#/api/system/menu/model';

import { computed, ref } from 'vue';

import { useAccess } from '@vben/access';
import { Fallback, Page, useVbenDrawer } from '@vben/common-ui';
import { $t } from '@vben/locales';
import { eachTree, getVxePopupContainer, treeToList } from '@vben/utils';

import { Popconfirm, Space, Switch, Tooltip } from 'ant-design-vue';

import { useVbenVxeGrid } from '#/adapter/vxe-table';
import { menuCascadeRemove, menuList, menuRemove } from '#/api/system/menu';

import { columns, querySchema } from './data';
import menuDrawer from './menu-drawer.vue';

// 空GUID，用于判断根节点
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

type MenuRow = Omit<Menu, 'parentId'> & {
  menuId: string;
  parentId: string | null;
};

const formOptions: VbenFormProps = {
  commonConfig: {
    labelWidth: 80,
    componentProps: {
      allowClear: true,
    },
  },
  schema: querySchema(),
  wrapperClass: 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4',
};

const gridOptions: VxeGridProps<Record<string, any>> = {
  columns,
  height: 'auto',
  keepSource: true,
  pagerConfig: {
    enabled: false,
  },
  proxyConfig: {
    ajax: {
      query: async (_, formValues = {}) => {
        const resp = await menuList({
          ...formValues,
        });
        // 统一处理数据：确保 menuId 和 parentId 存在，并将根节点的 parentId 置为 null
        const items = (resp || []).map((item) => {
          const menuId = String(item.id ?? '');
          const parentId = item.parentId ? String(item.parentId) : null;
          return {
            ...item,
            menuId,
            // 将根节点的 parentId 置为 null，以便 vxe-table 正确识别根节点
            parentId:
              !parentId ||
              parentId === EMPTY_GUID ||
              parentId === menuId
                ? null
                : parentId,
          } as MenuRow;
        });
        return { items };
      },
    },
  },
  rowConfig: {
    keyField: 'menuId',
  },
  /**
   * 开启虚拟滚动
   * 数据量小可以选择关闭
   * 如果遇到样式问题(空白、错位 滚动等)可以选择关闭虚拟滚动
   */
  scrollY: {
    enabled: true,
    gt: 0,
  },
  treeConfig: {
    parentField: 'parentId',
    rowField: 'menuId',
    transform: true,
  },
  id: 'system-menu-index',
};
// @ts-expect-error TS2589: MenuRow 与 proxyConfig 组合导致类型实例化层级过深；运行时泛型已被擦除，可控，先压制报错。
const [BasicTable, tableApi] = useVbenVxeGrid({
  formOptions,
  gridOptions,
  gridEvents: {
    cellDblclick: (e) => {
      const { row = {} } = e;
      if (!row?.children) {
        return;
      }
      const isExpanded = row?.expand;
      tableApi.grid.setTreeExpand(row, !isExpanded);
      row.expand = !isExpanded;
    },
    // 需要监听使用箭头展开的情况 否则展开/折叠的数据不一致
    toggleTreeExpand: (e) => {
      const { row = {}, expanded } = e;
      row.expand = expanded;
    },
  },
});
const [MenuDrawer, drawerApi] = useVbenDrawer({
  connectedComponent: menuDrawer,
});

function handleAdd() {
  drawerApi.setData({});
  drawerApi.open();
}

function handleSubAdd(row: MenuRow) {
  const { menuId } = row;
  drawerApi.setData({ id: menuId, update: false });
  drawerApi.open();
}

async function handleEdit(record: MenuRow) {
  drawerApi.setData({ id: record.menuId, update: true });
  drawerApi.open();
}

/**
 * 是否级联删除
 */
const cascadingDeletion = ref(false);
async function handleDelete(row: MenuRow) {
  if (cascadingDeletion.value) {
    // 级联删除
    const menuAndChildren = treeToList<MenuRow[]>([row], { id: 'menuId' });
    const menuIds = menuAndChildren.map((item) => String(item.menuId));
    await menuCascadeRemove(menuIds);
  } else {
    // 单删除
    await menuRemove([String(row.menuId)]);
  }
  await tableApi.query();
}

function removeConfirmTitle(row: MenuRow) {
  const menuName = $t(row.menuName);
  if (!cascadingDeletion.value) {
    return `是否确认删除 [${menuName}] ?`;
  }
  const menuAndChildren = treeToList<MenuRow[]>([row], { id: 'menuId' });
  if (menuAndChildren.length === 1) {
    return `是否确认删除 [${menuName}] ?`;
  }
  return `是否确认删除 [${menuName}] 及 [${menuAndChildren.length - 1}]个子项目 ?`;
}

/**
 * 编辑/添加成功后刷新表格
 */
async function afterEditOrAdd() {
  tableApi.query();
}

/**
 * 全部展开/折叠
 * @param expand 是否展开
 */
function setExpandOrCollapse(expand: boolean) {
  eachTree(tableApi.grid.getData(), (item) => (item.expand = expand));
  tableApi.grid?.setAllTreeExpand(expand);
}

/**
 * 与后台逻辑相同
 * 只有租户管理和超级管理能访问菜单管理
 * 注意: 只有超管才能对菜单进行`增删改`操作
 * 注意: 只有超管才能对菜单进行`增删改`操作
 * 注意: 只有超管才能对菜单进行`增删改`操作
 */
const { hasAccessByRoles } = useAccess();
const isAdmin = computed(() => {
  return hasAccessByRoles(['admin', 'superadmin']);
});
</script>

<template>
  <Page v-if="isAdmin" :auto-content-height="true">
    <BasicTable table-title="菜单列表" table-title-help="双击展开/收起子菜单">
      <template #toolbar-tools>
        <Space>
          <Tooltip title="删除菜单以及子菜单">
            <div
              v-access:role="['superadmin']"
              v-access:code="['system:menu:remove']"
              class="flex items-center"
            >
              <span class="mr-2 text-sm text-[#666666]">级联删除</span>
              <Switch v-model:checked="cascadingDeletion" />
            </div>
          </Tooltip>
          <a-button @click="setExpandOrCollapse(false)">
            {{ $t('pages.common.collapse') }}
          </a-button>
          <a-button @click="setExpandOrCollapse(true)">
            {{ $t('pages.common.expand') }}
          </a-button>
          <a-button
            type="primary"
            v-access:code="['system:menu:add']"
            v-access:role="['superadmin']"
            @click="handleAdd"
          >
            {{ $t('pages.common.add') }}
          </a-button>
        </Space>
      </template>
      <template #action="{ row }">
        <Space>
          <ghost-button
            v-access:code="['system:menu:edit']"
            v-access:role="['superadmin']"
            @click="handleEdit(row)"
          >
            {{ $t('pages.common.edit') }}
          </ghost-button>
          <!-- '按钮类型'无法再添加子菜单 -->
          <ghost-button
            v-if="row.menuType !== 'F'"
            class="btn-success"
            v-access:code="['system:menu:add']"
            v-access:role="['superadmin']"
            @click="handleSubAdd(row)"
          >
            {{ $t('pages.common.add') }}
          </ghost-button>
          <Popconfirm
            :get-popup-container="getVxePopupContainer"
            placement="left"
            :title="removeConfirmTitle(row)"
            @confirm="handleDelete(row)"
          >
            <ghost-button
              danger
              v-access:code="['system:menu:remove']"
              v-access:role="['superadmin']"
              @click.stop=""
            >
              {{ $t('pages.common.delete') }}
            </ghost-button>
          </Popconfirm>
        </Space>
      </template>
    </BasicTable>
    <MenuDrawer @reload="afterEditOrAdd" />
  </Page>
  <Fallback v-else description="您没有菜单管理的访问权限" status="403" />
</template>
