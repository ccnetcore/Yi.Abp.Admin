import request from '@/utils/request'

// 分页查询
export function listData(query) {
  return request({
    url: '/stock/pageList',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(code) {
  return request({
    url: '/stock/getById/' + code,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/stock/create',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/stock/update/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(code) {
  return request({
    url: '/stock/del',
    method: 'delete',
    data:"string"==typeof(code)?[code]:code
  })
}
