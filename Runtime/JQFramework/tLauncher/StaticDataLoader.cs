using System;
using System.Collections.Generic;
using ET;
using JQCore.tLoader;
using JQCore.tLog;
using JQCore.tPool.Loader;
using JQFramework.tMgr;

// using ProtoBuf;

namespace JQFramework.tLauncher
{
    public class StaticDataLoader : BaseLoader
    {
        private HashSet<Type> _configTypes;
        private Dictionary<string, Type> _url2TypeDic = new Dictionary<string, Type>();

        public StaticDataLoader(HashSet<Type> configTypes) : base("配置数据")
        {
            _configTypes = configTypes;
            initTotal(_configTypes.Count);
        }

        public void addConfigType(Type[] configTypes)
        {
            foreach (Type configType in configTypes)
            {
                _configTypes.Add(configType);
            }

            initTotal(_configTypes.Count);
        }

        public override void start()
        {
            base.start();
            _url2TypeDic.Clear();
            StaticDataMgr.Instance.clear();
            foreach (Type configType in _configTypes)
            {
                string configFilePath = $"Data/Static/{configType.Name}.bytes";
                // JQLog.LogError(configFilePath);
                _url2TypeDic[configFilePath] = configType;
                AssetLoaderUtil.LoadAsset(configFilePath, onLoadOneFinish);
            }
        }

        private void onLoadOneFinish(AssetLoader loader)
        {
            byte[] bytes = loader.getText().bytes;
            // JQLog.LogError("onLoadOneFinish:"+loader.Url + " bytes:"+bytes.Length);
            Type type = _url2TypeDic[loader.Url];
            int totalSize = StaticDataMgr.Instance.AddOneType(type, bytes);
            loader.reduceUse();
            finishOne();
            if (totalSize == _configTypes.Count)
            {
                AllLoadEnd();
            }
        }


        private void AllLoadEnd()
        {
            StaticDataMgr.Instance.Load();
            finishAll();
        }
    }
}