import request from "@/config/axios/service";

export function getAllList(data) {
    return request({
        url: "/discuss-lable/all",
        method: "get",
        params: data,
    });
}