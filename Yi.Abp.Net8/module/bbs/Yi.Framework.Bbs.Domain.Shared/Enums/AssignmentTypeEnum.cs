namespace Yi.Framework.Bbs.Domain.Shared.Enums;

public enum AssignmentTypeEnum
{
    /// <summary>
    /// 新手任务
    /// </summary>
    Novice,

    /// <summary>
    /// 每日任务
    /// </summary>
    Daily,

    /// <summary>
    /// 每周任务
    /// </summary>
    Weekly
}

public static class AssignmentTypeExtension
{
    public static DateTime? GetExpireTime(this AssignmentTypeEnum assignmentType)
    {
        switch (assignmentType)
        {
            case AssignmentTypeEnum.Novice:
                return null;
            case AssignmentTypeEnum.Daily:
                return DateTime.Now.Date.AddDays(1);
            case AssignmentTypeEnum.Weekly:
                DateTime currentDate = DateTime.Now; // 获取当前日期和时间
                // 计算今天是周几
                int daysUntilNextMonday = ((int)DayOfWeek.Monday - (int)currentDate.DayOfWeek + 7) % 7;
                // 如果今天是周一，则获取下下周一
                if (daysUntilNextMonday == 0)
                {
                    daysUntilNextMonday = 7;
                }
                // 计算下个周一的日期
                DateTime nextMonday = currentDate.AddDays(daysUntilNextMonday);
                // 返回下个周一的凌晨 0 点时间
                return nextMonday.Date;
            default:
                throw new ArgumentOutOfRangeException(nameof(assignmentType), assignmentType, null);
        }
    }
    
    public static bool IsExpire(this AssignmentTypeEnum assignmentType,DateTime time)
    {
        switch (assignmentType)
        {
            case AssignmentTypeEnum.Novice:
                return false;
            case AssignmentTypeEnum.Daily:
                //昨天之前发的，算过期
                return time.Date < DateTime.Now.Date;
            case AssignmentTypeEnum.Weekly:
                // 获取当前日期
                DateTime now = DateTime.Now;
                // 计算本周一的日期
                int daysToSubtract = (int)now.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysToSubtract < 0) daysToSubtract += 7; // 如果今天是周日，则需要调整
                DateTime startOfWeek = now.AddDays(-daysToSubtract).Date;
                // 获取本周一的凌晨 00:00
                DateTime mondayMidnight = startOfWeek; // .Date 默认为 00:00
                //本周一之前发的
                return  time<mondayMidnight ;
            default:
                throw new ArgumentOutOfRangeException(nameof(assignmentType), assignmentType, null);
        }
    }
}