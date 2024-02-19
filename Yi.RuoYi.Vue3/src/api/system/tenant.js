import request from '@/utils/request'
/* 以下为api的模板，通用的crud，将以下变量替换即可：
tenant : 实体模型
*/
// 分页查询
export function listData(query) {
  return request({
    url: '/tenant',
    method: 'get',
    params: query
  })
}
export function SelectData() {
  return request({
    url: '/tenant/select',
    method: 'get'
  })
}


// id查询
export function getData(id) {
  return request({
    url: `/tenant/${id}`,
    method: 'get'
  })
}

// 新增
export function addData(data) {
  return request({
    url: '/tenant',
    method: 'post',
    data: data
  })
}

// 修改
export function updateData(id,data) {
  return request({
    url: `/tenant/${id}`,
    method: 'put',
    data: data
  })
}

// 初始化
export function InitData(id) {
  return request({
    url: `/tenant/init/${id}`,
    method: 'put'
  })
}

// 删除
export function delData(ids) {
  return request({
    url: `/tenant`,
    method: 'delete',
    params:{id:ids}
  })
}
