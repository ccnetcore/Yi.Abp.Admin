import request from "@/config/axios/service";

/**
 * 根据字典类型获取字典列表
 * @param {*} dicType 字典类型
 * @returns
 */
export function getDictionaryList(dicType) {
  return request({
    url: `/dictionary/dic-type/${dicType}`,
    method: "get",
  });
}
