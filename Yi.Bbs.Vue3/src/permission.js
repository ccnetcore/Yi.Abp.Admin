import router from "./router";
import useAuths from "@/hooks/useAuths";
import { ElMessage } from "element-plus";
import NProgress from "nprogress";
import "nprogress/nprogress.css";
import useUserStore from "@/stores/user";

NProgress.configure({ showSpinner: false });
const { getToken, logoutFun } = useAuths();
const whiteList = ["/login", "/auth-redirect", "/bind", "/register"];

router.beforeEach((to, from, next) => {
  NProgress.start();
  const hasToken = getToken();
  if (hasToken) {
    if (to.path === "/login") {
      // 已经登陆跳转到首页
      next({ path: "/" });
      NProgress.done();
    } else {
      if (useUserStore().roles.length === 0) {
        // 判断当前用户是否已拉取完user_info信息
        useUserStore()
          .getInfo()
          .then(() => {
            next({ ...to, replace: true });
          })
          .catch((err) => {
            logoutFun.then(() => {
              ElMessage.error(err);
              next({ path: "/" });
            });
          });
      } else {
        next();
      }
    }
  } else {
    // 没有token
    if (whiteList.indexOf(to.path) !== -1) {
      // 在免登录白名单，直接进入
      next();
    } else {
      next();
      useUserStore().resetInfo();
      // next(`/login?redirect=${to.path}&unTourist=true`); // 否则全部重定向到登录页
      NProgress.done();
    }
  }
});

router.afterEach(() => {
  NProgress.done();
});
