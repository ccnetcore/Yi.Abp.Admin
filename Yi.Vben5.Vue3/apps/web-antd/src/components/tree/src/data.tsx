import type { VxeGridProps } from '#/adapter/vxe-table';
import type { ID } from '#/api/common';
import type { MenuOption } from '#/api/system/menu/model';

import { h, markRaw } from 'vue';

import { FolderIcon, MenuIcon, OkButtonIcon, VbenIcon } from '@vben/icons';

export interface Permission {
  checked: boolean;
  id: ID;
  label: string;
}

export interface MenuPermissionOption extends MenuOption {
  permissions: Permission[];
}

// （M目录 C菜单 F按钮）
// 支持多种格式的菜单类型值
const menuTypes: Record<string, { icon: ReturnType<typeof markRaw>; value: string }> = {
  c: { icon: markRaw(MenuIcon), value: '菜单' },
  menu: { icon: markRaw(MenuIcon), value: '菜单' },
  Menu: { icon: markRaw(MenuIcon), value: '菜单' },
  catalog: { icon: markRaw(FolderIcon), value: '目录' },
  directory: { icon: markRaw(FolderIcon), value: '目录' },
  folder: { icon: markRaw(FolderIcon), value: '目录' },
  m: { icon: markRaw(FolderIcon), value: '目录' },
  catalogue: { icon: markRaw(FolderIcon), value: '目录' },
  Catalogue: { icon: markRaw(FolderIcon), value: '目录' },
  component: { icon: markRaw(OkButtonIcon), value: '按钮' },
  Component: { icon: markRaw(OkButtonIcon), value: '按钮' },
  f: { icon: markRaw(OkButtonIcon), value: '按钮' },
  button: { icon: markRaw(OkButtonIcon), value: '按钮' },
};

export const nodeOptions = [
  { label: '节点关联', value: true },
  { label: '节点独立', value: false },
];

export const columns: VxeGridProps['columns'] = [
  {
    type: 'checkbox',
    title: '菜单名称',
    field: 'menuName',
    treeNode: true,
    headerAlign: 'left',
    align: 'left',
    width: 230,
  },
  {
    title: '图标',
    field: 'menuIcon',
    width: 80,
    slots: {
      default: ({ row }) => {
        if (row?.menuIcon === '#' || !row?.menuIcon) {
          return '';
        }
        return (
          <span class={'flex justify-center'}>
            <VbenIcon icon={row.menuIcon} />
          </span>
        );
      },
    },
  },
  {
    title: '类型',
    field: 'menuType',
    width: 80,
    slots: {
      default: ({ row }) => {
        const typeKey = `${row.menuType ?? ''}`.toString().trim().toLowerCase();
        const current = menuTypes[typeKey];
        if (!current) {
          return '未知';
        }
        return (
          <span class="flex items-center justify-center gap-1">
            {h(current.icon, { class: 'size-[18px]' })}
            <span>{current.value}</span>
          </span>
        );
      },
    },
  },
  {
    title: '权限标识',
    field: 'permissions',
    headerAlign: 'left',
    align: 'left',
    slots: {
      default: 'permissions',
    },
  },
];
