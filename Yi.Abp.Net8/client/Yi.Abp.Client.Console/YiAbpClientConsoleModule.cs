using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Yi.Abp.HttpApi.Client;

namespace Yi.Abp.Client.Console
{
    [DependsOn(typeof(YiAbpHttpApiClientModule))]
    public class YiAbpClientConsoleModule:AbpModule
    {
    }
}
