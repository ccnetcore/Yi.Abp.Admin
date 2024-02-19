import request from '@/utils/request'

// 查询缓存详细
export function getCache() {
  return request({
    url: '/monitor/cache',
    method: 'get'
  })
}

// 查询缓存名称列表
export function listCacheName() {
  return request({
    url: '/monitor-cache/name',
    method: 'get'
  })
}

// 查询缓存键名列表
export function listCacheKey(cacheName) {
  return request({
    url: '/monitor-cache/key/' + cacheName,
    method: 'get'
  })
}

// 查询缓存内容
export function getCacheValue(cacheName, cacheKey) {
  return request({
    url: '/monitor-cache/value/' + cacheName + '/' + cacheKey,
    method: 'get'
  })
}

// 清理指定名称缓存
export function clearCacheName(cacheName) {
  return request({
    url: '/monitor-cache/key/' + cacheName,
    method: 'delete'
  })
}

// 清理指定键名缓存
export function clearCacheKey(cacheName,cacheKey) {
  return request({
    url: '/monitor-cache/value/'+cacheName+'/' + cacheKey,
    method: 'delete'
  })
}

// 清理全部缓存
export function clearCacheAll() {
  return request({
    url: '/monitor-cache/clear',
    method: 'delete'
  })
}
