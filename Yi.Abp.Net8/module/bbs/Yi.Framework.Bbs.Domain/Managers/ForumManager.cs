using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Managers.ArticleImport;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Model;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Managers
{
    /// <summary>
    /// 论坛模块的领域服务
    /// </summary>
    public class ForumManager : DomainService
    {
        public readonly ISqlSugarRepository<DiscussAggregateRoot, Guid> _discussRepository;
        public readonly ISqlSugarRepository<PlateAggregateRoot, Guid> _plateEntityRepository;
        public readonly ISqlSugarRepository<CommentAggregateRoot, Guid> _commentRepository;
        public readonly ISqlSugarRepository<ArticleAggregateRoot, Guid> _articleRepository;
        public  readonly ISqlSugarRepository<DiscussRewardAggregateRoot,Guid> _discussRewardRepository;
        public ForumManager(ISqlSugarRepository<DiscussAggregateRoot, Guid> discussRepository, ISqlSugarRepository<PlateAggregateRoot, Guid> plateEntityRepository, ISqlSugarRepository<CommentAggregateRoot, Guid> commentRepository, ISqlSugarRepository<ArticleAggregateRoot, Guid> articleRepository, ISqlSugarRepository<DiscussRewardAggregateRoot, Guid> discussRewardRepository)
        {
            _discussRepository = discussRepository;
            _plateEntityRepository = plateEntityRepository;
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _discussRewardRepository = discussRewardRepository;
        }

        //主题是不能直接创建的，需要由领域服务统一创建
        public async Task<DiscussAggregateRoot> CreateDiscussAsync(DiscussAggregateRoot entity,DiscussRewardAggregateRoot rewardEntity=null)
        {
            entity.CreationTime = DateTime.Now;
            entity.AgreeNum = 0;
            entity.SeeNum = 0;
            var discuss = await _discussRepository.InsertReturnEntityAsync(entity);
            if (discuss.DiscussType==DiscussTypeEnum.Reward)
            {
                 rewardEntity.DiscussId=discuss.Id;
                 Check.NotNull(rewardEntity,"悬赏类型，悬赏信息不能为空");
                 await  _discussRewardRepository.InsertAsync(rewardEntity);
            }
            return discuss;
        }

        public async Task<CommentAggregateRoot> CreateCommentAsync(Guid discussId, Guid parentId, Guid rootId, string content)
        {
            var entity = new CommentAggregateRoot(discussId);
            entity.Content = content;
            entity.ParentId = parentId;
            entity.RootId = rootId;
            return await _commentRepository.InsertReturnEntityAsync(entity);
        }

        /// <summary>
        /// 校验主题查询权限
        /// </summary>
        /// <param name="discussId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> VerifyDiscussPermissionAsync(Guid discussId,Guid? userId,string[] roles=null,bool isVerifyLook=true)
        {
            var discuss = await _discussRepository.GetFirstAsync(x => x.Id == discussId);
            if (discuss is null)
            {
                throw new UserFriendlyException(DiscussConst.No_Exist);
            }
            //作者是自己，直接有权限
            if (discuss.CreatorId ==userId)
            {
                return true;
            }
            //管理员，直接放行
            if (roles is not null&&roles.Contains(UserConst.AdminRolesCode))
            {
                return true;
            }
            
            //是否为校验 查看权限， 其他操作权限（增删改）
            if (isVerifyLook)
            {
                //要求角色
                if (discuss.PermissionType == DiscussPermissionTypeEnum.Role)
                {
                    if (roles is null)
                    {
                        return false;
                    }
                
                    List<string> roleList = roles.ToList();
                    //所选角色，没有任何交集
                    if (!discuss.PermissionRoleCodes.Intersect(roleList).Any())
                    {
                        return false;
                    }
                }
                
                //通过了上面要求，剩下的都是有权限的，可以直接看
                return true;
            }
            else
            {
                //通过了上面的要求，剩下的就是没有权限了，直接拦截
                return false;
            }
            
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
