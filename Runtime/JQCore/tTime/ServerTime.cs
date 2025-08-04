
using System;
using JQCore;
using UnityEngine;

/* *******************************************************
 * function: 系统时间
 * *******************************************************/

namespace JQCore.tTime
{

    public class ServerTime
    {
        private static ServerTime _instance;
        private int _timestamp; //服务器本地时间的时间戳,单位到秒
        private static MilliSecond milliSecond;//服务器时间，毫秒
        public long Tick1970 = new DateTime(1970, 1, 1, 0, 0, 0).Ticks;
        public byte Time_zone = 0; /// 时区
        public void SetTick1970(byte zone)
        {
            Time_zone = zone;
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Time_zone <= 12)
                dtStart = dtStart.AddHours(Time_zone);
            else
                dtStart = dtStart.AddHours(12 - Time_zone);
            Tick1970 = dtStart.Ticks;
        }
        static public int OneDaySecond = 86400;
        private DateTime curDt;
        public int Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                Sys.sysTimerMgr.remove(UpdateServerTime);
                Sys.sysTimerMgr.addRepeat(1, this, UpdateServerTime);
                curDt = TimeUtil.GetServerDateTime();
            }
        }
        private void UpdateServerTime()
        {
            _timestamp += 1;
            if (secondCall != null) secondCall();
            if (dayChange != null)
            {
                DateTime tempDay = TimeUtil.GetServerDateTime();
                if (tempDay.Day != curDt.Day)
                {
                    curDt = tempDay;
                    dayChange();
                }
            }
        }
        public delegate void SecondCall();
        static public event SecondCall secondCall;
        public delegate void DayChange();
        static public event DayChange dayChange;
        public static void SetMilliSecond(long time)
        {
            milliSecond.AdjustTime(time);
        }

        public static void Update()
        {
            milliSecond.Update();
        }
        /// <summary>
        /// 与服务器保持毫秒级别的同步时间
        /// </summary>
        /// <returns></returns>
        public static long GetMilliSecond()
        {
            return milliSecond.time;
        }

        public static ServerTime Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerTime();
                }
                return _instance;
            }
            set { _instance = value; }
        }
    }
}

struct MilliSecond
{
    public long time;
    public long recordTime;
    public float realtimeSinceStartup;
    public bool isInited;
    public const int INTERVAL = 5;//每次纠正最大5毫秒
    public int count;
    public int offsetTimePerFame;

    public void AdjustTime(long serverTime)
    {
        if (!isInited)
        {
            time = serverTime;
            recordTime = serverTime;
            realtimeSinceStartup = Time.realtimeSinceStartup;
            isInited = true;
        }
        else if (serverTime != time)
        {
            int offsetTime = (int)(serverTime - time);
            count = Mathf.Abs(offsetTime / INTERVAL);
            if (count == 0)
            {
                offsetTimePerFame = offsetTime;
                count = 1;
            }
            else
            {
                offsetTimePerFame = offsetTime / count;
            }
        }
        else
        {
            count = 0;
        }
    }

    public int GetOffsetTime()
    {
        if (count > 0)
        {
            count--;
            return offsetTimePerFame;
        }
        return 0;
    }

    public void Update()
    {
        time = recordTime + (long)((Time.realtimeSinceStartup - realtimeSinceStartup) * 1000) + GetOffsetTime();
    }
}
