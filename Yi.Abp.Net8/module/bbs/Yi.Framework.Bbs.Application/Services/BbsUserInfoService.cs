using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Managers;

namespace Yi.Framework.Bbs.Application.Services
{
    public class BbsUserInfoService : ApplicationService, IBbsUserInfoService
    {
        private BbsUserManager _bbsUserManager;
        public BbsUserInfoService(BbsUserManager bbsUserManager)
        {
            _bbsUserManager = bbsUserManager;
        }

        [HttpGet("bbs-user/{userNameOrUserId}")]
        public async Task<BbsUserInfoDto> GetUserInfoByUserNameOrUserIdAsync([FromRoute][Required] string userNameOrUserId)
        {
            Guid userId;
            if (Guid.TryParse(userNameOrUserId, out var userGuidId))
            {
                userId = userGuidId;
            }
            else
            {
                var userEntity = await _bbsUserManager._userRepository.GetFirstAsync(x => x.UserName == userNameOrUserId);
                if (userEntity == null)
                {
                    throw new UserFriendlyException("该用户不存在");
                }
                userId= userEntity.Id;
            }

            var output =await _bbsUserManager.GetBbsUserInfoAsync(userId);
           
            //不是自己
            if (CurrentUser.Id != output.Id)
            {
                output.Phone = null;
                output.Email=null;
            }
            return output!;
        }
    }
}
