using System.Collections.Generic;
using UnityEngine;

namespace JQFramework.tUtil
{
    public enum EmptyGoType
    {
        simple = 1,
    }
    
    public static class EmptyGameObjectPoolUtil
    {
        
        
        
        private static Dictionary<EmptyGoType, EmptyGameObjectPool> _poolDic = new Dictionary<EmptyGoType, EmptyGameObjectPool>();

        private static EmptyGameObjectPool getEmptyPool(EmptyGoType type)
        {
            EmptyGameObjectPool emptyGameObjectPool = null;
            if (!_poolDic.TryGetValue(type, out emptyGameObjectPool))
            {
                switch (type)
                {
                    case EmptyGoType.simple:
                        emptyGameObjectPool = new EmptyGameObjectPool(simpleBuildFun);
                        break;
                }
                _poolDic[type] = emptyGameObjectPool;
            }

            return emptyGameObjectPool;
        }

        public static GameObject SpawnGameObject(EmptyGoType type, string name)
        {
            EmptyGameObjectPool pool = getEmptyPool(type);
            if (pool != null)
            {
                return pool.SpawnGameObject(name);
            }

            return null;
        }
        
        public static void DespawnGameObject(EmptyGoType type, GameObject gameObject)
        {
            EmptyGameObjectPool pool = getEmptyPool(type);
            if (pool != null)
            {
                pool.DespawnGameObject(gameObject);
            }
        }

        private static GameObject simpleBuildFun(int index)
        {
            return new GameObject("GameObject_"+index);
        }

        public static void Clear()
        {
            _poolDic.Clear();
        }
    }
}