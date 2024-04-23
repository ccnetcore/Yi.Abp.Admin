import request from "@/config/axios/service";

export function getList() {
  return request({
    url: "/chat-user",
    method: "get"
  });
}
