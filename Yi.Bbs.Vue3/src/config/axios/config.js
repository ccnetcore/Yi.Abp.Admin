const config = {
  /**
   * api请求基础路径
   */
  base_url: {
    // 开发环境接口前缀
    dev: import.meta.env.VITE_APP_BASEAPI,
    // 打包生产环境接口前缀
    pro: import.meta.env.VITE_APP_BASEAPI,
  },

  /**
   * 接口请求前缀
   */
  pre_interface: import.meta.env.VITE_APP_BASEAPI,

  /**
   * 接口成功返回状态码
   */
  result_code: "0000",

  /**
   * 接口请求超时时间
   */
  request_timeout: 60000,

  /**
   * 默认接口请求类型
   * 可选值：application/x-www-form-urlencoded multipart/form-data
   */
  default_headers: "application/json",
};

export { config };
