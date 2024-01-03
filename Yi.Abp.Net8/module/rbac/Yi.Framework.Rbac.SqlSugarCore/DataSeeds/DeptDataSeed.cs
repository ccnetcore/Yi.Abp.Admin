using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{

    public class DeptDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<DeptEntity> _repository;
        private IGuidGenerator _guidGenerator;
        public DeptDataSeed(ISqlSugarRepository<DeptEntity> repository, IGuidGenerator guidGenerator)
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
        public List<DeptEntity> GetSeedData()
        {
            var entities = new List<DeptEntity>();

            DeptEntity chengziDept = new DeptEntity(_guidGenerator.Create())
            {
                DeptName = "橙子科技",
                DeptCode = "Yi",
                OrderNum = 100,
                IsDeleted = false,
                Leader = "橙子",
                Remark = "如名所指"
            };
            entities.Add(chengziDept);


            DeptEntity shenzhenDept = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "深圳总公司",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = chengziDept.Id
            };
            entities.Add(shenzhenDept);


            DeptEntity jiangxiDept = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "江西总公司",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = chengziDept.Id
            };
            entities.Add(jiangxiDept);



            DeptEntity szDept1 = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "研发部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept1);

            DeptEntity szDept2 = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "市场部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept2);

            DeptEntity szDept3 = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "测试部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept3);

            DeptEntity szDept4 = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "财务部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept4);

            DeptEntity szDept5 = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "运维部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = shenzhenDept.Id
            };
            entities.Add(szDept5);


            DeptEntity jxDept1 = new DeptEntity(_guidGenerator.Create())
            {

                DeptName = "市场部门",
                OrderNum = 100,
                IsDeleted = false,
                ParentId = jiangxiDept.Id
            };
            entities.Add(jxDept1);


            DeptEntity jxDept2 = new DeptEntity(_guidGenerator.Create())
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
