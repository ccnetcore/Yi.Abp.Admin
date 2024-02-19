namespace Yi.Framework.CodeGen.Domain.Handlers
{
    public class NameSpaceTemplateHandler : TemplateHandlerBase, ITemplateHandler
    {
        public HandledTemplate Invoker(string str, string path)
        {
            var output = new HandledTemplate();
            output.TemplateStr = str.Replace("@namespace", "");
            output.BuildPath = path;
            return output;
        }
    }
}
