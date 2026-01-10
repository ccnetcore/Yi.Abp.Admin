import type { Menu, MenuOption, MenuQuery, MenuResp } from './model';

import type { ID, IDS } from '#/api/common';

import { requestClient } from '#/api/request';

enum Api {
  menuList = '/menu/list',
  menuTreeSelect = '/menu/tree',
  root = '/menu',
  tenantPackageMenuTreeselect = '/menu/tenantPackageMenuTreeselect',
}

/**
 * 菜单列表
 * @param params 参数
 * @returns 列表
 */
export function menuList(params?: MenuQuery) {
  return requestClient.get<Menu[]>(Api.menuList, { params });
}

/**
 * 菜单详情
 * @param menuId 菜单id
 * @returns 菜单详情
 */
export function menuInfo(menuId: ID) {
  return requestClient.get<Menu>(`${Api.root}/${menuId}`);
}

/**
 * 菜单新增
 * @param data 参数
 */
export function menuAdd(data: Partial<Menu>) {
  return requestClient.postWithMsg<void>(Api.root, data);
}

/**
 * 菜单更新
 * @param data 参数
 */
export function menuUpdate(data: Partial<Menu>) {
  return requestClient.putWithMsg<void>(`${Api.root}/${data.id}`, data);
}

/**
 * 菜单删除
 * @param menuIds ids
 */
export function menuRemove(menuIds: IDS) {
  return requestClient.deleteWithMsg<void>(Api.root, {
    params: { ids: menuIds.join(',') },
  });
}

/**
 * 下拉框使用  返回所有的菜单
 * @returns []
 */
export function menuTreeSelect() {
  return requestClient.get<MenuOption[]>(Api.menuTreeSelect);
}

/**
 * 租户套餐使用
 * @param packageId packageId
 * @returns resp
 */
export function tenantPackageMenuTreeSelect(packageId: ID) {
  return requestClient.get<MenuResp>(
    `${Api.tenantPackageMenuTreeselect}/${packageId}`,
  );
}

/**
 * 批量删除菜单
 * @param menuIds 菜单ids
 * @returns void
 */
export function menuCascadeRemove(menuIds: IDS) {
  return requestClient.deleteWithMsg<void>(`${Api.root}/cascade/${menuIds}`);
}
