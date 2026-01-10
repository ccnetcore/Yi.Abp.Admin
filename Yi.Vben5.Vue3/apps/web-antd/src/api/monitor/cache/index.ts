import { requestClient } from '#/api/request';

export interface CommandStats {
  name: string;
  value: string;
}

export interface RedisInfo {
  [key: string]: string;
}

export interface CacheInfo {
  commandStats: CommandStats[];
  dbSize: number;
  info: RedisInfo;
}

export interface CacheName {
  cacheName: string;
  remark: string;
}

export interface CacheValue {
  cacheName: string;
  cacheKey: string;
  cacheValue: string;
}

/**
 *
 * @returns redis信息
 */
export function redisCacheInfo() {
  return requestClient.get<CacheInfo>('/monitor/cache');
}

/**
 * 查询缓存名称列表
 * @returns 缓存名称列表
 */
export function listCacheName() {
  return requestClient.get<CacheName[]>('/monitor-cache/name');
}

/**
 * 查询缓存键名列表
 * @param cacheName 缓存名称
 * @returns 缓存键名列表
 */
export function listCacheKey(cacheName: string) {
  return requestClient.get<string[]>(`/monitor-cache/key/${cacheName}`);
}

/**
 * 查询缓存内容
 * @param cacheName 缓存名称
 * @param cacheKey 缓存键名
 * @returns 缓存内容
 */
export function getCacheValue(cacheName: string, cacheKey: string) {
  return requestClient.get<CacheValue>(
    `/monitor-cache/value/${cacheName}/${cacheKey}`,
  );
}

/**
 * 清理指定名称缓存
 * @param cacheName 缓存名称
 */
export function clearCacheName(cacheName: string) {
  return requestClient.deleteWithMsg<void>(`/monitor-cache/key/${cacheName}`);
}

/**
 * 清理指定键名缓存
 * @param cacheName 缓存名称
 * @param cacheKey 缓存键名
 */
export function clearCacheKey(cacheName: string, cacheKey: string) {
  return requestClient.deleteWithMsg<void>(
    `/monitor-cache/value/${cacheName}/${cacheKey}`,
  );
}

/**
 * 清理全部缓存
 */
export function clearCacheAll() {
  return requestClient.deleteWithMsg<void>('/monitor-cache/clear');
}
