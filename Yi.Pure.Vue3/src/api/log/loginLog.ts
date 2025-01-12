import { http } from "@/utils/http";
import type { Result, ResultPage } from "@/api/result";

/** 查询登录日志列表 */
export const getLoginLoglist = (query?: object) => {
  return http.request<ResultPage>("get", "/login-log", { params: query });
};

/** 删除登录日志列表 */
export const delLoginLoglist = id => {
  return http.request<Result>("delete", `/login-log/${id}`, {});
};
