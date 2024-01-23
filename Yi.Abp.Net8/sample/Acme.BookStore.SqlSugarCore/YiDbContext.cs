using Microsoft.Extensions.Logging;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.SqlSugarCore;

namespace Acme.BookStore.SqlSugarCore
{
    public class YiDbContext : YiRbacDbContext
    {
        public YiDbContext(IAbpLazyServiceProvider lazyServiceProvider) : base(lazyServiceProvider)
        {
        }
    }
}
