using SqlSugar;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.SqlSugarCore;

namespace Yi.Abp.SqlSugarCore
{
    public class YiDbContext : YiRbacDbContext
    {
        public YiDbContext(IAbpLazyServiceProvider lazyServiceProvider) : base(lazyServiceProvider)
        {
        }
    }
}
