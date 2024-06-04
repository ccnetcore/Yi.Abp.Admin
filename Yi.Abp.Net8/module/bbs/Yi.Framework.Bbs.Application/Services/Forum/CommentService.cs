using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Comment;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Extensions;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// 评论
    /// </summary>
    public class CommentService : YiCrudAppService<CommentAggregateRoot, CommentGetOutputDto, CommentGetListOutputDto, Guid, CommentGetListInputVo, CommentCreateInputVo, CommentUpdateInputVo>,
       ICommentService
    {
        private readonly ISqlSugarRepository<CommentAggregateRoot, Guid> _repository;
        private readonly BbsUserManager _bbsUserManager;
        public CommentService(ForumManager forumManager, ISqlSugarRepository<DiscussAggregateRoot> discussRepository, IDiscussService discussService, ISqlSugarRepository<CommentAggregateRoot, Guid> CommentRepository, BbsUserManager bbsUserManager) : base(CommentRepository)
        {
            _forumManager = forumManager;
            _discussRepository = discussRepository;
            _discussService = discussService;
            _repository = CommentRepository;
            _bbsUserManager = bbsUserManager;
        }

        private ForumManager _forumManager { get; set; }



        private ISqlSugarRepository<DiscussAggregateRoot> _discussRepository { get; set; }

        private IDiscussService _discussService { get; set; }
        /// <summary>
        /// 获取改主题下的评论,结构为二维列表，该查询无分页
        /// Todo: 可放入领域层
        /// </summary>
        /// <param name="discussId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<CommentGetListOutputDto>> GetDiscussIdAsync([FromRoute] Guid discussId, [FromQuery] CommentGetListInputVo input)
        {
            await _discussService.VerifyDiscussPermissionAsync(discussId);

            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.Content), x => x.Content.Contains(input.Content))
              .Where(x => x.DiscussId == discussId)
              .Includes(x => x.CreateUser)
             .ToListAsync();

            //该实体需要进行转换

            //同时为所有用户id进行bbs的扩展即可
            List<Guid> userIds = entities.Where(x => x.CreatorId != null).Select(x => x.CreatorId ?? Guid.Empty).ToList();
            var bbsUserInfoDic = (await _bbsUserManager.GetBbsUserInfoAsync(userIds)).ToDictionary(x => x.Id);





            //------数据查询完成------





            //从根目录开始组装
            //结果初始值，第一层等于全部根节点
            var allOutPut = entities.OrderByDescending(x => x.CreationTime).ToList();

            //获取全量主题评论， 先获取顶级的，将其他子组合到顶级下，形成一个二维,先转成dto
            List<CommentGetListOutputDto> allOutoutDto = await MapToGetListOutputDtosAsync(allOutPut);

            //开始映射额外用户信息字段
            allOutoutDto?.ForEach(x => x.CreateUser = bbsUserInfoDic[x.CreatorId ?? Guid.Empty].Adapt<BbsUserGetOutputDto>());

            //开始组装dto的层级关系
            //将全部数据进行hash
            var dic = allOutoutDto.ToDictionary(x => x.Id);

            foreach (var comment in allOutoutDto)
            {
                //不是根节点，需要赋值 被评论者用户信息等
                if (comment.ParentId != Guid.Empty)
                {
                    if (dic.ContainsKey(comment.ParentId))
                    {
                        var parentComment = dic[comment.ParentId];
                        comment.CommentedUser = parentComment.CreateUser;
                    }
                    else
                    {
                        continue;
                    }
                }
                //root或者parent id，根节点都是等于0的
                var id = comment.RootId;
                if (id != Guid.Empty)
                {
                    dic[id].Children.Add(comment);
                }

            }

            //子类需要排序
            var rootOutoutDto = allOutoutDto.Where(x => x.ParentId == Guid.Empty).ToList();
            rootOutoutDto?.ForEach(x =>
            {
                x.Children = x.Children.OrderByDescending(x => x.CreationTime).ToList();

            });



            return new PagedResultDto<CommentGetListOutputDto>(entities.Count(), rootOutoutDto);
        }


        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Permission("bbs:comment:add")]
        [Authorize]
        public override async Task<CommentGetOutputDto> CreateAsync(CommentCreateInputVo input)
        {
            var discuess = await _discussRepository.GetFirstAsync(x => x.Id == input.DiscussId);
            if (discuess is null)
            {
                throw new UserFriendlyException(DiscussConst.No_Exist);
            }
            //不是超级管理员，且主题开启禁止评论

            if (discuess.IsDisableCreateComment == true && !CurrentUser.GetPermissions().Contains(UserConst.AdminPermissionCode))
            {
                throw new UserFriendlyException("该主题已禁止评论功能");
            }


            var entity = await _forumManager.CreateCommentAsync(input.DiscussId, input.ParentId, input.RootId, input.Content);
            return await MapToGetOutputDtoAsync(entity);
        }

    }
}
