import { ElMessage, ElMessageBox } from "element-plus";
import useUserStore from "@/stores/user";
import router from "@/router";
import { Session, Local } from "@/utils/storage";
import{computed} from 'vue'
import {
  userLogin,
  getUserDetailInfo,
  userLogout,
  userRegister, userRetrievePassword,
} from "@/apis/auth";
const TokenKey = "AccessToken";
export const AUTH_MENUS = "AUTH_MENUS";
export const AUTH_USER = "AUTH_USER";


export default function useAuths(opt) {
 


  const defaultOpt = {
    loginUrl: "/login", // 登录页跳转url 默认: /login
    loginReUrl: "", // 登录页登陆成功后带重定向redirect=的跳转url 默认为空
    homeUrl: "/index", // 主页跳转url 默认: /index
    otherQuery: {}, // 成功登录后携带的（除redirect外）其他参数
  };

  let option = {
    ...defaultOpt,
    ...opt,
  };

  // 获取token
  const getToken = () => {
   var token= Local.get(TokenKey);
    return token;
  };

const isLogin=computed(()=>{
  return getToken()? true : false
});

const currentUserInfo=computed(()=>{
  return useUserStore();
}); 

  // 存储token到cookies
  const setToken = (token) => {
    if (token == null) {
      return false;
    }
    Local.set(TokenKey, token);
   
    return true;
  };

  // 退出登录
  const logoutFun = async () => {
    let flag = true;
    try {
       await userLogout().then((res) => {
        useUserStore().updateToken(null);
        ElMessage({
          message: "退出成功",
          type: "info",
          duration: 2000,
        });
      });
    } catch (error) {
      flag = await ElMessageBox.confirm(
        `退出登录失败，是否强制退出？`,
        "提示",
        {
          confirmButtonText: "确 定",
          cancelButtonText: "取 消",
          type: "warning",
        }
      )
        .then(() => {
          useUserStore().updateToken(null);
          return true;
        })
        .catch(() => {
          //取消
          return false;
        });
    }

    if (flag) {
      clearStorage();
    }
  };

  // 清空本地存储的信息
  const clearStorage = () => {
    Session.clear();
    Local.clear();
    removeToken();

  };

  // 用户名密码登录
  const loginFun = async (params) => {
    try {
      const res = await userLogin(params);

      ElMessage({
        message: `您好${params.userName}，登录成功！`,
        type: "success",
      });
     await loginSuccess(res);
      return res;
    } catch (error) {
      const { data } = error;
      if (error.status === 403 && data.error?.message === "验证码错误") {
        useUserStore().updateCodeImage();
      }
    }
  };

  // 获取用户基本信息、角色、菜单权限
  const getUserInfo = async () => {
    try {
      let { data } = await getUserDetailInfo();
      // useUserStore
      // store.dispatch("updateUserInfo", result);
      return data;
    } catch (error) {
      return {};
    }
  };


  // 删除token
  const removeToken = () => {
   // console.log("token发生改变22清除清除")
    Local.remove(TokenKey);
    return true;
  };


  // 登录成功之后的操作
  const loginSuccess = async (res) => {
    const { token } = res.data;

    setToken(token);
    useUserStore().updateToken(token);
    try {
      // 存储用户信息

      await useUserStore().getInfo(); // 用户信息
      // 登录成功后 路由跳转
      // 如果有记录当前跳转页面
      const currentPath = Session.get("currentPath");
      if (currentPath) {
        router.replace(currentPath);
      } else {
        router.replace({
          path: option.loginReUrl ? option.loginReUrl : option.homeUrl,
          query: option.otherQuery,
        });
      }
    } catch (error) {
      removeToken();
      return false;
    }
  };




  // 注册
  const registerFun = async (params) => {
    // try {
      await userRegister(params);
      ElMessage({
        message: `恭喜！${params.userName}，注册成功！请登录！`,
        type: "success",
      });
    // } catch (error) {
    //   console.log(error);
    // }
  };

  // 找回密码
  const retrievePasswordFun = async (params) => {
    // try {
   const {data}=await userRetrievePassword(params);
    ElMessage({
      message: `恭喜！账号：${data}，找回成功！密码已重置，请登录！`,
      type: "success",
      duration: 8000
    });
    // } catch (error) {
    //   console.log(error);
    // }
  };
  
  return {
    getToken,
    setToken,
    removeToken,
    loginFun,
    getUserInfo,
    logoutFun,
    retrievePasswordFun,
    clearStorage,
    registerFun,
    loginSuccess,
    isLogin,
    currentUserInfo
  };
}
