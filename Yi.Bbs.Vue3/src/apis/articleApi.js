import request from "@/config/axios/service";
export function getList(data) {
  return request({
    url: "/article",
    method: "get",
    params: data,
  });
}
export function get(id) {
  return request({
    url: `/article/${id}`,
    method: "get",
  });
}
export function add(data) {
  return request({
    url: `/article`,
    method: "post",
    data: data,
  });
}
export function update(id, data) {
  return request({
    url: `/article/${id}`,
    method: "put",
    data: data,
  });
}
export function del(ids) {
  return request({
    url: `/article/${ids}`,
    method: "delete",
  });
}
export function all(discussId) {
  return request({
    url: `/article/all/discuss-id/${discussId}`,
    method: "get",
  });
}
