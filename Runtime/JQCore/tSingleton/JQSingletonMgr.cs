using System;
using System.Collections.Generic;

namespace JQCore.tSingleton
{
    public class JQSingletonMgr
    {
        private static JQSingletonMgr instance;

        private readonly Dictionary<Type, IJQSingleton> _singletons = new();

        public static JQSingletonMgr Instance
        {
            get
            {
                if (instance == null) instance = new JQSingletonMgr();

                return instance;
            }
        }

        public IJQSingleton getSington(Type type)
        {
            IJQSingleton t = null;
            if (!_singletons.TryGetValue(type, out t))
            {
                t = (IJQSingleton)Activator.CreateInstance(type);
                _singletons.Add(type, t);
            }

            return t;
        }

        public void ClearAllInstance()
        {
            List<IJQSingleton> list = new List<IJQSingleton>(_singletons.Values);
            list.Sort((a, b) => a.GetDisposePriority() - b.GetDisposePriority());
            foreach (var singleton in list) singleton.Dispose();
            _singletons.Clear();
        }
    }
}