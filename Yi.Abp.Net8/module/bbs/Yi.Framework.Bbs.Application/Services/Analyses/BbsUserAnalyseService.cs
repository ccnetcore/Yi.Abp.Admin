using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Model;

namespace Yi.Framework.Bbs.Application.Services.Analyses
{
    public class BbsUserAnalyseService : ApplicationService, IApplicationService
    {
        private BbsUserManager _bbsUserManager;
        private IOnlineService _onlineService;
        public BbsUserAnalyseService(BbsUserManager bbsUserManager, IOnlineService onlineService)
        {
            _bbsUserManager = bbsUserManager;
            _onlineService = onlineService;
        }

        /// <summary>
        /// 推荐好友，随机返回好友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("analyse/bbs-user/random")]
        public async Task<List<BbsUserGetListOutputDto>> GetRandomUserAsync([FromQuery] PagedResultRequestDto input)
        {
            var randUserIds = await _bbsUserManager._userRepository._DbQueryable
               //.Where(x => x.UserName != UserConst.Admin)
                .OrderBy(x => SqlFunc.GetRandom())
                .Select(x => x.Id).
                ToPageListAsync(input.SkipCount, input.MaxResultCount);
            var output = await _bbsUserManager.GetBbsUserInfoAsync(randUserIds);
            return output.Adapt<List<BbsUserGetListOutputDto>>();
        }

        /// <summary>
        /// 积分钱钱排行榜
        /// </summary>
        /// <returns></returns>
        [HttpGet("analyse/bbs-user/integral-top")]
        public async Task<List<BbsUserGetListOutputDto>> GetIntegralTopUserAsync([FromQuery] PagedResultRequestDto input)
        {
            var randUserIds = await _bbsUserManager._userRepository._DbQueryable
               // .Where(user => user.UserName != UserConst.Admin)
                .LeftJoin<BbsUserExtraInfoEntity>((user, info) => user.Id==info.UserId)
                .OrderByDescending((user, info)=>info.Money)
                .Select((user, info) => user.Id).
                ToPageListAsync(input.SkipCount, input.MaxResultCount);
            var output = await _bbsUserManager.GetBbsUserInfoAsync(randUserIds);
            return output.OrderByDescending(x=>x.Money).ToList().Adapt<List<BbsUserGetListOutputDto>>();
        }

        /// <summary>
        /// 用户分析
        /// </summary>
        /// <returns></returns>
        [HttpGet("analyse/bbs-user")]
        public async Task<BbsUserAnalyseGetOutput> GetUserAnalyseAsync()
        {

            var registerUser = await _bbsUserManager._userRepository._DbQueryable.CountAsync();


            DateTime now = DateTime.Now;
            DateTime yesterday = now.AddDays(-1);
            DateTime startTime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0);
            DateTime endTime = startTime.AddHours(24);
            var yesterdayNewUser = await _bbsUserManager._userRepository._DbQueryable
                  .Where(x => x.CreationTime >= startTime && x.CreationTime <= endTime).CountAsync();

            var userOnline = (await _onlineService.GetListAsync(new OnlineUserModel { })).TotalCount;

            var output = new BbsUserAnalyseGetOutput() { OnlineNumber = userOnline, RegisterNumber = registerUser, YesterdayNewUser = yesterdayNewUser };

            return output;
        }

    }
}
