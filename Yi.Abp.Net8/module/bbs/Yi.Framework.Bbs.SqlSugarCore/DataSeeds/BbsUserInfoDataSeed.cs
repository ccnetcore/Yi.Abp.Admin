using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class BbsUserInfoDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _repository;
        private ISqlSugarRepository<UserAggregateRoot> _userRepository;
        public BbsUserInfoDataSeed(ISqlSugarRepository<BbsUserExtraInfoEntity> repository, ISqlSugarRepository<UserAggregateRoot> userReponse)
        {
            _repository = repository;
            _userRepository = userReponse;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            //如果没有bbs的用户额外数据，自动插入
            if (!await _repository.IsAnyAsync(x => true))
            {
                var userList = await _userRepository.GetListAsync(x => true);
                var userInfoList = userList.Select(x => new BbsUserExtraInfoEntity(x.Id)).ToList();
                await _repository.InsertManyAsync(userInfoList);
            }
        }
    }
}
