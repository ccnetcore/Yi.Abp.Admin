using Microsoft.Extensions.Logging;
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

        protected override void CustomDataFilter()
        {
            base.CustomDataFilter();
        }


        protected override void DataExecuted(object oldValue, DataAfterModel entityInfo)
        {
            base.DataExecuted(oldValue, entityInfo);
        }

        protected override void DataExecuting(object oldValue, DataFilterModel entityInfo)
        {
            base.DataExecuting(oldValue, entityInfo);
        }

        protected override void OnLogExecuting(string sql, SugarParameter[] pars)
        {
            base.OnLogExecuting(sql, pars);
        }

        protected override void OnLogExecuted(string sql, SugarParameter[] pars)
        {
            base.OnLogExecuted(sql, pars);
        }

        protected override void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient)
        {
            base.OnSqlSugarClientConfig(sqlSugarClient);
        }
    }
}
