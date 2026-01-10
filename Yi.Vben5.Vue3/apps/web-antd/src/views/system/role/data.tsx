import type { FormSchemaGetter } from '#/adapter/form';
import type { VxeGridProps } from '#/adapter/vxe-table';

import { getPopupContainer } from '@vben/utils';

import { Tag } from 'ant-design-vue';

/**
 * authScopeOptions user也会用到
 */
export const authScopeOptions = [
  { color: 'green', label: '全部数据权限', value: 'ALL' },
  { color: 'default', label: '自定数据权限', value: 'CUSTOM' },
  { color: 'orange', label: '本部门数据权限', value: 'DEPT' },
  { color: 'cyan', label: '本部门及以下数据权限', value: 'DEPT_FOLLOW' },
  { color: 'error', label: '仅本人数据权限', value: 'USER' },
];

export const querySchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    fieldName: 'roleName',
    label: '角色名称',
  },
  {
    component: 'Input',
    fieldName: 'roleCode',
    label: '角色编码',
  },
  {
    component: 'Select',
    componentProps: {
      getPopupContainer,
      options: [
        { label: '启用', value: true },
        { label: '禁用', value: false },
      ],
    },
    fieldName: 'state',
    label: '状态',
  },
  {
    component: 'RangePicker',
    fieldName: 'creationTime',
    label: '创建时间',
  },
];

export const columns: VxeGridProps['columns'] = [
  { type: 'checkbox', width: 60 },
  {
    title: '角色名称',
    field: 'roleName',
  },
  {
    title: '角色编码',
    field: 'roleCode',
    slots: {
      default: ({ row }) => {
        return <Tag color="processing">{row.roleCode}</Tag>;
      },
    },
  },
  {
    title: '数据权限',
    field: 'dataScope',
    slots: {
      default: ({ row }) => {
        const found = authScopeOptions.find(
          (item) => item.value === row.dataScope,
        );
        if (found) {
          return <Tag color={found.color}>{found.label}</Tag>;
        }
        return <Tag>{row.dataScope}</Tag>;
      },
    },
  },
  {
    title: '排序',
    field: 'orderNum',
  },
  {
    title: '状态',
    field: 'state',
    slots: { default: 'status' },
  },
  {
    title: '创建时间',
    field: 'creationTime',
  },
  {
    field: 'action',
    fixed: 'right',
    slots: { default: 'action' },
    title: '操作',
    resizable: false,
    width: 'auto',
  },
];

export const drawerSchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    dependencies: {
      show: () => false,
      triggerFields: [''],
    },
    fieldName: 'id',
    label: '角色ID',
  },
  {
    component: 'Input',
    fieldName: 'roleName',
    label: '角色名称',
    rules: 'required',
  },
  {
    component: 'Input',
    fieldName: 'roleCode',
    help: '如: admin, user 等',
    label: '角色编码',
    rules: 'required',
  },
  {
    component: 'InputNumber',
    fieldName: 'orderNum',
    label: '排序',
    rules: 'required',
    defaultValue: 0,
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: [
        { label: '启用', value: true },
        { label: '禁用', value: false },
      ],
      optionType: 'button',
    },
    defaultValue: true,
    fieldName: 'state',
    label: '状态',
  },
  {
    component: 'Input',
    defaultValue: [],
    fieldName: 'menuIds',
    label: '菜单权限',
    formItemClass: 'col-span-2',
  },
  {
    component: 'Textarea',
    defaultValue: '',
    fieldName: 'remark',
    formItemClass: 'col-span-2',
    label: '备注',
  },
];

export const authModalSchemas: FormSchemaGetter = () => [
  {
    component: 'Input',
    dependencies: {
      show: () => false,
      triggerFields: [''],
    },
    fieldName: 'id',
    label: '角色ID',
  },
  {
    component: 'Input',
    componentProps: {
      disabled: true,
    },
    fieldName: 'roleName',
    label: '角色名称',
  },
  {
    component: 'Input',
    componentProps: {
      disabled: true,
    },
    fieldName: 'roleCode',
    label: '角色编码',
  },
  {
    component: 'Select',
    componentProps: {
      allowClear: false,
      getPopupContainer,
      options: authScopeOptions,
    },
    fieldName: 'dataScope',
    help: '更改后需要用户重新登录才能生效',
    label: '权限范围',
  },
  {
    component: 'TreeSelect',
    defaultValue: [],
    dependencies: {
      show: (values) => values.dataScope === 'CUSTOM',
      triggerFields: ['dataScope'],
    },
    fieldName: 'deptIds',
    help: '更改后立即生效',
    label: '部门权限',
  },
];
