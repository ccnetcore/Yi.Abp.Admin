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

        public const string AgreeNotice = """
                                          <div>
                                           <h3 class="title" style="color: #333; font-size: 18px; margin: 0 0 10px;">🍗 您的主题 [<a href="/article/{2}" target="_blank" style="color: #007BFF;text-decoration: none;">{0}</a>] 有 [{1}] 用户点赞！</h3>
                                          </div>
                                          """;

        public const string CommentNotice = """
                                            <div>
                                             <h3 class="title" style="color: #333; font-size: 18px; margin: 0 0 10px;">🍖 您的主题 [<a href="/article/{3}" target="_blank" style="color: #007BFF;text-decoration: none;">{0}</a>] 有 [{1}] 用户评论!</h3>
                                            </div>
                                            """;

        public const string CommentNoticeToReply = """
                                                   <div>
                                                    <h3 class="title" style="color: #333; font-size: 18px; margin: 0 0 10px;">🍖 您在主题 [<a href="/article/{3}" target="_blank" style="color: #007BFF;text-decoration: none;">{0}</a>] 的评论有 [{1}] 用户回复!</h3>
                                                   </div>
                                                   """;
    }
}