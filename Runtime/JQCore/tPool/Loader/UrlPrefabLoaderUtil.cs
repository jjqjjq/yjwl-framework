using System;
using JQCore.tLog;
using JQCore.tPool.Manager;
using JQCore.tYooAssets;
using UnityEngine;

namespace JQCore.tPool.Loader
{
    public static class UrlPrefabLoaderUtil
    {
        public static UrlPrefabLoader getPrefab(string url)
        {
            var loader = PrefabLoaderPoolManager.GetPrefabLoader(1, url);
            if (loader == null) loader = PrefabLoaderPoolManager.CreateUrlPrefabLoader(1, url);
            //YYDebug.LogError("创建模型对象池：" + url + " _type:" + poolType); 
            return loader as UrlPrefabLoader;
        }

        public static bool IsLoaded(string url)
        {
            var urlPrefabLoader = getPrefab(url);
            return urlPrefabLoader.isLoaded;
        }

        public static void LoadAsset(string url, Action<UrlPrefabLoader> onLoadAction, bool setDontRelease = false)
        {
            if (setDontRelease) YooAssetMgr.SetDontRelease(url);
            var urlPrefabLoader = getPrefab(url);
            urlPrefabLoader.AddCompleteListener(onLoadAction);
            urlPrefabLoader.Load();
        }

        public static void CancelLoadAsset(string url, Action<UrlPrefabLoader> onLoadAction)
        {
            var urlPrefabLoader = getPrefab(url);
            urlPrefabLoader.RemoveCompleteListener(onLoadAction);
        }

        public static void DeSpawn(string url, GameObject gameObject, bool handleActive = false)
        {
            var urlPrefabLoader = getPrefab(url);
            urlPrefabLoader.DeSpawn(gameObject, handleActive);
        }

        public static void DeSpawnPerformance(string url, GameObject gameObject)
        {
            var urlPrefabLoader = getPrefab(url);
            urlPrefabLoader.DeSpawnPerformance(gameObject, false);
        }

        public static PrefabLoader getObjPrefab(string key, Transform prefab, int preloadAmount)
        {
            prefab.name = key;
            var prefabLoader = PrefabLoaderPoolManager.GetPrefabLoader(1, key);
            if (prefabLoader == null) prefabLoader = PrefabLoaderPoolManager.CreateObjPrefabLoader(key, 1, prefab, preloadAmount);
            return prefabLoader;
        }
    }
}