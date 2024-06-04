using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class RoleDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<RoleAggregateRoot> _repository;
        public RoleDataSeed(ISqlSugarRepository<RoleAggregateRoot> repository)
        {
            _repository = repository;
        }

        public List<RoleAggregateRoot> GetSeedData()
        {
            var entities = new List<RoleAggregateRoot>();
            RoleAggregateRoot role1 = new RoleAggregateRoot()
            {

                RoleName = "管理员",
                RoleCode = "admin",
                DataScope = DataScopeEnum.ALL,
                OrderNum = 999,
                Remark = "管理员",
                IsDeleted = false
            };
            entities.Add(role1);

            RoleAggregateRoot role2 = new RoleAggregateRoot()
            {

                RoleName = "测试角色",
                RoleCode = "test",
                DataScope = DataScopeEnum.ALL,
                OrderNum = 1,
                Remark = "测试用的角色",
                IsDeleted = false
            };
            entities.Add(role2);

            RoleAggregateRoot role3 = new RoleAggregateRoot()
            {

                RoleName = "普通角色",
                RoleCode = "common",
                DataScope = DataScopeEnum.ALL,
                OrderNum = 1,
                Remark = "正常用户",
                IsDeleted = false
            };
            entities.Add(role3);

            RoleAggregateRoot role4 = new RoleAggregateRoot()
            {

                RoleName = "默认角色",
                RoleCode = "default",
                DataScope = DataScopeEnum.ALL,
                OrderNum = 1,
                Remark = "可简单浏览",
                IsDeleted = false
            };
            entities.Add(role4);


            return entities;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => true))
            {
                await _repository.InsertManyAsync(GetSeedData());
            }
        }
    }
}
