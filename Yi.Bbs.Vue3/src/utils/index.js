//设置防抖，保证无论拖动窗口大小，只执行一次获取浏览器宽高的方法
export const debounce = (fun, delay) => {
  let timer;
  return function () {
    if (timer) {
      clearTimeout(timer);
    }
    timer = setTimeout(() => {
      fun();
    }, delay);
  };
};
