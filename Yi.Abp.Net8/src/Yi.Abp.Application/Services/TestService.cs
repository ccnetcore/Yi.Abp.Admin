using Mapster;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Banner;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Extensions;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Abp.Application.Services
{
    /// <summary>
    /// 常用魔改及扩展示例
    /// </summary>
    public class TestService : ApplicationService
    {
        /// <summary>
        /// 属性注入
        /// 不推荐，坑太多，容易把自己玩死，简单的东西可以用一用
        /// </summary>
        public ISqlSugarRepository<BannerEntity> sqlSugarRepository { get; set; }

        /// <summary>
        /// 动态Api
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("hello-world")]
        public string GetHelloWorld(string? name)
        {
            //会自动添加前缀，而不是重置，更符合习惯
            //如果需要重置以"/"根目录开头即可
            //你好世界
            return name ?? "HelloWord";
        }

        /// <summary>
        /// SqlSugar
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetSqlSugarDbAsync()
        {
            //用户体验优先，可直接使用Db操作，依赖抽象
            return await sqlSugarRepository._DbQueryable.ToListAsync();
        }

        /// <summary>
        /// 工作单元
        /// </summary>
        /// <returns></returns>
        public async Task GetUowAsync()
        {
            //魔改
            // 用户体验优先，万金油模式，支持高并发。支持单、多线程并发安全，支持多线程工作单元，支持多线程无工作单元，支持。。。
            // 自动在各个情况处理db客户端最优解之一
            int i = 10;
            List<Task> tasks = new List<Task>();

            while (i > 0)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await sqlSugarRepository.InsertAsync(new BannerEntity { Name = "插入2" });
                    using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
                    {
                        await sqlSugarRepository.InsertAsync(new BannerEntity { Name = "插入1" });
                        await uow.CompleteAsync();
                    }
                }));
                await sqlSugarRepository.InsertAsync(new BannerEntity { Name = "插入3" });
                i--;
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        /// <returns></returns>
        public void GetCurrentUser()
        {
            //当token鉴权之后，可以直接获取
            if (CurrentUser.Id is not null)
            {
                //权限
                CurrentUser.GetPermissions();

                //角色信息
                CurrentUser.GetRoleInfo();

                //部门id
                CurrentUser.GetDeptId();
            }
        }

        /// <summary>
        /// 数据权限
        /// </summary>
        public void GetDataFilter()
        {
            //这里会数据权限过滤
            using (DataFilter.DisablePermissionHandler())
            {
                //这里不会数据权限过滤
            }
            //这里会数据权限过滤
        }

        /// <summary>
        /// 对象映射
        /// </summary>
        public void GetMapper()
        {
            //直接无脑Adapt，无需配置
            var entity = new BannerEntity();
            var dto = entity.Adapt<BannerGetListOutputDto>();
        }
    }
}
