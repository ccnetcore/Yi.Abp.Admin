import request from '@/utils/request'
// 分页查询
export function listData(query) {
  return request({
    url: 'template',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(id) {
  return request({
    url: `template/${id}`,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: 'template',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `template/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(ids) {
  return request({
    url: `template/${ids}`,
    method: 'delete',
  })
}
