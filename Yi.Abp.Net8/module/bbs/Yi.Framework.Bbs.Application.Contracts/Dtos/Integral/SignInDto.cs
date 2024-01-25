using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Integral
{
    public class SignInDto
    {
        /// <summary>
        /// 签到数据
        /// </summary>
        public List<SignInItemDto> SignInItem { get; set; }=new List<SignInItemDto>();

        /// <summary>
        /// 当前连续签到次数
        /// </summary>
        public int CurrentContinuousNumber { get; set; }
    }


    public class SignInItemDto : EntityDto<Guid>
    {
        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
