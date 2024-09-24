import { http } from "@/utils/http";
import type { Result } from "@/api/result";

export const getAsyncRoutes = () => {
  return http.request<Result>("get", "/get-async-routes");
};

export const getRoutes = () => {
  return http.request<Result>("get", "/account/Vue3Router/pure");
};
