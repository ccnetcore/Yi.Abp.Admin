using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yi.Abp.Tool
{
    public class CommandSelector : ITransientDependency
    {
        private readonly IEnumerable<ICommand> _commands;
        public CommandSelector(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }
        public async Task SelectorAsync(string[] args)
        {
            //不指定命令，默认给help
            if (args.Length == 0)
            {
               await SelectorDefaultCommandAsync();
                return;
            }
            var commandStr = args[0];

            var commandOrNull = _commands.Where(x => x.CommandStrs.Select(x => x.ToUpper()).Contains(commandStr.ToUpper())).FirstOrDefault();

            //没有匹配到命令，，默认给help
            if (commandOrNull == null)
            {
                await SelectorDefaultCommandAsync();
                return;
            }

            var options = new Dictionary<string, string?>();

            //去除命令，剩下进行参数装载
            string[] commonArgs = args.Skip(1).ToArray();
            for (var i = 0; i < commonArgs.Length; i++)
            {
                var currentArg = commonArgs[i];
                //命令参数以-或者--开头
                if (IsCommandArg(currentArg))
                {
                    string? commonValue = null;
                    //参数值在他的下一位
                    if (i + 1 < commonArgs.Length)
                    {
                        var nextArg = commonArgs[i + 1];
                        if (!IsCommandArg(nextArg))
                        {
                            commonValue = nextArg;
                        }

                    }
                    //删除-就是参数名
                    options.Add(ArgToCommandMap(currentArg), commonValue);
                }



            }
            await commandOrNull.InvokerAsync(options,args);

        }
        /// <summary>
        /// 判断是否为命令参数
        /// </summary>
        /// <returns></returns>
        private bool IsCommandArg(string arg)
        {
            if (arg.StartsWith("-") || arg.StartsWith("--"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 参数到命令的转换
        /// </summary>
        /// <returns></returns>
        private string ArgToCommandMap(string arg)
        {
            return arg.TrimStart('-').TrimStart('-');
        }

        /// <summary>
        /// 选择默认命令
        /// </summary>
        /// <returns></returns>
        private async Task SelectorDefaultCommandAsync()
        {
            await SelectorAsync(["-h", "-error"]);
        }
    }
}
