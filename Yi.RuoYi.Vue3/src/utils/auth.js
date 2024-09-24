import Cookies from 'js-cookie'

const TokenKey = 'Admin-Token'
const RefreshTokenKey = 'Refresh-Token'
const TenantIdKey='Tenant-Id'
export function getToken() {
  return localStorage.getItem(TokenKey)
  // return Cookies.get(TokenKey)
}

export function setToken(token) {
  return localStorage.setItem(TokenKey, token)
  // return Cookies.set(TokenKey, token)
}

export function removeToken() {
  return localStorage.removeItem(TokenKey)
  // return Cookies.remove(TokenKey)
}

export function getRefreshToken() {
  return localStorage.getItem(RefreshTokenKey)
  // return Cookies.get(RefreshTokenKey)
}

export function setRefreshToken(token) {
  return localStorage.setItem(RefreshTokenKey, token)
  // return Cookies.set(RefreshTokenKey, token)
}

export function removeRefreshToken() {
  return localStorage.removeItem(RefreshTokenKey)
  // return Cookies.remove(RefreshTokenKey)
}

export function getTenantId() {
  return Cookies.get(TenantIdKey)
}

export function setTenantId(tenantId) {
  return Cookies.set(TenantIdKey, tenantId)
}

export function removeTenantId() {
  return Cookies.remove(TenantIdKey)
}
