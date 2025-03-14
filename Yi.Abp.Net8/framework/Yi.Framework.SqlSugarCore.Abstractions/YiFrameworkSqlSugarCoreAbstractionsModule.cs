using Volo.Abp.Modularity;
using Yi.Framework.Core;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    /// <summary>
    /// SqlSugar Core抽象层模块
    /// 提供SqlSugar ORM的基础抽象接口和类型定义
    /// </summary>
    [DependsOn(typeof(YiFrameworkCoreModule))]
    public class YiFrameworkSqlSugarCoreAbstractionsModule : AbpModule
    {
        // 模块配置方法可在此添加
    }
}