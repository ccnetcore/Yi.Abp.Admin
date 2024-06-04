using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.ChatHub.Domain.Shared.Enums;
using Yi.Framework.ChatHub.Domain.Shared.Model;

namespace Yi.Framework.ChatHub.Domain.Entities
{
    [SugarTable("YiMessage")]
    [SugarIndex($"index_{nameof(MessageAggregateRoot)}", 
        nameof(ReceiveId), OrderByType.Asc, 
        nameof(SendUserId), OrderByType.Asc, 
        nameof(CreationTime), OrderByType.Asc)]
    public class MessageAggregateRoot : AggregateRoot<Guid>, IHasCreationTime
    {
        public static MessageContext CreatePersonal(string content, Guid userId, Guid sendUserId)
        {
            return new MessageContext() { MessageType = MessageTypeEnum.Personal, Content = content, ReceiveId = userId, SendUserId = sendUserId };
        }

        public static MessageContext CreateAll(string content, Guid sendUserId)
        {
            return new MessageContext() { MessageType = MessageTypeEnum.All, Content = content, SendUserId = sendUserId };
        }


        public string Content { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        /// <summary>
        /// 接收者(用户id、群组id)
        /// </summary>
        public Guid? ReceiveId { get; set; }

        /// <summary>
        /// 发送者的用户id
        /// </summary>
        public Guid SendUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; protected set; }
    }
}
