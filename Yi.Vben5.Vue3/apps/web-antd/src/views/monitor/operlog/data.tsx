import type { FormSchemaGetter } from '#/adapter/form';
import type { VxeGridProps } from '#/adapter/vxe-table';

import { DictEnum } from '@vben/constants';

import { getDictOptions } from '#/utils/dict';
import { renderDict } from '#/utils/render';

export const querySchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    fieldName: 'title',
    label: '系统模块',
  },
  {
    component: 'Input',
    fieldName: 'operUser',
    label: '操作人员',
  },
  {
    component: 'Select',
    componentProps: {
      options: getDictOptions(DictEnum.SYS_OPER_TYPE),
    },
    fieldName: 'operType',
    label: '操作类型',
  },
  {
    component: 'Input',
    fieldName: 'operIp',
    label: '操作IP',
  },
  {
    component: 'RangePicker',
    fieldName: 'createTime',
    label: '操作时间',
    componentProps: {
      valueFormat: 'YYYY-MM-DD HH:mm:ss',
    },
  },
];

export const columns: VxeGridProps['columns'] = [
  { type: 'checkbox', width: 60 },
  { field: 'title', title: '系统模块' },
  {
    title: '操作类型',
    field: 'operType',
    slots: {
      default: ({ row }) => {
        return renderDict(row.operType, DictEnum.SYS_OPER_TYPE);
      },
    },
  },
  { field: 'operUser', title: '操作人员' },
  { field: 'operIp', title: 'IP地址' },
  { field: 'operLocation', title: 'IP信息' },
  { field: 'creationTime', title: '操作日期', sortable: true },
  {
    field: 'action',
    fixed: 'right',
    slots: { default: 'action' },
    title: '操作',
    resizable: false,
    width: 'auto',
  },
];
