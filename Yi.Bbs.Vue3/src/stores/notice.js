import { defineStore } from "pinia";
const chatStore = defineStore("notice", {
  state: () => ({
    noticeList: []
  }),
  getters: {
     noticeForNoReadCount:(state)=>{
      return state.noticeList.filter(x => x.isRead ==false).length;  
     }
  
  },
  actions:
  {
    addNotice(msg) {
      this.noticeList.unshift(msg);
    },
    addNotices(msgs) {
      
      msgs.forEach(item => {
        this.addNotice(item);
      });
     },
     setNotices(msgs) {
    this.noticeList=msgs;
     },
    removeNotice(id)
    {
       this.noticeList = this.noticeList.filter(obj => obj.id != id);
    }
  },
});

export default chatStore;
