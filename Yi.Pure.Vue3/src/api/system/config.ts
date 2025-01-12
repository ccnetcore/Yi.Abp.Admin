import { http } from "@/utils/http";
import type { Result, ResultPage } from "@/api/result";

/** 获取系统管理-参数配置列表 */
export const getConfigList = (data?: object) => {
  return http.request<ResultPage>("get", "/config", { data });
};

/** 查询参数详细 */
export const getConfig = id => {
  return http.request<Result>("get", `/config/${id}`, {});
};

/** 新增参数 */
export const addConfig = data => {
  return http.request<Result>("post", `/config`, { data });
};

/** 修改参数 */
export const updateConfig = (id, data) => {
  return http.request<Result>("put", `/config/${id}`, { data });
};

/** 删除参数 */
export const delConfig = id => {
  return http.request<Result>("delete", `/config`, { params: { id } });
};
