using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Yi.Framework.Ddd.Application.Contracts
{
    /// <summary>
    /// Yi框架DDD应用层契约模块
    /// </summary>
    [DependsOn(typeof(AbpDddApplicationContractsModule))]
    public class YiFrameworkDddApplicationContractsModule : AbpModule
    {
    }
}
