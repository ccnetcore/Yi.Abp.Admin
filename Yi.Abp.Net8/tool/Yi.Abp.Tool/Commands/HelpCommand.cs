using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Abp.Tool.Commands
{
    public class HelpCommand : ICommand
    {
        public List<string> CommandStrs => new List<string> { "h", "help", "-h", "-help" };

        public Task InvokerAsync(Dictionary<string, string> options, string[] args)
        {
            string? errorMsg = null;
            if (options.TryGetValue("error", out _))
            {
                errorMsg = "您输入的命令有误，请检查，以下帮助命令提示：";
            }
            Console.WriteLine($"""
                {errorMsg}
                使用:

                    yi-abp <command> <target> [options]

                命令列表:
                
                    > v: 查看yi-abp工具版本号
                    > help: 查看帮助列表，写下命令` yi-abp help <command> `
                    > new: 创建模块模板` yi-abp new <name> -t module -csf ` 
                    > new: 创建项目模板` yi-abp new <name> -csf `
                    > add-module: 将内容添加到当前解决方案` yi-abp add-module <moduleName> [-modulePath <path>] [-s <slnPath>] `
                    > clear: 清除当前目录及子目录下的obj、bin文件夹` yi-abp clear `

                """);
            return Task.CompletedTask;
        }
    }
}
