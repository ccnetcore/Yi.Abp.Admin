import request from '@/utils/request'

// 分页查询
export function listData(query) {
  return request({
    url: '/article',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(code) {
  return request({
    url: '/article/' + code,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/article',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(data) {
  return request({
    url: `/article/${data.id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(ids) {
  return request({
    url: '/article',
    method: 'delete',
    params:{id:ids}
  })
}
