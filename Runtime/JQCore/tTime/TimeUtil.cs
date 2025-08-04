using System;
using System.Text;
using JQCore.tMgr;

/* *******************************************************
 * function: 时间工具类
 * *******************************************************/

namespace JQCore.tTime
{

    public class TimeProp
    {
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
    }


    public class TimeUtil
    {
        static  StringBuilder _stringBuilder = new StringBuilder();
        private const char CHAR_ZERO = '0';
        private const char CHAR_MAOHAO = ':';
        
        public static string secendToTimeStr(int secend, string format)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            DateTime ts = dt.AddSeconds(secend);
            return ts.ToString(format);
        }
        
        public static string TimeStamp()
        {
            DateTime start = new DateTime(1970, 1, 1);
            long a = (DateTime.Now.Ticks - start.Ticks) / 10000000 - 8 * 60 * 60;
            return a.ToString();
        }
        
        /// <summary>
        /// 根据时间戳返回日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTime(long timeStamp)
        {
            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1,9,0,0));
            int zone = ServerTime.Instance.Time_zone;
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0);
            if (zone <= 12)
                dtStart = dtStart.AddHours(zone);
            else
                dtStart = dtStart.AddHours(12 - zone);

            long lTime = timeStamp * 10000000;
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static long GetTimeStamp(int year,int month,int day,int hour,int min,int  sec)
        {
            DateTime dtStart = new DateTime(year, month, day, hour, min, sec);
            TimeSpan ts = dtStart - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return Convert.ToInt32(ts.TotalSeconds);
        }
        public static TimeProp GetRealTime(long timeStame)
        {
            TimeProp timeProp = new TimeProp();
            timeProp.Days = (int)timeStame / 86400;
            timeProp.Hours = (int)(timeStame - timeProp.Days * 86400) / 3600;
            timeProp.Minutes = (int)(timeStame - timeProp.Days * 86400 - timeProp.Hours * 3600) / 60;
            timeProp.Seconds = (int)(timeStame - timeProp.Days * 86400 - timeProp.Hours * 3600 - timeProp.Minutes * 60);
            return timeProp;
        }
        public static int GetYearDays(int year)
        {
            return DateTime.IsLeapYear(year) ? 366 : 365;
        }
        public static int GetCurWeekDay()
        {
            DateTime dt = GetTime(ServerTime.Instance.Timestamp);
            return dt.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dt.DayOfWeek;
        }
        public static int GetDays(long timeStamp)
        {
            DateTime dt = GetTime(timeStamp);
            DateTime cur = GetServerDateTime();
            int days = 1;
            if (cur.Year > dt.Year)
            {
                days += GetYearDays(dt.Year) - dt.DayOfYear;
                for (int i = dt.Year + 1; i < cur.Year; i++)
                {
                    days += GetYearDays(i);
                }
                days += cur.DayOfYear;
            }
            else
            {
                days += cur.DayOfYear - dt.DayOfYear;
            }
            return days;
        }
        /// <summary>
        /// 根据时间戳返回yyyy/MM/dd HH:mm:ss格式的日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static string GetTimeYyyymmddHhmmss(uint timeStamp)
        {
            return string.Format("{0:yyyy/MM/dd HH:mm:ss}", GetTime(timeStamp));
        }
        public static string GetTimeMMDD(uint timeStamp)
        {
            DateTime dt = GetTime(timeStamp);
            return string.Format(LanMgr.GetStr("{0}月{1}日"), dt.Month, dt.Day);
        }
        /// <summary>
        /// 根据时间戳返回yyyy/MM/dd HH:mm格式的日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static string GetTimeYyyymmddHhmm(uint timeStamp)
        {
            return string.Format("{0:yyyy/MM/dd HH:mm}", GetTime(timeStamp));
        }

        public static string GetTimeYyyymmddHhmmss2(uint timeStamp)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", GetTime(timeStamp));
        }

        public static string GetTimeYyyymmddHhmmss3(uint timeStamp)
        {
            string format = "{0:HH:mm:ss}";
            if (GetTime(timeStamp).Day != GetTime(ServerTime.Instance.Timestamp).Day)
                format = "{0:yyyy/MM/dd HH:mm:ss}";

            return string.Format(format, GetTime(timeStamp));
        }

        public static string GetTimeYyyymmddHhmmss4(uint timeStamp)
        {
            return string.Format("{0", GetTime(timeStamp));
        }
        public static string GetTimeYyyymmddHhmmss5(uint timeStamp)
        {
            return string.Format("{0:MM-dd HH:mm:ss}", GetTime(timeStamp));
        }
        public static string GetTimeHhmm(uint timeStamp)
        {
            return string.Format("{0:HH:mm}", GetTime(timeStamp));
        }
        // 		public static string GetTimeHhmmss(int leftTime)
        // 		{
        // 			int hour = leftTime/3600;
        // 			string hourstr = hour < 10 ? "0" + hour : hour + "";
        // 
        // 			int min = (leftTime-hour*3600)/60;
        // 			string minstr = min < 10 ? "0" + min : min + "";
        // 
        // 			int sec = leftTime%60;
        // 			string secstr = sec < 10 ? "0" + sec : sec + "";
        // 
        // 			string result = hourstr + ":" + minstr + ":" + secstr;
        // 
        // 			return result;
        // 		}

        //获取  00:00   00分00秒 格式的时间字符串  leftTime(秒)
        public static string GetTimeMmss(int leftTime)
        {
            int min = leftTime / 60;
            int sec = (leftTime - min * 60);
            return (min < 10 ? "0" + min : min + "") + ":" + (sec < 10 ? "0" + sec : sec + "");
        }
        //获取  00:00:00   00时00分00秒 格式的时间字符串  leftTime(秒)
        public static string GetTimeHhmmss(int leftTime)
        {
            int hour = leftTime / 3600;
            int min = leftTime / 60;
            int sec = (leftTime - min * 60);
            min -= hour * 60;

            if (hour < 10)
            {
                _stringBuilder.Append(CHAR_ZERO).Append(hour);
            }
            else
            {
                _stringBuilder.Append(hour);
            }

            _stringBuilder.Append(CHAR_MAOHAO);
            if (min < 10)
            {
                _stringBuilder.Append(CHAR_ZERO).Append(min);
            }
            else
            {
                _stringBuilder.Append(min);
            }
            _stringBuilder.Append(CHAR_MAOHAO);

            if (sec < 10)
            {
                _stringBuilder.Append(CHAR_ZERO).Append(sec);
            }
            else
            {
                _stringBuilder.Append(sec);
            }

            string str = _stringBuilder.ToString();
            _stringBuilder.Length = 0;
            return str;
        }
        /// <summary>
        ///获取时分秒的数组
        /// </summary>
        /// <param name="leftTime"></param>
        /// <returns></returns>
        public static string[] GetTimeArrageToHMS(int leftTime)
        {
            int hour = leftTime / 3600;
            int min = leftTime / 60;
            int sec = (leftTime - min * 60);
            min -= hour * 60;
            string h = hour < 10 ? "0" + hour : hour.ToString();
            string m = min < 10 ? "0" + min : min.ToString();
            string s = sec < 10 ? "0" + sec : sec.ToString();
            string[] time = { s, m, h };
            return time;// (hour < 10 ? "0" + hour : hour + "") + ":" + (min < 10 ? "0" + min : min + "") + ":" + (sec < 10 ? "0" + sec : sec + "");
        }

        //获取  00:00:00   00时00分00秒 格式的时间字符串  leftTime(秒)
        public static string GetTimeHhmm(int leftTime)
        {
            int hour = leftTime / 3600;
            int min = leftTime / 60;
            min -= hour * 60;
            return (hour < 10 ? "0" + hour : hour + "") + ":" + (min < 10 ? "0" + min : min + "");
        }
        public static string GetTimeHHMMSS(int leftTime)
        {
            int hour = leftTime / 3600;
            int min = leftTime % 3600 / 60;
            int ss = leftTime % 3600 % 60;
            //min -= hour * 60;
            return (hour < 10 ? "0" + hour : hour + "") + ":" + (min < 10 ? "0" + min : min + "") + ":" + (ss < 10 ? "0" + ss : ss + "");
        }

        //获取 0天00时00分00秒
        public static string GetTimeDdHhmmss(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            int sec = leftTime % 60;
            return day + LanMgr.GetStr("天") + (hour < 10 ? "0" + hour : hour + "") +
                LanMgr.GetStr("时") + (min < 10 ? "0" + min : min + "") + "分" +
                (sec < 10 ? "0" + sec : sec + "") + "秒";
        }
        public static string GetTimeStr(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            int sec = leftTime % 60;
            return string.Format("{0}{1}:{2}:{3}", day > 0 ? (day.ToString() + ":") : "", hour > 9 ? hour.ToString() : ("0" + hour), min > 9 ? min.ToString() : ("0" + min), sec > 9 ? sec.ToString() : ("0" + sec));
        }

        public static string GetTimeStr2(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            //int sec = leftTime % 60;
            if (leftTime >= 86400)//一天
            {
                return string.Format("{0}{1}{2}",
                    day > 0 ? (day.ToString() + LanMgr.GetStr("天")) : "",
                    hour > 9 ? hour.ToString() + LanMgr.GetStr("时") : ("0" + hour) + LanMgr.GetStr("时"),
                    min > 9 ? min.ToString() + LanMgr.GetStr("分") : ("0" + min) + LanMgr.GetStr("分"));
            }
            else if (leftTime >= 3600)//一天
            {
                return string.Format("{0}{1}",
                    hour > 9 ? hour.ToString() + LanMgr.GetStr("时") : ("0" + hour) + LanMgr.GetStr("时"),
                    min > 9 ? min.ToString() + LanMgr.GetStr("分") : ("0" + min) + LanMgr.GetStr("分"));
            }
            else
            {
                return string.Format("{0}",
                    min > 9 ? min.ToString() + LanMgr.GetStr("分") : ("0" + min) + LanMgr.GetStr("分"));
            }
        }

        /// <summary>
        /// 天时，时分，分秒
        /// </summary>
        /// <returns></returns>
        public static string GetRemainTime(int leftTime)
        {
            if (leftTime >= 86400)
                return GetTimeDdHh(leftTime);
            else if (leftTime >= 3600)
            {
                int hour = leftTime / 3600;
                int min = (leftTime % 3600) / 60;
                return hour + "时" + min + LanMgr.GetStr("分");
            }
            else
            {
                int min = leftTime / 60;
                int sec = (leftTime % 60);
                return min + "分" + sec + LanMgr.GetStr("秒");
            }
        }
        //获取0天0时
        public static string GetTimeDdHh(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            return day + LanMgr.GetStr("天") + (hour < 10 ? "0" + hour : hour + "") +
               LanMgr.GetStr("时");
        }
        //获取0天
        public static string GetTimeDd(int leftTime)
        {
            int day = leftTime / 86400;
            return day + LanMgr.GetStr("天");
        }
        internal static string GetTimeDdHh(long v)
        {
            throw new NotImplementedException();
        }

        public static string GetTimeHhmmss2(int leftTime)
        {
            int hour = leftTime / 3600;
            int min = (leftTime - hour * 3600) / 60;
            int sec = leftTime % 60;
            return hour + LanMgr.GetStr("时") + (min < 10 ? "0" + min : min + "") + "分" +
                (sec < 10 ? "0" + sec : sec + "") + "秒";
        }
        public static string GetTimeHhmmss3(int leftTime)
        {
            int min = leftTime / 60;
            int sec = leftTime % 60;
            if (min > 0)
            {
                return min + LanMgr.GetStr("分") +
                    (sec < 10 ? "0" + sec : sec + "") + "秒";
            }
            else
            {
                return sec + LanMgr.GetStr("秒");
            }
        }
        //获取 0天00时00分
        public static string GetTimeDdHhmmss2(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            return day + LanMgr.GetStr("天") + (hour < 10 ? "0" + hour : hour + "") +
                LanMgr.GetStr("时") + (min < 10 ? "0" + min : min + "") + "分";
        }

        //获取 0时00分
        public static string GetTimeHhmm2(uint leftTime)
        {
            uint hour = leftTime / 3600;
            uint min = (leftTime - hour * 3600) / 60;
            return (hour > 0 ? hour + LanMgr.GetStr("小时") : "") + min + "分钟";
        }

        //大于一天显示天数，小于一天显示 时：分
        public static string GetTimeDdHhmm(int leftTime)
        {
            int day = leftTime / 86400;
            if (day > 0) return day + LanMgr.GetStr("天");
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            return (hour < 10 ? "0" + hour : hour + "") + ":" + (min < 10 ? "0" + min : min + "");
        }
        public static string GetTimeDdHhmm2(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            return (day + LanMgr.GetStr("天"))
                + (hour < 10 ? "0" + hour : hour + "") + "时";
        }


        public static string GetTimeDdHhmm3(int leftTime)
        {
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            return (day + LanMgr.GetStr("天"))
                + (hour < 10 ? "0" + hour : hour + "") + "时" + ((min < 10 ? "0" + min : min + "") + "分");
        }

        public static string GetTimeYyyymmdd(uint timeStamp)
        {
            return string.Format("{0:yyyy-MM-dd}", GetTime(timeStamp));
        }
        public static string GetTimeYyyymmdd2(uint timeStamp)
        {
            return string.Format("{0", GetTime(timeStamp));
        }
        public static string GetTimeYyyymmdd3(uint timeStamp)
        {
            return string.Format("{0:yyyy/MM/dd}", GetTime(timeStamp));
        }

        //根据时间戳获得距当前已过时间，如：30分钟前
        public static TimeProp GetElapsedTime(uint timeStamp)
        {
            TimeProp timeProp = new TimeProp();

            if (timeStamp > 0)
            {
                DateTime now = DateTime.Now;
                DateTime pre = GetTime(timeStamp);
                TimeSpan diff = now - pre;

                timeProp.Days = diff.Days;
                timeProp.Hours = diff.Hours;
                timeProp.Minutes = diff.Minutes;
                timeProp.Seconds = diff.Seconds;
            }
            else
            {
                timeProp.Days = 0;
                timeProp.Hours = 0;
                timeProp.Minutes = 0;
                timeProp.Seconds = 0;
            }

            return timeProp;
        }

        public static string GetElapesdTimeDesc(uint timeStamp)
        {
            TimeProp timeProp = GetElapsedTime(timeStamp);

            int year = timeProp.Days / 365;
            if (year > 0)
                return string.Format(LanMgr.GetStr("{0}年前"), year);
            else if (timeProp.Days > 0)
                return string.Format(LanMgr.GetStr("{0}天前"), timeProp.Days);
            else if (timeProp.Hours > 0)
                return string.Format(LanMgr.GetStr("{0}小时前"), timeProp.Hours);
            else if (timeProp.Minutes > 0)
                return string.Format(LanMgr.GetStr("{0}分钟前"), timeProp.Minutes);
            else
                return LanMgr.GetStr("刚刚");
        }

        /// <summary>
        /// 返回服务器时间戳
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerDateTime()
        {
            return GetTime(ServerTime.Instance.Timestamp);
        }

        // 转换DateTime为unix时间戳
        public static uint ConvertDateTimeInt(DateTime time)
        {
            uint intResult = 0;
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0);
            int zone = ServerTime.Instance.Time_zone;
            if (zone <= 12)
                startTime = startTime.AddHours(zone);
            else
                startTime = startTime.AddHours(12 - zone);

            intResult = (uint)(time - startTime).TotalSeconds;
            return intResult;
        }
        public static string GetTimemmddHhmm(uint timeStamp)
        {
            return string.Format("{0", GetTime(timeStamp));
        }
        public static string GetTimemmddHhmm2(uint timeStamp)
        {
            return string.Format("{0:MM-dd HH:mm}", GetTime(timeStamp));
        }
        public static int GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt32(ts.TotalSeconds);
        }
        private static string[] perUnit = { "天", "小时", "分", LanMgr.GetStr("秒") };
        public static string GetCountdown(int leftTime)
        {
            if (leftTime <= 0)
            {
                return 0 + LanMgr.GetStr("秒");
            }
            int day = leftTime / 86400;
            int hour = (leftTime - day * 86400) / 3600;
            int min = (leftTime - day * 86400 - hour * 3600) / 60;
            int sec = leftTime % 60;
            int[] times = { day, hour, min, sec };
            string st = "";
            for (int i = 0; i < times.Length; i++)
            {
                int t = times[i];
                if (t == 0)
                {
                    continue;
                }
                string p = perUnit[i];
                st += t + p;
            }
            return st;
        }
    }
}
