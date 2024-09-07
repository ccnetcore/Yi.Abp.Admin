using Mapster;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.Dtos.AccessLog;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services
{
    public class AccessLogService : ApplicationService, IAccessLogService
    {
        private readonly ISqlSugarRepository<AccessLogAggregateRoot> _repository;

        public AccessLogService(ISqlSugarRepository<AccessLogAggregateRoot> repository)
        {
            _repository = repository;
        }

        public DateTime GetWeekFirst()
        {
            var week = DateTime.Now.DayOfWeek;
            switch (week)
            {
                case DayOfWeek.Sunday:
                    return DateTime.Now.AddDays(-6).Date;

                case DayOfWeek.Monday:
                    return DateTime.Now.AddDays(-0).Date;

                case DayOfWeek.Tuesday:
                    return DateTime.Now.AddDays(-1).Date;

                case DayOfWeek.Wednesday:
                    return DateTime.Now.AddDays(-2).Date;

                case DayOfWeek.Thursday:
                    return DateTime.Now.AddDays(-3).Date;

                case DayOfWeek.Friday:
                    return DateTime.Now.AddDays(-4).Date;

                case DayOfWeek.Saturday:
                    return DateTime.Now.AddDays(-5).Date;

                default:
                    throw new ArgumentException("日期错误");
            }
        }


        /// <summary>
        /// 获取全部访问流量(3个月)
        /// </summary>
        /// <param name="AccessLogType"></param>
        /// <returns></returns>
        public async Task<List<AccessLogDto>> GetListAsync([FromQuery] AccessLogTypeEnum accessLogType)
        {
            var entities = await _repository._DbQueryable.Where(x => x.AccessLogType == accessLogType)
                .Where(x => x.CreationTime >= DateTime.Now.AddMonths(-3))
                .OrderBy(x => x.CreationTime).ToListAsync();
            var output = entities.Adapt<List<AccessLogDto>>();
            output?.ForEach(x => x.CreationTime = x.CreationTime.Date);
            return output;
        }

        /// <summary>
        /// 首页点击触发
        /// </summary>
        /// <returns></returns>
        [HttpPost("access-log")]
        public async Task AccessAsync()
        {
            //可判断http重复，防止同一ip多次访问
            var last = await _repository._DbQueryable.Where(x=>x.AccessLogType==AccessLogTypeEnum.HomeClick).OrderByDescending(x => x.CreationTime).FirstAsync();

            if (last is null || last.CreationTime.Date != DateTime.Today)
            {
                await _repository.InsertAsync(new AccessLogAggregateRoot(){AccessLogType=AccessLogTypeEnum.HomeClick});
            }
            else
            {
                await _repository._Db.Updateable<AccessLogAggregateRoot>().SetColumns(it => it.Number == it.Number + 1)
                    .Where(it => it.Id == last.Id).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 获取当前周首页点击数据
        /// </summary>
        /// <returns></returns>
        public async Task<AccessLogDto[]> GetWeekAsync([FromQuery] AccessLogTypeEnum accessLogType)
        {
            var lastSeven = await _repository._DbQueryable
                .Where(x => x.AccessLogType == accessLogType)
                .OrderByDescending(x => x.CreationTime).ToPageListAsync(1, 7);

            return WeekTimeHandler(lastSeven.ToArray());
        }

        /// <summary>
        /// Todo: 可放入领域层
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private AccessLogDto[] WeekTimeHandler(AccessLogAggregateRoot[] data)
        {
            data = data.Where(x => x.CreationTime >= GetWeekFirst()).OrderByDescending(x => x.CreationTime)
                .DistinctBy(x => x.CreationTime.DayOfWeek).ToArray();

            Dictionary<DayOfWeek, AccessLogDto> processedData = new Dictionary<DayOfWeek, AccessLogDto>();

            // 初始化字典，将每天的数据都设为0
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                processedData.Add(dayOfWeek, new AccessLogDto());
            }


            // 处理原始数据
            foreach (var item in data)
            {
                DayOfWeek dayOfWeek = item.CreationTime.DayOfWeek;
                // 如果当天有数据，则更新字典中的值为对应的Number
                var sss = data.Adapt<AccessLogDto>();
                processedData[dayOfWeek] = item.Adapt<AccessLogDto>();
            }

            var result = processedData.Values.ToList();

            //此时的时间是周日-周一-周二，需要处理
            var first = result[0]; // 获取第一个元素
            result.RemoveAt(0); // 移除第一个元素
            result.Add(first); // 将第一个元素添加到末尾

            return result.ToArray();
        }
    }
}