import { http } from "@/utils/http";

export type imageCaptcha = {
  status: number;
  data: {
    //uuid
    uuid: string; // 使用字符串表示 Guid
    //验证码图片字节数值
    img: Uint8Array | null;
  };
};

export type LoginResult = {
  status: number;
  data: {
    token: string;
    refreshToken: string;
  };
};

export type UserResult = {
  status: number;
  data: {
    user: {
      /** 头像 */
      icon: string;
      /** 用户名 */
      userName: string;
      /** 昵称 */
      nick: string;
    };

    /** 当前登录用户的角色 */
    roleCodes: Array<string>;
    /** 按钮级别权限 */
    permissions: Array<string>;
    /** `token` */
    accessToken: string;
    /** 用于调用刷新`accessToken`的接口时所需的`token` */
    refreshToken: string;
    /** `accessToken`的过期时间（格式'xxxx/xx/xx xx:xx:xx'） */
    expires: Date;
  };
};

export type RefreshTokenResult = {
  success: boolean;
  data: {
    /** `token` */
    accessToken: string;
    /** 用于调用刷新`accessToken`的接口时所需的`token` */
    refreshToken: string;
    /** `accessToken`的过期时间（格式'xxxx/xx/xx xx:xx:xx'） */
    expires: Date;
  };
};

/** 登录 */
export const getLogin = (data?: object) => {
  return http.request<LoginResult>("post", "/account/login", { data });
};

export const getUserInfo = () => {
  return http.request<UserResult>("get", "/account");
};

/** 获取验证码 */
export const getCodeImg = () => {
  return http.request<imageCaptcha>("get", "/account/captcha-image");
};

/** 刷新`token` */
export const refreshTokenApi = (data?: object) => {
  return http.request<RefreshTokenResult>("post", "/refresh-token", { data });
};
