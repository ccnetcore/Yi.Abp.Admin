import request from "@/config/axios/service";

// 登录方法
export function login(username, password, code, uuid) {
  const data = {
    username,
    password,
    code,
    uuid,
  };
  return request({
    url: "/account/login",
    headers: {
      isToken: false,
    },
    method: "post",
    data: data,
  });
}


//找回密码
export function retrievePassword(password, phone, code, uuid) {
  const data = {
    password,
    phone,
    code,
    uuid,
  };
  return request({
    url: "/account/retrieve-password",
    headers: {
      isToken: false,
    },
    method: "post",
    data: data,
  });
}
// 注册方法
export function register(userName, password, phone, code, uuid) {
  const data = {
    userName,
    password,
    phone,
    code,
    uuid,
  };
  return request({
    url: "/account/register",
    headers: {
      isToken: false,
    },
    method: "post",
    data: data,
  });
}

// 获取用户详细信息
export function getInfo() {
  return request({
    url: "/account",
    method: "get",
  });
}

// 退出方法
export function logout() {
  return request({
    url: "/account/logout",
    method: "post",
  });
}

// 获取验证码
export function getCodeImg() {
  return request({
    url: "/account/captcha-image",
    headers: {
      isToken: false,
    },
    method: "get",
    timeout: 20000,
  });
}
// 获取短信验证码
export function getCodePhone(phoneForm) {
  return request({
    url: "/account/captcha-phone",
    headers: {
      isToken: false,
    },
    method: "post",
    timeout: 20000,
    data: phoneForm,
  });
}
// 获取短信验证码-为了重置密码
export function getCodePhoneForRetrievePassword(form) {
  return request({
    url: "/account/captcha-phone/repassword",
    headers: {
      isToken: false,
    },
    method: "post",
    timeout: 20000,
    data: form,
  });
}
