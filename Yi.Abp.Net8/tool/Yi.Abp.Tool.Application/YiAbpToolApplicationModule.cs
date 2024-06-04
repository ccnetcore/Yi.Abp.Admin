using Yi.Abp.Tool.Application.Contracts;
using Yi.Abp.Tool.Domain;

namespace Yi.Abp.Tool.Application
{
    [DependsOn(typeof(YiAbpToolApplicationContractsModule),
        typeof(YiAbpToolDomainModule))]
    public class YiAbpToolApplicationModule:AbpModule
    {

    }
}
