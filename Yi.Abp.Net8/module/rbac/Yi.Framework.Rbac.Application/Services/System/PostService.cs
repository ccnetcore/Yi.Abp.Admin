using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Post;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Application.Services.System
{
    /// <summary>
    /// Post服务实现
    /// </summary>
    public class PostService : YiCrudAppService<PostAggregateRoot, PostGetOutputDto, PostGetListOutputDto, Guid,
            PostGetListInputVo, PostCreateInputVo, PostUpdateInputVo>,
        IPostService
    {
        private readonly ISqlSugarRepository<PostAggregateRoot, Guid> _repository;

        public PostService(ISqlSugarRepository<PostAggregateRoot, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        public override async Task<PagedResultDto<PostGetListOutputDto>> GetListAsync(PostGetListInputVo input)
        {
            RefAsync<int> total = 0;

            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.PostName),
                    x => x.PostName.Contains(input.PostName!))
                .WhereIF(input.State is not null, x => x.State == input.State)
                .OrderByDescending(x => x.OrderNum)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<PostGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }

        protected override async Task CheckCreateInputDtoAsync(PostCreateInputVo input)
        {
            var isExist =
                await _repository.IsAnyAsync(x => x.PostCode == input.PostCode);
            if (isExist)
            {
                throw new UserFriendlyException(PostConst.Exist);
            }
        }

        protected override async Task CheckUpdateInputDtoAsync(PostAggregateRoot entity, PostUpdateInputVo input)
        {
            var isExist = await _repository._DbQueryable.Where(x => x.Id != entity.Id)
                .AnyAsync(x => x.PostCode == input.PostCode);
            if (isExist)
            {
                throw new UserFriendlyException(RoleConst.Exist);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [Route("post/{id}/{state}")]
        public async Task<PostGetOutputDto> UpdateStateAsync([FromRoute] Guid id, [FromRoute] bool state)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
            {
                throw new ApplicationException("岗位未存在");
            }

            entity.State = state;
            await _repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }
    }
}