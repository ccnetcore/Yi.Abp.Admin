using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using Xunit;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Abp.Test.Demo
{
    public class ThreadDb_Test : YiAbpTestBase
    {
        /// <summary>
        /// 并发
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Repository_Test()
        {
            try
            {
                var rep = GetRequiredService<ISqlSugarRepository<UserAggregateRoot>>();
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await rep.GetListAsync();
                    }));
                }
                await Task.WhenAll(tasks);
                await Console.Out.WriteLineAsync("成功");
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 工作单元
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Uow_In_Test()
        {
            try
            {
                var rep = GetRequiredService<ISqlSugarRepository<UserAggregateRoot>>();
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        using (var uow = GetRequiredService<IUnitOfWorkManager>().Begin(true, true))
                        {
                            await rep.GetListAsync();
                            await uow.CompleteAsync();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
                await Console.Out.WriteLineAsync("成功");
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// 工作单元
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Uow_Out_Test()
        {
            try
            {
                var rep = GetRequiredService<ISqlSugarRepository<UserAggregateRoot>>();
                List<Task> tasks = new List<Task>();
                using (var uow = GetRequiredService<IUnitOfWorkManager>().Begin(true, true))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            await rep.GetListAsync();
                            await uow.CompleteAsync();
                        }));
                    }
                }
                await Task.WhenAll(tasks);
                await Console.Out.WriteLineAsync("成功");
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
