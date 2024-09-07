import { http } from "@/utils/http";
import type { ResultFile } from "@/api/result";

/** 上传文件*/
export const uploadFile = (data?: object) => {
  return http.request<ResultFile>("post", "/file", {
    headers: {
      "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
    },
    data
  });
};
