using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.SqlSugarCore
{
    public class TemplateDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<TemplateAggregateRoot> _repository;
        public TemplateDataSeed(ISqlSugarRepository<TemplateAggregateRoot> repository)
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
        public List<TemplateAggregateRoot> GetSeedData()
        {
            var entities = new List<TemplateAggregateRoot>();
            TemplateAggregateRoot entityTemplate = new TemplateAggregateRoot()
            {
                Name = "Entity",
                BuildPath = "D:\\code\\Entities\\@ModelEntity.cs",
                Remarks = "实体",
                TemplateStr = "using SqlSugar;\r\nusing lo.Abp;\r\nusing lo.Abp.Auditing;\r\nusing lo.Abp.Domain.Entities;\r\nusing Yi.Framework.Core.Data;\r\n\r\nnamespace Yi.Framework.Rbac.Domain.Entities\r\n{\r\n    /// <summary>\r\n    /// 实体\r\n    /// </summary>\r\n    [SugarTable(\"@Model\")]\r\n    public class @ModelEntity : Entity<Guid>\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(entityTemplate);


            TemplateAggregateRoot getListInputTemplate = new TemplateAggregateRoot()
            {
                Name = "GetListInput",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelGetListInput.cs",
                Remarks = "列表查询参数",
                TemplateStr = "using Yi.Framework.Ddd;\r\nusing Yi.Framework.Ddd.Application.Contracts;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    /// <summary>\r\n    /// 查询参数\r\n    /// </summary>\r\n    public class @ModelGetListInput : PagedAllResultRequestDto\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(getListInputTemplate);


            TemplateAggregateRoot getListOutputDtoTemplate = new TemplateAggregateRoot()
            {
                Name = "GetListOutputDto",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelGetListOutputDto.cs",
                Remarks = "列表返回dto",
                TemplateStr = "using lo.Abp.Application.Dtos;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    public class @ModelGetListOutputDto : EntityDto<Guid>\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(getListOutputDtoTemplate);


            TemplateAggregateRoot getOutputDtoTemplate = new TemplateAggregateRoot()
            {
                Name = "GetOutputDto",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelGetOutputDto.cs",
                Remarks = "单返回dto",
                TemplateStr = "using lo.Abp.Application.Dtos;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    public class @ModelGetOutputDto : EntityDto<Guid>\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(getOutputDtoTemplate);

            TemplateAggregateRoot updateInputTemplate = new TemplateAggregateRoot()
            {
                Name = "UpdateInput",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelUpdateInput.cs",
                Remarks = "更新输入",
                TemplateStr = "namespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    public class @ModelUpdateInput\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(updateInputTemplate);

            TemplateAggregateRoot createInputTemplate = new TemplateAggregateRoot()
            {
                Name = "CreateInput",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelCreateInput.cs",
                Remarks = "创建dto",
                TemplateStr = "namespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    /// <summary>\r\n    /// @Model输入创建对象\r\n    /// </summary>\r\n    public class @ModelCreateInput\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(createInputTemplate);


            TemplateAggregateRoot iServicesTemplate = new TemplateAggregateRoot()
            {
                Name = "IServices",
                BuildPath = "D:\\code\\IServices\\I@ModelService.cs",
                Remarks = "应用服务抽象",
                TemplateStr = "using lo.Abp.Application.Services;\r\nusing Yi.Framework.Ddd.Application.Contracts;\r\nusing Yi.Framework.Rbac.Application.Contracts.Dtos.@Model;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.IServices\r\n{\r\n    /// <summary>\r\n    /// @Model服务抽象\r\n    /// </summary>\r\n    public interface I@ModelService : IYiCrudAppService<@ModelGetOutputDto, @ModelGetListOutputDto, Guid, @ModelGetListInput, @ModelCreateInput, @ModelUpdateInput>\r\n    {\r\n\r\n    }\r\n}\r\n"
            };
            entities.Add(iServicesTemplate);



            TemplateAggregateRoot servicesTemplate = new TemplateAggregateRoot()
            {
                Name = "Service",
                BuildPath = "D:\\code\\Services\\@ModelService.cs",
                Remarks = "应用服务",
                TemplateStr = "using SqlSugar;\r\nusing lo.Abp.Application.Dtos;\r\nusing lo.Abp.Application.Services;\r\nusing Yi.Framework.Ddd.Application;\r\nusing Yi.Framework.Rbac.Application.Contracts.Dtos.@Model;\r\nusing Yi.Framework.Rbac.Application.Contracts.IServices;\r\nusing Yi.Framework.Rbac.Domain.Entities;\r\nusing Yi.Framework.SqlSugarCore.Abstractions;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Services\r\n{\r\n    /// <summary>\r\n    /// @Model服务实现\r\n    /// </summary>\r\n    public class @ModelService : YiCrudAppService<@ModelEntity, @ModelGetOutputDto, @ModelGetListOutputDto, Guid, @ModelGetListInput, @ModelCreateInput, @ModelUpdateInput>,\r\n       I@ModelService\r\n    {\r\n        private ISqlSugarRepository<@ModelEntity, Guid> _repository;\r\n        public @ModelService(ISqlSugarRepository<@ModelEntity, Guid> repository) : base(repository)\r\n        {\r\n            _repository = repository;\r\n        }\r\n\r\n        /// <summary>\r\n        /// 多查\r\n        /// </summary>\r\n        /// <param name=\"input\"></param>\r\n        /// <returns></returns>\r\n        public override async Task<PagedResultDto<@ModelGetListOutputDto>> GetListAsync(@ModelGetListInput input)\r\n        {\r\n            RefAsync<int> total = 0;\r\n\r\n            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.@ModelKey), x => x.@ModelKey.Contains(input.@ModelKey!))\r\n                          .WhereIF(!string.IsNullOrEmpty(input.@ModelName), x => x.@ModelName!.Contains(input.@ModelName!))\r\n                          .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)\r\n                          .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);\r\n            return new PagedResultDto<@ModelGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));\r\n        }\r\n    }\r\n}\r\n"
            };
            entities.Add(servicesTemplate);

            TemplateAggregateRoot apiTemplate = new TemplateAggregateRoot()
            {
                TemplateStr = "import request from '@/utils/request'\r\n\r\n// 分页查询\r\nexport function listData(query) {\r\n  return request({\r\n    url: '/@model',\r\n    method: 'get',\r\n    params: query\r\n  })\r\n}\r\n\r\n// id查询\r\nexport function getData(id) {\r\n  return request({\r\n    url: `/@model/${id}`,\r\n    method: 'get'\r\n  })\r\n}\r\n\r\n// 新增\r\nexport function addData(data) {\r\n  return request({\r\n    url: '/@model',\r\n    method: 'post',\r\n    data: data\r\n  })\r\n}\r\n\r\n// 修改\r\nexport function updateData(id,data) {\r\n  return request({\r\n    url: `/@model/${id}`,\r\n    method: 'put',\r\n    data: data\r\n  })\r\n}\r\n\r\n// 删除\r\nexport function delData(ids) {\r\n  return request({\r\n    url: `/@model`,\r\n    method: 'delete',\r\n    params:{id:ids}\r\n  })\r\n}\r\n",
                Name = "api",
                BuildPath = "D:\\code\\Api\\@ModelApi.vue",
                Remarks = "前端api"
            };
            entities.Add(apiTemplate);

            return entities;
        }
    }
}
