using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Volo.Abp.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yi.Abp.Tool
{
    public class CommandInvoker : ISingletonDependency
    {
        private readonly IEnumerable<ICommand> _commands;
        private CommandLineApplication Application { get; }

        public CommandInvoker(IEnumerable<ICommand> commands)
        {
            _commands = commands;
            Application = new CommandLineApplication();
            InitCommand();
        }

        private void InitCommand()
        {    
            Application.HelpOption("-h|--help");
            Application.VersionOption("-v|--versions", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            foreach (var command in _commands)
            {
                CommandLineApplication childrenCommandLineApplication = new CommandLineApplication(true)
                {
                    Name = command.Command,
                    Parent = Application,
                    Description =command.Description
                };
                Application.Commands.Add(childrenCommandLineApplication);
                command.CommandLineApplication(childrenCommandLineApplication);
            }
        }

        public async Task InvokerAsync(string[] args)
        {
            Application.Execute(args);
        }
    }
}