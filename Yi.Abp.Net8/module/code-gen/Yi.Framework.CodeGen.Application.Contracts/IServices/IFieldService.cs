using Yi.Framework.CodeGen.Application.Contracts.Dtos.Field;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.CodeGen.Application.Contracts.IServices
{
    public interface IFieldService : IYiCrudAppService<FieldDto, Guid, FieldGetListInput>
    {
    }
}
