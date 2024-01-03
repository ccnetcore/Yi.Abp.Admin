using Volo.Abp.Application.Services;
using Yi.Framework.Rbac.Application.Contracts.IServices;

namespace Yi.Framework.Rbac.Application.Services
{
    public class TaskService : ApplicationService, ITaskService
    {
        //private readonly ISchedulerFactory _schedulerFactory;
        //public TaskService(ISchedulerFactory schedulerFactory)
        //{
        //    _schedulerFactory = schedulerFactory;
        //}
        ///// <summary>
        ///// 单查job
        ///// </summary>
        ///// <param name="jobId"></param>
        ///// <returns></returns>
        //[HttpGet("{jobId}")]
        //public TaskGetOutput GetById([FromRoute] string jobId)
        //{
        //    var result = _schedulerFactory.TryGetJob(jobId, out var scheduler);
        //    var data = scheduler.GetModel();
        //    var output = data.JobDetail.Adapt<TaskGetOutput>();
        //    output.TriggerArgs = data.Triggers[0].Args;
        //    output.NextRunTime = data.Triggers[0].NextRunTime;
        //    output.LastRunTime = data.Triggers[0].LastRunTime;
        //    output.NumberOfRuns = data.Triggers[0].NumberOfRuns;
        //    return output;
        //}

        ///// <summary>
        ///// 多查job
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("")]
        //public PagedResultDto<TaskGetListOutput> GetList([FromQuery] TaskGetListInput input)
        //{
        //    var data = _schedulerFactory.GetJobsOfModels().Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize).OrderByDescending(x => x.JobDetail.UpdatedTime)

        //        .ToList();
        //    var output = data.Select(x =>
        //    {

        //        var res = new TaskGetListOutput();
        //        res = x.JobDetail.Adapt<TaskGetListOutput>();
        //        res.TriggerArgs = x.Triggers[0].Args;
        //        res.Status = x.Triggers[0].Status.ToString();
        //        return res;
        //    }).ToList();
        //    return new PagedResultDto<TaskGetListOutput>(data.Count(), output);
        //}

        ///// <summary>
        ///// 创建job
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public ScheduleResult Create(TaskCreateInput input)
        //{


        //    //jobBuilder
        //    var jobBuilder = JobBuilder.Create(input.AssemblyName, input.JobType).SetJobId(input.JobId).SetGroupName(input.GroupName)
        //    .SetConcurrent(input.Concurrent).SetDescription(input.Description);

        //    //triggerBuilder
        //    //毫秒
        //    TriggerBuilder triggerBuilder = null;
        //    switch (input.Type)
        //    {
        //        case Core.Rbac.Enums.JobTypeEnum.Cron:
        //            triggerBuilder = Triggers.Cron(input.Cron, CronStringFormat.WithSeconds);
        //            break;
        //        case Core.Rbac.Enums.JobTypeEnum.Millisecond:
        //            triggerBuilder = Triggers.Period(input.Millisecond);
        //            break;
        //    }

        //    //作业计划,单个jobBuilder与多个triggerBuilder组合
        //    var schedulerBuilder = SchedulerBuilder.Create(jobBuilder, triggerBuilder);


        //    //调度中心工厂，使用作业计划管理job,返回调度中心单个
        //    var result = _schedulerFactory.TryAddJob(schedulerBuilder, out var scheduler);

        //    return result;
        //}

        ///// <summary>
        ///// 移除job
        ///// </summary>
        ///// <param name="jobId"></param>
        ///// <returns></returns>
        //public ScheduleResult Remove(string jobId)
        //{
        //    var res = _schedulerFactory.TryRemoveJob(jobId, out var scheduler);
        //    return res;
        //}

        ///// <summary>
        ///// 暂停job
        ///// </summary>
        ///// <param name="jobId"></param>
        ///// <returns></returns>
        //[HttpPut]
        //public ScheduleResult Pause(string jobId)
        //{
        //    var res = _schedulerFactory.TryGetJob(jobId, out var scheduler);

        //    scheduler.Pause();
        //    return res;
        //}

        ///// <summary>
        ///// 开始job
        ///// </summary>
        ///// <param name="jobId"></param>
        ///// <returns></returns>
        //[HttpPut]
        //public ScheduleResult Start(string jobId)
        //{
        //    var res = _schedulerFactory.TryGetJob(jobId, out var scheduler);
        //    scheduler.Start();
        //    return res;
        //}

        ///// <summary>
        ///// 更新job
        ///// </summary>
        ///// <param name="jobId"></param>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public ScheduleResult Update(string jobId, TaskUpdateInput input)
        //{
        //    //jobBuilder
        //    var jobBuilder = JobBuilder.Create(input.AssemblyName, input.JobType).SetJobId(jobId).SetGroupName(input.GroupName)
        //        .SetConcurrent(input.Concurrent).SetDescription(input.Description);

        //    //triggerBuilder
        //    //毫秒
        //    TriggerBuilder triggerBuilder = null;
        //    switch (input.Type)
        //    {
        //        case Core.Rbac.Enums.JobTypeEnum.Cron:
        //            triggerBuilder = Triggers.Cron(input.Cron, CronStringFormat.WithSeconds);
        //            break;
        //        case Core.Rbac.Enums.JobTypeEnum.Millisecond:
        //            triggerBuilder = Triggers.Period(input.Millisecond);
        //            break;
        //    }

        //    //作业计划,单个jobBuilder与多个triggerBuilder组合
        //    var schedulerBuilder = SchedulerBuilder.Create(jobBuilder, triggerBuilder);


        //    var result = _schedulerFactory.TryUpdateJob(schedulerBuilder, out var scheduler);
        //    return result;
        //}

        //[HttpPost]
        //public bool RunOnce(string jobId)
        //{
        //    var result = _schedulerFactory.TryGetJob(jobId, out var scheduler);

        //    var triggerBuilder = Triggers.Period(100).SetRunOnStart(true).SetMaxNumberOfRuns(1);
        //    scheduler.AddTrigger(triggerBuilder);
        //    //设置启动时执行一次，然后最大只执行一次
        //    return true;
        //}
    }
}
