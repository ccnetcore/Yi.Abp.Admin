using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.SqlSugarCore
{
    public class TemplateDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<TemplateEntity> _repository;
        public TemplateDataSeed(ISqlSugarRepository<TemplateEntity> repository)
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
        public List<TemplateEntity> GetSeedData()
        {
            var entities = new List<TemplateEntity>();
            TemplateEntity entityTemplate = new TemplateEntity()
            {
                Name= "Entity",
                BuildPath= "D:\\code\\Entities\\@ModelEntity.cs",
                Remarks= "实体",
                TemplateStr= "using SqlSugar;\r\nusing Volo.Abp;\r\nusing Volo.Abp.Auditing;\r\nusing Volo.Abp.Domain.Entities;\r\nusing Yi.Framework.Core.Data;\r\n\r\nnamespace Yi.Framework.Rbac.Domain.Entities\r\n{\r\n    /// <summary>\r\n    /// 实体\r\n    /// </summary>\r\n    [SugarTable(\"@Model\")]\r\n    public class @ModelEntity : Entity<Guid>\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(entityTemplate);


            TemplateEntity getListInputTemplate = new TemplateEntity()
            {
                Name = "GetListInput",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelGetListInput.cs",
                Remarks = "列表查询参数",
                TemplateStr = "using Yi.Framework.Ddd;\r\nusing Yi.Framework.Ddd.Application.Contracts;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    /// <summary>\r\n    /// 查询参数\r\n    /// </summary>\r\n    public class @ModelGetListInputVo : PagedAllResultRequestDto\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(getListInputTemplate);


            TemplateEntity getListOutputDtoTemplate = new TemplateEntity()
            {
                Name = "GetListOutputDto",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelGetListOutputDto.cs",
                Remarks = "列表返回dto",
                TemplateStr = "using Volo.Abp.Application.Dtos;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    public class @ModelGetListOutputDto : EntityDto<Guid>\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(getListOutputDtoTemplate);


            TemplateEntity getOutputDtoTemplate = new TemplateEntity()
            {
                Name = "GetOutputDto",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelGetOutputDto.cs",
                Remarks = "单返回dto",
                TemplateStr = "using Volo.Abp.Application.Dtos;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    public class @ModelGetOutputDto : EntityDto<Guid>\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(getOutputDtoTemplate);

            TemplateEntity updateInputTemplate = new TemplateEntity()
            {
                Name = "UpdateInput",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelUpdateInput.cs",
                Remarks = "更新输入",
                TemplateStr = "namespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    public class @ModelUpdateInput\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(updateInputTemplate);

            TemplateEntity createInputTemplate = new TemplateEntity()
            {
                Name = "CreateInput",
                BuildPath = "D:\\code\\Dtos\\@Model\\@ModelCreateInput.cs",
                Remarks = "创建dto",
                TemplateStr = "namespace Yi.Framework.Rbac.Application.Contracts.Dtos.@Model\r\n{\r\n    /// <summary>\r\n    /// @Model输入创建对象\r\n    /// </summary>\r\n    public class @ModelCreateInput\r\n    {\r\n@field\r\n    }\r\n}\r\n"
            };
            entities.Add(createInputTemplate);


            TemplateEntity iServicesTemplate = new TemplateEntity()
            {
                Name = "IServices",
                BuildPath = "D:\\code\\IServices\\I@ModelService.cs",
                Remarks = "应用服务抽象",
                TemplateStr = "using Volo.Abp.Application.Services;\r\nusing Yi.Framework.Ddd.Application.Contracts;\r\nusing Yi.Framework.Rbac.Application.Contracts.Dtos.@Model;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Contracts.IServices\r\n{\r\n    /// <summary>\r\n    /// @Model服务抽象\r\n    /// </summary>\r\n    public interface I@ModelService : IYiCrudAppService<@ModelGetOutputDto, @ModelGetListOutputDto, Guid, @ModelGetListInputVo, @ModelCreateInputVo, @ModelUpdateInputVo>\r\n    {\r\n\r\n    }\r\n}\r\n"
            };
            entities.Add(iServicesTemplate);



            TemplateEntity servicesTemplate = new TemplateEntity()
            {
                Name = "Service",
                BuildPath = "D:\\code\\Services\\@ModelService.cs",
                Remarks = "应用服务",
                TemplateStr = "using SqlSugar;\r\nusing Volo.Abp.Application.Dtos;\r\nusing Volo.Abp.Application.Services;\r\nusing Yi.Framework.Ddd.Application;\r\nusing Yi.Framework.Rbac.Application.Contracts.Dtos.@Model;\r\nusing Yi.Framework.Rbac.Application.Contracts.IServices;\r\nusing Yi.Framework.Rbac.Domain.Entities;\r\nusing Yi.Framework.SqlSugarCore.Abstractions;\r\n\r\nnamespace Yi.Framework.Rbac.Application.Services\r\n{\r\n    /// <summary>\r\n    /// @Model服务实现\r\n    /// </summary>\r\n    public class @ModelService : YiCrudAppService<@ModelEntity, @ModelGetOutputDto, @ModelGetListOutputDto, Guid, @ModelGetListInputVo, @ModelCreateInputVo, @ModelUpdateInputVo>,\r\n       I@ModelService\r\n    {\r\n        private ISqlSugarRepository<@ModelEntity, Guid> _repository;\r\n        public @ModelService(ISqlSugarRepository<@ModelEntity, Guid> repository) : base(repository)\r\n        {\r\n            _repository = repository;\r\n        }\r\n\r\n        /// <summary>\r\n        /// 多查\r\n        /// </summary>\r\n        /// <param name=\"input\"></param>\r\n        /// <returns></returns>\r\n        public override async Task<PagedResultDto<@ModelGetListOutputDto>> GetListAsync(@ModelGetListInputVo input)\r\n        {\r\n            RefAsync<int> total = 0;\r\n\r\n            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.@ModelKey), x => x.@ModelKey.Contains(input.@ModelKey!))\r\n                          .WhereIF(!string.IsNullOrEmpty(input.@ModelName), x => x.@ModelName!.Contains(input.@ModelName!))\r\n                          .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)\r\n                          .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);\r\n            return new PagedResultDto<@ModelGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));\r\n        }\r\n    }\r\n}\r\n"
            };
            entities.Add(servicesTemplate);
            return entities;
        }
    }
}
