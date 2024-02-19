using Yi.Framework.CodeGen.Application.Contracts.Dtos.Template;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.CodeGen.Application.Contracts.IServices
{
    public interface ITemplateService : IYiCrudAppService<TemplateDto, Guid, TemplateGetListInput>
    {
    }
}
