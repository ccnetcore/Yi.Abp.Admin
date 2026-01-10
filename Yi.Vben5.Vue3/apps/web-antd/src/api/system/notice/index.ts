import type { Notice } from './model';

import type { ID, IDS, PageQuery, PageResult } from '#/api/common';

import { requestClient } from '#/api/request';

enum Api {
  root = '/notice',
}

/**
 * 通知公告分页
 * @param params 分页参数
 * @returns 分页结果
 */
export function noticeList(params?: PageQuery) {
  return requestClient.get<PageResult<Notice>>(Api.root, { params });
}

/**
 * 通知公告详情
 * @param id id
 * @returns 详情
 */
export function noticeInfo(id: ID) {
  return requestClient.get<Notice>(`${Api.root}/${id}`);
}

/**
 * 通知公告新增
 * @param data 参数
 */
export function noticeAdd(data: Partial<Notice>) {
  return requestClient.postWithMsg<void>(Api.root, data);
}

/**
 * 通知公告更新
 * @param data 参数
 */
export function noticeUpdate(data: any) {
  return requestClient.putWithMsg<void>(`${Api.root}/${data.id}`, data);
}

/**
 * 通知公告删除
 * @param ids ids
 */
export function noticeRemove(ids: IDS) {
  return requestClient.deleteWithMsg<void>(Api.root, {
    params: { ids: ids.join(',') },
  });
}
