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
 *  用户找回密码
 * @param {*} data 账号密码
 */
export function userRetrievePassword(data) {
  return request({
    url: `/account/retrieve-password`,
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

/**
 * 第三方账号登录
 * @param {*} params 参数
 * @param {*} scheme 类型
 * @returns
 */
export function authOtherLogin(params, scheme) {
  return request({
    url: `/auth/oauth/login/${scheme}`,
    method: "get",
    params: params,
  });
}

/**
 * 第三方账号绑定
 * @param {*} params 参数
 * @param {*} scheme 类型
 * @returns
 */
export function authOtherBind(params, scheme) {
  return request({
    url: `/auth/oauth/bind/${scheme}`,
    method: "post",
    params: params,
  });
}

/**
 * 第三方账号绑定
 * @param {*} params 参数
 * @param {*} scheme 类型
 * @returns
 */
export function getOtherAuthInfo(params) {
  return request({
    url: `auth/account`,
    method: "get",
    params: params,
  });
}

/**
 * 删除第三方授权
 * @param {*} ids
 * @returns
 */
export function delOtherAuth(ids) {
  return request({
    url: `/auth`,
    method: "delete",
    params: { id: ids },
  });
}
