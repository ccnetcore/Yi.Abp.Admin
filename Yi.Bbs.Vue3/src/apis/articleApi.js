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
    url: `/article`,
    method: "delete",
    params: { id: ids },
  });
}
export function all(discussId) {
  return request({
    url: `/article/all/discuss-id/${discussId}`,
    method: "get",
  });
}

/**
 *  导入文章
 */
export function importArticle(params, data) {
  return request({
    url: `/article/import`,
    headers: { "Content-Type": "multipart/form-data" },
    params: params,
    data,
    method: "post",
  });
}
