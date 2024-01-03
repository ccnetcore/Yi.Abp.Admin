import request from "@/config/axios/service";

/**
 *  用户登录
 * @param {*} data 账号密码
 */
export function userLogin(data) {
  return request({
    url: `/account/login`,
    method: "post",
    data,
  });
}

/**
 *  用户注册
 * @param {*} data 账号密码
 */
export function userRegister(data) {
  return request({
    url: `/account/register`,
    method: "post",
    data,
  });
}

/**
 *  获取用户详细信息
 */
export function getUserDetailInfo() {
  return request({
    url: `/account`,
    method: "get",
  });
}

/**
 *  用户退出
 */
export function userLogout() {
  return request({
    url: `/account/logout`,
    method: "post",
  });
}

/**
 *  获取验证码
 */
export function getCodeImg() {
  return request({
    url: `/account/captcha-image`,
    method: "get",
  });
}
/**
 *  获取短信验证码
 */
export function getCodePhone(data) {
  return request({
    url: `/account/captcha-phone`,
    method: "post",
    data,
  });
}

/**
 *  获取登录验证码
 */
export function getLoginCode() {
  return request({
    url: `/account/captcha-image`,
    method: "get",
  });
}
