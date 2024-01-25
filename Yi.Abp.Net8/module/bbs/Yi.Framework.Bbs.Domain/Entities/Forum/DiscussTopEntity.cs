using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    /// <summary>
    /// 首页置顶主题
    /// </summary>
    [SugarTable("DiscussTop")]
    public class DiscussTopEntity : Entity<Guid>, IHasModificationTime
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        public int OrderNum { get; set; }

        public Guid DiscussId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
