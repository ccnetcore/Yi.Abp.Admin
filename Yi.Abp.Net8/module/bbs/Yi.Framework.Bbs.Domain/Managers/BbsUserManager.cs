using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Managers
{
    public class BbsUserManager : DomainService
    {
        public ISqlSugarRepository<UserEntity> _userRepository;
        public ISqlSugarRepository<BbsUserExtraInfoEntity> _bbsUserInfoRepository;
        public BbsUserManager(ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<BbsUserExtraInfoEntity> bbsUserInfoRepository)
        {
            _userRepository = userRepository;
            _bbsUserInfoRepository = bbsUserInfoRepository;
        }

        public async Task<BbsUserInfoDto?> GetBbsUserInfoAsync(Guid userId)
        {
            return await _userRepository._DbQueryable.LeftJoin<BbsUserExtraInfoEntity>((user, info) => user.Id == info.UserId)
                     .Select((user, info) => new BbsUserInfoDto { 
                         Id = user.Id ,
                         Icon=user.Icon,
                         Level=info.Level,
                         UserLimit=info.UserLimit,
                         Money = info.Money,
                         Experience = info.Experience
                     }, true)
                     .FirstAsync(user => user.Id==userId);
        }

        public async Task<List<BbsUserInfoDto>> GetBbsUserInfoAsync(List<Guid> userIds)
        {
            return await _userRepository._DbQueryable
                     .Where(user => userIds.Contains(user.Id))
                .LeftJoin<BbsUserExtraInfoEntity>((user, info) => user.Id == info.UserId)
                     .Select((user, info) => new BbsUserInfoDto { Id = user.Id , Icon = user.Icon , Level = info.Level, UserLimit = info.UserLimit,
                         Money = info.Money,
                         Experience = info.Experience
                     },true)
                
                     .ToListAsync();
        }
    }

    public class BbsUserInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Nick { get; set; }
        public string? Email { get; set; }
        public string? Ip { get; set; }
        public string? Address { get; set; }
        public long? Phone { get; set; }
        public string? Introduction { get; set; }
        public string? Remark { get; set; }
        public SexEnum Sex { get; set; } = SexEnum.Unknown;
        public bool State { get; set; }
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 用户限制
        /// </summary>
        public UserLimitEnum UserLimit { get; set; }


        /// <summary>
        /// 钱钱
        /// </summary>
        public decimal Money { get; set; }


        /// <summary>
        /// 经验
        /// </summary>
        public long Experience { get; set; }

    }
}
