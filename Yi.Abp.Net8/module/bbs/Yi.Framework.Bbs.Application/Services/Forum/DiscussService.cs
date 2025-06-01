using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using TencentCloud.Pds.V20210701.Models;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss;
using Yi.Framework.Bbs.Application.Contracts.Dtos.DiscussLable;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Repositories;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Extensions;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// Discuss应用服务实现,用于参数校验、领域服务业务组合、日志记录、事务处理、账户信息
    /// </summary>
    public class DiscussService : YiCrudAppService<DiscussAggregateRoot, DiscussGetOutputDto, DiscussGetListOutputDto,
            Guid, DiscussGetListInputVo, DiscussCreateInput, DiscussUpdateInput>,
        IDiscussService
    {
        private ISqlSugarRepository<DiscussTopEntity> _discussTopRepository;
        private ISqlSugarRepository<AgreeEntity> _agreeRepository;
        private BbsUserManager _bbsUserManager;
        private IDiscussLableRepository _discussLableRepository;

        public DiscussService(BbsUserManager bbsUserManager, ForumManager forumManager,
            ISqlSugarRepository<DiscussTopEntity> discussTopRepository,
            ISqlSugarRepository<PlateAggregateRoot> plateEntityRepository, ILocalEventBus localEventBus,
            ISqlSugarRepository<AgreeEntity> agreeRepository, IDiscussLableRepository discussLableRepository) : base(
            forumManager._discussRepository)
        {
            _forumManager = forumManager;
            _plateEntityRepository = plateEntityRepository;
            _localEventBus = localEventBus;
            _agreeRepository = agreeRepository;
            _discussLableRepository = discussLableRepository;
            _discussTopRepository = discussTopRepository;
            _bbsUserManager = bbsUserManager;
        }

        private readonly ILocalEventBus _localEventBus;
        private ForumManager _forumManager { get; set; }


        private ISqlSugarRepository<PlateAggregateRoot> _plateEntityRepository { get; set; }

        /// <summary>
        /// 单查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<DiscussGetOutputDto> GetAsync(Guid id)
        {
            //查询主题发布 浏览主题 事件，浏览数+1
            var output = await _forumManager._discussRepository._DbQueryable
                .LeftJoin<UserAggregateRoot>((discuss, user) => discuss.CreatorId == user.Id)
                .LeftJoin<BbsUserExtraInfoEntity>((discuss, user, info) => user.Id == info.UserId)
                .LeftJoin<PlateAggregateRoot>((discuss, user, info, plate) => plate.Id == discuss.PlateId)
                .Select((discuss, user, info, plate) => new DiscussGetOutputDto
                {
                    Id = discuss.Id,
                    IsAgree = false,
                    User = new BbsUserGetListOutputDto()
                    {
                        UserName = user.UserName,
                        Nick = user.Nick,
                        Icon = user.Icon,
                        Id = user.Id,
                        Level = info.Level,
                        UserLimit = info.UserLimit,
                        Money = info.Money,
                        Experience = info.Experience
                    },
                    Plate = new Contracts.Dtos.Plate.PlateGetOutputDto()
                    {
                        Name = plate.Name,
                        Id = plate.Id,
                        Code = plate.Code,
                        Introduction = plate.Introduction,
                        Logo = plate.Logo
                    }
                }, true)
                .FirstAsync(discuss => discuss.Id == id);

            if (output is null)
            {
                throw new UserFriendlyException("该主题不存在", "404");
            }

            switch (output.DiscussType)
            {
                case DiscussTypeEnum.Article: break;
                //查询的是悬赏主题
                case DiscussTypeEnum.Reward:
                    var reward = await _forumManager._discussRewardRepository.GetAsync(x => x.DiscussId == output.Id);
                    output.RewardData = reward.Adapt<DiscussRewardGetOutputDto>();
                    break;
            }


            //组装点赞
            var agreeCreatorList =
                (await _agreeRepository._DbQueryable.Where(x => x.DiscussId == output.Id).Select(x => x.CreatorId)
                    .ToListAsync());
            //已登录
            if (CurrentUser.Id is not null)
            {
                output.IsAgree = agreeCreatorList.Contains(CurrentUser.Id);
            }

            //组装标签
            var lableDic = await _discussLableRepository.GetDiscussLableCacheMapAsync();
            foreach (var lableId in output.DiscussLableIds)
            {
                if (lableDic.TryGetValue(lableId, out var item))
                {
                    output.Lables.Add(item.Adapt<DiscussLableGetOutputDto>());
                }
            }

            //如果没有权限
            if (!await _forumManager.VerifyDiscussPermissionAsync(output.Id, CurrentUser.Id, CurrentUser.Roles))
            {
                output.SetNoPermission();
            }
            else
            {
                output.SetPassPermission();
            }

            await _localEventBus.PublishAsync(new SeeDiscussEventArgs
                { DiscussId = output.Id, OldSeeNum = output.SeeNum });
            return output;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<DiscussGetListOutputDto>> GetListAsync(
            [FromQuery] DiscussGetListInputVo input)
        {
            //需要关联创建者用户
            RefAsync<int> total = 0;
            var items = await _forumManager._discussRepository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.Title), x => x.Title.Contains(input.Title))
                .WhereIF(input.PlateId is not null, x => x.PlateId == input.PlateId)
                .WhereIF(input.IsTop is not null, x => x.IsTop == input.IsTop)
                .WhereIF(input.UserId is not null, x => x.CreatorId == input.UserId)
                .LeftJoin<UserAggregateRoot>((discuss, user) => discuss.CreatorId == user.Id)
                .WhereIF(input.UserName is not null, (discuss, user) => user.UserName == input.UserName!)
                .LeftJoin<BbsUserExtraInfoEntity>((discuss, user, info) => user.Id == info.UserId)
                .OrderByDescending(discuss => discuss.OrderNum)
                //已提示杰哥新增表达式
                // .OrderByIF(input.Type == QueryDiscussTypeEnum.New, 
                //    @"COALESCE(discuss.LastModificationTime, discuss.CreationTime) DESC")
                //采用上方写法
                .OrderByIF(input.Type == QueryDiscussTypeEnum.New, discuss => discuss.CreationTime, OrderByType.Desc)
                // .OrderByIF(input.Type == QueryDiscussTypeEnum.New,discuss=>SqlFunc.Coalesce(discuss.LastModificationTime,discuss.CreationTime),OrderByType.Desc)
                .OrderByIF(input.Type == QueryDiscussTypeEnum.Host, discuss => discuss.SeeNum, OrderByType.Desc)
                .OrderByIF(input.Type == QueryDiscussTypeEnum.Suggest, discuss => discuss.AgreeNum, OrderByType.Desc)
                .Select((discuss, user, info) => new DiscussGetListOutputDto
                {
                    Id = discuss.Id,
                    // 优化查询，不使用子查询
                    // IsAgree = SqlFunc.Subqueryable<AgreeEntity>().WhereIF(CurrentUser.Id != null, x => x.CreatorId == CurrentUser.Id && x.DiscussId == discuss.Id).Any(),
                    User = new BbsUserGetListOutputDto()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Nick = user.Nick,
                        Icon = user.Icon,
                        Level = info.Level,
                        UserLimit = info.UserLimit,
                        Money = info.Money,
                        Experience = info.Experience
                    }
                }, true)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            var discussId = items.Select(x => x.Id);

            //点赞字典，key为主题id，y为用户ids
            var agreeDic =
                (await _agreeRepository._DbQueryable.Where(x => discussId.Contains(x.DiscussId)).ToListAsync())
                .GroupBy(x => x.DiscussId)
                .ToDictionary(x => x.Key, y => y.Select(y => y.CreatorId).ToList());

            var levelCacheDic = await _bbsUserManager.GetLevelCacheMapAsync();
            var lableDic = await _discussLableRepository.GetDiscussLableCacheMapAsync();

            //组装等级、是否点赞赋值、标签
            items?.ForEach(x =>
            {
                x.User.LevelName = levelCacheDic[x.User.Level].Name;
                if (CurrentUser.Id is not null)
                {
                    //默认fasle
                    if (agreeDic.TryGetValue(x.Id, out var userIds))
                    {
                        x.IsAgree = userIds.Contains(CurrentUser.Id);
                    }
                }

                foreach (var lableId in x.DiscussLableIds)
                {
                    if (lableDic.TryGetValue(lableId, out var item))
                    {
                        x.Lables.Add(item.Adapt<DiscussLableGetOutputDto>());
                    }
                }
            });
            return new PagedResultDto<DiscussGetListOutputDto>(total, items);
        }

        /// <summary>
        /// 获取首页的置顶主题
        /// </summary>
        /// <returns></returns>
        public async Task<List<DiscussGetListOutputDto>> GetListTopAsync()
        {
            var output = await _discussTopRepository._DbQueryable
                .LeftJoin<DiscussAggregateRoot>((top, discuss) => top.DiscussId == discuss.Id)
                .LeftJoin<UserAggregateRoot>((top, discuss, user) => discuss.CreatorId == user.Id)
                .LeftJoin<BbsUserExtraInfoEntity>((top, discuss, user, info) => user.Id == info.UserId)
                .OrderByDescending(top => top.OrderNum)
                .Select((top, discuss, user, info) => new DiscussGetListOutputDto
                {
                    Id = discuss.Id,
                    IsAgree = SqlFunc.Subqueryable<AgreeEntity>().WhereIF(CurrentUser.Id != null,
                        x => x.CreatorId == CurrentUser.Id && x.DiscussId == discuss.Id).Any(),
                    User = new BbsUserGetListOutputDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Sex = user.Sex,
                        State = user.State,
                        Address = user.Address,
                        Age = user.Age,
                        CreationTime = user.CreationTime,
                        Level = info.Level,
                        Introduction = user.Introduction,
                        Icon = user.Icon,
                        Nick = user.Nick,
                        UserName = user.UserName,
                        Remark = user.Remark,
                        UserLimit = info.UserLimit,
                        Money = info.Money,
                        Experience = info.Experience,
                    }
                }, true)
                .ToListAsync();
            var levelCacheDic = await _bbsUserManager.GetLevelCacheMapAsync();
            var lableDic = await _discussLableRepository.GetDiscussLableCacheMapAsync();

            output?.ForEach(x =>
            {
                x.User.LevelName = levelCacheDic[x.User.Level].Name;
                foreach (var lableId in x.DiscussLableIds)
                {
                    if (lableDic.TryGetValue(lableId, out var item))
                    {
                        x.Lables.Add(item.Adapt<DiscussLableGetOutputDto>());
                    }
                }
            });
            return output;
        }

        /// <summary>
        /// 创建主题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Permission("bbs:discuss:add")]
        [Authorize]
        public override async Task<DiscussGetOutputDto> CreateAsync(DiscussCreateInput input)
        {
            var plate = await _plateEntityRepository.FindAsync(x => x.Id == input.PlateId);
            if (plate is null)
            {
                throw new UserFriendlyException(PlateConst.No_Exist);
            }

            if (await _forumManager._discussRepository.IsAnyAsync(x => x.Title == input.Title))
            {
                throw new UserFriendlyException(DiscussConst.Repeat);
            }

            //如果开启了禁用创建主题
            if (plate.IsDisableCreateDiscuss == true)
            {
                //只有超级管理员权限才能进行发布
                if (!CurrentUser.GetPermissions().Contains(UserConst.AdminPermissionCode))
                {
                    throw new UserFriendlyException("该板块已禁止创建主题，请在其他板块中发布");
                }
            }

            await _bbsUserManager.VerifyUserLimitAsync(CurrentUser.GetId());
            var entity = await _forumManager.CreateDiscussAsync(await MapToEntityAsync(input),
                input.RewardData.Adapt<DiscussRewardAggregateRoot>());
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 设置悬赏主题已解决
        /// </summary>
        /// <param name="discussId"></param>
        /// <exception cref="UserFriendlyException"></exception>
        [HttpPut("discuss/reward/resolve/{discussId}")]
        [Authorize]
        public async Task SetRewardResolvedAsync([FromRoute] Guid discussId)
        {
            var reward = await _forumManager._discussRewardRepository.GetFirstAsync(x => x.DiscussId == discussId);
            if (reward is null)
            {
                throw new UserFriendlyException("未找到该悬赏主题", "404");
            }

            //设置已解决
            reward.SetResolved();
            await _forumManager._discussRewardRepository.UpdateAsync(reward);
        }
    }
}