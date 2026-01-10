import type { OperationLog } from './model';

import type { IDS, PageQuery, PageResult } from '#/api/common';

import { commonExport } from '#/api/helper';
import { requestClient } from '#/api/request';

enum Api {
  operLogClean = '/operation-log/clean',
  operLogExport = '/operation-log/export',
  root = '/operation-log',
}

/**
 * 操作日志分页
 * @param params 查询参数
 * @returns 分页结果
 */
export function operLogList(params?: PageQuery) {
  return requestClient.get<PageResult<OperationLog>>(Api.root, {
    params,
  });
}

/**
 * 删除操作日志
 * @param operIds id/ids
 */
export function operLogRemove(operIds: IDS) {
  return requestClient.deleteWithMsg<void>(Api.root, {
    params: { ids: operIds.join(',') },
  });
}

/**
 * 清空全部分页日志
 */
export function operLogClean() {
  return requestClient.deleteWithMsg<void>(Api.operLogClean);
}

/**
 * 导出操作日志
 * @param data 查询参数
 */
export function operLogExport(data: Partial<OperationLog>) {
  return commonExport(Api.operLogExport, data);
}
