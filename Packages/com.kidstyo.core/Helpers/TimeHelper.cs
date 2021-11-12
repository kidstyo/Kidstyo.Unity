using System;
using static System.TimeZone;

namespace Helpers
{
    public static class TimeHelper
    {
        private static readonly DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    
        public static DateTime GetDateTimeByTimeStamp(long timeStamp)
        {
            return CurrentTimeZone.ToLocalTime(StartDate).AddSeconds(timeStamp);
        }
        
        public static long GetNowFormatTime(string format = "yyyyMMddhhmmss")
        {
            return long.Parse(DateTime.Now.ToString(format));
        }
        
        /// <summary>
        /// 当前时间戳（本地）
        /// </summary>
        /// <returns></returns>
        public static long GetNowTimeStamp()
        {
            TimeSpan ts = DateTime.Now - StartDate;
            return Convert.ToInt64(ts.TotalSeconds);
        }
        
        /// <summary>
        /// 当前时间戳（世界）
        /// </summary>
        /// <returns></returns>
        public static long GetUtcNowTimeStamp()
        {
            var ts = DateTime.UtcNow - StartDate;
            return Convert.ToInt64(ts.TotalSeconds);
        }
    
        public static long GetUtcNowTimeStampMs()
        {
            var ts = DateTime.UtcNow - StartDate;
            return Convert.ToInt64(ts.Ticks / 10000);
        }
        
        // dateTime -> 时间戳
        public static long GetTimeStampByDateTime(DateTime dateTime)
        {
            DateTime startTime = CurrentTimeZone.ToLocalTime(StartDate);
            return (long)(dateTime - startTime).TotalSeconds;
        }
        
        /// <summary>
        /// "2020-06-30 00:00:00" -> 时间戳
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        public static long GetTimeStampByTimeStr(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var startTime = CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }
        
        /// <summary>
        /// yyyyMMddhhmmss -> DateTime
        /// </summary>
        /// <param name="formatTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeByFormatTime(long formatTime, string formateType)
        {
            return DateTime.ParseExact(formatTime.ToString(), formateType, System.Globalization.CultureInfo.CurrentCulture);
        }
        
        public static bool IsSameWeek(DateTime lastDt)
        {
            DateTime today = DateTime.Today;
            TimeSpan ts = today - lastDt;
            int intDow = Convert.ToInt32(today.DayOfWeek);
            if (0 == intDow)
            {
                intDow = 7;
            }
            
            double days = ts.TotalDays;
            return !(days > 7) && !(days >= intDow);
        }
    }
}
