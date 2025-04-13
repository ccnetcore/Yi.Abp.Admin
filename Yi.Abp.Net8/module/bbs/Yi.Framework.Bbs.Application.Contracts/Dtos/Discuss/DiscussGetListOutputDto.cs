using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Application.Contracts.Dtos.DiscussLable;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss
{
    public class DiscussGetListOutputDto : EntityDto<Guid>
    {
        /// <summary>
        /// 是否禁止评论创建功能
        /// </summary>
        public bool IsDisableCreateComment { get; set; }
        /// <summary>
        /// 是否已点赞，默认未登录不点赞
        /// </summary>
        public bool IsAgree { get; set; } = false;
        public string Title { get; set; }
        public string? Introduction { get; set; }

        public int AgreeNum { get; set; }
        public int SeeNum { get; set; }

        //批量查询，不给内容，性能考虑
        //public string Content { get; set; }
        public string? Color { get; set; }

        public Guid PlateId { get; set; }

        //是否置顶，默认false
        public bool IsTop { get; set; }

        public DiscussTypeEnum DiscussType { get; set; }
        
        public DiscussPermissionTypeEnum PermissionType { get; set; }
        //是否禁止，默认false
        public bool IsBan { get; set; }


        /// <summary>
        /// 封面
        /// </summary>
        public string? Cover { get; set; }
        
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 所需角色
        /// </summary>
        public List<string>? PermissionRoleCodes { get; set; } = new List<string>();

        public BbsUserGetListOutputDto User { get; set; }
        public List<Guid>? DiscussLableIds { get; set; } = new List<Guid>();
        public List<DiscussLableGetOutputDto> Lables { get; set; } = new List<DiscussLableGetOutputDto>();
    }
}
