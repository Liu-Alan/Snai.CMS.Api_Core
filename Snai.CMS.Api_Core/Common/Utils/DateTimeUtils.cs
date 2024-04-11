﻿namespace Snai.CMS.Api_Core.Common.Utils
{
    public class DateTimeUtils
    {
        /// <summary>
        /// 将时间转换成unix时间戳
        /// </summary>
        /// <param name="time">本地时间</param>
        /// <returns>返回单位秒</returns>
        public static long DateTimeToUnixTimeStamp(DateTime time)
        {
            DateTimeOffset dto = new DateTimeOffset(time);
            return dto.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 将unix时间戳转换成时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(long timeStamp)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            return dto.ToLocalTime().DateTime;
        }
    }
}
