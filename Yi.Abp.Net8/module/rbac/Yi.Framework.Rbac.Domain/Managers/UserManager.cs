using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Domain.Managers
{
    public class UserManager : DomainService
    {
        private readonly ISqlSugarRepository<UserEntity> _repository;
        private readonly ISqlSugarRepository<UserRoleEntity> _repositoryUserRole;
        private readonly ISqlSugarRepository<UserPostEntity> _repositoryUserPost;

        private readonly IGuidGenerator _guidGenerator;
        public UserManager(ISqlSugarRepository<UserEntity> repository, ISqlSugarRepository<UserRoleEntity> repositoryUserRole, ISqlSugarRepository<UserPostEntity> repositoryUserPost, IGuidGenerator guidGenerator) =>
            (_repository, _repositoryUserRole, _repositoryUserPost, _guidGenerator) =
            (repository, repositoryUserRole, repositoryUserPost, guidGenerator);

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

    }

}