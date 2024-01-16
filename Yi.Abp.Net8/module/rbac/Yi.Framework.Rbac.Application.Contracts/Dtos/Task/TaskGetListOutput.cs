namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Task
{
    public class TaskGetListOutput
    {
        /// <summary>
        /// 作业 Id
        /// </summary>

        public string JobId { get; internal set; }

        /// <summary>
        /// 作业组名称
        /// </summary>

        public string GroupName { get; internal set; }

        /// <summary>
        /// 作业处理程序类型
        /// </summary>
        /// <remarks>存储的是类型的 FullName</remarks>

        public string JobType { get; internal set; }

        /// <summary>
        /// 作业处理程序类型所在程序集
        /// </summary>
        /// <remarks>存储的是程序集 Name</remarks>

        public string AssemblyName { get; internal set; }

        /// <summary>
        /// 描述信息
        /// </summary>

        public string Description { get; internal set; }

        /// <summary>
        /// 是否采用并行执行
        /// </summary>
        /// <remarks>如果设置为 false，那么使用串行执行</remarks>

        public bool Concurrent { get; internal set; } = true;

        /// <summary>
        /// 是否扫描 IJob 实现类 [Trigger] 特性触发器
        /// </summary>

        public bool IncludeAnnotations { get; internal set; } = false;

        /// <summary>
        /// 作业信息额外数据
        /// </summary>

        public string Properties { get; internal set; } = "{}";

        /// <summary>
        /// 作业更新时间
        /// </summary>

        public DateTime? LastRunTime { get; internal set; }

        /// <summary>
        /// 标记其他作业正在执行
        /// </summary>
        /// <remarks>当 <see cref="Concurrent"/> 为 false 时有效，也就是串行执行</remarks>
        internal bool Blocked { get; set; } = false;

        /// <summary>
        /// 作业处理程序运行时类型
        /// </summary>
        internal string RuntimeJobType { get; set; }

        /// <summary>
        /// 作业信息额外数据运行时实例
        /// </summary>
        internal string RuntimeProperties { get; set; }


        /// <summary>
        /// 触发器参数
        /// </summary>
        public string TriggerArgs { get; set; }

        //状态
        public string Status { get; set; }
    }
}
