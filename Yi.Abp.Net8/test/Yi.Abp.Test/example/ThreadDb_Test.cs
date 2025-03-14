using Volo.Abp.Uow;
using Xunit;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Abp.Test.example
{
    public class ThreadDb_Test : YiAbpTestBase
    {
        /// <summary>
        /// 工作单元
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Uow_In_Test()
        {
            try
            {
                var uowManager = GetRequiredService<IUnitOfWorkManager>();
                var tasks = new List<Task>();
                // 创建10个任务但不立即执行
                for (int i = 0; i < 10; i++)
                {
                    var task = new Task(async () =>
                    {
                        using (var uow = uowManager.Begin())
                        {
                            var rep = GetRequiredService<ISqlSugarRepository<UserAggregateRoot>>();
                            var result = await rep.GetListAsync();
                            await uow.CompleteAsync();
                        }
                    });
                    tasks.Add(task);
                }

                // 同时启动所有任务
                foreach (var task in tasks)
                {
                    task.Start();
                }

                await Task.WhenAll(tasks);
                // 如果执行到这里没有抛出异常,说明并发测试成功
                Assert.True(true, "并发工作单元测试成功");
            }
            catch (Exception ex)
            {
                Assert.True(false, $"并发工作单元测试失败: {ex.Message}");
            }
        }
    }
}
