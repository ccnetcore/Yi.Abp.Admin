import request from "@/config/axios/service";

export function signIn() {
  return request({
    url: "/integral/sign-in",
    method: "post"
  });
}

export function signInRecord() {
  return request({
    url: "/integral/sign-in/record",
    method: "get"
  });
}
