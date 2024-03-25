import signalR from "@/utils/signalR";
import useSocketStore from "@/stores/socket.js";
const receiveMsg=(connection)=> {
    connection.on("onlineNum", (data) => {
      const socketStore = useSocketStore();
      socketStore.setOnlineNum(data);
    });
    connection.on("forceOut", (msg) => {
      useUserStore()
        .logOut()
        .then(() => {
          alert(msg);
          location.href = "/index";
        });
    });
  };

  export default ()=>{
     signalR.start(`main`,receiveMsg);
} 


