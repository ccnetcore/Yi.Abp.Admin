using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Task
{
    public class TaskGetOutput
    {

        /// <summary>
        /// 作业 Id
        /// </summary>

        public string JobId { get;  set; }

        /// <summary>
        /// 作业组名称
        /// </summary>

        public string GroupName { get;  set; }

        /// <summary>
        /// 作业处理程序类型
        /// </summary>
        /// <remarks>存储的是类型的 FullName</remarks>

        public string JobType { get;  set; }

        /// <summary>
        /// 作业处理程序类型所在程序集
        /// </summary>
        /// <remarks>存储的是程序集 Name</remarks>

        public string AssemblyName { get;  set; }

        /// <summary>
        /// 描述信息
        /// </summary>

        public string Description { get;  set; }

        /// <summary>
        /// 是否采用并行执行
        /// </summary>
        /// <remarks>如果设置为 false，那么使用串行执行</remarks>

        public bool Concurrent { get;  set; } = true;

        /// <summary>
        /// 是否扫描 IJob 实现类 [Trigger] 特性触发器
        /// </summary>

        public bool IncludeAnnotations { get;  set; } = false;

        /// <summary>
        /// 作业信息额外数据
        /// </summary>

        public string Properties { get;  set; } = "{}";

        /// <summary>
        /// 作业更新时间
        /// </summary>

        public DateTime? UpdatedTime { get;  set; }
     

        public string TriggerArgs { get; set; }

        public DateTime? NextRunTime { get; set; }

        public DateTime? LastRunTime { get; set; }

        public long NumberOfRuns { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 触发器类型
        /// </summary>
        public JobTypeEnum Type { get; set; }

        public string? Cron { get; set; }
        public double? Millisecond { get; set; }
    }
}
