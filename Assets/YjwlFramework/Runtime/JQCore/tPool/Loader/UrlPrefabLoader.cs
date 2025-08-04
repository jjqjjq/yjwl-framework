#if PERFORMANCE_LOG
using System.Diagnostics;
#endif
using System;
using JQCore.tLog;
using JQCore.tPool.Manager;
using JQCore.tYooAssets;
using UnityEngine;
using YooAsset;

namespace JQCore.tPool.Loader
{
    public class UrlPrefabLoader : PrefabLoader
    {
        public const string COMPLETE = "UrlPrefabLoaderComplete";

        // private IrfResource _resource;
        private YooAssetAssetInfo _assetInfo;
        private bool _isLoading;
#if PERFORMANCE_LOG
            Stopwatch _stopwatch = new Stopwatch();
#endif

        public UrlPrefabLoader(int poolTypeType, string url) : base(url, poolTypeType)
        {
            _assetInfo = YooAssetMgr.GetAsset(_key);
        }

        public override GameObject prefab
        {
            get
            {
                _assetInfo.addUse();
                return base.prefab;
            }
        }

        public string Url => _key;

        public override GameObject Spawn(Vector2 pos, bool handleActive = false)
        {
            _assetInfo.addUse();
            return base.Spawn(pos);
        }

        public override GameObject Spawn(bool handleActive = false)
        {
            _assetInfo.addUse();
            return base.Spawn();
        }

        public void usePrefabEnd()
        {
            _assetInfo.reduceUse();
        }

        public override void DeSpawnPerformance(GameObject inst, bool handleActive)
        {
            _assetInfo.reduceUse();
            base.DeSpawnPerformance(inst, handleActive);
        }

        public override void DeSpawn(GameObject inst, bool handleActive)
        {
            _assetInfo.reduceUse();
            base.DeSpawn(inst, handleActive);
        }

        public void Load()
        {
#if PERFORMANCE_LOG
            _stopwatch.Reset();
            _stopwatch.Start();
#endif
//            HyDebug.Log($"load prefab:"+_key);
            if (_prefab != null) //加载完毕
            {
                TriggerEvent(COMPLETE, this);
                return;
            }

            if (_isLoading) //正在加载
                return;

            _isLoading = true;
            // IrfResourcesMgr.LoadResource(_key, OnLoadFinish);
            YooAssetMgr.LoadAssetAsync<GameObject>(_key, OnLoadFinish);
        }

        private void OnLoadFinish(AssetOperationHandle handle)
        {
            // _resource = tmpResource;
            
            if (_assetInfo == null) JQLog.LogError("资源Prefab为空？ =》" + handle.AssetObject.name);
            
            
#if PERFORMANCE_LOG
            _stopwatch.Stop();
            //ManagedHeapUtil.printMemory();
            JQLog.LogWarning($"------------[asset load finish]{_key} 耗时：{_stopwatch.ElapsedMilliseconds}ms ");
#endif
            
            _assetInfo.SetAssetOperationHandle(handle);

            var prefabGo = handle.AssetObject as GameObject;
            _prefab = prefabGo.transform;
            if (needPool) _prefabPool = PrefabLoaderPoolManager.initPool(_poolType, _prefab);

            _isLoading = false;
            TriggerEvent(COMPLETE, this);
        }
//         private void OnLoadFinish(IrfResource resource)
//         {
// //            HyDebug.Log($"OnLoadFinish prefab:" + _key);
//             _resource = resource;
//             Object mainObj = _resource.MainObject;
//             if (mainObj == null)
//             {
//                 HyDebug.LogError("资源Prefab为空？ =》" + _key); 
//             }
//             else
//             {
//                 _prefab = (mainObj as GameObject).transform;
// #if UNITY_EDITOR
//                 ShaderManager.Instance.resetShader(_prefab.gameObject, resource.RelativePath);
// #endif
//                 if (needPool)
//                 {
//                     _prefabPool = PrefabLoaderPoolManager.initPool(_poolType, _prefab);
//                 }
//             }
//             _isLoading = false;
//             TriggerEvent(COMPLETE, this);
//         }

        public void ReleaseAllAssetsBundle()
        {
            // if (_resource != null && !_resource.DontRelease)
            // {
            //     _resource = null;
            // }
        }

        public override void Dispose()
        {
            // HyDebug.LogError($"Dispose UrlPrefabLoader:{_key}");
            base.Dispose();
#if PERFORMANCE_LOG
            _stopwatch = null;
#endif
            if (_assetInfo != null && _assetInfo.DontRelease) JQLog.LogError("关联资源是通用资源啊 不能Dispose");

            _assetInfo = null;
            _prefab = null;
            _isLoading = false;
        }

        public override bool isInUse()
        {
            if (_isLoading) return true;
            if (_assetInfo == null) return base.isInUse();

            if (_assetInfo.DontRelease) return true;

            return base.isInUse();
        }

        public void AddCompleteListener(Action<UrlPrefabLoader> handler)
        {
            if (handler == null) return;
            _irfEventDispatcher.AddEventListener(COMPLETE, handler);
        }

        public void RemoveCompleteListener(Action<UrlPrefabLoader> handler)
        {
            _irfEventDispatcher.RemoveEventListener(COMPLETE, handler);
        }
    }
}