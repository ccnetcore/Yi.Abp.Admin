import request,{download} from '@/utils/request.js'
import { parseStrEmpty } from "@/utils/ruoyi";

// 导出角色
export function getExportExcel(query) {
  
  return download('/user/export-excel',query,'导出数据.xlsx')
}

// 查询用户列表
export function listUser(query) {
  return request({
    url: '/user',
    method: 'get',
    params: query
  })
}

// 查询用户详细
export function getUser(userId) {
  return request({
    url: '/user/' + parseStrEmpty(userId),
    method: 'get'
  })
}

// 新增用户
export function addUser(data) {
  data.phone=data.phone==""?null:data.phone;
  return request({
    url: '/user',
    method: 'post',
    data: data
  })
}

// 修改用户
export function updateUser(id, data) {
  data.phone=data.phone==""?null:data.phone;
  return request({
    url: `/user/${id}`,
    method: 'put',
    data: data
  })
}

// 删除用户
export function delUser(userId) {
  return request({
    url: `/user`,
    method: 'delete',
    params:{id:userId}
  })
}

// 用户密码重置
export function resetUserPwd(id, password) {
  const data = {
    password
  }


  return request({
    url: `/account/rest-password/${id}`,
    method: 'put',
    data: data
  })
}

// 用户状态修改
export function changeUserStatus(userId, isDel) {
  return request({
    url: `/user/${userId}/${isDel}`,
    method: 'put'
  })
}

// 查询用户个人信息
export function getUserProfile() {
  return request({
    url: '/account',
    method: 'get'
  })
}

// 修改用户个人信息
export function updateUserProfile(data) {
  return request({
    url: `/user/profile`,
    method: 'put',
    data:  data 
  })
}
// 只修改用户头像
export function updateUserIcon(data) {
  return request({
    url: `/account/icon`,
    method: 'put',
    data:{icon:data}  
  })
}


// 用户密码重置
export function updateUserPwd(oldPassword, newPassword) {
  const data = {
    oldPassword,
    newPassword
  }
  return request({
    url: '/account/password',
    method: 'put',
    data: data
  })
}

// 用户头像上传
export function uploadAvatar(data) {
  return request({
    url: '/system/user/profile/avatar',
    method: 'post',
    data: data
  })
}

// 查询授权角色
export function getAuthRole(userId) {
  return request({
    url: '/system/user/authRole/' + userId,
    method: 'get'
  })
}

// 保存授权角色
export function updateAuthRole(data) {
  return request({
    url: '/system/user/authRole',
    method: 'put',
    params: data
  })
}

// // 查询部门下拉树结构
// export function deptTreeSelect() {
//   return request({
//     url: '/system/user/deptTree',
//     method: 'get'
//   })
// }

