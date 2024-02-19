using Volo.Abp.DependencyInjection;
using Yi.Framework.CodeGen.Domain.Entities;

namespace Yi.Framework.CodeGen.Domain.Handlers
{
    public interface ITemplateHandler : ISingletonDependency
    {
        void SetTable(TableAggregateRoot table);
        HandledTemplate Invoker(string str, string path);
    }
}
