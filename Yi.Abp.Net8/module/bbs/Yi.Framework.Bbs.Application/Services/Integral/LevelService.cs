using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Level;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Integral;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Integral
{
    /// <summary>
    /// 等级服务
    /// </summary>
    public class LevelService : YiCrudAppService<LevelAggregateRoot, LevelOutputDto, Guid, LevelGetListInputDto>, ILevelService
    {
        private ISqlSugarRepository<LevelAggregateRoot, Guid> _repository;
        private LevelManager _levelManager;
        public LevelService(ISqlSugarRepository<LevelAggregateRoot, Guid> repository, LevelManager levelManager) : base(repository)
        {
            _repository = repository;
            _levelManager = levelManager;
        }

        /// <summary>
        /// 查询等级配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<LevelOutputDto>> GetListAsync(LevelGetListInputDto input)
        {
            RefAsync<int> total = 0;

            var entities = await _repository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name!))
                .WhereIF(input.MinLevel is not null, x => x.CurrentLevel >= input.MinLevel)
                .OrderBy(x => x.CurrentLevel)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<LevelOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }

        /// <summary>
        /// 升级等级
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task UpdateUpgradeAsync(int experience)
        {
            if (experience <= 0)
            {
                throw new UserFriendlyException(LevelConst.Level_Low_Zero);
            }
            await _levelManager.ChangeLevelByMoneyAsync(CurrentUser.Id!.Value, experience);
        }
    }
}
