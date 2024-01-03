namespace Yi.Framework.AspNetCore
{
    [Serializable]
    public class RemoteServiceSuccessInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteServiceSuccessInfo"/>.
        /// </summary>
        public RemoteServiceSuccessInfo()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteServiceSuccessInfo"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="details">Error details</param>
        /// <param name="message">Error message</param>
        /// <param name="data">Error data</param>
        public RemoteServiceSuccessInfo(string message, string? details = null, string? code = null, object? data = null)
        {
            Message = message;
            Details = details;
            Code = code;
            Data = data;
        }

        /// <summary>
        /// code.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// message.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// details.
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// data.
        /// </summary>
        public object? Data { get; set; }

    }
}
