using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class PostDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<PostEntity> _repository;
        public PostDataSeed(ISqlSugarRepository<PostEntity> repository)
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
        public List<PostEntity> GetSeedData()
        {
            var entites = new List<PostEntity>();

            PostEntity Post1 = new PostEntity()
            {

                PostName = "董事长",
                PostCode = "ceo",
                OrderNum = 100,
                IsDeleted = false
            };
            entites.Add(Post1);

            PostEntity Post2 = new PostEntity()
            {

                PostName = "项目经理",
                PostCode = "se",
                OrderNum = 100,
                IsDeleted = false
            };
            entites.Add(Post2);

            PostEntity Post3 = new PostEntity()
            {

                PostName = "人力资源",
                PostCode = "hr",
                OrderNum = 100,
                IsDeleted = false
            };
            entites.Add(Post3);

            PostEntity Post4 = new PostEntity()
            {

                PostName = "普通员工",
                PostCode = "user",
                OrderNum = 100,
                IsDeleted = false
            };

            entites.Add(Post4);
            return entites;
        }
    }


}
