import { getRefreshToken } from './auth'
import request from './request'

export function refreshToken() {
  return request({
    url: '/account/refresh',
    method: 'post',
    headers: {
      'Content-Type': 'application/json;charset=utf-8',
      'Authorization': 'Bearer ' + getRefreshToken(),
      'isToken' :false
    },
    __isRefreshToken: true
  })
}

export function isRefreshRequest(config) {
	return !!config.__isRefreshToken
}
