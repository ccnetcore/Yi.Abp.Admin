using Yi.Framework.CodeGen.Domain.Entities;

namespace Yi.Framework.CodeGen.Domain.Handlers
{
    public class TemplateHandlerBase
    {
        protected TableAggregateRoot Table { get; set; }

        public void SetTable(TableAggregateRoot table)
        {
            Table = table;
        }
    }
}
