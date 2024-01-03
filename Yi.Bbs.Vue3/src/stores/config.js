import { getAll } from "@/apis/configApi";
import { defineStore } from "pinia";
const useConfigStore = defineStore("config", {
  state: () => ({
    data: [],
  }),
  getters: {
    name: (state) =>
      state.data
        .filter((s) => s.configKey == "bbs.site.name")
        .map((x) => x.configValue)[0],
    author: (state) =>
      state.data
        .filter((s) => s.configKey == "bbs.site.author")
        .map((x) => x.configValue)[0],
    icp: (state) =>
      state.data
        .filter((s) => s.configKey == "bbs.site.icp")
        .map((x) => x.configValue)[0],
    bottom: (state) =>
      state.data
        .filter((s) => s.configKey == "bbs.site.bottom")
        .map((x) => x.configValue)[0],
  },
  actions: {
    // 登录
    async getConfig() {
      const response = await getAll();
      this.data = response.data.items;
    },
  },
  persist: {
    key: "configInfo",
    storage: window.sessionStorage,
  },
});
export default useConfigStore;
