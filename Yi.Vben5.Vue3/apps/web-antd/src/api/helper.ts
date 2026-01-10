import { requestClient } from './request';

/**
 * @description:  contentType
 */
export const ContentTypeEnum = {
  // form-data  upload
  FORM_DATA: 'multipart/form-data;charset=UTF-8',
  // form-data qs
  FORM_URLENCODED: 'application/x-www-form-urlencoded;charset=UTF-8',
  // json
  JSON: 'application/json;charset=UTF-8',
} as const;

/**
 * 通用下载接口 封装一层
 * @param url 请求地址
 * @param data  请求参数
 * @returns blob二进制
 */
export function commonExport(url: string, data: Record<string, any>) {
  return requestClient.post<Blob>(url, data, {
    data,
    headers: { 'Content-Type': ContentTypeEnum.FORM_URLENCODED },
    isTransformResponse: false,
    responseType: 'blob',
  });
}
