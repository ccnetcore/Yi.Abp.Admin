using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Integral
{
    /// <summary>
    /// 签到表
    /// </summary>
    [SugarTable("SignIn")]

    [SugarIndex($"index_{nameof(CreatorId)}", nameof(CreatorId), OrderByType.Asc)]
    public class SignInAggregateRoot : AggregateRoot<Guid>, ICreationAuditedObject
    {

        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        //签到用户
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 连续签到次数
        /// </summary>
        public int ContinuousNumber { get; set; } = 1;

    }
}
