import request from '@/utils/request'

// 全查询
export function allData() {
  return request({
    url: '/supplier/getList',
    method: 'get'
  })
}

// 分页查询
export function listData(query) {
  return request({
    url: '/supplier/pageList',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(code) {
  return request({
    url: '/supplier/getById/' + code,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/supplier/create',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/supplier/update/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(code) {
  return request({
    url: '/supplier/del',
    method: 'delete',
    data:"string"==typeof(code)?[code]:code
  })
}
