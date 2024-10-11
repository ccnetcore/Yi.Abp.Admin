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
        private ISqlSugarRepository<AgreeEntity> _agreeRepository;
        public BbsForumAnalyseService(ForumManager forumManager, ISqlSugarRepository<AgreeEntity> agreeRepository)
        {
            _forumManager = forumManager;
            _agreeRepository = agreeRepository;
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
                               // IsAgree = SqlFunc.Subqueryable<AgreeEntity>().WhereIF(CurrentUser.Id != null, x => x.CreatorId == CurrentUser.Id && x.DiscussId == discuss.Id).Any(),

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
            var discussId = output.Select(x => x.Id);
            //点赞字典，key为主题id，y为用户ids
            var agreeDic =
                (await _agreeRepository._DbQueryable.Where(x => discussId.Contains(x.DiscussId)).ToListAsync())
                .GroupBy(x => x.DiscussId)
                .ToDictionary(x => x.Key, y => y.Select(y => y.CreatorId).ToList());
            
            //等级、是否点赞赋值
            output?.ForEach(x =>
            {
                if (CurrentUser.Id is not null)
                {
                    //默认fasle
                    if (agreeDic.TryGetValue(x.Id,out var userIds))
                    {
                        x.IsAgree = userIds.Contains(CurrentUser.Id);
                    }
                }
            });
            
            return output;
        }


    }
}
