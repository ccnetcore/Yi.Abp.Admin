using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Services;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Managers.ArticleImport;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Model;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Managers
{
    /// <summary>
    /// 论坛模块的领域服务
    /// </summary>
    public class ForumManager : DomainService
    {
        public readonly ISqlSugarRepository<DiscussEntity, Guid> _discussRepository;
        public readonly ISqlSugarRepository<PlateEntity, Guid> _plateEntityRepository;
        public readonly ISqlSugarRepository<CommentEntity, Guid> _commentRepository;
        public readonly ISqlSugarRepository<ArticleEntity, Guid> _articleRepository;
        public ForumManager(ISqlSugarRepository<DiscussEntity, Guid> discussRepository, ISqlSugarRepository<PlateEntity, Guid> plateEntityRepository, ISqlSugarRepository<CommentEntity, Guid> commentRepository, ISqlSugarRepository<ArticleEntity, Guid> articleRepository)
        {
            _discussRepository = discussRepository;
            _plateEntityRepository = plateEntityRepository;
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
        }

        //主题是不能直接创建的，需要由领域服务统一创建
        public async Task<DiscussEntity> CreateDiscussAsync(DiscussEntity entity)
        {
            entity.CreationTime = DateTime.Now;
            entity.AgreeNum = 0;
            entity.SeeNum = 0;
            return await _discussRepository.InsertReturnEntityAsync(entity);
        }

        public async Task<CommentEntity> CreateCommentAsync(Guid discussId, Guid parentId, Guid rootId, string content)
        {
            var entity = new CommentEntity(discussId);
            entity.Content = content;
            entity.ParentId = parentId;
            entity.RootId = rootId;
            return await _commentRepository.InsertReturnEntityAsync(entity);
        }

        /// <summary>
        /// 导入文章
        /// </summary>
        /// <param name="discussId"></param>
        /// <param name="articleParentId"></param>
        /// <param name="fileObjs"></param>
        /// <param name="importType"></param>
        /// <returns></returns>
        public async Task PostImportAsync(Guid discussId,Guid articleParentId, List<FileObject> fileObjs, ArticleImportTypeEnum importType)
        {
            AbstractArticleImport abstractArticleImport = default;
            switch (importType)
            {
                case ArticleImportTypeEnum.Default:
                    abstractArticleImport = new DefaultArticleImport();

                    break;
                case ArticleImportTypeEnum.VuePress:
                    abstractArticleImport = new VuePressArticleImport();
                    break;

                default: abstractArticleImport = new DefaultArticleImport(); break;
            }
            abstractArticleImport.SetLogger(LoggerFactory);
            var articleHandled = abstractArticleImport.Import(discussId, articleParentId, fileObjs);

            await _articleRepository.InsertManyAsync(articleHandled);

        }
    }
}
