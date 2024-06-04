import signalR from "@/utils/signalR";
import useNoticeStore from "@/stores/notice";
import { dayjs } from 'element-plus'
const receiveMsg=(connection)=> {

  const noticeStore = useNoticeStore();
    connection.on("Personal", (message) => {
      noticeStore.addNotice({
        message:message,
        isRead:false,
       creationTime:dayjs().format()
      });
      });
  };

  export default ()=>{
     signalR.start(`bbs-notice`,receiveMsg);
} 


