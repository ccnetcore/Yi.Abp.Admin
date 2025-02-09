import request from "@/config/axios/service";

/**
 * 获取作者主题
 * @param userId
 * @param {*} data
 * @returns
 */
export function getAuthorTopic(userId,data) {
  return request({
    url: `/analyse/bbs-discuss/author/${userId}`,
    method: "get",
    data,
  });
}


/**
 * 获取推荐主题
 * @param {*} data
 * @returns
 */
export function getRecommendedTopic(data) {
  return request({
    url: "/analyse/bbs-discuss/random",
    method: "get",
    data,
  });
}

/**
 * 获取推荐好友
 * @param {*} data
 * @returns
 */
export function getRecommendedFriend(data) {
  return request({
    url: "/analyse/bbs-user/random",
    method: "get",
    data,
  });
}

/**
 * 获取钱钱排行
 * @param {*} data
 * @returns
 */
export function getMoneyTop(data) {
  return request({
    url: "/analyse/bbs-user/money-top",
    method: "get",
    params: data
  });
}


/**
 * 获取价值排行
 * @param {*} data
 * @returns
 */
export function getValueTop(data) {
  return request({
    url: "/analyse/dc-user/value-top",
    method: "get",
    params: data
  });
}

/**
 * 获取积分排行
 * @param {*} data
 * @returns
 */
export function getPointsTop(data) {
  return request({
    url: "/analyse/dc-user/points-top",
    method: "get",
    params: data
  });
}



/**
 * 获取用户分析
 * @param {*} data
 * @returns
 */
export function getUserAnalyse(data) {
  return request({
    url: "/analyse/bbs-user",
    method: "get",
    data,
  });
}


/**
 * 获取注册数量用户分析
 * @param {*} data
 * @returns
 */
export function getRegisterAnalyse(data) {
  return request({
    url: "/analyse/bbs-user/register",
    method: "get",
    data,
  });
}