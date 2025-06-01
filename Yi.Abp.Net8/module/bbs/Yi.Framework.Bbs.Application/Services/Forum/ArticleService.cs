using System.ComponentModel.DataAnnotations;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Article;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Repositories;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Model;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// Article服务实现
    /// </summary>
    public class ArticleService : YiCrudAppService<ArticleAggregateRoot, ArticleGetOutputDto, ArticleGetListOutputDto,
            Guid, ArticleGetListInputVo, ArticleCreateInputVo, ArticleUpdateInputVo>,
        IArticleService
    {
        public ArticleService(IArticleRepository articleRepository,
            ISqlSugarRepository<DiscussAggregateRoot> discussRepository,
            ForumManager forumManager) : base(articleRepository)
        {
            _articleRepository = articleRepository;
            _discussRepository = discussRepository;
            _forumManager = forumManager;
        }

        private readonly ForumManager _forumManager;
        private readonly IArticleRepository _articleRepository;
        private readonly ISqlSugarRepository<DiscussAggregateRoot> _discussRepository;

        public override async Task<PagedResultDto<ArticleGetListOutputDto>> GetListAsync(ArticleGetListInputVo input)
        {
            RefAsync<int> total = 0;

            var entities = await _articleRepository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name!))
                .WhereIF(input.StartTime is not null && input.EndTime is not null,
                    x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<ArticleGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }

        /// <summary>
        /// 查询文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<ArticleGetOutputDto> GetAsync(Guid id)
        {
            var entity = await _articleRepository.GetAsync(id);
            var output = entity.Adapt<ArticleGetOutputDto>();
            if (!await _forumManager.VerifyDiscussPermissionAsync(entity.DiscussId, CurrentUser.Id, CurrentUser.Roles))
            {
                output.SetNoPermission();
            }
            else
            {
                output.SetPassPermission();
            }

            return output;
        }

        /// <summary>
        /// 获取文章全部树级信息
        /// </summary>
        /// <param name="discussId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Route("article/all/discuss-id/{discussId}")]
        public async Task<List<ArticleAllOutputDto>> GetAllAsync([FromRoute] Guid discussId)
        {
            var entities = await _articleRepository.GetTreeAsync(x => x.DiscussId == discussId);
            var items = entities.Adapt<List<ArticleAllOutputDto>>();
            return items;
        }

        /// <summary>
        /// 查询文章概述
        /// </summary>
        /// <param name="discussId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<ArticleGetListOutputDto>> GetDiscussIdAsync([FromRoute] Guid discussId)
        {
            if (!await _discussRepository.IsAnyAsync(x => x.Id == discussId))
            {
                throw new UserFriendlyException(DiscussConst.No_Exist);
            }

            var entities = await _articleRepository.GetTreeAsync(x => x.DiscussId == discussId);
            var items = await MapToGetListOutputDtosAsync(entities);
            return items;
        }

        /// <summary>
        /// 发表文章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Permission("bbs:article:add")]
        [Authorize]
        public override async Task<ArticleGetOutputDto> CreateAsync(ArticleCreateInputVo input)
        {
            await VerifyPermissionAsync(input.DiscussId);
            return await base.CreateAsync(input);
        }

        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ArticleGetOutputDto> UpdateAsync(Guid id, ArticleUpdateInputVo input)
        {
            var entity = await _articleRepository.GetByIdAsync(id);
            await VerifyPermissionAsync(entity.DiscussId);
            return await base.UpdateAsync(id, input);
        }


        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            var entity = await _articleRepository.GetByIdAsync(id);
            await VerifyPermissionAsync(entity.DiscussId);
            await base.DeleteAsync(id);
        }


        /// <summary>
        /// 导入文章
        /// </summary>
        /// <returns></returns>
        public async Task PostImportAsync([FromQuery] ArticleImprotDto input,
            [FromForm] [Required] IFormFileCollection file)
        {
            await VerifyPermissionAsync(input.DiscussId);
            var fileObjs = new List<FileObject>();
            if (file.Count > 0)
            {
                foreach (var item in file)
                {
                    if (item.Length > 0)
                    {
                        using (var stream = item.OpenReadStream())
                        {
                            using (var fileStream = new MemoryStream())
                            {
                                await item.CopyToAsync(fileStream);
                                var bytes = fileStream.ToArray();

                                // 将字节转换成字符串
                                var content = Encoding.UTF8.GetString(bytes);
                                fileObjs.Add(new FileObject() { FileName = item.FileName, Content = content });
                            }
                        }
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("未选择文件");
            }

            //使用简单工厂根据传入的类型进行判断
            await _forumManager.PostImportAsync(input.DiscussId, input.ArticleParentId, fileObjs, input.ImportType);
        }


        private async Task VerifyPermissionAsync(Guid discussId)
        {
            if (!await _forumManager.VerifyDiscussPermissionAsync(discussId, CurrentUser.Id, isVerifyLook: false))
            {
                throw new UserFriendlyException("您无权限进行操作", "403");
            }
        }
    }
}