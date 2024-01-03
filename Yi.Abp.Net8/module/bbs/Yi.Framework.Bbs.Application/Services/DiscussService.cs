using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Extensions;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services
{
    /// <summary>
    /// Discuss应用服务实现,用于参数效验、领域服务业务组合、日志记录、事务处理、账户信息
    /// </summary>
    public class DiscussService : YiCrudAppService<DiscussEntity, DiscussGetOutputDto, DiscussGetListOutputDto, Guid, DiscussGetListInputVo, DiscussCreateInputVo, DiscussUpdateInputVo>,
       IDiscussService
    {
        private ISqlSugarRepository<DiscussTopEntity> _discussTopEntityRepository;
        public DiscussService(ForumManager forumManager, ISqlSugarRepository<DiscussTopEntity> discussTopEntityRepository, ISqlSugarRepository<PlateEntity> plateEntityRepository, ILocalEventBus localEventBus) : base(forumManager._discussRepository)
        {
            _forumManager = forumManager;
            _plateEntityRepository = plateEntityRepository;
            _localEventBus = localEventBus;
            _discussTopEntityRepository = discussTopEntityRepository;
        }
        private readonly ILocalEventBus _localEventBus;
        private ForumManager _forumManager { get; set; }


        private ISqlSugarRepository<PlateEntity> _plateEntityRepository { get; set; }




        /// <summary>
        /// 单查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async override Task<DiscussGetOutputDto> GetAsync(Guid id)
        {

            //查询主题发布 浏览主题 事件，浏览数+1
            var item = await _forumManager._discussRepository._DbQueryable.LeftJoin<UserEntity>((discuss, user) => discuss.CreatorId == user.Id)
                .LeftJoin<BbsUserExtraInfoEntity>((discuss, user, info) => user.Id == info.UserId)
                     .Select((discuss, user, info) => new DiscussGetOutputDto
                     {
                         Id=discuss.Id,
                         IsAgree = SqlFunc.Subqueryable<AgreeEntity>().WhereIF(CurrentUser.Id != null, x => x.CreatorId == CurrentUser.Id && x.DiscussId == discuss.Id).Any(),
                         User = new BbsUserGetListOutputDto()
                         {
                             UserName = user.UserName,
                             Nick = user.Nick,
                             Icon = user.Icon,
                             Id = user.Id,
                             Level = info.Level,
                             UserLimit = info.UserLimit
                         }
                     }, true)
                     .SingleAsync(discuss => discuss.Id == id);

            if (item is not null)
            {
                await VerifyDiscussPermissionAsync(item.Id);
                await _localEventBus.PublishAsync(new SeeDiscussEventArgs { DiscussId = item.Id, OldSeeNum = item.SeeNum });
            }

            return item;
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<DiscussGetListOutputDto>> GetListAsync([FromQuery] DiscussGetListInputVo input)
        {
            //需要关联创建者用户
            RefAsync<int> total = 0;
            var items = await _forumManager._discussRepository._DbQueryable
                 .WhereIF(!string.IsNullOrEmpty(input.Title), x => x.Title.Contains(input.Title))
                     .WhereIF(input.PlateId is not null, x => x.PlateId == input.PlateId)


                     .WhereIF(input.IsTop == true, x => x.IsTop == input.IsTop)

                     .LeftJoin<UserEntity>((discuss, user) => discuss.CreatorId == user.Id)
                         .LeftJoin<BbsUserExtraInfoEntity>((discuss, user, info) => user.Id == info.UserId)

                         .OrderByDescending(discuss => discuss.OrderNum)
                      .OrderByIF(input.Type == QueryDiscussTypeEnum.New, discuss => discuss.CreationTime, OrderByType.Desc)
                     .OrderByIF(input.Type == QueryDiscussTypeEnum.Host, discuss => discuss.SeeNum, OrderByType.Desc)
                      .OrderByIF(input.Type == QueryDiscussTypeEnum.Suggest, discuss => discuss.AgreeNum, OrderByType.Desc)

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
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

            //查询完主题之后，要过滤一下私有的主题信息
            items.ApplyPermissionTypeFilter(CurrentUser.Id ?? Guid.Empty);
            return new PagedResultDto<DiscussGetListOutputDto>(total, items);
        }

        /// <summary>
        /// 获取首页的置顶主题
        /// </summary>
        /// <returns></returns>
        public async Task<List<DiscussGetListOutputDto>> GetListTopAsync()
        {
            var output = await _discussTopEntityRepository._DbQueryable.LeftJoin<DiscussEntity>((top, discuss) => top.DiscussId == discuss.Id)
                .LeftJoin<UserEntity>((top, discuss, user) => discuss.CreatorId == user.Id)
                .LeftJoin<BbsUserExtraInfoEntity>((top, discuss, user, info) => user.Id == info.UserId)
                .OrderByDescending(top => top.OrderNum)
                .Select((top, discuss, user, info) => new DiscussGetListOutputDto
                {
                    Id = discuss.Id,
                    IsAgree = SqlFunc.Subqueryable<AgreeEntity>().WhereIF(CurrentUser.Id != null, x => x.CreatorId == CurrentUser.Id && x.DiscussId == discuss.Id).Any(),
                    User = new BbsUserGetListOutputDto
                    {
                        Id = user.Id,
                        Name=user.Name,
                        Sex = user.Sex,
                        State = user.State,
                        Address = user.Address,
                        Age = user.Age,
                        CreationTime = user.CreationTime,
                   
                        Level =info.Level,
                       Introduction = user.Introduction,
                       Icon= user.Icon,
                       Nick= user.Nick,
                       UserName=user.UserName,
                       Remark= user.Remark,
                       UserLimit=info.UserLimit
                       
                    }
                }, true)
                .ToListAsync();
            return output;
        }

        /// <summary>
        /// 创建主题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Permission("bbs:discuss:add")]
        [Authorize]
        public override async Task<DiscussGetOutputDto> CreateAsync(DiscussCreateInputVo input)
        {
            var plate = await _plateEntityRepository.FindAsync(x => x.Id == input.PlateId);
            if (plate is null)
            {
                throw new UserFriendlyException(PlateConst.No_Exist);
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

            var entity = await _forumManager.CreateDiscussAsync(await MapToEntityAsync(input));
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 效验主题查询权限
        /// </summary>
        /// <param name="discussId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task VerifyDiscussPermissionAsync(Guid discussId)
        {
            var discuss = await _forumManager._discussRepository.GetFirstAsync(x => x.Id == discussId);
            if (discuss is null)
            {
                throw new UserFriendlyException(DiscussConst.No_Exist);
            }
            if (discuss.PermissionType == DiscussPermissionTypeEnum.Oneself)
            {
                if (discuss.CreatorId != CurrentUser.Id)
                {
                    throw new UserFriendlyException(DiscussConst.Privacy);
                }
            }
            if (discuss.PermissionType == DiscussPermissionTypeEnum.User)
            {
                if (discuss.CreatorId != CurrentUser.Id && !discuss.PermissionUserIds.Contains(CurrentUser.Id ?? Guid.Empty))
                {
                    throw new UserFriendlyException(DiscussConst.Privacy);
                }
            }
        }
    }
}
