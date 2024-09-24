import { http } from "@/utils/http";
import type { Result, ResultPage } from "@/api/result";

/** 查询菜单下拉树结构 */
export const getMenuOption = () => {
  return http.request<ResultPage>("get", `/menu`, {});
};

/** 查询菜单列表 */
export const getListMenu = query => {
  return http.request<ResultPage>("get", `/menu`, { params: query });
};

/** 查询菜单详细 */
export const getMenu = id => {
  return http.request<Result>("get", `/menu/${id}`, {});
};

/** 新增菜单 */
export const addMenu = data => {
  return http.request<Result>("post", `/menu`, { data });
};

/** 更新菜单 */
export const updateMenu = (id, data) => {
  return http.request<Result>("put", `/menu/${id}`, { data });
};

/** 删除菜单 */
export const delMenu = ids => {
  return http.request<Result>("delete", `/menu`, { params: { id: ids } });
};
