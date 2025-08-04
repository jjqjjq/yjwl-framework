using System.Collections.Generic;
using JQCore.tSingleton;
using UnityEngine;

namespace JQCore.tMgr
{
    /// <summary>
    ///     本地变量管理器：PlayerPrefs的封装
    /// </summary>
    public class LocalVarManager:JQSingleton<LocalVarManager>
    {
        private readonly Dictionary<string, int> _cacheInt = new();
        private readonly Dictionary<string, string> _cacheString = new();
        private readonly Dictionary<string, bool> _cacheBool = new();
        private readonly Dictionary<string, float> _cacheFloat = new();
        private long _baseKey; //用于区分不同的角色、账号等

        public override void Dispose()
        {
            _cacheInt.Clear();
            _cacheString.Clear();
            _cacheBool.Clear();
            _cacheFloat.Clear();
        }

        public static void setBaseKey(long baseKey)
        {
            Instance.setBaseKeyy(baseKey);
        }
        
        public void setBaseKeyy(long baseKey)
        {
            _baseKey = baseKey;
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        #region Int

        public void SetGlobalIntt(string key, int value)
        {
            _cacheInt[key] = value;
            PlayerPrefs.SetInt(key, value);
        }

        public void SetIntt(string key, int value)
        {
            key = _baseKey + key;
            SetGlobalIntt(key, value);
        }

        public int GetGlobalIntt(string key, int defaultValue = 0)
        {
            if (_cacheInt.TryGetValue(key, out var i))
                return i;
            var result = PlayerPrefs.GetInt(key, defaultValue);
            _cacheInt.Add(key, result);
            return result;
        }

        public int GetIntt(string key, int defaultValue = 0)
        {
            key = _baseKey + key;
            return GetGlobalIntt(key, defaultValue);
        }
        
        public static void SetGlobalInt(string key, int value)
        {
            Instance.SetGlobalIntt(key, value);
        }

        public static void SetInt(string key, int value)
        {
            Instance.SetIntt(key, value);
        }

        public static int GetGlobalInt(string key, int defaultValue = 0)
        {
            return Instance.GetGlobalIntt(key, defaultValue);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return Instance.GetIntt(key, defaultValue);
        }

        #endregion

        #region String

        public void SetGlobalStringg(string key, string value)
        {
            _cacheString[key] = value;
            PlayerPrefs.SetString(key, value);
        }

        public void SetStringg(string key, string value)
        {
            key = _baseKey + key;
            SetGlobalStringg(key, value);
        }

        public string GetGlobalStringg(string key, string defaultValue = "")
        {
            if (_cacheString.TryGetValue(key, out var s))
                return s;
            var result = PlayerPrefs.GetString(key, defaultValue);
            _cacheString.Add(key, result);
            return result;
        }

        public string GetStringg(string key, string defaultValue = "")
        {
            key = _baseKey + key;
            return GetGlobalStringg(key, defaultValue);
        }

        
        public static void SetGlobalString(string key, string value)
        {
            Instance.SetGlobalStringg(key, value);
        }

        public static void SetString(string key, string value)
        {
            Instance.SetStringg(key, value);
        }

        public static string GetGlobalString(string key, string defaultValue = "")
        {
            return Instance.GetGlobalStringg(key, defaultValue);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return Instance.GetStringg(key, defaultValue);
        }
        #endregion

        #region Bool

        public void SetGlobalBooll(string key, bool value)
        {
            _cacheBool[key] = value;
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public void SetBooll(string key, bool value)
        {
            key = _baseKey + key;
            SetGlobalBooll(key, value);
        }

        public bool GetGlobalBooll(string key, bool defaultValue = false)
        {
            if (_cacheBool.TryGetValue(key, out var b))
                return b;
            var result = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
            _cacheBool.Add(key, result == 1);
            return result == 1;
        }

        public bool GetBooll(string key, bool defaultValue = false)
        {
            key = _baseKey + key;
            return GetGlobalBooll(key, defaultValue);
        }

        public static void SetGlobalBool(string key, bool value)
        {
            Instance.SetGlobalBooll(key, value);
        }

        public static void SetBool(string key, bool value)
        {
            Instance.SetBooll(key, value);
        }

        public static bool GetGlobalBool(string key, bool defaultValue = false)
        {
            return Instance.GetGlobalBooll(key, defaultValue);   
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return Instance.GetBooll(key, defaultValue);
        }
        #endregion

        #region Float

        public void SetGlobalFloatt(string key, float value)
        {
            _cacheFloat[key] = value;
            PlayerPrefs.SetFloat(key, value);
        }

        public void SetFloatt(string key, float value)
        {
            key = _baseKey + key;
            SetGlobalFloatt(key, value);
        }

        public float GetGlobalFloatt(string key, float defaultValue = 0.0f)
        {
            if (_cacheFloat.TryGetValue(key, out var f))
                return f;
            var result = PlayerPrefs.GetFloat(key, defaultValue);
            _cacheFloat.Add(key, result);
            return result;
        }

        public float GetFloatt(string key, float defaultValue = 0.0f)
        {
            key = _baseKey + key;
            return GetGlobalFloatt(key, defaultValue);
        }
        
        public static void SetGlobalFloat(string key, float value)
        {
            Instance.SetGlobalFloatt(key, value);
        }

        public static void SetFloat(string key, float value)
        {
            Instance.SetFloatt(key, value);
        }
        
        public static float GetGlobalFloat(string key, float defaultValue = 0.0f)
        {
            return Instance.GetGlobalFloatt(key, defaultValue);
        }

        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            return Instance.GetFloatt(key, defaultValue);
        }


        #endregion
    }
}