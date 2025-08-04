using System;
using JQCore.tPool.Manager;
using JQCore.tYooAssets;

namespace JQCore.tPool.Loader
{
    public static class AssetLoaderUtil
    {
        public static AssetLoader getAssetLoader(string url)
        {
            return AssetLoaderManager.getAssetLoader(url);
        }

        public static void LoadAsset(string url, Action<AssetLoader> onLoadAction, bool setDontRelease = false)
        {
            if (setDontRelease) YooAssetMgr.SetDontRelease(url);
            var assetLoader = getAssetLoader(url);
            assetLoader.AddCompleteListener(onLoadAction);
            assetLoader.Load();
        }

        public static void LoadScene(string url, Action<AssetLoader> onLoadAction)
        {
            var assetLoader = getAssetLoader(url);
            assetLoader.AddCompleteListener(onLoadAction);
            assetLoader.LoadScene();
        }


        public static void CancelLoadAsset(string url, Action<AssetLoader> onLoadAction)
        {
            var assetLoader = getAssetLoader(url);
            assetLoader.RemoveCompleteListener(onLoadAction);
        }

        public static void ReduceUse(string url)
        {
            var assetLoader = getAssetLoader(url);
            assetLoader.reduceUse();
        }
    }
}