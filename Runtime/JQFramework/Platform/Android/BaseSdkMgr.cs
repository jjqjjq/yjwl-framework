using System.Collections.Generic;

namespace JQFramework.Platform
{
    public class BaseSdkMgr
    {
        protected Dictionary<string, object> _paramDic = new Dictionary<string, object>();

        public void SetParam(string str, object obj)
        {
            _paramDic[str] = obj;
        }
    }
}