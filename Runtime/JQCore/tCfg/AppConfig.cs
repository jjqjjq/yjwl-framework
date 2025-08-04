using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JQCore;
using JQCore.tCoroutine;
using JQCore.tJson;
using JQCore.tLog;
using JQCore.tUtil;
using UnityEngine;
using UnityEngine.Networking;
using WeChatWASM;

namespace JQCore.tCfg
{
    public enum SdkPlatform
    {
        none = 0, //空包
        taptap = 1, //taptap
    }

    public static class AppConfig
    {
        // private static JSONObject _cfgJson;
        private static Dictionary<string, string> _cfgDic;

        // private static JSONObject _extraCfgJson;

        private static string _cfgType;

        public static string CfgType
        {
            get { return _cfgType; }
        }

        //channel
        public static int channelId = 0;

        public static string GetString(string key)
        {
            _cfgDic.TryGetValue(key, out string result);
            return result;
        }

        public static void init(Action callBack, string cfgType)
        {
            JQLog.Log("CfgType:" + cfgType);
            _cfgType = cfgType;
            _cfgDic = new Dictionary<string, string>();
            JQCoroutineHandler.Start(LoadCfgEdit(callBack));
        }

        public static void init(Action callBack, Dictionary<string, string> cfgDic)
        {
            _cfgDic = cfgDic;
            JQLog.Log("初始化系统配置syscfg.json完成");
            callBack();
        }
        

        public static string routerInfoPath
        {
            get
            {
                string configPath = null;
                if (string.IsNullOrEmpty(_cfgType))
                {
                    configPath = CDN;
                }
                else
                {
                    configPath = $"Assets/{PathUtil.RES_FOLDER}/Build/routers/router-{_cfgType}.txt";
                }

                return configPath;
            }
        }


        private static IEnumerator LoadCfgEdit(Action callBack)
        {
            string configPath = Path.Combine(Application.dataPath, $"{PathUtil.RES_FOLDER}/Build/sysCfgs/syscfg-{_cfgType}.json");
            JQLog.Log("初始化系统配置syscfg.json:" + configPath);
            string result;
            if (configPath.Contains("://"))
            {
                using (var webRequest = UnityWebRequest.Get(configPath))
                {
                    yield return webRequest.SendWebRequest();
                    result = webRequest.downloadHandler.text;
                }
            }
            else
            {
                result = File.ReadAllText(configPath);
            }

            JSONObject jsonObject = new JSONObject(result);
            for (int i = 0; i < jsonObject.keys.Count; i++)
            {
                string key = jsonObject.keys[i];
                string value = jsonObject[key].str;
                _cfgDic[key] = value;
            }

            JQLog.Log("初始化系统配置syscfg.json完成");
            callBack();
        }

        #region 属性

        /// <summary>
        ///     是否热更代码
        /// </summary>
        public static bool HybridCLR => bool.Parse(_cfgDic["HybridCLR"]);

        /// <summary>
        ///     是否显示ReporterLog
        /// </summary>
        public static bool ReporterLog => bool.Parse(_cfgDic["ReporterLog"]);

        /// <summary>
        ///     是否是调试模式
        /// </summary>
        public static bool IsDebug => bool.Parse(_cfgDic["IsDebug"]);

        /// <summary>
        ///     是否是调试模式
        /// </summary>
        public static SdkPlatform SdkPlatform => (SdkPlatform)Enum.Parse(typeof(SdkPlatform), _cfgDic["SdkPlatform"]);

        public static bool IsRelease
        {
            get { return Branch == "release"; }
        }


        /// <summary>
        ///     Branch
        /// </summary>
        public static string Branch => _cfgDic["branch"];

        /// <summary>
        ///     CDN
        /// </summary>
        public static string CDN => _cfgDic["CDN"];

        /// <summary>
        ///     App版本号
        /// </summary>
        public static string AppVersion => _cfgDic["AppVersion"];

        /// <summary>
        ///     GameLog地址
        /// </summary>
        public static string GameLogUrl => _cfgDic["GameLogUrl"];

        #endregion

        // #region 额外属性
        // private static bool getExtraJson(string key, bool defaultVal)
        // {
        //     bool result = defaultVal;
        //     if (_extraCfgJson.HasField(key))
        //     {
        //         bool.TryParse(_extraCfgJson[key].str, out result);
        //     }
        //
        //     return result;
        // }
        //
        // #endregion
    }
}