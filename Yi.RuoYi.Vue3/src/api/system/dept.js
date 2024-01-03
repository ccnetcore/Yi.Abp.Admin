import request from '@/utils/request'

// 查询部门列表
export function listDept(query) {
  return request({
    url: '/dept',
    method: 'get',
    params: query
  })
}

// // 查询部门列表（排除节点）
// export function listDeptExcludeChild(deptId) {
//   return request({
//     url: '/system/dept/list/exclude/' + deptId,
//     method: 'get'
//   })
// }

// 查询部门详细
export function getDept(deptId) {
  return request({
    url: '/dept/' + deptId,
    method: 'get'
  })
}

// 新增部门
export function addDept(data) {
  data.phone=data.phone==""?null:data.phone;
  return request({
    url: '/dept',
    method: 'post',
    data: data
  })
}

// 修改部门
export function updateDept(data) {
  data.phone=data.phone==""?null:data.phone;
  return request({
    url: `/dept/${data.id}`,
    method: 'put',
    data: data
  })
}

// 删除部门
export function delDept(deptId) {
  return request({
    url: `/dept`,
    method: 'delete',
    params:{id:deptId}
  })
}


// 根据角色ID查询菜单下拉树结构
export function roleDeptTreeselect(roleId) {
  return request({
    url: '/dept/role-id/' + roleId,
    method: 'get'
  })
}