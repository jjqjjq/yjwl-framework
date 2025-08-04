using System.Collections.Generic;
using JQCore.tLog;
using UnityEngine;

namespace JQFramework.tTimeCtrl.Clock
{

    public static class TC_Timekeeper
    {
        private static Dictionary<string, TC_GlobalClock> _clockDic = new Dictionary<string, TC_GlobalClock>();


        public static void AddClock(TC_GlobalClock globalClock)
        {
            JQLog.Log($"AddClock:{globalClock.key}");
            _clockDic.Add(globalClock.key, globalClock);
        }

        public static TC_GlobalClock GetClock(string key)
        {
            TC_GlobalClock globalClock = null;
            if(!_clockDic.TryGetValue(key, out globalClock))
            {
                JQLog.LogError($"不存在的GlobalClock:{key}");
            }
            return globalClock;
        }


        // Unscaled delta time does not automatically behave like delta time.
        // Hacky workaround for now, fixing 2 / 3 issues
        // See: http://forum.unity3d.com/threads/138432/#post-2251561
        internal static float unscaledDeltaTime
        {
            get
            {
                if (Time.frameCount <= 2)
                {
                    return 0.02f;
                }
                else
                {
                    return Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);
                }
            }
        }
    }
}
