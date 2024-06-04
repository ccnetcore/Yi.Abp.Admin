using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Abp.Tool.Application.Contracts.Dtos;

namespace Yi.Abp.Tool.Application.Contracts
{
    public interface ITemplateGenService: IApplicationService
    {
        Task<byte[]> CreateModuleAsync(TemplateGenCreateInputDto moduleCreateInputDto);
        Task<byte[]> CreateProjectAsync(TemplateGenCreateInputDto moduleCreateInputDto);
    }
}
