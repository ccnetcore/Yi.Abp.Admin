import type { FormSchemaGetter } from '#/adapter/form';
import type { VxeGridProps } from '#/adapter/vxe-table';

import { DictEnum } from '@vben/constants';
import { getPopupContainer } from '@vben/utils';

import { getDictOptions } from '#/utils/dict';
import { renderDict } from '#/utils/render';

export const querySchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    fieldName: 'title',
    label: '公告标题',
  },
  {
    component: 'Input',
    fieldName: 'creatorId',
    label: '创建人',
  },
  {
    component: 'Select',
    componentProps: {
      getPopupContainer,
      options: getDictOptions(DictEnum.SYS_NOTICE_TYPE),
    },
    fieldName: 'type',
    label: '公告类型',
  },
];

export const columns: VxeGridProps['columns'] = [
  { type: 'checkbox', width: 60 },
  {
    title: '公告标题',
    field: 'title',
  },
  {
    title: '公告类型',
    field: 'type',
    width: 120,
    slots: {
      default: ({ row }) => {
        return renderDict(row.type, DictEnum.SYS_NOTICE_TYPE);
      },
    },
  },
  {
    title: '状态',
    field: 'state',
    width: 120,
    slots: {
      default: ({ row }) => {
        return renderDict(row.state ? '1' : '0', DictEnum.SYS_NOTICE_STATUS);
      },
    },
  },
  {
    title: '创建人',
    field: 'creatorId',
    width: 150,
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

export const modalSchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    dependencies: {
      show: () => false,
      triggerFields: [''],
    },
    fieldName: 'id',
    label: '主键',
  },
  {
    component: 'Input',
    fieldName: 'title',
    label: '公告标题',
    rules: 'required',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: getDictOptions(DictEnum.SYS_NOTICE_STATUS),
      optionType: 'button',
    },
    defaultValue: '0',
    fieldName: 'state',
    label: '公告状态',
    rules: 'required',
    formItemClass: 'col-span-1',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: getDictOptions(DictEnum.SYS_NOTICE_TYPE),
      optionType: 'button',
    },
    defaultValue: '1',
    fieldName: 'type',
    label: '公告类型',
    rules: 'required',
    formItemClass: 'col-span-1',
  },
  {
    component: 'RichTextarea',
    componentProps: {
      width: '100%',
    },
    fieldName: 'content',
    label: '公告内容',
    rules: 'required',
  },
];
