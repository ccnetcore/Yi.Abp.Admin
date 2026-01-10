import type { DictData } from './dict-data-model';

import type { ID, IDS, PageQuery } from '#/api/common';

import { commonExport } from '#/api/helper';
import { requestClient } from '#/api/request';

enum Api {
  dictDataExport = '/dict/data/export',
  dictDataInfo = '/dictionary/dic-type',
  dictDataList = '/dict/data/list',
  root = '/dictionary',
}

/**
 * 主要是DictTag组件使用
 * @param dictType 字典类型
 * @returns 字典数据
 */
export function dictDataInfo(dictType: string) {
  return requestClient.get<DictData[]>(`${Api.dictDataInfo}/${dictType}`);
}

/**
 * 字典数据
 * @param params 查询参数
 * @returns 字典数据列表
 */
export function dictDataList(params?: PageQuery) {
  return requestClient.get<DictData[]>(Api.root, { params });
}

/**
 * 导出字典数据
 * @param data 表单参数
 * @returns blob
 */
export function dictDataExport(data: Partial<DictData>) {
  return commonExport(Api.dictDataExport, data);
}

/**
 * 删除
 * @param dictIds 字典ID Array
 * @returns void
 */
export function dictDataRemove(dictIds: IDS) {
  return requestClient.deleteWithMsg<void>(Api.root, {
    params: { ids: dictIds.join(',') },
  });
}

/**
 * 新增
 * @param data 表单参数
 * @returns void
 */
export function dictDataAdd(data: Partial<DictData>) {
  return requestClient.postWithMsg<void>(Api.root, data);
}

/**
 * 修改
 * @param data 表单参数
 * @returns void
 */
export function dictDataUpdate(data: Partial<DictData>) {
  return requestClient.putWithMsg<void>(`${Api.root}/${data.id}`, data);
}

/**
 * 查询字典数据详细
 * @param id 字典ID
 * @returns 字典数据
 */
export function dictDetailInfo(id: ID) {
  return requestClient.get<DictData>(`${Api.root}/${id}`);
}
