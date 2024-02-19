import axios from 'axios'
import { ElNotification, ElMessageBox, ElMessage, ElLoading } from 'element-plus'
import { getToken,getTenantId } from '@/utils/auth'
import errorCode from '@/utils/errorCode'
import { tansParams, blobValidate } from '@/utils/ruoyi'
import cache from '@/plugins/cache'
import { saveAs } from 'file-saver'
import useUserStore from '@/store/modules/user'
import JsonBig from 'json-bigint'
import qs from 'qs'

let downloadLoadingInstance;
// 是否显示重新登录
export let isRelogin = { show: false };

axios.defaults.headers['Content-Type'] = 'application/json;charset=utf-8'
// 创建axios实例
const service = axios.create({
  // axios中请求配置有baseURL选项，表示请求URL公共部分
  baseURL: import.meta.env.VITE_APP_BASE_API,
  // 超时
  timeout: 10000,
  //处理批零参数
  paramsSerializer:params => {
    // return qs.stringify(params,{indices:false})
  //  console.log(params,"params")
//     if(params.id!=undefined)
//     {
//       if(Array.isArray(params.id) )
//       {
//         return "id="+params.id.join("&id=")
//       }
//       else
//       {
//         return "id="+params.id;
//       }
    
//     }
// return request.param(params);
return qs.stringify(params, {arrayFormat: 'repeat'});
  },

  transformResponse: [data => {
    const json = JsonBig({
      storeAsString: true
    })
    try {
      return json.parse(data)
    }
    catch
    {
      return data;
    }
  }],
})




// request拦截器
service.interceptors.request.use(config => {

  // 是否需要设置 token
  const isToken = (config.headers || {}).isToken === false
  // 是否需要防止数据重复提交
  const isRepeatSubmit = (config.headers || {}).repeatSubmit === false
  if (getToken() && !isToken) {
    config.headers['Authorization'] = 'Bearer ' + getToken() // 让每个请求携带自定义token 请根据实际情况自行修改
  }
  console.log(getTenantId(),"uuu");
if(getTenantId()!==undefined&&getTenantId()!==null&&getTenantId()!=='null')
{
  console.log('爆炸');
  config.headers['__tenant'] = getTenantId()
}

  // get请求映射params参数
  if (config.method === 'get' && config.params) {
    let url = config.url + '?' + tansParams(config.params);
    url = url.slice(0, -1);
    config.params = {};
    config.url = url;
  }
  if (!isRepeatSubmit && (config.method === 'post' || config.method === 'put')) {
    const requestObj = {
      url: config.url,
      data: typeof config.data === 'object' ? JSON.stringify(config.data) : config.data,
      time: new Date().getTime()
    }
    const sessionObj = cache.session.getJSON('sessionObj')
    if (sessionObj === undefined || sessionObj === null || sessionObj === '') {
      cache.session.setJSON('sessionObj', requestObj)
    } else {
      const s_url = sessionObj.url;                // 请求地址
      const s_data = sessionObj.data;              // 请求数据
      const s_time = sessionObj.time;              // 请求时间
      const interval = 1000;                       // 间隔时间(ms)，小于此时间视为重复提交
      if (s_data === requestObj.data && requestObj.time - s_time < interval && s_url === requestObj.url) {
        const message = '数据正在处理，请勿重复提交';
        console.warn(`[${s_url}]: ` + message)
        return Promise.reject(new Error(message))
      } else {
        cache.session.setJSON('sessionObj', requestObj)
      }
    }
  }

  return config
}, error => {
  Promise.reject(error)
})

// 响应拦截器
service.interceptors.response.use(res => {

  // //如果code为200，不需要处理，直接返回数据即可
  // console.log(res,"res")
  // // 二进制数据则直接返回
  // if (res.request.responseType === 'blob' || res.request.responseType === 'arraybuffer') {
  //   return res.data
  // }

  // const code = res.data.status || 200;
  // // 获取错误信息
  // const msg = `${res.data.errors.message},详细信息：${details}` ;

  // handler(code, msg);
  return Promise.resolve(res);
},
  error => {

    console.log(error.response,"error")
    const errorRes=error.response;
    const code = errorRes.status || 200;
    const msg = `${errorRes.data?.error?.message}` ;
    handler(code, msg);
    return Promise.reject(error)
  }
)

// 通用下载方法
export function download(url, query, filename) {
  downloadLoadingInstance = ElLoading.service({ text: "正在下载数据，请稍候", background: "rgba(0, 0, 0, 0.7)", })
  return service({
    url: url,
    method: 'get',
    params: query,
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    responseType: 'blob'
  }).then(async (data) => {
    debugger;
      const blob = new Blob([data.data])
      saveAs(blob, filename)
    downloadLoadingInstance.close();
  }).catch((r) => {
    console.error(r)
    ElMessage.error('下载文件出现错误，请联系管理员！')
    downloadLoadingInstance.close();
  })
}

const handler = (code, msg) => {
  switch (code) {
    //服务器异常
    case 500:
      ElMessage({
        message: msg,
        type: 'error'
      });
      break;
    //业务异常
    case 403:
      ElNotification.error({
        title: msg
      })
      break;
    //接口异常
    case 400:
        ElNotification.error({
          title: msg
        })
        break;
    //未授权
    case 401:
      ElMessageBox.confirm('登录状态已过期，您可以继续留在该页面，或者重新登录', '系统提示', {
        confirmButtonText: '重新登录',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(() => {
          isRelogin.show = false;
          useUserStore().logOut().then(() => {
            location.href = '/index';
          })
        }).catch(() => {
          isRelogin.show = false;
        });
      break;
    case 404:
      ElMessage({
        message: "404未找到资源",
        type: 'error'
      });
      break;
    //正常
    case 200:
      break;
  }
}

export default service
