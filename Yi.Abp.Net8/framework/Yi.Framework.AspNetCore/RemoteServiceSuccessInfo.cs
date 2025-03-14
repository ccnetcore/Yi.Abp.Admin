namespace Yi.Framework.AspNetCore
{
    /// <summary>
    /// 远程服务成功响应信息
    /// </summary>
    [Serializable]
    public class RemoteServiceSuccessInfo
    {
        /// <summary>
        /// 获取或设置响应代码
        /// </summary>
        public string? Code { get; private set; }

        /// <summary>
        /// 获取或设置响应消息
        /// </summary>
        public string? Message { get; private set; }

        /// <summary>
        /// 获取或设置详细信息
        /// </summary>
        public string? Details { get; private set; }

        /// <summary>
        /// 获取或设置响应数据
        /// </summary>
        public object? Data { get; private set; }

        /// <summary>
        /// 初始化远程服务成功响应信息的新实例
        /// </summary>
        public RemoteServiceSuccessInfo()
        {
        }

        /// <summary>
        /// 使用指定参数初始化远程服务成功响应信息的新实例
        /// </summary>
        /// <param name="message">响应消息</param>
        /// <param name="details">详细信息</param>
        /// <param name="code">响应代码</param>
        /// <param name="data">响应数据</param>
        public RemoteServiceSuccessInfo(
            string message, 
            string? details = null, 
            string? code = null, 
            object? data = null)
        {
            Message = message;
            Details = details;
            Code = code;
            Data = data;
        }
    }
}
