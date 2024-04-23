using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.Rbac.Domain.Shared.Caches;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Options;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Domain.Managers
{
    public class UserManager : DomainService
    {
        public readonly ISqlSugarRepository<UserEntity> _repository;
        public readonly ISqlSugarRepository<UserRoleEntity> _repositoryUserRole;
        public readonly ISqlSugarRepository<UserPostEntity> _repositoryUserPost;
        private IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> _userCache;
        private readonly IGuidGenerator _guidGenerator;
        private IUserRepository _userRepository;
        public UserManager(ISqlSugarRepository<UserEntity> repository, ISqlSugarRepository<UserRoleEntity> repositoryUserRole, ISqlSugarRepository<UserPostEntity> repositoryUserPost, IGuidGenerator guidGenerator, IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> userCache, IUserRepository userRepository) =>
            (_repository, _repositoryUserRole, _repositoryUserPost, _guidGenerator, _userCache, _userRepository) =
            (repository, repositoryUserRole, repositoryUserPost, guidGenerator, userCache, userRepository);

        /// <summary>
        /// 给用户设置角色
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task GiveUserSetRoleAsync(List<Guid> userIds, List<Guid> roleIds)
        {
            //删除用户之前所有的用户角色关系（物理删除，没有恢复的必要）
            await _repositoryUserRole.DeleteAsync(u => userIds.Contains(u.UserId));

            if (roleIds is not null)
            {
                //遍历用户
                foreach (var userId in userIds)
                {
                    //添加新的关系
                    List<UserRoleEntity> userRoleEntities = new();

                    foreach (var roleId in roleIds)
                    {
                        userRoleEntities.Add(new UserRoleEntity() { UserId = userId, RoleId = roleId });
                    }
                    //一次性批量添加
                    await _repositoryUserRole.InsertRangeAsync(userRoleEntities);
                }
            }
        }


        /// <summary>
        /// 给用户设置岗位
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="postIds"></param>
        /// <returns></returns>
        public async Task GiveUserSetPostAsync(List<Guid> userIds, List<Guid> postIds)
        {
            //删除用户之前所有的用户角色关系（物理删除，没有恢复的必要）
            await _repositoryUserPost.DeleteAsync(u => userIds.Contains(u.UserId));
            if (postIds is not null)
            {
                //遍历用户
                foreach (var userId in userIds)
                {
                    //添加新的关系
                    List<UserPostEntity> userPostEntities = new();
                    foreach (var post in postIds)
                    {
                        userPostEntities.Add(new UserPostEntity() { UserId = userId, PostId = post });
                    }

                    //一次性批量添加
                    await _repositoryUserPost.InsertRangeAsync(userPostEntities);
                }

            }
        }


        /// <summary>
        /// 查询用户信息，已缓存
        /// </summary>
        /// <returns></returns>
        public async Task<UserRoleMenuDto> GetInfoAsync(Guid userId)
        {
            var user = await _userRepository.GetUserAllInfoAsync(userId);
            var output = await GetInfoByCacheAsync(user);
            return output;
        }

        /// <summary>
        /// 批量查询用户信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public async Task<List<UserRoleMenuDto>> GetInfoListAsync(List<Guid> userIds)
        {
            List<UserRoleMenuDto> output = new List<UserRoleMenuDto>();
            var users = await _userRepository.GetListUserAllInfoAsync(userIds);
            foreach (var user in users)
            {
                output.Add(await GetInfoByCacheAsync(user));
            }
            return output;
        }

        private async Task<UserRoleMenuDto> GetInfoByCacheAsync(UserEntity user)
        {
            //此处优先从缓存中获取
            UserRoleMenuDto output = null;
            var tokenExpiresMinuteTime = LazyServiceProvider.GetRequiredService<IOptions<JwtOptions>>().Value.ExpiresMinuteTime;
            var cacheData = await _userCache.GetOrAddAsync(new UserInfoCacheKey(user.Id),
               async () =>
               {
                   var data = EntityMapToDto(user);
                   //系统用户数据被重置，老前端访问重新授权
                   if (data is null)
                   {
                       throw new AbpAuthorizationException();
                   }
                   data.Menus.Clear();
                   output = data;
                   return new UserInfoCacheItem(data);
               },
             () => new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(tokenExpiresMinuteTime) });

            if (cacheData is not null)
            {
                output = cacheData.Info;
            }
            return output!;
        }

        private UserRoleMenuDto EntityMapToDto(UserEntity user)
        {

            var userRoleMenu = new UserRoleMenuDto();
            //首先获取到该用户全部信息，导航到角色、菜单，(菜单需要去重,完全交给Set来处理即可)
            //if (user is null)
            //{
            //    throw new UserFriendlyException($"数据错误，用户id：{nameof(userId)} 不存在，请重新登录");
            //}
            user.Password = string.Empty;
            user.Salt = string.Empty;

            //超级管理员特殊处理
            if (UserConst.Admin.Equals(user.UserName))
            {
                userRoleMenu.User = user.Adapt<UserDto>();
                userRoleMenu.RoleCodes.Add(UserConst.AdminRolesCode);
                userRoleMenu.PermissionCodes.Add(UserConst.AdminPermissionCode);
                return userRoleMenu;
            }

            //得到角色集合
            var roleList = user.Roles;

            //得到菜单集合
            foreach (var role in roleList)
            {
                userRoleMenu.RoleCodes.Add(role.RoleCode);

                if (role.Menus is not null)
                {
                    foreach (var menu in role.Menus)
                    {
                        if (!string.IsNullOrEmpty(menu.PermissionCode))
                        {
                            userRoleMenu.PermissionCodes.Add(menu.PermissionCode);
                        }
                        userRoleMenu.Menus.Add(menu.Adapt<MenuDto>());
                    }
                }

                //刚好可以去除一下多余的导航属性
                role.Menus = new List<MenuEntity>();
                userRoleMenu.Roles.Add(role.Adapt<RoleDto>());
            }

            user.Roles = new List<RoleEntity>();
            userRoleMenu.User = user.Adapt<UserDto>();
            userRoleMenu.Menus = userRoleMenu.Menus.OrderByDescending(x => x.OrderNum).ToHashSet();
            return userRoleMenu;
        }
    }

}