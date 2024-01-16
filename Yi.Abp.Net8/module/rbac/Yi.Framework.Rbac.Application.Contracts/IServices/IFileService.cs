using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
using Yi.Framework.Rbac.Application.Contracts.Dtos.FileManager;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    public interface IFileService : IApplicationService
    {
        Task<string> GetReturnPathAsync(Guid code, bool? isThumbnail);
        Task<List<FileGetListOutputDto>> Post(IFormFileCollection file);
    }
}
