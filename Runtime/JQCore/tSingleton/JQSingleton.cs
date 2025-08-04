using JQCore.tLog;

namespace JQCore.tSingleton
{
    public class JQSingleton<T> : IJQSingleton where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                var type = typeof(T);
                return (T)JQSingletonMgr.Instance.getSington(type);
            }
        }

        public virtual void Dispose()
        {
            JQLog.LogError($"没有编写Dispose噢！{this}");
        }

        public virtual int GetDisposePriority()
        {
            return 0;
        }
    }
}