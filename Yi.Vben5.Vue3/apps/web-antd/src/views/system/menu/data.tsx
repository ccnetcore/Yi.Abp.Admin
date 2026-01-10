import type { FormSchemaGetter } from '#/adapter/form';
import type { VxeGridProps } from '#/adapter/vxe-table';

import { h } from 'vue';

import { DictEnum } from '@vben/constants';
import { FolderIcon, MenuIcon, OkButtonIcon, VbenIcon } from '@vben/icons';
import { $t } from '@vben/locales';
import { getPopupContainer } from '@vben/utils';

import { z } from '#/adapter/form';
import { renderDict } from '#/utils/render';

export const querySchema: FormSchemaGetter = () => [
  {
    component: 'Input',
    fieldName: 'menuName',
    label: '菜单名称 ',
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
    label: '菜单状态 ',
  },
  {
    component: 'Select',
    componentProps: {
      getPopupContainer,
      options: [
        { label: '显示', value: true },
        { label: '隐藏', value: false },
      ],
    },
    fieldName: 'isShow',
    label: '显示状态',
  },
];

// 菜单类型
export const menuTypeOptions = [
  { label: '目录', value: 'Catalogue' },
  { label: '菜单', value: 'Menu' },
  { label: '按钮', value: 'Component' },
];

export const yesNoOptions = [
  { label: '是', value: '0' },
  { label: '否', value: '1' },
];

/**
 * 判断是否为菜单类型（Menu/C）
 */
function isMenuType(menuType: string): boolean {
  const type = menuType?.toLowerCase();
  return type === 'c' || type === 'menu';
}

/**
 * 判断是否为目录类型（Catalogue/M）
 */
function isCatalogueType(menuType: string): boolean {
  const type = menuType?.toLowerCase();
  return type === 'm' || type === 'catalogue' || type === 'catalog' || type === 'directory' || type === 'folder';
}

/**
 * 判断是否为按钮类型（Component/F）
 */
function isComponentType(menuType: string): boolean {
  const type = menuType?.toLowerCase();
  return type === 'f' || type === 'component' || type === 'button';
}

