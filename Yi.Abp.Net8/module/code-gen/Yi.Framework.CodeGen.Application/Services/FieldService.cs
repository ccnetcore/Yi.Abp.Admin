using System;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.CodeGen.Application.Contracts.Dtos.Field;
using Yi.Framework.CodeGen.Application.Contracts.IServices;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.CodeGen.Domain.Shared.Enums;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.Application.Services
{
    /// <summary>
    /// 字段管理
    /// </summary>
    public class FieldService : YiCrudAppService<FieldEntity, FieldDto, Guid, FieldGetListInput>, IFieldService
    {
        private ISqlSugarRepository<FieldEntity, Guid> _repository;
        public FieldService(ISqlSugarRepository<FieldEntity, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        public async override Task<PagedResultDto<FieldDto>> GetListAsync([FromQuery] FieldGetListInput input)
        {
            RefAsync<int> total = 0;
            var entities = await _repository._DbQueryable.WhereIF(input.TableId is not null, x => x.TableId.Equals(input.TableId!))
                      .WhereIF(input.Name is not null, x => x.Name!.Contains(input.Name!))

                      .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

            return new PagedResultDto<FieldDto>
            {
                TotalCount = total,
                Items = await MapToGetListOutputDtosAsync(entities)
            };
        }

        /// <summary>
        /// 获取类型枚举
        /// </summary>
        /// <returns></returns>
        [Route("field/type")]
        public object GetFieldTypeEnum()
        {
            return typeof(FieldTypeEnum).GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => new { lable = x.Name, value = (int)Enum.Parse(typeof(FieldTypeEnum), x.Name) }).ToList();
        }
    }
}
