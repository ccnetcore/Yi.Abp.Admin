using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Analyses
{
    public class BbsForumAnalyseService : ApplicationService, IApplicationService
    {
        private ForumManager _forumManager;
        public BbsForumAnalyseService(ForumManager forumManager)
        {
            _forumManager = forumManager;
        }

        /// <summary>
        /// 推荐主题，随机返回主题列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("analyse/bbs-discuss/random")]
        public async Task<List<DiscussGetListOutputDto>> GetRandomDiscussAsync([FromQuery] PagedResultRequestDto input)
        {
            var output = await _forumManager._discussRepository._DbQueryable
                .Where(discuss=>discuss.PermissionType== DiscussPermissionTypeEnum.Public)
                     .LeftJoin<UserAggregateRoot>((discuss, user) => discuss.CreatorId == user.Id)
                         .LeftJoin<BbsUserExtraInfoEntity>((discuss, user, info) => user.Id == info.UserId)

                            .OrderBy(discuss => SqlFunc.GetRandom())
                           .Select((discuss, user, info) => new DiscussGetListOutputDto
                           {
                               Id = discuss.Id,
                               IsAgree = SqlFunc.Subqueryable<AgreeEntity>().WhereIF(CurrentUser.Id != null, x => x.CreatorId == CurrentUser.Id && x.DiscussId == discuss.Id).Any(),

                               User = new BbsUserGetListOutputDto()
                               {
                                   Id = user.Id,
                                   UserName = user.UserName,
                                   Nick = user.Nick,
                                   Icon = user.Icon,
                                   Level = info.Level,
                                   UserLimit = info.UserLimit
                               }

                           }, true)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount);
            return output;
        }


    }
}
