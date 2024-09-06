using System;
using System.Text;
using UnityEngine;

namespace Utils
{
    public static class TimeUtils
    {
        internal static class TimeConst
        {
            /// <summary>
            ///1分钟的秒数
            /// </summary>
            public const uint SecondPerMinute = 60;
            /// <summary>
            ///1小时的秒数
            /// </summary>
            public const uint SecondPerHour = 60 * 60;
            /// <summary>
            ///1天的秒数
            /// </summary>
            public const uint SecondPerDay = 60 * 60 * 24;
            /// <summary>
            ///1周的秒数
            /// </summary>
            public const uint SecondPerWeek = 60 * 60 * 24 * 7;

        }

        /// <summary>
        /// 服务器同步过来的时间戳
        /// </summary>
        private static long _serverTimeStamp = 0;
        /// <summary>
        /// 服务器同步过来的时间点,通过Time.realtimeSinceStartup.误差和帧率相关.
        /// </summary>
        private static float _gotRecord = 0f;
        /// <summary>
        /// 与标准时间格林尼治时间的差值
        /// 秒计算
        /// </summary>

        public static int timeZoneDiffInSecs
        {
            get; private set;
        }

        /// <summary>
        /// 同步时间点的0点: DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0)
        /// </summary>
        private static DateTime _zeroDt;

        /// <summary>
        /// 系统时间；
        /// </summary>
        /// <param name="timestamp"></param>
        public static DateTime serverDateTime => TimestampToDateTime(serverTimestamp);

        private static bool _inited = false;
        /// <summary>
        /// 设置服务器时间和时区.如果服务器时区不给,就用本地时区.如果服务器时区和本地时区不一致,处理活动就要注意了.
        /// </summary>
        /// <param name="timestamp"></param>
        public static void SetServerTimeStamp(long value, int zoneDiffInSecs)
        {
            _inited = true;
            _serverTimeStamp = value;
            _gotRecord = AppStatus.realtimeSinceStartup;
            var dt = serverDateTime;
            _zeroDt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            SetTimeZoneDiff(zoneDiffInSecs);
        }

        public static void SetTimeZoneDiff(int zoneDiffInSecs)
        {
            UnityEngine.Debug.Log($"SetTimeZoneDiff:diff={zoneDiffInSecs}.hour:{zoneDiffInSecs / TimeSpan.FromHours(1).TotalSeconds}");
            timeZoneDiffInSecs = zoneDiffInSecs;
        }

        public static float GetRealTimeSinceStartup(long stamp)
        {
            return (stamp - _serverTimeStamp) + _gotRecord;
        }

        public static int GetLeftTime(long endTimeStamp)
        {
            return (int)(endTimeStamp - serverTimestamp);
        }

        /// <summary>
        /// 误差在两帧.
        /// </summary>
        public static long serverTimestamp
        {
            get
            {
                if (_inited)
                {
                    return (long)(AppStatus.realtimeSinceStartup - _gotRecord) + _serverTimeStamp;
                }
                return GetTimestamp(DateTime.Now);
            }
        }


        public static DateTime Get1970() {
            return _date1970;
        }

        //System.TimeZoneInfo.Local
        private static DateTime _date1970 = new System.DateTime(1970, 1, 1,0,0,0,DateTimeKind.Local);
        private static DateTime _date1970Zero = new System.DateTime(1970, 1, 1,0,0,0);
        public static DateTime TimestampToDateTime(long seconds)
        {
            return _date1970.AddSeconds(seconds + timeZoneDiffInSecs);
        }
       
        public static long GetTimestampFromHMS(int h = 0, int m = 0, int s = 0)
        {
            if (h > 24 || h < 0) h = 0;
            if (m > 60 || m < 0) m = 0;
            if (s > 60 || s < 0) s = 0;
            return GetTimestamp(new DateTime(serverDateTime.Year, serverDateTime.Month, serverDateTime.Day, h, m, s));
        }

        /// <summary>
        /// 自零点后经历了多少分钟
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        public static long GetTimeStampFromMinutes(int min)
        {
            var temp =  GetTimestamp(_zeroDt);
            return temp + min * 60 ;
        }

        public static long GetTimestamp(DateTime dataTime)
        {
            TimeSpan ts = dataTime - _date1970;
            return Convert.ToInt64(ts.TotalSeconds)- timeZoneDiffInSecs;
        }
        
        /// <summary>
        /// 获取时间是本周的第几天（1是周一，0是周日）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetDayOfWeek(DateTime dt)
        {
            return (int)dt.DayOfWeek;
        }

        /// <summary>
        /// 根据服务器时间返回当前是星期几
        /// 第几天（1是周一，0是周日）
        /// </summary>
        /// <returns></returns>
        public static int GetDayOfWeekByServerTime()
        {
            return GetDayOfWeek(serverDateTime);
        }
        
