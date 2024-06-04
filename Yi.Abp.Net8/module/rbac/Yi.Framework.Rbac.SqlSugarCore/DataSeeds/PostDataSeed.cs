using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class PostDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<PostAggregateRoot> _repository;
        public PostDataSeed(ISqlSugarRepository<PostAggregateRoot> repository)
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
        public List<PostAggregateRoot> GetSeedData()
        {
            var entites = new List<PostAggregateRoot>();

            PostAggregateRoot Post1 = new PostAggregateRoot()
            {

                PostName = "董事长",
                PostCode = "ceo",
                OrderNum = 100,
                IsDeleted = false
            };
            entites.Add(Post1);

            PostAggregateRoot Post2 = new PostAggregateRoot()
            {

                PostName = "项目经理",
                PostCode = "se",
                OrderNum = 100,
                IsDeleted = false
            };
            entites.Add(Post2);

            PostAggregateRoot Post3 = new PostAggregateRoot()
            {

                PostName = "人力资源",
                PostCode = "hr",
                OrderNum = 100,
                IsDeleted = false
            };
            entites.Add(Post3);

            PostAggregateRoot Post4 = new PostAggregateRoot()
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
