using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class BbsDictionaryDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<DictionaryEntity> _repository;
        private ISqlSugarRepository<DictionaryTypeAggregateRoot> _typeRepository;
        public BbsDictionaryDataSeed(ISqlSugarRepository<DictionaryEntity> repository, ISqlSugarRepository<DictionaryTypeAggregateRoot> typeRepository) {
            _repository=repository;
            _typeRepository=typeRepository;

        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _typeRepository.IsAnyAsync(x => x.DictType== "bbs_type_lable"))
            {
                await _typeRepository.InsertManyAsync(GetSeedDictionaryTypeData());
                await _repository.InsertManyAsync(GetSeedDictionaryData());
            }
        }
        public List<DictionaryEntity> GetSeedDictionaryData()
        {
            List<DictionaryEntity> entities = new List<DictionaryEntity>();
            DictionaryEntity dictInfo1 = new DictionaryEntity()
            {

                DictLabel = "前端",
                DictValue = "0",
                DictType = "bbs_type_lable",
                OrderNum = 100,
                Remark = "",
                IsDeleted = false,
                State = true

            };
            entities.Add(dictInfo1);

            DictionaryEntity dictInfo2 = new DictionaryEntity()
            {

                DictLabel = "后端",
                DictValue = "1",
                DictType = "bbs_type_lable",
                OrderNum = 99,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo2);

            DictionaryEntity dictInfo3 = new DictionaryEntity()
            {

                DictLabel = "运维",
                DictValue = "2",
                DictType = "bbs_type_lable",
                OrderNum = 98,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo3);
            DictionaryEntity dictInfo4 = new DictionaryEntity()
            {

                DictLabel = "测试",
                DictValue = "3",
                DictType = "bbs_type_lable",
                OrderNum = 97,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo4);

            DictionaryEntity dictInfo5 = new DictionaryEntity()
            {

                DictLabel = "UI",
                DictValue = "4",
                DictType = "bbs_type_lable",
                OrderNum = 96,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo5);


            DictionaryEntity dictInfo6 = new DictionaryEntity()
            {

                DictLabel = "产品",
                DictValue = "5",
                DictType = "bbs_type_lable",
                OrderNum = 95,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo6);

            DictionaryEntity dictInfo7 = new DictionaryEntity()
            {

                DictLabel = "项目",
                DictValue = "6",
                DictType = "bbs_type_lable",
                OrderNum = 94,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo7);

            DictionaryEntity dictInfo8 = new DictionaryEntity()
            {

                DictLabel = "C#",
                DictValue = "7",
                DictType = "bbs_type_lable",
                OrderNum = 93,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo8);

            DictionaryEntity dictInfo9 = new DictionaryEntity()
            {

                DictLabel = ".Net",
                DictValue = "8",
                DictType = "bbs_type_lable",
                OrderNum = 92,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo9);


            DictionaryEntity dictInfo10 = new DictionaryEntity()
            {

                DictLabel = ".NetCore",
                DictValue = "9",
                DictType = "bbs_type_lable",
                OrderNum = 91,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo10);


            DictionaryEntity dictInfo11 = new DictionaryEntity()
            {

                DictLabel = "Asp.NetCore",
                DictValue = "10",
                DictType = "bbs_type_lable",
                OrderNum = 90,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo11);

            DictionaryEntity dictInfo12 = new DictionaryEntity()
            {

                DictLabel = "Abp.vNext",
                DictValue = "11",
                DictType = "bbs_type_lable",
                OrderNum = 89,
                Remark = "",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo12);

            return entities;
        }

        public List<DictionaryTypeAggregateRoot> GetSeedDictionaryTypeData()
        {
            List<DictionaryTypeAggregateRoot> entities = new List<DictionaryTypeAggregateRoot>();
            DictionaryTypeAggregateRoot dict1 = new DictionaryTypeAggregateRoot()
            {

                DictName = "BBS类型标签",
                DictType = "bbs_type_lable",
                OrderNum = 200,
                Remark = "BBS类型标签",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict1);
            return entities;
        }
    }
}
