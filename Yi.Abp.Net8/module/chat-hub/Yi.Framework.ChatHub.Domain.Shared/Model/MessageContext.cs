using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Yi.Framework.ChatHub.Domain.Shared.Enums;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.ChatHub.Domain.Shared.Model
{

    public static class MessageContextExtensions
    {
        public static List<Guid> GetUserIds(this List<MessageContext> context)
        {
            return context.Where(x => x.ReceiveId is not null).Select(x => x.ReceiveId!.Value).Union(context.Select(x => x.SendUserId)).ToList();
        }
        public static List<MessageContext> MapperUserInfo(this List<MessageContext> messageContexts, List<UserRoleMenuDto> userRoleMenuDtos)
        {
            var userInfoDic = userRoleMenuDtos.Select(x => new UserRoleMenuDto { User = x.User }).ToDictionary(x => x.User.Id);
            foreach (var context in messageContexts)
            {
                if (context.ReceiveId is not null)
                {
                    context.ReceiveInfo = userInfoDic[context.ReceiveId.Value];
                }
                context.SendUserInfo = userInfoDic[context.SendUserId];
            }
            return messageContexts;
        }
    }
    public class MessageContext
    {
        /// <summary>
        /// 映射用户信息
        /// </summary>

        public static MessageContext CreatePersonal(string content, Guid userId, Guid sendUserId)
        {
            return new MessageContext() { MessageType = MessageTypeEnum.Personal, Content = content, ReceiveId = userId, SendUserId = sendUserId };
        }


        public static MessageContext CreateAi(string? content, Guid receiveId, Guid id )
        {
            return new MessageContext() { MessageType = MessageTypeEnum.Ai, Content = content??string.Empty, ReceiveId = receiveId, Id = id };
        }

        public static MessageContext CreateAll(string content, Guid sendUserId)
        {
            return new MessageContext() { MessageType = MessageTypeEnum.All, Content = content, SendUserId = sendUserId };
        }

        public void SetUserInfo(UserRoleMenuDto sendUserInfo, UserRoleMenuDto? receiveInfo)
        {
            this.SendUserInfo = sendUserInfo;
            this.ReceiveInfo = receiveInfo;

        }
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageTypeEnum MessageType { get; set; }
        /// <summary>
        /// 接收者(用户id、群组id)
        /// </summary>
        public Guid? ReceiveId { get; set; }

        /// <summary>
        /// 接收者用户信息
        /// </summary>
        public UserRoleMenuDto? ReceiveInfo { get; set; }

        /// <summary>
        /// 发送者的用户id
        /// </summary>
        public Guid SendUserId { get; set; }

        /// <summary>
        /// 发送者用户信息
        /// </summary>
        public UserRoleMenuDto SendUserInfo { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 上下文Id，主要用于ai流式传输
        /// </summary>
        public Guid Id { get; set; }
        public DateTime CreationTime { get; protected set; } = DateTime.Now;
    }


}
