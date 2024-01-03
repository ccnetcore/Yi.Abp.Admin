using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Ddd.Application
{
    [DependsOn(typeof(AbpDddApplicationModule),
        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiFrameworkDddApplicationModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //分页限制
            LimitedResultRequestDto.DefaultMaxResultCount = 10;
            LimitedResultRequestDto.MaxMaxResultCount = 10000;
        }
    }
}
