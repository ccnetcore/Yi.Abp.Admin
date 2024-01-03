using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
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
        public string Types { get; set; }
        public string? Introduction { get; set; }

        public int AgreeNum { get; set; }
        public int SeeNum { get; set; }

        //批量查询，不给内容，性能考虑
        //public string Content { get; set; }
        public string? Color { get; set; }

        public Guid PlateId { get; set; }

        //是否置顶，默认false
        public bool IsTop { get; set; }

        public DiscussPermissionTypeEnum PermissionType { get; set; }
        //是否禁止，默认false
        public bool IsBan { get; set; }


        /// <summary>
        /// 封面
        /// </summary>
        public string? Cover { get; set; }

        //私有需要判断code权限
        public string? PrivateCode { get; set; }
        public DateTime CreationTime { get; set; }

        public List<Guid>? PermissionUserIds { get; set; }

        public BbsUserGetListOutputDto User { get; set; }

        public void SetBan()
        {
            Title = DiscussConst.Privacy;
            Introduction = "";
            Cover = null;
            //被禁止
            IsBan = true;
        }
    }


    public static class DiscussGetListOutputDtoExtension
    {

        public static void ApplyPermissionTypeFilter(this List<DiscussGetListOutputDto> dtos, Guid userId)
        {
              dtos?.ForEach(dto =>
            {
                switch (dto.PermissionType)
                {
                    case DiscussPermissionTypeEnum.Public:
                        break;
                    case DiscussPermissionTypeEnum.Oneself:
                        //当前主题是仅自己可见，同时不是当前登录用户
                        if (dto.User.Id != userId)
                        {
                            dto.SetBan();
                        }
                        break;
                    case DiscussPermissionTypeEnum.User:
                        //当前主题为部分可见，同时不是当前登录用户 也 不在可见用户列表中
                        if (dto.User.Id != userId && !dto.PermissionUserIds.Contains(userId))
                        {
                            dto.SetBan();
                        }
                        break;
                    default:
                        break;
                }
            });
        }

    }

}
