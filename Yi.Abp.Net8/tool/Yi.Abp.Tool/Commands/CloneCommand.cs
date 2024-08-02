using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Abp.Tool.Commands
{
    public class CloneCommand : ICommand
    {
        public List<string> CommandStrs => new List<string> { "clone"};

        private const string cloneAddress= "https://gitee.com/ccnetcore/Yi";
        public Task InvokerAsync(Dictionary<string, string> options, string[] args)
        {
            StartCmd($"git clone {cloneAddress}");
            return Task.CompletedTask;
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
    }
}
