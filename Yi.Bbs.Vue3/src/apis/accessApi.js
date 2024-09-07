import request from "@/config/axios/service";

// 触发访问
export function access() {
  return request({
    url: "/access-log",
    method: "post",
  });
}

// 获取本周数据
export function getWeek(data) {
  return request({
    url: "/access-log/week",
    method: "get",
    params :data
  });
}
// 获取全部数据
export function getAccessList(data) {
  return request({
    url: "/access-log",
    method: "get",
    params :data
  });
}
