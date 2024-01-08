using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class DictionaryTypeDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<DictionaryTypeAggregateRoot> _repository;
        public DictionaryTypeDataSeed(ISqlSugarRepository<DictionaryTypeAggregateRoot> repository)
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
        public List<DictionaryTypeAggregateRoot> GetSeedData()
        {
            List<DictionaryTypeAggregateRoot> entities = new List<DictionaryTypeAggregateRoot>();
            DictionaryTypeAggregateRoot dict1 = new DictionaryTypeAggregateRoot()
            {

                DictName = "用户性别",
                DictType = "sys_user_sex",
                OrderNum = 100,
                Remark = "用户性别列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict1);

            DictionaryTypeAggregateRoot dict2 = new DictionaryTypeAggregateRoot()
            {

                DictName = "菜单状态",
                DictType = "sys_show_hide",
                OrderNum = 100,
                Remark = "菜单状态列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict2);

            DictionaryTypeAggregateRoot dict3 = new DictionaryTypeAggregateRoot()
            {

                DictName = "系统开关",
                DictType = "sys_normal_disable",
                OrderNum = 100,
                Remark = "系统开关列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict3);

            DictionaryTypeAggregateRoot dict4 = new DictionaryTypeAggregateRoot()
            {

                DictName = "任务状态",
                DictType = "sys_job_status",
                OrderNum = 100,
                Remark = "任务状态列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict4);

            DictionaryTypeAggregateRoot dict5 = new DictionaryTypeAggregateRoot()
            {

                DictName = "任务分组",
                DictType = "sys_job_group",
                OrderNum = 100,
                Remark = "任务分组列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict5);

            DictionaryTypeAggregateRoot dict6 = new DictionaryTypeAggregateRoot()
            {

                DictName = "系统是否",
                DictType = "sys_yes_no",
                OrderNum = 100,
                Remark = "系统是否列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict6);

            DictionaryTypeAggregateRoot dict7 = new DictionaryTypeAggregateRoot()
            {

                DictName = "通知类型",
                DictType = "sys_notice_type",
                OrderNum = 100,
                Remark = "通知类型列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict7);
            DictionaryTypeAggregateRoot dict8 = new DictionaryTypeAggregateRoot()
            {

                DictName = "通知状态",
                DictType = "sys_notice_status",
                OrderNum = 100,
                Remark = "通知状态列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict8);

            DictionaryTypeAggregateRoot dict9 = new DictionaryTypeAggregateRoot()
            {

                DictName = "操作类型",
                DictType = "sys_oper_type",
                OrderNum = 100,
                Remark = "操作类型列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict9);


            DictionaryTypeAggregateRoot dict10 = new DictionaryTypeAggregateRoot()
            {

                DictName = "系统状态",
                DictType = "sys_common_status",
                OrderNum = 100,
                Remark = "登录状态列表",
                IsDeleted = false,
                State = true
            };
            entities.Add(dict10);
            return entities;
        }
    }
}
