using JQCore.tLog;
using JQCore.tPool.Manager;
using UnityEngine;

namespace JQCore.tPool.Loader
{
    public class ObjPrefabLoader : PrefabLoader
    {
        private bool _isUsing;
        
        public ObjPrefabLoader(string key, int poolType, Transform prefab, int preloadAmount = 10) : base(key, poolType)
        {
            if (prefab == null) JQLog.LogErrorFormat("prefab 为空，无法构建Prefab池 key:{0}, poolType:{1}", key, poolType);
            _prefab = prefab;
            _prefabPool = PrefabLoaderPoolManager.initPool(_poolType, _prefab, preloadAmount);
            _isUsing = true;
        }

        public override bool isInUse()
        {
            return _isUsing;
        }

        public override void Dispose()
        {
            // HyDebug.LogError($"Dispose UrlPrefabLoader:{_key}");
            _isUsing = false;
            base.Dispose();
        }
    }
}