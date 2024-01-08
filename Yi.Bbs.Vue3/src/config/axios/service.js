import axios from "axios";
import router from "@/router";
import { ElMessage } from "element-plus";
import { config } from "@/config/axios/config";
import { Session } from "@/utils/storage";
import useAuths from "@/hooks/useAuths";

const { VITE_APP_ENV_NAME } = import.meta.env;
const { getToken, removeToken } = useAuths();

const { base_url, request_timeout, pre_interface } = config;
export const PATH_URL = base_url[VITE_APP_ENV_NAME];

// 配置新建一个 axios 实例
const service = axios.create({
  baseURL: PATH_URL, // api 的 base_url
  timeout: request_timeout, // 请求超时时间
  headers: { "Content-Type": "application/json" },
  hideerror: false, //是否在底层显示错误信息
  isFinish: false,
});

// 添加请求拦截器
service.interceptors.request.use(
  (config) => {
    // 在发送请求之前做些什么 token
    const token = getToken();
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    if (Session.get("tenantId")) {
      config.headers["TenantId"] = Session.get("tenantId");
    }
    return config;
  },
  (error) => {
    // 对请求错误做些什么
    return Promise.reject(error);
  }
);

// 添加响应拦截器
service.interceptors.response.use(
  (response) => {
    const { config } = response;
    config.isFinish = true;
    return Promise.resolve(response);
  },
  (error) => {
    const { config } = error;
    // 对响应错误做点什么
    if (error.message.indexOf("timeout") != -1) {
      ElMessage({
        type: "error",
        message: "网络超时",
      });
    } else if (error.message == "Network Error") {
      ElMessage({
        type: "error",
        message: "网络连接错误",
      });
    } else {
      const res = error.response || {};
      const status = Number(res.status) || 200;
      const message = res?.data?.error?.message;
      if (status === 401) {
        ElMessageBox.confirm("该功能需要登陆后享有,是否立即登录?", "提示", {
          confirmButtonText: "确认",
          cancelButtonText: "取消",
          type: "warning",
        }).then(() => {
          removeToken();
          router.push("/login");
        });
        return;
      }
      if (status !== 200) {
        if (status >= 500) {
          ElMessage({
            type: "error",
            message: "网络开小差了，请稍后再试",
          });
          return Promise.reject(new Error(message));
        }
        // 避开找不到后端接口的提醒
        if (status !== 404) {
          ElMessage({
            type: "error",
            message,
          });
        }
      }
    }
    config.isFinish = true;
    return Promise.reject(error.response);
  }
);

// 导出 axios 实例
export default service;
