import request from "@/config/axios/service";

/**
 *  获取商城列表
 */
export function getShopList(data) {
    return request({
        url: `/bbs-shop`,
        method: "get",
        params: data
    });
}


/**
 *  获取商城的用户信息
 */
export function getAccountInfo() {
    return request({
        url: `/bbs-shop/account`,
        method: "get",
    });
}

/**
 *  购买商品
 */
export function postBuy(data) {
    return request({
        url: `/bbs-shop/buy`,
        method: "post",
        data
    });
}
