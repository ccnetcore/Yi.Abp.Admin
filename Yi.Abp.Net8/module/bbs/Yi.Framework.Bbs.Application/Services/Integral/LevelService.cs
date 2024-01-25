using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Level;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Integral;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Integral
{
    /// <summary>
    /// 等级服务
    /// </summary>
    public class LevelService : YiCrudAppService<LevelEntity, LevelOutputDto, Guid, LevelGetListInputDto>, ILevelService
    {
        private ISqlSugarRepository<LevelEntity, Guid> _repository;
        public LevelService(ISqlSugarRepository<LevelEntity, Guid> repository) : base(repository)
        {
            _repository= repository;
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
                .WhereIF(input.MinLevel is not null , x => x.CurrentLevel>=input.MinLevel)
                .OrderBy(x => x.CurrentLevel)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<LevelOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }
    }
}
