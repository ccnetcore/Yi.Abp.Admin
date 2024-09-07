import { http } from "@/utils/http";
import type { Result, ResultPage } from "@/api/result";

/** 查询岗位列表 */
export const getPostList = (data?: object) => {
  return http.request<ResultPage>("get", "/post", { data });
};

/** 查询岗位详细 */
export const getPost = id => {
  return http.request<Result>("get", `/post/${id}`, {});
};

/** 新增岗位 */
export const addPost = data => {
  return http.request<Result>("post", `/post`, { data });
};

/** 修改岗位 */
export const updatePost = (id, data) => {
  return http.request<Result>("put", `/post/${id}`, { data });
};

/** 修改岗位状态 */
export const updatePostStatus = (id, state) => {
  return http.request<Result>("put", `/post/${id}/${state}`, {});
};

/** 删除岗位 */
export const delPost = ids => {
  return http.request<Result>("delete", `/post`, { params: { id: ids } });
};

/** 获取岗位选择框列表 */
export const getPostOptionSelect = () => {
  return http.request<Result>("get", `/post`, {});
};
