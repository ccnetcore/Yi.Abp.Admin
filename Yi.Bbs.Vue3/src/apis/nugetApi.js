import request from "@/config/axios/service";
export function GetResult() {
  return request({
    url: `https://ccnetcore.com:19009/api/app/nue-get-info/info`,
    method: "get",
  });
}
