import request from "@/config/axios/service";
export function operate(discussId) {
  if (discussId == undefined) {
    return;
  }
  return request({
    url: `/agree/operate/${discussId}`,
    method: "post",
  });
}
