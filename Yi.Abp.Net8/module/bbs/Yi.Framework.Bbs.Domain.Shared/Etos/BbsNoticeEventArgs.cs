using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Domain.Shared.Etos
{
    public class BbsNoticeEventArgs
    {
        /// <summary>
        /// 发送个人消息
        /// </summary>
        /// <param name="acceptUserId"></param>
        /// <param name="message"></param>
        public BbsNoticeEventArgs(Guid acceptUserId, string message)
        {
            NoticeType = NoticeTypeEnum.Personal;
            AcceptUserId = acceptUserId;
            Message = message;
        }

        /// <summary>
        /// 发送广播
        /// </summary>
        /// <param name="message"></param>
        public BbsNoticeEventArgs(string message)
        {
            NoticeType = NoticeTypeEnum.Broadcast;
            Message = message;
        }
        public NoticeTypeEnum NoticeType { get; private set; }

        public string Message { get; private set; }

        public Guid? AcceptUserId { get; private set; }
    }
}