// （M目录 C菜单 F按钮）
const menuTypes: Record<string, { icon: typeof MenuIcon; value: string }> = {
  c: { icon: MenuIcon, value: '菜单' },
  menu: { icon: MenuIcon, value: '菜单' },
  Menu: { icon: MenuIcon, value: '菜单' },
  catalog: { icon: FolderIcon, value: '目录' },
  directory: { icon: FolderIcon, value: '目录' },
  folder: { icon: FolderIcon, value: '目录' },
  m: { icon: FolderIcon, value: '目录' },
  catalogue: { icon: FolderIcon, value: '目录' },
  component: { icon: OkButtonIcon, value: '按钮' },
  f: { icon: OkButtonIcon, value: '按钮' },
  button: { icon: OkButtonIcon, value: '按钮' },
};
export const columns: VxeGridProps['columns'] = [
  {
    title: '菜单名称',
    field: 'menuName',
    treeNode: true,
    width: 200,
    slots: {
      // 需要i18n支持 否则返回原始值
      default: ({ row }) => $t(row.menuName),
    },
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
    title: '排序',
    field: 'orderNum',
    width: 120,
  },
  {
    title: '组件类型',
    field: 'menuType',
    width: 150,
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
    field: 'permissionCode',
  },
  {
    title: '组件路径',
    field: 'component',
  },
  {
    title: '状态',
    field: 'state',
    width: 100,
    slots: {
      default: ({ row }) => {
        return renderDict(String(row.state), DictEnum.SYS_NORMAL_DISABLE);
      },
    },
  },
  {
    title: '显示',
    field: 'isShow',
    width: 100,
    slots: {
      default: ({ row }) => {
        return row.isShow ? '显示' : '隐藏';
      },
    },
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
  },
  {
    component: 'TreeSelect',
    defaultValue: 0,
    fieldName: 'parentId',
    label: '上级菜单',
    rules: 'selectRequired',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: menuTypeOptions,
      optionType: 'button',
    },
    defaultValue: 'M',
    dependencies: {
      componentProps: (_, api) => {
        // 切换时清空校验
        // 直接抄的源码 没有清空校验的方法
        Object.keys(api.errors.value).forEach((key) => {
          api.setFieldError(key, undefined);
        });
        return {};
      },
      triggerFields: ['menuType'],
    },
    fieldName: 'menuType',
    label: '菜单类型',
  },
  {
    component: 'Input',
    dependencies: {
      // 类型不为按钮时显示
      show: (values) => !isComponentType(values.menuType),
      triggerFields: ['menuType'],
    },
    renderComponentContent: (model) => ({
      addonBefore: () => <VbenIcon icon={model.menuIcon} />,
      addonAfter: () => (
        <a href="https://icon-sets.iconify.design/" target="_blank">
          搜索图标
        </a>
      ),
    }),
    fieldName: 'menuIcon',
    help: '点击搜索图标跳转到iconify & 粘贴',
    label: '菜单图标',
  },
  {
    component: 'Input',
    fieldName: 'menuName',
    label: '菜单名称',
    help: '支持i18n写法, 如: menu.system.user',
    rules: 'required',
  },
  {
    component: 'InputNumber',
    fieldName: 'orderNum',
    help: '排序, 数字越小越靠前',
    label: '显示排序',
    defaultValue: 0,
    rules: 'required',
  },
  {
    component: 'Input',
    componentProps: (model) => {
      const placeholder = model.isLink
        ? '填写链接地址http(s)://  使用新页面打开'
        : '填写`路由地址`或者`链接地址`  链接默认使用内部iframe内嵌打开';
      return {
        placeholder,
      };
    },
    dependencies: {
      rules: (model) => {
        if (!model.isLink) {
          return z
            .string({ message: '请输入路由地址' })
            .min(1, '请输入路由地址')
            .refine((val) => !val.startsWith('/'), {
              message: '路由地址不需要带/',
            });
        }
        // 为链接
        return z
          .string({ message: '请输入链接地址' })
          .regex(/^https?:\/\//, { message: '请输入正确的链接地址' });
      },
      // 类型不为按钮时显示
      show: (values) => !isComponentType(values?.menuType),
      triggerFields: ['isLink', 'menuType'],
    },
    fieldName: 'router',
    help: `路由地址不带/, 如: menu, user\n 链接为http(s)://开头\n 链接默认使用内部iframe打开, 可通过{是否外链}控制打开方式`,
    label: '路由地址',
  },
  {
    component: 'Input',
    componentProps: (model) => {
      return {
        // 为链接时组件disabled
        disabled: model.isLink,
      };
    },
    defaultValue: '',
    dependencies: {
      rules: (model) => {
        // 非链接时为必填项
        if (model.router && !/^https?:\/\//.test(model.router)) {
          return z
            .string()
            .min(1, { message: '非链接时必填组件路径' })
            .refine((val) => !val.startsWith('/') && !val.endsWith('/'), {
              message: '组件路径开头/末尾不需要带/',
            });
        }
        // 为链接时非必填
        return z.string().optional();
      },
      // 类型为菜单时显示
      show: (values) => isMenuType(values.menuType),
      triggerFields: ['menuType', 'router'],
    },
    fieldName: 'component',
    help: '填写./src/views下的组件路径, 如system/menu/index',
    label: '组件路径',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: [
        { label: '是', value: true },
        { label: '否', value: false },
      ],
      optionType: 'button',
    },
    defaultValue: false,
    dependencies: {
      // 类型不为按钮时显示
      show: (values) => !isComponentType(values.menuType),
      triggerFields: ['menuType'],
    },
    fieldName: 'isLink',
    help: '外链为http(s)://开头\n 选择否时, 使用iframe从内部打开页面, 否则新窗口打开',
    label: '是否外链',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: [
        { label: '显示', value: true },
        { label: '隐藏', value: false },
      ],
      optionType: 'button',
    },
    defaultValue: true,
    dependencies: {
      // 类型不为按钮时显示
      show: (values) => !isComponentType(values.menuType),
      triggerFields: ['menuType'],
    },
    fieldName: 'isShow',
    help: '隐藏后不会出现在菜单栏, 但仍然可以访问',
    label: '是否显示',
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
    dependencies: {
      // 类型不为按钮时显示
      show: (values) => !isComponentType(values.menuType),
      triggerFields: ['menuType'],
    },
    fieldName: 'state',
    help: '停用后不会出现在菜单栏, 也无法访问',
    label: '菜单状态',
  },
  {
    component: 'Input',
    dependencies: {
      // 类型为菜单/按钮时显示
      show: (values) => !isCatalogueType(values.menuType),
      triggerFields: ['menuType'],
    },
    fieldName: 'permissionCode',
    help: `控制器中定义的权限字符\n 如: @SaCheckPermission("system:user:import")`,
    label: '权限标识',
  },
  {
    component: 'Input',
    componentProps: (model) => ({
      // 为链接时组件disabled
      disabled: model.isLink,
      placeholder: '必须为json字符串格式',
    }),
    dependencies: {
      // 类型为菜单时显示
      show: (values) => isMenuType(values.menuType),
      triggerFields: ['menuType'],
    },
    fieldName: 'query',
    help: 'vue-router中的query属性\n 如{"name": "xxx", "age": 16}',
    label: '路由参数',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: [
        { label: '是', value: true },
        { label: '否', value: false },
      ],
      optionType: 'button',
    },
    defaultValue: false,
    dependencies: {
      // 类型为菜单时显示
      show: (values) => isMenuType(values.menuType),
      triggerFields: ['menuType'],
    },
    fieldName: 'isCache',
    help: '路由的keepAlive属性',
    label: '是否缓存',
  },
];
