import type { ServerInfo } from './model';

import { requestClient } from '#/api/request';

/**
 * 获取服务器信息
 * @returns 服务器信息
 */
export function getServerInfo() {
  return requestClient.get<ServerInfo>('/monitor-server/info');
}

