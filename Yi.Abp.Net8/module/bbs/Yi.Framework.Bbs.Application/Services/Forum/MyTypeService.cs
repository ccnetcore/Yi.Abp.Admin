using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Yi.Framework.Bbs.Application.Contracts.Dtos.MyType;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// Label服务实现
    /// </summary>
    public class MyTypeService : YiCrudAppService<MyTypeEntity, MyTypeOutputDto, MyTypeGetListOutputDto, Guid, MyTypeGetListInputVo, MyTypeCreateInputVo, MyTypeUpdateInputVo>,
       IMyTypeService
    {
        private ISqlSugarRepository<MyTypeEntity, Guid> _repository;
        public MyTypeService(ISqlSugarRepository<MyTypeEntity, Guid> repository, IDataFilter dataFilter) : base(repository)
        {
            _repository = repository;
            _dataFilter = dataFilter;
        }

        private IDataFilter _dataFilter { get; set; }

        /// <summary>
        /// 获取当前用户的主题类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PagedResultDto<MyTypeGetListOutputDto>> GetListCurrentAsync(MyTypeGetListInputVo input)
        {
            //过滤器需要更换
            //_dataFilter.Enable<MyTypeEntity>(x => x.UserId == CurrentUser.Id);

            //_dataFilter.AddFilter<MyTypeEntity>(x => x.UserId == CurrentUser.Id);
            return base.GetListAsync(input);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<MyTypeOutputDto> CreateAsync(MyTypeCreateInputVo input)
        {
            var entity = await MapToEntityAsync(input);
            entity.UserId = CurrentUser.Id ?? Guid.Empty;
            entity.IsDeleted = false;
            var outputEntity = await _repository.InsertReturnEntityAsync(entity);
            return await MapToGetOutputDtoAsync(outputEntity);
        }
    }
}
