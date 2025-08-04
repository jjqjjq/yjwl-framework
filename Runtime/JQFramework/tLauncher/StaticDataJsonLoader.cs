using System;
using System.Collections.Generic;
using JQCore.tCfg;
using JQCore.tLoader;
using JQCore.tLog;
using JQCore.tPool.Loader;
using JQCore.tUtil;
using JQFramework.tMgr;
// using ProtoBuf;

namespace JQFramework.tLauncher
{
    public class StaticDataJsonLoader:BaseLoader
    {
        private HashSet<Type> _configTypes;
        private Dictionary<string, Type> _url2TypeDic = new Dictionary<string, Type>();

        public StaticDataJsonLoader(HashSet<Type> configTypes) : base("配置数据")
        {
            _configTypes = configTypes;
            initTotal(_configTypes.Count);
        }

        public override void start()
        {
            base.start();
            _url2TypeDic.Clear();
            StaticDataMgr.Instance.clear();
            foreach (Type configType in _configTypes)
            {
                string cfgName = configType.Name.Replace("Category", "");
                string url = $"StreamingAssets/cfg/{cfgName}.txt";
                #if UNITY_EDITOR
                url = $"{AppConfig.CDN}/{url}";
                #endif
                _url2TypeDic[url] = configType;
                JQLog.Log($"LoadCfg:{url}");
                HttpUtil.HttpsGet(url, onLoadOneFinish);
            }
        }
        
        private void onLoadOneFinish(string json, string url)
        {
            // JQLog.LogError("onLoadOneFinish:"+url + " bytes:"+json.Length);
            Type type = _url2TypeDic[url];
            int totalSize = StaticDataMgr.Instance.AddOneType(type, json);
            finishOne();
            if (totalSize == _configTypes.Count)
            {
                AllLoadEnd();
            }
        }

        private void AllLoadEnd()
        {
            StaticDataMgr.Instance.LoadByJson();
            finishAll();
        }
        

    }
}