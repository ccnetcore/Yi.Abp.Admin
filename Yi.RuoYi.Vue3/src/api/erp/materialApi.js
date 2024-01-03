import request from '@/utils/request'

// 分页查询
export function listData(query) {
  return request({
    url: '/material/pageList',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(code) {
  return request({
    url: '/material/getById/' + code,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/material/create',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/material/update/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(code) {
  return request({
    url: '/material/del',
    method: 'delete',
    data:"string"==typeof(code)?[code]:code
  })
}
