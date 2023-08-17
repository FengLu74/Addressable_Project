using System;
using System.Globalization;
using UnityEngine;

namespace Kit
{
    public static class TimeUtils
	{
	    public const long TicksPerMs = 10000;        // 每毫秒的 ticks 数
        /// <summary>
	    /// 获取客户端当前时间(毫秒), 0001 年开始
	    /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static long GetClientTimeMs() => DateTime.Now.Ticks / TicksPerMs;

        #region 服务器时间
        private static DateTime _serverBaseTime = new DateTime(1970, 1, 1);
        private static int _serverTimezone;                // 服务器时区GetServerTicks
        private static long _serverTime; // 第一次上线时间

        public static void SetStartServerTime(long time1,int zone)
	    {
	        //Log.LogWarning("SetStartServerTime  " + time1 + "  " + zone);
	        _serverTime = time1;
	        AppStartTime = DateTime.Now;
			_serverTimezone = zone;
            _serverBaseTime = new DateTime(1970, 1, 1, zone, 0, 0);
	    }

        private static DateTime AppStartTime { get; set; }

        // ReSharper disable once UnusedMember.Global
        public static long GetTick(long ms) => _serverBaseTime.Ticks + ms * TicksPerMs;

        //获取时区
        // ReSharper disable once UnusedMember.Global
        public static int Timezone() => _serverTimezone;


        /// <summary> 获取当前服务器时间 </summary>
	    public static DateTime GetServerCurTime()
	    {
            var startTime = _serverBaseTime;
	        var dt = startTime.AddSeconds(_serverTime);
	        dt = new DateTime( dt.Ticks + (DateTime.Now.Ticks - AppStartTime.Ticks));
	        return dt;
	    }

        // ReSharper disable once UnusedMember.Global
        public static DateTime GetLockTime() => AppStartTime;

        public static long GetServerTimestamp() => _serverTime + (DateTime.Now.Ticks - AppStartTime.Ticks) / TicksPerMs / 1000;

        //服务器时间距离 时间多久
        // ReSharper disable once UnusedMember.Global
        public static double GetServerToTime(string dtTime)
	    {
	        var endDateTime = Convert.ToDateTime(dtTime);
	        var serverTime = GetServerCurTime();

	        var nowTimespan = new TimeSpan(serverTime.Ticks);
	        var endTimespan = new TimeSpan(endDateTime.Ticks);
	        var timespan = nowTimespan.Subtract(endTimespan).Duration();
	        return timespan.TotalSeconds;
	    }

	    //时间是否大于等于服务器时间
        // ReSharper disable once UnusedMember.Global
        public static bool CompareServerToTime(string dtTime)
	    {
	        var endDateTime = Convert.ToDateTime(dtTime);
	        var serverTime = GetServerCurTime();
	        return 0 <= DateTime.Compare(endDateTime , serverTime);
        }

	    //是否相同一天
        // ReSharper disable once UnusedMember.Global
        public static bool IsInServerCurDay(string dtTime)
	    {
	        var endDateTime = Convert.ToDateTime(dtTime);
	        var serverTime = GetServerCurTime();
	        return serverTime.Day == endDateTime.Day;
        }

        /// <summary>
		/// 取得某月的第一天
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public static DateTime FirstDayOfMonth(DateTime datetime) => datetime.AddDays(1 - datetime.Day);

		/// 取得某月的最后一天
        // ReSharper disable once UnusedMember.Global
        public static DateTime LastDayOfMonth(DateTime datetime) => datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);

        // ReSharper disable once UnusedMember.Global
        public static DateTime FirstDayOfNextMonth(DateTime dateTime) =>
            dateTime.AddDays(1 - dateTime.Day).AddMonths(1);

		/// 取得上个月第一天
        // ReSharper disable once UnusedMember.Global
		public static DateTime FirstDayOfPreviousMonth(DateTime datetime) => datetime.AddDays(1 - datetime.Day).AddMonths(-1);

		/// 取得上个月的最后一天
        // ReSharper disable once UnusedMember.Global
		public static DateTime LastDayOfPreviousMonth(DateTime datetime) => datetime.AddDays(1 - datetime.Day).AddDays(-1);

        // ReSharper disable once UnusedMember.Global
	    public static float GetTimeTime() => Time.time;

        // ReSharper disable once UnusedMember.Global
        public static long ConvertToTimestamp(DateTime dateTime, int timeZone)
        {
            var startTime = Get1970Time(timeZone);
            return Convert.ToInt64((dateTime - startTime).TotalSeconds);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static DateTime Get1970Time(int timeZone)
        {
            var localTime = new DateTime(1970, 1, 1, 0, 0, 0);
            localTime = localTime.AddHours(timeZone);
            return localTime;
        }

        // ReSharper disable once UnusedMember.Global
        public static DateTime ToDateTime(ulong timestamp) => Get1970Time(_serverTimezone).AddSeconds(timestamp);

        public static int GetDayOfWeek(DateTime dt) => (int)dt.DayOfWeek;

        public static int GetDayOfWeekByTimestamp(ulong timestamp) {
            var dt = ToDateTime(timestamp);
            return GetDayOfWeek(dt);
        }

        #endregion


	    #region 格式化, Parse & Format

	    // 解析 "2012-12-31 12:00:00" 格式
        // ReSharper disable once UnusedMember.Global
        public static DateTime Parse_DateTime(string str) => DateTime.Parse(str);

        //解析 "20210101" 格式 不允许小于19700101, 不允许大于当前服务器时间
        // ReSharper disable once UnusedMember.Global
        public static bool TryParseExact_DateTime(string str) {
            // ReSharper disable once RedundantNameQualifier
            var result = DateTime.TryParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None, out var date);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (DateTime.Compare(date, Get1970Time(_serverTimezone)) < 0) {
                return false;
            }
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (DateTime.Compare(GetServerCurTime(), date) < 0) {
                return false;
            }
            return result;
        }
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once RedundantNameQualifier
        public static DateTime ParseExact_DateTime(string str) => DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

        // 返回 "2012-12-31" 格式
        // ReSharper disable once UnusedMember.Global
        public static string Format_Date(DateTime dt) => $"{dt:yyyy-MM-dd}";

        public static string Format_Date2(DateTime dt) => $"{dt:yyyy.MM.dd}";

        // 返回 "2012-12-31 12:00:00" 格式
        // ReSharper disable once UnusedMember.Global
        public static string Format_DateTime(DateTime dt) => $"{dt:yyyy-MM-dd HH:mm:ss}";

        // 返回12:00:00格式
        // ReSharper disable once UnusedMember.Global
        public static string Format_DateHour(DateTime dt) => $"{dt:HH:mm:ss}";

#endregion

	}
}
