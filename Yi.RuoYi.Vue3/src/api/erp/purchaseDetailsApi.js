import request from '@/utils/request'
export function getListByPurchaseId(id){
  return request({
    url: `/purchaseDetails/getListByPurchaseId/${id}`,
    method: 'get'
  })
}

// 分页查询
export function listData(query) {
  return request({
    url: '/purchaseDetails/pageList',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(code) {
  return request({
    url: '/purchaseDetails/getById/' + code,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/purchaseDetails/create',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/purchaseDetails/update/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(code) {
  return request({
    url: '/purchaseDetails/del',
    method: 'delete',
    data:"string"==typeof(code)?[code]:code
  })
}
