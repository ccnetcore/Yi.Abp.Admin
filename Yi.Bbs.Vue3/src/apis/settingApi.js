import request from "@/config/axios/service";

/**
 *  获取图标列表
 */
export function getIconList() {
  return request({
    url: `/setting/icon`,
    method: "get",
  });
}
