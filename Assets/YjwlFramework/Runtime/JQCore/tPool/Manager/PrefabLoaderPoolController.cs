using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tPool.Loader;

namespace JQCore.tPool.Manager
{
    public class PrefabLoaderPoolController
    {
        private readonly Dictionary<string, PrefabLoader> _prefabLoaderDic = new();

        public PrefabLoaderPoolController(int type)
        {
            Type = type;
        }

        public int Type { get; }

        public void ClearAll()
        {
            foreach (var keyValuePair in _prefabLoaderDic)
            {
                var prefabLoader = keyValuePair.Value;
                if (!prefabLoader.isInUse()) prefabLoader.Dispose();
            }

            _prefabLoaderDic.Clear();
        }

        public void ClearAllNotInUse()
        {
            var preCount = _prefabLoaderDic.Count;
            var removeList = new List<string>();
            foreach (var keyValuePair in _prefabLoaderDic)
            {
                var prefabLoader = keyValuePair.Value;
                //还需要判断是否没人用了
                if (!prefabLoader.isInUse())
                {
                    removeList.Add(prefabLoader.Key);
                    prefabLoader.Dispose();
                }
            }

            for (var i = 0; i < removeList.Count; i++)
            {
                var url = removeList[i];
                _prefabLoaderDic.Remove(url);
            }
            //HyDebug.Log($"[Clear]  type:{_type}   preCount:{preCount} currCount:{_prefabLoaderDic.Count}");
        }


        public PrefabLoader GetPrefabLoader(string key)
        {
            PrefabLoader prefabLoader = null;
            _prefabLoaderDic.TryGetValue(key, out prefabLoader);
            return prefabLoader;
        }

        public void AddPrefabLoader(string key, PrefabLoader prefabLoader)
        {
            if (GetPrefabLoader(key) != null) JQLog.Log("重复的PrefabLaoder Key:" + key);
            //            HyDebug.Log("                           AddPrefabLoader:"+key);
            _prefabLoaderDic[key] = prefabLoader;
        }

        public void Clear(string key)
        {
            PrefabLoader prefabLoader = null;
            if (_prefabLoaderDic.TryGetValue(key, out prefabLoader))
                if (!prefabLoader.isInUse())
                {
                    prefabLoader.Dispose();
                    _prefabLoaderDic.Remove(key);
                }
        }
    }
}