using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Notice
{
    public class BbsNoticeGetListOutputDto
    {
        /// <summary>
        /// 消息,支持html
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public NoticeTypeEnum NoticeType { get; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; private set; }

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime? ReadTime { get; private set; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
