using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Shared.Consts
{
    /// <summary>
    /// 常量定义
    /// </summary>

    public class DiscussConst
    {
        public const string No_Exist = "传入的主题id不存在";

        public const string Privacy = "【私密】您无该主题权限，可联系作者申请开放";

        public const string AgreeNotice = "您的主题[{0}]被[{1}]用户点赞！得到一致认可，发现宝藏内容！";

        public const string CommentNotice = "您的主题[{0}]被[{1}]用户评论!评论内容：[{2}]";
    }
}
