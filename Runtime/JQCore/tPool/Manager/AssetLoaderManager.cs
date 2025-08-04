/*----------------------------------------------------------------
// 文件名：AssetLoaderManager.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/9/8 23:11:50
//----------------------------------------------------------------*/

using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tPool.Loader;
using UnityEngine.Pool;

namespace JQCore.tPool.Manager
{
    public static class AssetLoaderManager
    {
        private static ObjectPool<AssetLoader> _assetLoaderPool = new(CreateAssetLoader);
        private static Dictionary<string, AssetLoader> _assetLoaderDic = new();


        private static AssetLoader CreateAssetLoader()
        {
            return new AssetLoader();
        }

        public static void ClearAll()
        {
            _assetLoaderPool = new ObjectPool<AssetLoader>(CreateAssetLoader);
            _assetLoaderDic = new Dictionary<string, AssetLoader>();
        }


        public static void ClearAllUnUse()
        {
            JQLog.LogWarning("AssetLoaderManager ClearAllUnUse");
            var removeList = new List<string>();
            foreach (var keyValuePair in _assetLoaderDic)
            {
                var assetLoader = keyValuePair.Value;
                if (assetLoader.canClear())
                {
                    removeList.Add(keyValuePair.Key);
                    assetLoader.reset();
                    _assetLoaderPool.Release(assetLoader);
                }
            }

            for (var i = 0; i < removeList.Count; i++)
            {
                var url = removeList[i];
                _assetLoaderDic.Remove(url);
            }

            JQLog.Log("showAssetLoaderCount:" + _assetLoaderDic.Count);
        }

        public static AssetLoader getAssetLoader(string url)
        {
            AssetLoader assetLoader = null;
            _assetLoaderDic.TryGetValue(url, out assetLoader);
            if (assetLoader == null)
            {
                assetLoader = _assetLoaderPool.Get();
                assetLoader.SetUrl(url);
                _assetLoaderDic[url] = assetLoader;
            }

            return assetLoader;
        }
    }
}