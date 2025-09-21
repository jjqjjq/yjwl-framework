using JQCore.tLog;
using JQCore.tSingleton;
using JQCore.tPool.Loader;
using JQCore.tRes;
using UnityEngine;

namespace JQFramework.tMgr
{
    public class CommonResMgr : JQSingleton<CommonResMgr>
    {
        private AssetObjectLib _lib;
        private UrlPrefabLoader _defaultModelPrefabLoader;

        public CommonResMgr()
        {
        }

        //虽然不会调用到，但是写一下方便做排查
        public override void Dispose()
        {
        }

        public void SetAssetObjectLib(UrlPrefabLoader urlPrefabLoader)
        {
            _lib = urlPrefabLoader.prefab.GetComponent<AssetObjectLib>();
        }

        public void SetDefaultModelPrefabLoader(UrlPrefabLoader urlPrefabLoader)
        {
            _defaultModelPrefabLoader = urlPrefabLoader;
        }

        public UnityEngine.Object GetAsset(string assetName)
        {
            UnityEngine.Object asset = null;
            asset = _lib.GetAsset(assetName);
            if (asset == null)
            {
                JQLog.LogError("获取通用资源失败：" + assetName);
            }

            return asset;
        }

        public GameObject SpawnDefault()
        {
            return _defaultModelPrefabLoader.Spawn();
        }

        public void DespawnDefault(GameObject gameObject)
        {
            _defaultModelPrefabLoader.DeSpawn(gameObject, false);
        }

        private PrefabLoader getPrefabLoader(string assetName, int preloadAmount)
        {
            GameObject prefab = _lib.GetAsset(assetName) as GameObject;
            if (prefab == null)
            {
                JQLog.LogError($"Can not find CommonRes Asset: {assetName}");
                return null;
            }
            PrefabLoader prefabLoader = UrlPrefabLoaderUtil.getObjPrefab(assetName, prefab.transform, preloadAmount);
            return prefabLoader;
        }

        public GameObject Spawn(string assetName, int preloadAmount = 10)
        {
            PrefabLoader prefabLoader = getPrefabLoader(assetName, preloadAmount);
            return prefabLoader.Spawn();
        }

        public Transform SpawnTransform(string assetName, int preloadAmount = 10)
        {
            GameObject gameObject = Spawn(assetName, preloadAmount);
            return gameObject.transform;
        }


        public void DeSpawn(string assetName, GameObject gameObject, int preloadAmount = 10)
        {
            PrefabLoader prefabLoader = getPrefabLoader(assetName, preloadAmount);
            prefabLoader.DeSpawn(gameObject, false);
        }

        public void DeSpawnPerformance(string assetName, GameObject gameObject, bool handleActive = false, int preloadAmount = 10)
        {
            PrefabLoader prefabLoader = getPrefabLoader(assetName, preloadAmount);
            prefabLoader.DeSpawnPerformance(gameObject, handleActive);
        }
    }
}