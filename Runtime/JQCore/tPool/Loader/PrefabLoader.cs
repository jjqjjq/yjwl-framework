using System;
using JQCore;
using JQCore.tEvent;
using JQCore.tCfg;
using JQCore.tPool;
using UnityEngine;

namespace JQCore.tPool.Loader
{
    public abstract class PrefabLoader
    {
        private static readonly Vector3 farVector3 = new(-10000, 0, -10000);

        // protected LuaEventDispatcher _luaEventDispatcher = new LuaEventDispatcher();
        protected readonly int _poolType;
        protected JQEventDispatcher _irfEventDispatcher = new();
        protected string _key;
        protected Transform _prefab;
        protected PrefabPool _prefabPool;
        public bool needPool = true;


        public PrefabLoader(string key, int poolTypeType)
        {
            _key = key;
            _poolType = poolTypeType;
        }

        public bool isLoaded => _prefab != null;

        public virtual GameObject prefab => _prefab.gameObject;

        public string Key => _key;

        public virtual bool isInUse()
        {
            if (_prefabPool != null) return _prefabPool.spawned.Count > 0;
            return false;
        }

        public void AddPreloadAmount(int addVal)
        {
            _prefabPool.cullAbove += addVal;
            _prefabPool.preloadAmount += addVal;
            _prefabPool.PreloadInstances();
        }

        public virtual void Dispose()
        {
            //HyDebug.LogError("移除所有对象Dispose");
            if (_prefabPool != null)
            {
                _prefabPool.spawnPool.RemovePrefabPool(_prefabPool);
                _prefabPool.SelfDestruct();
                _prefabPool = null; //这里可能还没处理好
            }

            _irfEventDispatcher.EventDispose();
            // _luaEventDispatcher.EventDispose();
            _key = null;
            _prefab = null;
        }


        public void TriggerEvent(string eventType, UrlPrefabLoader t)
        {
            _irfEventDispatcher.TriggerEvent(eventType, t, true);
            // _luaEventDispatcher.TriggerEvent(eventType, t);
            // _luaEventDispatcher.EventDispose(eventType);
        }

        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            _irfEventDispatcher.AddEventListener(eventType, handler);
        }

        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            _irfEventDispatcher.RemoveEventListener(eventType, handler);
        }

        public virtual GameObject Spawn(Vector2 pos, bool handleActive = false)
        {
            if (_prefabPool == null) return null;
            var go = _prefabPool.SpawnInstance(pos, Quaternion.identity).gameObject;
            if (handleActive) go.SetActive(true);
            if (Sys.isEditor && AppConfig.IsDebug) go.name = go.name.Replace("(pool)", "");
            return go;
        }

        public virtual GameObject Spawn(bool handleActive = false)
        {
            if (_prefabPool == null) return null;
            var go = _prefabPool.SpawnInstance(Vector3.zero, Quaternion.identity).gameObject;

            if (handleActive) go.SetActive(true);
            if (Sys.isEditor && AppConfig.IsDebug) go.name = go.name.Replace("(pool)", "");
            return go;
        }

        public virtual void DeSpawnPerformance(GameObject inst, bool handleActive)
        {
            if (_prefabPool != null && inst != null)
            {
                if (handleActive) inst.SetActive(false);
                //                inst.transform.SetParent(_prefabPool.spawnPool.group);
                inst.transform.localPosition = farVector3;
                inst.transform.localScale = Vector3.one;
                inst.transform.localRotation = Quaternion.Euler(Vector3.zero);
                if (Sys.isEditor && AppConfig.IsDebug)
                {
                    var name = inst.gameObject.name;
                    if (!name.Contains("(pool)")) inst.gameObject.name = inst.gameObject.name + "(pool)";
                }

                _prefabPool.DespawnInstance(inst.transform);
            }
        }

        public virtual void DeSpawn(GameObject inst, bool handleActive)
        {
            if (_prefabPool != null && inst != null)
            {
                if (handleActive) inst.SetActive(false);
                inst.transform.SetParent(_prefabPool.spawnPool.group);
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localScale = Vector3.one;
                inst.transform.localRotation = Quaternion.Euler(Vector3.zero);
                _prefabPool.DespawnInstance(inst.transform);
            }
        }
    }
}