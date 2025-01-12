using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Framework.Rbac.Application.Contracts.Dtos.FileManager;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    public interface IFileService : IApplicationService
    {
        /// <summary>
        /// 下载文件,支持缩略图
        /// </summary>
        /// <returns></returns>
        Task<IActionResult> Get([FromRoute] Guid code, [FromRoute] bool? isThumbnail);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        Task<List<FileGetListOutputDto>> Post([FromForm] IFormFileCollection file);
    }
}
