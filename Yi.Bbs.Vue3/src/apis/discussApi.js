import request from "@/config/axios/service";

export function setResolve(discussId) {
  return request({
    url: `/discuss/reward/resolve/${discussId}`,
    method: "put"
  });
}

export function getList(data) {
  return request({
    url: "/discuss",
    method: "get",
    params: data,
  });
}
export function getTopList(data) {
  if (data == undefined) {
    data = { isTop: true };
  } else {
    data["isTop"] = true;
  }

  return request({
    url: "/discuss",
    method: "get",
    params: data,
  });
}
export function get(id) {
  return request({
    url: `/discuss/${id}`,
    method: "get",
  });
}
export function add(data) {
  return request({
    url: `/discuss`,
    method: "post",
    data: data,
  });
}
export function update(id, data) {
  return request({
    url: `/discuss/${id}`,
    method: "put",
    data: data,
  });
}
export function del(ids) {
  return request({
    url: `/discuss`,
    method: "delete",
    params: { id: ids },
  });
}

export function getHomeDiscuss() {
  return request({
    url: `/discuss/top`,
    method: "get",
  });
}
