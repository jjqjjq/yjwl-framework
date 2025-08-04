/*----------------------------------------------------------------
// 文件名：BuildBase.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/8/31 17:41:25
//----------------------------------------------------------------*/

using System;
using System.Diagnostics;
using JQCore.tLog;

namespace JQEditor.Build
{
    public abstract class BuildBase
    {
        protected Action _endAction;
        protected string _name;
        private readonly string _saveCostKey;
        private readonly string _saveKey;
        private Stopwatch _stopwatch;

        public BuildBase(string name, string saveKey)
        {
            _name = name;
            _saveKey = saveKey;
            _saveCostKey = saveKey + "_cost";
        }

        public string Name => _name;

        public string lastBuildTime => BuildAppInfo.GetStringVal(_saveKey);

        public string lastBuildCostTime => BuildAppInfo.GetStringVal(_saveCostKey);

        public virtual void build(Action endAction)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _endAction = endAction;
            BuildAppInfo.SetStringVal(_saveKey, DateTime.Now.ToString("MM-dd HH:mm"));
        }

        protected void exeEndAction()
        {
            _stopwatch.Stop();
            BuildAppInfo.SetStringVal(_saveCostKey, GetTimeStr(_stopwatch.ElapsedMilliseconds));
            JQLog.Log($"{_name} ===>{_stopwatch.ElapsedMilliseconds}ms");
            if (_endAction != null)
            {
                _endAction();
                _endAction = null;
            }
        }

        //输入毫秒数，输出一个比较容易看懂的时间字符串
        public static string GetTimeStr(long ms)
        {
            var str = "";
            var hour = ms / 3600000;
            var min = (ms - hour * 3600000) / 60000;
            var sec = (ms - hour * 3600000 - min * 60000) / 1000;
            var msec = ms - hour * 3600000 - min * 60000 - sec * 1000;
            if (hour > 0) str += hour + "h";
            if (min > 0) str += min + "m";
            if (sec > 0) str += sec + "s";
            if (msec > 0) str += msec + "ms";
            return str;
        }
    }
}