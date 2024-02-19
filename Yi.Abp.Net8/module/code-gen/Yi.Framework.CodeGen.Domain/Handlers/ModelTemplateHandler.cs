namespace Yi.Framework.CodeGen.Domain.Handlers
{
    public class ModelTemplateHandler : TemplateHandlerBase, ITemplateHandler
    {
        public HandledTemplate Invoker(string str, string path)
        {
            var output = new HandledTemplate();
            output.TemplateStr = str.Replace("@model", char.ToLowerInvariant(Table.Name[0]) + Table.Name.Substring(1)).Replace("@Model", char.ToUpperInvariant(Table.Name[0]) + Table.Name.Substring(1));
            output.BuildPath = path.Replace("@model", char.ToLowerInvariant(Table.Name[0]) + Table.Name.Substring(1)).Replace("@Model", char.ToUpperInvariant(Table.Name[0]) + Table.Name.Substring(1));
            return output;
        }
    }
}
