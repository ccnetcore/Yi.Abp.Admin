using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Abp.Tool.Commands
{
    public class ClearCommand : ICommand
    {
        public List<string> CommandStrs => ["clear"];

        public Task InvokerAsync(Dictionary<string, string> options, string[] args)
        {
            List<string> delDirBlacklist = ["obj", "bin"];
            options.TryGetValue("path", out var path);

            if (string.IsNullOrEmpty(path))
            {
                path = "./";
            }
            DeleteObjBinFolders(path, delDirBlacklist);
            return Task.CompletedTask;
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
