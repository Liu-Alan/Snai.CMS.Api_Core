namespace Snai.CMS.Api_Core.Entities.Settings
{
    public class LogonSettings
    {
        //错误次数
        public int ErrorCount { get; set; }

        //错误时间（分钟）
        public int ErrorTime { get; set; }

        //锁定时间（分钟）
        public int LockMinute { get; set; }
    }
}
