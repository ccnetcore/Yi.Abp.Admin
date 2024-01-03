import request from "@/config/axios/service";

//获取配置
export function getAll() {
  return request({
    url: "/config",
    method: "get",
  });
}
