import request from '@/utils/request'

// 查询角色列表
export function listRole(query) {
  return request({
    url: '/role',
    method: 'get',
    params: query
  })
}



// 查询角色详细
export function getRole(roleId) {
  return request({
    url: '/role/' + roleId,
    method: 'get'
  })
}

// 新增角色
export function addRole(data) {
  return request({
    url: '/role',
    method: 'post',
    data: data
  })
}

// 修改角色
export function updateRole(data) {
  return request({
    url: `/role/${data.id}`,
    method: 'put',
    data: data
  })
}

// 角色数据权限
export function dataScope(data) {
  return request({
    url: '/role/data-scpoce',
    method: 'put',
    data: data
  })
}

// 角色状态修改
export function changeRoleStatus(roleId, isDel) {
  return request({
    url: `/role/${roleId}/${isDel}`,
    method: 'put'
  })
}

// 删除角色
export function delRole(roleId) {
  return request({
    url: `/role`,
    method: 'delete',
    params:{id:roleId}
  })
}

// 查询角色已授权用户列表
export function allocatedUserList(roleId,query) {
  return request({
    url: `/role/auth-user/${roleId}/true`,
    method: 'get',
    params: query
  })
}

// 查询角色未授权用户列表
export function unallocatedUserList(roleId,query) {
  return request({
    url: `/role/auth-user/${roleId}/false`,
    method: 'get',
    params: query
  })
}

// 取消用户授权角色
export function authUserCancel(data) {
  return request({
    url: '/role/auth-user',
    method: 'delete',
    data: data
  })
}

// 批量取消用户授权角色
export function authUserCancelAll(data) {
  return request({
    url: '/role/auth-user',
    method: 'delete',
    data: data
  })
}

// 授权用户选择
export function authUserSelectAll(data) {
  return request({
    url: '/role/auth-user',
    method: 'post',
    data: data
  })
}

// 根据角色ID查询部门树结构
// export function deptTreeSelect(roleId) {
//   return request({
//     url: '/system/role/deptTree/' + roleId,
//     method: 'get'
//   })
// }
// 获取角色选择框列表
export function roleOptionselect() {
  return request({
    url: '/role',
    method: 'get'
  })

}