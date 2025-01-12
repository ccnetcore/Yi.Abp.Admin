using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Volo.Abp.DependencyInjection;

namespace Yi.Abp.Tool
{
    public interface ICommand:ISingletonDependency
    {
        public string Command { get; }

        public string? Description { get; }
        void CommandLineApplication(CommandLineApplication application);

    }
}
