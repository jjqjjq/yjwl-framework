using JQCore.tLog;
using JQCore.tTime;
#if PERFORMANCE_LOG
using System.Diagnostics;
#endif

namespace JQCore.tLoader
{
    public class BaseLoader
    {
        public delegate void OnComplete(BaseLoader baseLoader);

        public delegate void OnProcessUpdate();

        protected string _name;
        protected int _remain; //剩余
        private float _startTime;
        protected int _total; //总数
        public OnComplete onComplete;
        public OnProcessUpdate onProcessUpdate;
#if PERFORMANCE_LOG
        Stopwatch _stopwatch = new Stopwatch();
#endif


        public virtual void dispose()
        {
#if PERFORMANCE_LOG
            _stopwatch = null;
#endif
            _name = null;
            onComplete = null;
            onProcessUpdate = null;
        }

        public BaseLoader(string name)
        {
            _name = name;
        }

        public int Remain => _remain;

        public int Total => _total;

        protected void initTotal(int total)
        {
            _total = total;
            _remain = total;
        }

        protected void checkRemain()
        {
            if (_remain == 0) finishAll();
        }

        protected void finishOne()
        {
            _remain--;
            JQLog.LogWarning($"[exe process]{Name}  {_total - _remain}/{_total}");
            if (onProcessUpdate != null) onProcessUpdate();
        }

        protected void finishAll()
        {
            _remain = 0;
#if PERFORMANCE_LOG
            _stopwatch.Stop();
            //ManagedHeapUtil.printMemory();
            JQLog.LogWarning($"---------------------------[exe finish]{Name} 耗时：{_stopwatch.ElapsedMilliseconds}ms ");
#else
            JQLog.LogWarning($"---------------------------[exe finish]{Name} ");
#endif

            if (onComplete != null) onComplete(this);
        }
        
        public string Name
        {
            get { return _name; }
        }

        public virtual void start()
        {
            JQLog.LogWarning($"[exe start]{Name}  total:{_total}");
#if PERFORMANCE_LOG
            _stopwatch.Reset();
            _stopwatch.Start();
#endif
        }


        public virtual void finalEnd()
        {
        }

        public virtual void ready()
        {
#if PERFORMANCE_LOG
            JQLog.LogFormat("[准备]{0}", _name);
            _stopwatch.Reset();
            _stopwatch.Start();
#endif
        }

        public void readyEnd()
        {
#if PERFORMANCE_LOG
            _stopwatch.Stop();
            JQLog.LogFormat("---------------------------[准备完毕]{0} time:{1}ms frame:{2}", _name,
                _stopwatch.ElapsedMilliseconds, SysTime.frameCount);
#endif
        }
    }
}