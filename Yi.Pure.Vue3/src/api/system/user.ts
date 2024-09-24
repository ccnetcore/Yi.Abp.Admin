import { http } from "@/utils/http";
import type { Result, ResultPage } from "@/api/result";

/** 获取系统管理-用户管理列表 */
export const getUserList = (data?: object) => {
  return http.request<ResultPage>("get", "/user", { params: data });
};

/** 获取一个用户详细消息 */
export const getUser = (userId: string) => {
  return http.request<Result>("get", `/user/${userId}`, {});
};

/** 删除用户 */
export const delUser = (userIds: string[]) => {
  return http.request<Result>("delete", `/user`, {
    params: { id: userIds }
  });
};

/** 用户密码重置 */
export const resetUserPwd = (id: string, password: string) => {
  return http.request<Result>("put", `/account/rest-password/${id}`, {
    data: { password }
  });
};

/** 改变用户状态 */
export const changeUserStatus = (userId: string, state: boolean) => {
  return http.request<Result>("put", `/user/${userId}/${state}`, {});
};

/** 修改用户 */
export const updateUser = (id: string, data: any) => {
  return http.request<Result>("put", `/user/${id}`, { data });
};

/** 新增用户 */
export const addUser = (data: any) => {
  return http.request<Result>("post", `/user`, { data });
};

/** 查询用户个人信息 */
export const getUserProfile = () => {
  return http.request<UserInfoResult>("get", `/account`, {});
};

/** 修改用户个人信息 */
export const updateUserProfile = data => {
  return http.request<Result>("put", `/user/profile`, { data });
};

/** 只修改用户头像 */
export const updateUserIcon = data => {
  return http.request<Result>("put", `/account/icon`, { data: { icon: data } });
};

/** 用户密码重置 */
export const updateUserPwd = (oldPassword, newPassword) => {
  const data = {
    oldPassword,
    newPassword
  };
  return http.request<Result>("put", `/account/password`, { data });
};

export type UserInfoResult = {
  status: number;
  data: UserInfo;
};

export type UserInfo = {
  user: {
    /** 头像 */
    icon: string;
    /** 用户名 */
    userName: string;
    /** 昵称 */
    nick: string;
    /** 邮箱 */
    email: string;
    /** 联系电话 */
    phone: string;
    /** 简介 */
    introduction: string;
  };
  /** 当前登录用户的角色 */
  roleCodes: Array<string>;
  /** 按钮级别权限 */
  permissions: Array<string>;
};
