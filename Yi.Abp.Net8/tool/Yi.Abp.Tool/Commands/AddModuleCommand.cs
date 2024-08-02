using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Abp.Tool.Commands
{
    public class AddModuleCommand : ICommand
    {
        public List<string> CommandStrs => new List<string> { "add-module" };

        public async Task InvokerAsync(Dictionary<string, string> options, string[] args)
        {
            //只有一个add-module
            if (args.Length <= 1)
            {
                throw new UserFriendlyException("命令错误，add-module命令后必须添加 模块名");
            }

            //需要添加名称
            var moduleName = args[1];
            options.TryGetValue("modulePath", out var modulePath);

            //模块路径默认按小写规则，当前路径
            if (string.IsNullOrEmpty(modulePath))
            {
                modulePath = moduleName.ToLower().Replace(".", "-");
            }


            //解决方案默认在模块文件夹上一级，也可以通过s进行指定
            var slnPath = string.Empty;
            options.TryGetValue("s", out var slnPath1);
            options.TryGetValue("solution", out var slnPath2);
            slnPath = string.IsNullOrEmpty(slnPath1) ? slnPath2 : slnPath1;
            if (string.IsNullOrEmpty(slnPath))
            {
                slnPath = "../";
            }

            CheckFirstSlnPath(slnPath);
            var dotnetSlnCommandPart1 = $"dotnet sln \"{slnPath}\" add \"{modulePath}\\{moduleName}.";
            var dotnetSlnCommandPart2 = new List<string>() { "Application", "Application.Contracts", "Domain", "Domain.Shared", "SqlSugarCore" };
            var paths = dotnetSlnCommandPart2.Select(x => $@"{modulePath}\{moduleName}." + x).ToArray();
            CheckPathExist(paths);

            var cmdCommands = dotnetSlnCommandPart2.Select(x => dotnetSlnCommandPart1 + x+"\"").ToArray();
            StartCmd(cmdCommands);

            await Console.Out.WriteLineAsync("恭喜~模块添加成功！");
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
