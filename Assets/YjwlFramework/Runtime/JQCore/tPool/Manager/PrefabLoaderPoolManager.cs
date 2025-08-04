using System;
using JQCore.tLog;
using JQCore.tPool.Loader;
using JQCore.tPool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JQCore.tPool.Manager
{
    public class PrefabLoaderPoolManager
    {
        public static void init()
        {
            var gameObject = new GameObject("Pool");
            parentTrans = gameObject.transform;
            if (Application.isPlaying) Object.DontDestroyOnLoad(gameObject);
        }

        public static void createTypePool(string poolName, int poolType)
        {
            var spawnPool = PoolManager.Pools.Create(poolName);
            spawnPool.dontDestroyOnLoad = true;
            spawnPool.transform.parent = parentTrans;
            controllers[poolType] = new PrefabLoaderPoolController(poolType);
            poolNames[poolType] = poolName;
        }

        #region typeCtrl

        public static PrefabLoaderPoolController[] controllers = new PrefabLoaderPoolController[100];
        public static string[] poolNames = new string[100];
        private static Transform parentTrans;

        public static PrefabLoader GetPrefabLoader(int poolType, string key)
        {
            var controller = controllers[poolType];
            if (controller == null) JQLog.LogError("缺少池类型：" + poolType);
            return controller.GetPrefabLoader(key);
        }

        public static ObjPrefabLoader CreateObjPrefabLoader(string key, int poolType, Transform prefab, int preloadAmount = 10)
        {
            var prefabLoader = new ObjPrefabLoader(key, poolType, prefab, preloadAmount);
            AddPrefabLoader(poolType, key, prefabLoader);
            return prefabLoader;
        }

        public static UrlPrefabLoader CreateUrlPrefabLoader(int poolType, string url)
        {
            var prefabLoader = new UrlPrefabLoader(poolType, url);
            AddPrefabLoader(poolType, url, prefabLoader);
            return prefabLoader;
        }

        public static void AddPrefabLoader(int poolType, string key, PrefabLoader prefabLoader)
        {
            var controller = controllers[poolType];
            controller.AddPrefabLoader(key, prefabLoader);
        }

        public static void Clear(int poolType, string key)
        {
            var controller = controllers[poolType];
            controller.Clear(key);
        }

        //切场景清理
        public static void ClearAll()
        {
            for (var i = 0; i < controllers.Length; i++)
            {
                var controller = controllers[i];
                if (controller != null) controller.ClearAll();
            }

            controllers = new PrefabLoaderPoolController[100];
        }

        public static void ClearAllNotInUse()
        {
            for (var i = 0; i < controllers.Length; i++)
            {
                var controller = controllers[i];
                if (controller != null) controller.ClearAllNotInUse();
            }
        }

        #endregion

        #region static

        public static PrefabPool initPool(int poolType, Transform prefab, int preloadAmount = 1)
        {
            SpawnPool spawnPool = null;
            PrefabPool prefabPool = null;
            var poolName = poolNames[poolType];
            spawnPool = PoolManager.Pools[poolName];
            prefabPool = createPool(spawnPool, prefab, preloadAmount);
            return prefabPool;
        }


        private static Action<string> _action;

        public static void setNewAction(Action<string> action)
        {
            _action = action;
        }

        private static PrefabPool createPool(SpawnPool spawnPool, Transform prefab, int preloadAmount = 1, int cullAbove = 200)
        {
            var prefabPool = spawnPool.GetPrefabPool(prefab);
            if (prefabPool == null)
            {
                //API.loadCounter.addCount(LoadCounter.AI_INSTAN, preloadAmount);
                prefabPool = new PrefabPool(prefab);
                prefabPool.preloadAmount = preloadAmount; // This is the default so may be omitted
                prefabPool.cullDespawned = true;
                prefabPool.cullAbove = cullAbove;
                prefabPool.cullDelay = 1;
                prefabPool.limitInstances = false;
                prefabPool.limitAmount = 3;
                prefabPool.limitFIFO = true;
                prefabPool.setNewAction(_action);
                spawnPool.CreatePrefabPool(prefabPool);
            }

            return prefabPool;
        }

        #endregion
    }
}