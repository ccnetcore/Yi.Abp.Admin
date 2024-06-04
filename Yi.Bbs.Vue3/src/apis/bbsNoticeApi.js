import request from "@/config/axios/service";

export function getList(data) {
  return request({
    url: "/bbs-notice",
    method: "get",
    params: data,
  });
}
export function read(id) {

  return request({
    url: `/bbs-notice/read/${id??''}`,
    method: "put"
  });


  }
  