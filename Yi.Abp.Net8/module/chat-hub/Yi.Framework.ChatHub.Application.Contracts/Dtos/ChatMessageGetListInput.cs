using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.ChatHub.Domain.Shared.Enums;

namespace Yi.Framework.ChatHub.Application.Contracts.Dtos
{
    public class ChatMessageGetListInput
    {
        public MessageTypeEnum? MessageType { get; set; }

        /// <summary>
        /// 接收者(用户id、群组id)
        /// </summary>
        public Guid? ReceiveId { get; set; }

        /// <summary>
        /// 发送者的用户id
        /// </summary>
        public Guid? SendUserId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
