using Mapster;
using Medallion.Threading;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Extensions;
using Yi.Framework.SettingManagement.Domain;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Abp.Application.Services
{
    /// <summary>
    /// 常用魔改及扩展示例
    /// </summary>
    public class TestService : ApplicationService
    {
        // /// <summary>
        // /// 属性注入
        // /// 不推荐，坑太多，容易把自己玩死，简单的东西可以用一用
        // /// </summary>
        public ISqlSugarRepository<ConfigAggregateRoot> sqlSugarRepository { get; set; }

        /// <summary>
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
        /// 异常处理
        /// </summary>
        /// <returns></returns>
        [HttpGet("error")]
        public string GetError()
        {
            throw new UserFriendlyException("业务异常");
            throw new Exception("系统异常");
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
            // 用户体验优先，万金油模式，支持高并发。支持单、多线程并发安全，支持多线程工作单元，支持。。。
            // 不支持多线程无工作单元，应由工作单元统一管理（来自abp工作单元设计）
            // 请注意，如果requiresNew: true只有在没有工作单元内使用，嵌套子工作单元，默认值false即可
            // 自动在各个情况处理db客户端最优解之一
            int i = 3;
            List<Task> tasks = new List<Task>();
            await sqlSugarRepository.GetListAsync();
            await sqlSugarRepository.GetListAsync();
            while (i > 0)
            {
                tasks.Add(Task.Run(async () =>
                {
                    //以下操作是错误的，不允许在新线程中，直接操作db，所有db操作应放在工作单元内，应由工作单元统一管理-来自abp工作单元设计
                    //await sqlSugarRepository.InsertAsync(new BannerAggregateRoot { Name = "插入2" });
                    using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
                    {
                        await sqlSugarRepository.InsertAsync(new ConfigAggregateRoot { ConfigKey = "插入1" });
                        await uow.CompleteAsync();
                    }
                }));
                await sqlSugarRepository.InsertAsync(new ConfigAggregateRoot { ConfigKey = "插入3" });
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
            var entity = new ConfigAggregateRoot();
            var dto = entity.Adapt<ConfigAggregateRoot>();
        }

        private static int RequestNumber { get; set; } = 0;

        /// <summary>
        /// 速率限制
        /// </summary>
        /// <returns></returns>
        // [DisableRateLimiting]
        //[EnableRateLimiting("sliding")]
        public int GetRateLimiting()
        {
            RequestNumber++;
            return RequestNumber;
        }


        public ISettingProvider _settingProvider { get; set; }

        public ISettingManager _settingManager { get; set; }

        /// <summary>
        /// 系统配置模块
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSettingAsync()
        {
            //DDD需要提前定义
            //默认来说，不提供修改操作，配置应该独立
            var enableOrNull = await _settingProvider.GetOrNullAsync("DDD");

            //如果要进行修改，可使用yi.framework下的ISettingManager
            await _settingManager.SetGlobalAsync("DDD", "false");

            var enableOrNull2 = await _settingManager.GetOrNullGlobalAsync("DDD");

            //当然，他的独特地方，是支持来自多个模块，例如配置文件？
            var result = await _settingManager.GetOrNullConfigurationAsync("Test");


            return result ?? string.Empty;
        }

        
        /// <summary>
        /// 分布式送abp版本：abp套了一层娃。但是纯粹鸡肋，不建议使用这个
        /// </summary>
        public IAbpDistributedLock AbpDistributedLock => LazyServiceProvider.LazyGetService<IAbpDistributedLock>();

        /// <summary>
        /// 分布式锁推荐使用版本：yyds，分布式锁永远的神！
        /// </summary>
        public IDistributedLockProvider DistributedLock => LazyServiceProvider.LazyGetService<IDistributedLockProvider>();

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <remarks>强烈吐槽一下abp，正如他们所说，abp的分布式锁单纯为了自己用，一切还是以DistributedLock为主</remarks>>
        /// <returns></returns>
        public async Task<string> GetDistributedLockAsync()
        {
            var number = 0;
            await Parallel.ForAsync(0, 100, async (i, cancellationToken) =>
            {
                await using (await DistributedLock.AcquireLockAsync("MyLockName"))
                {
                    //执行1秒
                    number += 1;
                }
            });
            var number2 = 0;
            await Parallel.ForAsync(0, 100, async (i, cancellationToken) =>
            {
                    //执行1秒
                    number2 += 1;
            });
            return $"加锁结果：{number},不加锁结果：{number2}";
        }

        public ICurrentTenant CurrentTenant { get; set; }
        public IRepository<ConfigAggregateRoot> repository { get; set; }
        /// <summary>
        /// 多租户
        /// </summary>
        /// <returns></returns>
        public async Task<string>  GetMultiTenantAsync()
        {
            using (var uow=UnitOfWorkManager.Begin())
            {
                //此处会实例化一个db,连接默认库
                var defautTenantData1= await repository.GetListAsync();
                using (CurrentTenant.Change(null,"Default"))
                {
                    var defautTenantData2= await repository.GetListAsync();
                    await repository.InsertAsync(new ConfigAggregateRoot
                    {
                        ConfigKey = "default",
                    });
                    var defautTenantData3= await repository.GetListAsync(x=>x.ConfigKey=="default");
                }
                //此处会实例化一个新的db连接MES
                using (CurrentTenant.Change(null,"Mes"))
                {
                    var otherTenantData1= await repository.GetListAsync();
                    await repository.InsertAsync(new ConfigAggregateRoot
                    {
                        ConfigKey = "Mes1",
                    });
                    var otherTenantData2= await repository.GetListAsync(x=>x.ConfigKey=="Mes1");
                }
                //此处会复用Mesdb，不会实例化新的db
                using (CurrentTenant.Change(Guid.Parse("33333333-3d72-4339-9adc-845151f8ada0")))
                {
                    var otherTenantData1= await repository.GetListAsync();
                    await repository.InsertAsync(new ConfigAggregateRoot
                    {
                        ConfigKey = "Mes2",
                    });
                    var otherTenantData2= await repository.GetListAsync(x=>x.ConfigKey=="Mes2");
                }
                //此处会将多库进行一起提交，前面的操作有报错，全部回滚
                await uow.CompleteAsync();
                return "根据租户切换不同的数据库，并管理db实例连接，涉及多库事务统一到最后提交";
            }
         
        }
    }
}