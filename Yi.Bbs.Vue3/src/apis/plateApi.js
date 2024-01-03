import request from "@/config/axios/service";

export function getList(data) {
  return request({
    url: "/plate",
    method: "get",
    params: data,
  });
}
