using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Yi.Framework.Core.Enums;
using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Application.Contracts.Dtos.FileManager;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Managers;
using Yi.Framework.Rbac.Domain.Shared.Caches;

namespace Yi.Framework.Rbac.Application.Services
{
    public class FileService : ApplicationService, IFileService
    {
        private readonly IRepository<FileAggregateRoot> _repository;
        private readonly FileManager _fileManager;
        private readonly IMemoryCache _memoryCache;

        public FileService(IRepository<FileAggregateRoot> repository, FileManager fileManager, IMemoryCache memoryCache)
        {
            _repository = repository;
            _fileManager = fileManager;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 下载文件,支持缩略图
        /// </summary>
        /// <returns></returns>
        [Route("file/{code}/{isThumbnail?}")]
        public async Task<IActionResult> Get([FromRoute] Guid code, [FromRoute] bool? isThumbnail)
        {
            var fileCache = await _memoryCache.GetOrCreateAsync($"File:{code}", async (options) =>
            {
                options.AbsoluteExpiration = DateTime.Now.AddDays(1);
                var file = await _repository.GetAsync(x => x.Id == code);
                if (file == null!) return null;
                return file.Adapt<FileCacheItem>();
            });
            var file = fileCache?.Adapt<FileAggregateRoot>();
            var path = file?.GetQueryFileSavePath(isThumbnail);
            if (path is null || !File.Exists(path))
            {
                return new NotFoundResult();
            }
            
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new FileStreamResult(stream, file!.GetMimeMapping());
        }
        
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileGetListOutputDto>> Post([FromForm] IFormFileCollection file)
        {
            var entities = await _fileManager.CreateAsync(file);

            for (int i = 0; i < file.Count; i++)
            {
                var entity = entities[i];
                using (var steam = file[i].OpenReadStream())
                {
                    await _fileManager.SaveFileAsync(entity, steam);
                }
            }

            return entities.Adapt<List<FileGetListOutputDto>>();
        }
    }
}