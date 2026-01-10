import type { OnlineUser } from './model';

import type { IDS, PageQuery, PageResult } from '#/api/common';

import { requestClient } from '#/api/request';

enum Api {
  root = '/online',
}

/**
 * 当前账号的在线设备 个人中心使用
 * @returns OnlineUser[]
 */
export function onlineDeviceList() {
  return requestClient.get<PageResult<OnlineUser>>(Api.root);
}

/**
 * 这里的分页参数无效 返回的是全部的分页
 * @param params 请求参数
 * @returns 结果
 */
export function onlineList(params?: PageQuery) {
  return requestClient.get<PageResult<OnlineUser>>(Api.root, { params });
}

/**
 * 强制下线
 * @param tokenId 连接Id
 * @returns void
 */
export function forceLogout(tokenId: IDS) {
  return requestClient.deleteWithMsg<void>(Api.root, {
    params: { ids: tokenId.join(',') },
  });
}

/**
 * 个人中心用的 跟上面的不同是用的Post
 * @param tokenId 连接Id
 * @returns void
 */
export function forceLogout2(tokenId: string) {
  return requestClient.deleteWithMsg<void>(`${Api.root}/myself/${tokenId}`);
}
