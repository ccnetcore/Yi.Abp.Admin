using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{

    public class DeptDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<DeptAggregateRoot> _repository;
        private IGuidGenerator _guidGenerator;
        public DeptDataSeed(ISqlSugarRepository<DeptAggregateRoot> repository, IGuidGenerator guidGenerator)
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
        public List<DeptAggregateRoot> GetSeedData()
        {
            var entities = new List<DeptAggregateRoot>();

            DeptAggregateRoot chengziDept = new DeptAggregateRoot(_guidGenerator.Create())
            {
                DeptName = "橙子科技",
                DeptCode = "Yi",
                OrderNum = 100,
                IsDeleted = false,
                Leader = "橙子",
                Remark = "如名所指"
            };
            entities.Add(chengziDept);


            DeptAggregateRoot shenzhenDept = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "深圳总公司",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = chengziDept.Id
            };
            entities.Add(shenzhenDept);


            DeptAggregateRoot jiangxiDept = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "江西总公司",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = chengziDept.Id
            };
            entities.Add(jiangxiDept);



            DeptAggregateRoot szDept1 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "研发部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept1);

            DeptAggregateRoot szDept2 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "市场部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept2);

            DeptAggregateRoot szDept3 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "测试部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept3);

            DeptAggregateRoot szDept4 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "财务部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept4);

            DeptAggregateRoot szDept5 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "运维部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept5);


            DeptAggregateRoot jxDept1 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "市场部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = jiangxiDept.Id
            };
            entities.Add(jxDept1);


            DeptAggregateRoot jxDept2 = new DeptAggregateRoot(_guidGenerator.Create())
            {

                DeptName = "财务部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = jiangxiDept.Id
            };
            entities.Add(jxDept2);


            return entities;
        }
    }
}
