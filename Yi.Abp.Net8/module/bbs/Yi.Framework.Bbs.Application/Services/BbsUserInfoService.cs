using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TencentCloud.Ame.V20190916.Models;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.Bbs.Application.Services
{
    public class BbsUserInfoService : ApplicationService, IBbsUserInfoService
    {
        private BbsUserManager _bbsUserManager;
        public BbsUserInfoService(BbsUserManager bbsUserManager)
        {
            _bbsUserManager = bbsUserManager;
        }

        [HttpGet("bbs-user/{userName}")]
        public async Task<BbsUserInfoDto> GetUserInfoByUserNameAsync([FromRoute][Required] string userName)
        {

            var userEntity = await _bbsUserManager._userRepository.GetFirstAsync(x => x.UserName == userName);
            if (userEntity == null)
            {
                throw new Volo.Abp.UserFriendlyException("该用户不存在");
            }
            var output =await _bbsUserManager.GetBbsUserInfoAsync(userEntity.Id);

            return output!;
        }
    }
}
