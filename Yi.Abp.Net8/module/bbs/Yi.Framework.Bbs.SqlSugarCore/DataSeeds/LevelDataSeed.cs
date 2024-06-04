using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Bbs.Domain.Entities.Integral;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class LevelDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<LevelAggregateRoot> _repository;
        public LevelDataSeed(ISqlSugarRepository<LevelAggregateRoot> repository)
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
        public List<LevelAggregateRoot> GetSeedData()
        {
            List<LevelAggregateRoot> entities = new List<LevelAggregateRoot>()
            {
                new LevelAggregateRoot(1,"小白",10),
                new LevelAggregateRoot(2,"中白",30),
                new LevelAggregateRoot(3,"大白",100),
                new LevelAggregateRoot(4,"精英",300),
                new LevelAggregateRoot(5,"熟练",600),
                new LevelAggregateRoot(6,"高手",1000),
                new LevelAggregateRoot(7,"老手",1500),
                new LevelAggregateRoot(8,"大佬",2000),
                new LevelAggregateRoot(9,"巨佬",2500),
                new LevelAggregateRoot(10,"大神",3000),
            };

            return entities;
        }
    }
}
