import { http } from "@/utils/http";
import type { Result, ResultPage } from "@/api/result";

/** 查询在线用户列表 */
export const getOnlineList = (query?: object) => {
  return http.request<ResultPage>("get", "/online", { params: query });
};

/** 强退用户 */
export const forceLogout = (tokenId?: object) => {
  return http.request<Result>("delete", `/online/${tokenId}`, {});
};
