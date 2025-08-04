using System;

namespace JQCore.tLog
{
    public interface IJQLog
    {
        public void Log(object message);
        public void Log(params object[] objs);
        public void LogFormat(string formatStr, params object[] objs);
        public void LogWarning(object message);
        public void LogWarrningFormat(string formatStr, params object[] objs);
        public void LogErrorSimple(object message);
        public void LogException(Exception e);
        public void LogError(object message);
        public void LogErrorFormat(string formatStr, params object[] objs);
        public void SetErrorReporter(Action<string, string> errorAction);
    }
}