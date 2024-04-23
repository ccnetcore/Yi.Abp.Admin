import { defineStore } from "pinia";
const chatStore = defineStore("chat", {
  state: () => ({
    userList: [],
    msgList: []
  }),
    getters: {
      allMsgContext: (state) => state.msgList.filter(x=>x.messageType=="All"),
      personalMsgContext: (state) => state.msgList.filter(x=>x.messageType=="Personal"),
    },
  actions:
  {
    setMsgList(value) {
      this.msgList = value;
    },
    addMsg(msg) {
      this.msgList.push(msg);
    },
    setUserList(value) {
      this.userList = value;
    },
    addUser(user) {
      this.userList.push(user);
    },
    delUser(userId) {
      this.userList = this.userList.filter(obj => obj.userId != userId);
    }
  },
});

export default chatStore;
