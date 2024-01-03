using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Rbac.Domain.Shared.Consts;

namespace Yi.Framework.Bbs.Application.Services.Analyses
{
    public class BbsUserAnalyseService : ApplicationService, IApplicationService
    {
        private BbsUserManager _bbsUserManager;
        public BbsUserAnalyseService(BbsUserManager bbsUserManager)
        {
            _bbsUserManager = bbsUserManager;
        }

        /// <summary>
        /// 推荐好友，随机返回好友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("analyse/bbs-user/random")]
        public async Task<List<BbsUserGetListOutputDto>> GetRandomUserAsync([FromQuery] PagedResultRequestDto input)
        {
            var randUserIds = await _bbsUserManager._userRepository._DbQueryable
                .Where(x => x.UserName != UserConst.Admin)
                .OrderBy(x => SqlFunc.GetRandom())
                .Select(x => x.Id).
                ToPageListAsync(input.SkipCount, input.MaxResultCount);
            var output = await _bbsUserManager.GetBbsUserInfoAsync(randUserIds);
            return output.Adapt<List<BbsUserGetListOutputDto>>();
        }

        /// <summary>
        /// 积分排行榜
        /// </summary>
        /// <returns></returns>
        [HttpGet("analyse/bbs-user/integral-top")]
        public async Task<List<BbsUserGetListOutputDto>> GetIntegralTopUserAsync([FromQuery] PagedResultRequestDto input)
        {
            var randUserIds = await _bbsUserManager._userRepository._DbQueryable
                .Where(x => x.UserName != UserConst.Admin)
                .OrderBy(x => SqlFunc.GetRandom())
                .Select(x => x.Id).
                ToPageListAsync(input.SkipCount, input.MaxResultCount);
            var output = await _bbsUserManager.GetBbsUserInfoAsync(randUserIds);
            return output.Adapt<List<BbsUserGetListOutputDto>>();
        }

    }
}
