using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Yi.Abp.Tool.Commands
{
    public class ClearCommand : ICommand
    {
        public List<string> CommandStrs => ["clear"];
      

        public string Command => "clear";
        public string? Description => "清除当前目录及子目录下的obj、bin文件夹` yi-abp clear `";

        public void CommandLineApplication(CommandLineApplication application)
        {
            application.HelpOption("-h|--help");
            List<string> delDirBlacklist = ["obj", "bin"];
            var pathOption=  application.Option("-path", "路径",CommandOptionType.SingleValue);


            application.OnExecute(() =>
            {
                var path = "./";
                if (pathOption.HasValue())
                {
                    path = pathOption.Value();
                }
                DeleteObjBinFolders(path, delDirBlacklist);
                return 0;
            });
        }
        
        
        private static void DeleteObjBinFolders(string directory, List<string> delDirBlacklist)
        {
            try
            {
                foreach (string subDir in Directory.GetDirectories(directory))
                {
                    if (delDirBlacklist.Contains(Path.GetFileName( subDir)))
                    {
                        Directory.Delete(subDir, true);
                        Console.WriteLine($"已删除文件夹：{subDir}");
                    }
                    else
                    {
                        DeleteObjBinFolders(subDir, delDirBlacklist);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法删除文件夹：{directory}，错误信息: {ex.Message}");
            }
        }
    }
}
