using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    internal class ConfigDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<ConfigAggregateRoot> _repository;
        private IGuidGenerator _guidGenerator;

        public ConfigDataSeed(ISqlSugarRepository<ConfigAggregateRoot> repository, IGuidGenerator guidGenerator)
        {
            _repository = repository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => true))
            {
                await _repository.InsertManyAsync(GetSeedData());
            }
        }
        public List<ConfigAggregateRoot> GetSeedData()
        {
            var entities = new List<ConfigAggregateRoot>();

            ConfigAggregateRoot initPassword = new ConfigAggregateRoot()
            {
                ConfigName = "初始密码",
                ConfigKey = "sys.user.initPassword",
                ConfigValue = "123456",
                OrderNum = 1,
                IsDeleted = false,
                Remark = "初始密码"
            };
            entities.Add(initPassword);

            return entities;
        }
    }
}
