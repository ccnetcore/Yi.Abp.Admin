using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class ConfigDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<ConfigEntity> _repository;
        public ConfigDataSeed(ISqlSugarRepository<ConfigEntity> repository)
        {
            _repository = repository;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => true))
            {
                await _repository.InsertManyAsync(GetSeedData());
            }
        }
        public List<ConfigEntity> GetSeedData()
        {
            List<ConfigEntity> entities = new List<ConfigEntity>();
            ConfigEntity config1 = new ConfigEntity()
            {
                ConfigKey = "bbs.site.name",
                ConfigName = "站点名称",
                ConfigValue = "意社区"
            };
            entities.Add(config1);

            ConfigEntity config2 = new ConfigEntity()
            {
                ConfigKey = "bbs.site.author",
                ConfigName = "站点作者",
                ConfigValue = "橙子"
            };
            entities.Add(config2);

            ConfigEntity config3 = new ConfigEntity()
            {
                ConfigKey = "bbs.site.icp",
                ConfigName = "站点Icp备案",
                ConfigValue = "赣ICP备20008025号"
            };
            entities.Add(config3);


            ConfigEntity config4 = new ConfigEntity()
            {
                ConfigKey = "bbs.site.bottom",
                ConfigName = "站点底部信息",
                ConfigValue = "你好世界"
            };
            entities.Add(config4);
            return entities;
        }
    }


}
