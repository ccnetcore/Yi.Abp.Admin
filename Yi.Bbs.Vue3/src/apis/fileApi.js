import request from "@/config/axios/service";

export function upload(data) {
  return request({
    url: "/file",
    method: "post",
    data: data,
    headers: { "Content-Type": "multipart/form-data" },
  });
}
