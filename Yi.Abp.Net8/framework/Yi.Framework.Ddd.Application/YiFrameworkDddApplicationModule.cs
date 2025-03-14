using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Ddd.Application
{
    /// <summary>
    /// Yi框架DDD应用层模块
    /// </summary>
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(YiFrameworkDddApplicationContractsModule)
    )]
    public class YiFrameworkDddApplicationModule : AbpModule
    {
        /// <summary>
        /// 应用程序初始化配置
        /// </summary>
        /// <param name="context">应用程序初始化上下文</param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            // 配置分页查询的默认值和最大值限制
            ConfigureDefaultPagingSettings();
        }

        /// <summary>
        /// 配置默认分页设置
        /// </summary>
        private void ConfigureDefaultPagingSettings()
        {
            // 设置默认每页显示记录数
            LimitedResultRequestDto.DefaultMaxResultCount = 10;
            
            // 设置最大允许的每页记录数
            LimitedResultRequestDto.MaxMaxResultCount = 10000;
        }
    }
}
