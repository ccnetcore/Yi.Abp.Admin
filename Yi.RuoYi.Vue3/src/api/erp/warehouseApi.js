import request from '@/utils/request'

// 分页查询
export function listData(query) {
  return request({
    url: '/warehouse/pageList',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(code) {
  return request({
    url: '/warehouse/getById/' + code,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/warehouse/create',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/warehouse/update/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(code) {
  return request({
    url: '/warehouse/del',
    method: 'delete',
    data:"string"==typeof(code)?[code]:code
  })
}
