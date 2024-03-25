import signalR from "@/utils/signalR";
const receiveMsg=(connection)=> {
    connection.on("receiveNotice", (type, title, content) => {
        switch (type) {
          case "MerryGoRound":
            ElNotification({
              title: title,
              dangerouslyUseHTMLString: true,
              message: content,
            })
            break;
            case "Popup":
              ElNotification({
                title: title,
                dangerouslyUseHTMLString: true,
                message: content,
              })
              break;
        }
      });
  
  };

  export default ()=>{
     signalR.start(`notice`,receiveMsg);
} 


