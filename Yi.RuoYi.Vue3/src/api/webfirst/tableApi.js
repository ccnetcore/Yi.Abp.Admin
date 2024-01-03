import request from '@/utils/request'

// 分页查询
export function listData(query) {
  return request({
    url: '/table',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(id) {
  return request({
    url: `/table/${id}`,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/table',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/table/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(ids) {
  return request({
    url: `/table/${ids}`,
    method: 'delete',
  })
}
