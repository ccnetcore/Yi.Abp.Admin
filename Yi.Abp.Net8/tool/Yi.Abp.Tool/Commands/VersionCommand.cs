namespace Yi.Abp.Tool.Commands
{
    public class VersionCommand : ICommand
    {
        public List<string> CommandStrs => new List<string> { "version", "v", "-version", "-v" };

        public Task InvokerAsync(Dictionary<string, string> options, string[] args)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Yi-ABP TOOL {version}");
            return Task.CompletedTask;
        }
    }
}
