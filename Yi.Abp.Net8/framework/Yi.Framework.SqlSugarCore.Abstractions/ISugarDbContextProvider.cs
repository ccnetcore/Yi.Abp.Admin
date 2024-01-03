using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    public interface ISugarDbContextProvider<TDbContext>
        where TDbContext : ISqlSugarDbContext
    {

        Task<TDbContext> GetDbContextAsync();

    }

}
