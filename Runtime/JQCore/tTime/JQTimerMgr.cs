using System;
using System.Collections.Generic;
using JQCore.tLog;

namespace JQCore.tTime
{
    public class JQTimerMgr
    {
        private string _name;
        private List<IrfTimer> _timerList;
        private List<IrfTimer> _tempList;
        private bool _isListChange;

        public JQTimerMgr(string name)
        {
            _name = name;
            _timerList = new List<IrfTimer>();
            _tempList = new List<IrfTimer>();
            _isListChange = true;
        }

        private IrfTimer getTimerByFunc(Action action)
        {
            for (int i = 0; i < _timerList.Count; i++)
            {
                IrfTimer timer = _timerList[i];
                if (timer.action == action)
                {
                    return timer;
                }
            }

            return null;
        }

        private static List<int> _resultIndexList = new List<int>();

        private List<int> getTimersByCaller(Object caller)
        {
            _resultIndexList.Clear();
            for (int i = 0; i < _timerList.Count; i++)
            {
                IrfTimer irfTimer = _timerList[i];
                if (irfTimer.caller == caller)
                {
                    _resultIndexList.Add(i);
                }
            }

            return _resultIndexList;
        }

        public void executeAllTimerByCaller(Object caller, string key = null)
        {
            List<IrfTimer> removeList = new List<IrfTimer>();
            for (int i = 0; i < _timerList.Count; i++)
            {
                IrfTimer irfTimer = _timerList[i];
                if (irfTimer.caller == caller && irfTimer.doOnce && irfTimer.key == key)
                {
                    irfTimer.action();
                    removeList.Add(irfTimer);
                }
            }

            foreach (IrfTimer irfTimer in removeList)
            {
                _timerList.Remove(irfTimer);
            }
            _isListChange = true;
        }

        private void add(float delay, Object caller, Action action, bool doOnce, string key = null)
        {
            if (action == null)
            {
                JQLog.LogError("参数错误！");
                return;
            }

            IrfTimer irfTimer = getTimerByFunc(action);
            if (irfTimer != null)
            {
                irfTimer.nextTime = SysTime.time + delay;
                return;
            }

            irfTimer = new IrfTimer()
            {
                nextTime = SysTime.time + delay,
                caller = caller,
                action = action,
                delay = delay,
                doOnce = doOnce,
                key = key
            };
            _timerList.Add(irfTimer);
            _isListChange = true;
        }

        private bool isExist(Object caller, Action action)
        {
            var timer = getTimerByFunc(action);
            return timer != null;
        }

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="delay">单位秒</param>
        /// <returns></returns>
        public void addOnce(float delay, Object caller, Action action, string key = null)
        {
            add(delay, caller, action, true, key);
        }

        public void addRepeat(float delay, Object caller, Action action, string key = null)
        {
            add(delay, caller, action, false, key);
        }

        public void remove(Action action)
        {
            var timer = getTimerByFunc(action);
            if (timer != null)
            {
                _timerList.Remove(timer);
                timer.Dispose();
                _isListChange = true;
            }
        }

        public void removeByCaller(Object caller)
        {
            List<int> timerIndexList = getTimersByCaller(caller);
            for (int i = timerIndexList.Count - 1; i >= 0; i--)
            {
                int index = timerIndexList[i];
                _timerList.RemoveAt(index);
            }

            _isListChange = true;
        }

        public void clear()
        {
            _timerList.Clear();
            _isListChange = true;
        }

        public void onTick()
        {
            float nowTime = SysTime.time;
            if (_isListChange)
            {
                _tempList.Clear();
                _tempList.AddRange(_timerList);
                _isListChange = false;
            }

            for (int i = 0; i < _tempList.Count; i++)
            {
                IrfTimer irfTimer = _tempList[i];
                if (nowTime > irfTimer.nextTime)
                {
                    if (irfTimer.doOnce)
                    {
                        _timerList.Remove(irfTimer);
                        _isListChange = true;
                    }
                    else
                    {
                        irfTimer.nextTime += irfTimer.delay;
                    }

                    try
                    {
                        irfTimer.action();
                    }
                    catch (Exception e)
                    {
                        JQLog.LogException(e);
                    }
                }
            }
        }

        public class IrfTimer
        {
            public float nextTime;
            public Object caller;
            public Action action;
            public float delay;
            public bool doOnce;
            public string key;

            public void Dispose()
            {
                caller = null;
                action = null;
            }
        }
    }
}