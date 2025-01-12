import signalR from "@/utils/signalR";
import useNoticeStore from "@/stores/notice";
import useUserStore from "@/stores/user";
import { dayjs } from 'element-plus'
const receiveMsg=(connection)=> {

  const noticeStore = useNoticeStore();
    connection.on("Personal", (message,creationTime) => {
      noticeStore.addNotice({
        message:message,
        isRead:false,
       creationTime
      });
      });


    const  userStore=useUserStore();
    connection.on("Money", (message,creationTime) => {
      const  updateMoneyNumber=Number(message)
      userStore.updateMoney(updateMoneyNumber)
  });
  };

  export default ()=>{
     signalR.start(`bbs-notice`,receiveMsg);
} 


