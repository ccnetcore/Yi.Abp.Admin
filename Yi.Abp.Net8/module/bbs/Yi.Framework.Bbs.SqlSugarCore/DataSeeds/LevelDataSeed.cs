using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Bbs.Domain.Entities.Integral;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class LevelDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<LevelEntity> _repository;
        public LevelDataSeed(ISqlSugarRepository<LevelEntity> repository)
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
        public List<LevelEntity> GetSeedData()
        {
            List<LevelEntity> entities = new List<LevelEntity>()
            {
                new LevelEntity(1,"小白",10),
                new LevelEntity(2,"中白",30),
                new LevelEntity(3,"大白",100),
                new LevelEntity(4,"精英",300),
                new LevelEntity(5,"熟练",600),
                new LevelEntity(6,"高手",1000),
                new LevelEntity(7,"老手",1500),
                new LevelEntity(8,"大佬",2000),
                new LevelEntity(9,"巨佬",2500),
                new LevelEntity(10,"大神",3000),
            };

            return entities;
        }
    }
}
