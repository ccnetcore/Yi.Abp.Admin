import type { VNode } from 'vue';

import type { FormSchemaGetter } from '#/adapter/form';
import type { VxeGridProps } from '#/adapter/vxe-table';

import { DictEnum } from '@vben/constants';

import { getDictOptions } from '#/utils/dict';
import { renderBrowserIcon, renderDict, renderOsIcon } from '#/utils/render';

export const querySchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    fieldName: 'loginIp',
    label: 'IP地址',
  },
  {
    component: 'Input',
    fieldName: 'loginUser',
    label: '用户账号',
  },
  {
    component: 'RangePicker',
    fieldName: 'dateTime',
    label: '登录日期',
  },
];

export const columns: VxeGridProps['columns'] = [
  { type: 'checkbox', width: 60 },
  {
    title: '用户账号',
    field: 'loginUser',
  },
  {
    title: 'IP地址',
    field: 'loginIp',
  },
  {
    title: 'IP地点',
    field: 'loginLocation',
    width: 200,
  },
  {
    title: '浏览器',
    field: 'browser',
    slots: {
      default: ({ row }) => {
        return renderBrowserIcon(row.browser, true) as VNode;
      },
    },
  },
  {
    title: '系统',
    field: 'os',
    slots: {
      default: ({ row }) => {
        /**
         *  Windows 10 or Windows Server 2016 太长了 分割一下 详情依旧能看到详细的
         */
        let value = row.os;
        if (value) {
          const split = value.split(' or ');
          if (split.length === 2) {
            value = split[0];
          }
        }
        return renderOsIcon(value, true) as VNode;
      },
    },
  },
  {
    title: '信息',
    field: 'logMsg',
  },
  {
    title: '日期',
    field: 'creationTime',
  },
  {
    field: 'action',
    fixed: 'right',
    slots: { default: 'action' },
    title: '操作',
    width: 150,
  },
];
