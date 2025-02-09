using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Domain.Shared.Consts;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleGetOutputDto : EntityDto<Guid>
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public Guid DiscussId { get; set; }
        public Guid ParentId { get; set; }

        public DateTime CreationTime { get; set; }
        
        public bool HasPermission { get;internal set; }

        /// <summary>
        /// 设置权限
        /// </summary>
        public void SetPassPermission()
        {
            HasPermission = true;
        }
        /// <summary>
        /// 设置无权限
        /// </summary>
        public void SetNoPermission()
        {
            HasPermission = false;
            Content=DiscussConst.Privacy;
        }
    }
}
