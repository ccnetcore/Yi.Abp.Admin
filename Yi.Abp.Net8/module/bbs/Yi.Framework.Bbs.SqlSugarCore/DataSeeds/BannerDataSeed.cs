using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class BannerDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<BannerAggregateRoot> _repository;
        public BannerDataSeed(ISqlSugarRepository<BannerAggregateRoot> repository)
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
        public List<BannerAggregateRoot> GetSeedData()
        {
            //这里临时使用了图床，图床地址：https://mjj.today/
            List<BannerAggregateRoot> entities = new List<BannerAggregateRoot>()
            {
                new BannerAggregateRoot{
                Name="欢迎",
                Logo="https://i.miji.bid/2023/12/15/e6478d5d15a4b941077e336790c414f6.png",
                Color=""

                },
                new BannerAggregateRoot{
                Name="前端",
                Logo="https://i.miji.bid/2023/12/15/07e9291c9311889a31a2b433d4decca0.jpeg",
                Color=""

                },
            };



            return entities;
        }
    }

}