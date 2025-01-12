using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Yi.Abp.Tool.Commands
{
    public class AddModuleCommand : ICommand
    {
        public string Command => "add-module";
        public string? Description => "将内容添加到当前解决方案` yi-abp add-module <moduleName> [-p <path>] [-s <solution>] ";
        public void CommandLineApplication(CommandLineApplication application)
        {
            application.HelpOption("-h|--help");
            var modulePathOption=  application.Option("-p|--modulePath", "模块路径",CommandOptionType.SingleValue);
            var solutionOption=  application.Option("-s|--solution", "解决方案路径",CommandOptionType.SingleValue);
            var moduleNameArgument = application.Argument("moduleName", "模块名", (_) => { });
            application.OnExecute(() =>
            {
                var moduleName = moduleNameArgument.Value;
  
                //模块路径默认按小写规则，默认在模块路径下一层
                var modulePath =moduleName.ToLower().Replace(".", "-");
                if (modulePathOption.HasValue())
                {
                    modulePath = modulePathOption.Value();
                }
                
                
                //解决方案默认在模块文件夹上一级，也可以通过s进行指定
                var slnPath = "../";
                
                if (solutionOption.HasValue())
                {
                    slnPath = solutionOption.Value();
                }
                
                CheckFirstSlnPath(slnPath);
                var dotnetSlnCommandPart1 = $"dotnet sln \"{slnPath}\" add \"{modulePath}\\{moduleName}.";
                var dotnetSlnCommandPart2 = new List<string>() { "Application", "Application.Contracts", "Domain", "Domain.Shared", "SqlSugarCore" };
                var paths = dotnetSlnCommandPart2.Select(x => $@"{modulePath}\{moduleName}." + x).ToArray();
                CheckPathExist(paths);
                
                var cmdCommands = dotnetSlnCommandPart2.Select(x => dotnetSlnCommandPart1 + x+"\"").ToArray();
                StartCmd(cmdCommands);
                
                Console.WriteLine("恭喜~模块添加成功！");
                return 0;
            });
            
        }
        
        /// <summary>
        /// 获取一个sln解决方案，多个将报错
        /// </summary>
        /// <returns></returns>
        private string CheckFirstSlnPath(string slnPath)
        {
            string[] slnFiles = Directory.GetFiles(slnPath, "*.sln");
            if (slnFiles.Length > 1)
            {
                throw new UserFriendlyException("当前目录包含多个sln解决方案，请只保留一个");
            }
            if (slnFiles.Length == 0)
            {
                throw new UserFriendlyException("当前目录未找到sln解决方案，请检查");
            }

            return slnFiles[0];
        }


        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="cmdCommands"></param>
        private void StartCmd(params string[] cmdCommands)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c chcp 65001&{string.Join("&", cmdCommands)}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process proc = new Process
            {
                StartInfo = psi
            };

            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            proc.WaitForExit();
        }


        /// <summary>
        /// 检查路径
        /// </summary>
        /// <param name="paths"></param>
        /// <exception cref="UserFriendlyException"></exception>
        private void CheckPathExist(string[] paths)
        {
            foreach (string path in paths)
            {
                if (!Directory.Exists(path))
                {
                    throw new UserFriendlyException($"路径错误，请检查你的路径，找不到：{path}");
                }
            }
        }


    }
}
