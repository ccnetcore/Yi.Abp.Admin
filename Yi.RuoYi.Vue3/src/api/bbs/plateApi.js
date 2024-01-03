import request from '@/utils/request'
/* 以下为api的模板，通用的crud，将以下变量替换即可：
plate : 实体模型
*/
// 分页查询
export function listData(query) {
  return request({
    url: '/plate',
    method: 'get',
    params: query
  })
}

// id查询
export function getData(id) {
  return request({
    url: `/plate/${id}`,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/plate',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/plate/${id}`,
    method: 'put',
    data: data
  })
}

// 删除
export function delData(ids) {
  return request({
    url: `/plate`,
    method: 'delete',
    params:{id:ids}
  })
}
