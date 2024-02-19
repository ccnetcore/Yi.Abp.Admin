using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.CodeGen.Application.Contracts.Dtos.Table;
using Yi.Framework.CodeGen.Application.Contracts.IServices;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.Ddd.Application;

namespace Yi.Framework.CodeGen.Application.Services
{
    public class TableService : YiCrudAppService<TableAggregateRoot, TableDto, Guid, TableGetListInput>, ITableService
    {
        public TableService(IRepository<TableAggregateRoot, Guid> repository) : base(repository)
        {
        }

        public override Task<PagedResultDto<TableDto>> GetListAsync(TableGetListInput input)
        {
            return base.GetListAsync(input);
        }
    }
}
