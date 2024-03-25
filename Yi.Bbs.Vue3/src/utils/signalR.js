// 官方文档：https://docs.microsoft.com/zh-cn/aspnet/core/signalr/javascript-client?view=aspnetcore-6.0&viewFallbackFrom=aspnetcore-2.2&tabs=visual-studio
import * as signalR from "@microsoft/signalr";
import useAuths from "@/hooks/useAuths";

const { getToken } = useAuths();
export default {
  SR: {},
   start(url,callFunc) {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_APP_BASE_WS}/` + url, {
        headers: {
          Authorization: `Bearer ${getToken()}`,
        },
        accessTokenFactory: () => {
          // 返回授权 token
          return `${getToken()}`;
        },
      })

      .withAutomaticReconnect(new ForeverRetryPolicy()) //自动重新连接
      .configureLogging(signalR.LogLevel.Error)
      .build();
    this.SR = connection;
    // 断线重连
    connection.onclose(() => {
      console.log("hub断开");
    });

    connection.onreconnected(() => {
      console.log("hub重新连接成功");
    });
    callFunc(connection);
    // 启动

      this.SR.start();
  },

};
class ForeverRetryPolicy {
  nextRetryDelayInMilliseconds(retryContext) {
    return 1000*3;
 } 
  
}