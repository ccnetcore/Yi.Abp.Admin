import request from "@/config/axios/service";

//接受任务
export function acceptAssignment(id) {
    return request({
        url: `/assignment/accept/${id}`,
        method: "post"
    });
}
//领取奖励
export function receiveAssignment(id) {
    return request({
        url: `/assignment/complete/${id}`,
        method: "post",
    });
}

//查询能够领取的任务
export function getCanReceiveAssignment() {
    return request({
        url: `/assignment/receive`,
        method: "get",
    });
}

//查询已领取的任务
export function getAssignmentList(data) {
    return request({
        url: `/assignment`,
        method: "get",
        params:data
    });
}