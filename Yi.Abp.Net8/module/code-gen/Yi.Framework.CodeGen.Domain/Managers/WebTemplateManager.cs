using System.ComponentModel.DataAnnotations;
using System.Reflection;
using SqlSugar;
using Volo.Abp.Domain.Services;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.CodeGen.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.Domain.Managers
{
    /// <summary>
    /// 与webfrist相关，同步到web，code to web
    /// </summary>
    public class WebTemplateManager : DomainService
    {
        private ISqlSugarRepository<TableAggregateRoot> _repository;
        private IModuleContainer _moduleContainer;
        public WebTemplateManager(ISqlSugarRepository<TableAggregateRoot> repository, IModuleContainer moduleContainer)
        {
            _repository = repository;
            _moduleContainer = moduleContainer;
        }
        /// <summary>
        /// 通过当前的实体代码获取表存储
        /// </summary>
        /// <returns></returns>

        public Task<List<TableAggregateRoot>> BuildCodeToWebAsync()
        {
            List<Type> entityTypes = new List<Type>();
            foreach (var module in _moduleContainer.Modules)
            {
                entityTypes.AddRange(module.Assembly.GetTypes()
                    .Where(x => x.GetCustomAttribute<IgnoreCodeFirstAttribute>() == null)
                    .Where(x => x.GetCustomAttribute<SugarTable>() != null)
                    .Where(x => x.GetCustomAttribute<SplitTableAttribute>() is null));
            }

            var tableAggregateRoots = new List<TableAggregateRoot>();
            foreach (var entityType in entityTypes)
            {
                tableAggregateRoots.Add(EntityTypeMapperToTable(entityType));
            }

            return Task.FromResult(tableAggregateRoots);
        }

        private TableAggregateRoot EntityTypeMapperToTable(Type entityType)
        {
            var tableAggregateRoot = new TableAggregateRoot();
            tableAggregateRoot.Fields = new List<FieldEntity>();
            var table = entityType.GetCustomAttribute<SugarTable>();

            tableAggregateRoot.Name = table.TableName;

            foreach (var p in entityType.GetProperties())
            {
                tableAggregateRoot.Fields.Add(PropertyMapperToFiled(p));
            }
            tableAggregateRoot.Fields.ForEach(x => x.TableId = tableAggregateRoot.Id);
            return tableAggregateRoot;
        }


        private FieldEntity PropertyMapperToFiled(PropertyInfo propertyInfo)
        {
            var fieldEntity = new FieldEntity();
            fieldEntity.Name = propertyInfo.Name;


            //获取数据类型，包括可空类型
            Type? fieldType = null;
            // 如果字段类型是 Nullable<T> 泛型类型
            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType)!;
                fieldType = nullableType;
            }
            else
            {
                fieldType = propertyInfo.PropertyType;
            }

            //判断类型
            var enumName = typeof(FieldTypeEnum).GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.GetCustomAttribute<DisplayAttribute>()?.Description == fieldType.Name).FirstOrDefault()?.Name;
            if (enumName is null)
            {
                fieldEntity.FieldType = FieldTypeEnum.String;
                // App.GetRequiredService<ILogger<WebTemplateManager>>().LogError($"字段类型：{propertyInfo.PropertyType.Name}，未定义");
            }
            else
            {
                fieldEntity.FieldType = (FieldTypeEnum)Enum.Parse(typeof(FieldTypeEnum), enumName);
            }

            //判断是否可空
            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                fieldEntity.IsRequired = false;
            }
            else
            {
                fieldEntity.IsRequired = true;
            }



            //判断是否主键
            if (propertyInfo.GetCustomAttribute<SugarColumn>()?.IsPrimaryKey == true)
            {
                fieldEntity.IsKey = true;
            }

            //判断长度
            var colum = propertyInfo.GetCustomAttribute<SugarColumn>();
            if (colum is not null && colum.Length != 0)
            {
                fieldEntity.Length = colum.Length;
            }
            return fieldEntity;
        }
    }
}