        //获取即将到来的最近一次t点的时间戳
        /// <summary>
        /// 获取明天几点的时间戳
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static long GetNextDayTimestamp(int hour)
        {
            //当前日期
            var cdate = TimestampToDateTime(serverTimestamp);
            //今日t点
            var stamp = serverTimestamp - cdate.Hour * 3600 - cdate.Minute * 60 - cdate.Second + hour * 3600;
            if(serverTimestamp >= stamp)
            {
                stamp += 24 * 3600;
            }
            return stamp;
        }

        /// <summary>
        /// beginTS时间的这天的hour点的ts
        /// 如果要计算下一天只需要加上TimeSpan.FromDays(1).TotalSeconds
        /// </summary>
        /// <param name="beginTS"></param>
        /// <returns></returns>
        public static long GetHourOfDayTS(long beginTS, int hour = 0)
        {
            var startDay = TimeUtils.TimestampToDateTime(beginTS);//开始的日期;
            return beginTS - (long)startDay.TimeOfDay.TotalSeconds + (long)TimeSpan.FromHours(hour).TotalSeconds;
        }

        /// <summary>
        /// 当月开始的时间
        /// </summary>
        /// <param name="beginTS"></param>
        /// <returns></returns>
        public static long GetBeginOfMonthTS(long beginTS)
        {
            var date = TimestampToDateTime(beginTS);
            var beginOfMonth = new DateTime(date.Year, date.Month, 1);
            return  beginTS + (long)(beginOfMonth - date).TotalSeconds;
        }

        /// <summary>
        /// 当月结束的时间
        /// </summary>
        /// <param name="beginTS"></param>
        /// <returns></returns>
        public static long GetEndOfMonthTS(long beginTS)
        {
            var date = TimestampToDateTime(beginTS);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime endOfMonth = new DateTime(date.Year, date.Month, daysInMonth);
            return beginTS + (long)(endOfMonth - date).TotalSeconds;
        }

        /// <summary>
        /// 当星期的开始时间
        /// </summary>
        /// <param name="beginTS"></param>
        /// <returns></returns>
        public static long GetWeekBegin(long currentTS)
        {
            var date = TimeUtils.TimestampToDateTime(currentTS);
            var weekBegin = (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
            long startWeekTS = currentTS - (int)date.TimeOfDay.TotalSeconds - (weekBegin - 1) * TimeConst.SecondPerDay;
            return startWeekTS;
        }


        /// <summary>
        /// 根据传入的年月日获取对应的服务器时间戳 2020.3.20
        /// </summary>
        /// <param name="serverTime"></param>
        public static long GetServerTimeFromYMD(int year, int month, int day)
        {
            DateTime dateTime = new System.DateTime(year, month, day,0,0,0);
            TimeSpan ts = dateTime - _date1970Zero;
            return Convert.ToInt64(ts.TotalSeconds + timeZoneDiffInSecs);
        }

        /// <summary>
        /// 把 格林尼治时间戳 转换到 服务器所在时区
        /// </summary>
        /// <param name="seconds">格林尼治时间戳</param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatDateTimeToServerLocal(long seconds, string format = "yyyy-MM-dd hh\\:mm\\:ss")
        {
            var ts = seconds + timeZoneDiffInSecs;
            DateTime dt = _date1970Zero.AddSeconds(ts);
            //Logger.Debug.LogError(string.Format("ts={0} seconds={1} timeZoneDiff={2} seconds", ts, seconds, timeZoneDiff, dt.Millisecond));
            return dt.ToString(format);
        }

        //获取时间戳所在当天零点的时间戳
        //警告!!! timeStamp必须是 格林尼治时间
        //警告!!! timeStamp必须是 格林尼治时间
        //警告!!! timeStamp必须是 格林尼治时间
        public static long GetTodayZeroTimeStamp(int timeStamp)
        {
            //日期
            var cdate = _date1970Zero.AddSeconds(ExToServerLocalTS(timeStamp));
            //零点
            return timeStamp - cdate.Hour * 3600 - cdate.Minute * 60 - cdate.Second ;
        }

        //将服务器TimeStamp转化为格林尼治时间
        //警告!!! timeStamp必须是 服务器本地时区的时间戳
        //警告!!! timeStamp必须是 服务器本地时区的时间戳
        //警告!!! timeStamp必须是 服务器本地时区的时间戳
        public static long ExToGLNZTS(long timeStamp)
        {
            return timeStamp - timeZoneDiffInSecs;
        }

        //将服务器TimeStamp转化为服务器本地时间
        //警告!!! timeStamp必须是 格林尼治时间
        //警告!!! timeStamp必须是 格林尼治时间
        //警告!!! timeStamp必须是 格林尼治时间
        public static long ExToServerLocalTS(long timeStamp)
        {
            return timeStamp + timeZoneDiffInSecs;
        }
    }
}