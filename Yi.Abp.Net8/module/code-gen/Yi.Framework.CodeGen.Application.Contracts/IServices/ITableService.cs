using Yi.Framework.CodeGen.Application.Contracts.Dtos.Table;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.CodeGen.Application.Contracts.IServices
{
    public interface ITableService : IYiCrudAppService<TableDto, Guid, TableGetListInput>
    {
    }
}
