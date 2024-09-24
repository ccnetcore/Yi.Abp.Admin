import { http } from "@/utils/http";
import type { Result, ResultList, ResultPage } from "@/api/result";

/** 获取角色选择框列表 */
export const getRoleOption = () => {
  return http.request<ResultPage>("get", `/role`, {});
};

/** 查询角色列表 */
export const getRoleList = query => {
  return http.request<ResultPage>("get", `/role`, { params: query });
};

/** 查询角色详细 */
export const getRole = roleId => {
  return http.request<Result>("get", `/role/${roleId}`, {});
};

/** 新增角色 */
export const addRole = data => {
  return http.request<Result>("post", `/role`, { data });
};

/** 修改角色 */
export const updateRole = (roleId, data) => {
  return http.request<Result>("put", `/role/${roleId}`, { data });
};

/** 修改角色状态 */
export const changeRoleStatus = (roleId, state) => {
  return http.request<Result>("put", `/role/${roleId}/${state}`, {});
};

/** 删除角色 */
export const delRole = roleIds => {
  return http.request<Result>("delete", `/role`, { params: { id: roleIds } });
};

/** 修改角色数据权限 */
export const updataDataScope = data => {
  return http.request<Result>("put", `/role/data-scpoce`, { data });
};

/** 根据角色ID查询菜单下拉树结构 */
export const getRoleMenuSelect = roleId => {
  return http.request<ResultList>("get", `/menu/role-id/${roleId}`, {});
};
