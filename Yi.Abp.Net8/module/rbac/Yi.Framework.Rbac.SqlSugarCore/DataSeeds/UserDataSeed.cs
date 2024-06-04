using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Entities.ValueObjects;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.Rbac.Domain.Shared.Options;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class UserDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<UserAggregateRoot> _repository;
        private RbacOptions _options;
        public UserDataSeed(ISqlSugarRepository<UserAggregateRoot> repository, IOptions<RbacOptions> options)
        {
            _repository = repository;
            _options = options.Value;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => true))
            {
                var entities = new List<UserAggregateRoot>();
                UserAggregateRoot user1 = new UserAggregateRoot()
                {
                    Name = "大橙子",
                    UserName = "cc",
                    Nick = "橙子",
                    EncryPassword = new EncryPasswordValueObject(_options.AdminPassword),
                    Email = "454313500@qq.com",
                    Phone = 13800000000,
                    Sex = SexEnum.Male,
                    Address = "深圳",
                    Age = 20,
                    Introduction = "还有谁？",
                    OrderNum = 999,
                    Remark = "描述是什么呢？",
                    State = true
                };
                user1.BuildPassword();
                entities.Add(user1);

                UserAggregateRoot user2 = new UserAggregateRoot()
                {

                    Name = "大测试",
                    UserName = "test",
                    Nick = "测试",
                    EncryPassword=new EncryPasswordValueObject("123456"),
                    Email = "454313500@qq.com",
                    Phone = 15900000000,
                    Sex = SexEnum.Woman,
                    Address = "深圳",
                    Age = 18,
                    Introduction = "还有我！",
                    OrderNum = 1,
                    Remark = "我没有描述！",
                    State = true

                };
                user2.BuildPassword();
                entities.Add(user2);

                UserAggregateRoot user3 = new UserAggregateRoot()
                {

                    Name = "游客",
                    UserName = "guest",
                    Nick = "测试",
                    EncryPassword = new EncryPasswordValueObject("123456"),
                    Email = "454313500@qq.com",
                    Phone = 15900000000,
                    Sex = SexEnum.Woman,
                    Address = "深圳",
                    Age = 18,
                    Introduction = "临时游客",
                    OrderNum = 1,
                    Remark = "懒得创账号",
                    State = true

                };
                user3.BuildPassword();
                entities.Add(user3);


                await _repository.InsertManyAsync(entities);
            }
        }
    }
}
