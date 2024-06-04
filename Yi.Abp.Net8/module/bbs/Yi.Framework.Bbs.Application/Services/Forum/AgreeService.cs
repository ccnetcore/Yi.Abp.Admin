using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Argee;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// 点赞功能
    /// </summary>
    public class AgreeService : ApplicationService, IApplicationService
    {
        public AgreeService(ISqlSugarRepository<AgreeEntity> repository, ISqlSugarRepository<DiscussAggregateRoot> discssRepository)
        {
            _repository = repository;
            _discssRepository = discssRepository;
        }

        private ISqlSugarRepository<AgreeEntity> _repository { get; set; }

        private ISqlSugarRepository<DiscussAggregateRoot> _discssRepository { get; set; }


        /// <summary>
        /// 点赞,返回true为点赞+1，返回false为点赞-1
        /// Todo: 可放入领域层
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<AgreeDto> PostOperateAsync(Guid discussId)
        {
            var entity = await _repository.GetFirstAsync(x => x.DiscussId == discussId && x.CreatorId == CurrentUser.Id);
            //判断是否已经点赞过
            if (entity is null)
            {

                //没点赞过，添加记录即可，,修改总点赞数量
                await _repository.InsertAsync(new AgreeEntity(discussId));
                var discussEntity = await _discssRepository.GetByIdAsync(discussId);
                if (discussEntity is null)
                {
                    throw new UserFriendlyException("主题为空");
                }
                discussEntity.AgreeNum += 1;
                await _discssRepository.UpdateAsync(discussEntity);

                return new AgreeDto(true);

            }
            else
            {
                //点赞过，删除即可,修改总点赞数量
                await _repository.DeleteAsync(entity);
                var discussEntity = await _discssRepository.GetByIdAsync(discussId);
                if (discussEntity is null)
                {
                    throw new UserFriendlyException("主题为空");
                }
                discussEntity.AgreeNum -= 1;
                await _discssRepository.UpdateAsync(discussEntity);

                return new AgreeDto(false);
            }
        }
    }
}
