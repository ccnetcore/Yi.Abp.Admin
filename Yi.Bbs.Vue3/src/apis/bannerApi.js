import request from "@/config/axios/service";

export function getList(data) {
  return request({
    url: "/banner",
    method: "get",
    params: data,
  });
}
