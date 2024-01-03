/**
 * 根据后缀判断文件类型
 * @param {string} fileName 文件后缀名
 * @returns
 */
export function matchType(fileName) {
  // 后缀获取
  let suffix = "";
  // 获取类型结果
  let result = "";
  try {
    let flieArr = fileName.split(".");
    suffix = flieArr[flieArr.length - 1];
  } catch (err) {
    suffix = "";
  }
  // fileName无后缀返回 false
  if (!suffix) {
    result = false;
    return result;
  }
  // 图片格式
  let imglist = ["png", "jpg", "jpeg", "bmp", "gif"];
  // 进行图片匹配
  result = imglist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "image";
    return result;
  }
  // 匹配txt
  let txtlist = ["txt"];
  result = txtlist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "txt";
    return result;
  }
  // 匹配 excel
  let excelist = ["xls", "xlsx"];
  result = excelist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "excel";
    return result;
  }
  // 匹配 word
  let wordlist = ["doc", "docx"];
  result = wordlist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "word";
    return result;
  }
  // 匹配 pdf
  let pdflist = ["pdf"];
  result = pdflist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "pdf";
    return result;
  }
  // 匹配 ppt
  let pptlist = ["ppt"];
  result = pptlist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "ppt";
    return result;
  }
  // 匹配 视频
  let videolist = ["mp4", "m2v", "mkv"];
  result = videolist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "video";
    return result;
  }
  // 匹配 音频
  let radiolist = ["mp3", "wav", "wmv"];
  result = radiolist.some(function (item) {
    return item == suffix;
  });
  if (result) {
    result = "radio";
    return result;
  }
  // 其他 文件类型
  result = "other";
  return result;
}

/**
 * url处理
 * @param {string} path url路径
 * @returns
 */
export function convertToUrl(path) {
  // 替换反斜杠为正斜杠
  const normalizedPathWithSlashes = path.replace(/\\/g, "/");
  // 去掉开始的点号和反斜杠
  const removedDotsAndSlashes = normalizedPathWithSlashes.replace(/^\.\//, "");
  // 添加斜杠作为根路径
  const url = `/${removedDotsAndSlashes}`;
  return url;
}

/**
 * 下载文件
 *
 * @param {*} path 下载地址/下载请求地址。
 * @param {string} name 下载文件的名字（考虑到兼容性问题，最好加上后缀名
 */
export const downLoadFile = (path, name) => {
  const link = document.createElement("a");
  link.href = path;
  link.download = name;

  if (isMobileDevice()) {
    link.target = "_blank";
    link.rel = "noopener noreferrer";
  }

  link.style.display = "none";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

// 判断是否移动设备
export function isMobileDevice() {
  return /Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent);
}
