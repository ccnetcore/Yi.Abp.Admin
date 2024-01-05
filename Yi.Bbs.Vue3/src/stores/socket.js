import { defineStore } from "pinia";

const socketStore = defineStore("socket", {
  state: () => ({
    onlineNum: 1,
  }),
  actions: {
    // 获取在线总数
    getOnlineNum() {
      return this.onlineNum;
    },
    // 设置在线总数
    setOnlineNum(value) {
      this.onlineNum = value;
    },
  },
  persist: {
    key: "onlineInfo",
    storage: window.sessionStorage,
  },
});

export default socketStore;
