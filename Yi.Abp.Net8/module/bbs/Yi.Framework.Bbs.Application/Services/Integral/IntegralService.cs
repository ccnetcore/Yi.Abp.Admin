using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Integral;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Rbac.Domain.Authorization;

namespace Yi.Framework.Bbs.Application.Services.Integral
{
    public class IntegralService : ApplicationService
    {
        private IntegralManager _integralManager;
        private ICurrentUser _currentUser;
        private ILocalEventBus _localEventBus;
        public IntegralService(IntegralManager integralManager, ICurrentUser currentUser, ILocalEventBus localEventBus)
        {
            _integralManager = integralManager;
            _currentUser = currentUser;
            _localEventBus = localEventBus;
        }


        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<object> PostSignInAsync()
        {
            var value = await _integralManager.SignInAsync(_currentUser.Id ?? Guid.Empty);
            return new { value };
        }

        /// <summary>
        /// 获取本月签到记录
        /// Todo: 可放入领域层
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("integral/sign-in/record")]
        public async Task<SignInDto> GetSignInRecordAsync()
        {
                var output = new SignInDto();
                DateTime lastMonth = DateTime.Now.AddMonths(-1);
                DateTime lastDayOfMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1).AddMonths(1).AddDays(-1);
                DateTime startOfLastDay = new DateTime(lastDayOfMonth.Year, lastDayOfMonth.Month, lastDayOfMonth.Day, 0, 0, 0);

                //获取当前用户本月的数据+上个月最后一天的数据
                var entities = await _integralManager._signInRepository.GetListAsync(x => x.CreatorId == CurrentUser.Id
                && x.CreationTime >= startOfLastDay);

                if (entities.Count() == 0)
                {
                    //返回默认值
                    return output;
                }
                //拿到最末尾的数据
                var lastEntity = entities.OrderBy(x => x.CreationTime).LastOrDefault();

                //判断当前时间和最后时间是否为连续的
                if (lastEntity.CreationTime.Day >= DateTime.Now.AddDays(-1).Day)
                {

                    output.CurrentContinuousNumber = lastEntity.ContinuousNumber;
                }

                //去除上个月查询的数据
                output.SignInItem = entities.Where(x => x.CreationTime.Month == DateTime.Now.Month).Select(x => new SignInItemDto { Id = x.Id, CreationTime = x.CreationTime }).OrderBy(x => x.CreationTime).ToList();
                return output;
  

        }
    }
}
