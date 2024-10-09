import { http } from "@/utils/http";
import type { ResultFile } from "@/api/result";

/** 上传文件*/
export const uploadFile = data => {
  return http.request<ResultFile>(
    "post",
    "/file",
    { data },
    {
      headers: {
        "Content-Type": "multipart/form-data"
      }
    }
  );
};
