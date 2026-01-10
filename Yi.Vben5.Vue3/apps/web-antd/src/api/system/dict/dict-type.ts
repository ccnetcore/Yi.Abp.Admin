import type { DictType } from './dict-type-model';

import type { ID, IDS, PageQuery, PageResult } from '#/api/common';

import { commonExport } from '#/api/helper';
import { requestClient } from '#/api/request';

enum Api {
  dictOptionSelectList = '/dictionary-type/select-data-list',
  dictTypeExport = '/dictionary-type/export',
  dictTypeList = '/dictionary-type/list',
  dictTypeRefreshCache = '/dictionary-type/refreshCache',
  root = '/dictionary-type',
}

/**
 * 获取字典类型列表
 * @param params 请求参数
 * @returns list
 */
export function dictTypeList(params?: PageQuery) {
  return requestClient.get<PageResult<DictType>>(Api.root, { params });
}

/**
 * 导出字典类型列表
 * @param data 表单参数
 * @returns blob
 */
export function dictTypeExport(data: Partial<DictType>) {
  return commonExport(Api.dictTypeExport, data);
}

/**
 * 删除字典类型
 * @param dictIds 字典类型id数组
 * @returns void
 */
export function dictTypeRemove(dictIds: IDS) {
  return requestClient.deleteWithMsg<void>(Api.root, {
    params: { ids: dictIds.join(',') },
  });
}

/**
 * 刷新字典缓存
 * @returns void
 */
export function refreshDictTypeCache() {
  return requestClient.deleteWithMsg<void>(Api.dictTypeRefreshCache);
}

/**
 * 新增
 * @param data 表单参数
 * @returns void
 */
export function dictTypeAdd(data: Partial<DictType>) {
  return requestClient.postWithMsg<void>(Api.root, data);
}

/**
 * 修改
 * @param data 表单参数
 * @returns void
 */
export function dictTypeUpdate(data: Partial<DictType>) {
  return requestClient.putWithMsg<void>(`${Api.root}/${data.id}`, data);
}

/**
 * 查询详情
 * @param dictId 字典类型id
 * @returns 信息
 */
export function dictTypeInfo(dictId: ID) {
  return requestClient.get<DictType>(`${Api.root}/${dictId}`);
}

/**
 * 这个在ele用到 v5用不上
 * 下拉框  返回值和list一样
 * @returns options
 */
export function dictOptionSelectList() {
  return requestClient.get<DictType[]>(Api.dictOptionSelectList);
}
