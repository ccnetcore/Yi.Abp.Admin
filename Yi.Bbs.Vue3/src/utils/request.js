
import axios from 'axios';
import { getToken } from '@/utils/auth'
export let isRelogin = { show: false };
// import JsonBig from 'json-bigint'
const myaxios = axios.create({
    baseURL:import.meta.env.VITE_APP_BASEAPI,
    timeout: 50000,
    // transformResponse: [data => {
    //     try {
    //         const json = JsonBig({
    //             storeAsString: true
    //           })
    //         return json.parse(data)
    //       } catch (err) {
    //         // 如果转换失败，则包装为统一数据格式并返回
    //         return {
    //           data
    //         }
    //       }
    // }],
})



// 请求拦截器
myaxios.interceptors.request.use(function (config) {
    if (getToken()) {
        config.headers['Authorization'] = 'Bearer ' + getToken() // 让每个请求携带自定义token 请根据实际情况自行修改
      }

    return config;
}, function (error) {
    return Promise.reject(error);
});

// 响应拦截器
myaxios.interceptors.response.use(function (response) {
    //业务错误
    if(response.data.statusCode==403)
    {
        ElMessage.error(response.data.errors)
    }
    return response;
}, function (error) {
    const code = error.response.status;
    const msg = error.message;
//业务异常+应用异常，统一处理
 switch(code) 
 {
    case 401:
    ElMessage.error('登录已过期')
    break;
    case 403:  
    ElMessage.error(msg)
    break;
    case 500:
    ElMessage.error(msg)    
    break;
 }

    return Promise.reject(error);
});
export default myaxios