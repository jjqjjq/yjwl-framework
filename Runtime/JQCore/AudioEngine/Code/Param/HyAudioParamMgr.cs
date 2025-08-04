using System.Collections.Generic;
using JQCore.tSingleton;

namespace framework.AudioEngine.Code.Param
{
    public class HyAudioParamMgr : JQSingleton<HyAudioParamMgr>
    {
        public void ClearAllParams(string eventName)
        {
            _paramFloatDic.Remove(eventName);
            _paramIntDic.Remove(eventName);
            _paramBoolDic.Remove(eventName);
        }

        public override void Dispose()
        {
            _paramFloatDic.Clear();
            _paramIntDic.Clear();
            _paramBoolDic.Clear();
        }

        #region float

        Dictionary<string, Dictionary<string, float>> _paramFloatDic = new Dictionary<string, Dictionary<string, float>>();

        private Dictionary<string, float> GetParamDicFloat(string eventName)
        {
            bool success = _paramFloatDic.TryGetValue(eventName, out Dictionary<string, float> dic);
            if (success)
            {
                return dic;
            }
            dic = new Dictionary<string, float>();
            _paramFloatDic.Add(eventName, dic);
            return dic;
        }
        
        public void SetParamFloat(string eventName, string paramKey, float paramValue)
        {
            Dictionary<string, float> dic = GetParamDicFloat(eventName);
            dic[paramKey] = paramValue;
        }
        
        public void RemoveParamFloat(string eventName, string paramKey)
        {
            Dictionary<string, float> dic = GetParamDicFloat(eventName);
            dic.Remove(paramKey);
        }

        public float GetParamFloat(string eventName, string paramKey, float defaultValue = 0)
        {
            Dictionary<string, float> dic = GetParamDicFloat(eventName);
            if (dic.TryGetValue(paramKey, out float value))
            {
                return value;
            }
            return defaultValue;
        }

        #endregion

        #region int

        Dictionary<string, Dictionary<string, int>> _paramIntDic = new Dictionary<string, Dictionary<string, int>>();

        private Dictionary<string, int> GetParamDicInt(string eventName)
        {
            bool success = _paramIntDic.TryGetValue(eventName, out Dictionary<string, int> dic);
            if (success)
            {
                return dic;
            }
            dic = new Dictionary<string, int>();
            _paramIntDic.Add(eventName, dic);
            return dic;
        }
        
        public void SetParamInt(string eventName, string paramKey, int paramValue)
        {
            Dictionary<string, int> dic = GetParamDicInt(eventName);
            dic[paramKey] = paramValue;
        }
        
        public int GetParamInt(string eventName, string paramKey, int defaultValue = 0)
        {
            Dictionary<string, int> dic = GetParamDicInt(eventName);
            if (dic.TryGetValue(paramKey, out int value))
            {
                return value;
            }
            return defaultValue;
        }
        

        #endregion

        #region bool

        Dictionary<string, Dictionary<string, bool>> _paramBoolDic = new Dictionary<string, Dictionary<string, bool>>();
        
        private Dictionary<string, bool> GetParamDicBool(string eventName)
        {
            bool success = _paramBoolDic.TryGetValue(eventName, out Dictionary<string, bool> dic);
            if (success)
            {
                return dic;
            }
            dic = new Dictionary<string, bool>();
            _paramBoolDic.Add(eventName, dic);
            return dic;
        }
        
        public void SetParamBool(string eventName, string paramKey, bool paramValue)
        {
            Dictionary<string, bool> dic = GetParamDicBool(eventName);
            dic[paramKey] = paramValue;
        }
        
        public bool GetParamBool(string eventName, string paramKey, bool defaultValue = false)
        {
            Dictionary<string, bool> dic = GetParamDicBool(eventName);
            if (dic.TryGetValue(paramKey, out bool value))
            {
                return value;
            }
            return defaultValue;
        }
        

        #endregion
    }
}