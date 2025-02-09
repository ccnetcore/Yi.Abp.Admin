import request from "@/config/axios/service";

export function sendPersonalMessage(data) {
  return request({
    url: "/chat-message/personal",
    method: "post",
    data
  });
}

export function sendGroupMessage(data) {
    return request({
      url: "/chat-message/group",
      method: "post",
      data
    });
  }
  export function getAccountList() {
    return request({
      url: "/chat-message/account",
      method: "get"
    });
  }

  export function sendAiChat(data,model) {
    return request({
      url: `/ai-chat/chat/${model}`,
      method: "post",
      data
    });
  }
  