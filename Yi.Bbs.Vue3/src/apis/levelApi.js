import request from "@/config/axios/service";

export function getList(data) {
  return request({
    url: "/level",
    method: "get",
    params: data,
  });
}
export function upgrade(experience) {
  return request({
    url: `/level/upgrade?experience=${experience}`,
    method: "put"
  });
}

