using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Banner;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// Banner服务实现
    /// </summary>
    public class BannerService : YiCrudAppService<BannerAggregateRoot, BannerGetOutputDto, BannerGetListOutputDto, Guid, BannerGetListInputVo, BannerCreateInputVo, BannerUpdateInputVo>,
       IBannerService
    {
        private ISqlSugarRepository<BannerAggregateRoot, Guid> _repository;
        public BannerService(ISqlSugarRepository<BannerAggregateRoot, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        public override async Task<PagedResultDto<BannerGetListOutputDto>> GetListAsync(BannerGetListInputVo input)
        {
            RefAsync<int> total = 0;
            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name!))
                          .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<BannerGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }
    }
}
