import request from '@/utils/request'

// 查询操作日志列表
export function list(query) {
  return request({
    url: '/operation-log',
    method: 'get',
    params: query
  })
}

// 删除操作日志
export function delOperlog(operId) {
  return request({
    url: '/operationLog/delList',
    method: 'delete',
    data:"string"==typeof(operId)?[operId]:operId
  })
}

// 清空操作日志
export function cleanOperlog() {
  return request({
    url: '/operationLog/clear',
    method: 'delete'
  })
}
