using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Notice;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services
{
    public class BbsNoticeService : ApplicationService
    {
        private ISqlSugarRepository<BbsNoticeAggregateRoot, Guid> _repository;
        public BbsNoticeService(ISqlSugarRepository<BbsNoticeAggregateRoot, Guid> repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// 查询用户的消息，需登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Authorize]
        public async Task<PagedResultDto<BbsNoticeGetListOutputDto>> GetListAsync(BbsNoticeGetListInputVo input)
        {
            RefAsync<int> total = 0;
            var entities = await _repository._DbQueryable.Where(x => x.AcceptUserId == CurrentUser.Id)
                   .OrderByDescending(x => x.CreationTime)
                      .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

            var output = entities.Adapt<List<BbsNoticeGetListOutputDto>>();
            return new PagedResultDto<BbsNoticeGetListOutputDto>(total, output);
        }

        /// <summary>
        /// 已读消息,不传guid，代表一键已读，需登录
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("bbs-notice/read/{noticeId?}")]
        public async Task PutReadAsync(Guid? noticeId)
        {
            //一键已读
            if (noticeId is null)
            {
                await _repository._Db.Updateable<BbsNoticeAggregateRoot>()
                    .SetColumns(it => it.IsRead == true)
                      .Where(x => x.AcceptUserId == CurrentUser.Id)
                      .Where(x => x.IsRead == false)
                      .ExecuteCommandAsync();
            }
            //已读一条
            else
            {
                await _repository._Db.Updateable<BbsNoticeAggregateRoot>()
              .SetColumns(it => it.IsRead == true)
                .Where(x => x.AcceptUserId == CurrentUser.Id)
                .Where(x => x.IsRead == false)
                .Where(x => x.Id == noticeId)
                .ExecuteCommandAsync();

            }

        }

    }
}
