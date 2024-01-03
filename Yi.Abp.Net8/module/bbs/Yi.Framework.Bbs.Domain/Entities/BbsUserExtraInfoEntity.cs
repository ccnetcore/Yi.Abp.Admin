using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Domain.Entities
{
    /// <summary>
    /// 评论表
    /// </summary>
    [SugarTable("BbsUserExtraInfo")]
    public class BbsUserExtraInfoEntity : Entity<Guid>
    {
        public BbsUserExtraInfoEntity() { }

        public BbsUserExtraInfoEntity(Guid userId) { this.UserId = userId; }

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 用户限制
        /// </summary>
        public UserLimitEnum UserLimit { get; set; } = UserLimitEnum.Normal;
    }

}
