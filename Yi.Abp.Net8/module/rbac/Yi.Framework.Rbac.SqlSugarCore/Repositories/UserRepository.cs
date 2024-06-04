using Mapster;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Yi.Framework.Rbac.SqlSugarCore.Repositories
{
    public class UserRepository : SqlSugarRepository<UserAggregateRoot>, IUserRepository, ITransientDependency
    {
        public UserRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
        {
        }
        /// <summary>
        /// 获取用户ids的全部信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<UserAggregateRoot>> GetListUserAllInfoAsync(List<Guid> userIds)
        {
            var users = await _DbQueryable.Where(x => userIds.Contains(x.Id)).Includes(u => u.Roles.Where(r => r.IsDeleted == false).ToList(), r => r.Menus.Where(m => m.IsDeleted == false).ToList()).ToListAsync();
            return users;
        }


        /// <summary>
        /// 获取用户id的全部信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<UserAggregateRoot> GetUserAllInfoAsync(Guid userId)
        {
            //得到用户
            var user = await _DbQueryable.Includes(u => u.Roles.Where(r => r.IsDeleted == false).ToList(), r => r.Menus.Where(m => m.IsDeleted == false).ToList()).InSingleAsync(userId);
            return user;
        }




    }
}
