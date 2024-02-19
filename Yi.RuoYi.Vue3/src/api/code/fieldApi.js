import request from '@/utils/request'

// 分页查询
export function listData(query) {
  return request({
    url: '/field',
    method: 'get',
    params: query
  })
}

// type查询
export function getType(query) {
  return request({
    url: '/field/type',
    method: 'get',
    params: query
  })
}


// id查询
export function getData(id) {
  return request({
    url: `/field/${id}`,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/field',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(data) {
  return request({
    url: `/field/${data.id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(ids) {
  return request({
    url: `/field/${ids}`,
    method: 'delete',
    params:{id:ids}
  })
}
