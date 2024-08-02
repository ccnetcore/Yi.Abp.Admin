using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class DictionaryDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<DictionaryEntity> _repository;
        public DictionaryDataSeed(ISqlSugarRepository<DictionaryEntity> repository)
        {
            _repository= repository;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => true))
            {
                await _repository.InsertManyAsync(GetSeedData());
            }
        }
        public  List<DictionaryEntity> GetSeedData()
        {
            List<DictionaryEntity> entities = new List<DictionaryEntity>();
            DictionaryEntity dictInfo1 = new DictionaryEntity()
            {

                DictLabel = "男",
                DictValue = "Male",
                DictType = "sys_user_sex",
                OrderNum = 100,
                Remark = "性别男",
                IsDeleted = false,
                State = true

            };
            entities.Add(dictInfo1);

            DictionaryEntity dictInfo2 = new DictionaryEntity()
            {

                DictLabel = "女",
                DictValue = "Woman",
                DictType = "sys_user_sex",
                OrderNum = 99,
                Remark = "性别女",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo2);

            DictionaryEntity dictInfo3 = new DictionaryEntity()
            {

                DictLabel = "未知",
                DictValue = "Unknown",
                DictType = "sys_user_sex",
                OrderNum = 98,
                Remark = "性别未知",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo3);



            DictionaryEntity dictInfo4 = new DictionaryEntity()
            {

                DictLabel = "显示",
                DictValue = "true",
                DictType = "sys_show_hide",
                OrderNum = 100,
                Remark = "显示菜单",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo4);

            DictionaryEntity dictInfo5 = new DictionaryEntity()
            {

                DictLabel = "隐藏",
                DictValue = "false",
                DictType = "sys_show_hide",
                OrderNum = 99,
                Remark = "隐藏菜单",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo5);



            DictionaryEntity dictInfo6 = new DictionaryEntity()
            {

                DictLabel = "正常",
                DictValue = "true",
                DictType = "sys_normal_disable",
                OrderNum = 100,
                Remark = "正常状态",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo6);
            DictionaryEntity dictInfo7 = new DictionaryEntity()
            {

                DictLabel = "停用",
                DictValue = "false",
                DictType = "sys_normal_disable",
                OrderNum = 99,
                Remark = "停用状态",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo7);



            DictionaryEntity dictInfo8 = new DictionaryEntity()
            {

                DictLabel = "正常",
                DictValue = "0",
                DictType = "sys_job_status",
                OrderNum = 100,
                Remark = "正常状态",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo8);
            DictionaryEntity dictInfo9 = new DictionaryEntity()
            {

                DictLabel = "暂停",
                DictValue = "1",
                DictType = "sys_job_status",
                OrderNum = 99,
                Remark = "停用状态",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo9);




            DictionaryEntity dictInfo10 = new DictionaryEntity()
            {

                DictLabel = "默认",
                DictValue = "DEFAULT",
                DictType = "sys_job_group",
                OrderNum = 100,
                Remark = "默认分组",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo10);
            DictionaryEntity dictInfo11 = new DictionaryEntity()
            {

                DictLabel = "系统",
                DictValue = "SYSTEM",
                DictType = "sys_job_group",
                OrderNum = 99,
                Remark = "系统分组",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo11);



            DictionaryEntity dictInfo12 = new DictionaryEntity()
            {

                DictLabel = "是",
                DictValue = "Y",
                DictType = "sys_yes_no",
                OrderNum = 100,
                Remark = "系统默认是",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo12);
            DictionaryEntity dictInfo13 = new DictionaryEntity()
            {

                DictLabel = "否",
                DictValue = "N",
                DictType = "sys_yes_no",
                OrderNum = 99,
                Remark = "系统默认否",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo13);



            DictionaryEntity dictInfo14 = new DictionaryEntity()
            {

                DictLabel = "通知",
                DictValue = "1",
                DictType = "sys_notice_type",
                OrderNum = 100,
                Remark = "通知",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo14);
            DictionaryEntity dictInfo15 = new DictionaryEntity()
            {

                DictLabel = "公告",
                DictValue = "2",
                DictType = "sys_notice_type",
                OrderNum = 99,
                Remark = "公告",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo15);

            DictionaryEntity dictInfo16 = new DictionaryEntity()
            {

                DictLabel = "正常",
                DictValue = "0",
                DictType = "sys_notice_status",
                OrderNum = 100,
                Remark = "正常状态",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo16);
            DictionaryEntity dictInfo17 = new DictionaryEntity()
            {

                DictLabel = "关闭",
                DictValue = "1",
                DictType = "sys_notice_status",
                OrderNum = 99,
                Remark = "关闭状态",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo17);

            DictionaryEntity dictInfo18 = new DictionaryEntity()
            {

                DictLabel = "新增",
                DictValue = "Insert",
                DictType = "sys_oper_type",
                OrderNum = 100,
                Remark = "新增操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo18);
            DictionaryEntity dictInfo19 = new DictionaryEntity()
            {

                DictLabel = "修改",
                DictValue = "Update",
                DictType = "sys_oper_type",
                OrderNum = 99,
                Remark = "修改操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo19);
            DictionaryEntity dictInfo22 = new DictionaryEntity()
            {

                DictLabel = "删除",
                DictValue = "Delete",
                DictType = "sys_oper_type",
                OrderNum = 98,
                Remark = "删除操作",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo22);
            DictionaryEntity dictInfo23 = new DictionaryEntity()
            {

                DictLabel = "授权",
                DictValue = "Auth",
                DictType = "sys_oper_type",
                OrderNum = 97,
                Remark = "授权操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo23);
            DictionaryEntity dictInfo24 = new DictionaryEntity()
            {

                DictLabel = "导出",
                DictValue = "Export",
                DictType = "sys_oper_type",
                OrderNum = 96,
                Remark = "导出操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo24);
            DictionaryEntity dictInfo25 = new DictionaryEntity()
            {

                DictLabel = "导入",
                DictValue = "Import",
                DictType = "sys_oper_type",
                OrderNum = 95,
                Remark = "导入操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo25);
            DictionaryEntity dictInfo26 = new DictionaryEntity()
            {

                DictLabel = "强退",
                DictValue = "ForcedOut",
                DictType = "sys_oper_type",
                OrderNum = 94,
                Remark = "强退操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo26);
            DictionaryEntity dictInfo27 = new DictionaryEntity()
            {

                DictLabel = "生成代码",
                DictValue = "GenerateCode",
                DictType = "sys_oper_type",
                OrderNum = 93,
                Remark = "生成代码操作",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo27);
            DictionaryEntity dictInfo28 = new DictionaryEntity()
            {

                DictLabel = "清空数据",
                DictValue = "ClearData",
                DictType = "sys_oper_type",
                OrderNum = 92,
                Remark = "清空数据操作",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo28);



            DictionaryEntity dictInfo20 = new DictionaryEntity()
            {

                DictLabel = "成功",
                DictValue = "false",
                DictType = "sys_common_status",
                OrderNum = 100,
                Remark = "正常状态",
                IsDeleted = false,
                State = true
            };
            entities.Add(dictInfo20);
            DictionaryEntity dictInfo21 = new DictionaryEntity()
            {

                DictLabel = "失败",
                DictValue = "true",
                DictType = "sys_common_status",
                OrderNum = 99,
                Remark = "失败状态",
                IsDeleted = false,
                State = true,
                ListClass = "danger"
            };
            entities.Add(dictInfo21);

            return entities;
        }
    }
}
