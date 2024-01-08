using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.Authorization
{    /// <summary>
     /// 第三方授权表
     ///</summary>
    [SugarTable("Auth")]
    public class AuthAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IHasCreationTime
    {

        public AuthAggregateRoot() { }

        public AuthAggregateRoot(string authType, Guid userId, string openId)
        {
            AuthType = authType;
            OpenId = openId;
            UserId = userId;

        }
        public AuthAggregateRoot(string authType, Guid userId, string openId, string name) : this(authType, userId, openId)
        {
            Name = name;
        }


        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public Guid UserId { get; set; }

        public string OpenId { get; set; }

        public string Name { get; set; }

        public string AuthType { get; set; }

        public bool IsDeleted { get; set; }

        [SugarColumn(IsIgnore = true)]
        public override ExtraPropertyDictionary ExtraProperties { get; protected set; }

        public DateTime CreationTime { get; set; }
    }
}
