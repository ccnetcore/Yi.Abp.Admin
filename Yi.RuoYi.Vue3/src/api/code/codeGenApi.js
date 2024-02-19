import request from '@/utils/request'
// code to web
export function codeToWeb() {
  return request({
    url: 'code-gen/code-build-web',
    method: 'post'
  })
}
// code to web
export function webToCode(ids) {
  return request({
    url: 'code-gen/web-build-code',
    method: 'post',
    data:ids
  })
}

// open zhe path
export function openPath(path) {
  return request({
    url: `code-gen/dir/${encodeURIComponent(path)}`,
    method: 'post'
  })
}
