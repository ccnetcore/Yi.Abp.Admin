import request from "@/config/axios/service";

//得到利息趋势
export function getInterestList() {
  return request({
    url: "/bank/interest",
    method: "get"
  });
}


// 获取用户的银行卡
export function getBankCardList() {
  return request({
    url: "/bank",
    method: "get",
  });
}

// 申请银行卡
export function applyingBankCard() {
  return request({
    url: "/bank/applying",
    method: "post"
  });
}

// 提款
export function drawMoney(cardId) {
  return request({
    url: `/bank/draw/${cardId}`,
    method: "put"
  });
}

// 存款
export function depositMoney(cardId,moneyNum) {
  return request({
    url: `/bank/deposit/${cardId}/${moneyNum}`,
    method: "put"
  });
}
