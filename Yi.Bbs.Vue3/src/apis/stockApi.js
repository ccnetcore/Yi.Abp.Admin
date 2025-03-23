import request from "@/config/axios/service";

// 获取股票新闻列表
export function getStockNews(params) {
  return request({
    url: "/stock/news",
    method: "get",
    params
  });
}

// 获取用户股票持仓
export function getUserHoldings() {
  return request({
    url: "/stock/user-holdings",
    method: "get"
  });
}

// 获取用户交易记录
export function getUserTransactions(stockCode) {
  return request({
    url: "/stock/user-transactions",
    method: "get",
    params: { stockCode }
  });
}

// 获取股票价格记录
export function getStockPriceRecords(stockId, startTime, endTime, periodType = 'Hour') {
  return request({
    url: "/stock/price-records",
    method: "get",
    params: {
      StockId: stockId,
      StartTime: startTime,
      EndTime: endTime,
      PeriodType: periodType,
      MaxResultCount : 100
    }
  });
}

// 获取股市列表
export function getStockMarkets() {
  return request({
    url: "/stock/markets",
    method: "get"
  });
}

// 买入股票
export function buyStock(data) {
  return request({
    url: "/stock/buy",
    method: "post",
    data
  });
}

// 卖出股票
export function sellStock(params) {
  return request({
    url: "/stock/sell",
    method: "delete",
    params
  });
} 