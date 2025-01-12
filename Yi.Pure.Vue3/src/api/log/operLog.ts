import { http } from "@/utils/http";
import type { ResultPage } from "@/api/result";

/** 查询操作日志列表 */
export const getOperLoglist = (query?: object) => {
  return http.request<ResultPage>("get", "/operation-log", { params: query });
};
